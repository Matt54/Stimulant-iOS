using System;
using System.Globalization;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Generic;

namespace Stimulant
{
    public class MidiModulation : INotifyPropertyChanged
    {
        // flag that allows modulation to occur if true
        public bool IsRunning { get; set; }

        // trigger for the modulation to take a step
        private bool _FireModulation;
        public bool FireModulation
        {
            get { return _FireModulation; }
            set { _FireModulation = value; OnPropertyChanged("FireModulation"); }
        }

        // controls the step's direction and amount (or simply just selects the next cc value)
        private int _PatternNumber;
        public int PatternNumber
        {
            get { return _PatternNumber; }
            set { _PatternNumber = value; OnPropertyChanged("PatternNumber"); }
        }

        // description of the pattern for the UI to readout
        private string _PatternString;
        public string PatternString
        {
            get { return _PatternString; }
            set { _PatternString = value; OnPropertyChanged("PatternString"); }
        }

        // cc value sent in the MIDI continuous controller message (0-127)
        public int CurrentCC { get; set; }

        // cc number used in the MIDI continuous controller message (it can be changed - see CCon)
        private int _CCNumber;
        public int CCNumber
        {
            get { return _CCNumber; }
            set { _CCNumber = value; OnPropertyChanged("CCNumber"); }
        }

        // flag to allow the cc number to be adjusted (CCNumber)
        private bool _CCOn;
        public bool CCOn
        {
            get { return _CCOn; }
            set { _CCOn = value; OnPropertyChanged("CCOn"); }
        }

        // beats per minute used in time mode
        private int _BPM;
        public int BPM
        {
            get { return _BPM; }
            set { _BPM = value; OnPropertyChanged("BPM"); }
        }

        // used to record amount of time between tap presses to set BPM
        Stopwatch bpmTapClock = new Stopwatch();

        // stores a window of beats per minute taps so we can average them
        List<int> bpmTaps = new List<int>();

        // flag to allow the BPM number to be adjusted (BPM)
        private bool _BPMOn;
        public bool BPMOn
        {
            get { return _BPMOn; }
            set { _BPMOn = value; OnPropertyChanged("BPMOn"); }
        }

        // values control how large the value range of the MIDI cc message is (default: min 0 - max 127)
        public int Minimum { get; set; }
        public int Maximum { get; set; }

        // sets the stepsize and stepcomma based on the rate of the modulation
        public int RateCatch { get; set; }

        // values control influences how large of a step to take
        public int StepSize { get; set; }
        public int StepComma { get; set; }

        // values influence how often steps are taken
        public int ClockCount { get; set; }
        public int ClockCutoff { get; set; }

        // Is this unused?
        public int RateRemember { get; set; }

        // previous value and other bool helpers for pattern purposes
        public int LastCC { get; set; }
        public bool EveryOther { get; set; }
        public bool OppositeHelper { get; set; }

        // flag to reverse pattern direction
        private bool _Opposite;
        public bool Opposite
        {
            get { return _Opposite; }
            set { _Opposite = value; OnPropertyChanged("Opposite"); }
        }

        // random number (if created each time a random number is required it results in less "random" of numbers)
        private Random random = new Random();

        // Currently there are only 2 modes (1 = MIDI mode, 2 = time/frequency mode, 3 = time/bpm mode)
        // MIDI mode uses the external MIDI clock for its pattern movements, time mode uses a built-in clock
        private int _ModeNumber;
        public int ModeNumber
        {
            get { return _ModeNumber; }
            set { _ModeNumber = value; OnPropertyChanged("ModeNumber"); }
        }

        // flag for the "auto mode" which just randomizes the modulation settings at a certain frequency/# of MIDI clock pulses
        private bool _IsAuto;
        public bool IsAuto
        {
            get { return _IsAuto; }
            set { _IsAuto = value; OnPropertyChanged("IsAuto"); }
        }

        // vales control the frequency of the modulation randomization
        public int AutoCounter { get; set; }
        public int AutoCutoff { get; set; }

        // trigger for the modulation to randomize its settings
        public bool IsRandomRoll { get; set; }

        // flag to allow the "auto mode" to have its frequency or # of MIDI clock pulses required before randomization to be adjusted
        private bool _SettingsOn;
        public bool SettingsOn
        {
            get { return _SettingsOn; }
            set { _SettingsOn = value; OnPropertyChanged("SettingsOn"); }
        }

        // (AR = auto range) flag to allow the minimum and maximum values to be set in auto mode
        private bool _IsAR;
        public bool IsAR
        {
            get { return _IsAR; }
            set { _IsAR = value; OnPropertyChanged("IsAR"); }
        }

        // trigger only prevents modulation except when at least one note is on
        private bool _IsTriggerOnly;
        public bool IsTriggerOnly
        {
            get { return _IsTriggerOnly; }
            set { _IsTriggerOnly = value; OnPropertyChanged("IsTriggerOnly"); }
        }

        // restarts modulation at the starting location (only in trigger only mode)
        private bool _IsRestartEachNote;
        public bool IsRestartEachNote
        {
            get { return _IsRestartEachNote; }
            set { _IsRestartEachNote = value; OnPropertyChanged("IsRestartEachNote"); }
        }

        // value the modulation restarts at in trigger only mode with the restart each note setting on
        public int StartingLocation { get; set; }

        // is there at least one note on?
        public bool IsNoteOn { get; set; }

        // number of notes currently pressed
        public int NumOfNotesOn { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }


        // Class Constructor
        public MidiModulation()
        {
            // Default Values for New Object of Class
            PatternNumber = 1;
            ModeNumber = 0;
            ClockCount = 0;
            ClockCutoff = 1;
            FireModulation = false;
            IsRunning = false;
            SettingsOn = false;
            CurrentCC = 0;
            LastCC = 0;
            StepSize = 2;
            Maximum = 127;
            Minimum = 0;
            CCNumber = 0;
            Opposite = false;
            OppositeHelper = false;
            IsRandomRoll = false;
            AutoCounter = 0;
            AutoCutoff = 10;
            IsAR = false;
            BPM = 120;
        }


        // takes in a value from a segmented control  to set the pattern number and relays back a description of the pattern
        public string UpdatePattern(nint patternIndex)
        {
            string labelText;
            switch (patternIndex)
            {
                case 0:
                    labelText = "Pattern 1: Up && Down";
                    PatternNumber = 1;
                    break;
                case 1:
                    labelText = "Pattern 2: Up || Down";
                    PatternNumber = 2;
                    break;
                case 2:
                    labelText = "Pattern 3: Fwd 2 Back 1";
                    PatternNumber = 3;
                    break;
                case 3:
                    labelText = "Pattern 4: Crisscross";
                    PatternNumber = 4;
                    break;
                case 4:
                    labelText = "Pattern 5: Min Jumper";
                    PatternNumber = 5;
                    break;
                case 5:
                    labelText = "Pattern 6:  Max Jumper";
                    PatternNumber = 6;
                    break;
                case 6:
                    labelText = "Pattern 7:  Min && Max";
                    PatternNumber = 7;
                    break;
                case 7:
                    labelText = "Pattern 8:  Random #'s";
                    PatternNumber = 8;
                    break;
                default:
                    labelText = "error";
                    break;
            }
            return labelText;
        }


        // used in MIDI clock mode to limit the rate of the modulation
        public void ClockCounter()
        {
            if (IsRunning)
            {
                // auto mode flag during midi mode - random modulation settings occur at some clock rate
                if (IsAuto)
                {
                    // AutoCutoff can be adjusted with settings button on and the rate slider
                    // RandomRoll is disabled when settings are on
                    if (!SettingsOn)
                    {
                        AutoCounter++;
                        if (AutoCounter > AutoCutoff)
                        {
                            AutoCounter = 0;
                            PatternString = RandomRoll();
                        }

                    }
                }

                // when the rate changes the ClockCutoff is sometimes adjusted. We need to reset the ClockCount if this condition occurs.
                if (ClockCount > ClockCutoff)
                {
                    ClockCount = 0;
                }

                // the actual counting
                ClockCount++;

                // tell modulation to take a step if clock count is at the cutoff
                if (ClockCount == ClockCutoff)
                {
                    FireModulation = true;
                }
            }
        }


        // UpdateValue stores the different available patterns
        // it steps the CC value to the next appropriate value based on the pattern number
        // lots of arithmetic here - only a short summary will be provided for each pattern
        public void UpdateValue()
        {
            // =========================================Program #1===========================================
            // "Pattern 1: Up && Down" - steps up until maximum then step down until minimum
            if (PatternNumber == 1)
            {
                if (Opposite == false)
                {
                    if ((CurrentCC < Maximum) && (CurrentCC >= LastCC))
                    {
                        LastCC = CurrentCC;
                        CurrentCC += StepSize;
                        if (CurrentCC >= Maximum)
                        {
                            CurrentCC = Maximum;
                        }
                    }
                    else if ((CurrentCC < Maximum) && (CurrentCC <= LastCC) && (CurrentCC > Minimum))
                    {
                        LastCC = CurrentCC;
                        CurrentCC -= StepSize;
                        if (CurrentCC < Minimum)
                        {
                            LastCC = Minimum;
                            CurrentCC = Minimum;
                        }
                    }
                    else if (CurrentCC >= Maximum)
                    {

                        if (LastCC == Maximum)
                        {
                            LastCC = CurrentCC;
                            CurrentCC -= StepSize;
                        }
                        else
                        {
                            LastCC = CurrentCC;
                            CurrentCC = Maximum;
                        }

                    }
                    else if (CurrentCC <= Minimum)
                    {
                        if (LastCC == Minimum)
                        {
                            LastCC = CurrentCC;
                            CurrentCC += StepSize;
                        }
                        else
                        {
                            LastCC = CurrentCC;
                            CurrentCC = Minimum;
                        }
                    }
                    if (OppositeHelper == false)
                    {
                        OppositeHelper = true;
                    }
                }

                else
                {
                    if ((CurrentCC < Maximum) && (CurrentCC < LastCC))
                    {
                        LastCC = CurrentCC;
                        if (OppositeHelper)
                        {
                            OppositeHelper = false;
                            CurrentCC -= StepSize;
                        }
                        CurrentCC += StepSize;
                        if (CurrentCC >= Maximum)
                        {
                            CurrentCC = Maximum;
                        }
                    }
                    else if ((CurrentCC < Maximum) && (CurrentCC >= LastCC) && (CurrentCC > Minimum))
                    {
                        LastCC = CurrentCC;
                        CurrentCC -= StepSize;
                        if (CurrentCC < Minimum)
                        {
                            LastCC = Minimum;
                            CurrentCC = Minimum;
                        }
                    }
                    else if (CurrentCC >= Maximum)
                    {
                        LastCC = CurrentCC;
                        CurrentCC -= StepSize;
                    }
                    else if (CurrentCC <= Minimum)
                    {
                        LastCC = CurrentCC;
                    }
                }
            }
            // ==============================================================================================

            // =========================================Program #2===========================================
            // "Pattern 2: Up || Down" - steps up until maximum then restarts at minimum or does the opposite
            if (PatternNumber == 2)
            {
                if (Opposite == false)
                {
                    if (CurrentCC < Maximum)
                    {
                        CurrentCC += StepSize;
                        if (CurrentCC > Maximum)
                        {
                            CurrentCC = Minimum;
                        }
                    }
                    else
                    {
                        CurrentCC = Minimum;
                    }
                }
                else
                {
                    if (CurrentCC > Minimum)
                    {
                        CurrentCC -= StepSize;
                        if (CurrentCC < Minimum)
                        {
                            CurrentCC = Maximum;
                        }
                    }
                    else
                    {
                        CurrentCC = Maximum;
                    }
                }
            }
            // ==============================================================================================

            // =========================================Program #3===========================================
            // "Pattern 3: Fwd 2 Back 1" - steps 2x forward then x backwards (direction can be reversed)
            if (PatternNumber == 3)
            {
                if (EveryOther)
                {
                    if (Opposite == false)
                    {
                        CurrentCC += StepSize * 2;
                    }
                    else
                    {
                        CurrentCC -= StepSize * 2;
                    }
                    EveryOther = false;
                }
                else
                {
                    EveryOther = true;
                    if (Opposite == false)
                    {
                        CurrentCC -= StepSize;
                    }
                    if (Opposite)
                    {
                        CurrentCC += StepSize;
                    }
                }
                if (Opposite == false)
                {
                    if (CurrentCC >= Maximum)
                    {
                        CurrentCC = Minimum;
                        LastCC = Minimum;
                    }
                }
                else
                {
                    if (CurrentCC <= Minimum)
                    {
                        CurrentCC = Maximum;
                        LastCC = Maximum;
                    }
                }
            }
            // ==============================================================================================

            // =========================================Program #4===========================================
            // "Pattern 4: Crisscross" - back and forth between up/down patterns moving in opposite direction
            if (PatternNumber == 4)
            {
                if (EveryOther)
                {
                    EveryOther = false;
                    CurrentCC = LastCC + StepSize;
                }
                else
                {
                    EveryOther = true;
                    LastCC = CurrentCC;
                    CurrentCC = Maximum - CurrentCC;
                }
                if (CurrentCC > Maximum)
                {
                    CurrentCC = Minimum;
                }
                if (CurrentCC < Minimum)
                {
                    CurrentCC = Maximum;
                }
            }
            // ==============================================================================================

            // =========================================Program #5===========================================
            // "Pattern 5: Min Jumper" - back and forth between minimum and an upward or downward pattern
            if (PatternNumber == 5)
            {
                if (EveryOther)
                {
                    LastCC = CurrentCC;
                    EveryOther = false;
                    CurrentCC = Minimum;
                }
                else
                {
                    EveryOther = true;
                    if (Opposite == false)
                    {
                        CurrentCC = LastCC + StepSize;
                    }
                    if (Opposite)
                    {
                        CurrentCC = LastCC - StepSize;
                    }
                }

                if (CurrentCC > Maximum)
                {
                    CurrentCC = Minimum;
                    LastCC = Minimum;
                }
                if (CurrentCC < Minimum)
                {
                    CurrentCC = Maximum;
                    LastCC = Maximum;
                }
            }
            // ==============================================================================================

            // =========================================Program #6===========================================
            // "Pattern 6:  Max Jumper" - back and forth between maximum and an upward or downward pattern
            if (PatternNumber == 6)
            {
                if (EveryOther)
                {
                    LastCC = CurrentCC;
                    EveryOther = false;
                    CurrentCC = Maximum;
                }
                else
                {
                    EveryOther = true;
                    if (Opposite == false)
                    {
                        CurrentCC = LastCC - StepSize;
                    }
                    if (Opposite)
                    {
                        CurrentCC = LastCC + StepSize;
                    }
                }
                if (CurrentCC <= Minimum)
                {
                    CurrentCC = Maximum;
                    LastCC = Maximum;
                }
                if ((Opposite) && (EveryOther))
                {
                    if (CurrentCC >= Maximum)
                    {
                        CurrentCC = Minimum;
                        LastCC = Minimum;
                    }
                }
            }
            // ==============================================================================================

            // =========================================Program #7===========================================
            // "Pattern 7:  Min && Max" - back and forth between minimum and maximum value
            if (PatternNumber == 7)
            {
                if (CurrentCC < Maximum)
                {
                    CurrentCC = Maximum;
                }
                else if (CurrentCC >= Maximum)
                {
                    CurrentCC = Minimum;
                }
            }
            // ==============================================================================================

            // =========================================Program #8===========================================
            // "Pattern 8:  Random #'s" - random values between minimum and maximum
            if (PatternNumber == 8)
            {
                CurrentCC = RandomNumber(Minimum, Maximum);
            }
            // ==============================================================================================
        }


        // toggles if the pattern is reversed or not
        public void ReversePattern()
        {
            if (Opposite)
            {
                Opposite = false;
            }
            else
            {
                Opposite = true;
            }
        }


        // Step Size Setter is used to determine how much the current cc value should change in order to adhere to the overall timing concept while in midi mode.
        // To put this simply, midi clock signals come in at 24 pulses per quarter note. In order to achieve the best resolution possible with our
        // modulation, we need to make use of all of these pulses. In order to reconcile 24 pulses with a 128 value range for the midi cc (0-127), we sometimes
        // have to make little adjustments to the step size (hence the StepComma) to make the overall movement through the pattern range hit at the designated
        // rate.
        public void StepSizeSetter()
        {

            bool stepBack = false, stepForward = false;
            switch (RateCatch)
            {
                case 1:
                case 3:
                case 5:
                    StepSize = 1;
                    stepForward = true;
                    break;
                case 2:
                case 4:
                    StepSize = 1;
                    break;
                case 6:
                    StepSize = 2;
                    break;
                case 7:
                    StepSize = 3;
                    stepBack = true;
                    break;
                case 8:
                    StepSize = 4;
                    break;
                case 9:
                    StepSize = 5;
                    stepForward = true;
                    break;
                case 10:
                    StepSize = 8;
                    break;
                case 11:
                    StepSize = 11;
                    stepBack = true;
                    break;
                case 12:
                    StepSize = 16;
                    break;
                case 13:
                    StepSize = 21;
                    stepForward = true;
                    break;
                case 14:
                    StepSize = 32;
                    break;
                case 15:
                    StepSize = 42;
                    stepBack = true;
                    break;
                case 16:
                    StepSize = 64;
                    break;
            }
            if (stepBack)
            {
                if (StepComma == 2)
                {
                    StepSize--;
                }
            }
            if (stepForward)
            {
                if (StepComma == 2)
                {
                    StepSize++;
                }
            }
        }


        // determines StepSize from a sliderValue and PatternNumber
        public void TimeSet(float sliderValue)
        {
            if (PatternNumber > 6)
            {
                StepSize = 1;
            }
            else
            {
                if (sliderValue > 125)
                {
                    StepSize = 32;
                }
                else if (sliderValue > 118)
                {
                    StepSize = 16;
                }
                else if (sliderValue > 107)
                {
                    StepSize = 8;
                }
                else if (sliderValue > 92)
                {
                    StepSize = 4;
                }
                else if (sliderValue > 74)
                {
                    StepSize = 2;
                }
                else
                {
                    StepSize = 1;
                }
            }
        }


        // converts the time interval being used for the timer for triggering the modulation steps into a frequency value for display purposes
        public string TimeIntervalToFrequency(float timeInterval)
        {
            string Frequency;
            int divFactor;
            switch (PatternNumber)
            {
                case 1:
                    divFactor = 256;
                    break;
                case 2:
                    divFactor = 128;
                    break;
                case 3:
                    divFactor = 256;
                    break;
                case 4:
                    divFactor = 256;
                    break;
                case 5:
                    divFactor = 256;
                    break;
                case 6:
                    divFactor = 256;
                    break;
                case 7:
                    divFactor = 2;
                    break;
                case 8:
                    divFactor = 1;
                    break;
                default:
                    divFactor = 2;
                    break;
            }
            Frequency = "Frequency: " + ((((double)StepSize) * 64 / divFactor) * Math.Round((1 / (timeInterval * (128 / 2) / 1000)), 3)).ToString("0.000", CultureInfo.InvariantCulture) + " Hz";
            return Frequency;
        }


        // changes the cc number by the inputed value but remaining in a 0-127 range (it swings around when min/max is surpassed)
        public void UpdateCCNumber(int val)
        {
            var max = 127;
            var min = 0;
            if (CCNumber + val >= min && CCNumber + val <= max)
            {
                CCNumber += val;
            }
            else if (CCNumber + val > max)
            {
                CCNumber += val - max - 1;
            }
            else
            {
                CCNumber += val + max + 1;
            }
        }

        // changes the bpm by the inputted value but only as large as the max and and small as the min (does not swing around)
        public void UpdateBPM(int val)
        {
            var max = 150;
            var min = 50;
            if (BPM + val >= min && BPM + val <= max)
            {
                BPM += val;
            }
            else if (BPM + val > max)
            {
                BPM = max;
            }
            else
            {
                BPM = min;
            }
        }


        // enables or disables auto mode
        public void AutoToggle()
        {
            if (IsAuto)
            {
                IsAuto = false;
            }
            else
            {
                IsAuto = true;
            }
        }

        // enables or disables trigger mode
        public void TriggerToggle()
        {
            if (IsTriggerOnly)
            {
                IsTriggerOnly = false;
            }
            else
            {
                IsTriggerOnly = true;
            }
        }


        // enables or disables auto range
        public void ARToggle()
        {
            if (IsAR)
            {
                IsAR = false;
            }
            else
            {
                IsAR = true;
            }
        }


        // randomizes modulation settings (called in auto mode)
        public string RandomRoll()
        {
            IsRandomRoll = true;
            string newlabel = "thisString";
            newlabel = UpdatePattern((nint)RandomNumber(0, 8));
            return newlabel;
        }


        // selects a random number
        public int RandomNumber(int min, int max)
        {
            return random.Next(min, max);
        }


        // enables or disables the ability to adjust the cc number
        public void CCToggle()
        {
            // Need this because they use the same GUI region
            if (BPMOn)
            {
                BPMToggle();
            }

            if (CCOn)
            {
                CCOn = false;
            }
            else
            {
                CCOn = true;
            }
        }

        public void BPMToggle()
        {
            // Need this because they use the same GUI region
            if (CCOn)
            {
                CCToggle();
            }

            if (BPMOn)
            {
                BPMOn = false;
            }
            else
            {
                BPMOn = true;
            }
        }

        public void BPMTap()
        {

            // BPM Tap algorithm here
            // 1 beat = 1 quarter note
            // 1 minute = 60 seconds = 60,000 milliseconds
            // Quarter note duration (ms) = 60,000 / bpm

            if (!bpmTapClock.IsRunning)
            {
                bpmTapClock.Start();
            }
            else
            {
                if (bpmTapClock.ElapsedMilliseconds < 400 || bpmTapClock.ElapsedMilliseconds > 1200)
                {
                    bpmTapClock.Restart();
                    bpmTaps.Clear();
                }
                else
                {
                    AverageBPM((double)bpmTapClock.ElapsedMilliseconds);
                    bpmTapClock.Restart();
                }
            }
        }

        private void AverageBPM(double elapsedTime)
        {
            int sumBPM = 0;
            int maxAverageWindow = 4; // max number of taps counted
            int deviationTolerance = 20; // max deviation in bpm before we considered it a new window

            int inputBPM = (int)Math.Round(60000 / elapsedTime);

            if (Math.Abs(BPM - inputBPM) > deviationTolerance)
            {
                bpmTaps.Clear();
            }

            if (bpmTaps.Count > maxAverageWindow)
            {
                bpmTaps.RemoveAt(0); // remove first count from list if above max window
            }

            bpmTaps.Add(inputBPM);

            foreach (int bpmTap in bpmTaps)
            {
                sumBPM += bpmTap;
            }

            BPM = sumBPM / bpmTaps.Count;
        }

        // switched between time based modes (bpm vs frequency)
        public void ClockToggle()
        {
            ModeNumber = 3;

            /*
            if (ModeNumber == 3)
            {
                ModeNumber = 2;
            }
            else
            {
                ModeNumber = 3;
            }
            */
        }

        // enables or disables auto range
        public void RestartToggle()
        {
            if (IsRestartEachNote)
            {
                IsRestartEachNote = false;
            }
            else
            {
                IsRestartEachNote = true;
            }
        }


        // enables or disables the ability to adjust the auto mode frequency
        public void SettingsToggle()
        {
            /*
            if (CCOn)
            {
                CCToggle();
            }
			*/

            // Instead we need (if starting location adjust is on)

            if (SettingsOn)
            {
                SettingsOn = false;
            }
            else
            {
                SettingsOn = true;
            }
        }


    }
}

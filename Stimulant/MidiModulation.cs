using System;
using System.Globalization;
using System.ComponentModel;

namespace Stimulant
{
    public class MidiModulation : INotifyPropertyChanged
    {

        private Random random = new Random();

        public bool IsRunning { get; set; }
        public bool IsRandomRoll { get; set; }

        private bool _IsAuto;
        public bool IsAuto
        {
            get { return _IsAuto; }
            set { _IsAuto = value; OnPropertyChanged("IsAuto"); }
        }

        private bool _IsAR;
        public bool IsAR
        {
            get { return _IsAR; }
            set { _IsAR = value; OnPropertyChanged("IsAR"); }
        }

        public int AutoCounter { get; set; }
        public int AutoCutoff { get; set; }

        private bool _FireModulation;
        public bool FireModulation
        {
            get { return _FireModulation; }
            set { _FireModulation = value; OnPropertyChanged("FireModulation"); }
        }

        private bool _Opposite;
        public bool Opposite
        {
            get { return _Opposite; }
            set { _Opposite = value; OnPropertyChanged("Opposite"); }
        }

        private bool _CCOn;
        public bool CCOn
        {
            get { return _CCOn; }
            set { _CCOn = value; OnPropertyChanged("CCOn"); }
        }

        private bool _SettingsOn;
        public bool SettingsOn
        {
            get { return _SettingsOn; }
            set { _SettingsOn = value; OnPropertyChanged("SettingsOn"); }
        }

        public bool OppositeHelper { get; set; }
        public bool EveryOther { get; set; }
        public int LastCC { get; set; }

        public int CurrentCC { get; set; }

        private int _ChannelCC;
        public int ChannelCC
        {
            get { return _ChannelCC; }
            set { _ChannelCC = value; OnPropertyChanged("ChannelCC"); }
        }

        private int _PatternNumber;
        public int PatternNumber
        {
            get { return _PatternNumber; }
            set { _PatternNumber = value; OnPropertyChanged("PatternNumber"); }
        }

        private int _ModeNumber;
        public int ModeNumber
        {
            get { return _ModeNumber; }
            set { _ModeNumber = value; OnPropertyChanged("ModeNumber"); }
        }

        private string _PatternString;
        public string PatternString
        {
            get { return _PatternString; }
            set { _PatternString = value; OnPropertyChanged("PatternString"); }
        }

        public int StepSize { get; set; }
        public int StepComma { get; set; }
        public int Minimum { get; set; }
        public int Maximum { get; set; }
        public int ClockCount { get; set; }
        public int ClockCutoff { get; set; }
        public int RateCatch { get; set; }
        public int RateRemember { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        //Class Constructor
        public MidiModulation()
        {
            //Default Values for New Object of Class
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
            ChannelCC = 0;
            Opposite = false;
            OppositeHelper = false;
            IsRandomRoll = false;
            AutoCounter = 0;
            AutoCutoff = 10;
            IsAR = false;
        }

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

        public void ClockCounter()
        {
            if (IsRunning)
            {
                //ClockCount = ClockCount + 1;
                ClockCount++;

                //Time Mode should be the only place where ClockCutoff is changed in this method
                if ((ModeNumber == 0) || (ModeNumber == 2))
                {
                    ClockCutoff = 1;
                }
                else
                {
                    //This is our auto catch during midi mode - random modulation settings occur at some clock rate
                    if (IsAuto)
                    {
                        //AutoCutoff can be adjusted with settings button on and the rate slider
                        //RandomRoll is disabled when settings are on
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

                }
                if (ClockCount > ClockCutoff)
                {
                    ClockCount = 1;
                }
                if (ClockCount == ClockCutoff)
                {
                    FireModulation = true;
                }
            }
        }

        //UpdateValue stores the different available patterns
        //Calling this program steps the CC value to the next appropriate value based on the pattern number
        public void UpdateValue()
        {
            // =========================================Program #1===========================================
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
            if (PatternNumber == 8)
            {
                CurrentCC = RandomNumber(Minimum, Maximum);
            }
            // ==============================================================================================
        }

        public int RandomNumber(int min, int max)
        {
            return random.Next(min, max);
        }

        public string RandomRoll()
        {
            IsRandomRoll = true;
            string newlabel = "thisString";
            newlabel = UpdatePattern((nint)RandomNumber(0, 8));
            return newlabel;
        }


        //Step Size Setter is used to determine how much the current cc value should change in order to adhere to the overall timing concept while in midi mode.
        //To put this simply, midi clock signals come in at 24 pulses per quarter note. In order to achieve the best resolution possible with our
        //modulation, we need to make use of all of these pulses. In order to reconcile 24 pulses with a 128 value range for the midi cc (0-127), we sometimes
        //have to make little adjustments to the step size (hence the StepComma) to make the overall movement through the pattern range hit at the designated
        //rate.
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


        // Determines StepSize from a sliderValue and PatternNumber
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

        public void IncrementCC(int val)
        {

            if (ChannelCC + val > -1 && ChannelCC + val < 128)
            {
                ChannelCC += val;
            }
            else if (ChannelCC + val > 127)
            {
                ChannelCC += val - 128;
            }
            else
            {
                ChannelCC += val + 128;
            }
        }


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

        public void CCToggle()
        {
            if (SettingsOn)
            {
                SettingsToggle();
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

        public void SettingsToggle()
        {
            if (CCOn)
            {
                CCToggle();
            }

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

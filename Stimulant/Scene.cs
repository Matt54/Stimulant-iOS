using System;
using System.Globalization;
using System.ComponentModel;

namespace Stimulant
{

    public class Scene : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private bool isDisabled;
        public bool IsDisabled()
        {
            return isDisabled;
        }
        public void SetDisabled(bool disabled)
        {
            isDisabled = disabled;
        }

        private int index;
        public int GetIndex()
        {
            return index;
        }
        public void SetIndex(int i)
        {
            index = i;
        }

        //Current scene running the modulations parameters
        private bool _IsRunning;
        public bool IsRunning
        {
            get { return _IsRunning; }
            set { _IsRunning = value; OnPropertyChanged("IsRunning"); }
        }

        //Current scene selected (for adjusting the scenes parameters)
        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { _IsSelected = value; OnPropertyChanged("IsSelected"); }
        }

        //By using the rate slider we can hopefully stay mode number agnostic
        private float _RateSliderValue;
        public float RateSliderValue
        {
            get { return _RateSliderValue; }
            set { _RateSliderValue = value; OnPropertyChanged("RateSliderValue"); }
        }

        private int _PatternNumber;
        public int PatternNumber
        {
            get { return _PatternNumber; }
            set { _PatternNumber = value; OnPropertyChanged("PatternNumber"); }
        }

        
        private bool _Opposite;
        public bool Opposite
        {
            get { return _Opposite; }
            set { _Opposite = value; OnPropertyChanged("Opposite"); }
        }
        

        private int _Minimum;
        public int Minimum
        {
            get { return _Minimum; }
            set { _Minimum = value; OnPropertyChanged("Minimum"); }
        }

        private int _Maximum;
        public int Maximum
        {
            get { return _Maximum; }
            set { _Maximum = value; OnPropertyChanged("Maximum"); }
        }

        private bool _IsTriggerOnly;
        public bool IsTriggerOnly
        {
            get { return _IsTriggerOnly; }
            set { _IsTriggerOnly = value; OnPropertyChanged("IsTriggerOnly"); }
        }

        private bool _IsRestartEachNote;
        public bool IsRestartEachNote
        {
            get { return _IsRestartEachNote; }
            set { _IsRestartEachNote = value; OnPropertyChanged("IsRestartEachNote"); }
        }

        private int _StartingLocation;
        public int StartingLocation
        {
            get { return _StartingLocation; }
            set { _StartingLocation = value; OnPropertyChanged("StartingLocation"); }
        }

        public Scene()
        {
            IsRunning = false;
            IsSelected = false;
            RateSliderValue = 64f;
            PatternNumber = 5;
            Opposite = false;
            Minimum = 0;
            Maximum = 127;
            IsTriggerOnly = false;
            IsRestartEachNote = false;
            StartingLocation = 63;
        }

        /*
        public void ReverseToggle()
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
        */

        public void TriggerToggle()
        {
            IsTriggerOnly = !IsTriggerOnly;
        }

        public void RestartToggle()
        {
            IsRestartEachNote = !IsRestartEachNote;
        }

        // takes in a value from a segmented control  to set the pattern number and relays back a description of the pattern
        public string UpdatePattern(nint patternIndex)
        {
            string labelText;
            switch (patternIndex)
            {
                case 0:
                    labelText = "Pattern 1: Up && Down";
                    break;
                case 1:
                    labelText = "Pattern 2: Up || Down";
                    break;
                case 2:
                    labelText = "Pattern 3: Fwd 2 Back 1";
                    break;
                case 3:
                    labelText = "Pattern 4: Crisscrossed";
                    break;
                case 4:
                    labelText = "Pattern 5: Min Jumper";
                    break;
                case 5:
                    labelText = "Pattern 6:  Max Jumper";
                    break;
                case 6:
                    labelText = "Pattern 7:  Min && Max";
                    break;
                case 7:
                    labelText = "Pattern 8:  Random #'s";
                    break;
                default:
                    labelText = "error";
                    break;
            }
            return labelText;
        }

        public string UpdateRate(int modeNumber)
        {
            string labelText;


            if (modeNumber == 2)
            {
                float timeInterval;
                int StepSize;

                if (PatternNumber > 2)
                {
                    StepSize = 1;
                }
                else
                {
                    if (RateSliderValue > 125)
                    {
                        StepSize = 32;
                    }
                    else if (RateSliderValue > 118)
                    {
                        StepSize = 16;
                    }
                    else if (RateSliderValue > 107)
                    {
                        StepSize = 8;
                    }
                    else if (RateSliderValue > 92)
                    {
                        StepSize = 4;
                    }
                    else if (RateSliderValue > 74)
                    {
                        StepSize = 2;
                    }
                    else
                    {
                        StepSize = 1;
                    }
                }

                switch (PatternNumber)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                        timeInterval = (float)((double)((((double)StepSize) / 2) * (Math.Round(-15.6631 + (1561.999 + 17.55931) / (1 + Math.Pow((RateSliderValue / 13.37739), 2.002958)), 3))));
                        break;
                    case 7:
                    case 8:
                        timeInterval = (float)((double)((((double)StepSize) / 2) * (Math.Round(-3933.384 + (5000.008 + 3933.384) / (1 + Math.Pow((RateSliderValue / 56.13086), 0.2707651)), 3))));
                        break;
                    default:
                        timeInterval = (float)((double)((((double)StepSize) / 2) * (Math.Round(-15.6631 + (1561.999 + 17.55931) / (1 + Math.Pow((RateSliderValue / 13.37739), 2.002958)), 3))));
                        break;
                }

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
                labelText = "Frequency: " + ((((double)StepSize) * 64 / divFactor) * Math.Round((1 / (timeInterval * (128 / 2) / 1000)), 3)).ToString("0.000", CultureInfo.InvariantCulture) + " Hz";
            }
            else
            {

                string displayText = "";
                int rateCatch;
                // Conditionals determine the correct rate based on sliderValue
                if (RateSliderValue >= (128 * 17 / 18))
                { // 32 note triples
                    displayText = "1/48";
                    rateCatch = 18;
                }
                else if (RateSliderValue >= (128 * 16 / 18))
                { // 32 notes
                    displayText = "1/32";
                    rateCatch = 17;
                }
                else if (RateSliderValue >= (128 * 15 / 18))
                {  // sixteenth note triples
                    displayText = "1/24";
                    rateCatch = 16;
                }
                else if (RateSliderValue >= (128 * 14 / 18))
                { // sixteenth notes
                    displayText = "1/16";
                    rateCatch = 15;
                }
                else if (RateSliderValue >= (128 * 13 / 18))
                {  // eighth note triples
                    displayText = "1/12";
                    rateCatch = 14;
                }
                else if (RateSliderValue >= (128 * 12 / 18))
                {  // eighth notes
                    displayText = "1/8 ";
                    rateCatch = 13;
                }
                else if (RateSliderValue >= (128 * 11 / 18))
                {  // quarter note triples
                    displayText = "1/6 ";
                    rateCatch = 12;
                }
                else if (RateSliderValue >= (128 * 10 / 18))
                {  // quarter notes
                    displayText = "1/4 ";
                    rateCatch = 11;
                }
                else if (RateSliderValue >= (128 * 9 / 18))
                {  // half note triples
                    displayText = "1/3 ";
                    rateCatch = 10;
                }
                else if (RateSliderValue >= (128 * 8 / 18))
                {  // half note
                    displayText = "1/2 ";
                    rateCatch = 9;
                }
                else if (RateSliderValue >= (128 * 7 / 18))
                { // whole note triples
                    displayText = "3/4 ";
                    rateCatch = 8;
                }
                else if (RateSliderValue >= (128 * 6 / 18))
                { // whole note
                    displayText = "1/1 ";
                    rateCatch = 7;
                }
                else if (RateSliderValue >= (128 * 5 / 18))
                { // 2 bar triples
                    displayText = "3/2 ";
                    rateCatch = 6;
                }
                else if (RateSliderValue >= (128 * 4 / 18))
                { // 2 bars
                    displayText = "2/1 ";
                    rateCatch = 5;
                }
                else if (RateSliderValue >= (128 * 3 / 18))
                { // 4 bar triples
                    displayText = "3/1 ";
                    rateCatch = 4;
                }
                else if (RateSliderValue >= (128 * 2 / 18))
                { // 4 bar
                    displayText = "4/1 ";
                    rateCatch = 3;
                }
                else if (RateSliderValue >= (128 * 1 / 18))
                { // 4 bar
                    displayText = "6/1 ";
                    rateCatch = 2;
                }
                else if (RateSliderValue < 7)
                { // 4 bar
                    displayText = "8/1 ";
                    rateCatch = 1;
                }

                if (modeNumber == 1)
                {
                    //EXT Clock Sync
                    labelText = "Ext. Clock Sync: " + displayText;
                }
                else
                {
                    //INT Clock Sync
                    labelText = "Int. Clock Sync: " + displayText;
                }
            }
            return labelText;
        }
    }
}

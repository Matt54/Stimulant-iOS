using System;
using System.Globalization;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Generic;

namespace Stimulant
{

    public class Scene : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
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

        public void TriggerToggle()
        {
            IsTriggerOnly = !IsTriggerOnly;
        }

        public void RestartToggle()
        {
            IsRestartEachNote = !IsRestartEachNote;
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
            StartingLocation = 0;
        }

    }
}

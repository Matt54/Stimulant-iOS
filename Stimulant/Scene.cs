using System;
namespace Stimulant
{

    public class Scene
    {

        //Current scene running the modulations parameters
        public bool IsRunning { get; set; }

        //Current scene selected (for adjusting the scenes parameters)
        public bool IsSelected { get; set; }

        //By using the rate slider we can hopefully stay mode number agnostic
        public float RateSliderValue { get; set; }

        public int PatternNumber { get; set; }
        public bool Opposite { get; set; }

        public int Minimum { get; set; }
        public int Maximum { get; set; }

        public bool IsTriggerOnly { get; set; }
        public bool IsRestartEachNote { get; set; }
        public int StartingLocation { get; set; }

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

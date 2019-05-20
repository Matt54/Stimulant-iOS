using System;
namespace Stimulant
{
    public class MidiModulation
    {
        public bool IsRunning { get; set; }
        public bool FireModulation { get; set; }
        public bool Opposite { get; set; }
        public bool OppositeHelper { get; set; }
        public bool EveryOther { get; set; }

        public int LastCC { get; set; }
        public int CurrentCC { get; set; }
        public int PatternNumber { get; set; }
        public int ModeNumber { get; set; }
        public int StepSize { get; set; }
        public int StepComma { get; set; }
        public int Minimum { get; set; }
        public int Maximum { get; set; }
        public int ClockCount { get; set; }
        public int ClockCutoff { get; set; }
        public int CutoffIncrease { get; set; }
        public int RateCatch { get; set; }



        public double TimerHz { get; set; }


        public MidiModulation()
        {


        }


    }
}

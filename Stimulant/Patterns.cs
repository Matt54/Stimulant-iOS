using System;

namespace Stimulant
{
    public class Patterns
    {
        public static int numPatterns;
        Pattern[] myPatterns;
        public int patternNumSelected;

        public int StepSize { get; set; } // property



        public Patterns()
        {
            Pattern1 pattern1 = new Pattern1();
            Pattern2 pattern2 = new Pattern2();

            myPatterns = new Pattern[pattern1.GetNumberOfPatterns()];

            myPatterns[1] = pattern1;
            myPatterns[2] = pattern2;
        }

        private bool opposite; // field
        public bool Opposite   // property
        {
            get { return opposite; }   // get method
            set
            {
                EchoOpposite(value);
                opposite = value;
            }  // set method
        }

        void EchoOpposite(bool opp)
        {
            foreach(Pattern pattern in myPatterns) pattern.Opposite = opp;
        }
        
        class Pattern1 : Pattern
        {
            public override int Function(int x)
            {
                if (x < domain / 2) return Min + 2 * x * range / domain; //Up for the half
                return Min + range - (2 * x * range / domain); //Down for the second
            }
        }

        class Pattern2 : Pattern
        {
            public override int Function(int x)
            {
                if (Opposite) return Min + x * range / domain; //Up when normal
                return Min + range - (x * range / domain); //Down when normal
            }
        }

        class Pattern3 : Pattern
        {
            public override int Function(int x)
            {
                stepNum++;
                if (stepNum > numSteps) stepNum = 1;

                if (offDuty) return Min;
                return Min + range * (numSteps / stepNum);
            }
            private int numSteps;
            private int stepNum;
            private bool offDuty;
        }

        abstract class Pattern
        {
            public static int counter;
            public int GetNumberOfPatterns() { return counter; }
            protected Pattern() { counter++; }

            public abstract int Function(int x);
            public bool Opposite { get; set; }
            public int domain = 1000;
            public int range = 128;

            private int min; // field
            public int Min   // property
            {
                get { return min; }   // get method
                set
                {
                    min = value;
                    range = max - min;
                }  // set method
            }

            private int max; // field
            public int Max   // property
            {
                get { return max; }   // get method
                set
                {
                    max = value;
                    range = max - min;
                }  // set method
            }

        }


        
    }
}

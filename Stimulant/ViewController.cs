using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using UIKit;
using CoreMidi;
using Xamarin.RangeSlider;
using System.Timers;
using System.Diagnostics;
using MonoTouch.Dialog;
using System.Threading;

namespace Stimulant
{
    public partial class ViewController : UIViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {// Note: this .ctor should not contain any initialization logic.  
        }
        
        MidiClient client;
        MidiPort outputPort, inputPort;

        System.Timers.Timer timer = new System.Timers.Timer();
        Random rand = new Random();
        //Section hardwareSection;


        MidiModulation my_mod = new MidiModulation();



        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            Midi.Restart();
            SetupMidi();
            MakeHardware();
            MakeDevices();
            ReloadDevices();

            timer.Interval = 100;
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;
            timer.Start();

            my_mod.PatternNumber = 1;
            my_mod.ModeNumber = 0;
            my_mod.ClockCount = 0;
            my_mod.ClockCutoff = 24;
            my_mod.FireModulation = false;
            my_mod.IsRunning = false;
            my_mod.CurrentCC = 0;
            my_mod.LastCC = 0;
            my_mod.StepSize = 2;
            my_mod.Maximum = 127;
            my_mod.Minimum = 0;
            my_mod.Opposite = false;
            my_mod.OppositeHelper = false;

        }

        partial void RateSliderChange(UISlider sender)
        {
            ReadSlider(my_mod, sender.Value);

            //throw new NotImplementedException();
        }

        partial void StartButton_TouchUpInside(UIButton sender)
        {
            //ToggleTimer(my_mod.IsRunning);

            if (my_mod.IsRunning)
            {
                my_mod.IsRunning = false;
                startButton.SetTitle("Start",UIControlState.Normal);
                startButton.BackgroundColor = UIColor.Yellow;
            }
            else
            {
                my_mod.IsRunning = true;
                startButton.SetTitle("Stop", UIControlState.Normal);
                //startButton.SetTitle(Convert.ToString(Convert.ToInt32(my_mod.CurrentCC * 100 / 128)) + "%", UIControlState.Normal);
                //startButton.SetTitle(Convert.ToString(my_mod.CurrentCC), UIControlState.Normal);
                startButton.BackgroundColor = UIColor.Green;
            }



            //throw new NotImplementedException();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            InvokeOnMainThread(() => {

                ClockCounter(my_mod);

                //ClockCounter(my_mod);

                //int ccVal = (byte)(rand.Next() % 127);
                //SendCC(ccVal);

                //lblTimer.Text = myDate.ToString("F");
                //toggleButton.TitleLabel.Text = "Stop";
            });
        }

        void ToggleTimer(bool is_running)
        {
            if (is_running)
            {
                timer.Stop();
                //toggleButton.TitleLabel.Text = "Start";
                is_running = false;
            }
            else
            {
                timer.Enabled = true;
                timer.Start();
                //toggleButton.TitleLabel.Text = "Stop";
                is_running = true;

            }
        }

        void ReadSlider(MidiModulation thisMod, float SliderValue)
        {

            string DisplayString = "";

            // Converts analog signal to 0-127 range
            //SliderValue = 128;

            // Conditionals determine the correct rate based on potentiometer location
            if (SliderValue >= (128 * 15 / 16))
            { // 32 note triples
                thisMod.ClockCutoff = 1;
                thisMod.CutoffIncrease = 1;

                DisplayString = "Rate: 1/32T";
                thisMod.RateCatch = 16;
            }
            else if (SliderValue >= (128 * 14 / 16))
            { // 32 notes
                thisMod.ClockCutoff = 1;
                thisMod.CutoffIncrease = 2;
                DisplayString = "Rate: 1/32";
                thisMod.RateCatch = 15;
            }
            else if (SliderValue >= (128 * 13 / 16))
            {  // sixteenth note triples
                thisMod.ClockCutoff = 1;
                thisMod.CutoffIncrease = 3;

                DisplayString = "Rate: 1/16T";
                thisMod.RateCatch = 14;
            }
            else if (SliderValue >= (128 * 12 / 16))
            { // sixteenth notes
                thisMod.ClockCutoff = 1;
                thisMod.CutoffIncrease = 5;

                DisplayString = "Rate: 1/16";
                thisMod.RateCatch = 13;
            }
            else if (SliderValue >= (128 * 11 / 16))
            {  // eighth note triples
                thisMod.ClockCutoff = 1;
                thisMod.CutoffIncrease = 7;

                DisplayString = "Rate: 1/8T";
                thisMod.RateCatch = 12;
            }
            else if (SliderValue >= (128 * 10 / 16))
            {  // eighth notes
                thisMod.ClockCutoff = 1;
                thisMod.CutoffIncrease = 11;

                DisplayString = "Rate: 1/8";
                thisMod.RateCatch = 11;
            }
            else if (SliderValue >= (128 * 9 / 16))
            {  // quarter note triples
                thisMod.ClockCutoff = 1;
                thisMod.CutoffIncrease = 15;

                DisplayString = "Rate: 1/4T";
                thisMod.RateCatch = 10;
            }
            else if (SliderValue >= (128 * 8 / 16))
            {  // quarter notes
                thisMod.ClockCutoff = 1;
                thisMod.CutoffIncrease = 23;

                DisplayString = "Rate: 1/4";
                thisMod.RateCatch = 9;
            }
            else if (SliderValue >= (128 * 7 / 16))
            {  // half note triples
                thisMod.ClockCutoff = 1;
                thisMod.CutoffIncrease = 31;

                DisplayString = "Rate: 1/2T";
                thisMod.RateCatch = 8;
            }
            else if (SliderValue >= (128 * 6 / 16))
            {  // half note
                thisMod.ClockCutoff = 1;
                thisMod.CutoffIncrease = 47;

                DisplayString = "Rate: 1/2";
                thisMod.RateCatch = 7;
            }
            else if (SliderValue >= (128 * 5 / 16))
            { // whole note triples
                thisMod.ClockCutoff = 1;
                thisMod.CutoffIncrease = 65;

                DisplayString = "Rate: 1/1T";
                thisMod.RateCatch = 6;
            }
            else if (SliderValue >= (128 * 4 / 16))
            { // whole note
                thisMod.ClockCutoff = 1;
                thisMod.CutoffIncrease = 95;
                DisplayString = "Rate: 1/1";
                thisMod.RateCatch = 5;
            }
            else if (SliderValue >= (128 * 3 / 16))
            { // 2 bar triples
                thisMod.ClockCutoff = 1;

                thisMod.CutoffIncrease = 95;
                DisplayString = "Rate: 2/1T";
                thisMod.RateCatch = 4;
            }
            else if (SliderValue >= (128 * 2 / 16))
            { // 2 bars
                thisMod.ClockCutoff = 2;

                thisMod.CutoffIncrease = 191;

                DisplayString = "Rate: 2/1";
                thisMod.RateCatch = 3;
            }
            else if (SliderValue >= (128 * 1 / 16))
            { // 4 bar triples
                thisMod.ClockCutoff = 2;

                thisMod.CutoffIncrease = 95;
                DisplayString = "Rate: 4/1T";
                thisMod.RateCatch = 2;
            }
            else if (SliderValue < 8)
            { // 4 bar
                thisMod.ClockCutoff = 4;
                thisMod.CutoffIncrease = 383;
                DisplayString = "Rate: 4/1";
                thisMod.RateCatch = 1;
            }
            StepSizeSetter(thisMod);
            rateLabel.Text = "Synced " + DisplayString;
            if (thisMod.ModeNumber == 2)
            {
                timer.Interval = Math.Round((7030.7 * Math.Pow(SliderValue + 1, -1.559)) - 3);
            }

        }

        void StepSizeSetter(MidiModulation thisMod)
        {
            switch (thisMod.RateCatch)
            {
                case 1:
                    thisMod.StepSize = 1;

                    if (thisMod.StepComma == 2)
                    {
                        thisMod.StepSize = thisMod.StepSize + 1;
                    }
                    break;
                case 2:
                    thisMod.StepSize = 1;
                    break;
                case 3:
                    thisMod.StepSize = 1;
                    if (thisMod.StepComma == 2)
                    {
                        thisMod.StepSize = thisMod.StepSize + 1;
                    }
                    break;
                case 4:
                    thisMod.StepSize = 1;
                    break;
                case 5:
                    thisMod.StepSize = 1;

                    if (thisMod.StepComma == 2)
                    {
                        thisMod.StepSize = thisMod.StepSize + 1;
                    }
                    break;
                case 6:
                    thisMod.StepSize = 2;
                    break;
                case 7:
                    thisMod.StepSize = 3;
                    if (thisMod.StepComma == 2)
                    {
                        thisMod.StepSize = thisMod.StepSize - 1;
                    }
                    break;
                case 8:
                    thisMod.StepSize = 4;
                    break;
                case 9:
                    thisMod.StepSize = 5;
                    if (thisMod.StepComma == 2)
                    {
                        thisMod.StepSize = thisMod.StepSize + 1;
                    }
                    break;
                case 10:
                    thisMod.StepSize = 8;
                    break;
                case 11:
                    thisMod.StepSize = 11;
                    if (thisMod.StepComma == 2)
                    {
                        thisMod.StepSize = thisMod.StepSize - 1;
                    }
                    break;
                case 12:
                    thisMod.StepSize = 16;
                    break;
                case 13:
                    thisMod.StepSize = 21;
                    if (thisMod.StepComma == 2)
                    {
                        thisMod.StepSize = thisMod.StepSize + 1;
                    }
                    break;
                case 14:
                    thisMod.StepSize = 32;
                    break;
                case 15:
                    thisMod.StepSize = 42;

                    if (thisMod.StepComma == 2)
                    {
                        thisMod.StepSize = thisMod.StepSize - 1;
                    }
                    break;
                case 16:
                    thisMod.StepSize = 64;
                    break;
            }
        }

        void ClockCounter(MidiModulation thisMod)
        {
            if (thisMod.IsRunning)
            {

                /*
                thisMod.ClockCount = thisMod.ClockCount + 1;
                if (thisMod.ClockCount > thisMod.ClockCutoff - 1)
                {
                    thisMod.FireModulation = true;
                }
                */

                if ((thisMod.ModeNumber == 0)|| (thisMod.ModeNumber == 2))
                {  // thisMod.ClockCutoff only changes in sync mode (time delay mode: thisMod.ClockCutoff = 1 always)
                    thisMod.ClockCutoff = 1;
                }
                else
                {
                    if (thisMod.PatternNumber > 2)
                    {
                        thisMod.ClockCutoff = thisMod.ClockCutoff + (thisMod.CutoffIncrease / 2);
                    }
                    else
                    {
                        thisMod.ClockCutoff = 1;
                    }
                }


                thisMod.ClockCount = thisMod.ClockCount + 1;

                if (thisMod.PatternNumber < 3)
                {
                    if (thisMod.ClockCount > thisMod.ClockCutoff)
                    {
                        thisMod.ClockCount = 1;
                    }
                    if (thisMod.ClockCount == thisMod.ClockCutoff)
                    {
                        thisMod.FireModulation = true;
                    }
                }
                else
                {
                    if (thisMod.ClockCount > thisMod.ClockCutoff)
                    {
                        thisMod.ClockCount = 1;
                    }
                    if (thisMod.ClockCount == thisMod.ClockCutoff)
                    {
                        thisMod.FireModulation = true;
                    }
                }




                if (thisMod.FireModulation)
                {
                    //GetPatternNumber(thisMod);
                    //thisMod.PatternNumber = Convert.ToInt32(pnumSegmentedControl.SelectedSegment) + 1;
                    /*
                    var index = pnumSegmentedControl.SelectedSegment;
                    if (index == 0)
                    {
                        thisMod.PatternNumber = 1;
                    }
                    else if (index == 1)
                    {
                        thisMod.PatternNumber = 2;
                    }
                    else if (index == 2)
                    {
                        thisMod.PatternNumber = 3;
                    }
                    else if (index == 3)
                    {
                        thisMod.PatternNumber = 4;
                    }
                    */
                    //thisMod.PatternNumber = (int)pnumSegmentedControl.SelectedSegment;
                    //thisMod.PatternNumber = thisMod.PatternNumber + 1;
                    //thisMod.PatternNumber = 2;
                    //my_mod.PatternNumber = (int)pnumSegmentedControl.SelectedSegment;
                    //my_mod.PatternNumber = (int)this.pnumSegmentedControl.SelectedSegment;
                    //my_mod.PatternNumber = (int)(my_mod.PatternNumber + 1);
                    UpdateValue(thisMod);
                    SendCC(thisMod.CurrentCC);
                    thisMod.FireModulation = false;
                    thisMod.ClockCount = 0;
                    //startButton.SetTitle(Convert.ToString(Convert.ToInt32(my_mod.CurrentCC*100/128)) + "%", UIControlState.Normal);
                    //startButton.SetTitle(Convert.ToString(my_mod.CurrentCC), UIControlState.Normal);
                }
            }
        }

        partial void ModeNumChanged(UISegmentedControl sender)
        {
            var index = sender.SelectedSegment;

            switch (index)
            {
                case 0:
                    timer.Interval = 100;
                    timer.Enabled = true;
                    timer.Start();
                    my_mod.ModeNumber = 0;
                    break;
                case 1:
                    timer.Stop();
                    my_mod.ModeNumber = 1;
                    break;
                case 2:
                    timer.Enabled = true;
                    timer.Start();
                    my_mod.ModeNumber = 2;
                    break;
                default:
                    break;
            }
            //throw new NotImplementedException();
        }

        partial void pnumChange(UISegmentedControl sender)
        {
            var index = sender.SelectedSegment;

            switch (index)
            {
                case 0:
                    programLabel.Text = "Pattern 1: UpandDown";
                    my_mod.PatternNumber = 1;
                    break;
                case 1:
                    programLabel.Text = "Pattern 2: Up/Down";
                    my_mod.PatternNumber = 2;
                    break;
                case 2:
                    programLabel.Text = "Pattern 3: Forward 2 Back 1";
                    my_mod.PatternNumber = 3;
                    break;
                case 3:
                    programLabel.Text = "Pattern 4: Crisscross";
                    my_mod.PatternNumber = 4;
                    break;
                case 4:
                    programLabel.Text = "Pattern 5: Min & Up/Down";
                    my_mod.PatternNumber = 5;
                    break;
                case 5:
                    programLabel.Text = "Pattern 6:  Max & Up/Down";
                    my_mod.PatternNumber = 6;
                    break;
                case 6:
                    programLabel.Text = "Pattern 7:  Min & Max";
                    my_mod.PatternNumber = 7;
                    break;
                case 7:
                    programLabel.Text = "Pattern 8:  Random";
                    my_mod.PatternNumber = 8;
                    break;
                default:
                    break;
            }
            //throw new NotImplementedException();
        }

        void SendCC(int ccVal)
        {
            byte ccByte = (byte)ccVal;
            
            for (int i = 0; i < Midi.DestinationCount; i++)
            {
                var endpoint = MidiEndpoint.GetDestination(i);
                outputPort.Send(endpoint, new MidiPacket[] { new MidiPacket(0, new byte[] { 0xB0, 1, ccByte }) });

                //var ccVal = (byte)(rand.Next () % 127);
                // play ccVal then turn off after 300 miliseconds
                /*
                outputPort.Send (endpoint, new MidiPacket [] { new MidiPacket (0, new byte [] { 0x90, ccVal, 127 }) });
                Thread.Sleep (300);
                outputPort.Send (endpoint, new MidiPacket [] { new MidiPacket (0, new byte [] { 0x80, ccVal, 0 }) });
                */
            }
        }

        void GetPatternNumber(MidiModulation thisMod)
        {

            var index = pnumSegmentedControl.SelectedSegment;
            if (index == 0)
            {
                thisMod.PatternNumber = 1;
            }
            else if (index == 1)
            {
                thisMod.PatternNumber = 2;
            }
            else if (index == 2)
            {
                thisMod.PatternNumber = 3;
            }
            else if (index == 3)
            {
                thisMod.PatternNumber = 4;
            }

            //thisMod.PatternNumber = Convert.ToInt32(pnumSegmentedControl.SelectedSegment)+1;

        }

        void GetMode(MidiModulation thisMod)
        {
            thisMod.ModeNumber = Convert.ToInt32(modeSegmentedControl.SelectedSegment);
        }




        void UpdateValue(MidiModulation thisMod)
        {

            // =========================================Program #1===========================================
            if (thisMod.PatternNumber == 1)
            {
                if (thisMod.Opposite == false)
                {
                    if ((thisMod.CurrentCC < thisMod.Maximum) && (thisMod.CurrentCC >= thisMod.LastCC))
                    {
                        thisMod.LastCC = thisMod.CurrentCC;
                        thisMod.CurrentCC = thisMod.CurrentCC + thisMod.StepSize;
                        if (thisMod.CurrentCC >= thisMod.Maximum)
                        {
                            thisMod.CurrentCC = thisMod.Maximum;
                        }
                    }
                    else if ((thisMod.CurrentCC < thisMod.Maximum) && (thisMod.CurrentCC <= thisMod.LastCC) && (thisMod.CurrentCC > thisMod.Minimum))
                    {
                        thisMod.LastCC = thisMod.CurrentCC;
                        thisMod.CurrentCC = thisMod.CurrentCC - thisMod.StepSize;
                        if (thisMod.CurrentCC < thisMod.Minimum)
                        {
                            thisMod.LastCC = thisMod.Minimum;
                            thisMod.CurrentCC = thisMod.Minimum;
                        }
                    }
                    else if (thisMod.CurrentCC >= thisMod.Maximum)
                    {
                        thisMod.LastCC = thisMod.CurrentCC;
                        thisMod.CurrentCC = thisMod.CurrentCC - thisMod.StepSize;
                    }
                    else if (thisMod.CurrentCC <= thisMod.Minimum)
                    {
                        thisMod.LastCC = thisMod.CurrentCC;
                    }
                    if (thisMod.OppositeHelper == false)
                    {
                        thisMod.OppositeHelper = true;
                    }
                }

                else
                {
                    if ((thisMod.CurrentCC < thisMod.Maximum) && (thisMod.CurrentCC < thisMod.LastCC))
                    {
                        thisMod.LastCC = thisMod.CurrentCC;
                        if (thisMod.OppositeHelper)
                        {
                            thisMod.OppositeHelper = false;
                            thisMod.CurrentCC = thisMod.CurrentCC - thisMod.StepSize;
                        }
                        thisMod.CurrentCC = thisMod.CurrentCC + thisMod.StepSize;
                        if (thisMod.CurrentCC >= thisMod.Maximum)
                        {
                            thisMod.CurrentCC = thisMod.Maximum;
                        }
                    }
                    else if ((thisMod.CurrentCC < thisMod.Maximum) && (thisMod.CurrentCC >= thisMod.LastCC) && (thisMod.CurrentCC > thisMod.Minimum))
                    {
                        thisMod.LastCC = thisMod.CurrentCC;
                        thisMod.CurrentCC = thisMod.CurrentCC - thisMod.StepSize;
                        if (thisMod.CurrentCC < thisMod.Minimum)
                        {
                            thisMod.LastCC = thisMod.Minimum;
                            thisMod.CurrentCC = thisMod.Minimum;
                        }
                    }
                    else if (thisMod.CurrentCC >= thisMod.Maximum)
                    {
                        thisMod.LastCC = thisMod.CurrentCC;
                        thisMod.CurrentCC = thisMod.CurrentCC - thisMod.StepSize;
                    }
                    else if (thisMod.CurrentCC <= thisMod.Minimum)
                    {
                        thisMod.LastCC = thisMod.CurrentCC;
                    }
                }
            }
            // ==============================================================================================

            // =========================================Program #2===========================================
            if (thisMod.PatternNumber == 2)
            {
                if (thisMod.Opposite == false)
                {
                    if (thisMod.CurrentCC < thisMod.Maximum)
                    {
                        thisMod.CurrentCC = thisMod.CurrentCC + thisMod.StepSize;
                        if (thisMod.CurrentCC >= thisMod.Maximum)
                        {
                            thisMod.CurrentCC = thisMod.Minimum;
                        }
                    }
                }
                else
                {
                    thisMod.CurrentCC = thisMod.CurrentCC - thisMod.StepSize;
                    if (thisMod.CurrentCC <= thisMod.Minimum)
                    {
                        thisMod.CurrentCC = thisMod.Maximum;
                    }
                }
            }
            // ==============================================================================================

            // =========================================Program #3===========================================
            if (thisMod.PatternNumber == 3)
            {
                if (thisMod.EveryOther)
                {
                    if (thisMod.Opposite == false)
                    {
                        thisMod.CurrentCC = thisMod.CurrentCC + thisMod.StepSize * 2;
                    }
                    else
                    {
                        thisMod.CurrentCC = thisMod.CurrentCC - thisMod.StepSize * 2;
                    }
                    thisMod.EveryOther = false;
                }
                else
                {
                    thisMod.EveryOther = true;
                    if (thisMod.Opposite == false)
                    {
                        thisMod.CurrentCC = thisMod.CurrentCC - thisMod.StepSize;
                    }
                    if (thisMod.Opposite)
                    {
                        thisMod.CurrentCC = thisMod.CurrentCC + thisMod.StepSize;
                    }
                }
                if (thisMod.Opposite == false)
                {
                    if (thisMod.CurrentCC >= thisMod.Maximum)
                    {
                        thisMod.CurrentCC = thisMod.Minimum;
                        thisMod.LastCC = thisMod.Minimum;
                    }
                }
                else
                {
                    if (thisMod.CurrentCC <= thisMod.Minimum)
                    {
                        thisMod.CurrentCC = thisMod.Maximum;
                        thisMod.LastCC = thisMod.Maximum;
                    }
                }
            }
            // ==============================================================================================

            // =========================================Program #4===========================================
            if (thisMod.PatternNumber == 4)
            {
                if (thisMod.EveryOther)
                {
                    thisMod.EveryOther = false;
                    thisMod.CurrentCC = thisMod.LastCC + thisMod.StepSize;
                }
                else
                {
                    thisMod.EveryOther = true;
                    thisMod.LastCC = thisMod.CurrentCC;
                    thisMod.CurrentCC = thisMod.Maximum - thisMod.CurrentCC;
                }
                if (thisMod.CurrentCC > thisMod.Maximum)
                {
                    thisMod.CurrentCC = thisMod.Minimum;
                }
                if (thisMod.CurrentCC < thisMod.Minimum)
                {
                    thisMod.CurrentCC = thisMod.Maximum;
                }
            }
            // ==============================================================================================

            // =========================================Program #5===========================================
            if (thisMod.PatternNumber == 5)
            {
                if (thisMod.EveryOther)
                {
                    thisMod.LastCC = thisMod.CurrentCC;
                    thisMod.EveryOther = false;
                    thisMod.CurrentCC = thisMod.Minimum;
                }
                else
                {
                    thisMod.EveryOther = true;
                    if (thisMod.Opposite == false)
                    {
                        thisMod.CurrentCC = thisMod.LastCC + thisMod.StepSize;
                    }
                    if (thisMod.Opposite)
                    {
                        thisMod.CurrentCC = thisMod.LastCC - thisMod.StepSize;
                    }
                }

                if (thisMod.CurrentCC > thisMod.Maximum)
                {
                    thisMod.CurrentCC = thisMod.Minimum;
                    thisMod.LastCC = thisMod.Minimum;
                }
                if (thisMod.CurrentCC < thisMod.Minimum)
                {
                    thisMod.CurrentCC = thisMod.Maximum;
                    thisMod.LastCC = thisMod.Maximum;
                }
            }
            // ==============================================================================================

            // =========================================Program #6===========================================
            if (thisMod.PatternNumber == 6)
            {
                if (thisMod.EveryOther)
                {
                    thisMod.LastCC = thisMod.CurrentCC;
                    thisMod.EveryOther = false;
                    thisMod.CurrentCC = thisMod.Maximum;
                }
                else
                {
                    thisMod.EveryOther = true;
                    if (thisMod.Opposite == false)
                    {
                        thisMod.CurrentCC = thisMod.LastCC - thisMod.StepSize;
                    }
                    if (thisMod.Opposite)
                    {
                        thisMod.CurrentCC = thisMod.LastCC + thisMod.StepSize;
                    }
                }
                if (thisMod.CurrentCC <= thisMod.Minimum)
                {
                    thisMod.CurrentCC = thisMod.Maximum;
                    thisMod.LastCC = thisMod.Maximum;
                }
                if ((thisMod.Opposite) && (thisMod.EveryOther))
                {
                    if (thisMod.CurrentCC >= thisMod.Maximum)
                    {
                        thisMod.CurrentCC = thisMod.Minimum;
                        thisMod.LastCC = thisMod.Minimum;
                    }
                }
            }
            // ==============================================================================================

            // =========================================Program #7===========================================
            if (thisMod.PatternNumber == 7)
            {
                if (thisMod.CurrentCC < thisMod.Maximum)
                {
                    thisMod.CurrentCC = thisMod.Maximum;
                }
                else if (thisMod.CurrentCC >= thisMod.Maximum)
                {
                    thisMod.CurrentCC = thisMod.Minimum;
                }
            }
            // ==============================================================================================

            // =========================================Program #8===========================================
            if (thisMod.PatternNumber == 8)
            {
                thisMod.CurrentCC = (rand.Next() % 127);
            }
            // ==============================================================================================

            //ccVal = (byte)control_value;
        }



        RootElement MakeHardware()
        {
            int sources = (int)Midi.SourceCount;
            int destinations = (int)Midi.DestinationCount;

            var sourcesSection = new Section("Sources");
            sourcesSection.AddAll(
                from x in Enumerable.Range(0, sources)
                let source = MidiEndpoint.GetSource(x)
                select (Element)new StringElement(source.DisplayName, source.IsNetworkSession ? "Network" : "Local")
            );
            var targetsSection = new Section("Targets");
            targetsSection.AddAll(
                from x in Enumerable.Range(0, destinations)
                let target = MidiEndpoint.GetDestination(x)
                select (Element)new StringElement(target.DisplayName, target.IsNetworkSession ? "Network" : "Local")
            );
            return new RootElement("Endpoints (" + sources + ", " + destinations + ")") {
                sourcesSection,
                targetsSection
            };
        }



        RootElement MakeDevices()
        {
            var internalDevices = new Section("Internal Devices");
            internalDevices.AddAll(
                from x in Enumerable.Range(0, (int)Midi.DeviceCount)
                let dev = Midi.GetDevice(x)
                where dev.EntityCount > 0
                select MakeDevice(dev)
            );
            var externalDevices = new Section("External Devices");
            externalDevices.AddAll(
                from x in Enumerable.Range(0, (int)Midi.ExternalDeviceCount)
                let dev = Midi.GetExternalDevice(x)
                where dev.EntityCount > 0
                select (Element)MakeDevice(dev)
            );
            return new RootElement("Devices (" + Midi.DeviceCount + ", " + Midi.ExternalDeviceCount + ")") {
                internalDevices,
                externalDevices
            };
        }

        Element MakeDevice(MidiDevice dev)
        {
            var entities = new Section("Entities");
            foreach (var ex in Enumerable.Range(0, (int)dev.EntityCount))
            {
                var entity = dev.GetEntity(ex);
                var sourceSection = new Section("Sources");
                sourceSection.AddAll(
                    from sx in Enumerable.Range(0, (int)entity.Sources)
                    let endpoint = entity.GetSource(sx)
                    select MakeEndpoint(endpoint)
                );
                var destinationSection = new Section("Destinations");
                destinationSection.AddAll(
                    from sx in Enumerable.Range(0, (int)entity.Destinations)
                    let endpoint = entity.GetDestination(sx)
                    select MakeEndpoint(endpoint)
                );
                entities.Add(new RootElement(entity.Name) {
                    sourceSection,
                    destinationSection
                });
            }

            return new RootElement(String.Format("{2} {0} {1}", dev.Manufacturer, dev.Model, dev.EntityCount)) {
                entities
            };
        }



        Element MakeEndpoint(MidiEndpoint endpoint)
        {
            Section s;
            var root = new RootElement(endpoint.Name) {
                (s = new Section ("Properties") {
                    new StringElement ("Driver Owner", endpoint.DriverOwner),
                    new StringElement ("Manufacturer", endpoint.Manufacturer),
                    new StringElement ("MaxSysExSpeed", endpoint.MaxSysExSpeed.ToString ()),
                    new StringElement ("Network Session", endpoint.IsNetworkSession ? "yes" : "no")
                })
            };
            try
            {
                s.Add(new StringElement("Offline", endpoint.Offline ? "yes" : "no"));
            }
            catch
            {
            }
            try
            {
                s.Add(new StringElement("Receive Channels", endpoint.ReceiveChannels.ToString()));
            }
            catch
            {
            }
            try
            {
                s.Add(new StringElement("Transmit Channels", endpoint.TransmitChannels.ToString()));
            }
            catch
            {
            }
            return root;
        }

        void ReloadDevices()
        {
            /*
            BeginInvokeOnMainThread(delegate {
                hardwareSection.Remove(0);
                hardwareSection.Remove(0);
                hardwareSection.Add((Element)MakeHardware());
                hardwareSection.Add((Element)MakeDevices());
            });
            */
        }

        void SetupMidi()
        {
            client = new MidiClient("CoreMidiSample MIDI CLient");
            client.ObjectAdded += delegate (object sender, ObjectAddedOrRemovedEventArgs e) {

            };
            client.ObjectAdded += delegate {
                ReloadDevices();
            };
            client.ObjectRemoved += delegate {
                ReloadDevices();
            };
            client.PropertyChanged += delegate (object sender, ObjectPropertyChangedEventArgs e) {
                Console.WriteLine("Changed");
            };
            client.ThruConnectionsChanged += delegate {
                Console.WriteLine("Thru connections changed");
            };
            client.SerialPortOwnerChanged += delegate {
                Console.WriteLine("Serial port changed");
            };

            outputPort = client.CreateOutputPort("CoreMidiSample Output Port");
            inputPort = client.CreateInputPort("CoreMidiSample Input Port");

            inputPort.MessageReceived += delegate (object sender, MidiPacketsEventArgs e) {
                Console.WriteLine("Got {0} packets", e.Packets.Length);
                //Debug.Write("Got " + Convert.ToString(e.Packets.Length) + " Packets");
                foreach (MidiPacket mPacket in e.Packets)
                {
                    if (my_mod.ModeNumber == 1)
                    {
                        var midiData = new byte[mPacket.Length];
                        Marshal.Copy(mPacket.Bytes, midiData, 0, mPacket.Length);
                        //The first four bits of the status byte tell MIDI what command
                        //The last four bits of the status byte tell MIDI what channel
                        byte StatusByte = midiData[0];
                        byte typeData = (byte)((StatusByte & 0xF0) >> 4);
                        byte channelData = (byte)(StatusByte & 0x0F);

                        //We should check to see if typeData is the clock or start or continue
                        //If we catch the clock, start, or continue then we execute


                        //-----------defines each midi byte---------------
                        byte midi_start = 0xfa;         // start byte
                        byte midi_stop = 0xfc;          // stop byte
                        byte midi_clock = 0xf8;         // clock byte
                        byte midi_continue = 0xfb;      // continue byte
                                                        //------------------------------------------------

                        if ((StatusByte == midi_start) || (StatusByte == midi_continue))
                        {
                            my_mod.FireModulation = true;
                            ClockCounter(my_mod);
                        }
                        if (StatusByte == midi_clock)
                        {
                            ClockCounter(my_mod);

                            if (my_mod.StepComma == 2)
                            {
                                my_mod.StepComma = 0;
                            }
                            else
                            {
                                my_mod.StepComma = my_mod.StepComma + 1;
                            }
                            StepSizeSetter(my_mod);

                        }
                    }
                }
            };

            ConnectExistingDevices();

            var session = MidiNetworkSession.DefaultSession;
            if (session != null)
            {
                session.Enabled = true;
                session.ConnectionPolicy = MidiNetworkConnectionPolicy.Anyone;
            }
        }

        void ConnectExistingDevices()
        {
            for (int i = 0; i < Midi.SourceCount; i++)
            {
                var code = inputPort.ConnectSource(MidiEndpoint.GetSource(i));
                if (code != MidiError.Ok)
                    Console.WriteLine("Failed to connect");
            }
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }



    }
}
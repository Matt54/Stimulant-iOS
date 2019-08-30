using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Globalization;
using UIKit;
using CoreMidi;
using CoreGraphics;
using MonoTouch.Dialog;
using System.ComponentModel;
using Foundation;

namespace Stimulant
{
    public partial class ViewController : UIViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {// Note: this .ctor should not contain any initialization logic.
        }

        // Declare MidiClient Object: The MidiClient class is used to communicate with the MIDI subsystem on MacOS and iOS
        // It exposes various events and creates input and output midi ports using CreateInputPort/CreateOutputPort methods
        MidiClient client;

        // Simply, the input and output port objects created by calling CreateInputPort/CreateOutputPort methods
        MidiPort outputPort, inputPort;

        // Declare MidiModulation object: MidiModulation class stores all the current modulation parameters
        // I worry that this is a poor way of doing this. It may not be ideal from a memory standpoint
        MidiModulation myMidiModulation = new MidiModulation();

        // Controls how fast the time-based modulation steps
        HighResolutionTimer timerHighRes;

        // Controls how often the random settings get applied when in automatic mode
        HighResolutionTimer timerAuto;



        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            //NSNotificationCenter.DefaultCenter.AddObserver((Foundation.NSString)"UIKeyboardWillShowNotification", KeyboardWillShow);
            //NSNotificationCenter.DefaultCenter.AddObserver((NSString)"UIKeyboardWillShowNotification", KeyboardWillShow);

            Midi.Restart(); //This stops the MIDI subsystems and forces it to be reinitialized
            SetupMidi();
            MakeHardware();
            MakeDevices();
            //ReloadDevices();


            timerHighRes = new HighResolutionTimer(100.0f);
            // UseHighPriorityThread = true, sets the execution thread 
            // to ThreadPriority.Highest.  It doesn't provide any precision gain
            // in most of the cases and may do things worse for other threads. 
            // It is suggested to do some studies before leaving it true
            timerHighRes.UseHighPriorityThread = false;
            timerHighRes.Elapsed += (s, e) =>
            {
                InvokeOnMainThread(() => {
                    if (myMidiModulation.IsRunning)
                    {
                        if (!(myMidiModulation.ModeNumber == 2))
                        {
                            myMidiModulation.ClockCounter();
                        }
                        else
                        {
                            myMidiModulation.FireModulation = true;
                        }
                    }
                });
            };
            timerHighRes.Start();

            timerAuto = new HighResolutionTimer(6300.0f);
            timerAuto.UseHighPriorityThread = false;
            timerAuto.Elapsed += (s, e) =>
            {
                InvokeOnMainThread(() => {
                    if (!myMidiModulation.SettingsOn)
                    {
                        if (myMidiModulation.ModeNumber == 2)
                        {
                            myMidiModulation.IsRandomRoll = true;
                            //labelPattern.Text = myMidiModulation.RandomRoll();

                        }
                        else
                        {
                            myMidiModulation.AutoCounter++;
                            if (myMidiModulation.AutoCounter > myMidiModulation.AutoCutoff)
                            {
                                myMidiModulation.AutoCounter = 0;
                                //myMidiModulation.PatternString = myMidiModulation.RandomRoll();
                                myMidiModulation.IsRandomRoll = true;
                            }
                        }
                    }
                });
            };

            //timerHighRes.Stop();    // by default Stop waits for thread.Join()
            // which, if called not from Elapsed subscribers,
            // would mean that all Elapsed subscribers
            // are finished when the Stop function exits 
            //timerHighRes.Stop(joinThread:false)   // Use if you don't care and don't want to wait

            UIHelper = false;

            LoadDisplay();
            myMidiModulation.ModeNumber = 2;
            ReadSlider(sliderRate.Value);

            this.textFieldBPM.ShouldReturn += (textField) => {
                textFieldBPM.ResignFirstResponder();
                return true;
            };

            myMidiModulation.PropertyChanged += (object s, PropertyChangedEventArgs e2) =>
            {
                switch (e2.PropertyName)
                {

                    case "Opposite":
                        {
                            if (myMidiModulation.Opposite)
                            {
                                buttonReverse.SetImage(UIImage.FromFile("graphicReverseButtonOn.png"), UIControlState.Normal);
                                labelMode.Text = "Opposite Direction";
                                labelDetails.Text = "Pattern Is Reversed";
                            }
                            else
                            {
                                buttonReverse.SetImage(UIImage.FromFile("graphicReverseButtonOff.png"), UIControlState.Normal);
                                ResetDisplay();
                            }
                            break;
                        }

                    case "PatternString":
                        {
                            InvokeOnMainThread(() =>
                            {
                                labelPattern.Text = myMidiModulation.PatternString;
                            });
                            break;
                        }

                    case "IsAuto":
                        {
                            if (myMidiModulation.IsAuto)
                            {

                                if (myMidiModulation.ModeNumber == 2 || myMidiModulation.ModeNumber == 3)
                                {
                                    timerAuto.Start();
                                    myMidiModulation.AutoCutoff = 64;
                                }
                                else
                                {
                                    myMidiModulation.AutoCutoff = (int)(64 * (1728 / 127));
                                }
                                buttonAuto.SetImage(UIImage.FromFile("graphicAutoButtonOn.png"), UIControlState.Normal);
                                buttonAuto.SetImage(UIImage.FromFile("graphicAutoButtonOn.png"), UIControlState.Highlighted);
                                buttonSettings.Enabled = true;
                                //buttonAR.Enabled = true;

                                labelMode.Text = " Automatic Mode ";
                                labelDetails.Text = "Randoms At A Set Rate";
                            }
                            else
                            {
                                timerAuto.Stop(joinThread: false);
                                buttonAuto.SetImage(UIImage.FromFile("graphicAutoButtonOff.png"), UIControlState.Normal);
                                buttonAuto.SetImage(UIImage.FromFile("graphicAutoButtonOff.png"), UIControlState.Highlighted);
                                buttonSettings.Enabled = false;
                                //buttonAR.Enabled = false;
                                if (myMidiModulation.SettingsOn)
                                {
                                    myMidiModulation.SettingsOn = false;
                                }
                                if (myMidiModulation.IsAR)
                                {
                                    myMidiModulation.IsAR = false;
                                }
                                ResetDisplay();
                            }
                            break;
                        }

                    case "IsRandomRoll":
                        {
                            if (myMidiModulation.IsRandomRoll)
                            {
                                InvokeOnMainThread(() =>
                                {
                                    if (myMidiModulation.IsAutoPattern)
                                    {
                                        labelPattern.Text = myMidiModulation.RandomRoll();
                                        segmentedPattern.SelectedSegment = myMidiModulation.PatternNumber - 1;
                                    }

                                    if (myMidiModulation.IsAutoRate)
                                    {
                                        sliderRate.Value = myMidiModulation.RandomNumber(1, 127);
                                        ReadSlider(sliderRate.Value);
                                    }

                                    if (myMidiModulation.IsAR)
                                    {
                                        rangeSlider.UpperValue = myMidiModulation.RandomNumber(1, 127);
                                        myMidiModulation.Maximum = (int)rangeSlider.UpperValue;
                                        rangeSlider.LowerValue = myMidiModulation.RandomNumber(1, myMidiModulation.Maximum);
                                        myMidiModulation.Minimum = (int)rangeSlider.LowerValue;
                                    }
                                    myMidiModulation.IsRandomRoll = false;
                                    //labelMode.Text = "Hello World";
                                });
                            }
                            break;
                        }

                    case "IsAR":
                        {
                            if (myMidiModulation.IsAR)
                            {
                                buttonAR.SetImage(UIImage.FromFile("graphicARButtonOn.png"), UIControlState.Normal);
                                labelMode.Text = "Automatic Range";
                                labelDetails.Text = "Randoms Change Range";
                                buttonRandom.Enabled = true;
                            }
                            else
                            {
                                buttonAR.SetImage(UIImage.FromFile("graphicARButtonOff.png"), UIControlState.Normal);
                                ResetDisplay();


                                if((!myMidiModulation.IsAR) && (!myMidiModulation.IsAutoPattern) && (!myMidiModulation.IsAR))
                                {
                                    buttonRandom.Enabled = false;
                                }

                            }
                            break;
                        }

                    case "IsAutoPattern":
                        {
                            if (myMidiModulation.IsAutoPattern)
                            {
                                buttonAutoPattern.SetImage(UIImage.FromFile("graphicAutoPatternButtonOn.png"), UIControlState.Normal);
                                labelMode.Text = "Automatic Pattern";
                                labelDetails.Text = "Randoms Change Pattern";
                                buttonRandom.Enabled = true;
                            }
                            else
                            {
                                buttonAutoPattern.SetImage(UIImage.FromFile("graphicAutoPatternButtonOff.png"), UIControlState.Normal);
                                ResetDisplay();

                                if ((!myMidiModulation.IsAR) && (!myMidiModulation.IsAutoPattern) && (!myMidiModulation.IsAR))
                                {
                                    buttonRandom.Enabled = false;
                                }
                            }
                            break;
                        }

                    case "IsAutoRate":
                        {
                            if (myMidiModulation.IsAutoRate)
                            {
                                buttonAutoRate.SetImage(UIImage.FromFile("graphicAutoRateButtonOn.png"), UIControlState.Normal);
                                labelMode.Text = "Automatic Rate";
                                labelDetails.Text = "Randoms Change Rate";
                                buttonRandom.Enabled = true;
                            }
                            else
                            {
                                buttonAutoRate.SetImage(UIImage.FromFile("graphicAutoRateButtonOff.png"), UIControlState.Normal);
                                ResetDisplay();

                                if ((!myMidiModulation.IsAR) && (!myMidiModulation.IsAutoPattern) && (!myMidiModulation.IsAR))
                                {
                                    buttonRandom.Enabled = false;
                                }
                            }
                            break;
                        }

                    case "IsRestartEachNote":
                        {
                            if (myMidiModulation.IsRestartEachNote)
                            {
                                buttonLocation.SetImage(UIImage.FromFile("graphicLocationButtonOn.png"), UIControlState.Normal);
                                sliderHidden.Hidden = false;
                                ReadHiddenSlider(sliderHidden.Value);
                                rangeSlider.Enabled = false;
                                labelRange.Text = "Starting Value: " + myMidiModulation.StartingLocation.ToString();

                                labelMode.Text = " Restart Pattern   ";
                                labelDetails.Text = "Begins At Starting Value";
                            }
                            else
                            {

                                buttonLocation.SetImage(UIImage.FromFile("graphicLocationButtonOff.png"), UIControlState.Normal);
                                sliderHidden.Hidden = true;
                                rangeSlider.Enabled = true;
                                labelRange.Text = "Modulation Range";
                                ResetDisplay();
                            }
                            break;
                        }

                    case "StartingLocation":
                        {
                            labelRange.Text = "Starting Value: " + myMidiModulation.StartingLocation.ToString();
                            break;
                        }

                    case "IsTriggerOnly":
                        {
                            if (myMidiModulation.IsTriggerOnly)
                            {
                                buttonTrigger.SetImage(UIImage.FromFile("graphicTriggerButtonOn.png"), UIControlState.Normal);
                                buttonTrigger.SetImage(UIImage.FromFile("graphicTriggerButtonOn.png"), UIControlState.Highlighted);
                                buttonLocation.Enabled = true;

                                labelMode.Text = " Note On Trigger   ";
                                labelDetails.Text = "Modulation When Playing";
                            }
                            else
                            {
                                buttonTrigger.SetImage(UIImage.FromFile("graphicTriggerButtonOff.png"), UIControlState.Normal);
                                buttonTrigger.SetImage(UIImage.FromFile("graphicTriggerButtonOff.png"), UIControlState.Highlighted);
                                myMidiModulation.NumOfNotesOn = 0;
                                myMidiModulation.IsNoteOn = false;
                                myMidiModulation.IsRestartEachNote = false;
                                buttonLocation.Enabled = false;
                                ResetDisplay();
                            }
                            break;
                        }

                    case "BPMOn":
                        {
                            if (myMidiModulation.BPMOn)
                            {
                                buttonBPM.SetImage(UIImage.FromFile("graphicBPMButtonOn.png"), UIControlState.Normal);
                                segmentedPattern.Hidden = true;
                                buttonPlus1.Hidden = false;
                                buttonPlus10.Hidden = false;
                                buttonMinus1.Hidden = false;
                                buttonMinus10.Hidden = false;
                                labelPattern.Text = "Current Tempo: " + myMidiModulation.BPM + "BPM";
                                buttonTap.Hidden = false;

                                labelMode.Text = "Tempo Adjustment";
                                labelDetails.Text = "Tap Or Use Arrows To Set";
                            }
                            else
                            {
                                buttonBPM.SetImage(UIImage.FromFile("graphicBPMButtonOff.png"), UIControlState.Normal);
                                segmentedPattern.Hidden = false;
                                buttonPlus1.Hidden = true;
                                buttonPlus10.Hidden = true;
                                buttonMinus1.Hidden = true;
                                buttonMinus10.Hidden = true;
                                ReadPattern(segmentedPattern.SelectedSegment);
                                buttonTap.Hidden = true;
                                ResetDisplay();
                            }
                            break;
                        }

                    case "BPM":
                        {
                            if (myMidiModulation.BPMOn)
                            {
                                labelPattern.Text = "Current Tempo: " + myMidiModulation.BPM + "BPM";
                                ReadSlider(sliderRate.Value);
                            }
                            break;
                        }

                    case "CCOn":
                        {
                            if (myMidiModulation.CCOn)
                            {
                                buttonCC.SetImage(UIImage.FromFile("graphicCCButtonOn.png"), UIControlState.Normal);
                                segmentedPattern.Hidden = true;
                                buttonPlus1.Hidden = false;
                                buttonPlus10.Hidden = false;
                                buttonMinus1.Hidden = false;
                                buttonMinus10.Hidden = false;
                                buttonTap.Hidden = false;
                                buttonTap.Enabled = false;
                                labelPattern.Text = "Current Channel: CC" + myMidiModulation.CCNumber;

                                labelMode.Text = "CC Number Setting";
                                labelDetails.Text = "Value Adjusted By Arrows";
                            }
                            else
                            {
                                buttonCC.SetImage(UIImage.FromFile("graphicCCButtonOff.png"), UIControlState.Normal);
                                segmentedPattern.Hidden = false;
                                buttonPlus1.Hidden = true;
                                buttonPlus10.Hidden = true;
                                buttonMinus1.Hidden = true;
                                buttonMinus10.Hidden = true;
                                buttonTap.Hidden = true;
                                buttonTap.Enabled = true;
                                ReadPattern(segmentedPattern.SelectedSegment);
                                ResetDisplay();
                            }
                            break;
                        }

                    case "CCNumber":
                        {
                            if (myMidiModulation.CCOn)
                            {
                                labelPattern.Text = "Current Channel: CC" + myMidiModulation.CCNumber;
                            }
                            break;
                        }

                    case "SettingsOn":
                        {
                            if (myMidiModulation.SettingsOn)
                            {
                                buttonSettings.SetImage(UIImage.FromFile("graphicSettingsButtonOn.png"), UIControlState.Normal);
                                myMidiModulation.RateRemember = (int)sliderRate.Value;

                                if (myMidiModulation.ModeNumber == 2)
                                {
                                    sliderRate.Value = (float)myMidiModulation.AutoCutoff;
                                }
                                else
                                {
                                    double tempVal;
                                    tempVal = myMidiModulation.AutoCutoff * 128 / 1728;
                                    sliderRate.Value = ((float)tempVal-3); //(oddly enough, this subtraction fixes a weird drifting bug..)
                                }
                                ReadSlider(sliderRate.Value);
                                labelMode.Text = "Auto Rate Setting";
                                labelDetails.Text = "Value Adjusted By Slider";
                            }
                            else
                            {
                                buttonSettings.SetImage(UIImage.FromFile("graphicSettingsButtonOff.png"), UIControlState.Normal);
                                sliderRate.Value = myMidiModulation.RateRemember;
                                ReadSlider(sliderRate.Value);
                                ResetDisplay();
                            }
                            break;
                        }

                    case "ModeNumber":
                        {
                            switch (myMidiModulation.ModeNumber)
                            {
                                case 1:
                                    timerHighRes.Stop(joinThread: false);
                                    //if (myMidiModulation.IsAuto)
                                    //{
                                    
                                    //}

                                    buttonMidi.SetImage(UIImage.FromFile("graphicMidiButtonOn.png"), UIControlState.Normal);
                                    buttonTime.SetImage(UIImage.FromFile("graphicTimeButtonOff.png"), UIControlState.Normal);
                                    buttonClock.SetImage(UIImage.FromFile("graphicClockButtonOff.png"), UIControlState.Normal);
                                    ReadSlider(sliderRate.Value);
                                    buttonBPM.Enabled = false;
                                    if (myMidiModulation.BPMOn)
                                    {
                                        myMidiModulation.BPMOn = false;
                                    }
                                    labelMode.Text = " Ext Clock Mode   ";
                                    labelDetails.Text = " Midi Clock Adjusts Rate ";
                                    //timerAuto.Stop(joinThread: false);
                                    if (myMidiModulation.IsAuto)
                                    {
                                        myMidiModulation.IsAuto = false;
                                        /*
                                        double tempVal;
                                        tempVal = myMidiModulation.AutoCutoff * 128 / 1728;
                                        sliderRate.Value = ((float)tempVal - 3); //(oddly enough, this subtraction fixes a weird drifting bug..)
                                        */
                                        //myMidiModulation.SettingsOn = true;
                                    }
                                    break;
                                case 2:
                                    
                                    timerHighRes.Start();
                                    buttonMidi.SetImage(UIImage.FromFile("graphicMidiButtonOff.png"), UIControlState.Normal);
                                    buttonTime.SetImage(UIImage.FromFile("graphicTimeButtonOn.png"), UIControlState.Normal);
                                    buttonClock.SetImage(UIImage.FromFile("graphicClockButtonOff.png"), UIControlState.Normal);
                                    ReadSlider(sliderRate.Value);
                                    buttonBPM.Enabled = false;
                                    
                                    if (myMidiModulation.BPMOn)
                                    {
                                        myMidiModulation.BPMOn = false;
                                    }
                                    labelMode.Text = "Free Timing Mode";
                                    labelDetails.Text = "Rate Based On Frequency";
                                    if (myMidiModulation.IsAuto)
                                    {
                                        //timerAuto.Start();
                                        myMidiModulation.IsAuto = false;
                                        //timerAuto.Start();
                                        //myMidiModulation.AutoCutoff = 64;
                                        //myMidiModulation.SettingsOn = true;
                                    }
                                    break;
                                case 3:
                                    //if (myMidiModulation.IsAuto)
                                    //{

                                    //}
                                    myMidiModulation.CutoffFactor = 1;
                                    myMidiModulation.ClockCutoff = 1;
                                    timerHighRes.Start();
                                    buttonMidi.SetImage(UIImage.FromFile("graphicMidiButtonOff.png"), UIControlState.Normal);
                                    buttonTime.SetImage(UIImage.FromFile("graphicTimeButtonOff.png"), UIControlState.Normal);
                                    buttonClock.SetImage(UIImage.FromFile("graphicClockButtonOn.png"), UIControlState.Normal);
                                    buttonBPM.Enabled = true;
                                    ReadSlider(sliderRate.Value);
                                    labelMode.Text = " Int Clock Mode   ";
                                    labelDetails.Text = "Current Clock Tempo = " + myMidiModulation.BPM.ToString();
                                    
                                    if (myMidiModulation.IsAuto)
                                    {
                                        myMidiModulation.IsAuto = false;
                                        /*timerAuto.Start();
                                        double tempVal;
                                        tempVal = myMidiModulation.AutoCutoff * 128 / 1728;
                                        sliderRate.Value = ((float)tempVal - 3);*/ //(oddly enough, this subtraction fixes a weird drifting bug..)
                                        //myMidiModulation.SettingsOn = true;
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        }

                    case "PatternNumber":
                        {

                            /*

                            if (myMidiModulation.IsRandomRoll == true)
                            {

                                sliderRate.Value = myMidiModulation.RandomNumber(1, 127);
                                segmentedPattern.SelectedSegment = myMidiModulation.PatternNumber - 1;

                                if (myMidiModulation.IsAR)
                                {
                                    rangeSlider.UpperValue = myMidiModulation.RandomNumber(1, 127);
                                    myMidiModulation.Maximum = (int)rangeSlider.UpperValue;
                                    rangeSlider.LowerValue = myMidiModulation.RandomNumber(1, myMidiModulation.Maximum);
                                    myMidiModulation.Minimum = (int)rangeSlider.LowerValue;
                                }
                                myMidiModulation.IsRandomRoll = false;
                            }
                            */

                            //if (myMidiModulation.ModeNumber == 2)
                            //{
                            ReadSlider(sliderRate.Value);
                            //}

                            switch (myMidiModulation.PatternNumber)
                            {
                                case 1:
                                case 4:
                                case 7:
                                case 8:
                                    myMidiModulation.Opposite = false;
                                    //buttonReverse.Hidden = true;
                                    buttonReverse.Enabled = false;
                                    buttonReverse.SetImage(UIImage.FromFile("graphicReverseButtonOff.png"), UIControlState.Normal);
                                    break;
                                default:
                                    buttonReverse.Hidden = false;
                                    buttonReverse.Enabled = true;
                                    break;
                            }


                            if (myMidiModulation.PatternNumber > 6)
                            {
                                myMidiModulation.IsRestartEachNote = false;
                                buttonLocation.Enabled = false;
                            }
                            else
                            {
                                if (myMidiModulation.IsTriggerOnly)
                                {
                                    buttonLocation.Enabled = true;
                                }
                            }

                            break;
                        }

                    case "FireModulation":
                        InvokeOnMainThread(() => {
                            if (myMidiModulation.FireModulation)
                            {
                                if (!myMidiModulation.IsTriggerOnly || (myMidiModulation.IsTriggerOnly && myMidiModulation.IsNoteOn))
                                {
                                    myMidiModulation.UpdateValue();
                                    SendMIDI(0xB0, (byte)myMidiModulation.CCNumber, (byte)myMidiModulation.CurrentCC);
                                    myMidiModulation.ClockCount = 0;

                                    var pi_mult = (myMidiModulation.CurrentCC * 2.0f / 127);

                                    //Momentarily remove from superview so that it can be on top of progress bar
                                    myCircularProgressBar.RemoveFromSuperview();
                                    //Declare progress bar object (Instantiating my CircularProgressBar class)
                                    myCircularProgressBar = new CircularProgressBar(progressSize, lineWidth, pi_mult, barColor);
                                    buttonOnOff.RemoveFromSuperview();

                                    //Add Views
                                    View.AddSubview(myCircularProgressBar);
                                    View.AddSubview(buttonOnOff);

                                    myMidiModulation.FireModulation = false;
                                }
                            }
                        });
                        break;
                }
            };

            myMidiModulation.ModeNumber = 2;
        }

        private void ResetDisplay()
        {
            switch (myMidiModulation.ModeNumber)
            {
                case 1:
                    labelMode.Text = "  Ext Clock Mode  ";
                    labelDetails.Text = " Midi Clock Adjusts Rate ";
                    break;
                case 2:
                    labelMode.Text = "Free Timing Mode";
                    labelDetails.Text = "Rate Based On Frequency";
                    break;
                case 3:
                    labelMode.Text = " Int Clock Mode ";
                    labelDetails.Text = "Current Clock Tempo = " + myMidiModulation.BPM.ToString();
                    break;
            }
        }

        private void HandlePlus1TouchDown(object sender, System.EventArgs e)
        {
            if (myMidiModulation.CCOn)
            {
                myMidiModulation.UpdateCCNumber(1);
            }
            else
            {
                myMidiModulation.UpdateBPM(1);
            }
        }

        private void HandlePlus10TouchDown(object sender, System.EventArgs e)
        {
            if (myMidiModulation.CCOn)
            {
                myMidiModulation.UpdateCCNumber(10);
            }
            else
            {
                myMidiModulation.UpdateBPM(10);
            }
        }

        private void HandleMinus1TouchDown(object sender, System.EventArgs e)
        {
            if (myMidiModulation.CCOn)
            {
                myMidiModulation.UpdateCCNumber(-1);
            }
            else
            {
                myMidiModulation.UpdateBPM(-1);
            }
        }

        private void HandleMinus10TouchDown(object sender, System.EventArgs e)
        {
            if (myMidiModulation.CCOn)
            {
                myMidiModulation.UpdateCCNumber(-10);
            }
            else
            {
                myMidiModulation.UpdateBPM(-10);
            }
        }




        partial void RateSliderChange(UISlider sender) { }
        partial void StartButton_TouchUpInside(UIButton sender) { }

        protected void HandleTouchDown(object sender, System.EventArgs e)
        {
            PowerPushed();
        }

        protected void HandleTouchUpInside(object sender, System.EventArgs e)
        {
            FlipPower();
        }

        private void FlipPower()
        {
            myCircularProgressBar.Hidden = false;
            buttonOnOff.Frame = bigStartSize;

            //Declare color object (Instantiating the UIColor class)
            //UIColor barColor = UIColor.FromRGB(0, 255, 0);

            //Control Logic: switches between program ON and program OFF states when button is pressed
            if (UIHelper)
            {
                buttonOnOff.SetImage(UIImage.FromFile("graphicPowerButtonOff.png"), UIControlState.Normal);
                buttonOnOff.SetImage(UIImage.FromFile("graphicPowerButtonOn.png"), UIControlState.Highlighted);
                myMidiModulation.IsRunning = false;
                var pi_mult = (myMidiModulation.CurrentCC * 2.0f / 127);
                myCircularProgressBar = new CircularProgressBar(progressSize, lineWidth, pi_mult, barColor);

                UIHelper = false;
            }
            else
            {
                buttonOnOff.SetImage(UIImage.FromFile("graphicPowerButtonOn.png"), UIControlState.Normal);
                buttonOnOff.SetImage(UIImage.FromFile("graphicPowerButtonOff.png"), UIControlState.Highlighted);
                myMidiModulation.IsRunning = true;
            }

            //Add Views
            View.AddSubview(myCircularProgressBar);
            View.AddSubview(buttonOnOff);
        }

        private void PowerPushed()
        {
            if (myMidiModulation.IsRunning)
            {
                myMidiModulation.IsRunning = false;
                UIHelper = true;
                myCircularProgressBar.RemoveFromSuperview();
            }
            myCircularProgressBar.Hidden = true;
            buttonOnOff.Frame = smallStartSize;
        }

        protected void HandleLocationTouchDown(object sender, System.EventArgs e)
        {
            myMidiModulation.RestartToggle();
        }

        protected void HandleHiddenSliderChange(object sender, System.EventArgs e)
        {
            var myObject = (UISlider)sender;
            ReadHiddenSlider(myObject.Value);
        }

        void ReadHiddenSlider(float sliderValue)
        {
            myMidiModulation.StartingLocation = (int)sliderValue;
        }

        protected void HandleReverseTouchDown(object sender, System.EventArgs e)
        {
            myMidiModulation.ReversePattern();
        }

        protected void HandleCCTouchDown(object sender, System.EventArgs e)
        {
            myMidiModulation.CCToggle();
        }

        private void HandleBPMTouchDown(object sender, System.EventArgs e)
        {
            myMidiModulation.BPMToggle();
        }

        private void HandleAutoPatternTouchDown(object sender, System.EventArgs e)
        {
            myMidiModulation.AutoPatternToggle();
        }

        private void HandleAutoRateTouchDown(object sender, System.EventArgs e)
        {
            myMidiModulation.AutoRateToggle();
        }

        protected void HandleSettingsTouchDown(object sender, System.EventArgs e)
        {
            myMidiModulation.SettingsToggle();
        }

        protected void HandleARTouchDown(object sender, System.EventArgs e)
        {
            myMidiModulation.ARToggle();
        }

        protected void HandleMidiTouchDown(object sender, System.EventArgs e)
        {
            myMidiModulation.ModeNumber = 1;
        }
        protected void HandleTimeTouchDown(object sender, System.EventArgs e)
        {
            myMidiModulation.ModeNumber = 2;
        }

        protected void HandleRandomTouchDown(object sender, System.EventArgs e)
        {
            //labelPattern.Text = myMidiModulation.RandomRoll();
            //myMidiModulation.IsRandomRoll = true;
            myMidiModulation.RandomToggle();
        }

        protected void HandleAutoTouchDown(object sender, System.EventArgs e)
        {
            myMidiModulation.AutoToggle();
        }

        protected void HandleClockTouchDown(object sender, System.EventArgs e)
        {
            myMidiModulation.ClockToggle();
        }

        protected void HandleTapTouchDown(object sender, System.EventArgs e)
        {
            myMidiModulation.BPMTap();
        }

        protected void HandleTriggerTouchDown(object sender, System.EventArgs e)
        {
            myMidiModulation.TriggerToggle();
        }

        protected void HandleRateSliderChange(object sender, System.EventArgs e)
        {
            var myObject = (UISlider)sender;
            ReadSlider(myObject.Value);
        }

        void ReadSlider(float sliderValue)
        {
            if (myMidiModulation.ModeNumber == 2)
            {
                //TIME
                if (myMidiModulation.SettingsOn)
                {
                    myMidiModulation.AutoCutoff = (int)sliderValue;
                    timerAuto.Interval = (float)Math.Round(((128 - sliderValue) * 100), 0);
                    labelRate.Text = "Randoms in: " + timerAuto.Interval + " ms";
                }
                else
                {
                    myMidiModulation.TimeSet(sliderValue); // Determines StepSize from a sliderValue and PatternNumber
                    timerHighRes.Interval = ValueToTimeInterval(sliderValue); // Magic function to get time interval from sliderValue
                    labelRate.Text = myMidiModulation.TimeIntervalToFrequency(timerHighRes.Interval); // Convert time interval to frequency for label display
                }
            }
            else
            {

                string displayText = "";
                // Conditionals determine the correct rate based on sliderValue
                if (sliderValue >= (128 * 17 / 18))
                { // 32 note triples
                    displayText = "1/48";
                    myMidiModulation.RateCatch = 18;
                }
                else if (sliderValue >= (128 * 16 / 18))
                { // 32 notes
                    displayText = "1/32";
                    myMidiModulation.RateCatch = 17;
                }
                else if (sliderValue >= (128 * 15 / 18))
                {  // sixteenth note triples
                    displayText = "1/24";
                    myMidiModulation.RateCatch = 16;
                }
                else if (sliderValue >= (128 * 14 / 18))
                { // sixteenth notes
                    displayText = "1/16";
                    myMidiModulation.RateCatch = 15;
                }
                else if (sliderValue >= (128 * 13 / 18))
                {  // eighth note triples
                    displayText = "1/12";
                    myMidiModulation.RateCatch = 14;
                }
                else if (sliderValue >= (128 * 12 / 18))
                {  // eighth notes
                    displayText = "1/8 ";
                    myMidiModulation.RateCatch = 13;
                }
                else if (sliderValue >= (128 * 11 / 18))
                {  // quarter note triples
                    displayText = "1/6 ";
                    myMidiModulation.RateCatch = 12;
                }
                else if (sliderValue >= (128 * 10 / 18))
                {  // quarter notes
                    displayText = "1/4 ";
                    myMidiModulation.RateCatch = 11;
                }
                else if (sliderValue >= (128 * 9 / 18))
                {  // half note triples
                    displayText = "1/3 ";
                    myMidiModulation.RateCatch = 10;
                }
                else if (sliderValue >= (128 * 8 / 18))
                {  // half note
                    displayText = "1/2 ";
                    myMidiModulation.RateCatch = 9;
                }
                else if (sliderValue >= (128 * 7 / 18))
                { // whole note triples
                    displayText = "3/4 ";
                    myMidiModulation.RateCatch = 8;
                }
                else if (sliderValue >= (128 * 6 / 18))
                { // whole note
                    displayText = "1/1 ";
                    myMidiModulation.RateCatch = 7;
                }
                else if (sliderValue >= (128 * 5 / 18))
                { // 2 bar triples
                    displayText = "3/2 ";
                    myMidiModulation.RateCatch = 6;
                }
                else if (sliderValue >= (128 * 4 / 18))
                { // 2 bars
                    displayText = "2/1 ";
                    myMidiModulation.RateCatch = 5;
                }
                else if (sliderValue >= (128 * 3 / 18))
                { // 4 bar triples
                    displayText = "3/1 ";
                    myMidiModulation.RateCatch = 4;
                }
                else if (sliderValue >= (128 * 2 / 18))
                { // 4 bar
                    displayText = "4/1 ";
                    myMidiModulation.RateCatch = 3;
                }
                else if (sliderValue >= (128 * 1 / 18))
                { // 4 bar
                    displayText = "6/1 ";
                    myMidiModulation.RateCatch = 2;
                }
                else if (sliderValue < 7)
                { // 4 bar
                    displayText = "8/1 ";
                    myMidiModulation.RateCatch = 1;
                }

                if (!myMidiModulation.SettingsOn)
                {
                    myMidiModulation.CheckCutoff();
                    /*
                    if (myMidiModulation.RateCatch > 5)
                    {
                        myMidiModulation.ClockCutoff = 1 * myMidiModulation.CutoffFactor;
                    }
                    else if (myMidiModulation.RateCatch > 3)
                    {
                        myMidiModulation.ClockCutoff = 2 * myMidiModulation.CutoffFactor;
                    }
                    else if (myMidiModulation.RateCatch > 1)
                    {
                        myMidiModulation.ClockCutoff = 4 * myMidiModulation.CutoffFactor;
                    }
                    else
                    {
                        myMidiModulation.ClockCutoff = 8 * myMidiModulation.CutoffFactor;
                    }
                    */
                }

                if (myMidiModulation.SettingsOn)
                {
                    myMidiModulation.AutoCutoff = myMidiModulation.RateCatch * 24 * 4;
                    timerAuto.Interval = (float)(60000 / myMidiModulation.BPM / 24); // quarter notes
                    labelRate.Text = "Randoms in: " + myMidiModulation.RateCatch + " Beats";
                }
                else if (myMidiModulation.ModeNumber == 1)
                {
                    //MIDI
                    myMidiModulation.StepSizeSetter();
                    //EXT Clock Sync
                    labelRate.Text = "Ext. Clock Sync: " + displayText;
                }
                else
                {
                    //myMidiModulation.TimeSet(sliderValue); // Determines StepSize that will prevent too high of a screen refresh
                    timerHighRes.Interval = BeatsPerMinuteIntoMilliSeconds((float)myMidiModulation.BPM, myMidiModulation.RateCatch);
                    //INT Clock Sync
                    labelRate.Text = "Int. Clock Sync: " + displayText;
                }

            }
        }

        public float ValueToTimeInterval(float value)
        {
            float timeInterval;

            switch (myMidiModulation.PatternNumber)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    timeInterval = (float)((double)((((double)myMidiModulation.StepSize) / 2) * (Math.Round(-15.6631 + (1561.999 + 17.55931) / (1 + Math.Pow((value / 13.37739), 2.002958)), 3))));
                    break;
                case 7:
                case 8:
                    timeInterval = (float)((double)((((double)myMidiModulation.StepSize) / 2) * (Math.Round(-3933.384 + (5000.008 + 3933.384) / (1 + Math.Pow((value / 56.13086), 0.2707651)), 3))));
                    break;
                default:
                    timeInterval = (float)((double)((((double)myMidiModulation.StepSize) / 2) * (Math.Round(-15.6631 + (1561.999 + 17.55931) / (1 + Math.Pow((value / 13.37739), 2.002958)), 3))));
                    break;
            }
            return timeInterval;
        }

        public float BeatsPerMinuteIntoMilliSeconds(float bpm, int rateCatch)
        {
            float myTimeInterval; // milliseconds
            double intervalMultiplier;

            // 1 beat = 1 quarter note
            // 1 minute = 60 seconds = 60,000 milliseconds
            // Quarter note duration (ms) = 60,000 / bpm

            switch (rateCatch)
            {
                case 1: // 8/1
                    intervalMultiplier = 32;
                    break;
                case 2: // 6/1
                    intervalMultiplier = 21.33;
                    break;
                case 3: // 4/1
                    intervalMultiplier = 16;
                    break;
                case 4: // 3/1
                    intervalMultiplier = 10.667; //16 * (2 / 3);
                    break;
                case 5: // 2/1
                    intervalMultiplier = 8;
                    break;
                case 6: // 3/2
                    intervalMultiplier = 5.333;// 8 * (2 / 3);
                    break;
                case 7:
                    intervalMultiplier = 4;
                    break;
                case 8:
                    intervalMultiplier = 2.667;// 4 * (2 / 3);
                    break;
                case 9:
                    intervalMultiplier = 2;
                    break;
                case 10:
                    intervalMultiplier = 1.333;//2 * (2 / 3);
                    break;
                case 11:
                    intervalMultiplier = 1;
                    break;
                case 12:
                    intervalMultiplier = 0.667;// 1/6;
                    break;
                case 13:
                    intervalMultiplier = 0.5;
                    break;
                case 14:
                    intervalMultiplier = 0.333;// 1 / 3;
                    break;
                case 15:
                    intervalMultiplier = 0.25;
                    break;
                case 16:
                    intervalMultiplier = 0.167;// 1 / 6;
                    break;
                case 17:
                    intervalMultiplier = 0.125;
                    break;
                case 18:
                    intervalMultiplier = 0.083;// 1 / 12;
                    break;
                default:
                    intervalMultiplier = 1;
                    break;
            }

            // we need to determine the step size that we should use to keep the screen refresh under 60Hz
            // 60000 / bpm * intervalMultiplier / StepsUntilFullModulation cannot cause an interval that exceeds 60Hz
            // assume bpm is 160 (this will be our max value)
            int fullModSteps;
            if (myMidiModulation.PatternNumber > 2)
            {
                myMidiModulation.StepSize = 8;
                fullModSteps = 8;

                /*
                if (myMidiModulation.PatternNumber > 3)
                {
                    
                }
                else
                {
                    myMidiModulation.StepSize = 1;
                    fullModSteps = 128;
                }
                */
                
                if (myMidiModulation.PatternNumber > 6)
                {
                    myMidiModulation.StepSize = 1;
                    fullModSteps = 4;
                }
                
            }
            else
            {
                switch (rateCatch)
                {
                    case 1: //added later on
                    case 2: //added later on
                    case 3:
                    case 4:
                    case 5:
                        //step size = 1
                        myMidiModulation.StepSize = 1;
                        //128 steps
                        fullModSteps = 128;
                        break;
                    case 6:
                    case 7:
                        //step size = 2
                        myMidiModulation.StepSize = 2;
                        //64 steps
                        fullModSteps = 64;
                        break;
                    case 8:
                    case 9:
                        //step size = 4
                        myMidiModulation.StepSize = 4;
                        //32 steps
                        fullModSteps = 32;
                        break;
                    case 10:
                    case 11:
                        //step size = 8
                        myMidiModulation.StepSize = 8;
                        //16 steps
                        fullModSteps = 16;
                        break;
                    case 12:
                    case 13:
                        //step size = 16
                        myMidiModulation.StepSize = 16;
                        //8 steps
                        fullModSteps = 8;
                        break;
                    case 14:
                    case 15:
                        //step size = 32
                        myMidiModulation.StepSize = 32;
                        //4 steps
                        fullModSteps = 4;
                        break;
                    case 16:
                    case 17:
                    case 18:
                        //step size = 64
                        myMidiModulation.StepSize = 64;
                        //2 steps
                        fullModSteps = 2;
                        break;
                    default:
                        //step size = 1
                        myMidiModulation.StepSize = 1;
                        fullModSteps = 128;
                        break;
                }
            }

            //myTimeInterval = (float)Math.Round(60000.0f / bpm * (float)Math.Round(intervalMultiplier, 2) / (float)fullModSteps,2);
            myTimeInterval = (float)Math.Round((double)60000 / (double)bpm * (double)intervalMultiplier / (double)fullModSteps, 3);
            return myTimeInterval;
        }

        //partial void ModeNumChanged(UISegmentedControl sender) { }

        protected void HandlePatternSegmentChange(object sender, System.EventArgs e)
        {
            var seg = sender as UISegmentedControl;
            //var index = seg.SelectedSegment;
            //ReadPattern(index);
            ReadPattern(seg.SelectedSegment);
        }

        void ReadPattern(nint patternIndex)
        {
            string labelText = myMidiModulation.UpdatePattern(patternIndex);
            labelPattern.Text = labelText;
        }



        /*
        private void ProgramChange()
        {
            var index = segmentedPattern.SelectedSegment;
            string labelText = myMidiModulation.UpdatePattern(index);
            labelPattern.Text = labelText;
        }
        */


        partial void pnumChange(UISegmentedControl sender) { }

        void SendMIDI(byte type, byte channel, byte value)
        {

            for (int i = 0; i < Midi.DestinationCount; i++)
            {
                var endpoint = MidiEndpoint.GetDestination(i);
                outputPort.Send(endpoint, new MidiPacket[] { new MidiPacket(0, new byte[] { type, channel, value }) });

                //outputPort.Send(endpoint, new MidiPacket[] { new MidiPacket(0, new byte[] { 0xB0, (byte)(myMidiModulation.CCNumber), ccByte }) });

                //var ccVal = (byte)(rand.Next () % 127);
                // play ccVal then turn off after 300 miliseconds
                /*
                outputPort.Send (endpoint, new MidiPacket [] { new MidiPacket (0, new byte [] { 0x90, ccVal, 127 }) });
                Thread.Sleep (300);
                outputPort.Send (endpoint, new MidiPacket [] { new MidiPacket (0, new byte [] { 0x80, ccVal, 0 }) });
                */
            }
        }

        // I don't think this is required
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

        // I don't think this is required
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

        //I don't think this is required
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


        //I don't think this is required
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

        void SetupMidi()
        {
            client = new MidiClient("Stimulant iOS MIDI Client");
            client.ObjectAdded += delegate (object sender, ObjectAddedOrRemovedEventArgs e) {
            };
            client.ObjectAdded += delegate {
            };
            client.ObjectRemoved += delegate {
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

            outputPort = client.CreateOutputPort("Stimulant iOS Output Port");
            inputPort = client.CreateInputPort("Stimulant iOS Input Port");

            inputPort.MessageReceived += delegate (object sender, MidiPacketsEventArgs e) {
                foreach (MidiPacket mPacket in e.Packets)
                {
                    //if (myMidiModulation.ModeNumber == 1)
                    //{
                    var midiData = new byte[mPacket.Length];
                    Marshal.Copy(mPacket.Bytes, midiData, 0, mPacket.Length);
                    //The first four bits of the status byte tell MIDI what command
                    //The last four bits of the status byte tell MIDI what channel
                    byte StatusByte = midiData[0];
                    byte typeData = (byte)((StatusByte & 0xF0) >> 4);
                    byte channelData = (byte)(StatusByte & 0x0F);

                    //We should check to see if typeData is clock/start/continue/stop/note on/note off


                    //-----------defines each midi byte---------------
                    byte midi_start = 0xfa;         // start byte
                    byte midi_stop = 0xfc;          // stop byte
                    byte midi_clock = 0xf8;         // clock byte
                    byte midi_continue = 0xfb;      // continue byte
                    byte midi_note_on = 0x90;         // note on
                    byte midi_note_off = 0x80;         // note off
                                                       //------------------------------------------------


                    if ((StatusByte == midi_start) || (StatusByte == midi_continue))
                    {
                        if (!myMidiModulation.IsRunning)
                        {
                            InvokeOnMainThread(() => {
                                PowerPushed();
                                FlipPower();
                            });
                        }
                        myMidiModulation.FireModulation = true; //I'm not sure if we should be firing one off at the start here
                    }

                    if (StatusByte == midi_clock)
                    {
                        if (myMidiModulation.ModeNumber == 1)
                        { 
                            myMidiModulation.ClockCounter();
                            if (myMidiModulation.StepComma == 2)
                            {
                                myMidiModulation.StepComma = 0;
                            }
                            else
                            {
                                myMidiModulation.StepComma = myMidiModulation.StepComma + 1;
                            }
                            myMidiModulation.StepSizeSetter();
                        }

                    }

                    if (StatusByte == midi_stop)
                    {
                        if (myMidiModulation.IsRunning)
                        {
                            InvokeOnMainThread(() => {
                                PowerPushed();
                                FlipPower();
                            });
                        }
                    }

                    if (myMidiModulation.IsTriggerOnly)
                    {
                        if (StatusByte == midi_note_on)
                        {
                            // handle note on
                            myMidiModulation.IsNoteOn = true;
                            myMidiModulation.NumOfNotesOn += 1;

                            // Restart first if in restart mode
                            if (myMidiModulation.IsRestartEachNote)
                            {
                                myMidiModulation.CurrentCC = myMidiModulation.StartingLocation;

                                if (myMidiModulation.PatternNumber == 4)
                                {
                                    myMidiModulation.EveryOther = false;
                                }
                                else if (myMidiModulation.PatternNumber == 5 || myMidiModulation.PatternNumber == 6)
                                {
                                    myMidiModulation.EveryOther = true;
                                }
                            }
                        }
                        if (StatusByte == midi_note_off)
                        {

                            if (myMidiModulation.NumOfNotesOn > 0)
                            {
                                myMidiModulation.NumOfNotesOn -= 1;
                            }
                            if (myMidiModulation.NumOfNotesOn < 1)
                            {
                                // handle note off
                                myMidiModulation.IsNoteOn = false;
                                myMidiModulation.NumOfNotesOn = 0;
                            }
                        }
                    }
                    //}
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
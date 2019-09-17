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

        // Declare MidiModulation object: MidiModulation class stores all the current modulation parameters
        // I worry that this is a poor way of doing this. It may not be ideal from a memory standpoint
        MidiModulation myMidiModulation = new MidiModulation();

        //Scenes myScenes = new Scenes();
        Scene[] sceneArray = new Scene[8];


        // Controls how fast the time-based modulation steps
        HighResolutionTimer timerHighRes;

        // Controls how often the random settings get applied when in automatic mode
        HighResolutionTimer timerAuto;

        HighResolutionTimer timerArrangement;


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


            timerArrangement = new HighResolutionTimer(1000.0f);
            timerArrangement.UseHighPriorityThread = false;
            timerArrangement.Elapsed += (s, e) =>
            {
                InvokeOnMainThread(() =>
                {
                    MoveToNextScene();
                    /*
                    int currentSceneRunning=0;
                    //Do stuff here
                    for (int ii = 0; ii < 8; ii++)
                    {
                        if (sceneArray[ii].IsRunning)
                        {
                            currentSceneRunning = ii;
                        }
                    }

                    sceneArray[currentSceneRunning].IsRunning = false;
                    
                    if (currentSceneRunning < myMidiModulation.MaxScene)
                    {
                        sceneArray[currentSceneRunning + 1].IsRunning = true;
                    }
                    else
                    {
                        sceneArray[myMidiModulation.MinScene].IsRunning = true;
                    }
                    */
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

            for (int ii = 0; ii < 8; ii++)
            {
                sceneArray[ii] = new Scene();
            }

            sceneArray[0].IsSelected = true;
            sceneArray[0].IsRunning = true;

            for (int ii = 0; ii < 8; ii++)
            {
                myMidiModulation.setParameters(sliderRate.Value, sceneArray[ii]);
                UpdateSceneGraphic();
            }

            


            sceneArray[0].PropertyChanged += (object s, PropertyChangedEventArgs e2) =>
            {
                CombinedSceneProperty(e2.PropertyName,0);
            };
            sceneArray[1].PropertyChanged += (object s, PropertyChangedEventArgs e2) =>
            {
                CombinedSceneProperty(e2.PropertyName, 1);
            };
            sceneArray[2].PropertyChanged += (object s, PropertyChangedEventArgs e2) =>
            {
                CombinedSceneProperty(e2.PropertyName, 2);
            };
            sceneArray[3].PropertyChanged += (object s, PropertyChangedEventArgs e2) =>
            {
                CombinedSceneProperty(e2.PropertyName, 3);
            };
            sceneArray[4].PropertyChanged += (object s, PropertyChangedEventArgs e2) =>
            {
                CombinedSceneProperty(e2.PropertyName, 4);
            };
            sceneArray[5].PropertyChanged += (object s, PropertyChangedEventArgs e2) =>
            {
                CombinedSceneProperty(e2.PropertyName, 5);
            };
            sceneArray[6].PropertyChanged += (object s, PropertyChangedEventArgs e2) =>
            {
                CombinedSceneProperty(e2.PropertyName, 6);
            };
            sceneArray[7].PropertyChanged += (object s, PropertyChangedEventArgs e2) =>
            {
                CombinedSceneProperty(e2.PropertyName, 7);
            };


            myMidiModulation.PropertyChanged += (object s, PropertyChangedEventArgs e2) =>
            {
                switch (e2.PropertyName)
                {
                    case "IsSceneMode":
                        {
                            if (myMidiModulation.IsSceneMode)
                            {
                                //myCircularProgressBar.RemoveFromSuperview();
                                //buttonOnOff.RemoveFromSuperview();
                                var pi_mult = (myMidiModulation.CurrentCC * 2.0f / 127);

                                //Momentarily remove from superview so that the start button can be on top of progress bar
                                myCircularProgressBar.RemoveFromSuperview();
                                //Declare progress bar object (Instantiating my CircularProgressBar class)
                                myCircularProgressBar = new CircularProgressBar(C_progressSceneSize, C_lineWidthSmall, pi_mult, barColor);
                                buttonOnOff.RemoveFromSuperview();
                                buttonOnOff.Frame = sceneStartSize;

                                //Add Views
                                View.AddSubview(myCircularProgressBar);
                                View.AddSubview(buttonOnOff);
                                buttonRandom.Hidden = true;
                                buttonReverse.Frame = frameReverseScene;

                                if (myMidiModulation.Opposite)
                                {
                                    buttonReverse.SetImage(UIImage.FromFile("graphicReverseSceneButtonOn.png"), UIControlState.Normal);
                                }
                                else
                                {
                                    buttonReverse.SetImage(UIImage.FromFile("graphicReverseSceneButtonOff.png"), UIControlState.Normal);
                                }
                                buttonReverse.SetImage(UIImage.FromFile("graphicReverseSceneButtonOff.png"), UIControlState.Highlighted);
                                buttonReverse.SetImage(UIImage.FromFile("graphicReverseSceneButtonDisabled.png"), UIControlState.Disabled);



                                buttonScenes.SetImage(UIImage.FromFile("graphicScenesButtonOn"), UIControlState.Normal);

                                /*
                                for (int ii = 0; ii < 8; ii++)
                                {
                                    myMidiModulation.setParameters(sliderRate.Value, sceneArray[ii]);
                                    UpdateSceneGraphic();
                                }
                                */

                                for (int ii = 0; ii < 8; ii++)
                                {
                                    View.AddSubview(buttonArray[ii]);
                                    if (sceneArray[ii].IsSelected)
                                    {
                                        myMidiModulation.setParameters(sliderRate.Value, sceneArray[ii]);
                                    }
                                }
                                View.AddSubview(buttonArrange);
                                
                                

                                //View.AddSubview(myHorizontalProgressBar);

                                sliderCC.Hidden = false;
                                //sceneArray[0].IsSelected = true;
                                UpdateSceneGraphic();
                                //UpdateSceneGraphic();

                                labelMode.Text = " Scene Select ";
                                labelDetails.Text = "Click to Load Settings";



                            }
                            else
                            {
                                buttonArrange.RemoveFromSuperview();
                                myMidiModulation.IsArrangementMode = false;
                                buttonOnOff.Frame = bigStartSize;
                                

                                myCircularProgressBar.RemoveFromSuperview();
                                var pi_mult = (myMidiModulation.CurrentCC * 2.0f / 127);
                                myCircularProgressBar = new CircularProgressBar(C_progressSize, C_lineWidth, pi_mult, barColor);

                                View.AddSubview(myCircularProgressBar);
                                View.AddSubview(buttonOnOff);
                                buttonRandom.Hidden = false;
                                buttonReverse.Frame = frameReverseOrig;


                                if (myMidiModulation.Opposite)
                                {
                                    buttonReverse.SetImage(UIImage.FromFile("graphicReverseButtonOn.png"), UIControlState.Normal);
                                }
                                else
                                {
                                    buttonReverse.SetImage(UIImage.FromFile("graphicReverseButtonOff.png"), UIControlState.Normal);
                                }

                                buttonReverse.SetImage(UIImage.FromFile("graphicReverseButtonOff.png"), UIControlState.Highlighted);
                                buttonReverse.SetImage(UIImage.FromFile("graphicReverseButtonDisabled.png"), UIControlState.Disabled);

                                //myHorizontalProgressBar.RemoveFromSuperview();


                                sliderCC.Hidden = true;

                                buttonScenes.SetImage(UIImage.FromFile("graphicScenesButtonOff"), UIControlState.Normal);
                                for (int ii = 0; ii < 8; ii++)
                                {
                                    buttonArray[ii].RemoveFromSuperview();
                                }

                                ResetDisplay();
                            }
                            break;
                        }

                    case "IsArrangementMode":
                        {
                            if (myMidiModulation.IsArrangementMode)
                            {
                                UpdateSceneRunning();
                                View.AddSubview(rangeScenesSlider);
                                buttonArrange.SetImage(UIImage.FromFile("graphicArrangeButtonOn"), UIControlState.Normal);

                                if (!(myMidiModulation.ModeNumber == 1))
                                {
                                    timerArrangement.Start();
                                }
                            }
                            else
                            {
                                myRunningSymbol.RemoveFromSuperview();
                                rangeScenesSlider.RemoveFromSuperview();
                                buttonArrange.SetImage(UIImage.FromFile("graphicArrangeButtonOff"), UIControlState.Normal);
                                if (timerArrangement.IsRunning)
                                {
                                    timerArrangement.Stop();
                                }
                            }
                            break;
                        }

                    case "SceneMove":
                        {
                            if (myMidiModulation.SceneMove)
                            {
                                MoveToNextScene();
                                myMidiModulation.SceneMove = false;
                            }
                            break;
                        }

                    case "Opposite":
                        {
                            if (myMidiModulation.Opposite)
                            {
                                if (myMidiModulation.IsSceneMode)
                                {
                                    buttonReverse.SetImage(UIImage.FromFile("graphicReverseSceneButtonOn.png"), UIControlState.Normal);
                                }
                                else
                                {
                                    buttonReverse.SetImage(UIImage.FromFile("graphicReverseButtonOn.png"), UIControlState.Normal);
                                }
                                
                                labelMode.Text = "Opposite Direction";
                                labelDetails.Text = "Pattern Is Reversed";
                            }
                            else
                            {
                                if (myMidiModulation.IsSceneMode)
                                {
                                    buttonReverse.SetImage(UIImage.FromFile("graphicReverseSceneButtonOff.png"), UIControlState.Normal);
                                }
                                else
                                {
                                    buttonReverse.SetImage(UIImage.FromFile("graphicReverseButtonOff.png"), UIControlState.Normal);
                                }
                                //buttonReverse.SetImage(UIImage.FromFile("graphicReverseButtonOff.png"), UIControlState.Normal);
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


                                if ((!myMidiModulation.IsAR) && (!myMidiModulation.IsAutoPattern) && (!myMidiModulation.IsAR))
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
                                if (!myMidiModulation.IsArrangementMode)
                                {
                                    buttonLocation.SetImage(UIImage.FromFile("graphicLocationButtonOn.png"), UIControlState.Normal);
                                    sliderHidden.Hidden = false;
                                    rangeSlider.Enabled = false;
                                    labelRange.Text = "Starting Value: " + myMidiModulation.StartingLocation.ToString();

                                    labelMode.Text = " Restart Pattern   ";
                                    labelDetails.Text = "Begins At Starting Value";
                                }
                                ReadHiddenSlider(sliderHidden.Value);
                            }
                            else
                            {
                                if (!myMidiModulation.IsArrangementMode)
                                {
                                    buttonLocation.SetImage(UIImage.FromFile("graphicLocationButtonOff.png"), UIControlState.Normal);
                                    sliderHidden.Hidden = true;
                                    rangeSlider.Enabled = true;
                                    labelRange.Text = "Modulation Range";
                                    ResetDisplay();
                                }
                            }
                            break;
                        }

                    case "StartingLocation":
                        {
                            if (myMidiModulation.IsRestartEachNote)
                            {
                                labelRange.Text = "Starting Value: " + myMidiModulation.StartingLocation.ToString();
                            }
                            
                            break;
                        }

                    case "IsTriggerOnly":
                        {
                            if (myMidiModulation.IsTriggerOnly)
                            {
                                if (!myMidiModulation.IsArrangementMode)
                                {
                                    buttonTrigger.SetImage(UIImage.FromFile("graphicTriggerButtonOn.png"), UIControlState.Normal);
                                    buttonTrigger.SetImage(UIImage.FromFile("graphicTriggerButtonOn.png"), UIControlState.Highlighted);
                                    buttonLocation.Enabled = true;
                                    labelMode.Text = " Note On Trigger   ";
                                    labelDetails.Text = "Modulation When Playing";
                                }
                            }
                            else
                            {
                                if (!myMidiModulation.IsArrangementMode)
                                {
                                    buttonTrigger.SetImage(UIImage.FromFile("graphicTriggerButtonOff.png"), UIControlState.Normal);
                                    buttonTrigger.SetImage(UIImage.FromFile("graphicTriggerButtonOff.png"), UIControlState.Highlighted);
                                    buttonLocation.Enabled = false;
                                    ResetDisplay();
                                }
                                myMidiModulation.NumOfNotesOn = 0;
                                myMidiModulation.IsNoteOn = false;
                                myMidiModulation.IsRestartEachNote = false;
                                
                                
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
                                    sliderRate.Value = ((float)tempVal - 3); //(oddly enough, this subtraction fixes a weird drifting bug..)
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

                                //MODE 1 = MIDI MODE
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

                                    if (timerArrangement.IsRunning)
                                    {
                                        timerArrangement.Stop();
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
                                    if (myMidiModulation.IsArrangementMode)
                                    {
                                        timerArrangement.Start();
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
                                    if (myMidiModulation.IsArrangementMode)
                                    {
                                        timerArrangement.Start();
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        }

                    case "PatternNumber":
                        {
                            ReadSlider(sliderRate.Value);

                            if (!myMidiModulation.IsArrangementMode)
                            {

                                switch (myMidiModulation.PatternNumber)
                                {
                                    case 1:
                                    case 4:
                                    case 7:
                                    case 8:
                                        myMidiModulation.Opposite = false;
                                        //buttonReverse.Hidden = true;
                                        buttonReverse.Enabled = false;

                                        if (myMidiModulation.IsSceneMode)
                                        {
                                            buttonReverse.SetImage(UIImage.FromFile("graphicReverseSceneButtonOff.png"), UIControlState.Normal);
                                        }
                                        else
                                        {
                                            buttonReverse.SetImage(UIImage.FromFile("graphicReverseButtonOff.png"), UIControlState.Normal);
                                        }


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

                                    updateProgressBar();

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
            /*
            if (!myMidiModulation.IsRestartEachNote)
            {
                labelRange.Text = "Modulation Range";
            }
            */
        }

        

        void updateProgressBar()
        {
            if (!(myMidiModulation.IsSceneMode))
            {
                var pi_mult = (myMidiModulation.CurrentCC * 2.0f / 127);

                //Momentarily remove from superview so that the start button can be on top of progress bar
                myCircularProgressBar.RemoveFromSuperview();
                //Declare progress bar object (Instantiating my CircularProgressBar class)
                myCircularProgressBar = new CircularProgressBar(C_progressSize, C_lineWidth, pi_mult, barColor);
                buttonOnOff.RemoveFromSuperview();

                //Add Views
                View.AddSubview(myCircularProgressBar);
                View.AddSubview(buttonOnOff);
            }
            else
            {

                var pi_mult = (myMidiModulation.CurrentCC * 2.0f / 127);

                //Momentarily remove from superview so that the start button can be on top of progress bar
                myCircularProgressBar.RemoveFromSuperview();
                //Declare progress bar object (Instantiating my CircularProgressBar class)
                myCircularProgressBar = new CircularProgressBar(C_progressSceneSize, C_lineWidthSmall, pi_mult, barColor);
                buttonOnOff.RemoveFromSuperview();

                //Add Views
                View.AddSubview(myCircularProgressBar);
                View.AddSubview(buttonOnOff);

                /*
                var progressPercent = (myMidiModulation.CurrentCC * 1.0f / 127);
                myHorizontalProgressBar.RemoveFromSuperview();
                myHorizontalProgressBar = new HorizontalProgressBar(H_progressSize, H_lineWidth, progressPercent, barColor);
                View.AddSubview(myHorizontalProgressBar);
                sliderCC.Value = myMidiModulation.CurrentCC;
                */
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
            if (!myMidiModulation.IsSceneMode)
            {
                buttonOnOff.Frame = bigStartSize;
            }

            //Declare color object (Instantiating the UIColor class)
            //UIColor barColor = UIColor.FromRGB(0, 255, 0);

            
                //Control Logic: switches between program ON and program OFF states when button is pressed
            if (UIHelper)
            {
                

                    buttonOnOff.SetImage(UIImage.FromFile("graphicPowerButtonOff.png"), UIControlState.Normal);
                    buttonOnOff.SetImage(UIImage.FromFile("graphicPowerButtonOn.png"), UIControlState.Highlighted);
                    myMidiModulation.IsRunning = false;

                if (!myMidiModulation.IsSceneMode)
                {
                    var pi_mult = (myMidiModulation.CurrentCC * 2.0f / 127);
                    myCircularProgressBar = new CircularProgressBar(C_progressSize, C_lineWidth, pi_mult, barColor);

                    //TODO: need different logic for scene mode here
                }

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
            if (!myMidiModulation.IsSceneMode)
            {
                
                
            }
        }

        private void PowerPushed()
        {
            if (myMidiModulation.IsRunning)
            {
                myMidiModulation.IsRunning = false;
                UIHelper = true;
            }
            myCircularProgressBar.RemoveFromSuperview();
            myCircularProgressBar.Hidden = true;

            if (!myMidiModulation.IsSceneMode)
            {
                buttonOnOff.Frame = smallStartSize;
            }

        }

        private void HandleScenesTouchDown(object sender, System.EventArgs e)
        {
            myMidiModulation.ScenesToggle();
        }

        private void HandleArrangeTouchDown(object sender, System.EventArgs e)
        {
            myMidiModulation.ArrangeToggle();
        }

        private void HandleSceneTouchDown(object sender, System.EventArgs e)
        {
            UIButton myButton = (UIButton)sender;
            UpdateSceneGraphic(myButton);
        }

        protected void HandleLocationTouchDown(object sender, System.EventArgs e)
        {
            

            if (!myMidiModulation.IsSceneMode)
            {
                myMidiModulation.RestartToggle();
            }
            else
            {
                for (int ii = 0; ii < 8; ii++)
                {
                    if (sceneArray[ii].IsSelected)
                    {
                        sceneArray[ii].RestartToggle();
                    }
                }
            }
        }

        protected void HandleHiddenSliderChange(object sender, System.EventArgs e)
        {
            //var myObject = (UISlider)sender;
            //ReadHiddenSlider(myObject.Value);


            var myObject = (UISlider)sender;

            if (!myMidiModulation.IsSceneMode)
            {
                ReadHiddenSlider(myObject.Value);
            }
            else
            {
                for (int ii = 0; ii < 8; ii++)
                {
                    if (sceneArray[ii].IsSelected)
                    {
                        sceneArray[ii].StartingLocation = (int)myObject.Value;
                    }
                }
            }
        }

        void ReadHiddenSlider(float sliderValue)
        {
            myMidiModulation.StartingLocation = (int)sliderValue;
        }

        protected void HandleReverseTouchDown(object sender, System.EventArgs e)
        {
            if (!myMidiModulation.IsSceneMode)
            {
                myMidiModulation.ReversePattern();
            }
            else
            {
                for (int ii = 0; ii < 8; ii++)
                {
                    if (sceneArray[ii].IsSelected)
                    {
                        sceneArray[ii].ReverseToggle();
                    }
                }
            }
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
            //myMidiModulation.TriggerToggle();
            if (!myMidiModulation.IsSceneMode)
            {
                myMidiModulation.TriggerToggle();
            }
            else
            {
                for (int ii = 0; ii < 8; ii++)
                {
                    if (sceneArray[ii].IsSelected)
                    {
                        sceneArray[ii].TriggerToggle();
                    }
                }
            }
        }

        protected void HandleRateSliderChange(object sender, System.EventArgs e)
        {
            var myObject = (UISlider)sender;
            if (!myMidiModulation.IsSceneMode)
            {
                ReadSlider(myObject.Value);
            }
            else
            {
                for (int ii = 0; ii < 8; ii++)
                {
                    if (sceneArray[ii].IsSelected)
                    {
                        sceneArray[ii].RateSliderValue = myObject.Value;
                    }
                }
            }
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
                    if (!myMidiModulation.IsArrangementMode)
                    {
                        labelRate.Text = myMidiModulation.TimeIntervalToFrequency(timerHighRes.Interval); // Convert time interval to frequency for label display
                    }
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

                //Cutoff for??
                if (!myMidiModulation.SettingsOn)
                {
                    myMidiModulation.CheckCutoff();
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
                    if (!myMidiModulation.IsArrangementMode)
                    {
                        labelRate.Text = "Ext. Clock Sync: " + displayText;
                    }
                }
                else
                {
                    //myMidiModulation.TimeSet(sliderValue); // Determines StepSize that will prevent too high of a screen refresh
                    timerHighRes.Interval = BeatsPerMinuteIntoMilliSeconds((float)myMidiModulation.BPM, myMidiModulation.RateCatch);
                    //INT Clock Sync
                    if (!myMidiModulation.IsArrangementMode)
                    {
                        labelRate.Text = "Int. Clock Sync: " + displayText;
                    }
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

        protected void HandlePatternSegmentChange(object sender, System.EventArgs e)
        {
            var seg = sender as UISegmentedControl;
            if (!myMidiModulation.IsSceneMode)
            {
                ReadPattern(seg.SelectedSegment);
            }
            else
            {
                for (int ii = 0; ii < 8; ii++)
                {
                    if (sceneArray[ii].IsSelected) {
                        sceneArray[ii].PatternNumber = (int)seg.SelectedSegment+1;
                    }
                }
            }
        }

        void ReadPattern(nint patternIndex)
        {

            string labelText = myMidiModulation.UpdatePattern(patternIndex);
            labelPattern.Text = labelText;

        }


        
    }
}
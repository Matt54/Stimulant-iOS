using System;
using UIKit;
using CoreMidi;
using CoreGraphics;
using System.ComponentModel;
using System.Diagnostics;

namespace Stimulant
{
    public partial class ViewController : UIViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {// Note: this .ctor should not contain any initialization logic.
        }

        // Declare MidiModulation object: MidiModulation class stores all the current modulation parameters
        MidiModulation myMidiModulation = new MidiModulation();

        // Controls how fast the time-based modulation steps
        //HighResolutionTimer timerModTrigger;

        // Controls how often the random settings get applied when in automatic mode
        HighResolutionTimer timerAuto;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            Midi.Restart(); //This stops the MIDI subsystems and forces it to be reinitialized
            SetupMidi();
            MakeHardware();
            MakeDevices();

            myMidiModulation.ModTimerElapsed += HandleModTrigger;

            timerAuto = new HighResolutionTimer(6300.0f);
            timerAuto.UseHighPriorityThread = false;
            timerAuto.Elapsed += (s, e) =>
            {
                InvokeOnMainThread(() => {
                    if (!myMidiModulation.SettingsOn)
                    {
                        if (myMidiModulation.ModeNumber == 2) myMidiModulation.IsRandomRoll = true;
                        else
                        {
                            myMidiModulation.AutoCounter++;
                            if (myMidiModulation.AutoCounter > myMidiModulation.AutoCutoff)
                            {
                                myMidiModulation.AutoCounter = 0;
                                myMidiModulation.IsRandomRoll = true;
                            }
                        }
                    }
                });
            };

            UIHelper = false;
            LoadDisplay();

            myMidiModulation.ModeNumber = 2;
            ReadSlider(sliderRate.Value);

            sceneDisplay.GetScene(0).IsSelected = true;
            sceneDisplay.GetScene(0).IsRunning = true;
            for (int ii = 0; ii < sceneDisplay.GetNumberOfScenes(); ii++) myMidiModulation.setParameters(sliderRate.Value, sceneDisplay.GetScene(ii));

            sceneDisplay.UpdateAllSceneGraphics();

            myMidiModulation.PropertyChanged += (object s, PropertyChangedEventArgs e2) =>
            {
                switch (e2.PropertyName)
                {
                    case "FireModulation":
                        InvokeOnMainThread(() => {
                            if (myMidiModulation.FireModulation)
                            {
                                if (!myMidiModulation.IsTriggerOnly || (myMidiModulation.IsTriggerOnly && myMidiModulation.IsNoteOn))
                                {
                                    SendMIDI(0xB0, (byte)myMidiModulation.CCNumber, (byte)myMidiModulation.CurrentCC);

                                    //Debug.WriteLine(myMidiModulation.CurrentCC);

                                    myMidiModulation.ClockCount = 0;

                                    updateProgressBar();

                                    myMidiModulation.FireModulation = false;

                                }
                            }
                        });
                        break;
                    case "IsRunning":
                        {
                            if (myMidiModulation.ModeNumber != 1)
                            {
                                if (myMidiModulation.IsRunning) myMidiModulation.StartTimer();
                                else myMidiModulation.StopTimer();
                            }

                            break;
                        }

                    case "IsPatternRestart":
                        {
                            if (myMidiModulation.IsPatternRestart)
                            {
                                if (myMidiModulation.IsArrangementMode)
                                {
                                    sceneDisplay.MoveToNextScene(); //MoveToNextScene();
                                    myMidiModulation.ResetPatternValues();
                                }
                                myMidiModulation.HasMoved = false;
                                myMidiModulation.IsPatternRestart = false;
                            }
                            break;
                        }

                    case "IsSceneMode":
                        {
                            if (myMidiModulation.IsSceneMode)
                            {
                                powerButton.UpdateFrame(new CGRect(screenWidth * .7, screenHeight * 0.415, screenWidth * .3, screenHeight * 0.08));
                                var pi_mult = (myMidiModulation.CurrentCC * 2.0f / 127);

                                powerButton.UpdateProgress(pi_mult);


                                buttonRandom.Hidden = true;

                                /*
                                buttonReverse.Frame = frameReverseScene;

                                if (myMidiModulation.Opposite) buttonReverse.SetImage(UIImage.FromFile("graphicReverseSceneButtonOn.png"), UIControlState.Normal);
                                else buttonReverse.SetImage(UIImage.FromFile("graphicReverseSceneButtonOff.png"), UIControlState.Normal);
                                buttonReverse.SetImage(UIImage.FromFile("graphicReverseSceneButtonOff.png"), UIControlState.Highlighted);
                                buttonReverse.SetImage(UIImage.FromFile("graphicReverseSceneButtonDisabled.png"), UIControlState.Disabled);
                                */

                                buttonScenes.SetImage(UIImage.FromFile("graphicScenesButtonOn"), UIControlState.Normal);

                                myMidiModulation.setParameters( sliderRate.Value, sceneDisplay.GetScene( sceneDisplay.GetSceneSelected() ) );
                                sceneDisplay.View.Hidden = false;
                                View.AddSubview(buttonArrange);

                                sceneDisplay.UpdateAllSceneGraphics(); // UpdateSceneGraphic();

                                infoDisplay.UpdateTitle(" Scene Select ");
                                infoDisplay.UpdateDesc("Click to Load Settings");
                            }
                            else
                            {
                                buttonArrange.RemoveFromSuperview();
                                myMidiModulation.IsArrangementMode = false;

                                powerButton.UpdateFrame(new CGRect(0, screenHeight * 0.25, screenWidth, screenHeight * 0.25));

                                var pi_mult = (myMidiModulation.CurrentCC * 2.0f / 127);
                                powerButton.UpdateProgress(pi_mult);

                                buttonRandom.Hidden = false;

                                //buttonReverse.Frame = frameReverseOrig;

                                /*
                                if (myMidiModulation.Opposite) buttonReverse.SetImage(UIImage.FromFile("graphicReverseButtonOn.png"), UIControlState.Normal);
                                else buttonReverse.SetImage(UIImage.FromFile("graphicReverseButtonOff.png"), UIControlState.Normal);
                                buttonReverse.SetImage(UIImage.FromFile("graphicReverseButtonOff.png"), UIControlState.Highlighted);
                                buttonReverse.SetImage(UIImage.FromFile("graphicReverseButtonDisabled.png"), UIControlState.Disabled);
                                */

                                buttonScenes.SetImage(UIImage.FromFile("graphicScenesButtonOff"), UIControlState.Normal);
                                sceneDisplay.View.Hidden = true;

                                ResetDisplay();
                            }
                            break;
                        }

                    case "IsArrangementMode":
                        {
                            if (myMidiModulation.IsArrangementMode) buttonArrange.SetImage(UIImage.FromFile("graphicArrangeButtonOn"), UIControlState.Normal);
                            else buttonArrange.SetImage(UIImage.FromFile("graphicArrangeButtonOff"), UIControlState.Normal);
                            break;
                        }

                    case "SceneMove":
                        {
                            if (myMidiModulation.SceneMove)
                            {
                                sceneDisplay.MoveToNextScene();
                                myMidiModulation.SceneMove = false;
                            }
                            break;
                        }

                        /*
                    case "Opposite":
                        {
                            if (myMidiModulation.Opposite)
                            {
                                myMidiModulation.EveryOther = !myMidiModulation.EveryOther;

                                if (myMidiModulation.IsSceneMode) buttonReverse.SetImage(UIImage.FromFile("graphicReverseSceneButtonOn.png"), UIControlState.Normal);
                                else buttonReverse.SetImage(UIImage.FromFile("graphicReverseButtonOn.png"), UIControlState.Normal);

                                infoDisplay.UpdateTitle("Opposite Direction");
                                infoDisplay.UpdateDesc("Pattern Is Reversed");
                            }
                            else
                            {
                                if (myMidiModulation.IsSceneMode) buttonReverse.SetImage(UIImage.FromFile("graphicReverseSceneButtonOff.png"), UIControlState.Normal);
                                else buttonReverse.SetImage(UIImage.FromFile("graphicReverseButtonOff.png"), UIControlState.Normal);

                                ResetDisplay();
                            }
                            break;
                        }
                        */

                    case "PatternString":
                        {
                            InvokeOnMainThread(() =>
                            {
                                patternSelection.UpdateLabelText(myMidiModulation.PatternString);
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
                                else myMidiModulation.AutoCutoff = 64 * (1728 / 127);

                                buttonAuto.SetImage(UIImage.FromFile("graphicAutoButtonOn.png"), UIControlState.Normal);
                                buttonAuto.SetImage(UIImage.FromFile("graphicAutoButtonOn.png"), UIControlState.Highlighted);
                                buttonSettings.Enabled = true;

                                infoDisplay.UpdateTitle(" Automatic Mode ");
                                infoDisplay.UpdateDesc("Randoms At A Set Rate");
                            }
                            else
                            {
                                timerAuto.Stop(joinThread: false);
                                buttonAuto.SetImage(UIImage.FromFile("graphicAutoButtonOff.png"), UIControlState.Normal);
                                buttonAuto.SetImage(UIImage.FromFile("graphicAutoButtonOff.png"), UIControlState.Highlighted);
                                buttonSettings.Enabled = false;

                                if (myMidiModulation.SettingsOn) myMidiModulation.SettingsOn = false;

                                if (myMidiModulation.IsAR) myMidiModulation.IsAR = false;

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
                                        patternSelection.UpdateLabelText( myMidiModulation.RandomRoll() );
                                        patternSelection.SetPattern(myMidiModulation.PatternNumber - 1);
                                    }

                                    if (myMidiModulation.IsAutoRate)
                                    {
                                        sliderRate.Value = myMidiModulation.RandomNumber(1, 127);
                                        ReadSlider(sliderRate.Value);
                                    }

                                    if (myMidiModulation.IsAR)
                                    {
                                        rangeSelection.SetMaximum(myMidiModulation.RandomNumber(1, 127));
                                        myMidiModulation.Maximum = rangeSelection.GetMaximum();
                                        rangeSelection.SetMinimum( myMidiModulation.RandomNumber( 1, rangeSelection.GetMaximum() ) );
                                        myMidiModulation.Minimum = rangeSelection.GetMinimum();
                                    }
                                    myMidiModulation.IsRandomRoll = false;
                                });
                            }
                            break;
                        }

                    case "IsAR":
                        {
                            if (myMidiModulation.IsAR)
                            {
                                buttonAR.SetImage(UIImage.FromFile("graphicARButtonOn.png"), UIControlState.Normal);
                                infoDisplay.UpdateTitle("Automatic Range");
                                infoDisplay.UpdateDesc("Randoms Change Range");
                                buttonRandom.Enabled = true;
                            }
                            else
                            {
                                buttonAR.SetImage(UIImage.FromFile("graphicARButtonOff.png"), UIControlState.Normal);
                                ResetDisplay();

                                if ((!myMidiModulation.IsAR) && (!myMidiModulation.IsAutoPattern)) buttonRandom.Enabled = false;

                            }
                            break;
                        }

                    case "IsAutoPattern":
                        {
                            if (myMidiModulation.IsAutoPattern)
                            {
                                buttonAutoPattern.SetImage(UIImage.FromFile("graphicAutoPatternButtonOn.png"), UIControlState.Normal);
                                infoDisplay.UpdateTitle("Automatic Pattern");
                                infoDisplay.UpdateDesc("Randoms Change Pattern");
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
                                infoDisplay.UpdateTitle("Automatic Rate");
                                infoDisplay.UpdateDesc("Randoms Change Rate");
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

                                    rangeSelection.LocationSelection(true);

                                    rangeSelection.UpdateLabelText("Starting Value: " + myMidiModulation.StartingLocation.ToString());

                                    infoDisplay.UpdateTitle(" Restart Pattern   ");
                                    infoDisplay.UpdateDesc("Begins At Starting Value");
                                }

                                myMidiModulation.StartingLocation = rangeSelection.GetStartingLocation();
                            }
                            else
                            {
                                if (!myMidiModulation.IsArrangementMode)
                                {
                                    buttonLocation.SetImage(UIImage.FromFile("graphicLocationButtonOff.png"), UIControlState.Normal);

                                    rangeSelection.LocationSelection(false);

                                    rangeSelection.UpdateLabelText("Modulation Range");
                                    ResetDisplay();
                                }
                            }
                            break;
                        }

                    case "StartingLocation":
                        {
                            if (myMidiModulation.IsRestartEachNote)
                                rangeSelection.UpdateLabelText("Starting Value: " + myMidiModulation.StartingLocation.ToString());
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
                                    infoDisplay.UpdateTitle(" Note On Trigger   ");
                                    infoDisplay.UpdateDesc("Modulation When Playing");
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

                                patternSelection.SetVisibility(true);

                                buttonPlus1.Hidden = false;
                                buttonPlus10.Hidden = false;
                                buttonMinus1.Hidden = false;
                                buttonMinus10.Hidden = false;

                                patternSelection.UpdateLabelText("Current Tempo: " + myMidiModulation.BPM + "BPM");

                                buttonTap.Hidden = false;

                                infoDisplay.UpdateTitle("Tempo Adjustment");
                                infoDisplay.UpdateDesc("Tap Or Use Arrows To Set");
                            }
                            else
                            {
                                buttonBPM.SetImage(UIImage.FromFile("graphicBPMButtonOff.png"), UIControlState.Normal);

                                patternSelection.SetVisibility(false);

                                buttonPlus1.Hidden = true;
                                buttonPlus10.Hidden = true;
                                buttonMinus1.Hidden = true;
                                buttonMinus10.Hidden = true;

                                ReadPattern( patternSelection.GetPatternNumber() );

                                buttonTap.Hidden = true;
                                ResetDisplay();
                            }
                            break;
                        }

                    case "BPM":
                        {
                            if (myMidiModulation.BPMOn)
                            {
                                patternSelection.UpdateLabelText("Current Tempo: " + myMidiModulation.BPM + "BPM");
                                ReadSlider(sliderRate.Value);
                            }
                            break;
                        }

                    case "CCOn":
                        {
                            if (myMidiModulation.CCOn)
                            {
                                buttonCC.SetImage(UIImage.FromFile("graphicCCButtonOn.png"), UIControlState.Normal);

                                patternSelection.SetVisibility(true);

                                buttonPlus1.Hidden = false;
                                buttonPlus10.Hidden = false;
                                buttonMinus1.Hidden = false;
                                buttonMinus10.Hidden = false;
                                buttonTap.Hidden = false;
                                buttonTap.Enabled = false;

                                patternSelection.UpdateLabelText("Current Channel: CC" + myMidiModulation.CCNumber);

                                infoDisplay.UpdateTitle("CC Number Setting");
                                infoDisplay.UpdateDesc("Value Adjusted By Arrows");
                            }
                            else
                            {
                                buttonCC.SetImage(UIImage.FromFile("graphicCCButtonOff.png"), UIControlState.Normal);

                                patternSelection.SetVisibility(false);

                                buttonPlus1.Hidden = true;
                                buttonPlus10.Hidden = true;
                                buttonMinus1.Hidden = true;
                                buttonMinus10.Hidden = true;
                                buttonTap.Hidden = true;
                                buttonTap.Enabled = true;

                                ReadPattern(patternSelection.GetPatternNumber());

                                ResetDisplay();
                            }
                            break;
                        }

                    case "CCNumber":
                        {
                            if (myMidiModulation.CCOn) patternSelection.UpdateLabelText("Current Channel: CC" + myMidiModulation.CCNumber);
                            break;
                        }

                    case "SettingsOn":
                        {
                            if (myMidiModulation.SettingsOn)
                            {
                                buttonSettings.SetImage(UIImage.FromFile("graphicSettingsButtonOn.png"), UIControlState.Normal);
                                myMidiModulation.RateRemember = (int)sliderRate.Value;

                                if (myMidiModulation.ModeNumber == 2) sliderRate.Value = (float)myMidiModulation.AutoCutoff;
                                else
                                {
                                    double tempVal;
                                    tempVal = myMidiModulation.AutoCutoff * 128 / 1728;
                                    sliderRate.Value = ((float)tempVal - 3); //(oddly enough, this subtraction fixes a weird drifting bug..)
                                }
                                ReadSlider(sliderRate.Value);
                                infoDisplay.UpdateTitle("Auto Rate Setting");
                                infoDisplay.UpdateDesc("Value Adjusted By Slider");
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
                                    //timerModTrigger.Stop(joinThread: false);

                                    ReadSlider(sliderRate.Value);
                                    buttonBPM.Enabled = false;
                                    if (myMidiModulation.BPMOn) myMidiModulation.BPMOn = false;

                                    infoDisplay.UpdateTitle(" Ext Clock Mode   ");
                                    infoDisplay.UpdateDesc(" Midi Clock Adjusts Rate ");

                                    if (myMidiModulation.IsAuto) myMidiModulation.IsAuto = false;

                                    rateSelection.SetSliderSnap(true);

                                    break;

                                //MODE 2 = FREE TIMING MODE
                                case 2:

                                    //timerModTrigger.Start();

                                    ReadSlider(sliderRate.Value);
                                    buttonBPM.Enabled = false;

                                    if (myMidiModulation.BPMOn) myMidiModulation.BPMOn = false;

                                    infoDisplay.UpdateTitle("Free Timing Mode");
                                    infoDisplay.UpdateDesc("Rate Based On Frequency");

                                    rateSelection.SetSliderSnap(false);

                                    if (myMidiModulation.IsAuto) myMidiModulation.IsAuto = false;
                                    break;

                                // MODE 3 = INTERNAL CLOCK / BPM TIMING 
                                case 3:
                                    myMidiModulation.CutoffFactor = 1;
                                    myMidiModulation.ClockCutoff = 1;
                                    //timerModTrigger.Start();

                                    buttonBPM.Enabled = true;
                                    ReadSlider(sliderRate.Value);

                                    infoDisplay.UpdateTitle(" Int Clock Mode   ");
                                    infoDisplay.UpdateDesc("Current Clock Tempo = " + myMidiModulation.BPM.ToString());

                                    rateSelection.SetSliderSnap(true);

                                    if (myMidiModulation.IsAuto) myMidiModulation.IsAuto = false;

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

                                /*
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
                                            buttonReverse.SetImage(UIImage.FromFile("graphicReverseSceneButtonOff.png"), UIControlState.Normal);
                                        else buttonReverse.SetImage(UIImage.FromFile("graphicReverseButtonOff.png"), UIControlState.Normal);

                                        break;
                                    default:
                                        buttonReverse.Hidden = false;
                                        buttonReverse.Enabled = true;
                                        break;
                                }
                                */


                                if (myMidiModulation.PatternNumber > 6)
                                {
                                    myMidiModulation.IsRestartEachNote = false;
                                    buttonLocation.Enabled = false;
                                }
                                else if (myMidiModulation.IsTriggerOnly) buttonLocation.Enabled = true;

                                patternSelection.UpdateLabelText( myMidiModulation.GetPatternText() );
                            }
                            break;
                        }

                    
                }
            };

            myMidiModulation.ModeNumber = 2;
            myMidiModulation.PatternNumber = 1;

            SavePattern();
            rateSelection.UpdateLabel(myMidiModulation.GetIntervalFrequencyString());

            //Make sure the starting rate is accurate
            RecalculateRates();
        }

        private void ResetDisplay()
        {
            switch (myMidiModulation.ModeNumber)
            {
                case 1:
                    infoDisplay.UpdateTitle("  Ext Clock Mode  ");
                    infoDisplay.UpdateDesc(" Midi Clock Adjusts Rate ");
                    break;
                case 2:
                    infoDisplay.UpdateTitle("Free Timing Mode");
                    infoDisplay.UpdateDesc("Rate Based On Frequency");
                    break;
                case 3:
                    infoDisplay.UpdateTitle(" Int Clock Mode   ");
                    infoDisplay.UpdateDesc("Current Clock Tempo = " + myMidiModulation.BPM.ToString());
                    break;
            }
        }

        

        void updateProgressBar()
        {
            var pi_mult = myMidiModulation.CurrentCC * 2.0f / 127;
            powerButton.UpdateProgress(pi_mult);
        }

        



        //private void HandleSceneChange(object sender, PropertyChangedEventArgs e)
        private void HandleSceneChange(object sender, string propertyName, int index)
        {
            //See scene logic file
            CombinedSceneProperty(propertyName, index);
        }

        private void HandlePlus1TouchDown(object sender, System.EventArgs e)
        {
            if (myMidiModulation.CCOn) myMidiModulation.UpdateCCNumber(1);
            else myMidiModulation.UpdateBPM(1);
        }

        private void HandlePlus10TouchDown(object sender, System.EventArgs e)
        {
            if (myMidiModulation.CCOn) myMidiModulation.UpdateCCNumber(10);
            else myMidiModulation.UpdateBPM(10);
        }

        private void HandleMinus1TouchDown(object sender, System.EventArgs e)
        {
            if (myMidiModulation.CCOn) myMidiModulation.UpdateCCNumber(-1);
            else myMidiModulation.UpdateBPM(-1);
        }

        private void HandleMinus10TouchDown(object sender, System.EventArgs e)
        {
            if (myMidiModulation.CCOn) myMidiModulation.UpdateCCNumber(-10);
            else myMidiModulation.UpdateBPM(-10);
        }

        partial void RateSliderChange(UISlider sender) { }
        partial void StartButton_TouchUpInside(UIButton sender) { }

        private void HandlePowerButtonStateChange(object sender, System.EventArgs e)
        {
            myMidiModulation.IsRunning = powerButton.IsOn();
            if (!myMidiModulation.IsRunning) myMidiModulation.HoldOnStart = true;
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
        }

        protected void HandleLocationTouchDown(object sender, System.EventArgs e)
        {
            if (!myMidiModulation.IsSceneMode) myMidiModulation.RestartToggle();
            else sceneDisplay.GetScene(sceneDisplay.GetSceneSelected()).RestartToggle();
        }

        protected void HandleRangeSliderChange(object sender, System.EventArgs e)
        {
            if (!myMidiModulation.IsSceneMode)
            {
                myMidiModulation.Maximum = rangeSelection.GetMaximum();
                myMidiModulation.Minimum = rangeSelection.GetMinimum();
            }
            else
            {
                sceneDisplay.GetScene( sceneDisplay.GetSceneSelected() ).Maximum = rangeSelection.GetMaximum();
                sceneDisplay.GetScene( sceneDisplay.GetSceneSelected() ).Minimum = rangeSelection.GetMinimum();
            }
        }

        protected void HandleHiddenSliderChange(object sender, System.EventArgs e)
        {
            if (!myMidiModulation.IsSceneMode) myMidiModulation.StartingLocation = rangeSelection.GetStartingLocation();
            else sceneDisplay.GetScene(sceneDisplay.GetSceneSelected()).StartingLocation = rangeSelection.GetStartingLocation();
        }

        /*
        protected void HandleReverseTouchDown(object sender, System.EventArgs e)
        {
            if (!myMidiModulation.IsSceneMode) myMidiModulation.ReversePattern();
            else sceneDisplay.GetScene(sceneDisplay.GetSceneSelected()).ReverseToggle();
        }
        */

        protected void HandleCCTouchDown(object sender, System.EventArgs e)
        {
            myMidiModulation.CCToggle();
        }

        private void HandleBPMTouchDown(object sender, System.EventArgs e)
        {
            myMidiModulation.BPMToggle();
            powerButton.UpdateProgress(2f);
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
            if (!myMidiModulation.IsSceneMode) myMidiModulation.TriggerToggle();
            else sceneDisplay.GetScene(sceneDisplay.GetSceneSelected()).TriggerToggle();
        }

        protected void HandleRateSliderChange(object sender, System.EventArgs e)
        {
            var myObject = (UISlider)sender;
            if (!myMidiModulation.IsSceneMode) ReadSlider(myObject.Value);
            else sceneDisplay.GetScene(sceneDisplay.GetSceneSelected()).RateSliderValue = myObject.Value;
        }

        private void HandlePatternChange(object sender, System.EventArgs e)
        {
            if (!myMidiModulation.IsSceneMode) myMidiModulation.SetPatternNumber(patternSelection.GetPatternNumber());
            else
            {
                sceneDisplay.GetScene(sceneDisplay.GetSceneSelected()).PatternNumber = patternSelection.GetPatternNumber();
                if (myMidiModulation.IsArrangementMode) patternSelection.UpdateLabelText(myMidiModulation.GetPatternText(patternSelection.GetPatternNumber()));
            }
        }

        private void HandlePatternModify(object sender, System.EventArgs e)
        {
            myMidiModulation.SetPatternNumber(patternSelection.GetPatternNumber());
            patternGraphVC.SetCurveLayer(patternSelection.GetSelectedCurveLayer());
            patternGraphVC.SetPatternSlot(patternSelection.GetPatternNumber());
            View.AddSubview(patternGraphVC.View); 
            patternSelection.IsBeingModified = true;
        }

        protected void HandleExitPatternModify(object sender, System.EventArgs e)
        {
            SavePattern();
            /*
            patternSelection.CopyCurveLayer(patternGraphVC.GetCurveLayer());
            patternSelection.IsBeingModified = false;
            patternGraphVC.View.RemoveFromSuperview();
            Debug.WriteLine(patternSelection.GetMidiValFromPattern(60, 0));
            */
        }

        private void SavePattern()
        {
            patternSelection.CopyCurveLayer(patternGraphVC.GetCurveLayer());
            patternSelection.IsBeingModified = false;
            patternGraphVC.View.RemoveFromSuperview();
        }

        protected void HandleModeChange(object sender, System.EventArgs e)
        {
            myMidiModulation.ModeNumber = timingModeSelection.GetModeNumber();
            RecalculateRates();
        }

        //TODO write the new rate functionality
        //our new rate function
        private void HandleRateChange(object sender, System.EventArgs e)
        {
            RecalculateRates();
        }

        private void RecalculateRates()
        {
            if ( (myMidiModulation.ModeNumber != 1) & (myMidiModulation.IsRunning) ) myMidiModulation.StartTimer();
            //else myMidiModulation.StopTimer();

            float sliderValue = rateSelection.GetValue();

            //MIDI OR INTERNAL CLOCK
            if (rateSelection.IsSliderSnap)
            {
                myMidiModulation.SetXStepSnapped(sliderValue); // Determines StepSize from a sliderValue
                rateSelection.UpdateLabel(myMidiModulation.GetIntervalBeatFractionString(sliderValue));
            }

            //LOOSE FREQUENCY
            else
            {
                myMidiModulation.SetXStep(sliderValue); // Determines StepSize from a sliderValue
                myMidiModulation.SetTimerInterval(myMidiModulation.ValueToTimeInterval(sliderValue)); // Magic function to get time interval from sliderValue
                rateSelection.UpdateLabel(myMidiModulation.GetIntervalFrequencyString());
            }
        }

        // event is fired by timer from MidiModulation
        private void HandleModTrigger(object sender, System.EventArgs e)
        {

            if(myMidiModulation.HoldOnStart)
            {
                myMidiModulation.HoldOnStart = false;
            }
            else
            { 
                //Steps to the next X value
                myMidiModulation.StepX();
            }

            //Get the corresponding Y value from the graph using the X value
            InvokeOnMainThread(() =>
            {
                myMidiModulation.CurrentCC = patternSelection.GetMidiValFromPattern(myMidiModulation.CurrentXVal, myMidiModulation.PatternNumber - 1);
            });

            //Send the value
            myMidiModulation.FireModulation = true;
        }












        //TODO get rid of this function and replace it with something that is reading the patterns
        //this function is used to determine the rate
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
                    /*
                    timerModTrigger.Interval = ValueToTimeInterval(sliderValue); // Magic function to get time interval from sliderValue
                    if (!myMidiModulation.IsArrangementMode)
                    {
                        labelRate.Text = myMidiModulation.TimeIntervalToFrequency(timerModTrigger.Interval); // Convert time interval to frequency for label display
                    }
                    */
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
                    //myMidiModulation.StepSizeSetter();

                    //EXT Clock Sync
                    if (!myMidiModulation.IsArrangementMode)
                    {
                        labelRate.Text = "Ext. Clock Sync: " + displayText;
                    }
                }
                else
                {
                    //myMidiModulation.TimeSet(sliderValue); // Determines StepSize that will prevent too high of a screen refresh
                    /*
                    timerModTrigger.Interval = BeatsPerMinuteIntoMilliSeconds((float)myMidiModulation.BPM, myMidiModulation.RateCatch);
                    //INT Clock Sync
                    if (!myMidiModulation.IsArrangementMode)
                    {
                        labelRate.Text = "Int. Clock Sync: " + displayText;
                    }
                    */
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
                    intervalMultiplier = 1; // Whole
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
            if (!myMidiModulation.IsSceneMode) ReadPattern(seg.SelectedSegment);
            else sceneDisplay.GetScene(sceneDisplay.GetSceneSelected()).PatternNumber = (int)seg.SelectedSegment + 1;
        }

        void ReadPattern(nint patternIndex)
        {
            myMidiModulation.SetPatternNumber((int)patternIndex);
        }

    }
}
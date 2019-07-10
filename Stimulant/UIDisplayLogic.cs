using System;
using UIKit;
using CoreGraphics;
using Xamarin.RangeSlider;
using System.Diagnostics;
using Foundation;

namespace Stimulant
{
    public partial class ViewController
    {

        //These variables are used consistently to update the 
        CGRect bigStartSize;
        CGRect smallStartSize;
        CGRect progressSize;
        int lineWidth;
        UIColor barColor;
        bool UIHelper;

        //Declare View References
        UIButton buttonOnOff;
        UIButton buttonTime;
        UIButton buttonMidi;
        UIButton buttonReverse;
        UIButton buttonCC;
        UIButton buttonSettings;
        UIButton buttonAR;
        UIButton buttonInfo;
        UIButton buttonClock;
        UIButton buttonBPM;
        UIButton buttonRandom;
        UIButton buttonAuto;
        UISegmentedControl segmentedPattern;
        UILabel labelPattern;
        CircularProgressBar myCircularProgressBar;
        UILabel labelRate;
        UISlider sliderRate;
        RangeSliderControl rangeSlider;
        UIButton buttonPlus1;
        UIButton buttonPlus10;
        UIButton buttonMinus1;
        UIButton buttonMinus10;



        public void LoadDisplay()
        {
            CGRect screenBound = UIScreen.MainScreen.Bounds;
            CGSize screenSize;
            float screenWidth;
            float screenHeight;
            string device;
            string background;
            string overlay;

            float buttonYAdjust;
            float sizeSubtract;
            float textAdjustRate;
            float textAdjustPattern;
            float controlAdjustRate;
            float controlAdjustPattern;
            float controlAdjustRange;
            float controlAdjustSettings;
            float controlAdjustRandoms;
            float controlAdjustAR;
            float controlAdjustMode;
            float controlAdjustCCInc;
            float controlAdjustBPM;
            float sizeIncrease;

            //Get the Screen Size, Width, and Height
            screenSize = screenBound.Size;
            screenWidth = (float)screenSize.Width;
            screenHeight = (float)screenSize.Height;

            float sliderHeight = (float)(screenHeight / 5);
            float segHeight = (float)(screenHeight / 18);

            Debug.WriteLine(screenWidth.ToString());
            Debug.WriteLine(screenHeight.ToString());

            float screenAspectRatio = screenHeight / screenWidth;

            device = GetDevice(screenHeight);

            float rangeFontSize = screenHeight / 25;
            sizeSubtract = (float)2.165333 / screenAspectRatio;

            //SetBackground(device); //This sets so many values (and is also not really just setting the background) - we should unpack this.
                                   //^Just leave the switch statement inside LoadDisplay

            switch(device)
            {
                case "8":
                    background = "Back8.png";
                    overlay = "Over8.png";
                    textAdjustRate = 0.96f;
                    textAdjustPattern = 0.975f;
                    controlAdjustRate = 0.99f;
                    controlAdjustPattern = 0.99f;
                    controlAdjustRange = 0.85f;
                    controlAdjustSettings = 0.98f;
                    controlAdjustRandoms = 0.99f;
                    controlAdjustAR = 1.00f;
                    controlAdjustMode = 1.00f;
                    controlAdjustCCInc = 0.99f;
                    controlAdjustBPM = 1f;
                    buttonYAdjust = 1.0f;
                    sizeIncrease = 1.0f;
                    break;
                case "8Plus":
                    background = "Back8Plus.png";
                    overlay = "Over8Plus.png";
                    textAdjustRate = 0.99f;
                    textAdjustPattern = 1.00f;
                    controlAdjustRate = 1.0f;
                    controlAdjustPattern = 1.0f;
                    controlAdjustRange = 1.0f;
                    controlAdjustSettings = 0.995f;
                    controlAdjustRandoms = 1.005f;
                    controlAdjustAR = 1.00f;
                    controlAdjustMode = 1.00f;
                    controlAdjustCCInc = 1.00f;
                    controlAdjustBPM = 1f;
                    buttonYAdjust = 1.0f;
                    sizeIncrease = 1.0f;
                    break;
                case "XS":
                    background = "BackXS.png";
                    overlay = "OverXS.png";
                    textAdjustRate = 0.995f;
                    textAdjustPattern = 1f;
                    controlAdjustRate = 1f;
                    controlAdjustPattern = 1.0f;
                    controlAdjustRange = 1.0f;
                    controlAdjustSettings = 0.995f;
                    controlAdjustRandoms = 1.0f;
                    controlAdjustAR = 1.00f;
                    controlAdjustMode = 1.00f;
                    controlAdjustCCInc = 1.00f;
                    controlAdjustBPM = 1f;
                    buttonYAdjust = 1.005f;
                    sizeIncrease = 1.2f;
                    break;
                case "XR":
                    background = "BackXSMax.png";
                    overlay = "OverXSMax.png";
                    textAdjustRate = 1.015f;
                    textAdjustPattern = 1.01f;
                    controlAdjustRate = 1.0f;
                    controlAdjustPattern = 1.0f;
                    controlAdjustRange = 1.0f;
                    controlAdjustSettings = 1.00f;
                    controlAdjustRandoms = 1.0f;
                    controlAdjustAR = 1.00f;
                    controlAdjustMode = 1.00f;
                    controlAdjustCCInc = 1.00f;
                    controlAdjustBPM = 1f;
                    buttonYAdjust = 1.0f;
                    sizeIncrease = 1.0f;
                    break;
                case "iPad":
                    background = "BackiPad.png";
                    overlay = "OveriPad.png";
                    textAdjustRate = 1.04f;
                    textAdjustPattern = 1.035f;
                    controlAdjustRate = 1.025f;
                    controlAdjustPattern = 1.01f;
                    controlAdjustRange = 1.0f;
                    controlAdjustSettings = 1.015f;
                    controlAdjustRandoms = 1.015f;
                    controlAdjustAR = 1.1f;
                    controlAdjustMode = 1.035f;
                    controlAdjustCCInc = 1.025f;
                    controlAdjustBPM = 0.95f;
                    buttonYAdjust = 1.0f;
                    sizeIncrease = 1.0f;
                    break;
                case "iPadPro":
                    background = "BackiPadPro.png";
                    overlay = "OveriPadPro.png";
                    textAdjustRate = 1.04f;
                    textAdjustPattern = 1.035f;
                    controlAdjustRate = 1.025f;
                    controlAdjustPattern = 1.01f;
                    controlAdjustRange = 1.0f;
                    controlAdjustSettings = 1.015f;
                    controlAdjustRandoms = 1.015f;
                    controlAdjustAR = 1.1f;
                    controlAdjustMode = 1.035f;
                    controlAdjustCCInc = 1.025f;
                    controlAdjustBPM = 0.95f;
                    buttonYAdjust = 1.0f;
                    sizeIncrease = 1.0f;
                    break;
                case "iPadPro11":
                    background = "BackiPadPro11.png";
                    overlay = "OveriPadPro11.png";
                    textAdjustRate = 1.053f;
                    textAdjustPattern = 1.043f;
                    controlAdjustRate = 1.024f;
                    controlAdjustPattern = 1.01f;
                    controlAdjustRange = 1.0f;
                    controlAdjustSettings = 1.014f;
                    controlAdjustRandoms = 1.015f;
                    controlAdjustAR = 1.1f;
                    controlAdjustMode = 1.035f;
                    controlAdjustCCInc = 1.024f;
                    controlAdjustBPM = 0.95f;
                    buttonYAdjust = 1.0f;
                    sizeIncrease = 1.0f;
                    break;
                case "iPadNext":
                    background = "BackiPadNext.png";
                    overlay = "OveriPadNext.png";
                    textAdjustRate = 1.055f;
                    textAdjustPattern = 1.045f;
                    controlAdjustRate = 1.025f;
                    controlAdjustPattern = 1.009f;
                    controlAdjustRange = 1.0f;
                    controlAdjustSettings = 1.01f;
                    controlAdjustRandoms = 1.013f;
                    controlAdjustAR = 1.05f;
                    controlAdjustMode = 1.02f;
                    controlAdjustCCInc = 1.025f;
                    controlAdjustBPM = 0.95f;
                    buttonYAdjust = 1.0f;
                    sizeIncrease = 1.0f;
                    break;
                default:
                    background = "BackXS.png";
                    overlay = "OverXS.png";
                    textAdjustRate = 0.995f;
                    textAdjustPattern = 1f;
                    controlAdjustRate = 1f;
                    controlAdjustPattern = 1.0f;
                    controlAdjustRange = 1.0f;
                    controlAdjustSettings = 0.995f;
                    controlAdjustRandoms = 1.0f;
                    controlAdjustAR = 1.00f;
                    controlAdjustMode = 1.00f;
                    controlAdjustCCInc = 1.00f;
                    controlAdjustBPM = 1f;
                    buttonYAdjust = 1.005f;
                    sizeIncrease = 1.0f;
                    break;
            }

            //Add the overlay view (Just UI Graphics - no controls)
            if (UIImage.FromBundle(overlay) != null)
            {
                this.View.InsertSubview(new UIImageView(UIImage.FromBundle(overlay)), 0);
            }

            //Add the background Graphic
            if (UIImage.FromFile(background) != null)
            {
                View.BackgroundColor = UIColor.FromPatternImage(UIImage.FromFile(background));
            }

            LoadStartButton(screenWidth, screenHeight, buttonYAdjust, sizeSubtract);
            LoadProgressBar(screenWidth, screenHeight, buttonYAdjust, sizeSubtract);
            LoadReverseButton(screenWidth, screenHeight, sizeSubtract);
            LoadCCButton(screenWidth, screenHeight, sizeSubtract, controlAdjustSettings);
            LoadSettingsButton(screenWidth, screenHeight, sizeSubtract, controlAdjustSettings);
            LoadARButton(screenWidth, screenHeight, sizeSubtract, controlAdjustAR);
            LoadInfoButton(screenWidth, screenHeight, sizeSubtract);
            LoadClockButton(screenWidth, screenHeight, sizeSubtract);
            LoadBPMButton(screenWidth, screenHeight, sizeSubtract, controlAdjustBPM);
            LoadRandomButton(screenWidth, screenHeight, sizeSubtract, controlAdjustRandoms);
            LoadAutoButton(screenWidth, screenHeight, sizeSubtract, controlAdjustRandoms);
            LoadTimeandMidiButtons(screenWidth, screenHeight, sizeSubtract, controlAdjustMode);
            LoadSegmented(screenWidth, screenHeight, controlAdjustPattern, segHeight);
            LoadRateSlider(screenWidth, screenHeight, controlAdjustRate, sliderHeight);
            LoadPatternLabel(screenWidth, screenHeight, textAdjustPattern, segHeight);
            LoadRateLabel(screenWidth, screenHeight, textAdjustRate, sliderHeight);
            LoadRangeSlider(screenWidth, screenHeight, controlAdjustRange, rangeFontSize);
            LoadCCIncButtons(screenWidth, screenHeight, sizeSubtract, controlAdjustCCInc, sizeIncrease);
            AddSubviewsToMainview();

        }

        public string GetDevice(float h)
        {
            string dev;
            switch (h)
            {
                case 667f:
                    dev = "8";
                    break;
                case 736f:
                    dev = "8Plus";
                    break;
                case 812f:
                    dev = "XS";
                    break;
                case 896f:
                    dev = "XR";
                    break;
                case 1024f:
                    dev = "iPad";
                    break;
                case 1112f:
                    dev = "iPadPro";
                    break;
                case 1194f:
                    dev = "iPadPro11";
                    break;
                case 1366f:
                    dev = "iPadNext";
                    break;
                default:
                    dev = "defaulted";
                    break;
            }

            return dev;
        }

        /*
        public void SetBackground(string dev)
        {
            
        }
        */

        public void LoadStartButton(float screenWidth, float screenHeight, float buttonYAdjust, float sizeSubtract)
        {
            //Button is half the screen size
            float buttonWidth = screenWidth / (1 + sizeSubtract);
            float buttonHeight = buttonWidth;

            //Centers the button in the screen Horizontally and moves it 25%ish down the screen
            float buttonXLoc = (screenWidth - buttonWidth) / 2;
            float buttonYLoc = (float)(((screenHeight - buttonHeight) / 3.15) - (1 - buttonYAdjust) * screenHeight);

            //Button is less than half the screen size (this creates the pressed in effect)
            float buttonWidthSmaller = (float)(screenWidth / (1.2 + sizeSubtract));
            float buttonHeightSmaller = buttonWidthSmaller;

            //Centers the button in the screen
            float buttonXLocSmaller = (screenWidth - buttonWidthSmaller) / 2;
            float buttonYLocSmaller = (float)((screenHeight - buttonHeightSmaller) / 3.12 - (1 - buttonYAdjust) * screenHeight);

            bigStartSize = new CGRect(buttonXLoc, buttonYLoc, buttonWidth, buttonHeight);
            smallStartSize = new CGRect(buttonXLocSmaller, buttonYLocSmaller, buttonWidthSmaller, buttonHeightSmaller);

            //Declare button object with its graphics, location, and size
            buttonOnOff = UIButton.FromType(UIButtonType.Custom);
            buttonOnOff.SetImage(UIImage.FromFile("graphicPowerButtonOff.png"), UIControlState.Normal);
            buttonOnOff.SetImage(UIImage.FromFile("graphicPowerButtonOn.png"), UIControlState.Highlighted);
            buttonOnOff.SetImage(UIImage.FromFile("graphicPowerButtonOff.png"), UIControlState.Disabled);
            buttonOnOff.Frame = bigStartSize;

            //Button Event Handler Delegates
            buttonOnOff.TouchUpInside += HandleTouchUpInside;
            buttonOnOff.TouchUpOutside += HandleTouchUpInside;
            buttonOnOff.TouchDown += HandleTouchDown;
        }

        public void LoadProgressBar(float screenWidth, float screenHeight, float buttonYAdjust, float sizeSubtract)
        {
            //Set progress bar size based on screen width (it will fit exactly around the button)
            lineWidth = (int)(screenWidth / 35);
            float progressWidth = (float)((screenWidth / (1 + sizeSubtract)) + 3 * lineWidth);
            float progressHeight = progressWidth;

            //Centers the progress bar on the screen
            float progressXLoc = (screenWidth - progressWidth) / 2;
            float progressYLoc = (float)(((screenHeight - progressHeight) / 3.15 - 0.55 * lineWidth) - (1 - buttonYAdjust) * screenHeight);

            //Declare rectangle object (Instantiating the CGRect class)
            progressSize = new CGRect(progressXLoc, progressYLoc, progressWidth, progressHeight);

            //Declare color object (Instantiating the UIColor class)
            barColor = UIColor.FromRGB(0, 255, 0);

            //Declare progress bar object (Instantiating my CircularProgressBar class)
            myCircularProgressBar = new CircularProgressBar(progressSize, lineWidth, 0.0f, barColor);
        }

        public void LoadReverseButton(float screenWidth, float screenHeight, float sizeSubtract)
        {
            float buttonReverseWidth = screenWidth / (5 + sizeSubtract);
            float buttonReverseHeight = buttonReverseWidth;
            //Centers the button in the screen Horizontally and moves it 25%ish down the screen
            float buttonReverseXLoc = (float)((screenWidth - buttonReverseWidth) / 1.1);
            float buttonReverseYLoc = (float)((screenHeight - buttonReverseHeight) / 2.05);
            buttonReverse = UIButton.FromType(UIButtonType.Custom);
            buttonReverse.SetImage(UIImage.FromFile("graphicReverseButtonOff.png"), UIControlState.Normal);
            buttonReverse.SetImage(UIImage.FromFile("graphicReverseButtonOff.png"), UIControlState.Highlighted);
            buttonReverse.SetImage(UIImage.FromFile("graphicReverseButtonOff.png"), UIControlState.Disabled);
            buttonReverse.Frame = new CGRect(buttonReverseXLoc, buttonReverseYLoc, buttonReverseWidth, buttonReverseHeight);
            buttonReverse.Hidden = true;
            buttonReverse.TouchDown += HandleReverseTouchDown;
        }

        public void LoadCCButton(float screenWidth, float screenHeight, float sizeSubtract,float controlAdjustSettings)
        {
            float buttonCCWidth = screenWidth / (12 + sizeSubtract);
            float buttonCCHeight = buttonCCWidth;
            float buttonCCXLoc = (float)((screenWidth - buttonCCWidth) * (0.09));
            float buttonCCYLoc = (float)((screenHeight - buttonCCHeight) / 1.67) * controlAdjustSettings;
            buttonCC = UIButton.FromType(UIButtonType.Custom);
            buttonCC.SetImage(UIImage.FromFile("graphicCCButtonOff.png"), UIControlState.Normal);
            buttonCC.SetImage(UIImage.FromFile("graphicCCButtonOff.png"), UIControlState.Highlighted);
            buttonCC.SetImage(UIImage.FromFile("graphicCCButtonOff.png"), UIControlState.Disabled);
            buttonCC.Frame = new CGRect(buttonCCXLoc, buttonCCYLoc, buttonCCWidth, buttonCCHeight);
            buttonCC.TouchDown += HandleCCTouchDown;
        }

        public void LoadSettingsButton(float screenWidth, float screenHeight, float sizeSubtract,float controlAdjustSettings)
        {
            float buttonSettingsWidth = screenWidth / (12 + sizeSubtract);
            float buttonSettingsHeight = buttonSettingsWidth;
            float buttonSettingsXLoc = (float)((screenWidth - buttonSettingsWidth) * (0.91));
            float buttonSettingsYLoc = (float)((screenHeight - buttonSettingsHeight) / 1.67) * controlAdjustSettings;
            buttonSettings = UIButton.FromType(UIButtonType.Custom);
            buttonSettings.SetImage(UIImage.FromFile("graphicSettingsButtonOff.png"), UIControlState.Normal);
            buttonSettings.SetImage(UIImage.FromFile("graphicSettingsButtonOff.png"), UIControlState.Highlighted);
            buttonSettings.SetImage(UIImage.FromFile("graphicSettingsButtonOff.png"), UIControlState.Disabled);
            buttonSettings.Frame = new CGRect(buttonSettingsXLoc, buttonSettingsYLoc, buttonSettingsWidth, buttonSettingsHeight);
            buttonSettings.Hidden = true;
            buttonSettings.TouchDown += HandleSettingsTouchDown;
        }

        public void LoadInfoButton(float screenWidth, float screenHeight, float sizeSubtract)
        {
            float buttonInfoWidth = screenWidth / (12 + sizeSubtract);
            float buttonInfoHeight = buttonInfoWidth;
            float buttonInfoXLoc = (float)((screenWidth - buttonInfoWidth) * (0.95));
            float buttonInfoYLoc = (float)((screenHeight - buttonInfoHeight) / 1.02);
            buttonInfo = UIButton.FromType(UIButtonType.Custom);
            buttonInfo.SetImage(UIImage.FromFile("graphicInfoButtonOff.png"), UIControlState.Normal);
            buttonInfo.SetImage(UIImage.FromFile("graphicInfoButtonOff.png"), UIControlState.Highlighted);
            buttonInfo.SetImage(UIImage.FromFile("graphicInfoButtonOff.png"), UIControlState.Disabled);
            buttonInfo.Frame = new CGRect(buttonInfoXLoc, buttonInfoYLoc, buttonInfoWidth, buttonInfoHeight);
            //buttonInfo.TouchDown += HandleInfoTouchDown;
        }

        public void LoadClockButton(float screenWidth, float screenHeight, float sizeSubtract)
        {
            float buttonClockWidth = screenWidth / (12 + sizeSubtract);
            float buttonClockHeight = buttonClockWidth;
            float buttonClockXLoc = (float)((screenWidth - buttonClockWidth) * (0.05));
            float buttonClockYLoc = (float)((screenHeight - buttonClockHeight) / 1.02);
            buttonClock = UIButton.FromType(UIButtonType.Custom);
            buttonClock.SetImage(UIImage.FromFile("graphicClockButtonOff.png"), UIControlState.Normal);
            buttonClock.SetImage(UIImage.FromFile("graphicClockButtonOff.png"), UIControlState.Highlighted);
            buttonClock.SetImage(UIImage.FromFile("graphicClockButtonOff.png"), UIControlState.Disabled);
            buttonClock.Frame = new CGRect(buttonClockXLoc, buttonClockYLoc, buttonClockWidth, buttonClockHeight);
            //buttonClock.TouchDown += HandleClockTouchDown;
        }

        public void LoadBPMButton(float screenWidth, float screenHeight, float sizeSubtract,float controlAdjustBPM)
        {
            float buttonBPMWidth = screenWidth / (12 + sizeSubtract);
            float buttonBPMHeight = buttonBPMWidth;
            float buttonBPMXLoc = (float)((screenWidth - buttonBPMWidth) * (0.05));
            float buttonBPMYLoc = (float)((screenHeight - buttonBPMHeight) / 1.07) * controlAdjustBPM;
            buttonBPM = UIButton.FromType(UIButtonType.Custom);
            buttonBPM.SetImage(UIImage.FromFile("graphicBPMButtonOff.png"), UIControlState.Normal);
            buttonBPM.SetImage(UIImage.FromFile("graphicBPMButtonOff.png"), UIControlState.Highlighted);
            buttonBPM.SetImage(UIImage.FromFile("graphicBPMButtonOff.png"), UIControlState.Disabled);
            buttonBPM.Frame = new CGRect(buttonBPMXLoc, buttonBPMYLoc, buttonBPMWidth, buttonBPMHeight);
            //buttonBPM.TouchDown += HandleBPMTouchDown;
        }

        public void LoadARButton(float screenWidth, float screenHeight, float sizeSubtract, float controlAdjustAR)
        {
            float buttonARWidth = screenWidth / (12 + sizeSubtract);
            float buttonARHeight = buttonARWidth;
            float buttonARXLoc = (float)((screenWidth - buttonARWidth) * (0.05));
            float buttonARYLoc = (float)((screenHeight - buttonARHeight) * (0.23)) * controlAdjustAR;
            buttonAR = UIButton.FromType(UIButtonType.Custom);
            buttonAR.SetImage(UIImage.FromFile("graphicARButtonOff.png"), UIControlState.Normal);
            buttonAR.SetImage(UIImage.FromFile("graphicARButtonOff.png"), UIControlState.Highlighted);
            buttonAR.SetImage(UIImage.FromFile("graphicARButtonOff.png"), UIControlState.Disabled);
            buttonAR.Frame = new CGRect(buttonARXLoc, buttonARYLoc, buttonARWidth, buttonARHeight);
            buttonAR.TouchDown += HandleARTouchDown;
        }

        public void LoadRandomButton(float screenWidth, float screenHeight, float sizeSubtract, float controlAdjustRandoms)
        {
            float buttonRandomWidth = screenWidth / (12 + sizeSubtract);
            float buttonRandomHeight = buttonRandomWidth;
            float buttonRandomXLoc = (float)((screenWidth - buttonRandomWidth) * (0.09));
            float buttonRandomYLoc = (float)((screenHeight - buttonRandomHeight) / 1.415) * controlAdjustRandoms;
            buttonRandom = UIButton.FromType(UIButtonType.Custom);
            buttonRandom.SetImage(UIImage.FromFile("graphicRandomButtonOff.png"), UIControlState.Normal);
            buttonRandom.SetImage(UIImage.FromFile("graphicRandomButtonOn.png"), UIControlState.Highlighted);
            buttonRandom.SetImage(UIImage.FromFile("graphicRandomButtonOff.png"), UIControlState.Disabled);
            buttonRandom.Frame = new CGRect(buttonRandomXLoc, buttonRandomYLoc, buttonRandomWidth, buttonRandomHeight);
            buttonRandom.TouchDown += HandleRandomTouchDown;
        }

        public void LoadAutoButton(float screenWidth, float screenHeight, float sizeSubtract, float controlAdjustRandoms)
        {
            float buttonAutoWidth = screenWidth / (12 + sizeSubtract);
            float buttonAutoHeight = buttonAutoWidth;
            float buttonAutoXLoc = (float)((screenWidth - buttonAutoWidth) * (0.91));
            float buttonAutoYLoc = (float)((screenHeight - buttonAutoHeight) / 1.415) * controlAdjustRandoms;
            buttonAuto = UIButton.FromType(UIButtonType.Custom);
            buttonAuto.SetImage(UIImage.FromFile("graphicAutoButtonOff.png"), UIControlState.Normal);
            buttonAuto.SetImage(UIImage.FromFile("graphicAutoButtonOff.png"), UIControlState.Highlighted);
            buttonAuto.SetImage(UIImage.FromFile("graphicAutoButtonOff.png"), UIControlState.Disabled);
            buttonAuto.Frame = new CGRect(buttonAutoXLoc, buttonAutoYLoc, buttonAutoWidth, buttonAutoHeight);
            buttonAuto.TouchDown += HandleAutoTouchDown;
        }

        public void LoadTimeandMidiButtons(float screenWidth, float screenHeight, float sizeSubtract, float controlAdjustMode)
        {
            float buttonTimeWidth = (float)(screenWidth / (2.1 + sizeSubtract));
            float buttonTimeHeight = buttonTimeWidth * 3 / 5;
            float buttonTimeXLoc = (float)((screenWidth - buttonTimeWidth) / 4.5);
            float buttonTimeYLoc = (float)((screenHeight - buttonTimeHeight) / 1.08) * controlAdjustMode;
            buttonTime = UIButton.FromType(UIButtonType.Custom);
            buttonTime.SetImage(UIImage.FromFile("graphicTimeButtonOff.png"), UIControlState.Normal);
            buttonTime.SetImage(UIImage.FromFile("graphicTimeButtonOff.png"), UIControlState.Highlighted);
            buttonTime.SetImage(UIImage.FromFile("graphicTimeButtonOff.png"), UIControlState.Disabled);
            buttonTime.Frame = new CGRect(buttonTimeXLoc, buttonTimeYLoc, buttonTimeWidth, buttonTimeHeight);

            buttonMidi = UIButton.FromType(UIButtonType.Custom);
            buttonMidi.SetImage(UIImage.FromFile("graphicMidiButtonOff.png"), UIControlState.Normal);
            buttonMidi.SetImage(UIImage.FromFile("graphicMidiButtonOff.png"), UIControlState.Highlighted);
            buttonMidi.SetImage(UIImage.FromFile("graphicMidiButtonOff.png"), UIControlState.Disabled);
            buttonMidi.Frame = new CGRect(screenWidth - buttonTimeXLoc - buttonTimeWidth, buttonTimeYLoc, buttonTimeWidth, buttonTimeHeight);

            //Button Event Handler Delegates
            buttonTime.TouchDown += HandleTimeTouchDown;
            buttonMidi.TouchDown += HandleMidiTouchDown;
        }

        public void LoadCCIncButtons(float screenWidth, float screenHeight, float sizeSubtract, float controlAdjustCCInc, float sizeIncrease)
        {
            float buttonIncWidth = (float)screenWidth / (8 + sizeSubtract);
            float buttonIncHeight = (float)(buttonIncWidth * 0.8 * sizeIncrease);
            float buttonIncXLoc = (float)((screenWidth - buttonIncWidth) / 1.7);
            float buttonIncYLoc = (float)((screenHeight - buttonIncHeight) / 1.56) * controlAdjustCCInc;
            buttonPlus1 = UIButton.FromType(UIButtonType.Custom);
            buttonPlus1.SetImage(UIImage.FromFile("graphicPlus1ButtonOff.png"), UIControlState.Normal);
            buttonPlus1.SetImage(UIImage.FromFile("graphicPlus1ButtonOn.png"), UIControlState.Highlighted);
            buttonPlus1.SetImage(UIImage.FromFile("graphicPlus1ButtonOff.png"), UIControlState.Disabled);
            buttonPlus1.Frame = new CGRect(buttonIncXLoc, buttonIncYLoc, buttonIncWidth, buttonIncHeight);
            buttonPlus1.Hidden = true;

            buttonPlus10 = UIButton.FromType(UIButtonType.Custom);
            buttonPlus10.SetImage(UIImage.FromFile("graphicPlus10ButtonOff.png"), UIControlState.Normal);
            buttonPlus10.SetImage(UIImage.FromFile("graphicPlus10ButtonOn.png"), UIControlState.Highlighted);
            buttonPlus10.SetImage(UIImage.FromFile("graphicPlus10ButtonOff.png"), UIControlState.Disabled);
            buttonPlus10.Frame = new CGRect(buttonIncXLoc * (1.25), buttonIncYLoc, buttonIncWidth, buttonIncHeight);
            buttonPlus10.Hidden = true;

            buttonMinus1 = UIButton.FromType(UIButtonType.Custom);
            buttonMinus1.SetImage(UIImage.FromFile("graphicMinus1ButtonOff.png"), UIControlState.Normal);
            buttonMinus1.SetImage(UIImage.FromFile("graphicMinus1ButtonOn.png"), UIControlState.Highlighted);
            buttonMinus1.SetImage(UIImage.FromFile("graphicMinus1ButtonOff.png"), UIControlState.Disabled);
            buttonMinus1.Frame = new CGRect(screenWidth - buttonIncXLoc - buttonIncWidth, buttonIncYLoc, buttonIncWidth, buttonIncHeight);
            buttonMinus1.Hidden = true;

            buttonMinus10 = UIButton.FromType(UIButtonType.Custom);
            buttonMinus10.SetImage(UIImage.FromFile("graphicMinus10ButtonOff.png"), UIControlState.Normal);
            buttonMinus10.SetImage(UIImage.FromFile("graphicMinus10ButtonOn.png"), UIControlState.Highlighted);
            buttonMinus10.SetImage(UIImage.FromFile("graphicMinus10ButtonOff.png"), UIControlState.Disabled);
            buttonMinus10.Frame = new CGRect(screenWidth - buttonIncXLoc * (1.25) - buttonIncWidth, buttonIncYLoc, buttonIncWidth, buttonIncHeight);
            buttonMinus10.Hidden = true;

            buttonPlus1.TouchDown += HandlePlus1TouchDown;
            buttonPlus10.TouchDown += HandlePlus10TouchDown;
            buttonMinus1.TouchDown += HandleMinus1TouchDown;
            buttonMinus10.TouchDown += HandleMinus10TouchDown;
        }

        public void LoadSegmented(float screenWidth, float screenHeight, float controlAdjustPattern, float segHeight)
        {
            float segWidth = (float)(screenWidth / 1.2);
            float segXLoc = (float)((screenWidth - segWidth) / 2);
            float segYLoc = (float)((screenHeight - segHeight) / 1.29) * controlAdjustPattern;
            segmentedPattern = new UISegmentedControl("1", "2", "3", "4", "5", "6", "7", "8");
            segmentedPattern.Frame = new CGRect(segXLoc, segYLoc, segWidth, segHeight);
            segmentedPattern.TintColor = UIColor.Green;
            segmentedPattern.SelectedSegment = 0;
            segmentedPattern.ValueChanged += PnumChange;
        }

        public void LoadPatternLabel(float screenWidth, float screenHeight, float textAdjustPattern, float segHeight)
        {
            float labelPatternWidth = (float)(screenWidth / 1.8);
            float labelPatternHeight = (float)(screenHeight / 8);
            float labelPatternXLoc = (float)((screenWidth - labelPatternWidth) / 2);
            float labelPatternYLoc = (float)(((screenHeight - segHeight) / 1.57) * textAdjustPattern);
            labelPattern = new UILabel();
            labelPattern.Text = "Pattern 1: Up & Down";
            labelPattern.Frame = new CGRect(labelPatternXLoc, labelPatternYLoc, labelPatternWidth, labelPatternHeight);
            labelPattern.TextAlignment = UITextAlignment.Center;
            labelPattern.TextColor = UIColor.White;
            labelPattern.Font = UIFont.SystemFontOfSize(100);
            labelPattern.AdjustsFontSizeToFitWidth = true;
        }

        public void LoadRateSlider(float screenWidth, float screenHeight, float controlAdjustRate, float sliderHeight)
        {
            float sliderWidth = (float)(screenWidth / 1.2);
            float sliderXLoc = (float)((screenWidth - sliderWidth) / 2);
            float sliderYLoc = (float)((screenHeight - sliderHeight) / 1.49) * controlAdjustRate;
            sliderRate = new UISlider();
            sliderRate.Frame = new CGRect(sliderXLoc, sliderYLoc, sliderWidth, sliderHeight);
            sliderRate.MinValue = 0;
            sliderRate.MaxValue = 127;
            sliderRate.Value = 63;
            sliderRate.TintColor = UIColor.Green;
            sliderRate.ValueChanged += HandleRateSliderChange;
        }

        public void LoadRateLabel(float screenWidth, float screenHeight, float textAdjustRate, float sliderHeight)
        {
            float labelRateWidth = (float)(screenWidth / 1.8);
            float labelRateHeight = (float)(screenHeight / 8);
            float labelRateXLoc = (float)((screenWidth - labelRateWidth) / 2);
            float labelRateYLoc = (float)(((screenHeight - sliderHeight) / 1.61) * textAdjustRate);
            labelRate = new UILabel();
            labelRate.Frame = new CGRect(labelRateXLoc, labelRateYLoc, labelRateWidth, labelRateHeight);
            labelRate.Text = "Press Button Above";
            labelRate.TextAlignment = UITextAlignment.Center;
            labelRate.TextColor = UIColor.White;
            labelRate.Font = UIFont.SystemFontOfSize(100);
            labelRate.AdjustsFontSizeToFitWidth = true;
        }

        public void LoadRangeSlider(float screenWidth, float screenHeight, float controlAdjustRange, float rangeFontSize)
        {
            float rangeWidth = (float)(screenWidth / 1.15);
            float rangeHeight = (float)(screenHeight / 10);
            float rangeXLoc = (float)((screenWidth - rangeWidth) / 2);
            float rangeYLoc = (float)((screenHeight - rangeHeight) / 16) * controlAdjustRange;
            rangeSlider = new RangeSliderControl();
            rangeSlider.ShowTextAboveThumbs = true;
            rangeSlider.TextSize = rangeFontSize;
            rangeSlider.Frame = new CGRect(rangeXLoc, rangeYLoc, rangeWidth, rangeHeight);
            rangeSlider.TintColor = UIColor.Green;
            rangeSlider.MaximumValue = 127;
            rangeSlider.MinimumValue = 0;
            rangeSlider.TextColor = UIColor.White;
            rangeSlider.LowerValue = 0;
            rangeSlider.UpperValue = 127;
            rangeSlider.DragCompleted += (object sender, EventArgs e) =>
            {
                var myObj = (RangeSliderControl)sender;
                myMidiModulation.Maximum = (int)myObj.UpperValue;
                myMidiModulation.Minimum = (int)myObj.LowerValue;
            };
        }

        public void AddSubviewsToMainview()
        {
            //Add Subviews to main view
            View.AddSubview(myCircularProgressBar);
            View.AddSubview(buttonOnOff);
            View.AddSubview(buttonTime);
            View.AddSubview(segmentedPattern);
            View.AddSubview(labelPattern);
            View.AddSubview(buttonMidi);
            View.AddSubview(sliderRate);
            View.AddSubview(labelRate);
            View.AddSubview(rangeSlider);
            View.AddSubview(buttonReverse);
            View.AddSubview(buttonCC);
            View.AddSubview(buttonSettings);
            View.AddSubview(buttonInfo);
            View.AddSubview(buttonBPM);
            View.AddSubview(buttonClock);
            View.AddSubview(buttonRandom);
            View.AddSubview(buttonAuto);
            View.AddSubview(buttonAR);
            View.AddSubview(buttonPlus1);
            View.AddSubview(buttonPlus10);
            View.AddSubview(buttonMinus1);
            View.AddSubview(buttonMinus10);
        }

    }
}
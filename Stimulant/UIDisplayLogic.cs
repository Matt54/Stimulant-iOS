using System;
using UIKit;
using CoreGraphics;
using Xamarin.RangeSlider;
using System.Diagnostics;
using Foundation;

using System.Drawing;

namespace Stimulant
{
    public partial class ViewController
    {

        //These variables are used to update the display during runtime
        CGRect bigStartSize;
        CGRect smallStartSize;
        CGRect sceneStartSize;

        CGRect C_progressSize;
        CGRect C_progressSceneSize;

        CGRect H_progressSize;

        CGRect frameReverseOrig;
        CGRect frameReverseScene;

        CGRect frameRunningSymbol;

        int runningSymbol_lineWidth;

        int C_lineWidth;
        int C_lineWidthSmall;
        int H_lineWidth;

        float widthScenes;
        float marginScenes;
        float locationScenes;

        UIColor barColor;
        bool UIHelper;
        bool highRes;

        //Declare View References

        UILabel labelRange;
        UIButton buttonAR;
        UIButton buttonLocation;
        UISlider sliderHidden;
        UISlider sliderCC;
        RangeSliderControl rangeSlider;
        RangeSliderControl rangeScenesSlider;
        

        CircularProgressBar myCircularProgressBar;
        HorizontalProgressBar myHorizontalProgressBar;
        UIButton buttonOnOff;

        UILabel labelRate;
        UISlider sliderRate;
        UIButton buttonSettings;
        UIButton buttonRandom;
        UIButton buttonAutoRate;

        UILabel labelPattern;
        UISegmentedControl segmentedPattern;
        UIButton buttonReverse;
        UIButton buttonCC;
        UIButton buttonBPM;
        UIButton buttonAutoPattern;
        UIButton buttonPlus1;
        UIButton buttonPlus10;
        UIButton buttonMinus1;
        UIButton buttonMinus10;
        UIButton buttonTap;

        UIButton buttonTrigger;
        UIButton buttonAuto;
        UILabel labelMode;
        UILabel labelDetails;

        UIButton buttonTime;
        UIButton buttonMidi;
        UIButton buttonClock;

        UITextField textFieldBPM;
        UIButton buttonInfo;

        UIButton buttonScenes;
        UIButton buttonArrange;

        RunningSymbol myRunningSymbol;

        /*
        UIButton buttonScene0;
        UIButton buttonScene1;
        UIButton buttonScene2;
        UIButton buttonScene3;
        UIButton buttonScene4;
        UIButton buttonScene5;
        UIButton buttonScene6;
        UIButton buttonScene7;
        */

        UIButton[] buttonArray = new UIButton[8];
        int sceneSelected;

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
            float textAdjustMode;
            float textAdjustDetails;
            float textAdjustRate;
            float textAdjustRange;
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
            float controlAdjustTap;
            float controlAdjustTrigger;
            float controlAdjustScenes;
            //float controlAdjustTap;
            float controlAdjustHidden;
            float controlAdjustProgress;
            float controlAdjustHorizontalProgress;
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

            float rangeFontSize = screenHeight / 30;
            sizeSubtract = (float)2.165333 / screenAspectRatio;

            //SetBackground(device); //This sets so many values (and is also not really just setting the background) - we should unpack this.
            //^Just leave the switch statement inside LoadDisplay

            switch (device)
            {
                case "8":
                    background = "Back8.png";
                    //overlay = "Over8.png";
                    textAdjustMode = 1f;
                    textAdjustDetails = 1.01f;
                    textAdjustRate = 0.98f;// 0.96f;
                    textAdjustRange = 0.3f;
                    textAdjustPattern = 0.98f;//0.975f;
                    controlAdjustRate = 1f;//0.99f;
                    controlAdjustPattern = 1f;//0.99f;
                    controlAdjustRange = 0.6f; //0.85f;
                    controlAdjustSettings = 0.985f;//0.98f;
                    controlAdjustRandoms = 0.79f;//0.99f;
                    controlAdjustAR = 0.4f;
                    controlAdjustMode = 1.0f;
                    controlAdjustCCInc = 1f;//0.99f;
                    controlAdjustBPM = 1f;
                    controlAdjustTrigger = 1.026f;
                    controlAdjustTap = 1f;
                    controlAdjustHidden = 0.475f;
                    buttonYAdjust = 0.975f;
                    controlAdjustProgress = 1.01f;
                    controlAdjustHorizontalProgress = 1f;
                    controlAdjustScenes = 1f;
                    sizeIncrease = 1.0f;
                    sizeSubtract = 1.0f;
                    break;
                case "8Plus":
                    background = "Back8Plus.png";
                    overlay = "Over8Plus.png";
                    textAdjustMode = 1f;
                    textAdjustDetails = 1.01f;
                    textAdjustRate = 0.98f;// 0.96f;
                    textAdjustRange = 0.3f;
                    textAdjustPattern = 0.98f;//0.975f;
                    controlAdjustRate = 1f;//0.99f;
                    controlAdjustPattern = 1f;//0.99f;
                    controlAdjustRange = 0.6f; //0.85f;
                    controlAdjustSettings = 0.985f;//0.98f;
                    controlAdjustRandoms = 1f;//0.99f;
                    controlAdjustAR = 0.4f;
                    controlAdjustMode = 1.0f;
                    controlAdjustCCInc = 1f;//0.99f;
                    controlAdjustBPM = 1f;
                    controlAdjustTrigger = 1.026f;
                    controlAdjustTap = 1f;
                    controlAdjustHidden = 0.475f;
                    buttonYAdjust = 0.975f;
                    controlAdjustProgress = 1.01f;
                    controlAdjustHorizontalProgress = 1f;
                    controlAdjustScenes = 1f;
                    sizeIncrease = 1.0f;
                    sizeSubtract = 1.0f;
                    break;
                case "XS":
                    background = "BackXS.png";
                    overlay = "OverXS.png";
                    textAdjustMode = 1f;
                    textAdjustDetails = 1f;
                    textAdjustRate = 0.995f;
                    textAdjustRange = 1f;
                    textAdjustPattern = 1f;
                    controlAdjustRate = 1f;
                    controlAdjustPattern = 1.0f;
                    controlAdjustRange = 0.98f;
                    controlAdjustSettings = 0.995f;
                    controlAdjustRandoms = 1.0f;
                    controlAdjustAR = 1.00f;
                    controlAdjustMode = 0.952f;
                    controlAdjustCCInc = 1.00f;
                    controlAdjustBPM = 1f;
                    controlAdjustTrigger = 1f;
                    controlAdjustTap = 1f;
                    controlAdjustHidden = 0.9825f;
                    buttonYAdjust = 1.005f;
                    controlAdjustProgress = 1.0f;
                    controlAdjustHorizontalProgress = 1f;
                    controlAdjustScenes = 1f;
                    sizeIncrease = 1f;
                    sizeSubtract = 1f;
                    highRes = true;
                    break;
                case "XR":
                    background = "BackXSMax.png";
                    overlay = "OverXSMax.png";
                    textAdjustMode = 1f;
                    textAdjustDetails = 1f;
                    textAdjustRate = 1.015f;
                    textAdjustRange = 1f;
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
                    controlAdjustTrigger = 1f;
                    controlAdjustTap = 1f;
                    controlAdjustHidden = 1f;
                    controlAdjustProgress = 1.0f;
                    controlAdjustHorizontalProgress = 1f;
                    controlAdjustScenes = 1f;
                    buttonYAdjust = 1.0f;
                    sizeIncrease = 1.0f;

                    break;
                case "iPad":
                    background = "BackiPad.png";
                    overlay = "OveriPad.png";
                    textAdjustMode = 1.03f;
                    textAdjustDetails = 1.05f;
                    textAdjustRate = 1.04f;
                    textAdjustRange = 0.5f;
                    textAdjustPattern = 1.03f;
                    controlAdjustRate = 1.025f;
                    controlAdjustPattern = 1.01f;
                    controlAdjustRange = 0.7f;
                    controlAdjustSettings = 1.005f;
                    controlAdjustRandoms = 0.85f;
                    controlAdjustAR = 0.5f;
                    controlAdjustMode = 1f;
                    controlAdjustCCInc = 1.01f;
                    controlAdjustBPM = 0.95f;
                    controlAdjustTrigger = 1.026f;
                    controlAdjustTap = 1.01f;
                    controlAdjustHidden = 0.615f;
                    controlAdjustProgress = 1.01f;
                    controlAdjustHorizontalProgress = 1f;
                    controlAdjustScenes = 1f;
                    buttonYAdjust = 0.99f;
                    sizeIncrease = 1.0f;
                    sizeSubtract = 1.335f;
                    break;
                case "iPadPro":
                    background = "BackiPadPro.png";
                    overlay = "OveriPadPro.png";
                    textAdjustMode = 1f;
                    textAdjustDetails = 1f;
                    textAdjustRate = 1.04f;
                    textAdjustRange = 1f;
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
                    controlAdjustTrigger = 0.95f;
                    controlAdjustTap = 1f;
                    controlAdjustHidden = 1f;
                    controlAdjustProgress = 1.0f;
                    controlAdjustHorizontalProgress = 1f;
                    controlAdjustScenes = 1f;
                    buttonYAdjust = 1.0f;
                    sizeIncrease = 1.0f;
                    sizeSubtract = 1f;
                    break;
                case "iPadPro11":
                    background = "BackiPadPro11.png";
                    overlay = "OveriPadPro11.png";
                    textAdjustMode = 1f;
                    textAdjustDetails = 1f;
                    textAdjustRate = 1.053f;
                    textAdjustRange = 1f;
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
                    controlAdjustTrigger = 0.95f;
                    controlAdjustTap = 1f;
                    controlAdjustHidden = 1f;
                    controlAdjustProgress = 1.0f;
                    controlAdjustHorizontalProgress = 1f;
                    controlAdjustScenes = 1f;
                    buttonYAdjust = 1.0f;
                    sizeIncrease = 1.0f;
                    sizeSubtract = 1f;
                    break;
                case "iPadNext":
                    background = "BackiPadNext.png";
                    overlay = "OveriPadNext.png";
                    textAdjustMode = 1f;
                    textAdjustDetails = 1f;
                    textAdjustRate = 1.055f;
                    textAdjustRange = 1f;
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
                    controlAdjustTrigger = 0.95f;
                    controlAdjustTap = 1f;
                    controlAdjustHidden = 1f;
                    controlAdjustProgress = 1.0f;
                    controlAdjustHorizontalProgress = 1f;
                    controlAdjustScenes = 1f;
                    buttonYAdjust = 1.0f;
                    sizeIncrease = 1.0f;
                    sizeSubtract = 1f;
                    break;
                default:
                    background = "BackXS.png";
                    overlay = "OverXS.png";
                    textAdjustMode = 1f;
                    textAdjustDetails = 1f;
                    textAdjustRate = 0.995f;
                    textAdjustRange = 1f;
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
                    controlAdjustTrigger = 1f;
                    controlAdjustTap = 1f;
                    controlAdjustHidden = 1f;
                    controlAdjustProgress = 1.0f;
                    controlAdjustHorizontalProgress = 1f;
                    controlAdjustScenes = 1f;
                    buttonYAdjust = 1.005f;
                    sizeIncrease = 1.0f;
                    break;
            }

            //Add the overlay view (Just UI Graphics - no controls)
            /*if (UIImage.FromBundle(overlay) != null)
            {
                this.View.InsertSubview(new UIImageView(UIImage.FromBundle(overlay)), 0);
            }
            */

            //Add the background Graphic
            if (UIImage.FromFile(background) != null)
            {
                View.BackgroundColor = UIColor.FromPatternImage(UIImage.FromFile(background));
            }

            LoadStartButton(screenWidth, screenHeight, buttonYAdjust, sizeSubtract);
            LoadProgressBar(screenWidth, screenHeight, buttonYAdjust, sizeSubtract, controlAdjustProgress);
            LoadHorizontalProgressBar(screenWidth, screenHeight, buttonYAdjust, sizeSubtract, controlAdjustHorizontalProgress);
            //LoadReverseButton(screenWidth, screenHeight, sizeSubtract);
            LoadCCButton(screenWidth, screenHeight, sizeSubtract, controlAdjustSettings);
            LoadSettingsButton(screenWidth, screenHeight, sizeSubtract, controlAdjustSettings);
            LoadARButton(screenWidth, screenHeight, sizeSubtract, controlAdjustAR);
            //LoadLocationButton(screenWidth, screenHeight, sizeSubtract, controlAdjustAR);
            LoadInfoButton(screenWidth, screenHeight, sizeSubtract);
            //LoadClockButton(screenWidth, screenHeight, sizeSubtract);
            //LoadBPMButton(screenWidth, screenHeight, sizeSubtract, controlAdjustBPM);
            LoadTriggerButton(screenWidth, screenHeight, sizeSubtract, controlAdjustTrigger);
            LoadRandomButton(screenWidth, screenHeight, sizeSubtract, controlAdjustRandoms, controlAdjustScenes);
            //LoadAutoButton(screenWidth, screenHeight, sizeSubtract, controlAdjustRandoms);
            LoadTimeandMidiButtons(screenWidth, screenHeight, sizeSubtract, controlAdjustMode);
            LoadModeLabel(screenWidth, screenHeight, textAdjustMode);
            LoadDetailsLabel(screenWidth, screenHeight, textAdjustDetails);
            LoadSegmented(screenWidth, screenHeight, controlAdjustPattern, segHeight);
            LoadRateSlider(screenWidth, screenHeight, controlAdjustRate, sliderHeight);
            LoadPatternLabel(screenWidth, screenHeight, textAdjustPattern, segHeight);
            LoadRateLabel(screenWidth, screenHeight, textAdjustRate, sliderHeight);
            LoadRangeLabel(screenWidth, screenHeight, textAdjustRange, rangeFontSize);
            LoadRangeSlider(screenWidth, screenHeight, controlAdjustRange, rangeFontSize);
            LoadCCIncButtons(screenWidth, screenHeight, sizeSubtract, controlAdjustCCInc, sizeIncrease);
            LoadTapButton(screenWidth, screenHeight, sizeSubtract, controlAdjustTap);
            LoadHiddenSlider(screenWidth, screenHeight, controlAdjustHidden, sliderHeight);
            LoadBPMTextField(screenWidth, screenHeight);

            LoadSceneButtons(screenWidth, screenHeight);

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
            Debug.Print(h.ToString());
            Debug.Print(dev);
            return dev;
        }

        /*
        public void SetBackground(string dev)
        {
            
        }
        */

        public void LoadRangeLabel(float screenWidth, float screenHeight, float textAdjustRange, float rangeFontSize)
        {
            float labelRangeWidth = (float)(screenWidth / 1.6);
            float labelRangeHeight = (float)(screenHeight / 12);
            float labelRangeXLoc = (float)((screenWidth - labelRangeWidth) / 2);
            float labelRangeYLoc = (float)(((screenHeight - labelRangeHeight) / 15) * textAdjustRange);
            labelRange = new UILabel();
            labelRange.Frame = new CGRect(labelRangeXLoc, labelRangeYLoc, labelRangeWidth, labelRangeHeight);
            labelRange.Text = "Modulation Range";
            labelRange.TextAlignment = UITextAlignment.Center;

            labelRange.TextColor = UIColor.Black;
            //labelRange.BackgroundColor = UIColor.Green;
            labelRange.Font = UIFont.SystemFontOfSize(100);

            labelRange.AdjustsFontSizeToFitWidth = true;
            labelRange.Font = UIFont.SystemFontOfSize((float)(rangeFontSize * 1.3));
            //labelRange.Frame = new CGRect(labelRangeXLoc, labelRangeYLoc, labelRangeWidth, labelRangeHeight/2);
            //labelRange.SizeToFit();
        }

        public void LoadARButton(float screenWidth, float screenHeight, float sizeSubtract, float controlAdjustAR)
        {
            float buttonARWidth = screenWidth / (12 * sizeSubtract);
            float buttonARHeight = buttonARWidth;
            float buttonARXLoc = (float)((screenWidth - buttonARWidth) * (0.98));
            float buttonARYLoc = (float)((screenHeight - buttonARHeight) * (0.08)) * controlAdjustAR;
            buttonAR = UIButton.FromType(UIButtonType.Custom);
            buttonAR.SetImage(UIImage.FromFile("graphicARButtonOff.png"), UIControlState.Normal);
            buttonAR.SetImage(UIImage.FromFile("graphicARButtonOff.png"), UIControlState.Highlighted);
            buttonAR.SetImage(UIImage.FromFile("graphicARButtonDisabled.png"), UIControlState.Disabled);
            //buttonAR.Enabled = false;
            buttonAR.Frame = new CGRect(buttonARXLoc, buttonARYLoc, buttonARWidth, buttonARHeight);
            buttonAR.TouchDown += HandleARTouchDown;

            buttonLocation = UIButton.FromType(UIButtonType.Custom);
            buttonLocation.SetImage(UIImage.FromFile("graphicLocationButtonOff.png"), UIControlState.Normal);
            buttonLocation.SetImage(UIImage.FromFile("graphicLocationButtonOff.png"), UIControlState.Highlighted);
            buttonLocation.SetImage(UIImage.FromFile("graphicLocationButtonDisabled.png"), UIControlState.Disabled);
            buttonLocation.Frame = new CGRect((float)((screenWidth - buttonARWidth) * (0.02)), buttonARYLoc, buttonARWidth, buttonARHeight);
            buttonLocation.Enabled = false;
            buttonLocation.TouchDown += HandleLocationTouchDown;
        }

        public void LoadRangeSlider(float screenWidth, float screenHeight, float controlAdjustRange, float rangeFontSize)
        {
            float rangeWidth = (float)(screenWidth / 1.09);
            float rangeHeight = (float)(screenHeight / 10);
            float rangeXLoc = (float)((screenWidth - rangeWidth) / 2);
            float rangeYLoc = (float)((screenHeight - rangeHeight) / 7) * controlAdjustRange;
            rangeSlider = new RangeSliderControl();
            rangeSlider.ShowTextAboveThumbs = true;
            rangeSlider.TextSize = rangeFontSize;
            rangeSlider.Frame = new CGRect(rangeXLoc, rangeYLoc, rangeWidth, rangeHeight);
            rangeSlider.TintColor = UIColor.Black;
            rangeSlider.MaximumValue = 127;
            rangeSlider.MinimumValue = 0;
            rangeSlider.TextColor = UIColor.Black;

            rangeSlider.LowerValue = 0;
            rangeSlider.UpperValue = 127;
            rangeSlider.DragCompleted += (object sender, EventArgs e) =>
            {
                var myObj = (RangeSliderControl)sender;
                if (!myMidiModulation.IsSceneMode)
                {
                    myMidiModulation.Maximum = (int)myObj.UpperValue;
                    myMidiModulation.Minimum = (int)myObj.LowerValue;
                }
                else
                {
                    for(int ii = 0; ii < 8; ii++)
                    {
                        if (sceneArray[ii].IsSelected)
                        {
                            sceneArray[ii].Maximum = (int)myObj.UpperValue;
                            sceneArray[ii].Minimum = (int)myObj.LowerValue;
                        }
                    }
                }
            };
        }

        public void LoadHiddenSlider(float screenWidth, float screenHeight, float controlAdjustHidden, float sliderHeight)
        {
            float sliderWidth = (float)(screenWidth / 1.09);
            float sliderXLoc = (float)((screenWidth - sliderWidth) / 2);
            float sliderYLoc = (float)((screenHeight - sliderHeight) / 8.19) * controlAdjustHidden;
            sliderHidden = new UISlider();
            sliderHidden.Frame = new CGRect(sliderXLoc, sliderYLoc, sliderWidth, sliderHeight);
            sliderHidden.MinValue = 0;
            sliderHidden.MaxValue = 127;
            sliderHidden.Value = 63;
            sliderHidden.TintColor = UIColor.Clear;
            sliderHidden.Hidden = true;
            //sliderHidden.MinimumTrackTintColor = UIColor.Clear;
            sliderHidden.SetMinTrackImage(new UIImage(), UIControlState.Normal);
            sliderHidden.SetMaxTrackImage(new UIImage(), UIControlState.Normal);
            sliderHidden.ValueChanged += HandleHiddenSliderChange;
            if (highRes)
            {
                sliderHidden.SetThumbImage(UIImage.FromFile("graphicLocationThumb@2x.png"), UIControlState.Normal);
            }
            else
            {
                sliderHidden.SetThumbImage(UIImage.FromFile("graphicLocationThumb.png"), UIControlState.Normal);
            }

            sliderCC = new UISlider();
            sliderCC.Frame = new CGRect(sliderXLoc, sliderYLoc, sliderWidth, sliderHeight);
            sliderCC.MinValue = 0;
            sliderCC.MaxValue = 127;
            sliderCC.Value = 63;
            sliderCC.TintColor = UIColor.Clear;
            sliderCC.Hidden = true;
            sliderCC.UserInteractionEnabled = false;
            //sliderCC.Enabled = false;
            //sliderHidden.MinimumTrackTintColor = UIColor.Clear;
            sliderCC.SetMinTrackImage(new UIImage(), UIControlState.Normal);
            sliderCC.SetMaxTrackImage(new UIImage(), UIControlState.Normal);

            if (highRes)
            {
                sliderCC.SetThumbImage(UIImage.FromFile("graphicValueThumb@2x.png"), UIControlState.Normal);
            }
            else
            {
                sliderCC.SetThumbImage(UIImage.FromFile("graphicValueThumb.png"), UIControlState.Normal);
            }
            //sliderCC.ValueChanged += HandleCCSliderChange;

        }

        public void LoadRandomButton(float screenWidth, float screenHeight, float sizeSubtract, float controlAdjustRandoms, float controlAdjustScenes)
        {
            float buttonRandomWidth = screenWidth / (4 + sizeSubtract);
            float buttonRandomHeight = buttonRandomWidth;
            float buttonRandomXLoc = 0; //(float)((screenWidth - buttonRandomWidth) * (0.09));
            float buttonRandomYLoc = (float)((screenHeight) / 4.4) * controlAdjustRandoms;
            buttonRandom = UIButton.FromType(UIButtonType.Custom);
            buttonRandom.SetImage(UIImage.FromFile("graphicRandomButtonOff.png"), UIControlState.Normal);
            buttonRandom.SetImage(UIImage.FromFile("graphicRandomButtonOn.png"), UIControlState.Highlighted);
            buttonRandom.SetImage(UIImage.FromFile("graphicRandomButtonDisabled.png"), UIControlState.Disabled);
            buttonRandom.Frame = new CGRect(buttonRandomXLoc, buttonRandomYLoc, buttonRandomWidth, buttonRandomHeight);
            buttonRandom.Enabled = false;
            buttonRandom.TouchDown += HandleRandomTouchDown;

            frameReverseOrig = new CGRect(screenWidth - buttonRandomWidth, buttonRandomYLoc, buttonRandomWidth, buttonRandomHeight);
            frameReverseScene = new CGRect(2 * buttonRandomWidth, buttonRandomYLoc * 1.795 * controlAdjustScenes, buttonRandomWidth, buttonRandomHeight);

            buttonReverse = UIButton.FromType(UIButtonType.Custom);
            buttonReverse.SetImage(UIImage.FromFile("graphicReverseButtonOff.png"), UIControlState.Normal);
            buttonReverse.SetImage(UIImage.FromFile("graphicReverseButtonOff.png"), UIControlState.Highlighted);
            buttonReverse.SetImage(UIImage.FromFile("graphicReverseButtonDisabled.png"), UIControlState.Disabled);
            buttonReverse.Frame = frameReverseOrig;
            buttonReverse.Enabled = false;
            buttonReverse.TouchDown += HandleReverseTouchDown;



            buttonScenes = UIButton.FromType(UIButtonType.Custom);
            buttonScenes.SetImage(UIImage.FromFile("graphicScenesButtonOff.png"), UIControlState.Normal);
            buttonScenes.SetImage(UIImage.FromFile("graphicScenesButtonOff.png"), UIControlState.Highlighted);
            buttonScenes.SetImage(UIImage.FromFile("graphicScenesButtonOff.png"), UIControlState.Disabled);
            buttonScenes.Frame = new CGRect(buttonRandomXLoc, buttonRandomYLoc * 1.795 * controlAdjustScenes, buttonRandomWidth, buttonRandomHeight);
            buttonScenes.TouchDown += HandleScenesTouchDown;

            buttonArrange = UIButton.FromType(UIButtonType.Custom);
            buttonArrange.SetImage(UIImage.FromFile("graphicArrangeButtonOff.png"), UIControlState.Normal);
            buttonArrange.SetImage(UIImage.FromFile("graphicArrangeButtonOff.png"), UIControlState.Highlighted);
            buttonArrange.SetImage(UIImage.FromFile("graphicArrangeButtonOff.png"), UIControlState.Disabled);
            buttonArrange.Frame = new CGRect(buttonRandomWidth, buttonRandomYLoc * 1.795 * controlAdjustScenes, buttonRandomWidth, buttonRandomHeight);
            buttonArrange.TouchDown += HandleArrangeTouchDown;
        }

        public void LoadStartButton(float screenWidth, float screenHeight, float buttonYAdjust, float sizeSubtract)
        {
            //float buttonRandomWidth = screenWidth / (4 + sizeSubtract);
            //float buttonRandomHeight = buttonRandomWidth;
            //float buttonRandomXLoc = 0; //(float)((screenWidth - buttonRandomWidth) * (0.09));
            //float buttonRandomYLoc = (float)((screenHeight) / 4.4) * controlAdjustRandoms;

            float margin = (float)(screenWidth / 32);

            //Button is half the screen size
            float buttonWidth = screenWidth / (2 * sizeSubtract);
            float buttonHeight = (float)(buttonWidth / 1.061);

            //Centers the button in the screen Horizontally and moves it 25%ish down the screen
            float buttonXLoc = (screenWidth - buttonWidth) / 2;
            float buttonYLoc = (float)(((screenHeight - buttonHeight) / 3.16) - (1 - buttonYAdjust) * screenHeight);

            //Button is less than half the screen size (this creates the pressed in effect)
            float buttonWidthSmaller = (float)(screenWidth / (1.2 + sizeSubtract));
            float buttonHeightSmaller = buttonWidthSmaller;

            //Centers the button in the screen
            float buttonXLocSmaller = (screenWidth - buttonWidthSmaller) / 2;
            float buttonYLocSmaller = (float)((screenHeight - buttonHeightSmaller) / 3.12 - (1 - buttonYAdjust) * screenHeight);

            float buttonWidthScene = screenWidth / (4 + sizeSubtract) - 2 * margin;
            float buttonHeightScene = (float)(buttonWidthScene / 1.061);


            bigStartSize = new CGRect(buttonXLoc, buttonYLoc, buttonWidth, buttonHeight);
            smallStartSize = new CGRect(buttonXLocSmaller, buttonYLocSmaller, buttonWidthSmaller, buttonHeightSmaller);
            sceneStartSize = new CGRect(screenWidth - buttonWidthScene - margin, buttonYLoc * 1.7, buttonWidthScene, buttonHeightScene);

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

        public void LoadProgressBar(float screenWidth, float screenHeight, float buttonYAdjust, float sizeSubtract, float controlAdjustProgress)
        {
            float margin = (float)(screenWidth / 32);

            //Set progress bar size based on screen width (it will fit exactly around the button)
            C_lineWidth = (int)(screenWidth / 33);
            float progressWidth = (float)((screenWidth / (2.219 * sizeSubtract)) + 3 * C_lineWidth);
            float progressHeight = progressWidth;

            //Centers the progress bar on the screen
            float progressXLoc = (screenWidth - progressWidth) / 2;
            float progressYLoc = (float)(((screenHeight - progressHeight) / 3.05 - 0.55 * C_lineWidth - (1 - buttonYAdjust) * screenHeight)) * controlAdjustProgress;

            //Declare rectangle object (Instantiating the CGRect class)
            C_progressSize = new CGRect(progressXLoc, progressYLoc, progressWidth, progressHeight);

            C_lineWidthSmall = (int)(screenWidth / 66);
            float progressWidthSmall = (float)(screenWidth / (4+sizeSubtract) - 2 * margin + 2 * C_lineWidthSmall);



            float progressHeightSmall = (float)progressWidthSmall;
            float progressSmallXLoc = (float) screenWidth - progressWidthSmall - margin + C_lineWidthSmall;
            float progressSmallYLoc = (float)(progressYLoc * 1.74);
            C_progressSceneSize = new CGRect(progressSmallXLoc, progressSmallYLoc, progressWidthSmall, progressHeightSmall);


            //Declare color object (Instantiating the UIColor class)
            barColor = UIColor.FromRGB(0, 0, 0); //black

            //Declare progress bar object (Instantiating my CircularProgressBar class)
            myCircularProgressBar = new CircularProgressBar(C_progressSize, C_lineWidth, 0.0f, barColor);
        }

        public void LoadHorizontalProgressBar(float screenWidth, float screenHeight, float buttonYAdjust, float sizeSubtract, float controlHorizontalAdjustProgress)
        {
            H_lineWidth = (int)(screenWidth / 33);
            float progressWidth = (float)((screenWidth / (2.219 * sizeSubtract)) + 3 * H_lineWidth);
            float progressHeight = H_lineWidth;

            float progressXLoc = (screenWidth - progressWidth) / 2;
            float progressYLoc = (float)(((screenHeight - progressHeight) / 3.5 * controlHorizontalAdjustProgress));

            H_progressSize = new CGRect(progressXLoc, progressYLoc, progressWidth, progressHeight);

            barColor = UIColor.FromRGB(0, 0, 0); //black

            myHorizontalProgressBar = new HorizontalProgressBar(H_progressSize, H_lineWidth, 0.5f, barColor);

            myHorizontalProgressBar.Hidden = true;
        }

        public void LoadSceneButtons(float screenWidth, float screenHeight)
        {
            float margin = (float)(screenWidth / 32);

            float buttonSceneWidth = (float)(screenWidth / 8) - (margin * 11/8);
            float buttonSceneHeight = buttonSceneWidth;
            float buttonSceneXLoc = (float)0;
            float buttonSceneYLoc = (float)((screenHeight - buttonSceneHeight) / 3.5);

            

            for (int ii = 0; ii < 8; ii++)
            {
                buttonArray[ii] = UIButton.FromType(UIButtonType.Custom);
                buttonArray[ii].SetImage(UIImage.FromFile("graphicP1NOff.png"), UIControlState.Normal);
                buttonArray[ii].SetImage(UIImage.FromFile("graphicP1NOff.png"), UIControlState.Highlighted);
                buttonArray[ii].SetImage(UIImage.FromFile("graphicP1NOff.png"), UIControlState.Disabled);
                buttonArray[ii].Frame = new CGRect(margin + margin * (ii+1) + buttonSceneWidth * ii, buttonSceneYLoc, buttonSceneWidth, buttonSceneHeight);
                buttonArray[ii].TouchDown += HandleSceneTouchDown;
            }

            widthScenes = buttonSceneWidth;
            marginScenes = margin;
            locationScenes = buttonSceneYLoc;
            frameRunningSymbol = new CGRect(marginScenes + marginScenes, locationScenes - widthScenes, widthScenes, widthScenes);
            runningSymbol_lineWidth = (int)(screenWidth / 120);
            myRunningSymbol = new RunningSymbol(frameRunningSymbol, runningSymbol_lineWidth);


            rangeScenesSlider = new RangeSliderControl();
            rangeScenesSlider.Frame = new CGRect(0, locationScenes+1.5*widthScenes, margin*11 + 8 * widthScenes, widthScenes);
            rangeScenesSlider.TintColor = UIColor.Black;
            rangeScenesSlider.MaximumValue = 8;
            rangeScenesSlider.MinimumValue = 0;
            rangeScenesSlider.TextColor = UIColor.Black;
            rangeScenesSlider.StepValue = 1;

            rangeScenesSlider.LowerValue = 0;
            rangeScenesSlider.UpperValue = 8;
            
            rangeScenesSlider.DragCompleted += (object sender, EventArgs e) =>
            {
                var myObj = (RangeSliderControl)sender;

                if((int)myObj.LowerValue == (int)myObj.UpperValue)
                {
                    if ((int)myObj.UpperValue == (int)myObj.MaximumValue)
                    {
                        myObj.LowerValue -= 1;
                    }
                    else
                    {
                        myObj.UpperValue += 1;
                    }
                }
                myMidiModulation.MinScene = (int)myObj.LowerValue;
                myMidiModulation.MaxScene = (int)myObj.UpperValue-1;
                /*
                var myObj = (RangeSliderControl)sender;
                if (!myMidiModulation.IsSceneMode)
                {
                    myMidiModulation.Maximum = (int)myObj.UpperValue;
                    myMidiModulation.Minimum = (int)myObj.LowerValue;
                }
                else
                {
                    for (int ii = 0; ii < 8; ii++)
                    {
                        if (sceneArray[ii].IsSelected)
                        {
                            sceneArray[ii].Maximum = (int)myObj.UpperValue;
                            sceneArray[ii].Minimum = (int)myObj.LowerValue;
                        }
                    }
                }
                */
            };
        }

        public void LoadRateLabel(float screenWidth, float screenHeight, float textAdjustRate, float sliderHeight)
        {
            float labelRateWidth = (float)(screenWidth / 1.8);
            float labelRateHeight = (float)(screenHeight / 8);
            float labelRateXLoc = (float)((screenWidth - labelRateWidth) / 2);
            float labelRateYLoc = (float)(((screenHeight - sliderHeight) / 1.81) * textAdjustRate);
            labelRate = new UILabel();
            labelRate.Frame = new CGRect(labelRateXLoc, labelRateYLoc, labelRateWidth, labelRateHeight);
            labelRate.Text = "Press Button Above";
            labelRate.TextAlignment = UITextAlignment.Center;
            labelRate.TextColor = UIColor.Black;
            labelRate.Font = UIFont.SystemFontOfSize(100);
            labelRate.AdjustsFontSizeToFitWidth = true;
        }

        public void LoadSettingsButton(float screenWidth, float screenHeight, float sizeSubtract, float controlAdjustSettings)
        {

            float buttonSettingsWidth = screenWidth / (12 * sizeSubtract);
            float buttonSettingsHeight = buttonSettingsWidth;
            float buttonSettingsXLoc = (float)((screenWidth - buttonSettingsWidth) * .98);
            float buttonSettingsYLoc = (float)((screenHeight - buttonSettingsHeight) / 1.85) * controlAdjustSettings;

            buttonSettings = UIButton.FromType(UIButtonType.Custom);
            buttonSettings.SetImage(UIImage.FromFile("graphicSettingsButtonOff.png"), UIControlState.Normal);
            buttonSettings.SetImage(UIImage.FromFile("graphicSettingsButtonOff.png"), UIControlState.Highlighted);
            buttonSettings.SetImage(UIImage.FromFile("graphicSettingsButtonDisabled.png"), UIControlState.Disabled);
            buttonSettings.Frame = new CGRect(screenWidth - buttonSettingsXLoc - buttonSettingsWidth, buttonSettingsYLoc, buttonSettingsWidth, buttonSettingsHeight);

            //buttonSettings.Hidden = true;
            buttonSettings.Enabled = false;
            buttonSettings.TouchDown += HandleSettingsTouchDown;

            buttonAutoRate = UIButton.FromType(UIButtonType.Custom);
            buttonAutoRate.SetImage(UIImage.FromFile("graphicAutoRateButtonOff.png"), UIControlState.Normal);
            buttonAutoRate.SetImage(UIImage.FromFile("graphicAutoRateButtonOff.png"), UIControlState.Highlighted);
            buttonAutoRate.SetImage(UIImage.FromFile("graphicAutoRateButtonDisabled.png"), UIControlState.Disabled);
            buttonAutoRate.Frame = new CGRect(buttonSettingsXLoc, buttonSettingsYLoc, buttonSettingsWidth, buttonSettingsHeight);
            buttonAutoRate.TouchDown += HandleAutoRateTouchDown;

        }

        public void LoadRateSlider(float screenWidth, float screenHeight, float controlAdjustRate, float sliderHeight)
        {
            float sliderWidth = (float)(screenWidth / 1.12);
            float sliderXLoc = (float)((screenWidth - sliderWidth) / 2);
            float sliderYLoc = (float)((screenHeight - sliderHeight) / 1.63) * controlAdjustRate;
            sliderRate = new UISlider();
            sliderRate.Frame = new CGRect(sliderXLoc, sliderYLoc, sliderWidth, sliderHeight);
            sliderRate.MinValue = 0;
            sliderRate.MaxValue = 127;
            sliderRate.Value = 63;
            sliderRate.TintColor = UIColor.Black;
            //sliderRate.TintColor = UIColor.FromRGB(127, 255, 0);
            sliderRate.ValueChanged += HandleRateSliderChange;
            if (highRes)
            {
                sliderRate.SetThumbImage(UIImage.FromFile("graphicRateThumb@2x.png"), UIControlState.Normal);
                sliderRate.MinValueImage = UIImage.FromFile("graphicRateMin@2x.png");
                sliderRate.MaxValueImage = UIImage.FromFile("graphicRateMax@2x.png");
            }
            else
            {
                sliderRate.SetThumbImage(UIImage.FromFile("graphicRateThumb.png"), UIControlState.Normal);
                sliderRate.MinValueImage = UIImage.FromFile("graphicRateMin.png");
                sliderRate.MaxValueImage = UIImage.FromFile("graphicRateMax.png");
            }
        }

        public void LoadPatternLabel(float screenWidth, float screenHeight, float textAdjustPattern, float segHeight)
        {
            float labelPatternWidth = (float)(screenWidth / 1.8);
            float labelPatternHeight = (float)(screenHeight / 8);
            float labelPatternXLoc = (float)((screenWidth - labelPatternWidth) / 2);
            float labelPatternYLoc = (float)(((screenHeight - segHeight) / 1.66) * textAdjustPattern);
            labelPattern = new UILabel();
            labelPattern.Text = "Pattern 1: Up & Down";
            labelPattern.Frame = new CGRect(labelPatternXLoc, labelPatternYLoc, labelPatternWidth, labelPatternHeight);
            labelPattern.TextAlignment = UITextAlignment.Center;
            labelPattern.TextColor = UIColor.Black;
            labelPattern.Font = UIFont.SystemFontOfSize(100);
            labelPattern.AdjustsFontSizeToFitWidth = true;
        }

        public void LoadCCButton(float screenWidth, float screenHeight, float sizeSubtract, float controlAdjustSettings)
        {
            float buttonCCWidth = screenWidth / (12 * sizeSubtract);
            float buttonCCHeight = buttonCCWidth;
            float buttonCCXLoc = (float)((screenWidth - buttonCCWidth) * (0.98));
            //float buttonCCYLoc = (float)((screenHeight - buttonCCHeight) / 1.67) * controlAdjustSettings;
            float buttonCCYLoc = (float)((screenHeight - buttonCCHeight) / 1.475) * controlAdjustSettings;
            //float buttonRandomYLoc = (float)((screenHeight - buttonRandomHeight) / 1.415) * controlAdjustRandoms;
            buttonCC = UIButton.FromType(UIButtonType.Custom);
            buttonCC.SetImage(UIImage.FromFile("graphicCCButtonOff.png"), UIControlState.Normal);
            buttonCC.SetImage(UIImage.FromFile("graphicCCButtonOff.png"), UIControlState.Highlighted);
            buttonCC.SetImage(UIImage.FromFile("graphicCCButtonOff.png"), UIControlState.Disabled);
            buttonCC.Frame = new CGRect((float)(screenWidth - buttonCCXLoc + buttonCCWidth / 5), buttonCCYLoc, buttonCCWidth, buttonCCHeight);
            buttonCC.TouchDown += HandleCCTouchDown;

            buttonBPM = UIButton.FromType(UIButtonType.Custom);
            buttonBPM.SetImage(UIImage.FromFile("graphicBPMButtonOff.png"), UIControlState.Normal);
            buttonBPM.SetImage(UIImage.FromFile("graphicBPMButtonOff.png"), UIControlState.Highlighted);
            buttonBPM.SetImage(UIImage.FromFile("graphicBPMButtonDisabled.png"), UIControlState.Disabled);
            buttonBPM.Enabled = false;
            buttonBPM.Frame = new CGRect((float)(screenWidth - buttonCCXLoc - buttonCCWidth), buttonCCYLoc, buttonCCWidth, buttonCCHeight);
            buttonBPM.TouchDown += HandleBPMTouchDown;

            /*
            buttonReverse = UIButton.FromType(UIButtonType.Custom);
            buttonReverse.SetImage(UIImage.FromFile("graphicReverseButtonOff.png"), UIControlState.Normal);
            buttonReverse.SetImage(UIImage.FromFile("graphicReverseButtonOff.png"), UIControlState.Highlighted);
            buttonReverse.SetImage(UIImage.FromFile("graphicReverseButtonDisabled.png"), UIControlState.Disabled);
            buttonReverse.Frame = new CGRect(buttonCCXLoc - (buttonCCWidth + buttonCCWidth / 5), buttonCCYLoc, buttonCCWidth, buttonCCHeight);
            buttonReverse.Enabled = false;
            buttonReverse.TouchDown += HandleReverseTouchDown;
            //buttonReverse.Hidden = true;
            */

            buttonAutoPattern = UIButton.FromType(UIButtonType.Custom);
            buttonAutoPattern.SetImage(UIImage.FromFile("graphicAutoPatternButtonOff.png"), UIControlState.Normal);
            buttonAutoPattern.SetImage(UIImage.FromFile("graphicAutoPatternButtonOff.png"), UIControlState.Highlighted);
            buttonAutoPattern.SetImage(UIImage.FromFile("graphicAutoPatternButtonDisabled.png"), UIControlState.Disabled);
            buttonAutoPattern.Frame = new CGRect(buttonCCXLoc, buttonCCYLoc, buttonCCWidth, buttonCCHeight);
            //buttonAutoPattern.Hidden = true;
            //buttonAutoPattern.Enabled = false;
            buttonAutoPattern.TouchDown += HandleAutoPatternTouchDown;
        }

        public void LoadSegmented(float screenWidth, float screenHeight, float controlAdjustPattern, float segHeight)
        {
            float segWidth = (float)(screenWidth / 1.07);
            float segXLoc = (float)((screenWidth - segWidth) / 2);
            float segYLoc = (float)((screenHeight - segHeight) / 1.35) * controlAdjustPattern;
            segmentedPattern = new UISegmentedControl("1", "2", "3", "4", "5", "6", "7", "8");
            //segmentedPattern = new UISegmentedControl("1");
            //segmentedPattern = new UISegmentedControl(UIImage.FromFile("graphicP1NOff.png").Scale(new CGSize(segWidth,segHeight)));
            //UIImageView.AppearanceWhenContainedIn().;//.segmentedPattern
            //segmentedPattern.SizeToFit();
            //segmentedPattern.SetImage(UIImage.FromFile("graphicP1NOff.png").Scale(new nfloat(0.5f),0);
            segmentedPattern.Frame = new CGRect(segXLoc, segYLoc, segWidth, segHeight);
            /*
            foreach (UIView image in segmentedPattern.Subviews)
            {
                image.SizeToFit();
            }
            */
            //segmentedPattern.TintColor = UIColor.Clear;
            segmentedPattern.TintColor = UIColor.Black;
            segmentedPattern.SelectedSegment = 0;
            segmentedPattern.ValueChanged += HandlePatternSegmentChange;
        }

        public void LoadCCIncButtons(float screenWidth, float screenHeight, float sizeSubtract, float controlAdjustCCInc, float sizeIncrease)
        {
            float buttonIncWidth = (float)screenWidth / (7 * sizeSubtract);
            float buttonIncHeight = (float)(buttonIncWidth * 0.8 * sizeIncrease);
            float buttonIncXLoc = (float)((screenWidth - buttonIncWidth) / 1.4);
            float buttonIncYLoc = (float)((screenHeight - buttonIncHeight) / 1.35) * controlAdjustCCInc;
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
            buttonPlus10.Frame = new CGRect(buttonIncXLoc * (1.3), buttonIncYLoc, buttonIncWidth, buttonIncHeight);
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
            buttonMinus10.Frame = new CGRect(screenWidth - buttonIncXLoc * (1.3) - buttonIncWidth, buttonIncYLoc, buttonIncWidth, buttonIncHeight);
            buttonMinus10.Hidden = true;

            buttonPlus1.TouchDown += HandlePlus1TouchDown;
            buttonPlus10.TouchDown += HandlePlus10TouchDown;
            buttonMinus1.TouchDown += HandleMinus1TouchDown;
            buttonMinus10.TouchDown += HandleMinus10TouchDown;
        }

        public void LoadTapButton(float screenWidth, float screenHeight, float sizeSubtract, float controlAdjustTap)
        {
            float buttonTapWidth = (float)(screenWidth / (9 * sizeSubtract));
            float buttonTapHeight = buttonTapWidth;// * 3 / 5;
            float buttonTapXLoc = (float)((screenWidth - buttonTapWidth) * 0.5);
            float buttonTapYLoc = (float)(((screenHeight - buttonTapHeight) / 1.35) * controlAdjustTap);
            buttonTap = UIButton.FromType(UIButtonType.Custom);
            buttonTap.Hidden = true;
            buttonTap.SetImage(UIImage.FromFile("graphicTapButtonOff.png"), UIControlState.Normal);
            buttonTap.SetImage(UIImage.FromFile("graphicTapButtonOn.png"), UIControlState.Highlighted);
            buttonTap.SetImage(UIImage.FromFile("graphicTapButtonDisabled.png"), UIControlState.Disabled);
            buttonTap.Frame = new CGRect(buttonTapXLoc, buttonTapYLoc, buttonTapWidth, buttonTapHeight);
            buttonTap.TouchDown += HandleTapTouchDown;
        }

        public void LoadTriggerButton(float screenWidth, float screenHeight, float sizeSubtract, float controlAdjustTrigger)
        {
            float buttonTriggerWidth = (float)(screenWidth / (5 * sizeSubtract));
            float buttonTriggerHeight = buttonTriggerWidth;
            float buttonTriggerXLoc = 0;// (float)((screenWidth - buttonTriggerWidth) * (0.95));
            float buttonTriggerYLoc = (float)((screenHeight - buttonTriggerHeight) / 1.175) * controlAdjustTrigger;
            buttonTrigger = UIButton.FromType(UIButtonType.Custom);
            buttonTrigger.SetImage(UIImage.FromFile("graphicTriggerButtonOff.png"), UIControlState.Normal);
            buttonTrigger.SetImage(UIImage.FromFile("graphicTriggerButtonOn.png"), UIControlState.Highlighted);
            buttonTrigger.SetImage(UIImage.FromFile("graphicTriggerButtonOff.png"), UIControlState.Disabled);
            buttonTrigger.Frame = new CGRect(buttonTriggerXLoc, buttonTriggerYLoc, buttonTriggerWidth, buttonTriggerHeight);
            buttonTrigger.TouchDown += HandleTriggerTouchDown;

            buttonAuto = UIButton.FromType(UIButtonType.Custom);
            buttonAuto.SetImage(UIImage.FromFile("graphicAutoButtonOff.png"), UIControlState.Normal);
            buttonAuto.SetImage(UIImage.FromFile("graphicAutoButtonOn.png"), UIControlState.Highlighted);
            buttonAuto.SetImage(UIImage.FromFile("graphicAutoButtonOff.png"), UIControlState.Disabled);
            buttonAuto.Frame = new CGRect((float)(screenWidth - buttonTriggerWidth), buttonTriggerYLoc, buttonTriggerWidth, buttonTriggerHeight);
            buttonAuto.TouchDown += HandleAutoTouchDown;
        }

        public void LoadModeLabel(float screenWidth, float screenHeight, float textAdjustMode)
        {
            float labelModeWidth = (float)(screenWidth / 2);
            float labelModeHeight = (float)(screenHeight / 8);
            float labelModeXLoc = (float)((screenWidth - labelModeWidth) / 2);
            float labelModeYLoc = (float)(((screenHeight - labelModeHeight) / 1.24) * textAdjustMode);
            labelMode = new UILabel();
            labelMode.Frame = new CGRect(labelModeXLoc, labelModeYLoc, labelModeWidth, labelModeHeight);
            labelMode.Text = "Hello World";
            labelMode.TextAlignment = UITextAlignment.Center;
            labelMode.TextColor = UIColor.Black;
            labelMode.Font = UIFont.SystemFontOfSize(100);
            labelMode.AdjustsFontSizeToFitWidth = true;
        }

        public void LoadDetailsLabel(float screenWidth, float screenHeight, float textAdjustDetails)
        {
            float labelDetailsWidth = (float)(screenWidth / 2);
            float labelDetailsHeight = (float)(screenHeight / 8);
            float labelDetailsXLoc = (float)((screenWidth - labelDetailsWidth) / 2);
            float labelDetailsYLoc = (float)(((screenHeight - labelDetailsHeight) / 1.185) * textAdjustDetails);
            labelDetails = new UILabel();
            labelDetails.Frame = new CGRect(labelDetailsXLoc, labelDetailsYLoc, labelDetailsWidth, labelDetailsHeight);
            labelDetails.Text = "Current Mode Information";
            labelDetails.TextAlignment = UITextAlignment.Center;
            labelDetails.TextColor = UIColor.Black;
            labelDetails.Font = UIFont.SystemFontOfSize(100);
            labelDetails.AdjustsFontSizeToFitWidth = true;
        }

        public void LoadTimeandMidiButtons(float screenWidth, float screenHeight, float sizeSubtract, float controlAdjustMode)
        {
            float buttonTimeWidth = (float)(screenWidth / (3 * sizeSubtract));
            float buttonTimeHeight = (float)(buttonTimeWidth / 1.68);
            float buttonTimeXLoc = (float)((screenWidth / 2) - buttonTimeWidth / 2);
            float buttonTimeYLoc = (float)(screenHeight - buttonTimeHeight) * controlAdjustMode;
            buttonTime = UIButton.FromType(UIButtonType.Custom);
            buttonTime.SetImage(UIImage.FromFile("graphicTimeButtonOff.png"), UIControlState.Normal);
            buttonTime.SetImage(UIImage.FromFile("graphicTimeButtonOn.png"), UIControlState.Highlighted);
            buttonTime.SetImage(UIImage.FromFile("graphicTimeButtonOff.png"), UIControlState.Disabled);
            buttonTime.Frame = new CGRect(buttonTimeXLoc, buttonTimeYLoc, buttonTimeWidth, buttonTimeHeight);

            buttonMidi = UIButton.FromType(UIButtonType.Custom);
            buttonMidi.SetImage(UIImage.FromFile("graphicMidiButtonOff.png"), UIControlState.Normal);
            buttonMidi.SetImage(UIImage.FromFile("graphicMidiButtonOn.png"), UIControlState.Highlighted);
            buttonMidi.SetImage(UIImage.FromFile("graphicMidiButtonOff.png"), UIControlState.Disabled);
            buttonMidi.Frame = new CGRect((float)(buttonTimeXLoc + (buttonTimeWidth)), buttonTimeYLoc, buttonTimeWidth, buttonTimeHeight);

            //float buttonClockWidth = screenWidth / (12 + sizeSubtract);
            //float buttonClockHeight = buttonClockWidth;

            //float buttonClockYLoc = (float)((screenHeight - buttonClockHeight) / 1.02);

            float buttonClockXLoc = buttonTimeXLoc - buttonTimeWidth;// (float)((screenWidth-buttonTimeWidth*3)*0.5);// (float)((screenWidth - buttonClockWidth) * (0.05));
            buttonClock = UIButton.FromType(UIButtonType.Custom);
            buttonClock.SetImage(UIImage.FromFile("graphicClockButtonOff.png"), UIControlState.Normal);
            buttonClock.SetImage(UIImage.FromFile("graphicClockButtonOn.png"), UIControlState.Highlighted);
            buttonClock.SetImage(UIImage.FromFile("graphicClockButtonOff.png"), UIControlState.Disabled);
            buttonClock.Frame = new CGRect(buttonClockXLoc, buttonTimeYLoc, buttonTimeWidth, buttonTimeHeight);
            buttonClock.TouchDown += HandleClockTouchDown;

            //Button Event Handler Delegates
            buttonTime.TouchDown += HandleTimeTouchDown;
            buttonMidi.TouchDown += HandleMidiTouchDown;
        }

        public void LoadBPMTextField(float screenWidth, float screenHeight)
        {
            float BPMWidth = (float)(screenWidth / 3);
            float BPMHeight = (float)(screenHeight / 10);
            float BPMXLoc = (float)((screenWidth - BPMWidth) / 2);
            float BPMYLoc = (float)((screenHeight - BPMHeight) / 10);
            textFieldBPM = new UITextField();
            textFieldBPM.Frame = new CGRect(BPMXLoc, BPMYLoc, BPMWidth, BPMHeight);
            textFieldBPM.Layer.BorderColor = UIColor.White.CGColor;
            textFieldBPM.TextColor = UIColor.Black;
            textFieldBPM.Layer.BorderWidth = 1f;
            textFieldBPM.TextAlignment = UITextAlignment.Center;
            textFieldBPM.KeyboardType = UIKeyboardType.NumbersAndPunctuation;
            textFieldBPM.ReturnKeyType = UIReturnKeyType.Done;
            //textFieldBPM.Placeholder = "HELLO WORLD";
            textFieldBPM.AttributedPlaceholder = new NSAttributedString("100", null, UIColor.White);
            textFieldBPM.MinimumFontSize = 17f;
            textFieldBPM.AdjustsFontSizeToFitWidth = true;

            textFieldBPM.ShouldChangeCharacters = (textField, range, replacement) =>
            {
                var newContent = new NSString(textField.Text).Replace(range, new NSString(replacement)).ToString();
                int number;
                return newContent.Length <= 3 && (replacement.Length == 0 || int.TryParse(replacement, out number));
            };
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







        /*
        public void KeyboardWillShow(NSNotification notification)
        {
            var doneButton = new UIButton(UIButtonType.Custom);
            doneButton.Frame = new RectangleF(0, 163, 106, 53);
            doneButton.SetTitle("DONE", UIControlState.Normal);
            doneButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
            doneButton.SetTitleColor(UIColor.White, UIControlState.Highlighted);

            doneButton.TouchUpInside += (sender, e) =>
            {
                // Make the Done button do its thing!  The textfield shouldn't be the first responder
                textFieldBPM.ResignFirstResponder();
            };

            // This is the 'magic' that could change with future version of iOS
            var keyboard =textFieldBPM.WeakInputDelegate as UIView;
            if (keyboard != null)
            {
                keyboard.AddSubview(doneButton);
            }
        }
        */


        public void AddSubviewsToMainview()
        {
            //Add Subviews to main view
            View.AddSubview(myCircularProgressBar);

            View.AddSubview(buttonOnOff);
            View.AddSubview(buttonTime);
            View.AddSubview(segmentedPattern);
            View.AddSubview(labelPattern);
            View.AddSubview(buttonMidi);
            View.AddSubview(labelMode);
            View.AddSubview(labelDetails);
            View.AddSubview(sliderRate);
            View.AddSubview(labelRate);
            View.AddSubview(buttonAutoRate);
            View.AddSubview(labelRange);
            View.AddSubview(rangeSlider);
            View.AddSubview(sliderHidden);
            View.AddSubview(sliderCC);
            View.AddSubview(buttonReverse);
            View.AddSubview(buttonScenes);
            //View.AddSubview(myRunningSymbol);
            //View.AddSubview(buttonArrange);
            View.AddSubview(buttonCC);
            View.AddSubview(buttonSettings);
            //View.AddSubview(buttonInfo);
            View.AddSubview(buttonBPM);
            View.AddSubview(buttonAutoPattern);
            View.AddSubview(buttonTap);
            View.AddSubview(buttonTrigger);
            View.AddSubview(buttonClock);
            View.AddSubview(buttonRandom);
            View.AddSubview(buttonAuto);
            View.AddSubview(buttonAR);
            View.AddSubview(buttonLocation);
            View.AddSubview(buttonPlus1);
            View.AddSubview(buttonPlus10);
            View.AddSubview(buttonMinus1);
            View.AddSubview(buttonMinus10);
            


            //View.AddSubview(myHorizontalProgressBar);
            //View.AddSubview(textFieldBPM);

            /*
            View.AddSubview(buttonScene0);
            View.AddSubview(buttonScene1);
            View.AddSubview(buttonScene2);
            View.AddSubview(buttonScene3);
            View.AddSubview(buttonScene4);
            View.AddSubview(buttonScene5);
            View.AddSubview(buttonScene6);
            View.AddSubview(buttonScene7);
            

            for (int ii = 0; ii < 8; ii++)
            {
                View.AddSubview(buttonArray[ii]);
            }
            */
        }
    }
}
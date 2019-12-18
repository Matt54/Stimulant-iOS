using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Stimulant
{
    public class PowerButton : UIViewController
    {
        public PowerButton(CGRect rect)
        {
            View.Frame = rect;
            C_lineWidth = (int)(rect.Width / 33);

            //View.BackgroundColor = UIColor.Yellow;
            CreateButton(new CGRect(0, 0, rect.Width, rect.Height)); //-2 * C_lineWidth
            CreateCircularProgressBar(new CGRect(0, 0, rect.Width, rect.Height));

            View.AddSubview(myCircularProgressBar);
            View.AddSubview(buttonOnOff);
        }

        private void CreateCircularProgressBar(CGRect rect)
        {
            //float pBarWidth = buttonWidth;
            //CGRect pBarRect = new CGRect( (rect.Width - pBarWidth) / 2 , 0, rect.Width, rect.Height);
            CGRect pBarRect = new CGRect(0, 0, rect.Width, rect.Height);
            myCircularProgressBar = new CircularProgressBar(pBarRect, (int)C_lineWidth, 0f, UIColor.FromRGB(0, 0, 0));
        }

        public void UpdateProgress(float piMult)
        {
            myCircularProgressBar.UpdateGraph(piMult);
            //g = myCircularProgressBar.GetContext();
            //myCircularProgressBar.UpdateGraph(piMult , g);
        }

        private void CreateButton(CGRect rect)
        {
            buttonOnOff = UIButton.FromType(UIButtonType.Custom);
            buttonOnOff.SetImage(UIImage.FromFile("graphicPowerButtonOff.png"), UIControlState.Normal);
            //buttonOnOff.SetImage(UIImage.FromFile("graphicPowerButtonOn.png"), UIControlState.Highlighted);
            //buttonOnOff.SetImage(UIImage.FromFile("graphicPowerButtonOff.png"), UIControlState.Disabled);

            buttonHeight = (float)rect.Height;
            buttonWidth = (float)(buttonHeight * 1);

            //float buttonWidth = (float)rect.Width / 2;
            //float buttonHeight = (float)(buttonWidth / 1.061);
            buttonOnOff.Frame = new CGRect((rect.Width - buttonWidth) / 2, rect.Top, buttonWidth, buttonHeight);
            //buttonOnOff.Frame = new CGRect((rect.Width - buttonWidth) / 2, rect.Top, rect.Width, rect.Width);
            buttonFrame = buttonOnOff.Frame;

            buttonOnOff.TouchDown += (object sender, EventArgs e) =>
            {
                HandleTouchDown();
            };

            buttonOnOff.TouchUpInside += (object sender, EventArgs e) =>
            {
                HandleTouchUp();
                StateChange?.Invoke(this, e);
            };
        }

        public void UpdateFrame(CGRect rect)
        {
            View.Frame = rect;
            C_lineWidth = (int)(rect.Width / 33);
            UpdateButtonFrame(new CGRect(0, 0, rect.Width, rect.Height));
            UpdateCircularProgressBarFrame(new CGRect(0, 0, rect.Width, rect.Height));
        }

        private void UpdateButtonFrame(CGRect rect)
        {
            buttonHeight = (float)rect.Height;
            buttonWidth = (float)(buttonHeight * 1);
            buttonOnOff.Frame = new CGRect((rect.Width - buttonWidth) / 2, rect.Top, buttonWidth, buttonHeight);
            buttonFrame = buttonOnOff.Frame;
        }

        private void UpdateCircularProgressBarFrame(CGRect rect)
        {
            myCircularProgressBar.Frame = new CGRect(0, 0, rect.Width, rect.Height);
        }


        private void HandleTouchDown()
        {
            myCircularProgressBar.Hidden = true;
            MakeButtonSmaller();
        }

        private void HandleTouchUp()
        {
            myCircularProgressBar.Hidden = false;
            MakeButtonFullSize();
            TogglePower();
        }

        private void MakeButtonFullSize()
        {
            buttonOnOff.Frame = buttonFrame;
        }

        private void MakeButtonSmaller()
        {
            if(isOn) buttonOnOff.SetImage(UIImage.FromFile("graphicPowerButtonOn.png"), UIControlState.Highlighted);
            else buttonOnOff.SetImage(UIImage.FromFile("graphicPowerButtonOff.png"), UIControlState.Highlighted);
            buttonOnOff.Frame = new CGRect(buttonFrame.Left + buttonFrame.Width * 0.05, buttonFrame.Height * 0.05, buttonFrame.Width * 0.9, buttonFrame.Height * 0.9);
        }

        public bool TogglePower()
        {
            isOn = !isOn;
            if (isOn) buttonOnOff.SetImage(UIImage.FromFile("graphicPowerButtonOn.png"), UIControlState.Normal);
            else buttonOnOff.SetImage(UIImage.FromFile("graphicPowerButtonOff.png"), UIControlState.Normal);
            return isOn;
        }

        public bool IsOn()
        {
            return isOn;
        }

        public void SetOn()
        {
            isOn = true;
            buttonOnOff.SetImage(UIImage.FromFile("graphicPowerButtonOn.png"), UIControlState.Normal);
        }

        public void SetOff()
        {
            isOn = false;
            buttonOnOff.SetImage(UIImage.FromFile("graphicPowerButtonOff.png"), UIControlState.Normal);
        }

        public event EventHandler StateChange;

bool isOn;

        CGRect buttonFrame;
        float buttonHeight;
        float buttonWidth;
        float C_lineWidth;

        UIButton buttonOnOff;
        CircularProgressBar myCircularProgressBar;
    }
}

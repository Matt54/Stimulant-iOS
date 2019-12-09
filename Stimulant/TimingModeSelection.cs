using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Stimulant
{
    public class TimingModeSelection : UIViewController
    {
        public TimingModeSelection(CGRect rect)
        {
            View.Frame = rect;
            CreateButtons(new CGRect(0, 0, rect.Width, rect.Height));
            SetButtonImages();
            AddButtonsToView();
        }

        private void CreateButtons(CGRect rect)
        {

            //first try sizing to the width of the rectangle while holding the image aspect ratio
            float buttonWidth = (float)rect.Width / 3;
            float buttonHeight = (float)(buttonWidth / 1.68);
            float padding = 0.0f;

            //if the full width results in buttons that are too tall, we size the height to the rect,
            //size the width according to the aspect ratio, and then pad the sides
            if (rect.Height < buttonHeight)
            {
                buttonHeight = (float) rect.Height;
                buttonWidth = (float) (buttonHeight * 1.68);
                padding = (float) (rect.Width - buttonWidth * 3) / 2;
            }

            buttonInternalClock = UIButton.FromType(UIButtonType.Custom);
            buttonFrequency = UIButton.FromType(UIButtonType.Custom);
            buttonMidi = UIButton.FromType(UIButtonType.Custom);

            buttonInternalClock.Frame = new CGRect(padding, 0, buttonWidth, rect.Height);
            buttonInternalClock.TouchDown += (object sender, EventArgs e) =>
            {
                SetMode(3);
                ButtonPressed?.Invoke(this, e);
            };

            buttonFrequency.Frame = new CGRect(buttonInternalClock.Frame.Right, 0, buttonWidth, rect.Height);
            buttonFrequency.TouchDown += (object sender, EventArgs e) =>
            {
                SetMode(2);
                ButtonPressed?.Invoke(this, e);
            };

            buttonMidi.Frame = new CGRect(buttonFrequency.Frame.Right, 0, buttonWidth, rect.Height);
            buttonMidi.TouchDown += (object sender, EventArgs e) =>
            {
                SetMode(1);
                ButtonPressed?.Invoke(this, e);
            };
        }

        public event EventHandler ButtonPressed;

        private void SetButtonImages()
        {
            if(currentSelection == 2)
            {
                buttonFrequency.SetImage(UIImage.FromFile("graphicTimeButtonOn.png"), UIControlState.Normal);
            }
            else
            {
                buttonFrequency.SetImage(UIImage.FromFile("graphicTimeButtonOff.png"), UIControlState.Normal);
            }
            buttonFrequency.SetImage(UIImage.FromFile("graphicTimeButtonOn.png"), UIControlState.Highlighted);
            buttonFrequency.SetImage(UIImage.FromFile("graphicTimeButtonOff.png"), UIControlState.Disabled);
            

            if (currentSelection == 1)
            {
                buttonMidi.SetImage(UIImage.FromFile("graphicMidiButtonOn.png"), UIControlState.Normal);
            }
            else
            {
                buttonMidi.SetImage(UIImage.FromFile("graphicMidiButtonOff.png"), UIControlState.Normal);
            }
            buttonMidi.SetImage(UIImage.FromFile("graphicMidiButtonOn.png"), UIControlState.Highlighted);
            buttonMidi.SetImage(UIImage.FromFile("graphicMidiButtonOff.png"), UIControlState.Disabled);
            

            if (currentSelection == 3)
            {
                buttonInternalClock.SetImage(UIImage.FromFile("graphicClockButtonOn.png"), UIControlState.Normal);
            }
            else
            {
                buttonInternalClock.SetImage(UIImage.FromFile("graphicClockButtonOff.png"), UIControlState.Normal);
            }
            buttonInternalClock.SetImage(UIImage.FromFile("graphicClockButtonOn.png"), UIControlState.Highlighted);
            buttonInternalClock.SetImage(UIImage.FromFile("graphicClockButtonOff.png"), UIControlState.Disabled);
        }

        private void AddButtonsToView()
        {
            View.AddSubview(buttonInternalClock);
            View.AddSubview(buttonFrequency);
            View.AddSubview(buttonMidi);
        }

        private UIButton buttonInternalClock;
        private UIButton buttonFrequency;
        private UIButton buttonMidi;

        public int GetModeNumber()
        {
            return currentSelection;

        }

        public void SetMode(int selectedMode)
        {
            currentSelection = selectedMode;
            SetButtonImages();
        }

        private int currentSelection = 2;
    }
}

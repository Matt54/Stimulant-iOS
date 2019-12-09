using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Stimulant
{

    public class PatternSelection : UIViewController
    {

        public PatternSelection(CGRect rect)
        {
            View.Frame = rect;
            //View.BackgroundColor = UIColor.Black;
            CreateLabel(new CGRect(0, 0, rect.Width, rect.Height / 2));
            CreateButtons(new CGRect(0, label.Frame.Bottom, rect.Width, rect.Height / 2));

            View.AddSubview(label);
            AddButtonsToView();
        }

        void CreateLabel(CGRect rect)
        {
            label = new UILabel();
            //UpdateLabelText();
            label.Frame = rect;
            //label.BackgroundColor = UIColor.Black;
            //label.TextColor = UIColor.White;
            label.TextAlignment = UITextAlignment.Center;
            //valueLabel.Lines = 0;
            //valueLabel.LineBreakMode = UILineBreakMode.WordWrap;
        }

        public void UpdateLabelText(string patternText)
        {
            label.Text = patternText;
        }

        void CreateButtons(CGRect rect)
        {
            SetFrames(rect);
            SetButtonImages(0);
        }

        /*
        void SetButtonFrames(CGRect rect)
        {
            for (int i = 0; i < buttonArray.Length; i++)
            {
                buttonArray[i] = new UIButton();
                buttonArray[i].Frame = new CGRect(rect.Left + rect.Width / buttonArray.Length * i,
                                                    rect.Top,
                                                    rect.Width / buttonArray.Length,
                                                    rect.Height);
            }
        }
        */


        public void SetFrames(CGRect rect)//, float padding)
        {
            float padding = 5.0f;
            for (int i = 0; i < buttonArray.Length; i++)
            {
                int n = buttonArray.Length;
                float individualWidth = (float)((rect.Width / n) - padding * (n + 1) / n);

                buttonArray[i] = UIButton.FromType(UIButtonType.Custom);
                buttonArray[i].Frame = new CGRect(rect.Left + padding * (i + 1) + individualWidth * i,
                                                    rect.Top + padding,
                                                    individualWidth,
                                                    individualWidth);
                buttonArray[i].TouchDown += HandleButtonPress;
            }
        }

        void SetButtonImages(int selectedPattern)
        {
            for (int i = 0; i < buttonArray.Length; i++)
            {
                
                if (i == selectedPattern)
                {
                    buttonArray[i].SetImage(UIImage.FromFile("graphicP" + Convert.ToString(i + 1) + "NOn.png"), UIControlState.Normal);
                }
                else
                {
                    buttonArray[i].SetImage(UIImage.FromFile("graphicP" + Convert.ToString(i + 1) + "NOff.png"), UIControlState.Normal);
                }
                buttonArray[i].SetImage(UIImage.FromFile("graphicP" + Convert.ToString(i + 1) + "NOn.png"), UIControlState.Highlighted);
                buttonArray[i].SetImage(UIImage.FromFile("graphicP" + Convert.ToString(i + 1) + "NOff.png"), UIControlState.Disabled);
            }
        }

        void AddButtonsToView()
        {
            for (int i = 0; i < buttonArray.Length; i++)
            {
                View.AddSubview(buttonArray[i]);
            }
        }

        public event EventHandler ButtonPressed;
        void HandleButtonPress(object sender, System.EventArgs e)
        {
            for (int i = 0; i < buttonArray.Length; i++)
            {
                if(sender == buttonArray[i])
                {
                    currentSelection = i;
                }
            }
            SetButtonImages(currentSelection);

            var handler = ButtonPressed;
            if (handler != null) handler(this, e);

            //UpdateLabelText();
        }

        public int GetPatternNumber()
        {
            return currentSelection + 1;
        }

        public void SetPattern(int selectedPattern)
        {
            currentSelection = selectedPattern - 1;
            SetButtonImages(currentSelection);
        }

        public void SetVisibility(bool isVisible)
        {

        }

        private UIButton[] buttonArray = new UIButton[8];
        private UILabel label = new UILabel();
        private int currentSelection;

    }
}

using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.RangeSlider;

namespace Stimulant
{
    public class RangeSelection : UIViewController
    {

        public RangeSelection(CGRect rect)
        {
            View.Frame = rect;
            //View.BackgroundColor = UIColor.Black;
            CreateLabel(new CGRect(0, rect.Height * 0.1, rect.Width, rect.Height * 0.6 ));
            CreateRangeSlider(new CGRect(0, rect.Height * 0.6, rect.Width, rect.Height * 0.4));
            View.AddSubview(label);
            View.AddSubview(rangeSlider);
        }

        private void CreateLabel(CGRect rect)
        {
            label = new UILabel();
            UpdateLabelText("Modulation Range");
            label.MinimumFontSize = 20;
            label.Frame = rect;
            //label.BackgroundColor = UIColor.Black;
            //label.TextColor = UIColor.White;
            label.TextAlignment = UITextAlignment.Center;
            //valueLabel.Lines = 0;
            //valueLabel.LineBreakMode = UILineBreakMode.WordWrap;
        }

        public void UpdateLabelText(string text)
        {
            label.Text = text;
        }

        private void CreateRangeSlider(CGRect rect)
        {
            rangeSlider = new RangeSliderControl();
            rangeSlider.ShowTextAboveThumbs = true;
            //rangeSlider.BackgroundColor = UIColor.Black;
            //rangeSlider.VerticalAlignment = UIControlContentVerticalAlignment.Center;
            rangeSlider.TextSize = 20;

            float padding = 5.0f;

            rangeSlider.Frame = new CGRect(padding, rect.Top, rect.Width - padding * 2, rect.Height);
            rangeSlider.TintColor = UIColor.Black;
            rangeSlider.MaximumValue = 127;
            rangeSlider.MinimumValue = 0;
            rangeSlider.TextColor = UIColor.Black;

            rangeSlider.LowerValue = 0;
            rangeSlider.UpperValue = 127;

            rangeSlider.DragCompleted += HandleSliderChange;
        }

        public event EventHandler SliderMoved;
        void HandleSliderChange(object sender, System.EventArgs e)
        {
            var myObj = (RangeSliderControl)sender;
            minimum = (int)myObj.LowerValue;
            maximum = (int)myObj.UpperValue;

            SliderMoved?.Invoke(this, e);
        }

        public void SliderEnabled(bool isEnabled)
        {
            if(isEnabled)
            {
                rangeSlider.Enabled = true;
            }
            else
            {
                rangeSlider.Enabled = false;
            }
        }

        public void SetMinimum(int min)
        {
            minimum = min;
            rangeSlider.LowerValue = min;
        }

        public void SetMaximum(int max)
        {
            maximum = max;
            rangeSlider.UpperValue = max;
        }

        public int GetMinimum()
        {
            return minimum;
        }

        public int GetMaximum()
        {
            return maximum;
        }

        private int minimum;
        private int maximum = 127;
        private UILabel label = new UILabel();
        private RangeSliderControl rangeSlider = new RangeSliderControl();
    }
}

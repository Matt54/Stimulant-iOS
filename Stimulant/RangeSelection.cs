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

            CreateSliders(new CGRect(0, rect.Height * 0.6, rect.Width, rect.Height * 0.4));


            //CreateRangeSlider(new CGRect(0, rect.Height * 0.6, rect.Width, rect.Height * 0.4));
            //CreateHiddenSlider(new CGRect(0, rect.Height * 0.73, rect.Width, rect.Height * 0.27));

            View.AddSubview(label);
            View.AddSubview(rangeSlider);
            View.AddSubview(hiddenSlider);
        }

        private void CreateLabel(CGRect rect)
        {
            label = new UILabel();
            UpdateLabelText("Modulation Range");
            label.MinimumFontSize = 20;
            label.Frame = rect;
            label.TextAlignment = UITextAlignment.Center;
        }

        public void UpdateLabelText(string text)
        {
            label.Text = text;
        }

        private void CreateSliders(CGRect rect)
        {
            rangeSlider = new RangeSliderControl();
            rangeSlider.ShowTextAboveThumbs = true;

            float textSize = 20.0f;

            rangeSlider.TextSize = textSize;
            float someHeight = (float) UIFont.SystemFontOfSize(20).LineHeight;
            
            //The vertical offset between the sliders fluctuates with the text size
            //If we need to dynamically change the text size then we need to adjust this value as well
            float verticalOffset = (float) (textSize * 0.82);

            float padding = 5.0f;

            rangeSlider.Frame = new CGRect(padding, rect.Top, rect.Width - padding * 2, rect.Height);
            rangeSlider.TintColor = UIColor.Black;
            rangeSlider.MaximumValue = 127;
            rangeSlider.MinimumValue = 0;
            rangeSlider.TextColor = UIColor.Black;

            rangeSlider.LowerValue = 0;
            rangeSlider.UpperValue = 127;

            rangeSlider.DragCompleted += HandleSliderChange;

            hiddenSlider = new UISlider();
            hiddenSlider.Frame = new CGRect(padding, rect.Top + verticalOffset, rect.Width - padding * 2, rect.Height - (verticalOffset * 0.5) );
            
            hiddenSlider.MinValue = 0;
            hiddenSlider.MaxValue = 127;
            hiddenSlider.Value = startingLocation;
            hiddenSlider.TintColor = UIColor.Clear;
            LocationSelection(false);
            hiddenSlider.SetMinTrackImage(new UIImage(), UIControlState.Normal);
            hiddenSlider.SetMaxTrackImage(new UIImage(), UIControlState.Normal);
            hiddenSlider.ValueChanged += HandleHiddenSliderChange;

            bool highRes = false;
            if (highRes)
            {
                hiddenSlider.SetThumbImage(UIImage.FromFile("graphicLocationThumb@2x.png"), UIControlState.Normal);
            }
            else
            {
                hiddenSlider.SetThumbImage(UIImage.FromFile("graphicLocationThumb.png"), UIControlState.Normal);
            }


        }

        public event EventHandler SliderMoved;
        void HandleSliderChange(object sender, System.EventArgs e)
        {
            var myObj = (RangeSliderControl)sender;
            minimum = (int)myObj.LowerValue;
            maximum = (int)myObj.UpperValue;

            SliderMoved?.Invoke(this, e);
        }

        public event EventHandler HiddenSliderMoved;
        void HandleHiddenSliderChange(object sender, System.EventArgs e)
        {
            var myObj = (UISlider)sender;
            startingLocation = (int) myObj.Value;
            HiddenSliderMoved?.Invoke(this, e);
        }

        void SliderEnabled(bool isEnabled)
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

        void SliderHidden(bool isHidden)
        {
            if (isHidden)
            {
                hiddenSlider.Hidden = true;
            }
            else
            {
                hiddenSlider.Hidden = false;
            }
        }

        public void LocationSelection(bool isLocationSelection)
        {
            if(isLocationSelection)
            {
                SliderEnabled(false);
                SliderHidden(false);
            }
            else
            {
                SliderEnabled(true);
                SliderHidden(true);
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

        public void SetStartingLocation(int loc)
        {
            startingLocation = loc;
            hiddenSlider.Value = startingLocation;
        }

        public int GetStartingLocation()
        {
            return startingLocation;
        }

        public int GetMinimum()
        {
            return minimum;
        }

        public int GetMaximum()
        {
            return maximum;
        }

        private int startingLocation = 63;
        private int minimum;
        private int maximum = 127;
        private UILabel label;
        private RangeSliderControl rangeSlider;
        private UISlider hiddenSlider;
    }
}

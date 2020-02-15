using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;


namespace Stimulant
{
    public class RateSelection : UIViewController
    {
        public RateSelection(CGRect rect, bool isHighRes)
        {
            View.Frame = rect;
            highRes = isHighRes;

            //View.BackgroundColor = UIColor.Black;

            CreateLabel(new CGRect(0, 0, rect.Width, rect.Height / 2));
            CreateSlider(new CGRect(rect.Width * .05, label.Frame.Bottom, rect.Width * .9 , rect.Height / 2));

            View.AddSubview(label);
            View.AddSubview(slider);
        }

        void CreateLabel(CGRect rect)
        {
            label = new UILabel();
            label.Frame = rect;
            label.TextAlignment = UITextAlignment.Center;
            //UpdateLabel("Current Slider Value = " + Convert.ToString(slider.Value));
        }

        public void UpdateLabel(String text)
        {
            label.Text = text;
        }

        void CreateSlider(CGRect rect)
        {
            slider = new UISlider();
            slider.Frame = rect;
            slider.MinValue = 0;
            slider.MaxValue = 127;
            slider.Value = 50;
            slider.TintColor = UIColor.Black;
            //sliderRate.TintColor = UIColor.FromRGB(127, 255, 0);
            slider.ValueChanged += HandleSliderChange;
            if (highRes)
            {
                slider.SetThumbImage(UIImage.FromFile("graphicRateThumb@2x.png"), UIControlState.Normal);
                slider.MinValueImage = UIImage.FromFile("graphicRateMin@2x.png");
                slider.MaxValueImage = UIImage.FromFile("graphicRateMax@2x.png");
            }
            else
            {
                slider.SetThumbImage(UIImage.FromFile("graphicRateThumb.png"), UIControlState.Normal);
                slider.MinValueImage = UIImage.FromFile("graphicRateMin.png");
                slider.MaxValueImage = UIImage.FromFile("graphicRateMax.png");
            }
        }

        public event EventHandler SliderChange;
        void HandleSliderChange(object sender, System.EventArgs e)
        {
            //TODO if isSliderSnap then snap the value to nearest integer
            if (isSliderSnap) slider.Value = (float)Math.Round(slider.Value, 0);

            SliderChange?.Invoke(this, e);
        }

       public float GetValue()
        {
            return slider.Value;
        }

        public void SetSliderSnap(bool isSnap)
        {
            isSliderSnap = isSnap;
            if (isSliderSnap)
            {
                slider.MaxValue = 17;
                slider.Value = 8;
            }
            else
            {
                slider.MaxValue = 127;
                slider.Value = 64;
            }
        }

        //private int stepSize;
        private bool highRes;

        private bool isSliderSnap;
        public bool IsSliderSnap
        {
            get { return isSliderSnap; }
            set { isSliderSnap = value;}
        }

        private UILabel label = new UILabel();
        private UISlider slider = new UISlider();
    }
}

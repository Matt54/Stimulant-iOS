using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Stimulant
{
    public class InfoDisplay : UIViewController
    {
        public InfoDisplay(CGRect rect)
        {
            View.Frame = rect;
            View.BackgroundColor = UIColor.Black;


            CreateInnerRect(rect);
            CreateTitleLabel(new CGRect(0, 0, rect.Width, rect.Height * 0.5));
            CreateDescLabel(new CGRect(0, titleLabel.Frame.Bottom, rect.Width, rect.Height * 0.5));

            View.AddSubview(innerRect);
            View.AddSubview(titleLabel);
            View.AddSubview(descLabel);
        }

        void CreateInnerRect(CGRect rect)
        {
            borderWidth = 3.0f;
            innerRect = new UIView(new CGRect(borderWidth, borderWidth, rect.Width - 2 * borderWidth, rect.Height - 2 * borderWidth));
            innerRect.BackgroundColor = UIColor.White;
        }

        void CreateTitleLabel(CGRect rect)
        {
            titleLabel = new UILabel();
            titleLabel.Frame = rect;
            titleLabel.TextAlignment = UITextAlignment.Center;
            UpdateTitle("Title Text");
        }

        void CreateDescLabel(CGRect rect)
        {
            descLabel = new UILabel();
            descLabel.Frame = rect;
            descLabel.TextAlignment = UITextAlignment.Center;
            UpdateDesc("Description Text");
        }

        public void UpdateTitle(string text)
        {
            titleLabel.Text = text;
        }

        public void UpdateDesc(string text)
        {
            descLabel.Text = text;
        }

        private float borderWidth;
        private UIView innerRect;
        private UILabel titleLabel;
        private UILabel descLabel;
    }
}

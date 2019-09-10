using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;


namespace Stimulant
{
    [Register("HorizontalProgressBar"), DesignTimeVisible(true)]


    public class HorizontalProgressBar : UIView, System.ComponentModel.IComponent
    {
        public HorizontalProgressBar(IntPtr handle) : base(handle) { }

        public override void AwakeFromNib()
        {
            // Initialize the view here.
        }

        public event EventHandler Disposed;
        public ISite Site
        {
            get;
            set;
        }

        int _radius;
        int _lineWidth;
        nfloat _progressPercent;

        UIColor _barColor;

        public HorizontalProgressBar(CGRect frame, int lineWidth, nfloat progressPercent, UIColor barColor)
        {
            _barColor = barColor;
            _progressPercent = progressPercent;
            _lineWidth = lineWidth;
            this.Frame = new CGRect(frame.X, frame.Y, frame.Width, frame.Height);
            this.BackgroundColor = UIColor.Clear;
        }

        public override void Draw(CoreGraphics.CGRect rect)
        {
            base.Draw(rect);

            using (CGContext g = UIGraphics.GetCurrentContext())
            {
                _radius = (int)((this.Bounds.Width) / 2) - _lineWidth;
                DrawGraph(g, this.Bounds.GetMinX(), this.Bounds.GetMaxX(), this.Bounds.GetMidY(), _progressPercent); // Remember you changed this to min x
            };
        }

        public void DrawGraph(CGContext g, nfloat x0, nfloat x1, nfloat y0, nfloat progressPercent)
        {
            g.SetLineWidth(_lineWidth);

            _progressPercent = progressPercent;

            g.SetStrokeColor(UIColor.FromRGB(155, 155, 155).CGColor);
 
            g.MoveTo(x0, y0);
            g.AddLineToPoint(x0 + (x1-x0), y0);
            g.StrokePath();

            g.SetStrokeColor(_barColor.CGColor);
            g.MoveTo(x0, y0);
            g.AddLineToPoint(x0 + _progressPercent * (x1 - x0), y0);
            g.StrokePath();

  
        }

    }
}

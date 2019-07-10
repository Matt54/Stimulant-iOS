using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;


namespace Stimulant
{
    [Register("CircularProgressBar"), DesignTimeVisible(true)]

    public class CircularProgressBar : UIView, System.ComponentModel.IComponent
    {

        public CircularProgressBar(IntPtr handle) : base(handle) { }

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
        nfloat _piMult;
        UIColor _barColor;

        public CircularProgressBar(CGRect frame, int lineWidth, nfloat piMult, UIColor barColor)
        {
            _barColor = barColor;
            _piMult = piMult;
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
                DrawGraph(g, this.Bounds.GetMidX(), this.Bounds.GetMidY(), _piMult);
            };
        }


        public void DrawGraph(CGContext g, nfloat x0, nfloat y0, nfloat piMult)
        {
            g.SetLineWidth(_lineWidth);
            _piMult = piMult;

            // Draw circle
            CGPath path = new CGPath();
            UIColor.FromRGB(15, 15, 15).SetStroke();
            path.AddArc(x0, y0, _radius, 0, 2.0f * (float)Math.PI, true);
            g.AddPath(path);
            g.DrawPath(CGPathDrawingMode.Stroke);
            CGPath path2 = new CGPath();
            _barColor.SetStroke();
            path2.AddArc(x0, y0, _radius, 0.5f * (float)Math.PI, 0.5f * (float)Math.PI + _piMult * (float)Math.PI, false);
            //path2.AddArc(x0, y0, _radius, -0.5f * (float)Math.PI, 0.5f * (float)Math.PI + _piMult * (float)Math.PI * 0.99, true);
            g.AddPath(path2);
            g.DrawPath(CGPathDrawingMode.Stroke);
        }

    }
}

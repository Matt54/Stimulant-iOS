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
        nfloat _x0;
        nfloat _y0;
        CGContext _g;
        CGRect _frame;


        public CircularProgressBar(CGRect frame, int lineWidth, nfloat piMult, UIColor barColor)
        {
            _frame = frame;
            _barColor = barColor;
            _piMult = piMult;
            _lineWidth = lineWidth;
            this.Frame = new CGRect(frame.X, frame.Y, frame.Width, frame.Height);
            this.BackgroundColor = UIColor.Clear;
        }

        public void UpdateFrame(CGRect frame, int lineWidth)
        {
            _frame = frame;
            _lineWidth = lineWidth;
            this.Frame = new CGRect(frame.X, frame.Y, frame.Width, frame.Height);
        }

        public override void Draw(CoreGraphics.CGRect rect)
        {
            base.Draw(rect);

            using (CGContext g = UIGraphics.GetCurrentContext())
            {
                _g = g;
                _radius = (int)((this.Bounds.Width) / 2) - _lineWidth;
                DrawGraph(_g, this.Bounds.GetMidX(), this.Bounds.GetMidY(), _piMult);
            }
        }


        public void DrawGraph(CGContext g, nfloat x0, nfloat y0, nfloat piMult)
        {
            _g = g;
            _x0 = x0;
            _y0 = y0;

            _g.SetLineWidth(_lineWidth);
            _piMult = piMult;

            // Draw circle
            CGPath path = new CGPath();
            UIColor.FromRGB(155, 155, 155).SetStroke();
            path.AddArc(_x0, _y0, _radius, 0.78f * (float)Math.PI, (2.22f) * (float)Math.PI, false);
            _g.AddPath(path);
            _g.DrawPath(CGPathDrawingMode.Stroke);
            CGPath path2 = new CGPath();
            _barColor.SetStroke();
            path2.AddArc(_x0, _y0, _radius, 0.78f * (float)Math.PI, (0.78f * (float)Math.PI + (float)(0.72*_piMult) * (float)Math.PI), false);
            //path2.AddArc(x0, y0, _radius, -0.5f * (float)Math.PI, 0.5f * (float)Math.PI + _piMult * (float)Math.PI * 0.99, true);
            _g.AddPath(path2);
            _g.DrawPath(CGPathDrawingMode.Stroke);
        }

        public void UpdateGraph(nfloat piMult)
        {
            _piMult = piMult;

            // Draw circle
            CGPath path = new CGPath();
            UIColor.FromRGB(155, 155, 155).SetStroke();
            path.AddArc(_x0, _y0, _radius, 0.78f * (float)Math.PI, (2.22f) * (float)Math.PI, false);
            _g.AddPath(path);
            _g.DrawPath(CGPathDrawingMode.Stroke);
            CGPath path2 = new CGPath();
            _barColor.SetStroke();
            path2.AddArc(_x0, _y0, _radius, 0.78f * (float)Math.PI, (0.78f * (float)Math.PI + (float)(0.72 * _piMult) * (float)Math.PI), false);
            _g.AddPath(path2);
            _g.DrawPath(CGPathDrawingMode.Stroke);
            SetNeedsDisplay();
        }

    }
}

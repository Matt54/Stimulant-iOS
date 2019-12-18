using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;


namespace Stimulant
{
    [Register("RunningSymbol"), DesignTimeVisible(true)]


    public class RunningSymbol : UIView, System.ComponentModel.IComponent
    {
        public RunningSymbol(IntPtr handle) : base(handle) { }

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

        CGRect _frame;

        public RunningSymbol(int lineWidth)
        {
            _lineWidth = lineWidth;
        }

        public RunningSymbol(CGRect frame, int lineWidth)
        {
            _frame = frame;
            //UIColor.FromRGB(0, 0, 0).CGColor;
            _lineWidth = lineWidth;
            this.Frame = new CGRect(frame.X, frame.Y, frame.Width, frame.Height);
            this.BackgroundColor = UIColor.Clear;
        }

        public void UpdateFrame(CGRect frame)
        {
            _frame = frame;
            //_lineWidth = lineWidth;
            this.Frame = new CGRect(frame.X, frame.Y, frame.Width, frame.Height);
        }

        public override void Draw(CoreGraphics.CGRect rect)
        {
            base.Draw(rect);

            using (CGContext g = UIGraphics.GetCurrentContext())
            {
                _radius = (int)((this.Bounds.Width) / 2) - _lineWidth;
                DrawGraph(g, this.Bounds.GetMinX(), this.Bounds.GetMaxX(), this.Bounds.GetMinY(), this.Bounds.GetMaxY()); // Remember you changed this to min x
            };
        }

        public void DrawGraph(CGContext g, nfloat x0, nfloat x1, nfloat y0, nfloat y1)
        {

            nfloat frameWidth = x1 - x0;
            nfloat frameHeight = y1 - y0;

            nfloat padding = frameWidth / 8;

            nfloat insideWidth = frameWidth - padding * 2;
            //nfloat insideHeight = insideWidth;

            g.SetLineWidth(_lineWidth);
            g.SetStrokeColor(UIColor.FromRGB(0, 0, 0).CGColor);
            g.MoveTo(x0 + padding, y0 + padding);// + (frameHeight) / 2);
            g.AddLineToPoint(x0 + (frameWidth) / 2, y1 - padding);// y1 - padding);
            g.AddLineToPoint(x1 - padding, y0 + padding);// + (frameHeight) / 2);
            g.StrokePath();


            /*
            g.SetLineWidth(_lineWidth);

            g.SetStrokeColor(UIColor.FromRGB(155, 155, 155).CGColor);

            g.MoveTo(x0, y0);
            g.AddLineToPoint(x0 + (x1 - x0), y0);
            g.StrokePath();

            g.SetStrokeColor(_barColor.CGColor);
            g.MoveTo(x0, y0);
            g.AddLineToPoint(x0 + _progressPercent * (x1 - x0), y0);
            g.StrokePath();
            */


        }


    }
    
}

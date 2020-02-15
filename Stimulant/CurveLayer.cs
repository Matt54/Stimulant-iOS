using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Stimulant
{
    public class CurveLayer : UIView
    {

        CGPoint[] points;

        private int patternSlot;
        public int PatternSlot;
        public int GetPatternSlot()
        {
            return patternSlot;
        }
        public void SetPatternSlot(int ps)
        {
            patternSlot = ps;
        }

        private bool displayPoints;
        public bool DisplayPoints
        {
            get { return displayPoints; }
            set
            {
                displayPoints = value;
                InvertColors = !displayPoints;
                SetNeedsDisplay();
            }
        }

        private bool invertColors;
        public bool InvertColors
        {
            get { return invertColors; }
            set
            {
                invertColors = value;
                if(invertColors) BackgroundColor = UIColor.Black;
                else BackgroundColor = UIColor.White;
                SetNeedsDisplay();
            }
        }

        public void ResizeLayer(CGRect newRect)
        {
            //Here we are dealing with a resize of our DrawPattern object
            //I think this should actually be done inside the PatternSelection Class
            for (int i = 0; i < points.Length; i++)
            {
                points[i].X = points[i].X * (newRect.Width / Frame.Width);
                points[i].Y = points[i].Y * (newRect.Height / Frame.Height);
            }
            Update(new CGRect(0, 0, newRect.Width, newRect.Height), points);
            Frame = newRect;
        }

        // this is how we get values from patterns, you feed in an x location from 0-127 and it feeds back a y
        public int GetYForX(nfloat x)
        {
            nfloat xVal = x * Frame.Width / GlobalVar.domain;
            int foundIndex = 0;
            bool found = false;
            for (int i = 0; i < points.Length; i++)
            {
                if(points[i].X > xVal)
                {
                    foundIndex = i - 1;
                    found = true;
                    break;
                }
            }
            if ( (found) & (points.Length > foundIndex + 1) )
            {
                nfloat slope = ( (Frame.Height - points[foundIndex + 1].Y) - (Frame.Height - points[foundIndex].Y) )
                                / (points[foundIndex + 1].X - points[foundIndex].X);

                nfloat yVal = slope * (xVal - points[foundIndex].X) + (Frame.Height - points[foundIndex].Y);

                return (int)(yVal * GlobalVar.range / Frame.Height);
            }
            return 0;
        }

        public void UpdatePoints(CGPoint[] _points)
        {
            points = _points;
            SetNeedsDisplay();
        }

        public CGPoint[] GetPoints()
        {
            return points;
        }

        public void UpdateFrame(CGRect myRect)
        {
            Frame = myRect;
            SetNeedsDisplay();
        }

        public void Update(CGRect myRect, CGPoint[] _points)
        {
            Frame = myRect;
            points = _points;
            SetNeedsDisplay();
        }

        

        public CurveLayer()
        {
            BackgroundColor = UIColor.White;
        }

        public CurveLayer(CGRect myRect, CGPoint[] _points)
        {
            DisplayPoints = true;
            Frame = myRect;
            BackgroundColor = UIColor.Clear;
            points = _points;
        }

        public override void Draw(CoreGraphics.CGRect rect)
        {
            base.Draw(rect);

            using (CGContext g = UIGraphics.GetCurrentContext())
            {
                if (points != null)
                {
                    CGPath currentPath = new CGPath();
                    currentPath.AddLines(points);
                    g.AddPath(currentPath);
                    g.SetLineWidth(2);
                    if (invertColors) g.SetStrokeColor(UIColor.FromRGB(255, 255, 255).CGColor);
                    else g.SetStrokeColor(UIColor.FromRGB(0, 0, 0).CGColor);

                    g.AddLineToPoint(rect.Right, rect.Bottom);
                    g.AddLineToPoint(rect.Left, rect.Bottom);
                    g.ClosePath();

                    if (displayPoints)
                    {
                        g.SetAlpha(.7f);
                        g.SetFillColor(UIColor.FromRGB(100, 100, 100).CGColor);
                    }
                    else if (invertColors) g.SetFillColor(UIColor.FromRGB(255, 255, 255).CGColor);
                    else g.SetFillColor(UIColor.FromRGB(0, 0, 0).CGColor);

                    g.DrawPath(CGPathDrawingMode.FillStroke);
                    g.SetAlpha(1f);

                    if (displayPoints)
                    {
                        nfloat smallRadius = Frame.Width * .01f;
                        nfloat bigRadius = Frame.Width * .03f;
                        foreach (CGPoint point in points)
                        {
                            g.SetFillColor(UIColor.FromRGB(0, 0, 0).CGColor);
                            g.SetAlpha(.3f);
                            g.AddArc(point.X, point.Y, bigRadius, 0.0f, (float)Math.PI * 2.0f, true);
                            g.FillPath();
                            g.SetAlpha(1f);
                            g.AddArc(point.X, point.Y, smallRadius, 0.0f, (float)Math.PI * 2.0f, true);
                            g.FillPath();
                        }
                    }

                }
            }
        }

    }
}

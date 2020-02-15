using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;
using System.Diagnostics;

namespace Stimulant
{
    public class DrawPattern : UIViewController
    {

        public class GridLayer : UIView
        {
            int xNum;
            public nfloat[] xLocations;

            int yNum;
            public nfloat[] yLocations;

            public GridLayer(CGRect rect, int _xNum, int _yNum)
            {
                xNum = _xNum;
                yNum = _yNum;

                Frame = rect;
                SetXGridNum(xNum);
                SetYGridNum(yNum);
                BackgroundColor = UIColor.Clear;
            }

            void SetXGridNum(int num)
            {
                xLocations = new nfloat[num];

                for (int i = 0; i < num; i++)
                {
                    xLocations[i] = Frame.Width / num * i;
                }
            }

            void SetYGridNum(int num)
            {
                yLocations = new nfloat[num + 1];

                for (int i = 0; i < yLocations.Length; i++)
                {
                    yLocations[i] = Frame.Height / num * i;
                }
            }

            public override void Draw(CoreGraphics.CGRect rect)
            {
                base.Draw(rect);

                using (CGContext g = UIGraphics.GetCurrentContext())
                {
                    if ( (xLocations != null) && (yLocations != null) )
                    {
                        CGPath currentPath = new CGPath();

                        for (int i = 0; i < xLocations.Length; i++)
                            currentPath.AddLines(new CGPoint[] { new CGPoint(xLocations[i], 0), new CGPoint(xLocations[i], Frame.Height) });

                        for (int i = 0; i < yLocations.Length; i++)
                            currentPath.AddLines(new CGPoint[] { new CGPoint(0, yLocations[i]), new CGPoint(Frame.Width, yLocations[i]) });

                        g.SetLineWidth(1);
                        g.AddPath(currentPath);
                        g.SetStrokeColor(UIColor.FromRGB(0, 0, 0).CGColor);
                        g.DrawPath(CGPathDrawingMode.Stroke);
                    }
                }
            }
        }

        private CurveLayer curveLayer;
        public CurveLayer CurveLayer
        {
            get
            {
                return curveLayer;
            }
            set
            {
                curveLayer = value;
                curveLayer.BackgroundColor = UIColor.Clear;
                points = curveLayer.GetPoints();
                View.AddSubview(curveLayer);
            }
        }

        private GridLayer gridLayer;


        CGPoint[] points;

        CGPoint currentPoint;
        CGPoint monitorPoint;

        int currentPointIndex;

        nfloat proximityCutoff;
        double longPressTime;

        bool newPoint;
        bool touchMonitoring;
        bool isEndPoint;

        private bool isSnapToGrid;
        public bool IsSnapToGrid
        {
            get { return isSnapToGrid; }
            set {
                isSnapToGrid = value;
                if (isSnapToGrid)
                    SnapPoints();
            }
        }

        Stopwatch stopwatch;

        public DrawPattern(CGRect myRect)
        {
            View.Frame = myRect;
            isSnapToGrid = false;

            View.BackgroundColor = UIColor.White;
            longPressTime = 200;

            points = new CGPoint[] { new CGPoint(0, myRect.Bottom - myRect.Top), new CGPoint(myRect.Right - myRect.Left - 0, 0) };
            proximityCutoff = View.Frame.Width * .03f;

            CGRect curveLayerFrame = new CGRect(0, 0, View.Frame.Width, View.Frame.Height);
            curveLayer = new CurveLayer(curveLayerFrame, points);

            gridLayer = new GridLayer(curveLayerFrame,16, 16);

            View.AddSubview(gridLayer);
            View.AddSubview(curveLayer);

            stopwatch = new Stopwatch();
        }

        public void ResizeLayer(CGRect newRect)
        {
            //Here we are dealing with a resize of our DrawPattern object
            //I think this should actually be done inside the PatternSelection Class
            for (int i = 0; i < points.Length; i++)
            {
                points[i].X = points[i].X * (newRect.Width / View.Frame.Width);
                points[i].Y = points[i].Y * (newRect.Height / View.Frame.Height);
            }
            curveLayer.Update( new CGRect(0, 0, newRect.Width, newRect.Height) , points);
            View.Frame = newRect;
        }

        public void SizeLayerToModify(CGRect curveLayerRect)
        {
            //Here we are dealing with sizing a Curve Layer to our DrawPattern
            for (int i = 0; i < points.Length; i++)
            {
                points[i].X = points[i].X * (View.Frame.Width / curveLayerRect.Width);
                points[i].Y = points[i].Y * (View.Frame.Height / curveLayerRect.Height);
            }
            curveLayer.Update(new CGRect(0, 0, View.Frame.Width, View.Frame.Height), points);
        }

        public void SetToPatternView()
        {
            curveLayer.DisplayPoints = false;
        }

        public void SetToPatternModify()
        {
            curveLayer.DisplayPoints = true;
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            UITouch touch = touches.AnyObject as UITouch;
            CGPoint p = touch.LocationInView(touch.View);

            stopwatch.Restart(); //start the timer

            isEndPoint = CheckBounds(p); // if true - this sets our index to lower or upper bound (now moving)
            if(!isEndPoint)
            {
                bool overlap = CatchOverlap(p); // if true - this sets our index to overlap point found (now moving or deleting)
                if (!overlap)
                {
                    newPoint = true;
                    AddPoint(p); // Our index is set to the newly added point
                }
                else
                {
                    touchMonitoring = true; // flag to watch the touch to see if we have a long press
                    monitorPoint = p; // remember where the point is
                }
            }
        }

        public bool CatchOverlap(CGPoint p)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if ((Math.Abs(points[i].X - p.X) < 2 * proximityCutoff) & (Math.Abs(points[i].Y - p.Y) < 2 * proximityCutoff))
                {
                    currentPoint = p;
                    currentPointIndex = i;
                    return true;
                }
            }
            return false;
        }

        public bool CheckBounds(CGPoint p)
        {
            if(CheckLowerBound(p)) return true;
            if (CheckUpperBound(p)) return true;
            return false;
        }

        public bool CheckLowerBound(CGPoint p)
        {
            if (p.X < proximityCutoff)
            {
                currentPoint = p;
                currentPointIndex = 0;
                return true;
            }
            return false;
        }

        public bool CheckUpperBound(CGPoint p)
        {
            if ((View.Frame.Width - p.X) < proximityCutoff)
            {
                currentPoint = p;
                currentPointIndex = points.Length - 1;
                return true;
            }
            return false;
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);
            UITouch touch = touches.AnyObject as UITouch;
            currentPoint = touch.LocationInView(touch.View);

            if (newPoint || isEndPoint)
            {
                if (isSnapToGrid)
                {
                    bool stillInRange = CheckDragProximity();
                    if (!stillInRange) UpdatePoint();
                }
                else UpdatePoint(); ;
            }
            else
            {
                if (touchMonitoring)
                {
                    bool stillInRange = CheckDragProximity();
                    if (stillInRange)
                    {
                        if (stopwatch.ElapsedMilliseconds > longPressTime)
                        {
                            touchMonitoring = false;
                            RemovePoint();
                        }
                    }
                    else UpdatePoint();
                }
            }
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            if (touchMonitoring)
            {
                bool stillInRange = CheckDragProximity();
                if (stillInRange)
                {
                    RemovePoint();
                }
            }

            if (touchMonitoring) touchMonitoring = false;
            if (newPoint) newPoint = false;
            if (isEndPoint) isEndPoint = false;
        }

        public void AddPoint(CGPoint p)
        {
            currentPoint = p;
            CGPoint[] prevPoints = points;
            bool foundLocation = false;
            points = new CGPoint[prevPoints.Length + 1];
            for(int i = 0; i < prevPoints.Length; i++)
            {
                if (!foundLocation)
                {
                    if (prevPoints[i].X < currentPoint.X) points[i] = prevPoints[i];
                    else
                    {
                        points[i] = currentPoint;
                        currentPointIndex = i;
                        foundLocation = true;
                    }
                }
                else
                {
                    points[i] = prevPoints[i - 1];
                }
            }
            points[prevPoints.Length] = prevPoints[prevPoints.Length - 1];
            if (isSnapToGrid) SnapPoint();
            else curveLayer.UpdatePoints(points);
        }

        public void RemovePoint()
        {
            Debug.WriteLine("");
            Debug.WriteLine("current point index: " + currentPointIndex);
            Debug.WriteLine("current point x: " + currentPoint.X);
            Debug.WriteLine("current point y: " + currentPoint.Y);
            Debug.WriteLine("");

            CGPoint[] prevPoints = points;
            bool foundLocation = false;
            points = new CGPoint[prevPoints.Length - 1];

            for (int i = 0; i < prevPoints.Length - 1; i++)
            {
                if (!foundLocation)
                {
                    if ((Math.Abs(prevPoints[i].X - currentPoint.X) < proximityCutoff)
                        & (Math.Abs(prevPoints[i].Y - currentPoint.Y) < proximityCutoff))
                    {
                        foundLocation = true;
                        points[i] = prevPoints[i + 1];
                    }
                    else points[i] = prevPoints[i];
                }
                else
                {
                    points[i] = prevPoints[i + 1];
                }
            }

            if (foundLocation)
            {
                //numPoints--;
                curveLayer.UpdatePoints(points);
            }
            else points = prevPoints;
        }

        public void UpdatePoint()
        {
            
            bool isOutOfY = CheckY(currentPoint);
            if (!isOutOfY)
            {
                if (isEndPoint)
                {
                    if (currentPointIndex == 0) currentPoint.X = 0;
                    else currentPoint.X = View.Frame.Width;
                    UpdatePoint(currentPoint, currentPointIndex);
                }

                else if (currentPointIndex == 1) //First Inside Graph Point - Watch Left For Boundary Point
                {
                    if (points[currentPointIndex - 1].X + proximityCutoff > currentPoint.X) //if we are too far left we pick the lowest x value
                    {
                        currentPoint.X = points[currentPointIndex - 1].X + proximityCutoff;
                    }

                    //Last Inside Graph Point - Watch Right For Boundary Point
                    //AND if we are too far right we pick the highest x value
                    else if ( (currentPointIndex == points.Length - 2) & (points[currentPointIndex + 1].X - proximityCutoff < currentPoint.X) ) 
                    {
                        currentPoint.X = points[currentPointIndex + 1].X - proximityCutoff;
                    }

                    else if (points[currentPointIndex + 1].X <= currentPoint.X) //Pressed Into Another Point On The Right
                    {
                        currentPoint.X = points[currentPointIndex + 1].X;
                    }
                    UpdatePoint(currentPoint, currentPointIndex);
                }
                else if (currentPointIndex == points.Length - 2) //Last Inside Graph Point - Watch Right For Boundary Point
                {
                    if (points[currentPointIndex + 1].X - proximityCutoff < currentPoint.X) //if we are too far right we pick the highest x value
                    {
                        currentPoint.X = points[currentPointIndex + 1].X - proximityCutoff;
                    }
                    else if (points[currentPointIndex - 1].X >= currentPoint.X) //Pressed Into Another Point On The Left
                    {
                        currentPoint.X = points[currentPointIndex - 1].X;
                    }
                    UpdatePoint(currentPoint, currentPointIndex);
                }
                
                else
                {
                    //Snap to its X value bounds
                    if (points[currentPointIndex - 1].X >= currentPoint.X) //Pressed Into Another Point On The Left
                    {
                        currentPoint.X = points[currentPointIndex - 1].X; 
                    }
                    else if (points[currentPointIndex + 1].X <= currentPoint.X) //Pressed Into Another Point On The Right
                    {
                        currentPoint.X = points[currentPointIndex + 1].X;
                    }

                    UpdatePoint(currentPoint, currentPointIndex);
                }
            }
        }

        public bool WithinSurroundingPointsPadBoth()
        {
            if ( (points[currentPointIndex - 1].X < currentPoint.X)// + proximityCutoff < currentPoint.X)
                        && (points[currentPointIndex + 1].X > currentPoint.X) )// - proximityCutoff > currentPoint.X))
                return true;
            return false;
        }

        public bool WithinSurroundingPointsPadFirst()
        {
            if ((points[currentPointIndex - 1].X + proximityCutoff < currentPoint.X)
                        && (points[currentPointIndex + 1].X > currentPoint.X)) return true;
            return false;
        }

        public bool WithinSurroundingPointsPadLast()
        {
            if ((points[currentPointIndex - 1].X < currentPoint.X)
                        && (points[currentPointIndex + 1].X - proximityCutoff > currentPoint.X)) return true;
            return false;
        }

        public bool WithinSurroundingPoints()
        {
            if ((points[currentPointIndex - 1].X < currentPoint.X)
                        && (points[currentPointIndex + 1].X > currentPoint.X)) return true;
            return false;
        }

        public void UpdatePoint(CGPoint p, int index)
        {
            if (isSnapToGrid)
            {
                bool isX = false;
                bool isY = false;
                if (Math.Abs(points[index].X - p.X) > proximityCutoff) isX = true;
                if (Math.Abs(points[index].Y - p.Y) > proximityCutoff) isY = true;
                if (isX)
                {
                    bool isRight;
                    if ( (points[index].X - p.X) < 0) isRight = true;
                    else isRight = false;
                    for (int i = 0; i < gridLayer.xLocations.Length; i++)
                    {
                        if( isRight && (points[index].X < gridLayer.xLocations[i]) )
                        {
                            points[index].X = gridLayer.xLocations[i]; 
                            break;
                        }
                        else if ( (!isRight) && (points[index].X <= gridLayer.xLocations[i]) )
                        {
                            if (i == 1) points[index].X = gridLayer.xLocations[i];
                            else points[index].X = gridLayer.xLocations[i - 1];
                            break;
                        }
                    }
                    curveLayer.UpdatePoints(points);
                }
                if (isY)
                {
                    bool isDown;
                    if ( (points[index].Y - p.Y) < 0) isDown = true;
                    else isDown = false;
                    for (int i = 0; i < gridLayer.yLocations.Length; i++)
                    {
                        if ( isDown && (points[index].Y < gridLayer.yLocations[i]))
                        {
                            points[index].Y = gridLayer.yLocations[i];
                            break;
                        }
                        else if ( (!isDown) && (points[index].Y <= gridLayer.yLocations[i]))
                        {
                            points[index].Y = gridLayer.yLocations[i - 1];
                            break;
                        }
                    }
                    curveLayer.UpdatePoints(points);
                }
            }
            else
            {
                points[index] = p;
                curveLayer.UpdatePoints(points);
            }
        }

        public void SnapPoint()
        {
            for (int ii = 1; ii < gridLayer.xLocations.Length; ii++)
            {
                if (points[currentPointIndex].X < gridLayer.xLocations[ii])
                {
                    if (Math.Abs(points[currentPointIndex].X - gridLayer.xLocations[ii]) > Math.Abs(points[currentPointIndex].X - gridLayer.xLocations[ii - 1]))
                    {
                        points[currentPointIndex].X = gridLayer.xLocations[ii - 1];
                    }
                    else if (Math.Abs(points[currentPointIndex].X - gridLayer.xLocations[ii]) < Math.Abs(points[currentPointIndex].X - gridLayer.xLocations[ii - 1]))
                    {
                        points[currentPointIndex].X = gridLayer.xLocations[ii];
                    }
                    break;
                }
                else if (points[currentPointIndex].X == gridLayer.xLocations[ii]) break; //don't move the point if it's already on the xGrid
            }

            for (int ii = 1; ii < gridLayer.yLocations.Length; ii++)
            {
                if (points[currentPointIndex].Y < gridLayer.yLocations[ii])
                {
                    if (Math.Abs(points[currentPointIndex].Y - gridLayer.yLocations[ii]) > Math.Abs(points[currentPointIndex].Y - gridLayer.yLocations[ii - 1]))
                    {
                        points[currentPointIndex].Y = gridLayer.yLocations[ii - 1];
                    }
                    else if (Math.Abs(points[currentPointIndex].Y - gridLayer.yLocations[ii]) < Math.Abs(points[currentPointIndex].Y - gridLayer.yLocations[ii - 1]))
                    {
                        points[currentPointIndex].Y = gridLayer.yLocations[ii];
                    }
                    break;
                }
                else if (points[currentPointIndex].Y == gridLayer.yLocations[ii]) break; //don't move the point if it's already on the xGrid
            }
            curveLayer.UpdatePoints(points);
        }

        public void SnapPoints()
        {
            for (int i = 1; i < points.Length; i++)
            {
                
                for (int ii = 1; ii < gridLayer.xLocations.Length; ii++)
                {
                    if (points[i].X < gridLayer.xLocations[ii])
                    {
                        if (Math.Abs(points[i].X - gridLayer.xLocations[ii]) > Math.Abs(points[i].X - gridLayer.xLocations[ii - 1]))
                        {
                            // conditional prevents snap to left boundary
                            if (!(ii == 1)) points[i].X = gridLayer.xLocations[ii - 1];
                            else points[i].X = gridLayer.xLocations[ii];
                        }
                        else if (Math.Abs(points[i].X - gridLayer.xLocations[ii]) < Math.Abs(points[i].X - gridLayer.xLocations[ii - 1]))
                        {
                            points[i].X = gridLayer.xLocations[ii];
                        }
                        break;
                    }
                    else if (points[i].X == gridLayer.xLocations[ii]) break; //don't move the point if it's already on the xGrid
                    else if ( (ii == gridLayer.xLocations.Length - 1) && (i != points.Length - 1)) 
                    {
                        points[i].X = gridLayer.xLocations[ii]; //Grid Snap On The Second Last Line
                    }
                }

                for (int ii = 1; ii < gridLayer.yLocations.Length; ii++)
                {
                    if (points[i].Y < gridLayer.yLocations[ii])
                    {
                        if (Math.Abs(points[i].Y - gridLayer.yLocations[ii]) > Math.Abs(points[i].Y - gridLayer.yLocations[ii - 1]))
                        {
                            points[i].Y = gridLayer.yLocations[ii - 1];
                        }
                        else if (Math.Abs(points[i].Y - gridLayer.yLocations[ii]) < Math.Abs(points[i].Y - gridLayer.yLocations[ii - 1]))
                        {
                            points[i].Y = gridLayer.yLocations[ii];
                        }
                        break;
                    }
                    else if (points[i].Y == gridLayer.yLocations[ii]) break; //don't move the point if it's already on the yGrid
                }

            }
            curveLayer.UpdatePoints(points);
        }

        public bool CheckY(CGPoint p)
        {
            if (p.Y < 0 || p.Y > View.Frame.Height) return true;
            return false;
        }

        public bool CheckPoint()
        {
            for (int i = 0; i < points.Length; i++)
            {
                if ((Math.Abs(points[i].X - currentPoint.X) < proximityCutoff)
                    & (Math.Abs(points[i].Y - currentPoint.Y) < proximityCutoff))
                {
                    currentPoint = points[i];
                    currentPointIndex = i;
                    return true;
                }
            }
            return false;
        }

        public bool CheckDragProximity()
        {
            if ((Math.Abs(monitorPoint.X - currentPoint.X) < proximityCutoff)
                & (Math.Abs(monitorPoint.Y - currentPoint.Y) < proximityCutoff))
            { return true;}
            return false;
        }

        public void FlipPoints()
        {
            nfloat[] xVal = new nfloat[points.Length];
            nfloat[] yVal = new nfloat[points.Length];

            //CGPoint[] newPoints = new CGPoint[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                xVal[i] = View.Frame.Width - points[points.Length - i - 1].X;
                yVal[i] = points[points.Length - i - 1].Y;
            }
            for (int i = 0; i < points.Length; i++)
            {
                points[i].X = xVal[i];
                points[i].Y = yVal[i];
            }
            curveLayer.UpdatePoints(points);
        }


    }
}

using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;
using System.Diagnostics;

namespace Stimulant
{

    public class PatternSelection : UIViewController
    {

        public PatternSelection(CGRect rect, int numberOfPatterns)
        {
            stopwatch = new Stopwatch();
            numPatterns = numberOfPatterns;
            View.Frame = rect;
            //View.BackgroundColor = UIColor.Black;
            CreateLabel(new CGRect(0, 0, rect.Width, rect.Height / 2));
            CreateViews(new CGRect(0, label.Frame.Bottom, rect.Width, rect.Height / 2));
            AddDefaultPatternPoints();

            View.AddSubview(label);
            AddViewArrayToView();
            SetCurveArrayStates();
        }

        void CreateLabel(CGRect rect)
        {
            label = new UILabel();
            label.Frame = rect;
            label.TextAlignment = UITextAlignment.Center;
        }

        public void UpdateLabelText(string patternText)
        {
            label.Text = patternText;
        }

        void SetCurveArrayStates()
        {
            for (int i = 0; i < curveArray.Length; i++)
            {
                curveArray[i].BackgroundColor = UIColor.White;
            }
            curveArray[currentSelection].InvertColors = true;
        }

        void CreateViews(CGRect rect)
        {
            //buttonArray = new UIButton[numPatterns];
            viewArray = new UIView[numPatterns];
            curveArray = new CurveLayer[numPatterns];
            SetFrames(rect);
            SetButtonImages(0);
        }

        public void SetFrames(CGRect rect)//, float padding)
        {
            frameArray = new CGRect[numPatterns];

            float margin = 5.0f;
            padding = 2.0f;
            for (int i = 0; i < curveArray.Length; i++)
            {
                int n = curveArray.Length;
                float individualWidth = (float)((rect.Width / n) - margin * (n + 1) / n);

                viewArray[i] = new UIView();
                viewArray[i].Frame = new CGRect(rect.Left + margin * (i + 1) + individualWidth * i,
                                                    rect.Top + margin,
                                                    individualWidth,
                                                    individualWidth);

                viewArray[i].BackgroundColor = UIColor.Black;

                curveWidth = (float)((rect.Width / n) - (margin + 2 * padding) * (n + 1) / n);

                curveArray[i] = new CurveLayer();
                /*
                curveArray[i].Frame = new CGRect(rect.Left + margin * (i + 1) + curveWidth * i,
                                                    rect.Top + margin + padding,
                                                    curveWidth,
                                                    curveWidth);
                                                    */

                curveArray[i].Frame = new CGRect(padding, padding, curveWidth, curveWidth);
                //
                viewArray[i].AddSubview(curveArray[i]);
                
                /*
                buttonArray[i] = UIButton.FromType(UIButtonType.Custom);
                buttonArray[i].Frame = frameArray[i];
                */

                //curveArray[i] = new CurveLayer();
                //curveArray[i].Frame = frameArray[i];


                /*new CGRect(rect.Left + padding * (i + 1) + individualWidth * i,
                                                rect.Top + padding,
                                                individualWidth,
                                                individualWidth);*/

                //buttonArray[i].TouchDown += HandleButtonPress;
            }
        }

        void SetButtonImages(int selectedPattern)
        {
            for (int i = 0; i < curveArray.Length; i++)
            {
                curveArray[i].BackgroundColor = UIColor.Black;

                /*
                if (i == selectedPattern)
                {
                    buttonArray[i].SetImage(UIImage.FromFile("graphicP" + Convert.ToString(i + 1) + "NOn.png"), UIControlState.Normal);
                }
                else
                {
                    buttonArray[i].SetImage(UIImage.FromFile("graphicP" + Convert.ToString(i + 1) + "NOff.png"), UIControlState.Normal);
                }
                buttonArray[i].SetImage(UIImage.FromFile("graphicP" + Convert.ToString(i + 1) + "NOn.png"), UIControlState.Highlighted);
                buttonArray[i].SetImage(UIImage.FromFile("graphicP" + Convert.ToString(i + 1) + "NOff.png"), UIControlState.Disabled);
                */
            }
        }

        void AddDefaultPatternPoints()
        {
            nfloat left = 0;
            nfloat right = curveArray[0].Frame.Right - curveArray[0].Frame.Left;
            nfloat mid = right / 2;

            nfloat top = 0;
            nfloat bottom = curveArray[0].Frame.Bottom - curveArray[0].Frame.Top;
            
            
            /*
            CGPoint[] P1Points = { new CGPoint(left, bottom),
                                    new CGPoint(mid, top),
                                    new CGPoint(right, bottom)};
            CGPoint[] P2Points = { new CGPoint(left, bottom),
                                    new CGPoint(right, top) };
            CGPoint[] P3Points = { new CGPoint(left, bottom),
                                    new CGPoint(right, top) };
            CGPoint[] P4Points = { new CGPoint(left, bottom),
                                    new CGPoint(right, top) };
            CGPoint[] P5Points = { new CGPoint(left, bottom),
                                    new CGPoint(right, top) };
            CGPoint[] P6Points = { new CGPoint(left, bottom),
                                    new CGPoint(right, top) };
            CGPoint[] P7Points = { new CGPoint(left, bottom),
                                    new CGPoint(right, top) };
            CGPoint[] P8Points = { new CGPoint(left, bottom),
                                    new CGPoint(right, top) };
            */

            /*
            curveArray[0].UpdatePoints(P1Points);
            curveArray[0].BackgroundColor = UIColor.Clear;
            curveArray[1].UpdatePoints(P2Points);
            curveArray[1].BackgroundColor = UIColor.Clear;
            */

            /*
            curveArray[0].UpdatePoints(P1Points);
            curveArray[0].BackgroundColor = UIColor.Clear;
            curveArray[1].UpdatePoints(P2Points);
            curveArray[1].BackgroundColor = UIColor.Clear;
            curveArray[2].UpdatePoints(P3Points);
            curveArray[2].BackgroundColor = UIColor.Clear;
            curveArray[3].UpdatePoints(P4Points);
            curveArray[3].BackgroundColor = UIColor.Clear;
            curveArray[4].UpdatePoints(P5Points);
            curveArray[4].BackgroundColor = UIColor.Clear;
            curveArray[5].UpdatePoints(P6Points);
            curveArray[5].BackgroundColor = UIColor.Clear;
            curveArray[6].UpdatePoints(P7Points);
            curveArray[6].BackgroundColor = UIColor.Clear;
            curveArray[7].UpdatePoints(P8Points);
            curveArray[7].BackgroundColor = UIColor.Clear;
            */

            
            for (int i = 1; i < curveArray.Length; i++)
            {
                CGPoint[] Points = { new CGPoint(left, bottom),
                                    new CGPoint(mid, top),
                                    new CGPoint(right, bottom)};
                curveArray[i].UpdatePoints(Points);
                curveArray[i].BackgroundColor = UIColor.Clear;
                curveArray[i].SetPatternSlot(i);
                /*
                curveArray[i].UpdatePoints(P2Points);
                curveArray[i].BackgroundColor = UIColor.Clear;
                */
            }
            

        }

        /*
        public void SetButtonView(int numPattern, CurveLayer curveView)
        {
            curveView.Frame = buttonArray[numPattern].Frame;
            buttonArray[numPattern].InsertSubview(curveView, 0);
            //buttonArray[numPattern].AddSubview(curveView);
        }
        */

        void AddViewArrayToView()
        {
            for (int i = 0; i < viewArray.Length; i++)
            {
                //View.AddSubview(buttonArray[i]);
                View.AddSubview(viewArray[i]);
            }
        }

        


       

        /*
        void HandleButtonPress(object sender, System.EventArgs e)
        {
            for (int i = 0; i < buttonArray.Length; i++)
            {
                if(sender == buttonArray[i])
                {
                    currentSelection = i;
                }
            }
            SetButtonImages(currentSelection);

            ButtonPressed?.Invoke(this, e);

            //UpdateLabelText();
        }
        */

        public int GetMidiValFromPattern(nfloat x, int patternIndex)
        {
            return curveArray[patternIndex].GetYForX(x);
        }

        public int GetPatternNumber()
        {
            return currentSelection + 1;
        }

        public void SetPattern(int selectedPattern)
        {
            currentSelection = selectedPattern - 1;
            SetButtonImages(currentSelection);
        }

        public void SetVisibility(bool isVisible)
        {

        }

        public CurveLayer GetSelectedCurveLayer()
        {
            return curveArray[currentSelection];
        }

        public void CopyCurveLayer(CurveLayer cL)
        {
            cL.ResizeLayer( new CGRect(padding, padding, curveWidth, curveWidth) );
            cL.DisplayPoints = false;
            //cL.InvertColors = true;

            curveArray[cL.GetPatternSlot()] = cL;
            //curveArray[cL.GetPatternSlot()].BackgroundColor = UIColor.White;

            viewArray[cL.GetPatternSlot()].AddSubview(curveArray[cL.GetPatternSlot()]);
        }

        public event EventHandler ButtonPressed;
        public event EventHandler ButtonHeld;

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            //if (!stopwatch.IsRunning) stopwatch.Start();
            //stopwatch.Reset();
            stopwatch.Restart();
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            if (!isBeingModified)
            {
                EventArgs e = new EventArgs();
                base.TouchesBegan(touches, evt);
                UITouch touch = touches.AnyObject as UITouch;

                CGPoint p = touch.LocationInView(touch.View);
                for (int i = 0; i < curveArray.Length; i++)
                {
                    if (viewArray[i].Frame.Contains(touch.LocationInView(View)))
                    {
                        if (stopwatch.ElapsedMilliseconds > 200)
                        {
                            curveArray[currentSelection].InvertColors = false;
                            currentSelection = i;
                            curveArray[currentSelection].InvertColors = true;
                            ButtonHeld?.Invoke(this, e);

                            //ButtonPressed?.Invoke(this, e);
                        }
                        else
                        {
                            // the touch event happened inside the UIView
                            curveArray[currentSelection].InvertColors = false;
                            currentSelection = i;
                            curveArray[currentSelection].InvertColors = true;
                            ButtonPressed?.Invoke(this, e);
                        }
                    }
                }
            }
        }

        private bool isBeingModified;
        public bool IsBeingModified
        {
            get { return isBeingModified; }
            set
            {
                isBeingModified = value;
            }
        }

        private nfloat curveWidth;
        private float padding;

        private Stopwatch stopwatch;

        private int numPatterns;
        //private UIButton[] buttonArray;
        private CGRect[] frameArray;

        private UIView[] viewArray;
        private CurveLayer[] curveArray;

        private UILabel label = new UILabel();

        private int currentSelection;
    }
}

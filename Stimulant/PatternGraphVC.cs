using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Stimulant
{
    public class PatternGraphVC : UIViewController
    {
        DrawPattern drawPattern;
        UIButton buttonSnap;
        UIButton buttonSave;
        UIButton buttonFlip;
        bool isSmall;

        public PatternGraphVC(CGRect rect)
        {
            View.Frame = rect;
            View.BackgroundColor = UIColor.Black;

            AddSnapButton(new CGRect(0, 0, View.Frame.Width / 3, View.Frame.Height * .1));
            AddSaveButton(new CGRect(View.Frame.Width / 3, 0, View.Frame.Width / 3, View.Frame.Height * .1));
            AddFlipButton(new CGRect(View.Frame.Width * 2 / 3,0 , View.Frame.Width / 3, View.Frame.Height * .1));
            AddCurveLayerVC( new CGRect(0, View.Frame.Height * .1, View.Frame.Width, View.Frame.Height * .9) );
        }

        void AddSnapButton(CGRect rect)
        {
            buttonSnap = new UIButton(rect);
            buttonSnap.SetTitle("Grid Snap", UIControlState.Normal);
            View.AddSubview(buttonSnap);
            buttonSnap.TouchUpInside += (sender, e) => {
                drawPattern.IsSnapToGrid = !drawPattern.IsSnapToGrid;

                if(drawPattern.IsSnapToGrid)
                {
                    buttonSnap.SetTitleColor(UIColor.Black, UIControlState.Normal);
                    buttonSnap.BackgroundColor = UIColor.White;
                }
                else
                {
                    buttonSnap.SetTitleColor(UIColor.White, UIControlState.Normal);
                    buttonSnap.BackgroundColor = UIColor.Black;
                }
            };
        }
        void AddSaveButton(CGRect rect)
        {
            buttonSave = new UIButton(rect);
            buttonSave.SetTitle("Save", UIControlState.Normal);
            View.AddSubview(buttonSave);
            buttonSave.TouchUpInside += (sender, e) => {
                //DoSomething();
                PatternModifyButtonPress?.Invoke(this, e);
            };
        }
        void AddFlipButton(CGRect rect)
        {
            buttonFlip = new UIButton(rect);
            buttonFlip.SetTitle("Flip", UIControlState.Normal);
            View.AddSubview(buttonFlip);
            buttonFlip.TouchUpInside += (sender, e) => {
                drawPattern.FlipPoints();
                //PatternModifyButtonPress?.Invoke(this, e);
            };
        }


        public event EventHandler PatternModifyButtonPress;

        void DoSomething()
        {
            //PatternModifyButtonPress?.Invoke(this, e);
            if (!isSmall)
            {
                isSmall = true;
                drawPattern.ResizeLayer(new CGRect(View.Frame.Width * .1, View.Frame.Height * .1, View.Frame.Width * .9, View.Frame.Height * .5));
                drawPattern.SetToPatternView();
            }
            else
            {
                isSmall = false;
                drawPattern.ResizeLayer(new CGRect(0 , View.Frame.Height * .1, View.Frame.Width, View.Frame.Height * .9));
                drawPattern.SetToPatternModify();
            }
        }

        void AddCurveLayerVC(CGRect rect)
        {
            drawPattern = new DrawPattern(rect);
            View.AddSubview(drawPattern.View);
            AddChildViewController(drawPattern);
        }

        public CurveLayer GetCurveLayer()
        {
            return drawPattern.CurveLayer;
        }

        public void SetCurveLayer(CurveLayer newCurveLayer)
        {
            newCurveLayer.DisplayPoints = true;
            drawPattern.CurveLayer = newCurveLayer;
            drawPattern.SizeLayerToModify(newCurveLayer.Frame);
        }

        public void SetPatternSlot(int num)
        {
            drawPattern.CurveLayer.SetPatternSlot(num - 1);
        }

    }
}

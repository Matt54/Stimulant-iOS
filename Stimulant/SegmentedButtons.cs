using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Stimulant
{

    [Register("SegmentedButtons"), DesignTimeVisible(true)]

    public class SegmentedButtons : UIView, System.ComponentModel.IComponent
    {
        public SegmentedButtons(IntPtr handle) : base(handle) { }

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


        public SegmentedButtons(int numOfButtons, float width, float height, float xloc, float yloc)
        {
            buttonArray = new UIButton[numOfButtons];
            numberOfButtons = numOfButtons;
            setCustomType(true);
            setAllImages("graphicP1NOff.png");
            setFrames(width, height, xloc, yloc);
            addButtonsToView();
        }

        public void addButtonsToView()
        {
            for (int i = 0; i < numberOfButtons; i++)
            {
                AddSubview(buttonArray[i]);
            }
        }

        public void setCustomType(bool custom)
        {
            for (int i = 0; i < numberOfButtons; i++)
            {
                if (custom)
                {
                    buttonArray[i] = UIButton.FromType(UIButtonType.Custom);
                }
                else
                {
                    buttonArray[i] = UIButton.FromType(UIButtonType.Plain);
                }
            }
        }

        public void setFrames(float width, float height, float xloc, float yloc)
        {
            for (int i = 0; i < numberOfButtons; i++)
            {
                buttonArray[i].Frame = new CGRect(width/ numberOfButtons * i,
                    yloc,
                    (width / numberOfButtons),
                    height);
            }
        }

        public void setAllImages(string imgStr)
        {
            for(int i = 0; i < numberOfButtons; i++)
            {
                setAllStates(i, imgStr);
            }
        }

        public void setAllStates(int btnNum, string imgStr)
        {
            buttonArray[btnNum].SetImage(UIImage.FromFile(imgStr), UIControlState.Normal);
            buttonArray[btnNum].SetImage(UIImage.FromFile(imgStr), UIControlState.Highlighted);
            buttonArray[btnNum].SetImage(UIImage.FromFile(imgStr), UIControlState.Disabled);
        }

        public void setSpecificState(int btnNum, string imgStr, UIControlState state)
        {
            buttonArray[btnNum].SetImage(UIImage.FromFile(imgStr), state);
        }

        public UIButton[] buttonArray;
        private int numberOfButtons;
    }
}

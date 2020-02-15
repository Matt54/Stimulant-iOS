using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;
using System.Diagnostics;

namespace Stimulant
{
    public class SceneDisplay : UIViewController
    {
        public SceneDisplay(CGRect rect, int _numScenes)
        {
            numScenes = _numScenes;
            View.Frame = rect;
            View.Hidden = true;
            View.BackgroundColor = UIColor.Black;

            CreateInnerRect(rect);
            CreateSceneArray();

            stopwatch = new Stopwatch();

            float effectiveHeight = (float)rect.Height - borderWidth * 2;
            lineWidth = 3;

            buttonsRect = new CGRect(0, borderWidth + effectiveHeight * .25, rect.Width, effectiveHeight * .5);

            CreateButtons(buttonsRect);
            CreateRunningSymbol(new CGRect(0, borderWidth, rect.Width, effectiveHeight *.25));
            CreateStepper( new CGRect(0, borderWidth + effectiveHeight * .75, rect.Width, effectiveHeight * .25) );

            View.AddSubview(innerRect);
            AddButtonsToView();
            View.AddSubview(runningSymbol);
            View.AddSubview(stepper);
        }

        void CreateInnerRect(CGRect rect)
        {
            borderWidth = 3.0f;
            innerRect = new UIView(new CGRect(borderWidth, borderWidth, rect.Width - 2 * borderWidth, rect.Height - 2 * borderWidth));
            innerRect.BackgroundColor = UIColor.White;
        }

        void CreateStepper(CGRect rect)
        {
            stepper = new UIStepper(rect);
            stepper.Value = numScenes;
            stepper.MinimumValue = 2;
            stepper.MaximumValue = 20;
            stepper.StepValue = 1;
            stepper.ValueChanged += HandleStepperChange;
        }

        private void CreateButtons(CGRect rect)
        {
            buttonArray = new UIButton[numScenes];
            margin = (float)(rect.Width / (4 * numScenes));
            buttonWidth = (float)(rect.Width / numScenes) - (margin * (numScenes + 3) / numScenes);
            float yLocation = (float)(rect.Top + (rect.Height - buttonWidth) / 2);

            //If buttons are too big, size button to height of frame and recalculate margins between them
            if (rect.Height < buttonWidth)
            {
                buttonWidth = (float) rect.Height;
                yLocation = (float)(rect.Top + (rect.Height - buttonWidth) / 2);
                margin = (float)(rect.Width - buttonWidth * numScenes) / (numScenes + 3);
            }

            for (int ii = 0; ii < buttonArray.Length; ii++)
            {
                buttonArray[ii] = UIButton.FromType(UIButtonType.Custom);

                UpdateSceneGraphic(ii);

                buttonArray[ii].Frame = new CGRect(margin + margin * (ii + 1) + buttonWidth * ii, yLocation, buttonWidth, buttonWidth);
                buttonArray[ii].TouchDown += HandleButtonPress;
                buttonArray[ii].TouchUpInside += HandleButtonRelease;
            }
        }

        void UpdateSceneGraphic(int sceneToChange)
        {
            string fileName = LookUpStringForGraphic(sceneArray[sceneToChange].Opposite, sceneArray[sceneToChange].PatternNumber, sceneArray[sceneToChange].IsSelected);
            buttonArray[sceneToChange].SetImage(UIImage.FromFile(fileName), UIControlState.Normal);
            buttonArray[sceneToChange].SetImage(UIImage.FromFile(fileName), UIControlState.Focused);
            buttonArray[sceneToChange].SetImage(UIImage.FromFile(fileName), UIControlState.Highlighted);
        }

        public void UpdateAllSceneGraphics()
        {
            for (int ii = 0; ii < buttonArray.Length; ii++) UpdateSceneGraphic(ii);
        }

        private void CreateSceneArray()
        {
            if (sceneArray == null) CreateIndividualScenes();
            else
            {
                Scene[] arrayHolder = sceneArray;
                CreateIndividualScenes();
                for (int i = 0; i < arrayHolder.Length; i++) if (numScenes > i) sceneArray[i] = arrayHolder[i];
            }
            CreateSceneEvents();
        }

        private void CreateIndividualScenes()
        {
            sceneArray = new Scene[numScenes];
            for (int ii = 0; ii < sceneArray.Length; ii++)
            {
                sceneArray[ii] = new Scene();
                sceneArray[ii].SetIndex(ii);
            }
        }

        private void CreateRunningSymbol(CGRect rect)
        {
            float xLocation = margin + margin * (currentRunning + 1) + buttonWidth * currentRunning;
            CGRect frameRunningSymbol = new CGRect(xLocation, rect.Top, buttonWidth, rect.Height);
            runningSymbol = new RunningSymbol(frameRunningSymbol, lineWidth);
        }

        private void UpdateRunningSymbolView()
        {
            float xLocation = margin + margin * (currentRunning + 1) + buttonWidth * currentRunning;
            CGRect frameRunningSymbol = new CGRect(xLocation, runningSymbol.Frame.Top, buttonWidth, runningSymbol.Frame.Height);
            runningSymbol.UpdateFrame(frameRunningSymbol);
        }

        void AddButtonsToView()
        {
            for (int i = 0; i < buttonArray.Length; i++) View.AddSubview(buttonArray[i]);
        }

        void RemoveButtonsFromView()
        {
            for (int i = 0; i < buttonArray.Length; i++) buttonArray[i].RemoveFromSuperview();
        }

        public event EventHandler ButtonPressed;
        void HandleButtonPress(object sender, System.EventArgs e)
        {
            stopwatch.Restart();
            /*
            int buttonPressed = 0;
            for (int i = 0; i < buttonArray.Length; i++) if (sender == buttonArray[i]) buttonPressed = i;

            ButtonPressed?.Invoke(this, e); //not currently using this

            SceneSelection(buttonPressed);
            UpdateAllSceneGraphics();
            */
        }

        void ToggleDisabled(int index)
        {
            if( sceneArray[index].IsDisabled() )
            {
                UpdateSceneGraphic(index);
                sceneArray[index].SetDisabled(false);
            }
            else
            {
                //TODO: we need to change the look of the button when it is disabled
                //buttonArray[index].Enabled = !buttonArray[index].Enabled;
                sceneArray[index].SetDisabled(true);
            }
            
        }

        void HandleButtonRelease(object sender, System.EventArgs e)
        {
            timeHeld = stopwatch.ElapsedMilliseconds;

            //Debug.Print(timeHeld.ToString());

            int buttonPressed = 0;
            for (int i = 0; i < buttonArray.Length; i++) if (sender == buttonArray[i]) buttonPressed = i;

            if (timeHeld > 200)
            {
                //buttonArray[buttonPressed].Enabled = !buttonArray[buttonPressed].Enabled;
                ToggleDisabled(buttonPressed);
            }
            else
            {
                //ButtonPressed?.Invoke(this, e); //not currently using this

                SceneSelection(buttonPressed);
                UpdateAllSceneGraphics();
            }
        }

        void HandleStepperChange(object sender, System.EventArgs e)
        {
            UIStepper myObj = (UIStepper)sender;
            numScenes = (int)myObj.Value;
            UpdateNumberOfScenes();
        }

        //This depends on numScenes being changed before being ran
        void UpdateNumberOfScenes()
        {
            CreateSceneArray();
            RemoveButtonsFromView();
            CreateButtons(buttonsRect);
            AddButtonsToView();
            UpdateRunningSymbolView();
        }

        void SceneSelection(int newScene)
        {
            selectedScene = newScene;
            for (int i = 0; i < buttonArray.Length; i++) sceneArray[i].IsSelected = i == newScene;
        }

        public void MoveToNextScene()
        {
            sceneArray[currentRunning].IsRunning = false;

            do currentRunning = currentRunning < (numScenes - 1) ? currentRunning + 1 : currentRunning = 0;
            while ( sceneArray[currentRunning].IsDisabled() );

            sceneArray[currentRunning].IsRunning = true;

            UpdateRunningSymbolView();
        }

        public delegate void MyEventHandler(object sender, string name, int index);

        public event MyEventHandler SceneChanged;
        void CreateSceneEvents()
        {
            for (int ii = 0; ii < sceneArray.Length; ii++)
            {
                sceneArray[ii].PropertyChanged += (object s, PropertyChangedEventArgs e2)
                    =>
                {
                    Scene myScene = (Scene)s;
                    SceneChanged?.Invoke(this, e2.PropertyName, myScene.GetIndex());
                };
            }
        }

        public int GetSceneSelected()
        {
            return selectedScene;
        }

        public int GetSceneRunning()
        {
            return currentRunning;
        }

        public Scene GetScene(int index)
        {
            return sceneArray[index];
        }

        string LookUpStringForGraphic(bool Opp, int Pattern, bool selected)
        {
            string myFile = "";
            switch (Pattern)
            {
                case 1:
                    myFile = selected ? "graphicP1NOn" : "graphicP1NOff";
                    break;
                case 2:
                    if (Opp) { myFile = selected ? "graphicP2ROn" : "graphicP2ROff"; }
                    else     { myFile = selected ? "graphicP2NOn" : "graphicP2NOff"; }
                    break;
                case 3:
                    if (Opp) { myFile = selected ? "graphicP3ROn" : "graphicP3ROff"; }
                    else     { myFile = selected ? "graphicP3NOn" : "graphicP3NOff"; }
                    break;
                case 4:
                    myFile = selected ? "graphicP4NOn" : "graphicP4NOff";
                    break;
                case 5:
                    if (Opp) { myFile = selected ? "graphicP5ROn" : "graphicP5ROff"; }
                    else {     myFile = selected ? "graphicP5NOn" : "graphicP5NOff"; }
                    break;
                case 6:
                    if (Opp) { myFile = selected ? "graphicP6ROn" : "graphicP6ROff"; }
                    else {     myFile = selected ? "graphicP6NOn" : "graphicP6NOff";}
                    break;
                case 7:
                    myFile = selected ? "graphicP7NOn" : "graphicP7NOff";
                    break;
                case 8:
                    myFile = selected ? "graphicP8NOn" : "graphicP8NOff";
                    break;
            }
            return myFile;
        }

        public int GetMin()
        {
            return min;
        }

        public int GetMax()
        {
            return numScenes - 1;
        }

        public int GetNumberOfScenes()
        {
            return sceneArray.Length;
        }

        private Stopwatch stopwatch;
        private double timeHeld;

        private int min;
        //private int max = 8;

        private int selectedScene;
        private float buttonWidth;
        private float margin;
        private int lineWidth;
        private int currentRunning;
        private int numScenes;
        private float borderWidth;

        private CGRect buttonsRect;

        Scene[] sceneArray;

        UIStepper stepper;
        private UIView innerRect;
        UIButton[] buttonArray;
        RunningSymbol runningSymbol;
    }
}

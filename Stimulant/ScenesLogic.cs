using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Globalization;
using UIKit;
using CoreMidi;
using CoreGraphics;
using MonoTouch.Dialog;
using System.ComponentModel;
using Foundation;

namespace Stimulant
{
    public partial class ViewController
    {

        void MoveToNextScene()
        {
            int currentSceneRunning = 0;
            //Do stuff here
            for (int ii = 0; ii < 8; ii++)
            {
                if (sceneArray[ii].IsRunning)
                {
                    currentSceneRunning = ii;
                }
            }

            sceneArray[currentSceneRunning].IsRunning = false;

            if (currentSceneRunning < myMidiModulation.MaxScene)
            {
                sceneArray[currentSceneRunning + 1].IsRunning = true;
            }
            else
            {
                sceneArray[myMidiModulation.MinScene].IsRunning = true;
            }
        }

        void UpdateSceneGraphic(UIButton myButton)
        {

            // loops through all scenes but only changes from old to new selected (does not update unchanging scenes)
            for (int ii = 0; ii < 8; ii++)
            {

                // Catches the old selection
                if (!(myButton == buttonArray[ii]))
                {
                    if (sceneArray[ii].IsSelected)
                    {
                        sceneArray[ii].IsSelected = false;
                        string fileName = LookUpStringForGraphic(sceneArray[ii].Opposite, sceneArray[ii].PatternNumber, sceneArray[ii].IsSelected);
                        buttonArray[ii].SetImage(UIImage.FromFile(fileName), UIControlState.Normal);
                        buttonArray[ii].SetImage(UIImage.FromFile(fileName), UIControlState.Focused);
                        buttonArray[ii].SetImage(UIImage.FromFile(fileName), UIControlState.Highlighted);
                    }
                }

                // Selects the new scene if not already selected
                else
                {
                    if (!(sceneArray[ii].IsSelected))
                    {
                        sceneSelected = ii; // Might be unnessesary
                        sceneArray[ii].IsSelected = true;
                        string fileName = LookUpStringForGraphic(sceneArray[ii].Opposite, sceneArray[ii].PatternNumber, sceneArray[ii].IsSelected);
                        buttonArray[ii].SetImage(UIImage.FromFile(fileName), UIControlState.Normal);
                        buttonArray[ii].SetImage(UIImage.FromFile(fileName), UIControlState.Focused);
                        buttonArray[ii].SetImage(UIImage.FromFile(fileName), UIControlState.Highlighted);
                    }
                }
            }
            // sceneSelected could be used here outside the for loop
        }

        void UpdateSceneGraphic()
        {
            for (int ii = 0; ii < 8; ii++)
            {
                string fileName = LookUpStringForGraphic(sceneArray[ii].Opposite, sceneArray[ii].PatternNumber, sceneArray[ii].IsSelected);
                buttonArray[ii].SetImage(UIImage.FromFile(fileName), UIControlState.Normal);
                buttonArray[ii].SetImage(UIImage.FromFile(fileName), UIControlState.Focused);
                buttonArray[ii].SetImage(UIImage.FromFile(fileName), UIControlState.Highlighted);
            }
        }

        void UpdateSceneRunning()
        {
            //myRunningSymbol.RemoveFromSuperview();
            int currentSceneRunning = 0;
            //Do stuff here
            for (int ii = 0; ii < 8; ii++)
            {
                if (sceneArray[ii].IsRunning)
                {
                    currentSceneRunning = ii;
                }
            }
            float xLocation = marginScenes + marginScenes * (currentSceneRunning + 1) + widthScenes * currentSceneRunning;
            frameRunningSymbol = new CGRect(xLocation, locationScenes - widthScenes, widthScenes, widthScenes);
            myRunningSymbol.UpdateFrame(frameRunningSymbol);
            //myRunningSymbol = new RunningSymbol(frameRunningSymbol, runningSymbol_lineWidth);
            //View.AddSubview(myRunningSymbol);
        }

        

        private void CombinedSceneProperty(string propertyName, int index)
        {
            switch (propertyName)
            {
                case "IsRunning":
                    {
                        if (myMidiModulation.IsSceneMode)
                        {
                            if (sceneArray[index].IsRunning)
                            {
                                InvokeOnMainThread(() =>
                                {
                                    //This needs to update myMidiModulation without effecting the display
                                    if (!myMidiModulation.IsArrangementMode && myMidiModulation.IsSceneMode)
                                    {
                                        myMidiModulation.getParameters(sceneArray[index]);

                                        //segmentedPattern.SelectedSegment = sceneArray[index].PatternNumber - 1;
                                        patternSelection.SetPattern(sceneArray[index].PatternNumber);

                                        //ReadPattern(sceneArray[index].PatternNumber - 1);
                                        ReadPattern(sceneArray[index].PatternNumber);

                                        ReadSlider(sceneArray[index].RateSliderValue);
                                        sliderRate.Value = sceneArray[index].RateSliderValue;

                                        //rangeSlider.LowerValue = myMidiModulation.Minimum;
                                        //rangeSlider.UpperValue = myMidiModulation.Maximum;
                                        rangeSelection.SetMinimum(myMidiModulation.Minimum);
                                        rangeSelection.SetMaximum(myMidiModulation.Maximum);


                                    }
                                    else
                                    {
                                        myMidiModulation.getParameters(sceneArray[index]);
                                        UpdateSceneRunning();

                                        //ReadPattern(sceneArray[index].PatternNumber - 1);
                                        myMidiModulation.PatternNumber = sceneArray[index].PatternNumber;

                                        ReadSlider(sceneArray[index].RateSliderValue);
                                    }

                                });
                            }
                        }
                        break;
                    }
                case "IsSelected":
                    {
                        if (sceneArray[index].IsSelected)
                        {
                            if (!myMidiModulation.IsArrangementMode && myMidiModulation.IsSceneMode)
                            {
                                sceneArray[index].IsRunning = true;
                            }
                            else if (myMidiModulation.IsArrangementMode) //Update display without effecting myMidiModulation
                            {

                                //Pattern

                                //segmentedPattern.SelectedSegment = sceneArray[index].PatternNumber - 1;
                                //patternSelection.SetPattern(sceneArray[index].PatternNumber);

                                switch (sceneArray[index].PatternNumber)
                                {
                                    case 1:
                                    case 4:
                                    case 7:
                                    case 8:
                                        sceneArray[index].Opposite = false;
                                        //buttonReverse.Hidden = true;
                                        buttonReverse.Enabled = false;
                                        buttonReverse.SetImage(UIImage.FromFile("graphicReverseSceneButtonOff.png"), UIControlState.Normal);
                                        break;
                                    default:
                                        buttonReverse.Hidden = false;
                                        buttonReverse.Enabled = true;
                                        break;
                                }
                                if (sceneArray[index].PatternNumber > 6)
                                {
                                    sceneArray[index].IsRestartEachNote = false;
                                    buttonLocation.Enabled = false;
                                }
                                else
                                {
                                    if (sceneArray[index].IsTriggerOnly)
                                    {
                                        buttonLocation.Enabled = true;
                                    }
                                }

                                string labelText = sceneArray[index].UpdatePattern(sceneArray[index].PatternNumber - 1);
                                patternSelection.UpdateLabelText(labelText);

                                //labelPattern.Text = labelText;


                                //Opposite
                                if (sceneArray[index].Opposite)
                                {
                                    buttonReverse.SetImage(UIImage.FromFile("graphicReverseSceneButtonOn.png"), UIControlState.Normal);
                                }
                                else
                                {
                                    buttonReverse.SetImage(UIImage.FromFile("graphicReverseSceneButtonOff.png"), UIControlState.Normal);
                                }

                                //Min&Max
                                //rangeSlider.LowerValue = sceneArray[index].Minimum;
                                //rangeSlider.UpperValue = sceneArray[index].Maximum;
                                rangeSelection.SetMinimum(sceneArray[index].Minimum);
                                rangeSelection.SetMaximum(sceneArray[index].Maximum);


                                //Trigger
                                if (sceneArray[index].IsTriggerOnly)
                                {
                                    buttonTrigger.SetImage(UIImage.FromFile("graphicTriggerButtonOn.png"), UIControlState.Normal);
                                    buttonTrigger.SetImage(UIImage.FromFile("graphicTriggerButtonOn.png"), UIControlState.Highlighted);
                                    buttonLocation.Enabled = true;
                                }
                                else
                                {
                                    buttonTrigger.SetImage(UIImage.FromFile("graphicTriggerButtonOff.png"), UIControlState.Normal);
                                    buttonTrigger.SetImage(UIImage.FromFile("graphicTriggerButtonOff.png"), UIControlState.Highlighted);
                                    buttonLocation.Enabled = false;
                                }

                                //Restart
                                if (sceneArray[index].IsRestartEachNote)
                                {
                                    buttonLocation.SetImage(UIImage.FromFile("graphicLocationButtonOn.png"), UIControlState.Normal);
                                    sliderHidden.Hidden = false;
                                    rangeSelection.SliderEnabled(false);//rangeSlider.Enabled = false;
                                    //labelRange.Text = "Starting Value: " + sceneArray[index].StartingLocation.ToString();
                                    rangeSelection.UpdateLabelText("Starting Value: " + sceneArray[index].StartingLocation.ToString());
                                    sliderHidden.Value = sceneArray[index].StartingLocation;
                                }
                                else
                                {
                                    buttonLocation.SetImage(UIImage.FromFile("graphicLocationButtonOff.png"), UIControlState.Normal);
                                    sliderHidden.Hidden = true;
                                    rangeSelection.SliderEnabled(true);// rangeSlider.Enabled = true;
                                    //labelRange.Text = "Modulation Range";
                                    rangeSelection.UpdateLabelText("Modulation Range");
                                }

                                //Rate
                                labelRate.Text = sceneArray[index].UpdateRate(myMidiModulation.ModeNumber);
                                sliderRate.Value = sceneArray[index].RateSliderValue;
                            }
                        }
                        else
                        {
                            if (!myMidiModulation.IsArrangementMode && myMidiModulation.IsSceneMode)
                            {
                                sceneArray[index].IsRunning = false;
                            }
                        }
                        break;
                    }

                case "PatternNumber":
                    {
                        if (!myMidiModulation.IsArrangementMode)
                        {
                            //ReadPattern((nint)(sceneArray[index].PatternNumber - 1));
                            ReadPattern((nint)sceneArray[index].PatternNumber);
                            UpdateSceneGraphic();


                        }
                        else
                        {
                            //segmentedPattern.SelectedSegment = sceneArray[index].PatternNumber - 1;
                            patternSelection.SetPattern(sceneArray[index].PatternNumber);

                            switch (sceneArray[index].PatternNumber)
                            {
                                case 1:
                                case 4:
                                case 7:
                                case 8:
                                    sceneArray[index].Opposite = false;
                                    //buttonReverse.Hidden = true;
                                    buttonReverse.Enabled = false;
                                    buttonReverse.SetImage(UIImage.FromFile("graphicReverseSceneButtonOff.png"), UIControlState.Normal);
                                    break;
                                default:
                                    buttonReverse.Hidden = false;
                                    buttonReverse.Enabled = true;
                                    break;
                            }


                            if (sceneArray[index].PatternNumber > 6)
                            {
                                sceneArray[index].IsRestartEachNote = false;
                                buttonLocation.Enabled = false;
                            }
                            else
                            {
                                if (sceneArray[index].IsTriggerOnly)
                                {
                                    buttonLocation.Enabled = true;
                                }
                            }

                            string labelText = sceneArray[index].UpdatePattern(sceneArray[index].PatternNumber - 1);
                            //labelPattern.Text = labelText;
                            patternSelection.UpdateLabelText(labelText);

                            UpdateSceneGraphic();

                            if (sceneArray[index].IsRunning) //catches if you are selecting the current running and pushes value through
                            {
                                myMidiModulation.PatternNumber = sceneArray[index].PatternNumber;
                            }
                        }
                        break;
                    }
                case "Opposite":
                    {
                        if (!myMidiModulation.IsArrangementMode)
                        {
                            myMidiModulation.Opposite = sceneArray[index].Opposite;
                        }
                        else
                        {
                            if (sceneArray[index].Opposite)
                            {
                                buttonReverse.SetImage(UIImage.FromFile("graphicReverseSceneButtonOn.png"), UIControlState.Normal);
                            }
                            else
                            {
                                buttonReverse.SetImage(UIImage.FromFile("graphicReverseSceneButtonOff.png"), UIControlState.Normal);
                            }
                            if (sceneArray[index].IsRunning) //catches if you are selecting the current running and pushes value through
                            {
                                myMidiModulation.Opposite = sceneArray[index].Opposite;
                            }
                        }
                        UpdateSceneGraphic();
                        break;
                    }
                case "RateSliderValue":
                    {
                        if (!myMidiModulation.IsArrangementMode)
                        {
                            ReadSlider(sceneArray[index].RateSliderValue);
                        }
                        else
                        {

                            labelRate.Text = sceneArray[index].UpdateRate(myMidiModulation.ModeNumber);

                            if (sceneArray[index].IsRunning) //catches if you are selecting the current running and pushes value through
                            {
                                ReadSlider(sceneArray[index].RateSliderValue);
                            }
                        }
                        break;
                    }
                case "IsTriggerOnly":
                    {
                        if (!myMidiModulation.IsArrangementMode)
                        {
                            myMidiModulation.IsTriggerOnly = sceneArray[index].IsTriggerOnly;
                        }
                        else
                        {
                            if (sceneArray[index].IsTriggerOnly)
                            {
                                buttonTrigger.SetImage(UIImage.FromFile("graphicTriggerButtonOn.png"), UIControlState.Normal);
                                buttonTrigger.SetImage(UIImage.FromFile("graphicTriggerButtonOn.png"), UIControlState.Highlighted);
                                buttonLocation.Enabled = true;
                            }
                            else
                            {
                                buttonTrigger.SetImage(UIImage.FromFile("graphicTriggerButtonOff.png"), UIControlState.Normal);
                                buttonTrigger.SetImage(UIImage.FromFile("graphicTriggerButtonOff.png"), UIControlState.Highlighted);
                                buttonLocation.Enabled = false;
                            }

                            if (sceneArray[index].IsRunning) //catches if you are selecting the current running and pushes value through
                            {
                                myMidiModulation.IsTriggerOnly = sceneArray[index].IsTriggerOnly;
                            }
                        }
                        break;
                    }
                case "IsRestartEachNote":
                    {
                        if (!myMidiModulation.IsArrangementMode)
                        {
                            myMidiModulation.IsRestartEachNote = sceneArray[index].IsRestartEachNote;
                        }
                        else
                        {
                            if (sceneArray[index].IsRestartEachNote)
                            {
                                buttonLocation.SetImage(UIImage.FromFile("graphicLocationButtonOn.png"), UIControlState.Normal);
                                sliderHidden.Hidden = false;
                                rangeSelection.SliderEnabled(false);// rangeSlider.Enabled = false;
                                //labelRange.Text = "Starting Value: " + sceneArray[index].StartingLocation.ToString();
                                rangeSelection.UpdateLabelText("Starting Value: " + sceneArray[index].StartingLocation.ToString());
                                sliderHidden.Value = sceneArray[index].StartingLocation;
                            }
                            else
                            {

                                buttonLocation.SetImage(UIImage.FromFile("graphicLocationButtonOff.png"), UIControlState.Normal);
                                sliderHidden.Hidden = true;
                                rangeSelection.SliderEnabled(true); //rangeSlider.Enabled = true;
                                //labelRange.Text = "Modulation Range";
                                rangeSelection.UpdateLabelText("Modulation Range");
                            }

                            if (sceneArray[index].IsRunning) //catches if you are selecting the current running and pushes value through
                            {
                                myMidiModulation.IsRestartEachNote = sceneArray[index].IsRestartEachNote;
                            }
                        }
                        break;
                    }
                case "StartingLocation":
                    {
                        if (!myMidiModulation.IsArrangementMode)
                        {
                            myMidiModulation.StartingLocation = sceneArray[index].StartingLocation;
                        }
                        else
                        {
                            //labelRange.Text = "Starting Value: " + sceneArray[index].StartingLocation.ToString();
                            rangeSelection.UpdateLabelText("Starting Value: " + sceneArray[index].StartingLocation.ToString());

                            if (sceneArray[index].IsRunning) //catches if you are selecting the current running and pushes value through
                            {
                                myMidiModulation.StartingLocation = sceneArray[index].StartingLocation;
                            }
                        }
                        break;
                    }
                case "Maximum":
                    {
                        if (!myMidiModulation.IsArrangementMode)
                        {
                            myMidiModulation.Maximum = sceneArray[index].Maximum;
                        }
                        else
                        {
                            if (sceneArray[index].IsRunning) //catches if you are selecting the current running and pushes value through
                            {
                                myMidiModulation.Maximum = sceneArray[index].Maximum;
                            }
                        }
                        break;
                    }
                case "Minimum":
                    {
                        if (!myMidiModulation.IsArrangementMode)
                        {
                            myMidiModulation.Minimum = sceneArray[index].Minimum;
                        }
                        else
                        {
                            if (sceneArray[index].IsRunning) //catches if you are selecting the current running and pushes value through
                            {
                                myMidiModulation.Minimum = sceneArray[index].Minimum;
                            }
                        }
                        break;
                    }
            }
        }

        string LookUpStringForGraphic(bool Opp, int Pattern, bool selected)
        {
            string myFile = "";
            switch (Pattern)
            {
                case 1:
                    if (selected)
                    {
                        myFile = "graphicP1NOn";
                    }
                    else
                    {
                        myFile = "graphicP1NOff";
                    }
                    break;
                case 2:
                    if (Opp)
                    {
                        if (selected)
                        {
                            myFile = "graphicP2ROn";
                        }
                        else
                        {
                            myFile = "graphicP2ROff";
                        }
                    }
                    else
                    {
                        if (selected)
                        {
                            myFile = "graphicP2NOn";
                        }
                        else
                        {
                            myFile = "graphicP2NOff";
                        }
                    }
                    break;
                case 3:
                    if (Opp)
                    {
                        if (selected)
                        {
                            myFile = "graphicP3ROn";
                        }
                        else
                        {
                            myFile = "graphicP3ROff";
                        }
                    }
                    else
                    {
                        if (selected)
                        {
                            myFile = "graphicP3NOn";
                        }
                        else
                        {
                            myFile = "graphicP3NOff";
                        }
                    }
                    break;
                case 4:
                    if (selected)
                    {
                        myFile = "graphicP4NOn";
                    }
                    else
                    {
                        myFile = "graphicP4NOff";
                    }
                    break;
                case 5:
                    if (Opp)
                    {
                        if (selected)
                        {
                            myFile = "graphicP5ROn";
                        }
                        else
                        {
                            myFile = "graphicP5ROff";
                        }
                    }
                    else
                    {
                        if (selected)
                        {
                            myFile = "graphicP5NOn";
                        }
                        else
                        {
                            myFile = "graphicP5NOff";
                        }
                    }
                    break;
                case 6:
                    if (Opp)
                    {
                        if (selected)
                        {
                            myFile = "graphicP6ROn";
                        }
                        else
                        {
                            myFile = "graphicP6ROff";
                        }
                    }
                    else
                    {
                        if (selected)
                        {
                            myFile = "graphicP6NOn";
                        }
                        else
                        {
                            myFile = "graphicP6NOff";
                        }
                    }
                    break;
                case 7:
                    if (selected)
                    {
                        myFile = "graphicP7NOn";
                    }
                    else
                    {
                        myFile = "graphicP7NOff";
                    }
                    break;
                case 8:
                    if (selected)
                    {
                        myFile = "graphicP8NOn";
                    }
                    else
                    {
                        myFile = "graphicP8NOff";
                    }
                    break;
            }
            return myFile;
        }




    }
}

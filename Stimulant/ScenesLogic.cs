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

        //Determines which scene .IsRunning and assigns that to currentSceneRunning
        //Makes the current scene stop running and makes next scene .IsRunning
        /*
        void MoveToNextScene()
        {
            int currentSceneRunning = 0;
            //Do stuff here
            for (int ii = 0; ii < 8; ii++)
            {
                //if (sceneArray[ii].IsRunning)
                if (sceneDisplay.GetScene(ii).IsRunning)
                {
                    currentSceneRunning = ii;
                }
            }

            //sceneArray[currentSceneRunning].IsRunning = false;
            sceneDisplay.GetScene(currentSceneRunning).IsRunning = false;

            if (currentSceneRunning < myMidiModulation.MaxScene)
            {
                //sceneArray[currentSceneRunning + 1].IsRunning = true;
                sceneDisplay.GetScene(currentSceneRunning + 1).IsRunning = true;
            }
            else
            {
                //sceneArray[myMidiModulation.MinScene].IsRunning = true;
                sceneDisplay.GetScene(myMidiModulation.MinScene).IsRunning = true;
            }
        }
        */
        

        //This also selects the scene
        /*
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
        }
        */

        /*
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
        */

        //Determines which scene .IsRunning
        //Uses that to update the running symbol
        /*
        void UpdateSceneRunning()
        {
            //myRunningSymbol.RemoveFromSuperview();
            int currentSceneRunning = 0;
            //Do stuff here
            for (int ii = 0; ii < 8; ii++)
            {
                //if (sceneArray[ii].IsRunning)
                if (sceneDisplay.GetScene(ii).IsRunning)
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
        */

        

        private void CombinedSceneProperty(string propertyName, int index)
        {
            /*
            if(index > 0)
            {
                string hello;
            }
            */
            switch (propertyName)
            {
                case "IsRunning":
                    {
                        if (myMidiModulation.IsSceneMode)
                        {
                            if (sceneDisplay.GetScene(index).IsRunning)
                            {
                                InvokeOnMainThread(() =>
                                {
                                    //This needs to update myMidiModulation without effecting the display
                                    if (!myMidiModulation.IsArrangementMode && myMidiModulation.IsSceneMode)
                                    {
                                        myMidiModulation.getParameters(sceneDisplay.GetScene(index));

                                        //segmentedPattern.SelectedSegment = sceneDisplay.GetScene(index).PatternNumber - 1;
                                        patternSelection.SetPattern(sceneDisplay.GetScene(index).PatternNumber);

                                        //ReadPattern(sceneDisplay.GetScene(index).PatternNumber - 1);
                                        ReadPattern(sceneDisplay.GetScene(index).PatternNumber);
                                        //ReadPattern(myMidiModulation.PatternNumber);

                                        ReadSlider(sceneDisplay.GetScene(index).RateSliderValue);
                                        sliderRate.Value = sceneDisplay.GetScene(index).RateSliderValue;

                                        //rangeSlider.LowerValue = myMidiModulation.Minimum;
                                        //rangeSlider.UpperValue = myMidiModulation.Maximum;
                                        rangeSelection.SetMinimum(myMidiModulation.Minimum);
                                        rangeSelection.SetMaximum(myMidiModulation.Maximum);
                                    }
                                    else
                                    {
                                        myMidiModulation.getParameters(sceneDisplay.GetScene(index));
                                        //UpdateSceneRunning();

                                        //ReadPattern(sceneDisplay.GetScene(index).PatternNumber - 1);
                                        myMidiModulation.PatternNumber = sceneDisplay.GetScene(index).PatternNumber;

                                        ReadSlider(sceneDisplay.GetScene(index).RateSliderValue);
                                    }

                                });
                            }
                        }
                        break;
                    }
                case "IsSelected":
                    {
                        if (sceneDisplay.GetScene(index).IsSelected)
                        {
                            if (!myMidiModulation.IsArrangementMode && myMidiModulation.IsSceneMode)
                            {
                                sceneDisplay.GetScene(index).IsRunning = true;
                            }
                            else if (myMidiModulation.IsArrangementMode) //Update display without effecting myMidiModulation
                            {

                                //Pattern

                                //segmentedPattern.SelectedSegment = sceneDisplay.GetScene(index).PatternNumber - 1;
                                //patternSelection.SetPattern(sceneDisplay.GetScene(index).PatternNumber);

                                switch (sceneDisplay.GetScene(index).PatternNumber)
                                {
                                    case 1:
                                    case 4:
                                    case 7:
                                    case 8:
                                        sceneDisplay.GetScene(index).Opposite = false;
                                        //buttonReverse.Hidden = true;
                                        buttonReverse.Enabled = false;
                                        buttonReverse.SetImage(UIImage.FromFile("graphicReverseSceneButtonOff.png"), UIControlState.Normal);
                                        break;
                                    default:
                                        buttonReverse.Hidden = false;
                                        buttonReverse.Enabled = true;
                                        break;
                                }
                                if (sceneDisplay.GetScene(index).PatternNumber > 6)
                                {
                                    sceneDisplay.GetScene(index).IsRestartEachNote = false;
                                    buttonLocation.Enabled = false;
                                }
                                else
                                {
                                    if (sceneDisplay.GetScene(index).IsTriggerOnly)
                                    {
                                        buttonLocation.Enabled = true;
                                    }
                                }

                                string labelText = sceneDisplay.GetScene(index).UpdatePattern(sceneDisplay.GetScene(index).PatternNumber - 1);
                                patternSelection.UpdateLabelText(labelText);

                                //labelPattern.Text = labelText;


                                //Opposite
                                if (sceneDisplay.GetScene(index).Opposite)
                                {
                                    buttonReverse.SetImage(UIImage.FromFile("graphicReverseSceneButtonOn.png"), UIControlState.Normal);
                                }
                                else
                                {
                                    buttonReverse.SetImage(UIImage.FromFile("graphicReverseSceneButtonOff.png"), UIControlState.Normal);
                                }

                                //Min&Max
                                //rangeSlider.LowerValue = sceneDisplay.GetScene(index).Minimum;
                                //rangeSlider.UpperValue = sceneDisplay.GetScene(index).Maximum;
                                rangeSelection.SetMinimum(sceneDisplay.GetScene(index).Minimum);
                                rangeSelection.SetMaximum(sceneDisplay.GetScene(index).Maximum);


                                //Trigger
                                if (sceneDisplay.GetScene(index).IsTriggerOnly)
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
                                if (sceneDisplay.GetScene(index).IsRestartEachNote)
                                {
                                    buttonLocation.SetImage(UIImage.FromFile("graphicLocationButtonOn.png"), UIControlState.Normal);

                                    //sliderHidden.Hidden = false;
                                    //rangeSelection.SliderEnabled(false);//rangeSlider.Enabled = false;
                                    rangeSelection.LocationSelection(true);

                                    //labelRange.Text = "Starting Value: " + sceneDisplay.GetScene(index).StartingLocation.ToString();
                                    rangeSelection.UpdateLabelText("Starting Value: " + sceneDisplay.GetScene(index).StartingLocation.ToString());

                                    //sliderHidden.Value = sceneDisplay.GetScene(index).StartingLocation;
                                    rangeSelection.SetStartingLocation(sceneDisplay.GetScene(index).StartingLocation);
                                }
                                else
                                {
                                    buttonLocation.SetImage(UIImage.FromFile("graphicLocationButtonOff.png"), UIControlState.Normal);

                                    //sliderHidden.Hidden = true;
                                    //rangeSelection.SliderEnabled(true);// rangeSlider.Enabled = true;

                                    rangeSelection.LocationSelection(false);

                                    //labelRange.Text = "Modulation Range";
                                    rangeSelection.UpdateLabelText("Modulation Range");
                                }

                                //Rate
                                labelRate.Text = sceneDisplay.GetScene(index).UpdateRate(myMidiModulation.ModeNumber);
                                sliderRate.Value = sceneDisplay.GetScene(index).RateSliderValue;
                            }
                        }
                        else
                        {
                            if (!myMidiModulation.IsArrangementMode && myMidiModulation.IsSceneMode)
                            {
                                sceneDisplay.GetScene(index).IsRunning = false;
                            }
                        }
                        break;
                    }

                case "PatternNumber":
                    {
                        if (!myMidiModulation.IsArrangementMode)
                        {
                            //ReadPattern((nint)(sceneDisplay.GetScene(index).PatternNumber - 1));
                            ReadPattern((nint)sceneDisplay.GetScene(index).PatternNumber);
                            sceneDisplay.UpdateAllSceneGraphics(); // UpdateSceneGraphic();


                        }
                        else
                        {
                            //segmentedPattern.SelectedSegment = sceneDisplay.GetScene(index).PatternNumber - 1;
                            patternSelection.SetPattern(sceneDisplay.GetScene(index).PatternNumber);

                            switch (sceneDisplay.GetScene(index).PatternNumber)
                            {
                                case 1:
                                case 4:
                                case 7:
                                case 8:
                                    sceneDisplay.GetScene(index).Opposite = false;
                                    //buttonReverse.Hidden = true;
                                    buttonReverse.Enabled = false;
                                    buttonReverse.SetImage(UIImage.FromFile("graphicReverseSceneButtonOff.png"), UIControlState.Normal);
                                    break;
                                default:
                                    buttonReverse.Hidden = false;
                                    buttonReverse.Enabled = true;
                                    break;
                            }


                            if (sceneDisplay.GetScene(index).PatternNumber > 6)
                            {
                                sceneDisplay.GetScene(index).IsRestartEachNote = false;
                                buttonLocation.Enabled = false;
                            }
                            else
                            {
                                if (sceneDisplay.GetScene(index).IsTriggerOnly)
                                {
                                    buttonLocation.Enabled = true;
                                }
                            }

                            string labelText = sceneDisplay.GetScene(index).UpdatePattern(sceneDisplay.GetScene(index).PatternNumber - 1);
                            //labelPattern.Text = labelText;
                            patternSelection.UpdateLabelText(labelText);

                            sceneDisplay.UpdateAllSceneGraphics(); // UpdateSceneGraphic();

                            if (sceneDisplay.GetScene(index).IsRunning) //catches if you are selecting the current running and pushes value through
                            {
                                myMidiModulation.PatternNumber = sceneDisplay.GetScene(index).PatternNumber;
                            }
                        }
                        break;
                    }
                case "Opposite":
                    {
                        if (!myMidiModulation.IsArrangementMode)
                        {
                            myMidiModulation.Opposite = sceneDisplay.GetScene(index).Opposite;
                        }
                        else
                        {
                            if (sceneDisplay.GetScene(index).Opposite)
                            {
                                buttonReverse.SetImage(UIImage.FromFile("graphicReverseSceneButtonOn.png"), UIControlState.Normal);
                            }
                            else
                            {
                                buttonReverse.SetImage(UIImage.FromFile("graphicReverseSceneButtonOff.png"), UIControlState.Normal);
                            }
                            if (sceneDisplay.GetScene(index).IsRunning) //catches if you are selecting the current running and pushes value through
                            {
                                myMidiModulation.Opposite = sceneDisplay.GetScene(index).Opposite;
                            }
                        }
                        sceneDisplay.UpdateAllSceneGraphics(); // UpdateSceneGraphic();
                        break;
                    }
                case "RateSliderValue":
                    {
                        if (!myMidiModulation.IsArrangementMode)
                        {
                            ReadSlider(sceneDisplay.GetScene(index).RateSliderValue);
                        }
                        else
                        {

                            labelRate.Text = sceneDisplay.GetScene(index).UpdateRate(myMidiModulation.ModeNumber);

                            if (sceneDisplay.GetScene(index).IsRunning) //catches if you are selecting the current running and pushes value through
                            {
                                ReadSlider(sceneDisplay.GetScene(index).RateSliderValue);
                            }
                        }
                        break;
                    }
                case "IsTriggerOnly":
                    {
                        if (!myMidiModulation.IsArrangementMode)
                        {
                            myMidiModulation.IsTriggerOnly = sceneDisplay.GetScene(index).IsTriggerOnly;
                        }
                        else
                        {
                            if (sceneDisplay.GetScene(index).IsTriggerOnly)
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

                            if (sceneDisplay.GetScene(index).IsRunning) //catches if you are selecting the current running and pushes value through
                            {
                                myMidiModulation.IsTriggerOnly = sceneDisplay.GetScene(index).IsTriggerOnly;
                            }
                        }
                        break;
                    }
                case "IsRestartEachNote":
                    {
                        if (!myMidiModulation.IsArrangementMode)
                        {
                            myMidiModulation.IsRestartEachNote = sceneDisplay.GetScene(index).IsRestartEachNote;
                        }
                        else
                        {
                            if (sceneDisplay.GetScene(index).IsRestartEachNote)
                            {
                                buttonLocation.SetImage(UIImage.FromFile("graphicLocationButtonOn.png"), UIControlState.Normal);


                                //sliderHidden.Hidden = false;
                                //rangeSelection.SliderEnabled(false);// rangeSlider.Enabled = false;
                                rangeSelection.LocationSelection(true);

                                //labelRange.Text = "Starting Value: " + sceneDisplay.GetScene(index).StartingLocation.ToString();
                                rangeSelection.UpdateLabelText("Starting Value: " + sceneDisplay.GetScene(index).StartingLocation.ToString());

                                //sliderHidden.Value = sceneDisplay.GetScene(index).StartingLocation;
                                rangeSelection.SetStartingLocation(sceneDisplay.GetScene(index).StartingLocation);
                            }
                            else
                            {

                                buttonLocation.SetImage(UIImage.FromFile("graphicLocationButtonOff.png"), UIControlState.Normal);

                                //sliderHidden.Hidden = true;
                                //rangeSelection.SliderEnabled(true); //rangeSlider.Enabled = true;
                                rangeSelection.LocationSelection(false);


                                //labelRange.Text = "Modulation Range";
                                rangeSelection.UpdateLabelText("Modulation Range");
                            }

                            if (sceneDisplay.GetScene(index).IsRunning) //catches if you are selecting the current running and pushes value through
                            {
                                myMidiModulation.IsRestartEachNote = sceneDisplay.GetScene(index).IsRestartEachNote;
                            }
                        }
                        break;
                    }
                case "StartingLocation":
                    {
                        if (!myMidiModulation.IsArrangementMode)
                        {
                            myMidiModulation.StartingLocation = sceneDisplay.GetScene(index).StartingLocation;
                        }
                        else
                        {
                            //labelRange.Text = "Starting Value: " + sceneDisplay.GetScene(index).StartingLocation.ToString();
                            rangeSelection.UpdateLabelText("Starting Value: " + sceneDisplay.GetScene(index).StartingLocation.ToString());

                            if (sceneDisplay.GetScene(index).IsRunning) //catches if you are selecting the current running and pushes value through
                            {
                                myMidiModulation.StartingLocation = sceneDisplay.GetScene(index).StartingLocation;
                            }
                        }
                        break;
                    }
                case "Maximum":
                    {
                        if (!myMidiModulation.IsArrangementMode)
                        {
                            myMidiModulation.Maximum = sceneDisplay.GetScene(index).Maximum;
                        }
                        else
                        {
                            if (sceneDisplay.GetScene(index).IsRunning) //catches if you are selecting the current running and pushes value through
                            {
                                myMidiModulation.Maximum = sceneDisplay.GetScene(index).Maximum;
                            }
                        }
                        break;
                    }
                case "Minimum":
                    {
                        if (!myMidiModulation.IsArrangementMode)
                        {
                            myMidiModulation.Minimum = sceneDisplay.GetScene(index).Minimum;
                        }
                        else
                        {
                            if (sceneDisplay.GetScene(index).IsRunning) //catches if you are selecting the current running and pushes value through
                            {
                                myMidiModulation.Minimum = sceneDisplay.GetScene(index).Minimum;
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

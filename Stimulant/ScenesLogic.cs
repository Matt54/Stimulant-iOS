using System;
using UIKit;

namespace Stimulant
{
    public partial class ViewController
    {
        private void CombinedSceneProperty(string propertyName, int index)
        {
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

                                        patternSelection.SetPattern(sceneDisplay.GetScene(index).PatternNumber);

                                        ReadPattern(sceneDisplay.GetScene(index).PatternNumber);

                                        ReadSlider(sceneDisplay.GetScene(index).RateSliderValue);
                                        sliderRate.Value = sceneDisplay.GetScene(index).RateSliderValue;

                                        rangeSelection.SetMinimum(myMidiModulation.Minimum);
                                        rangeSelection.SetMaximum(myMidiModulation.Maximum);
                                    }
                                    else
                                    {
                                        myMidiModulation.getParameters(sceneDisplay.GetScene(index));

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
                            if (!myMidiModulation.IsArrangementMode && myMidiModulation.IsSceneMode) sceneDisplay.GetScene(index).IsRunning = true;
                            else if (myMidiModulation.IsArrangementMode) //Update display without effecting myMidiModulation
                            {
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
                                else if (sceneDisplay.GetScene(index).IsTriggerOnly) buttonLocation.Enabled = true;

                                string labelText = sceneDisplay.GetScene(index).UpdatePattern(sceneDisplay.GetScene(index).PatternNumber - 1);
                                patternSelection.UpdateLabelText(labelText);

                                //Opposite
                                if (sceneDisplay.GetScene(index).Opposite) buttonReverse.SetImage(UIImage.FromFile("graphicReverseSceneButtonOn.png"), UIControlState.Normal);
                                else buttonReverse.SetImage(UIImage.FromFile("graphicReverseSceneButtonOff.png"), UIControlState.Normal);

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
                                    rangeSelection.LocationSelection(true);
                                    rangeSelection.UpdateLabelText("Starting Value: " + sceneDisplay.GetScene(index).StartingLocation.ToString());
                                    rangeSelection.SetStartingLocation(sceneDisplay.GetScene(index).StartingLocation);
                                }
                                else
                                {
                                    buttonLocation.SetImage(UIImage.FromFile("graphicLocationButtonOff.png"), UIControlState.Normal);
                                    rangeSelection.LocationSelection(false);
                                    rangeSelection.UpdateLabelText("Modulation Range");
                                }

                                //Rate
                                labelRate.Text = sceneDisplay.GetScene(index).UpdateRate(myMidiModulation.ModeNumber);
                                sliderRate.Value = sceneDisplay.GetScene(index).RateSliderValue;
                            }
                        }
                        else if (!myMidiModulation.IsArrangementMode && myMidiModulation.IsSceneMode) sceneDisplay.GetScene(index).IsRunning = false;

                        break;
                    }

                case "PatternNumber":
                    {
                        if (!myMidiModulation.IsArrangementMode)
                        {
                            ReadPattern((nint)sceneDisplay.GetScene(index).PatternNumber);
                            sceneDisplay.UpdateAllSceneGraphics();
                        }
                        else
                        {
                            patternSelection.SetPattern(sceneDisplay.GetScene(index).PatternNumber);

                            switch (sceneDisplay.GetScene(index).PatternNumber)
                            {
                                case 1:
                                case 4:
                                case 7:
                                case 8:
                                    sceneDisplay.GetScene(index).Opposite = false;
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
                            else if (sceneDisplay.GetScene(index).IsTriggerOnly) buttonLocation.Enabled = true;

                            string labelText = sceneDisplay.GetScene(index).UpdatePattern(sceneDisplay.GetScene(index).PatternNumber - 1);

                            patternSelection.UpdateLabelText(labelText);

                            sceneDisplay.UpdateAllSceneGraphics();

                            if (sceneDisplay.GetScene(index).IsRunning) myMidiModulation.PatternNumber = sceneDisplay.GetScene(index).PatternNumber;
                        }
                        break;
                    }
                case "Opposite":
                    {
                        if (!myMidiModulation.IsArrangementMode) myMidiModulation.Opposite = sceneDisplay.GetScene(index).Opposite;
                        else
                        {
                            if (sceneDisplay.GetScene(index).Opposite) buttonReverse.SetImage(UIImage.FromFile("graphicReverseSceneButtonOn.png"), UIControlState.Normal);
                            else buttonReverse.SetImage(UIImage.FromFile("graphicReverseSceneButtonOff.png"), UIControlState.Normal);

                            //catches if you are selecting the current running and pushes value through
                            if (sceneDisplay.GetScene(index).IsRunning) myMidiModulation.Opposite = sceneDisplay.GetScene(index).Opposite;
                        }
                        sceneDisplay.UpdateAllSceneGraphics();
                        break;
                    }
                case "RateSliderValue":
                    {
                        if (!myMidiModulation.IsArrangementMode) ReadSlider(sceneDisplay.GetScene(index).RateSliderValue);
                        else
                        {
                            labelRate.Text = sceneDisplay.GetScene(index).UpdateRate(myMidiModulation.ModeNumber);

                            //catches if you are selecting the current running and pushes value through
                            if (sceneDisplay.GetScene(index).IsRunning) ReadSlider(sceneDisplay.GetScene(index).RateSliderValue);
                        }
                        break;
                    }
                case "IsTriggerOnly":
                    {
                        if (!myMidiModulation.IsArrangementMode) myMidiModulation.IsTriggerOnly = sceneDisplay.GetScene(index).IsTriggerOnly;
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

                            //catches if you are selecting the current running and pushes value through
                            if (sceneDisplay.GetScene(index).IsRunning) myMidiModulation.IsTriggerOnly = sceneDisplay.GetScene(index).IsTriggerOnly;
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

                                rangeSelection.LocationSelection(true);

                                rangeSelection.UpdateLabelText("Starting Value: " + sceneDisplay.GetScene(index).StartingLocation.ToString());

                                rangeSelection.SetStartingLocation(sceneDisplay.GetScene(index).StartingLocation);
                            }
                            else
                            {
                                buttonLocation.SetImage(UIImage.FromFile("graphicLocationButtonOff.png"), UIControlState.Normal);
                                rangeSelection.LocationSelection(false);
                                rangeSelection.UpdateLabelText("Modulation Range");
                            }

                            //catches if you are selecting the current running and pushes value through
                            if (sceneDisplay.GetScene(index).IsRunning) myMidiModulation.IsRestartEachNote = sceneDisplay.GetScene(index).IsRestartEachNote;
                        }
                        break;
                    }
                case "StartingLocation":
                    {
                        if (!myMidiModulation.IsArrangementMode) myMidiModulation.StartingLocation = sceneDisplay.GetScene(index).StartingLocation;

                        else
                        {
                            rangeSelection.UpdateLabelText("Starting Value: " + sceneDisplay.GetScene(index).StartingLocation.ToString());

                            //catches if you are selecting the current running and pushes value through
                            if (sceneDisplay.GetScene(index).IsRunning) myMidiModulation.StartingLocation = sceneDisplay.GetScene(index).StartingLocation;
                        }
                        break;
                    }
                case "Maximum":
                    {
                        if (!myMidiModulation.IsArrangementMode) myMidiModulation.Maximum = sceneDisplay.GetScene(index).Maximum;

                        else
                        {
                            //catches if you are selecting the current running and pushes value through
                            if (sceneDisplay.GetScene(index).IsRunning) myMidiModulation.Maximum = sceneDisplay.GetScene(index).Maximum;
                        }
                        break;
                    }
                case "Minimum":
                    {
                        if (!myMidiModulation.IsArrangementMode) myMidiModulation.Minimum = sceneDisplay.GetScene(index).Minimum;
                        else
                        {
                            //catches if you are selecting the current running and pushes value through
                            if (sceneDisplay.GetScene(index).IsRunning) myMidiModulation.Minimum = sceneDisplay.GetScene(index).Minimum;
                        }
                        break;
                    }
            }
        }
    }
}

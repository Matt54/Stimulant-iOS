﻿using System;
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
    public partial class ViewController : UIViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {// Note: this .ctor should not contain any initialization logic.
        }

        //Declare MidiClient Object: The MidiClient class is used to communicate with the MIDI subsystem on MacOS and iOS
        //-It exposes various events and creates input and output midi ports using CreateInputPort/CreateOutputPort methods
        MidiClient client;

        //Simply, the input and output port objects created by calling CreateInputPort/CreateOutputPort methods
        MidiPort outputPort, inputPort;

        //Declare MidiModulation object: MidiModulation class stores all the current modulation parameters
        //I worry that this is a poor way of doing this. It may not be ideal from a memory standpoint
        MidiModulation myMidiModulation = new MidiModulation();

        //This timer will control how fast the time-based modulation steps
        HighResolutionTimer timerHighRes;

        HighResolutionTimer timerAuto;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            //NSNotificationCenter.DefaultCenter.AddObserver((NSString)"UIKeyboardWillShowNotification", KeyboardWillShow);

            Midi.Restart(); //This stops the MIDI subsystems and forces it to be reinitialized
            SetupMidi();
            MakeHardware();
            MakeDevices();
            //ReloadDevices();


            timerHighRes = new HighResolutionTimer(100.0f);
            // UseHighPriorityThread = true, sets the execution thread 
            // to ThreadPriority.Highest.  It doesn't provide any precision gain
            // in most of the cases and may do things worse for other threads. 
            // It is suggested to do some studies before leaving it true
            timerHighRes.UseHighPriorityThread = false;
            timerHighRes.Elapsed += (s, e) =>
            {
                InvokeOnMainThread(() => {
                    if (myMidiModulation.IsRunning)
                    {
                        myMidiModulation.FireModulation = true;
                    }
                });
            };
            timerHighRes.Start();

            timerAuto = new HighResolutionTimer(6300.0f);
            timerAuto.UseHighPriorityThread = false;
            timerAuto.Elapsed += (s, e) =>
            {
                InvokeOnMainThread(() => {
                    if (!myMidiModulation.SettingsOn)
                    {
                        labelPattern.Text = myMidiModulation.RandomRoll();
                    }
                });
            };

            //timerHighRes.Stop();    // by default Stop waits for thread.Join()
            // which, if called not from Elapsed subscribers,
            // would mean that all Elapsed subscribers
            // are finished when the Stop function exits 
            //timerHighRes.Stop(joinThread:false)   // Use if you don't care and don't want to wait


            UIHelper = false;

            LoadDisplay();
            myMidiModulation.ModeNumber = 2;
            ReadSlider(sliderRate.Value);

            myMidiModulation.PropertyChanged += (object s, PropertyChangedEventArgs e2) =>
            {
                switch (e2.PropertyName)
                {

                    case "Opposite":
                        {
                            if (myMidiModulation.Opposite)
                            {
                                buttonReverse.SetImage(UIImage.FromFile("graphicReverseButtonOn.png"), UIControlState.Normal);
                            }
                            else
                            {
                                buttonReverse.SetImage(UIImage.FromFile("graphicReverseButtonOff.png"), UIControlState.Normal);
                            }
                            break;
                        }

                    case "PatternString":
                        {
                            labelPattern.Text = myMidiModulation.PatternString;
                            break;
                        }

                    case "IsAuto":
                        {
                            if (myMidiModulation.IsAuto)
                            {
                                if (myMidiModulation.ModeNumber == 2)
                                {
                                    timerAuto.Start();
                                }
                                buttonAuto.SetImage(UIImage.FromFile("graphicAutoButtonOn.png"), UIControlState.Normal);
                                buttonSettings.Hidden = false;
                            }
                            else
                            {
                                timerAuto.Stop(joinThread: false);
                                buttonAuto.SetImage(UIImage.FromFile("graphicAutoButtonOff.png"), UIControlState.Normal);
                                buttonSettings.Hidden = true;
                                if (myMidiModulation.SettingsOn)
                                {
                                    myMidiModulation.SettingsOn = false;
                                }
                            }
                            break;
                        }

                    case "IsAR":
                        {
                            if (myMidiModulation.IsAR)
                            {
                                buttonAR.SetImage(UIImage.FromFile("graphicARButtonOn.png"), UIControlState.Normal);
                            }
                            else
                            {
                                buttonAR.SetImage(UIImage.FromFile("graphicARButtonOff.png"), UIControlState.Normal);
                            }
                            break;
                        }

                    case "CCOn":
                        {
                            if (myMidiModulation.CCOn)
                            {
                                buttonCC.SetImage(UIImage.FromFile("graphicCCButtonOn.png"), UIControlState.Normal);
                                sliderRate.Hidden = true;
                                buttonPlus1.Hidden = false;
                                buttonPlus10.Hidden = false;
                                buttonMinus1.Hidden = false;
                                buttonMinus10.Hidden = false;
                                labelRate.Text = "Current Channel: CC" + myMidiModulation.CCNumber;
                            }
                            else
                            {
                                buttonCC.SetImage(UIImage.FromFile("graphicCCButtonOff.png"), UIControlState.Normal);
                                sliderRate.Hidden = false;
                                buttonPlus1.Hidden = true;
                                buttonPlus10.Hidden = true;
                                buttonMinus1.Hidden = true;
                                buttonMinus10.Hidden = true;
                                ReadSlider(sliderRate.Value);
                            }
                            break;
                        }

                    case "CCNumber":
                        {
                            if (myMidiModulation.CCOn)
                            {
                                labelRate.Text = "Current Channel: CC" + myMidiModulation.CCNumber;
                            }
                            break;
                        }

                    case "SettingsOn":
                        {
                            if (myMidiModulation.SettingsOn)
                            {
                                buttonSettings.SetImage(UIImage.FromFile("graphicSettingsButtonOn.png"), UIControlState.Normal);
                                myMidiModulation.RateRemember = (int)sliderRate.Value;
                                sliderRate.Value = (float)myMidiModulation.AutoCutoff;
                                ReadSlider(sliderRate.Value);
                            }
                            else
                            {
                                buttonSettings.SetImage(UIImage.FromFile("graphicSettingsButtonOff.png"), UIControlState.Normal);
                                sliderRate.Value = myMidiModulation.RateRemember;
                                ReadSlider(sliderRate.Value);
                            }
                            break;
                        }

                    case "ModeNumber":
                        {
                            switch (myMidiModulation.ModeNumber)
                            {
                                case 1:
                                    timerHighRes.Stop(joinThread: false);
                                    timerAuto.Stop(joinThread: false);
                                    buttonMidi.SetImage(UIImage.FromFile("graphicMidiButtonOn.png"), UIControlState.Normal);
                                    buttonTime.SetImage(UIImage.FromFile("graphicTimeButtonOff.png"), UIControlState.Normal);
                                    ReadSlider(sliderRate.Value);
                                    break;
                                case 2:
                                    if (myMidiModulation.IsAuto)
                                    {
                                        timerAuto.Start();
                                    }
                                    timerHighRes.Start();
                                    buttonMidi.SetImage(UIImage.FromFile("graphicMidiButtonOff.png"), UIControlState.Normal);
                                    buttonTime.SetImage(UIImage.FromFile("graphicTimeButtonOn.png"), UIControlState.Normal);
                                    ReadSlider(sliderRate.Value);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        }

                    case "PatternNumber":
                        {

                            if (myMidiModulation.IsRandomRoll == true)
                            {
                                sliderRate.Value = myMidiModulation.RandomNumber(1, 127);
                                segmentedPattern.SelectedSegment = myMidiModulation.PatternNumber - 1;

                                if (myMidiModulation.IsAR)
                                {
                                    rangeSlider.UpperValue = myMidiModulation.RandomNumber(1, 127);
                                    myMidiModulation.Maximum = (int)rangeSlider.UpperValue;
                                    rangeSlider.LowerValue = myMidiModulation.RandomNumber(1, myMidiModulation.Maximum);
                                    myMidiModulation.Minimum = (int)rangeSlider.LowerValue;
                                }
                                myMidiModulation.IsRandomRoll = false;
                            }

                            if (myMidiModulation.ModeNumber == 2)
                            {
                                ReadSlider(sliderRate.Value);
                            }

                            switch (myMidiModulation.PatternNumber)
                            {
                                case 1:
                                case 4:
                                case 7:
                                case 8:
                                    myMidiModulation.Opposite = false;
                                    buttonReverse.Hidden = true;
                                    buttonReverse.SetImage(UIImage.FromFile("graphicReverseButtonOff.png"), UIControlState.Normal);
                                    break;
                                default:
                                    buttonReverse.Hidden = false;
                                    break;
                            }
                            break;
                        }

                    case "FireModulation":
                        InvokeOnMainThread(() => {
                            if (myMidiModulation.FireModulation)
                            {
                                myMidiModulation.UpdateValue();
                                SendMIDI(0xB0, (byte)myMidiModulation.CCNumber, (byte)myMidiModulation.CurrentCC);
                                myMidiModulation.ClockCount = 0;

                                var pi_mult = (myMidiModulation.CurrentCC * 2.0f / 127);

                                //Momentarily remove from superview so that it can be on top of progress bar
                                myCircularProgressBar.RemoveFromSuperview();
                                //Declare progress bar object (Instantiating my CircularProgressBar class)
                                myCircularProgressBar = new CircularProgressBar(progressSize, lineWidth, pi_mult, barColor);
                                buttonOnOff.RemoveFromSuperview();

                                //Add Views
                                View.AddSubview(myCircularProgressBar);
                                View.AddSubview(buttonOnOff);

                                myMidiModulation.FireModulation = false;
                            }
                        });
                        break;
                }
            };


        }

        protected void HandleRateSliderChange(object sender, System.EventArgs e)
        {
            var myObject = (UISlider)sender;
            ReadSlider(myObject.Value);
        }

        private void HandlePlus1TouchDown(object sender, System.EventArgs e)
        {
            myMidiModulation.UpdateCCNumber(1);
        }

        private void HandlePlus10TouchDown(object sender, System.EventArgs e)
        {
            myMidiModulation.UpdateCCNumber(10);
        }

        private void HandleMinus1TouchDown(object sender, System.EventArgs e)
        {
            myMidiModulation.UpdateCCNumber(-1);
        }

        private void HandleMinus10TouchDown(object sender, System.EventArgs e)
        {
            myMidiModulation.UpdateCCNumber(-10);
        }

        partial void RateSliderChange(UISlider sender) { }
        partial void StartButton_TouchUpInside(UIButton sender) { }

        protected void HandleTouchDown(object sender, System.EventArgs e)
        {
            PowerPushed();
        }

        protected void HandleTouchUpInside(object sender, System.EventArgs e)
        {
            FlipPower();
        }

        private void FlipPower()
        {
            myCircularProgressBar.Hidden = false;
            buttonOnOff.Frame = bigStartSize;

            //Declare color object (Instantiating the UIColor class)
            //UIColor barColor = UIColor.FromRGB(0, 255, 0);

            //Control Logic: switches between program ON and program OFF states when button is pressed
            if (UIHelper)
            {
                buttonOnOff.SetImage(UIImage.FromFile("graphicPowerButtonOff.png"), UIControlState.Normal);
                buttonOnOff.SetImage(UIImage.FromFile("graphicPowerButtonOn.png"), UIControlState.Highlighted);
                myMidiModulation.IsRunning = false;
                var pi_mult = (myMidiModulation.CurrentCC * 2.0f / 127);
                myCircularProgressBar = new CircularProgressBar(progressSize, lineWidth, pi_mult, barColor);

                UIHelper = false;
            }
            else
            {
                buttonOnOff.SetImage(UIImage.FromFile("graphicPowerButtonOn.png"), UIControlState.Normal);
                buttonOnOff.SetImage(UIImage.FromFile("graphicPowerButtonOff.png"), UIControlState.Highlighted);
                myMidiModulation.IsRunning = true;
            }

            //Add Views
            View.AddSubview(myCircularProgressBar);
            View.AddSubview(buttonOnOff);
        }

        private void PowerPushed()
        {
            if (myMidiModulation.IsRunning)
            {
                myMidiModulation.IsRunning = false;
                UIHelper = true;
                myCircularProgressBar.RemoveFromSuperview();
            }
            myCircularProgressBar.Hidden = true;
            buttonOnOff.Frame = smallStartSize;
        }

        protected void HandleReverseTouchDown(object sender, System.EventArgs e)
        {
            myMidiModulation.ReversePattern();
        }

        protected void HandleCCTouchDown(object sender, System.EventArgs e)
        {
            myMidiModulation.CCToggle();
        }

        protected void HandleSettingsTouchDown(object sender, System.EventArgs e)
        {
            myMidiModulation.SettingsToggle();
        }

        protected void HandleARTouchDown(object sender, System.EventArgs e)
        {
            myMidiModulation.ARToggle();
        }

        protected void HandleMidiTouchDown(object sender, System.EventArgs e)
        {
            myMidiModulation.ModeNumber = 1;
        }
        protected void HandleTimeTouchDown(object sender, System.EventArgs e)
        {
            myMidiModulation.ModeNumber = 2;
        }

        protected void HandleRandomTouchDown(object sender, System.EventArgs e)
        {
            labelPattern.Text = myMidiModulation.RandomRoll();
        }

        protected void HandleAutoTouchDown(object sender, System.EventArgs e)
        {
            myMidiModulation.AutoToggle();
        }

        void ReadSlider(float sliderValue)
        {
            if (myMidiModulation.ModeNumber == 2)
            {
                //TIME
                if (myMidiModulation.SettingsOn)
                {
                    timerAuto.Interval = (float)Math.Round(((128 - sliderValue) * 100), 0);
                    labelRate.Text = "Randoms in: " + timerAuto.Interval + " ms";
                }
                else
                {
                    myMidiModulation.TimeSet(sliderValue); // Determines StepSize from a sliderValue and PatternNumber
                    timerHighRes.Interval = ValueToTimeInterval(sliderValue); // Magic function to get time interval from sliderValue
                    labelRate.Text = myMidiModulation.TimeIntervalToFrequency(timerHighRes.Interval); // Convert time interval to frequency for label display
                }
            }
            else
            {
                //MIDI
                string displayText = "";
                // Conditionals determine the correct rate based on sliderValue
                if (sliderValue >= (128 * 15 / 16))
                { // 32 note triples
                    myMidiModulation.ClockCutoff = 1;
                    displayText = "1/32T";
                    myMidiModulation.RateCatch = 16;
                }
                else if (sliderValue >= (128 * 14 / 16))
                { // 32 notes
                    myMidiModulation.ClockCutoff = 1;
                    displayText = "1/32";
                    myMidiModulation.RateCatch = 15;
                }
                else if (sliderValue >= (128 * 13 / 16))
                {  // sixteenth note triples
                    myMidiModulation.ClockCutoff = 1;
                    displayText = "1/16T";
                    myMidiModulation.RateCatch = 14;
                }
                else if (sliderValue >= (128 * 12 / 16))
                { // sixteenth notes
                    myMidiModulation.ClockCutoff = 1;
                    displayText = "1/16";
                    myMidiModulation.RateCatch = 13;
                }
                else if (sliderValue >= (128 * 11 / 16))
                {  // eighth note triples
                    myMidiModulation.ClockCutoff = 1;
                    displayText = "1/8T";
                    myMidiModulation.RateCatch = 12;
                }
                else if (sliderValue >= (128 * 10 / 16))
                {  // eighth notes
                    myMidiModulation.ClockCutoff = 1;
                    displayText = "1/8";
                    myMidiModulation.RateCatch = 11;
                }
                else if (sliderValue >= (128 * 9 / 16))
                {  // quarter note triples
                    myMidiModulation.ClockCutoff = 1;
                    displayText = "1/4T";
                    myMidiModulation.RateCatch = 10;
                }
                else if (sliderValue >= (128 * 8 / 16))
                {  // quarter notes
                    myMidiModulation.ClockCutoff = 1;
                    displayText = "1/4";
                    myMidiModulation.RateCatch = 9;
                }
                else if (sliderValue >= (128 * 7 / 16))
                {  // half note triples
                    myMidiModulation.ClockCutoff = 1;
                    displayText = "1/2T";
                    myMidiModulation.RateCatch = 8;
                }
                else if (sliderValue >= (128 * 6 / 16))
                {  // half note
                    myMidiModulation.ClockCutoff = 1;
                    displayText = "1/2";
                    myMidiModulation.RateCatch = 7;
                }
                else if (sliderValue >= (128 * 5 / 16))
                { // whole note triples
                    myMidiModulation.ClockCutoff = 1;
                    displayText = "1/1T";
                    myMidiModulation.RateCatch = 6;
                }
                else if (sliderValue >= (128 * 4 / 16))
                { // whole note
                    myMidiModulation.ClockCutoff = 1;
                    displayText = "1/1";
                    myMidiModulation.RateCatch = 5;
                }
                else if (sliderValue >= (128 * 3 / 16))
                { // 2 bar triples
                    myMidiModulation.ClockCutoff = 1;
                    displayText = "2/1T";
                    myMidiModulation.RateCatch = 4;
                }
                else if (sliderValue >= (128 * 2 / 16))
                { // 2 bars
                    myMidiModulation.ClockCutoff = 2;
                    displayText = "2/1";
                    myMidiModulation.RateCatch = 3;
                }
                else if (sliderValue >= (128 * 1 / 16))
                { // 4 bar triples
                    myMidiModulation.ClockCutoff = 2;
                    displayText = "4/1T";
                    myMidiModulation.RateCatch = 2;
                }
                else if (sliderValue < 8)
                { // 4 bar
                    myMidiModulation.ClockCutoff = 4;
                    displayText = "4/1";
                    myMidiModulation.RateCatch = 1;
                }

                if (myMidiModulation.SettingsOn)
                {
                    myMidiModulation.AutoCutoff = (17 - myMidiModulation.RateCatch) * 24 * 4;
                    labelRate.Text = "Randoms in: " + (17 - myMidiModulation.RateCatch) + " Beats";
                }
                else
                {
                    myMidiModulation.StepSizeSetter(); //Do we need this here? I think it gets set every time a clock comes in..
                    labelRate.Text = "EXT Clock Sync: " + displayText;
                }

            }
        }

        public float ValueToTimeInterval(float value)
        {
            float timeInterval;

            switch (myMidiModulation.PatternNumber)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    timeInterval = (float)((double)((((double)myMidiModulation.StepSize) / 2) * (Math.Round(-15.6631 + (1561.999 + 17.55931) / (1 + Math.Pow((value / 13.37739), 2.002958)), 3))));
                    break;
                case 7:
                case 8:
                    timeInterval = (float)((double)((((double)myMidiModulation.StepSize) / 2) * (Math.Round(-3933.384 + (5000.008 + 3933.384) / (1 + Math.Pow((value / 56.13086), 0.2707651)), 3))));
                    break;
                default:
                    timeInterval = (float)((double)((((double)myMidiModulation.StepSize) / 2) * (Math.Round(-15.6631 + (1561.999 + 17.55931) / (1 + Math.Pow((value / 13.37739), 2.002958)), 3))));
                    break;
            }
            return timeInterval;
        }

        partial void ModeNumChanged(UISegmentedControl sender) { }

        protected void PnumChange(object sender, System.EventArgs e)
        {
            var seg = sender as UISegmentedControl;
            var index = seg.SelectedSegment;
            string labelText = myMidiModulation.UpdatePattern(index);
            labelPattern.Text = labelText;
        }

        private void ProgramChange()
        {
            var index = segmentedPattern.SelectedSegment;
            string labelText = myMidiModulation.UpdatePattern(index);
            labelPattern.Text = labelText;
        }


        partial void pnumChange(UISegmentedControl sender) { }

        void SendMIDI(byte type, byte channel, byte value)
        {

            for (int i = 0; i < Midi.DestinationCount; i++)
            {


                var endpoint = MidiEndpoint.GetDestination(i);
                outputPort.Send(endpoint, new MidiPacket[] { new MidiPacket(0, new byte[] { type, channel, value }) });


                //outputPort.Send(endpoint, new MidiPacket[] { new MidiPacket(0, new byte[] { 0xB0, (byte)(myMidiModulation.CCNumber), ccByte }) });

                //var ccVal = (byte)(rand.Next () % 127);
                // play ccVal then turn off after 300 miliseconds
                /*
                outputPort.Send (endpoint, new MidiPacket [] { new MidiPacket (0, new byte [] { 0x90, ccVal, 127 }) });
                Thread.Sleep (300);
                outputPort.Send (endpoint, new MidiPacket [] { new MidiPacket (0, new byte [] { 0x80, ccVal, 0 }) });
                */
            }
        }

        // I don't think this is required
        RootElement MakeHardware()
        {
            int sources = (int)Midi.SourceCount;
            int destinations = (int)Midi.DestinationCount;

            var sourcesSection = new Section("Sources");
            sourcesSection.AddAll(
                from x in Enumerable.Range(0, sources)
                let source = MidiEndpoint.GetSource(x)
                select (Element)new StringElement(source.DisplayName, source.IsNetworkSession ? "Network" : "Local")
            );
            var targetsSection = new Section("Targets");
            targetsSection.AddAll(
                from x in Enumerable.Range(0, destinations)
                let target = MidiEndpoint.GetDestination(x)
                select (Element)new StringElement(target.DisplayName, target.IsNetworkSession ? "Network" : "Local")
            );
            return new RootElement("Endpoints (" + sources + ", " + destinations + ")") {
                sourcesSection,
                targetsSection
            };
        }

        // I don't think this is required
        RootElement MakeDevices()
        {
            var internalDevices = new Section("Internal Devices");
            internalDevices.AddAll(
                from x in Enumerable.Range(0, (int)Midi.DeviceCount)
                let dev = Midi.GetDevice(x)
                where dev.EntityCount > 0
                select MakeDevice(dev)
            );
            var externalDevices = new Section("External Devices");
            externalDevices.AddAll(
                from x in Enumerable.Range(0, (int)Midi.ExternalDeviceCount)
                let dev = Midi.GetExternalDevice(x)
                where dev.EntityCount > 0
                select (Element)MakeDevice(dev)
            );
            return new RootElement("Devices (" + Midi.DeviceCount + ", " + Midi.ExternalDeviceCount + ")") {
                internalDevices,
                externalDevices
            };
        }

        //I don't think this is required
        Element MakeDevice(MidiDevice dev)
        {
            var entities = new Section("Entities");
            foreach (var ex in Enumerable.Range(0, (int)dev.EntityCount))
            {
                var entity = dev.GetEntity(ex);
                var sourceSection = new Section("Sources");
                sourceSection.AddAll(
                    from sx in Enumerable.Range(0, (int)entity.Sources)
                    let endpoint = entity.GetSource(sx)
                    select MakeEndpoint(endpoint)
                );
                var destinationSection = new Section("Destinations");
                destinationSection.AddAll(
                    from sx in Enumerable.Range(0, (int)entity.Destinations)
                    let endpoint = entity.GetDestination(sx)
                    select MakeEndpoint(endpoint)
                );
                entities.Add(new RootElement(entity.Name) {
                    sourceSection,
                    destinationSection
                });
            }

            return new RootElement(String.Format("{2} {0} {1}", dev.Manufacturer, dev.Model, dev.EntityCount)) {
                entities
            };
        }


        //I don't think this is required
        Element MakeEndpoint(MidiEndpoint endpoint)
        {
            Section s;
            var root = new RootElement(endpoint.Name) {
                (s = new Section ("Properties") {
                    new StringElement ("Driver Owner", endpoint.DriverOwner),
                    new StringElement ("Manufacturer", endpoint.Manufacturer),
                    new StringElement ("MaxSysExSpeed", endpoint.MaxSysExSpeed.ToString ()),
                    new StringElement ("Network Session", endpoint.IsNetworkSession ? "yes" : "no")
                })
            };
            try
            {
                s.Add(new StringElement("Offline", endpoint.Offline ? "yes" : "no"));
            }
            catch
            {
            }
            try
            {
                s.Add(new StringElement("Receive Channels", endpoint.ReceiveChannels.ToString()));
            }
            catch
            {
            }
            try
            {
                s.Add(new StringElement("Transmit Channels", endpoint.TransmitChannels.ToString()));
            }
            catch
            {
            }
            return root;
        }

        void SetupMidi()
        {
            client = new MidiClient("Stimulant iOS MIDI Client");
            client.ObjectAdded += delegate (object sender, ObjectAddedOrRemovedEventArgs e) {
            };
            client.ObjectAdded += delegate {
            };
            client.ObjectRemoved += delegate {
            };
            client.PropertyChanged += delegate (object sender, ObjectPropertyChangedEventArgs e) {
                Console.WriteLine("Changed");
            };
            client.ThruConnectionsChanged += delegate {
                Console.WriteLine("Thru connections changed");
            };
            client.SerialPortOwnerChanged += delegate {
                Console.WriteLine("Serial port changed");
            };

            outputPort = client.CreateOutputPort("Stimulant iOS Output Port");
            inputPort = client.CreateInputPort("Stimulant iOS Input Port");

            inputPort.MessageReceived += delegate (object sender, MidiPacketsEventArgs e) {
                //Console.WriteLine("Got {0} packets", e.Packets.Length);
                //Debug.Write("Got " + Convert.ToString(e.Packets.Length) + " Packets");
                foreach (MidiPacket mPacket in e.Packets)
                {
                    if (myMidiModulation.ModeNumber == 1)
                    {
                        var midiData = new byte[mPacket.Length];
                        Marshal.Copy(mPacket.Bytes, midiData, 0, mPacket.Length);
                        //The first four bits of the status byte tell MIDI what command
                        //The last four bits of the status byte tell MIDI what channel
                        byte StatusByte = midiData[0];
                        byte typeData = (byte)((StatusByte & 0xF0) >> 4);
                        byte channelData = (byte)(StatusByte & 0x0F);

                        //We should check to see if typeData is clock/start/continue/stop/note on/note off


                        //-----------defines each midi byte---------------
                        byte midi_start = 0xfa;         // start byte
                        byte midi_stop = 0xfc;          // stop byte
                        byte midi_clock = 0xf8;         // clock byte
                        byte midi_continue = 0xfb;      // continue byte
                        byte midi_note_on = 0x90;         // note on
                        byte midi_note_off = 0x80;         // note off
                        //------------------------------------------------


                        if ((StatusByte == midi_start) || (StatusByte == midi_continue))
                        {
                            if (!myMidiModulation.IsRunning)
                            {
                                InvokeOnMainThread(() => {
                                    PowerPushed();
                                    FlipPower();
                                });
                            }
                            myMidiModulation.FireModulation = true; //I'm not sure if we should be firing one off at the start here
                        }
                        if (StatusByte == midi_clock)
                        {
                            myMidiModulation.ClockCounter();
                            if (myMidiModulation.StepComma == 2)
                            {
                                myMidiModulation.StepComma = 0;
                            }
                            else
                            {
                                myMidiModulation.StepComma = myMidiModulation.StepComma + 1;
                            }
                            myMidiModulation.StepSizeSetter();
                        }


                        if (StatusByte == midi_stop)
                        {
                            if (myMidiModulation.IsRunning)
                            {
                                InvokeOnMainThread(() => {
                                    PowerPushed();
                                    FlipPower();
                                });
                            }
                        }
                    }
                }
            };

            ConnectExistingDevices();

            var session = MidiNetworkSession.DefaultSession;
            if (session != null)
            {
                session.Enabled = true;
                session.ConnectionPolicy = MidiNetworkConnectionPolicy.Anyone;
            }
        }

        void ConnectExistingDevices()
        {
            for (int i = 0; i < Midi.SourceCount; i++)
            {
                var code = inputPort.ConnectSource(MidiEndpoint.GetSource(i));
                if (code != MidiError.Ok)
                    Console.WriteLine("Failed to connect");
            }
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}
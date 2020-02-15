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
using System.Diagnostics;

namespace Stimulant
{
    public partial class ViewController
    {

        // Declare MidiClient Object: The MidiClient class is used to communicate with the MIDI subsystem on MacOS and iOS
        // It exposes various events and creates input and output midi ports using CreateInputPort/CreateOutputPort methods
        MidiClient client;

        // Simply, the input and output port objects created by calling CreateInputPort/CreateOutputPort methods
        MidiPort outputPort, inputPort;

        void SendMIDI(byte type, byte channel, byte value)
        {

            for (int i = 0; i < Midi.DestinationCount; i++)
            {
                var endpoint = MidiEndpoint.GetDestination(i);
                outputPort.Send(endpoint, new MidiPacket[] { new MidiPacket(0, new byte[] { type, channel, value }) });
                Debug.WriteLine("Midi Value: " + value + " Sent @ " + DateTime.Now.Millisecond.ToString() + "\r\n");


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
                foreach (MidiPacket mPacket in e.Packets)
                {
                    //if (myMidiModulation.ModeNumber == 1)
                    //{
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
                                //PowerPushed();
                                //FlipPower();
                                //powerButton.SetOn();
                                myMidiModulation.IsRunning = powerButton.TogglePower(); // true;//powerButton.TogglePower();
                                Debug.WriteLine("MIDI START @ " + DateTime.Now.Millisecond.ToString());
                                if (myMidiModulation.IsRunning) myMidiModulation.CatchClock();
                                
                            });
                        }
                        //myMidiModulation.FireModulation = true; //I'm not sure if we should be firing one off at the start here
                    }

                    if (StatusByte == midi_clock)
                    {
                        if (myMidiModulation.ModeNumber == 1)
                        {
                            Debug.WriteLine("MIDI CLOCK @ " + DateTime.Now.Millisecond.ToString());
                            if (myMidiModulation.IsRunning) myMidiModulation.CatchClock();

                            

                            /*
                            myMidiModulation.ClockCounter();
                            if (myMidiModulation.StepComma > 1)
                            {
                                myMidiModulation.StepComma = 0;
                            }
                            else
                            {
                                myMidiModulation.StepComma++;
                            }
                            myMidiModulation.StepSizeSetter();
                            */
                        }

                    }

                    if (StatusByte == midi_stop)
                    {
                        if (myMidiModulation.IsRunning)
                        {
                            InvokeOnMainThread(() => {
                                //PowerPushed();
                                //FlipPower();
                                myMidiModulation.IsRunning = powerButton.TogglePower();
                            });
                            //myMidiModulation.Reset();
                        }
                    }

                    if (myMidiModulation.IsTriggerOnly)
                    {
                        if (StatusByte == midi_note_on)
                        {
                            // handle note on
                            myMidiModulation.IsNoteOn = true;
                            myMidiModulation.NumOfNotesOn += 1;

                            // Restart first if in restart mode
                            if (myMidiModulation.IsRestartEachNote)
                            {
                                //myMidiModulation.CurrentCC = myMidiModulation.StartingLocation;
                                myMidiModulation.CurrentXVal = myMidiModulation.StartingLocation;

                                if (myMidiModulation.PatternNumber == 4)
                                {
                                    myMidiModulation.EveryOther = false;
                                }
                                else if (myMidiModulation.PatternNumber == 5 || myMidiModulation.PatternNumber == 6)
                                {
                                    myMidiModulation.EveryOther = true;
                                }
                            }
                        }
                        if (StatusByte == midi_note_off)
                        {

                            if (myMidiModulation.NumOfNotesOn > 0)
                            {
                                myMidiModulation.NumOfNotesOn -= 1;
                            }
                            if (myMidiModulation.NumOfNotesOn < 1)
                            {
                                // handle note off
                                myMidiModulation.IsNoteOn = false;
                                myMidiModulation.NumOfNotesOn = 0;
                            }
                        }
                    }
                    //}
                }
            };

            ConnectExistingDevices();

            //var session = MidiNetworkSession.DefaultSession;
            
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

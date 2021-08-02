using Microsoft.AspNetCore.SignalR.Client;
using NAudio.Midi;
using SharedModels;
using System;
using System.Threading.Tasks;

namespace SignalRMidiRelay
{
    class Program
    {
        static int selectedMIDIDevice { get; set; } = -1;
        static MidiOut midiOut { get; set; }

        static async Task Main(string[] args)
        {
            if (args.Length < 2)
            {
                ConsoleError("Usage: SignalRMidiRelay.exe <URLtoHub> <MIDI Device Match String>", true);
                return;
            }

            //midi output device setup
            for (int device = 0; device < MidiOut.NumberOfDevices; device++)
            {
                ConsoleLog(MidiOut.DeviceInfo(device).ProductName, true);
                if (MidiOut.DeviceInfo(device).ProductName.ToUpper().Contains(args[1].ToUpper()))
                {
                    selectedMIDIDevice = device;
                }
            }
            if (selectedMIDIDevice < 0)
            {
                ConsoleError($"Could not find a MIDI device matching '{args[1]}'", true);
                return;
            }
            midiOut = new MidiOut(selectedMIDIDevice);
            //int channel = 1;
            //int noteNumber = int.Parse("50");
            //var noteOnEvent = new NoteOnEvent(0, channel, noteNumber, 100, 50);
            //mOut.Send(noteOnEvent.GetAsShortMessage());


            HubConnection hubConnection;

            //Create a connection for the SignalR server
            ConsoleLog($"Connecting to '{args[0]}' ...", false);
            try
            {
                hubConnection = new HubConnectionBuilder().WithUrl(args[0]).Build();

                hubConnection.On<MIDIMessage>("ReceiveMessage", (message) =>
                {
                    Console.WriteLine($"{message.Note}: {message.Velocity}");
                    int channel = 1;
                    int noteNumber = int.Parse(message.Note);
                    var noteOnEvent = new NoteOnEvent(0, channel, noteNumber, 100, 50);
                    midiOut.Send(noteOnEvent.GetAsShortMessage());
                });

                await hubConnection.StartAsync();
            }
            catch (Exception ex)
            {
                ConsoleLog("Error", true);
                ConsoleError(ex.ToString(), true);
                return;
            }

            ConsoleLog("Done", true);

            ConsoleLog("Starting wait loop...", true);

            while (true)
            {
                System.Threading.Thread.Sleep(1);
            }
        }

        private static void MidiIn_MessageReceived(object sender, MidiInMessageEventArgs e)
        {
        }

        private static void ConsoleLog(string msg, bool newLine)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            if (newLine)
            {
                Console.WriteLine(msg);
            }
            else
            {
                Console.Write(msg);
            }
            Console.ResetColor();
        }

        private static void ConsoleError(string msg, bool newLine)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if (newLine)
            {
                Console.WriteLine(msg);
            }
            else
            {
                Console.Write(msg);
            }
            Console.ResetColor();
        }


    }

}

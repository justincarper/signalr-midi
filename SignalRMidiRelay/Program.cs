using Microsoft.AspNetCore.SignalR.Client;
using NAudio.Midi;
using SharedModels;
using System;
using System.Threading;
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
            ConsoleLog("MIDI Devices...",true);
            for (int device = 0; device < MidiOut.NumberOfDevices; device++)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(MidiOut.DeviceInfo(device).ProductName);
                if (MidiOut.DeviceInfo(device).ProductName.ToUpper().Contains(args[1].ToUpper()))
                {
                    selectedMIDIDevice = device;
                }
            }
            Console.ResetColor();
            if (selectedMIDIDevice < 0)
            {
                ConsoleError($"Could not find a MIDI device matching '{args[1]}'", true);
                return;
            }
            else
            {
                ConsoleLog($"'{MidiOut.DeviceInfo(selectedMIDIDevice).ProductName}' device selected.", true);
            }


            midiOut = new MidiOut(selectedMIDIDevice);

            HubConnection hubConnection;

            //Create a connection for the SignalR server
            ConsoleLog($"Connecting to '{args[0]}' ...", false);
            try
            {
                hubConnection = new HubConnectionBuilder().WithUrl(args[0]).Build();

                hubConnection.On<MIDIMessage>("ReceiveMessage", async (message) =>
                {
                    Console.WriteLine($"{message.Note}: {message.Velocity}");
                    int channel = 1;
                    int noteNumber = int.Parse(message.Note);
                    //noteNumber += 36;
                    Thread t = new Thread(() => PlayMidiNote(midiOut, noteNumber, 90, channel));
                    t.Start();
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

        public async static void PlayMidiNote(MidiOut midiOut, int notenum, int velocity, int channel)
        {
            midiOut.Send(MidiMessage.StartNote(notenum, velocity, channel).RawData);
            await Task.Delay(250).ConfigureAwait(false);
            midiOut.Send(MidiMessage.StopNote(notenum, velocity, channel).RawData); 
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

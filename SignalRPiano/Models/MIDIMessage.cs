using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRPiano.Models
{
    public class MIDIMessage
    {
        public string Note { get; set; }
        public int Velocity { get; set; }
    }
}

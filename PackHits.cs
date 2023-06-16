using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Torn5
{
    internal class PackHits
    {
        public string name {  get; set; }
        public decimal phasor { get; set; }
        public decimal chest { get; set; }
        public decimal flShoulder { get; set; }
        public decimal frShoulder { get; set; }
        public decimal blShoulder { get; set; }
        public decimal brShoulder { get; set; }
        public decimal back { get; set; }
        public PackHits(string name)
        {
            this.name = name;
            this.back = 0;
            this.phasor = 0;
            this.chest = 0;
            this.flShoulder = 0;
            this.frShoulder = 0;
            this.blShoulder = 0;
            this.brShoulder = 0;
            this.back = 0;
        }

        public decimal TotalHits()
        {
            return phasor + chest + back + flShoulder + frShoulder + blShoulder + brShoulder;
        }
    }
}

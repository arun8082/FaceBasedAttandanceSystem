using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testdlibdotnetNuget.Models
{
    class Institute
    {
        public int instituteId { get; set; }
        public string instituteName { get; set; }
        public DateTime created { get; set; }
        public DateTime updated { get; set; }

        public override string ToString()
        {
            return instituteName;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testdlibdotnetNuget
{
    class Description
    {

        public int id { get; set; }
        public string indosNo { get; set; }
        public string imageName { get; set; }
        public List<double> imageDescription { get; set; }
        public string studentName { get; set; }

        public override bool Equals(object obj)
        {
            var description = obj as Description;
            return description != null &&
                   indosNo == description.indosNo;
        }
    }
}

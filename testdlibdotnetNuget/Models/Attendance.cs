using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testdlibdotnetNuget.Models
{
    class Attendance
    {
        private int id { get; set; }
        private int studentId { get; set; }
        private string indosNo { get; set; }
        private DateTime entry { get; set; }
        private DateTime exit { get; set; }
        private bool present { get; set; }

        public override bool Equals(object obj)
        {
            var attendance = obj as Attendance;
            return attendance != null &&
                   indosNo == attendance.indosNo;
        }
    }
}

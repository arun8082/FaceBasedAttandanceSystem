using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testdlibdotnetNuget
{
    class Student
    {
        public int StudentId { get ; set ; }
        public string IndosNo { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ContactNo { get; set; }
        public string Dob { get; set; }
        public string Role { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string Course { get; set; }
        public int Institute { get; set; }

        public override string ToString()
        {
            return StudentId + "," + IndosNo + "," + FirstName + "," + MiddleName + "," + LastName + "," + Email + "," + Password + "," + ContactNo
                + "," + Dob + "," + Role + "," + Course + "," + Institute;
        }
    }
}

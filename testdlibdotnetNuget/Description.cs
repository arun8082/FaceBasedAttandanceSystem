﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testdlibdotnetNuget
{
    class Description
    {
        
        public string studentName{get;set;}
        public double[] description{get;set;}
        public long rollNo { get; set; }
        public string image { get; set; }

        public Description(long rollNo,string studentName, string image,double[] description)
        {
            this.studentName = studentName;
            this.description = description;
            this.rollNo = rollNo;
            this.image = image;
        }
        
        public override string ToString()
        {
            return rollNo + " " + studentName + ""+image+" \n" + description;
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testdlibdotnetNuget
{
    class SeafarerApplication
    {
        public String sidno { get; set; }
        public String applicationid { get; set; }
        public String firstname { get; set; }
        public String middlename { get; set; }
        public String lastname { get; set; }
        public String gender { get; set; }
        public DateTime dob { get; set; }
        public String pob { get; set; }
        public String nationality { get; set; }
        public String identification_mark { get; set; }
        public DateTime sid_doi { get; set; }
        public DateTime sid_doe { get; set; }
        public String cdcno { get; set; }
        public String transactionid { get; set; }
        public String sid_status { get; set; }
        public Int32 countrycode { get; set; }
        public String sid_poi { get; set; }
        public String indos_no { get; set; }
        public Int32 sid_counter { get; set; }
        public byte[] barcode { get; set; }
        public String document_id { get; set; }
        public SeafarerDocument document { get; set; }
        public string emailid { get; set; }

        public override string ToString()
        {
            return "SeafarerApplication[ "+sidno + "," + applicationid + "," + firstname + "," + middlename + "," + lastname + "," + gender + "," + dob + "," + pob + "," + nationality + "," + identification_mark + "," + sid_doi + "," + sid_doe + "," + cdcno + "," + transactionid + "," + sid_status + "," + countrycode + "," + sid_poi + "," + indos_no + "," + sid_counter + "," + barcode + "," +document.ToString()+","+emailid+"]";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAP_S4_Hana_API.Models.FICO
{
    public class CustomerModel
    {
        public string code { get; set; }
        public string name { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public string telephoneNumber { get; set; }
        public string faxNumber { get; set; }
        public string address { get; set; }
    }
}
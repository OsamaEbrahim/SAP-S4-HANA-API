using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAP_S4_Hana_API.Models.MM
{
    public class MaterialModel
    {
        public string MaterialNumber { get; set; }
        public decimal StockQuantity { get; set; }
        public string MaterialDescription { get; set; }
    }
}
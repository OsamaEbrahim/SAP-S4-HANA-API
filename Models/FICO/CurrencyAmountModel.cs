using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAP_S4_Hana_API.Models.FICO
{
    public class CurrencyAmountModel
    {
        public string ItemNo { get; set; }
        public string DocumentCurrency { get; set; }
        public decimal AmountInDocumentCurrency { get; set; }
    }
}
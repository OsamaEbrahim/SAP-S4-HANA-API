using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAP_S4_Hana_API.Models.FICO
{
    public class AccountGLModel
    {
        public string ItemNo { get; set; }
        public string CompanyCode { get; set; }
        public string FiscalPeriod { get; set; }
        public DateTime PostingDate { get; set; }
        public DateTime ValueDate { get; set; }
        public string LineItemText { get; set; }
        public string GLAccount { get; set; }
        public string ProfitCenter { get; set; }
        public string HouseBankId { get; set; }
        public string HouseBankAccount { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAP_S4_Hana_API.Models.FICO
{
    public class AccountingDocumentModel
    {
        public string CompanyCode { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime PostingDate { get; set; }
        public string FiscalYear { get; set; }
        public string FiscalPeriod { get; set; }
        public string DocumentType { get; set; }
        public string ReferenceDocumentNo { get; set; }
        public string DocumentHeaderText { get; set; }
        public string Username { get; set; }
        public List<AccountGLModel> CreditEntries { get; set; }
        public List<AccountGLModel> DebitEntries { get; set; }
        public List<CurrencyAmountModel> CurrencyAmounts { get; set; }
    }
}
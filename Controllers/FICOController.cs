using SAP.Middleware.Connector;
using SAP_S4_Hana_API.Models.FICO;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace SAP_S4_Hana_API.Controllers
{
    [RoutePrefix("api/FICO")]
    public class FICOController : ApiController
    {
        [HttpGet]
        [Route("Customers")]
        public IHttpActionResult GetCustomers()
        {
            try
            {
                // establish connection
                var sapConnection = SapConnectionManager.Connection;

                List<CustomerModel> customers = new List<CustomerModel>();
                IRfcFunction getCustomerList = sapConnection.Repository.CreateFunction("BAPI_CUSTOMER_GETLIST");
                IRfcTable customerList = getCustomerList.GetTable("ADDRESSDATA");

                IRfcTable idRange = getCustomerList.GetTable("IDRANGE");
                idRange.Append();
                idRange.SetValue("SIGN", "I");
                idRange.SetValue("OPTION", "BT");
                idRange.SetValue("LOW", "");
                idRange.SetValue("HIGH", "");

                getCustomerList.Invoke(sapConnection);
                // Check the return type of the BAPIRETURN structure
                var bapiReturn = getCustomerList.GetStructure("RETURN");
                var returnType = bapiReturn.GetString("TYPE");

                if (returnType == "E")
                {
                    var errorMessage = bapiReturn.GetString("MESSAGE");
                    return BadRequest(errorMessage);
                }

                foreach (IRfcStructure c in customerList)
                {
                    var customer = new CustomerModel
                    {
                        code = c.GetString("CUSTOMER"),
                        name = c.GetString("NAME"),
                        country = c.GetString("COUNTRY"),
                        city = c.GetString("CITY"),
                        telephoneNumber = c.GetString("TEL1_NUMBR"),
                        faxNumber = c.GetString("FAX_NUMBER"),
                        address = c.GetString("ADDRESS"),
                    };
                    customers.Add(customer);
                }

                IRfcStructure returnTable = getCustomerList.GetStructure("RETURN");

                return Ok(customers);
            }
            catch (Exception ex)
            {
                // Handle any exceptions, log, and return an error response
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("ProfitCenters")]
        public IHttpActionResult GetProfitCenters()
        {
            try
            {
                // establish connection
                var sapConnection = SapConnectionManager.Connection;

                IRfcFunction bapiProfitCenterGetList = sapConnection.Repository.CreateFunction("BAPI_PROFITCENTER_GETLIST");

                // Set the Company Code input parameter
                bapiProfitCenterGetList.SetValue("CONTROLLINGAREA", "CompnayCode");

                // Execute the BAPI
                bapiProfitCenterGetList.Invoke(sapConnection);

                // Check the return type of the BAPIRETURN structure
                var bapiReturn = bapiProfitCenterGetList.GetStructure("RETURN");
                var returnType = bapiReturn.GetString("TYPE");

                if (returnType == "E")
                {
                    var errorMessage = bapiReturn.GetString("MESSAGE");
                    return BadRequest(errorMessage);
                }

                // Retrieve the list of profit centers from the output table
                IRfcTable profitCenters = bapiProfitCenterGetList.GetTable("PROFITCENTER_LIST");

                List<ProfitCenterModel> profitCenterList = new List<ProfitCenterModel>();

                // Loop through the profit centers and create a list of ProfitCenterModel
                foreach (IRfcStructure profitcenter in profitCenters)
                {
                    string pc = profitcenter.GetString("PROFIT_CTR");

                    // You should create a ProfitCenterModel to store the profit center information.
                    ProfitCenterModel profitCenter = new ProfitCenterModel
                    {
                        code = pc
                    };
                    profitCenterList.Add(profitCenter);
                }

                return Ok(profitCenterList);
            }
            catch (Exception ex)
            {
                // Handle any exceptions, log, and return an error response
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("CostCenters")]
        public IHttpActionResult GetCostCenters()
        {
            try
            {
                // establish connection
                var sapConnection = SapConnectionManager.Connection;

                List<CostCenterModel> costCenters = new List<CostCenterModel>();

                IRfcFunction bapiCostCenterGetList = sapConnection.Repository.CreateFunction("BAPI_COSTCENTER_GETLIST");

                // Set the Company Code input parameter
                bapiCostCenterGetList.SetValue("CONTROLLINGAREA", "1000");

                // Execute the BAPI
                bapiCostCenterGetList.Invoke(sapConnection);

                // Check the return type of the BAPIRETURN structure
                var bapiReturn = bapiCostCenterGetList.GetStructure("RETURN");
                var returnType = bapiReturn.GetString("TYPE");

                if (returnType == "E")
                {
                    var errorMessage = bapiReturn.GetString("MESSAGE");
                    return BadRequest(errorMessage);
                }

                // Retrieve the list of cost centers from the output table
                IRfcTable costcenters = bapiCostCenterGetList.GetTable("COSTCENTER_LIST");

                // Loop through the cost centers and create a list of CostCenterModel
                foreach (IRfcStructure costenters in costcenters)
                {
                    var costCenter = new CostCenterModel
                    {
                        code = costenters.GetString("COSTCENTER"),
                        description = costenters.GetString("COCNTR_TXT")
                    };
                    costCenters.Add(costCenter);
                }

                return Ok(costCenters);
            }
            catch (Exception ex)
            {
                // Handle any exceptions, log, and return an error response
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("AccDocument")]
        public IHttpActionResult PostAccDocument(AccountingDocumentModel document)
        {
            try
            {
                // establish connection
                var sapConnection = SapConnectionManager.Connection;

                IRfcFunction bapiAccDocPost = sapConnection.Repository.CreateFunction("BAPI_ACC_DOCUMENT_POST");
                IRfcFunction commitFunction = sapConnection.Repository.CreateFunction("BAPI_TRANSACTION_COMMIT");

                // Set the header data for the accounting document
                IRfcStructure headerData = bapiAccDocPost.GetStructure("DOCUMENTHEADER");
                headerData.SetValue("COMP_CODE", document.CompanyCode);
                headerData.SetValue("DOC_DATE", document.DocumentDate);
                headerData.SetValue("PSTNG_DATE", document.PostingDate);
                headerData.SetValue("FISC_YEAR", document.FiscalYear);
                headerData.SetValue("FIS_PERIOD", document.FiscalPeriod);
                headerData.SetValue("DOC_TYPE", document.DocumentType);
                headerData.SetValue("REF_DOC_NO", document.ReferenceDocumentNo);
                headerData.SetValue("HEADER_TXT", document.DocumentHeaderText);
                headerData.SetValue("USERNAME", document.Username);

                // Create and set credit and debit entries based on the provided models
                foreach (var creditEntryModel in document.CreditEntries)
                {
                    IRfcTable creditEntry = bapiAccDocPost.GetTable("ACCOUNTGL");
                    creditEntry.Append();
                    creditEntry.SetValue("ITEMNO_ACC", creditEntryModel.ItemNo);
                    creditEntry.SetValue("COMP_CODE", creditEntryModel.CompanyCode);
                    creditEntry.SetValue("FIS_PERIOD", creditEntryModel.FiscalPeriod);
                    creditEntry.SetValue("PSTNG_DATE", creditEntryModel.PostingDate);
                    creditEntry.SetValue("VALUE_DATE", creditEntryModel.ValueDate);
                    creditEntry.SetValue("ITEM_TEXT", creditEntryModel.LineItemText);
                    creditEntry.SetValue("GL_ACCOUNT", creditEntryModel.GLAccount);
                    creditEntry.SetValue("PROFIT_CTR", creditEntryModel.ProfitCenter);
                    creditEntry.SetValue("HOUSEBANKID", creditEntryModel.HouseBankId);
                    creditEntry.SetValue("HOUSEBANKACCTID", creditEntryModel.HouseBankAccount);
                }

                foreach (var debitEntryModel in document.DebitEntries)
                {
                    IRfcTable debitEntry = bapiAccDocPost.GetTable("ACCOUNTGL");
                    debitEntry.Append();
                    debitEntry.SetValue("ITEMNO_ACC", debitEntryModel.ItemNo);
                    debitEntry.SetValue("COMP_CODE", debitEntryModel.CompanyCode);
                    debitEntry.SetValue("FIS_PERIOD", debitEntryModel.FiscalPeriod);
                    debitEntry.SetValue("PSTNG_DATE", debitEntryModel.PostingDate);
                    debitEntry.SetValue("VALUE_DATE", debitEntryModel.ValueDate);
                    debitEntry.SetValue("ITEM_TEXT", debitEntryModel.LineItemText);
                    debitEntry.SetValue("GL_ACCOUNT", debitEntryModel.GLAccount);
                    debitEntry.SetValue("PROFIT_CTR", debitEntryModel.ProfitCenter);
                    debitEntry.SetValue("HOUSEBANKID", debitEntryModel.HouseBankId);
                    debitEntry.SetValue("HOUSEBANKACCTID", debitEntryModel.HouseBankAccount);
                }

                // Create and set currency amounts based on the provided models
                foreach (var currencyAmountModel in document.CurrencyAmounts)
                {
                    IRfcTable currencyAmt = bapiAccDocPost.GetTable("CURRENCYAMOUNT");
                    currencyAmt.Append();
                    currencyAmt.SetValue("ITEMNO_ACC", currencyAmountModel.ItemNo);
                    currencyAmt.SetValue("CURRENCY", currencyAmountModel.DocumentCurrency);
                    currencyAmt.SetValue("AMT_DOCCUR", currencyAmountModel.AmountInDocumentCurrency);
                }

                RfcSessionManager.BeginContext(sapConnection);

                // Call the BAPI to post the accounting document
                bapiAccDocPost.Invoke(sapConnection);
                // Call the BAPI to commit the accounting document
                commitFunction.Invoke(sapConnection);

                RfcSessionManager.EndContext(sapConnection);

                // Check for any errors in the response
                IRfcTable returnTable = bapiAccDocPost.GetTable("RETURN");
                var returnType = returnTable.GetString("TYPE");

                foreach (IRfcStructure returnRow in returnTable)
                {
                    string message = returnRow.GetString("MESSAGE");
                    if (!string.IsNullOrEmpty(message))
                    {
                        return BadRequest($"An error occurred while posting the accounting document: {message}");
                    }
                }
                string documentNumber = bapiAccDocPost.GetString("OBJ_KEY");


                return Ok($"Accounting Document {documentNumber} posted successfully");
            }
            catch (Exception ex)
            {
                // Handle any exceptions, log, and return an error response
                return InternalServerError(ex);
            }
        }


    }
}

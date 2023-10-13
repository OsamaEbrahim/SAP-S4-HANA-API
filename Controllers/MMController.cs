using System;
using System.Collections.Generic;
using SAP.Middleware.Connector;
using System.Web.Http;
using SAP_S4_Hana_API.Models.MM;

namespace SAP_S4_Hana_API.Controllers
{
    [RoutePrefix("api/MM")]
    public class MMController : ApiController
    {
        [HttpGet]
        [Route("Materials")]
        public IHttpActionResult GetMaterials(string plant, string storageLocation)
        {

            try
            {
                // establish connection
                var sapConnection = SapConnectionManager.Connection;

                // Call the function to get materials
                IRfcFunction readTableFunc = sapConnection.Repository.CreateFunction("RFC_READ_TABLE");
                readTableFunc.SetValue("QUERY_TABLE", "MARD");
                readTableFunc.SetValue("DELIMITER", "|");
                readTableFunc.SetValue("ROWCOUNT", 1000);

                // Set the filter options for the storage location
                IRfcTable options = readTableFunc.GetTable("OPTIONS");
                options.Append();
                options.SetValue("TEXT", "LGORT ='" + storageLocation + "' AND WERKS ='" + plant + "'");


                // select the fields you want to retrieve
                IRfcTable fields = readTableFunc.GetTable("FIELDS");
                fields.Append();
                fields.SetValue("FIELDNAME", "MATNR");
                fields.Append();
                fields.SetValue("FIELDNAME", "LABST");
                fields.Append();
                fields.SetValue("FIELDNAME", "PSTAT");

                // Call the function and retrieve the results
                readTableFunc.Invoke(sapConnection);
                IRfcTable results = readTableFunc.GetTable("DATA");

                // Loop through the results and extract the material numbers and stock quantities
                List<MaterialModel> materials = new List<MaterialModel>();
                foreach (IRfcStructure row in results)
                {
                    string[] fieldsArr = row.GetString("WA").Split('|'); // Split the row data using the delimiter
                    string matnr = fieldsArr[0]; // The material number is the first field in the row
                    decimal labst = decimal.Parse(fieldsArr[1]); // The stock quantity is the second field in the row

                    string maktx = GetMaterialDescription(matnr); // the material description is stored in a different table


                    // Create a new MaterialModel instance with the extracted values and add it to the list
                    MaterialModel material = new MaterialModel
                    {
                        MaterialNumber = matnr,
                        StockQuantity = labst,
                        MaterialDescription = maktx
                    };
                    materials.Add(material);
                }
                // Return the materials as JSON
                return Ok(materials);
            }
            catch (Exception ex)
            {
                // Handle any exceptions, log, and return an error response
                return InternalServerError(ex);
            }
        }

        private string GetMaterialDescription(string matnr)
        {
            var sapConnection = SapConnectionManager.Connection;

            try
            {
                IRfcFunction readTableFunc = sapConnection.Repository.CreateFunction("RFC_READ_TABLE");
                readTableFunc.SetValue("QUERY_TABLE", "MAKT");
                readTableFunc.SetValue("DELIMITER", "|");
                readTableFunc.SetValue("ROWCOUNT", 1);

                IRfcTable options = readTableFunc.GetTable("OPTIONS");
                options.Append();
                options.SetValue("TEXT", $"MATNR = '{matnr}' AND SPRAS = 'EN'");

                IRfcTable fields = readTableFunc.GetTable("FIELDS");
                fields.Append();
                fields.SetValue("FIELDNAME", "MAKTX");

                readTableFunc.Invoke(sapConnection);
                IRfcTable results = readTableFunc.GetTable("DATA");

                if (results.Count > 0)
                {
                    return results[0].GetString("WA").Trim();
                }
            }
            catch
            {
                // You can handle or log exceptions if needed
            }
            return string.Empty;
        }
    }
}

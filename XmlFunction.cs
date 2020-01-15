using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Xml;
using Microsoft.Data.SqlClient;

namespace AzureFunctionApp
{
    public static class XmlFunction
    {
        [FunctionName("AddXmlDataToSqlDb")]
        public static async Task<IActionResult> AddXmlDataToSqlDb(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            XmlDocument doc = new XmlDocument();
            doc.Load(req.Body);
            XmlNode xmlNode = doc.DocumentElement;
            //  
            string connectionString = "Server=tcp:itlsqlmi03.public.d998d840e597.database.windows.net,3342;Persist Security Info=False;User ID=sqladmin112;Password=PeachtreeDelta889!@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            if (xmlNode.Name.ToUpper() == "FEEDLOT")
            {

                try
                {
                    string source = "1";
                    string feedyardName = xmlNode.Attributes["FeedyardName"].Value.Trim();
                    string feedyardPhone = xmlNode.Attributes["FeedyardPhone"].Value.Trim();
                    string creationDate = xmlNode.Attributes["CreationDate"].Value.Trim();
                    string clientUID = xmlNode.Attributes["UID"].Value.Trim();
                    string sourceVersion = xmlNode.Attributes["SourceVersion"].Value.Trim();
                    string hashCode = feedyardName + "," + feedyardPhone + "," + creationDate;
                    string fileGUID = Guid.NewGuid().ToString();
                    string accountID = "1";
                    SqlParameter[] param = new SqlParameter[10];
                    param[0]= new SqlParameter("@Source", source);
                    param[1]= new SqlParameter("@FeedyardName", feedyardName);
                    param[2]= new SqlParameter("@FeedyardPhone", feedyardPhone);
                    param[3]= new SqlParameter("@CreationDate", creationDate);
                    param[4]= new SqlParameter("@ModificationDate", DateTime.Now);
                    param[5]= new SqlParameter("@ClientUID", clientUID);
                    param[6]= new SqlParameter("@SourceVersion", sourceVersion);
                    param[7]= new SqlParameter("@HashCode", hashCode);
                    param[8]= new SqlParameter("@FileGUID", fileGUID);
                    param[9]= new SqlParameter("@AccountID", accountID);

                    SqlHelper.ExecuteNonQuery(connectionString, "IPC_DB_KX.dbo.sp_CIPInsertFeedLotToOrgByAccount", param);
                }
                catch (Exception ex)
                {
                    log.LogInformation(ex.Message);
                }
            }

            return (ActionResult)new OkObjectResult($"Sucessful");
        }
    }
}

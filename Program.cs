using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace AzureSQLSharePoint
{
    class Program
    {
        static void Main(string[] args)
        {
            
            using (var clientContext = new ClientContext("https://yoursite.sharepoint.com"))
            {
                // SharePoint Online Credentials    
                clientContext.Credentials = new SharePointOnlineCredentials(GetSPOAccountName(), GetSPOSecureStringPassword());
                Web web = clientContext.Web;
                clientContext.Load(web);
                clientContext.ExecuteQuery();

                List productList = web.Lists.GetByTitle("Chicken");
                DataTable dt = new DataTable();
                dt = GetDatafromSQL();
                foreach (DataRow dr in dt.Rows) // Loop over the rows.  
                {
                    ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                    ListItem newItem = productList.AddItem(itemCreateInfo);
              
                    newItem["firstname"] = dr["firstname"];
                    newItem["lastname"] = dr["lastname"];
                    newItem["phone"] = dr["phone"];
                    newItem["street"] = dr["street"];
                    newItem["city"] = dr["city"];
                    newItem["state"] = dr["state"];
                    newItem["zip"] = dr["zip"];
                    newItem["DateCreated"] = dr["DateCreated"];
                    newItem.Update();
                    clientContext.Load(newItem);
                    clientContext.ExecuteQuery();

                }
                clientContext.Load(productList);
                clientContext.ExecuteQuery();

            }
        }

        private static DataTable GetDatafromSQL()
        {
            //string connString = @"Server=YOURSERVERNAME;Database=YOURDATABE;uid=YOURUSERID;password=YOURPASSWORD";
            string connString = ConfigurationManager.ConnectionStrings["Products"].ConnectionString;
            //string query = "SELECT p.customer_id, p.first_name, p.last_name, p.phone, p.street, p.city, p.state, p.zip_code from sales.customers p where p.customer_Id<500;";
            SqlConnection connection = new SqlConnection(connString);
            //SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataAdapter sqlDa = new SqlDataAdapter("AllClients", connection);
            DataTable dataTable = new DataTable();
            sqlDa.Fill(dataTable);
            connection.Open();
            //SqlDataAdapter da = new SqlDataAdapter(cmd);
            //da.Fill(dataTable);
            connection.Close();
            sqlDa.Dispose();
            return dataTable;
        }



        private static SecureString GetSPOSecureStringPassword()
        {
            try
            {
                var secureString = new SecureString();
                foreach (char c in ConfigurationManager.AppSettings["SPOPassword"])
                {
                    secureString.AppendChar(c);
                }
                return secureString;
            }
            catch
            {
                throw;
            }
        }

        private static string GetSPOAccountName()
        {
            try
            {
                return ConfigurationManager.AppSettings["SPOAccount"];
            }
            catch
            {
                throw;
            }
        }
    }
}

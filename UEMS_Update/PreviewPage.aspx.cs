using DocumentFormat.OpenXml.Office2010.Word;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using iText.Pdfa;
//using System.Drawing;
//using iText.Kernel.Pdf;

public partial class PreviewPage : System.Web.UI.Page
{
    string ConnectionString = XCryptEngine.ConnectionStringEncryption.Decrypt(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
    int DocumentID = 0;
    protected void Page_Load(object sender, EventArgs e)
    
    {
        if(!IsPostBack)
        {
            // Retrieve the image URL from the session variable
            DocumentID = Int32.Parse(Request.QueryString["DocumentID"].ToString());
            selectImageFromDatabase(DocumentID);
            SlectData();
        }
    }


    public void selectImageFromDatabase(int DocumentID)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            connection.Open();
            string query = String.Format("SELECT Donnees From Documents Where DocumentID={0}", DocumentID);

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        
                        byte[] imageData = (byte[])reader["Donnees"];
                        string base64String = Convert.ToBase64String(imageData);

                        // Set the image URL and dimensions
                        Image1.ImageUrl = "data:image/jpeg;base64," + base64String;
                        Image1.Style["width"] = "100%"; // Set the width as needed
                        Image1.Style["height"] = "auto"; // Maintain aspect ratio

                    }
                }
            }
        }
    }

    public void SlectData()
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            connection.Open();
            string query = String.Format("SELECT Donnees From Documents Where DocumentID={0}", DocumentID);

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {

                        byte[] imageData = (byte[])reader["Donnees"];
                        Response.ContentType = "application/pdf";
                        Response.AddHeader("Content-Disposition", "inline; filename=output.pdf");

                        Response.BinaryWrite(imageData);
                        Response.Flush();
                        Response.End();
                        //string base64String = Convert.ToBase64String(imageData);

                        //// Set the image URL and dimensions
                        //Image1.ImageUrl = "data:image/jpeg;base64," + base64String;
                        //Image1.Style["width"] = "100%"; // Set the width as needed
                        //Image1.Style["height"] = "auto"; // Maintain aspect ratio

                    }
                }
            }
        }
    }

}
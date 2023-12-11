
using Bytescout.PDFRenderer;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class VoirLesDocumentsPourUnEtudiant : Page
{
    string ConnectionString = XCryptEngine.ConnectionStringEncryption.Decrypt(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
    static string sPersonneID = string.Empty;
    int index;
    List<Dossier> dossiers = new List<Dossier>();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                sPersonneID = Request.QueryString["PersonneID"];
                /// txtPersonneID.Text = sPersonneID;
            }
            catch (Exception ex)
            {

            }
            BindGridView();

            findStudentName(sPersonneID);
        }

    }

    private void findStudentName(string studentID)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = String.Format("SELECT Nom, Prenom From Personnes where PersonneID = '{0}'", studentID);

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            lblInfoEtudiant.Text += reader["Prenom"].ToString() + " " + reader["Nom"].ToString();
                        }
                    }
                }
            }
        } catch (Exception ex)
        {
            lblError.Text = ex.Message;
        }
    }
    private void BindGridView()
    {

        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            connection.Open();
            string query = String.Format("SELECT D.DocumentID, P.Nom, P.Prenom, D.Donnees, T.Description FROM Personnes AS P JOIN Documents AS D ON P.PersonneID = D.PersonneID JOIN TypeDocument AS T ON D.TypeDocumentID = T.TypeDocumentID WHERE P.PersonneID = '{0}'", sPersonneID);

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        byte[] imageData = (byte[])reader["Donnees"];
                        string base64String = Convert.ToBase64String(imageData);
                        dossiers.Add(new Dossier
                        {
                            documentId = Int32.Parse(reader["DocumentID"].ToString()),
                            ImageData = "data:image/png;base64," + PdfToImage(imageData), //"data:image/jpeg;base64," + base64String, // Add a property for image data in your Dossier class
                            description = reader["Description"].ToString()
                        });
                    }
                }
            }

            GridView1.DataSource = dossiers;
            GridView1.DataBind();
        }
    }


    private static System.Drawing.Image ConvertByteArrayToImage(byte[] byteArray)
    {
        try
        {
            if (byteArray == null || byteArray.Length == 0)
            {
                // Handle the case where the byte array is null or empty
                return null;
            }

            using (MemoryStream memoryStream = new MemoryStream(byteArray))
            {
                // Try to create an Image object from the byte array
                System.Drawing.Image image = System.Drawing.Image.FromStream(memoryStream);

                // Check if the image is in a valid format
                if (image.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.MemoryBmp.Guid)
                {
                    // The image is in an invalid format (e.g., MemoryBmp)
                    return null;
                }

                return image;
            }
        }
        catch (Exception ex)
        {
            // Log or handle the exception appropriately
            // Console.WriteLine($"Error converting byte array to image: {ex.Message}");
            return null;
        }
    }

    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Preview")
        {

            // Get the index of the row where the button was clicked
            int rowIndex = Convert.ToInt32(e.CommandArgument);


            Response.Redirect(string.Format("PreviewPage.aspx?DocumentID={0}", rowIndex));
        }
        if(e.CommandName == "addNew")
        {
            DropDownList dropDownFromFooter = (DropDownList)GridView1.FooterRow.FindControl("ddlDescription");
           
            // Find controls within the GridView row
            FileUpload fileUploadFooter = (FileUpload)GridView1.FooterRow.FindControl("fileUploadID");

            if (dropDownFromFooter.SelectedValue == string.Empty || dropDownFromFooter.SelectedIndex == 0)
            {
                lblError.Text = "Choisissez le type de fichier...";
                return;
            }
            if (fileUploadFooter.PostedFile != null && fileUploadFooter.PostedFile.ContentLength > 0)
            {
                // The FileUpload control has a file
                string fileName = Path.GetFileName(fileUploadFooter.FileName);

                string tempFilePath = Path.Combine(Server.MapPath("~\\"), fileName);
                fileUploadFooter.SaveAs(tempFilePath);

                byte[] fileBytes = File.ReadAllBytes(tempFilePath);

                AjouterDossiers(fileBytes, dropDownFromFooter.SelectedValue);
            }
            else
            {
                lblError.Text = "Choisissez un fichier...";
            }
        }
    }

    protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
    {

        GridView1.EditIndex = e.NewEditIndex;
        BindGridView();

    }

    protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        lblError.Text = string.Empty;
        //// Accessing the updated values
        ///
        string id = GridView1.DataKeys[e.RowIndex].Values["documentId"].ToString();

        int documentID = Int32.Parse(id);
        // You can now use the 'id' and 'name' variables as needed.

        GridViewRow editingRow = GridView1.Rows[e.RowIndex];

        // Find the FileUpload control in the editing row
        FileUpload fileUpload = (FileUpload)editingRow.FindControl("fileUploadID");
        DropDownList ddlDescription = (DropDownList)editingRow.FindControl("ddlDescription");
       
        if (ddlDescription.SelectedValue == string.Empty || ddlDescription.SelectedIndex == 0)
        {
            lblError.Text = "Choisissez le type de fichier...";
            return;
        }
        else
        {
            if (fileUpload.PostedFile != null && fileUpload.PostedFile.ContentLength > 0)
            {
                // The FileUpload control has a file
                string fileName = Path.GetFileName(fileUpload.FileName);

                string tempFilePath = Path.Combine(Server.MapPath("~\\"), fileName);
                fileUpload.SaveAs(tempFilePath);

                byte[] fileBytes = File.ReadAllBytes(tempFilePath);

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    // Create the SQL UPDATE statement
                    string updateQuery = "UPDATE Documents SET Donnees = @Donnees , TypeDocumentID = @TypeDocumentID, CreeParUsername=@CreeParUsername WHERE DocumentID = @DocumentID";

                    using (SqlCommand command = new SqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Donnees", fileBytes);
                        command.Parameters.AddWithValue("@TypeDocumentID", Int32.Parse(ddlDescription.SelectedValue));
                        command.Parameters.AddWithValue("@DocumentID", documentID);
                        command.Parameters.AddWithValue("@CreeParUsername", Environment.UserName);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            lblError.Text ="Row updated successfully.";
                            File.Delete(tempFilePath);
                            BindGridView();
                            Response.Redirect(string.Format("VoirLesDocumentsPourUnEtudiant.aspx?PersonneID={0}", sPersonneID));
                        }
                        else
                        {
                            Console.WriteLine("No rows were updated.");
                        }

                    }
                }
                // Cancel the edit mode
                GridView1.EditIndex = -1;

                BindGridView();
            }
            else
            {
                lblError.Text = "Vous devez choisir un fichier";
                return;
            }
        }

    }

    protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GridView1.EditIndex = -1;

        BindGridView();
    }

    protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridView1.PageIndex = e.NewPageIndex;
        // Rebind your data to refresh the GridView with the new page
        BindGridView();
    }
   
    private DataTable GetDataFromDatabase(String query)
    {
        DataTable dt = new DataTable();
        try
        {           
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                dt = new DataTable();
                adapter.Fill(dt);
               
            }
        }catch(Exception ex)
        {
            
        }

        return dt;
    }

    protected void AjouterDossiers(byte[] fileBytes, string TypeDocument)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            connection.Open();

            // Create the SQL INSERT statement
            string insertQuery = "INSERT INTO Documents (PersonneID,TypeDocumentID, Donnees, CreeParUsername) VALUES (@PersonneID,@TypeDocumentID, @Donnees, @CreeParUsername)";

            using (SqlCommand command = new SqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@PersonneID", sPersonneID);
                command.Parameters.AddWithValue("@Donnees", fileBytes);
                command.Parameters.AddWithValue("@TypeDocumentID", TypeDocument);
                command.Parameters.AddWithValue("@CreeParUsername", Environment.UserName);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    lblError.Text = "Row updated successfully.";
                    BindGridView();
                    Response.Redirect(string.Format("VoirLesDocumentsPourUnEtudiant.aspx?PersonneID={0}", sPersonneID));
                }
                else
                {
                    Console.WriteLine("No rows were updated.");
                }

            }
        }
    }

    public String PdfToImage(byte[] pdfBytes)
    {
        // Replace this with your actual PDF byte array
        string base64String = string.Empty;

        // Create an instance of Bytescout.PDFRenderer.RasterRenderer object and register it.
        RasterRenderer renderer = new RasterRenderer();
        renderer.RegistrationName = "demo";
        renderer.RegistrationKey = "demo";

        try
        {
            // Load PDF document from byte array
            using (MemoryStream pdfStream = new MemoryStream(pdfBytes))
            {
                // Load the document without closing the MemoryStream
                renderer.LoadDocumentFromStream(pdfStream);

                // Assume you want to display the first page
                int pageIndex = 0;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // Render document page and save to memory stream
                    renderer.Save(memoryStream, RasterImageFormat.PNG, pageIndex, 300);

                    // Convert the MemoryStream to a byte array
                    byte[] pngBytes = memoryStream.ToArray();

                    // Convert the byte array to a Base64 string
                    base64String = Convert.ToBase64String(pngBytes);

                    
                }
            }

        }
        catch (Exception ex)
        {
            lblError.Text= "Error: " +ex.Message;
        }
        finally
        {
            // Cleanup
            renderer.Dispose();
        }

        return base64String;
    }


    //static System.Drawing.Image ConvertPdfToImage(byte[] pdfBytes)
    //{
    //    using (MemoryStream pdfStream = new MemoryStream(pdfBytes))
    //    {
    //        using (PdfReader pdfReader = new PdfReader(pdfStream))
    //        {
    //            using (PdfDocument pdfDocument = new PdfDocument(pdfReader))
    //            {
    //                // Extract text from the first page (adjust as needed)
    //                ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
    //                string text = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1), strategy);

    //                // For simplicity, let's just create an image with the extracted text
    //                Bitmap image = new Bitmap(800, 600);
    //                using (Graphics g = Graphics.FromImage(image))
    //                {
    //                    g.DrawString(text, SystemFonts.DefaultFont, Brushes.Black, new PointF(10, 10));
    //                }

    //                return image;
    //            }
    //        }
    //    }
    //}
    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {   
            if(e.Row.RowType == DataControlRowType.EmptyDataRow)
            {
                // Get the DropDownList in the edit mode
                DropDownList ddlDescription = (DropDownList)e.Row.FindControl("ddlDescriptionEmptyFooter");

                // Check if the DropDownList is found
                if (ddlDescription != null)
                {
                    // Populate the DropDownList with data from the database
                    ddlDescription.DataSource = GetDataFromDatabase("Select * from TypeDocument"); // Implement this method to fetch data
                    ddlDescription.DataTextField = "Description"; // Set the appropriate field name
                    ddlDescription.DataValueField = "TypeDocumentID"; // Set the appropriate field name              
                    ddlDescription.DataBind();

                    // Optionally, set a default value
                    ddlDescription.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Choisir le type de fichier", "-1"));
                    ddlDescription.SelectedValue = "-1"; // Set the default value
                }
            }
            else
            {
                // Get the DropDownList in the edit mode
                DropDownList ddlDescription = (DropDownList)e.Row.FindControl("ddlDescription");

                // Check if the DropDownList is found
                if (ddlDescription != null)
                {
                    // Populate the DropDownList with data from the database
                    ddlDescription.DataSource = GetDataFromDatabase("Select * from TypeDocument"); // Implement this method to fetch data
                    ddlDescription.DataTextField = "Description"; // Set the appropriate field name
                    ddlDescription.DataValueField = "TypeDocumentID"; // Set the appropriate field name              
                    ddlDescription.DataBind();

                    // Optionally, set a default value
                    ddlDescription.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Choisir le type de fichier", "-1"));
                    ddlDescription.SelectedValue = "-1"; // Set the default value
                }
            }

        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
        }
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {

        FileUpload fileUploadfromFooterEmpty = (FileUpload)GridView1.Controls[0].Controls[1].FindControl("fileUploadID");
        DropDownList dropDownfromFooterEmpty = (DropDownList)GridView1.Controls[0].Controls[1].FindControl("ddlDescription");
        if (dropDownfromFooterEmpty.SelectedValue == string.Empty || dropDownfromFooterEmpty.SelectedIndex == 0)
        {
            lblError.Text = "Choisissez le type de fichier...";
            return;
        }
        if (fileUploadfromFooterEmpty.PostedFile != null && fileUploadfromFooterEmpty.PostedFile.ContentLength > 0)
        {
            // The FileUpload control has a file
            string fileName = Path.GetFileName(fileUploadfromFooterEmpty.FileName);

            string tempFilePath = Path.Combine(Server.MapPath("~\\"), fileName);
            fileUploadfromFooterEmpty.SaveAs(tempFilePath);

            byte[] fileBytes = File.ReadAllBytes(tempFilePath);

            AjouterDossiers(fileBytes, dropDownfromFooterEmpty.SelectedValue);
            File.Delete(tempFilePath);
        }
        else
        {
            lblError.Text = "Choisissez un fichier...";
        }
    }
    public class Dossier
    {
        public int documentId { get; set; }
        public string ImageData { get; set; }
        public string description { get; set; }
        public string createParUser { get; set; }
    }

}
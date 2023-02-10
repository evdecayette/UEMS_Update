using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;

public partial class _Default : System.Web.UI.Page
{
    string ConnectionString = XCryptEngine.ConnectionStringEncryption.Decrypt(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Loop through all records
            DB_Access db = new DB_Access();
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {                
                try
                {
                    sqlConn.Open();
                    int iPosition = 1;
                    DateTime convertDate = new DateTime();

                    string sSql = "SELECT * FROM Evenements WHERE EvenementDate >= GetDate() ORDER BY EvenementDate";

                    SqlDataReader dt = db.GetDataReader(sSql, sqlConn);
                    if (dt != null)
                    {
                        while (dt.Read())
                        {
                            convertDate = DateTime.Parse(dt["EvenementDate"].ToString());
                            switch (iPosition++)
                            {
                                case 1:
                                    Label1_header.Text = convertDate.ToString("dddd, dd MMMM yyyy");
                                    Literal1.Text = dt["EvenementDescription"].ToString();
                                    break;
                                case 2:
                                    Label2_header.Text = convertDate.ToString("dddd, dd MMMM yyyy");
                                    Literal2.Text = dt["EvenementDescription"].ToString();
                                    break;
                                case 3:
                                    Label3_header.Text = convertDate.ToString("dddd, dd MMMM yyyy");
                                    Literal3.Text = dt["EvenementDescription"].ToString();
                                    break;
                                case 4:
                                    Label4_header.Text = convertDate.ToString("dddd, dd MMMM yyyy");
                                    Literal4.Text = dt["EvenementDescription"].ToString();
                                    break;
                                case 5:
                                    Label5_header.Text = convertDate.ToString("dddd, dd MMMM yyyy");
                                    Literal5.Text = dt["EvenementDescription"].ToString();
                                    break;
                                case 6:
                                    Label6_header.Text = convertDate.ToString("dddd, dd MMMM yyyy");
                                    Literal6.Text = dt["EvenementDescription"].ToString();
                                    break;
                                default:
                                    break;  // want only the first 9 
                            }
                        }
                        dt.Close();
                        dt = null;
                    }
                    //db = null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    ////db = null;
                }
            }
        }
    }

    protected void btnEditAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect("AddEditEvenements.aspx");
    }
}
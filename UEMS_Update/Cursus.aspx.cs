using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;
using System.Data;

public partial class Cursus : System.Web.UI.Page
{
    string ConnectionString = XCryptEngine.ConnectionStringEncryption.Decrypt(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

           String DisciplineID = Request.QueryString["DisciplineID"];
            try
            {
                ContentLiteralID.Text = BuildString(DisciplineID);
                //// Enlever le cookie
                Response.Cookies.Remove("PersonneID");                              // First method
                HttpCookie cookie = new HttpCookie("PersonneID", String.Empty);     // Second remove method, in case
                Response.Cookies.Set(cookie);
            }
            catch (Exception NoNeed)
            {
                Debug.WriteLine(NoNeed.Message);
            }
            
        }
    }

    String BuildString(String DisciplineID)
    {
        String returnedString = String.Empty;

        returnedString += @"<div class='row'><div class='col-lg-12'><h2 class='page-header'>Liste des Cours</h2></div></div>";        
        returnedString += @"<div class='panel-body'>";
        returnedString += @"<table width='100%' class='table table-striped table-bordered table-hover' id='dataTables-Etudiants'>";
        returnedString += @"<thead><tr><th>Cours</th><th>Numéro</th><th>Crédits</th><th>Pré-Requis</th>";
        returnedString += @"<th>Lien</th></thead><tbody>";

        // Loop through all records
        DB_Access db = new DB_Access();
        if (DisciplineID != String.Empty)
        {
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                try
                {
                    sqlConn.Open();
                    SqlParameter ParamDisciplineID = new SqlParameter("@DisciplineID", SqlDbType.Text);
                    ParamDisciplineID.Value = DisciplineID;
                    string sSql = "SELECT * FROM Disciplines DI, Cours C WHERE DI.NumeroCours = C.NumeroCours AND DisciplineID = @DisciplineID";
                    int iActif = 0;

                    SqlDataReader dt = db.GetDataReader(sSql, sqlConn);
                    if (dt != null)
                    {
                        while (dt.Read())
                        {
                            returnedString += String.Format(@"<tr class='odd gradeX'><td>{0}</td>", dt["DisciplineNom"].ToString());
                            returnedString += String.Format(@"<td>{0}</td>", dt["DisciplineDescription"].ToString());
                            returnedString += String.Format(@"<td class='center'>{0}</td>", dt["DepartementNom"].ToString());
                            returnedString += String.Format(@"<td class='center'>{0}</td>", dt["Actif"].ToString());

                            iActif = int.Parse(dt["Actif"].ToString());
                            if (iActif == 1)
                            {
                                returnedString += String.Format(@"<td class='center'><a href='Cursus.aspx?DepartementID={0}'>Cursus</a></td>",
                                        dt["DepartementID"].ToString());
                            }
                            else
                            {
                                returnedString += String.Format(@"<td class='center'></td>");
                            }
                        }
                    }
                    db = null;
                }
                catch (Exception ex)
                {
                    returnedString = ex.Message;
                    return returnedString;
                }
                finally
                {
                    db = null;
                }
            }
        }
        returnedString += @"</tbody></table>";
        returnedString += @"</div></div></div></div>";


        return returnedString;
    }
}
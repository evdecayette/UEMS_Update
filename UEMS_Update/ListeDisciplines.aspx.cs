using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;

public partial class ListeDisciplines : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ContentLiteralID.Text = BuildString();
        }
    }

    String BuildString()
    {
        String returnedString = String.Empty;

        returnedString += @"<div class='row'><div class='col-lg-12'><h1 class='page-header'>Liste des Disciplines</h1></div></div>";
        returnedString += @"<div class='row'><div class='col-lg-12'><div class='panel panel-default'><div class='panel-heading'>Sélectionnez Une Discipline Pour Voir le Cursus</div>";
        returnedString += @"<div class='panel-body'>";
        returnedString += @"<table width='100%' class='table table-striped table-bordered table-hover' id='dataTables-Etudiants'>";
        returnedString += @"<thead><tr><th>Discipline</th><th>Description</th><th>Département</th>";
        returnedString += @"<th>Status</th>";
        returnedString += @"<th>Lien</th></thead><tbody>";

        // Loop through all records
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            try
            {
                sqlConn.Open();
                string sSql = "SELECT * FROM Disciplines DI, Departements DE WHERE DI.DepartementID = DE.DepartementID";
                int iActif = 0;
                SqlDataReader dt = db.GetDataReader(sSql, sqlConn);
                if (dt != null)
                {
                    while (dt.Read())
                    {
                        returnedString += String.Format(@"<tr class='odd gradeX'><td>{0}</td>", dt["DisciplineNom"].ToString());
                        returnedString += String.Format(@"<td>{0}</td>", dt["DisciplineDescription"].ToString());
                        returnedString += String.Format(@"<td class='center'>{0}</td>", dt["DepartementNom"].ToString());
                        iActif = int.Parse(dt["Actif"].ToString());
                        returnedString += String.Format(@"<td class='center'>{0}</td>", iActif == 1 ? "Active" : "Inactive" );
                        
                        if (iActif == 1)
                        {
                            returnedString += String.Format(@"<td class='center'><a href='Cursus.aspx?DisciplineID={0}'>Cursus</a></td>",
                                    dt["DisciplineID"].ToString());
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
        returnedString += @"</tbody></table>";
        returnedString += @"</div></div></div></div>";

        return returnedString;
    }

}
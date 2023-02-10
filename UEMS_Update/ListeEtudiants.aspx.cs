using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;

public partial class ListeEtudiants : System.Web.UI.Page
{
    string ConnectionString = XCryptEngine.ConnectionStringEncryption.Decrypt(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);

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

        returnedString += @"<div class='row'><div class='col-lg-12'><h1 class='page-header'>Recherche d'un Etudiant</h1></div></div>";
        returnedString += @"<div class='row'><div class='col-lg-12'><div class='panel panel-default'><div class='panel-heading'>Liste de Tous les Etudiants</div>";
        returnedString += @"<div class='panel-body'>";
        returnedString += @"<table width='100%' class='table table-striped table-bordered table-hover' id='dataTables-Etudiants'>";
        returnedString += @"<thead><tr><th>Nom de Famille</th><th>Prénom</th><th>Téléphone</th>";
        returnedString += @"<th>Email</th><th>NIF ou Matricule</th><th>No. Etudiant</th><th>Selection</th></thead><tbody>";

        // Loop through all records
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
        {
            try
            {
                sqlConn.Open();
                string sSql = "SELECT * FROM Personnes WHERE Etudiant = 1 ";
                int iActif = 0;

                SqlDataReader dt = db.GetDataReader(sSql, sqlConn);
                if (dt != null)
                {
                    while (dt.Read())
                    {
                        returnedString += String.Format(@"<tr class='odd gradeX'><td>{0}</td>", dt["Nom"].ToString());
                        returnedString += String.Format(@"<td>{0}</td><td>{1}</td>", dt["Prenom"].ToString(), dt["Telephone1"].ToString());
                        returnedString += String.Format(@"<td class='center'>{0}</td>", dt["email"].ToString());
                        returnedString += String.Format(@"<td class='center'>{0}</td>", dt["NIF"].ToString());
                        returnedString += String.Format(@"<td class='center'>{0}</td>", dt["EtudiantID"].ToString());
                        iActif = int.Parse(dt["Actif"].ToString());
                        if (iActif == 1)
                        {
                            returnedString += String.Format(@"<td class='center'><a href='InfoEtudiant.aspx?personneid={0}'><image src='images/lion.png'  alt='Choisir Etudiant ACTIF' style='width:25px;height:25px;'></a>" +
                                    "&#32;&#32;&#32;--&#32;&#32;&#32;<a href='RecevoirPaiements.aspx?personneid={0}'><image src='images/gourdes.jpg'  alt='Choisir' style='width:40px;height:25px;'></a></td></tr>", dt["PersonneID"].ToString());
                        }
                        else
                        {
                            returnedString += String.Format(@"<td class='center'><a href='InfoEtudiant.aspx?personneid={0}'><image src='images/redx.png'  alt='Choisir Etudiant INACTIF' style='width:25px;height:25px;'></a>" +
                                    "&#32;&#32;&#32;--&#32;&#32;&#32;<a href='RecevoirPaiements.aspx?personneid={0}'><image src='images/gourdes.jpg'  alt='Choisir' style='width:40px;height:25px;'></a></td></tr>", dt["PersonneID"].ToString());
                        }
                    }
                }
                //db = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                returnedString = "ERREUR: Base de Données dans Construction Chaine";
                return returnedString;
            }
            finally
            {
                //db = null;
            }
        }
        returnedString += @"</tbody></table>";
        returnedString += @"</div></div></div></div>";

        return returnedString;
    }
    protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
    {

    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class EtudiantsNombreCredits : System.Web.UI.Page
{
    string sSql = "";
    string ConnectionString = XCryptEngine.ConnectionStringEncryption.Decrypt(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            sSql = "SELECT DISTINCT P.Nom, P.Prenom, P.EtudiantID, P.PersonneID, SUM(C.Credits) AS Credits, DisciplineNom " +
                 " FROM CoursPris CP, Personnes P, Cours C, Disciplines D " +
                 " WHERE CP.PersonneID = P.PersonneID AND CP.NumeroCours = C.NumeroCours AND P.DisciplineID = D.DisciplineID " +
                 " AND P.Actif = 1 AND C.ExamenEntree = 0 AND NoteSurCent >= NotePassage " +
                 " group by P.PersonneID, P.Nom, P.Prenom, P.EtudiantID, P.PersonneID, DisciplineNom " +
                 " ORDER BY Credits DESC, P.Nom, P.Prenom";
            litBody.Text = ProcessInfo(sSql);
        }
    }

    String ProcessInfo(String sSql)
    {
        String sRetString = String.Format("<div style=\'page-break-after:always;\'></div>");    // Start with page break in order not to print the button 'print'
        int nombreEtudiants = 0;
        double moyenne;

        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
        {
            try
            {
                sqlConn.Open();
                SqlDataReader dtTemp = db.GetDataReader(sSql, sqlConn);

                // start new table
                sRetString += String.Format("<TABLE style='width:80%;align:center'>");
                sRetString += String.Format("<TR><TD Colspan='6' style='width:80%;text-align:center;font-weight:bold;font-size:14px'>Université Espoir</TD></TR>");
                sRetString += String.Format("<TR><TD Colspan='6' style='width:80%;text-align:center;font-weight:bold;font-size:14px'>Liste des Etudiants et le nombre de Crédits</TD></TR>");
                sRetString += String.Format("<TR><TD Colspan='6' style='width:80%;text-align:center;font-weight:bold;font-size:14px'>Date d'Impression: {0}</TD></TR>", DateTime.Today.Date.ToString("dd-MMM-yyyy"));
                sRetString += String.Format("<TR><TD Colspan='6' width:'80%'><hr style='background-color:#669999;' size='3'/></TD></TR>");
                sRetString += String.Format("<TR><TD width:'40%' style='text-align:left;font-weight:bold;font-size:14px'>Nom</TD>" +
                    "<TD style='text-align:center;font-weight:bold;font-size:14px'>Prénom</TD>" +
                    "<TD style='text-align:center;font-weight:bold;font-size:14px'>Numéro Etudiant</TD>" +
                    "<TD style='text-align:center;font-weight:bold;font-size:14px'>Nombre de Crédits</TD>" + 
                    "<TD style='text-align:center;font-weight:bold;font-size:14px'>Discipline</TD>" +
                    "<TD style='text-align:center;font-weight:bold;font-size:14px'>Moyenne sur 4.0</TD></TR>");
                sRetString += String.Format("<TR><TD Colspan='6' width:'80%'><hr style='background-color:#669999;' size='3'/></TD></TR>");

                if (dtTemp.Read())
                    do
                    {
                        nombreEtudiants++;
                        //string EtudiantID = dtTemp["EtudiantID"].ToString();

                        moyenne = db.GetMoyennePersonneID(dtTemp["PersonneID"].ToString())/25;

                        sRetString += String.Format("<TR><TD>&nbsp;&nbsp;&nbsp;&nbsp;{0}</TD>" +
                        "<TD style='text-align:center;'>{1}</TD>" +
                        "<TD style='text-align:center;'>{2}</TD>" +
                        "<TD style='text-align:center;'>{3}</TD>" +
                        "<TD style='text-align:center;'>{4}</TD>" +
                        "<TD style='text-align:center;'>{5}</TD></TR>",
                          dtTemp["Nom"].ToString(),
                          dtTemp["Prenom"].ToString(),
                          dtTemp["EtudiantID"].ToString(),
                          dtTemp["Credits"].ToString(),
                          dtTemp["DisciplineNom"].ToString(),
                          moyenne.ToString("F")
                          );
                    }
                    while (dtTemp.Read());

                db = null;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                sRetString += "<br> ERREUR - ERREUR - ERREUR !!!";
                db = null;
            }
        }
        sRetString += String.Format("<TR><TD Colspan='6' width:'80%'><hr style='background-color:#669999;' size='2' width='100%'/></TD></TR>");
        sRetString += String.Format("<TR><TD width:'40%' style='text-align:left;font-weight:bold;font-size:14px'>Nombre D'Etudiants: {0}</TD>", nombreEtudiants);
        sRetString += String.Format("<TR><TD Colspan='6' width:'80%'><hr style='background-color:#669999;' size='2' width='100%'/></TD></TR>");
        sRetString += "</TABLE>";
        return sRetString;
    }
}
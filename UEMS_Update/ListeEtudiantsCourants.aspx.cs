using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;

public partial class ListeEtudiantsCourants : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                string sSql = String.Format("SELECT DISTINCT P.EtudiantIdPlus, P.Nom, P.Prenom, count(CO.CoursOffertID) AS NombreDeCours " +
                    " FROM CoursPris CP, Cours C, Personnes P, CoursOfferts CO " +
                    " WHERE CP.NumeroCours = C.NumeroCours AND CP.PersonneID = P.PersonneID " +
                     " AND CP.CoursOffertID = CO.CoursOffertID " +
                     " AND C.ExamenEntree = 0 AND C.Credits > 0 AND IsNull(CP.DateReussite, '') = '' " +
                     " AND CP.CoursOffertID IN(SELECT CoursOffertID FROM CoursOfferts CO, Cours C WHERE CO.NumeroCours = C.NumeroCours AND CO.Actif = 1 AND Credits > 0) " +
                     " GROUP BY P.EtudiantIdPlus, P.Nom, P.Prenom " +
                     " ORDER BY P.Nom, P.Prenom DESC");

                litBody.Text = ProcessInfo(sSql);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }

    String ProcessInfo(String sSql)
    {
        String sRetString = String.Format("<div style=\'page-break-after:always;\'></div>");    // Start with page break in order not to print the button 'print'
        //int creditsTotal = 0;

        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            try
            {
                sqlConn.Open();

                SqlDataReader dtTemp = db.GetDataReader(sSql, sqlConn);
                if (dtTemp.Read())
                {
                    sRetString += String.Format("<TABLE style='width:80%;align:center'>");
                    sRetString += String.Format("<TR><TD Colspan='5' style='width:80%;text-align:center;font-weight:bold;font-size:14px'>Université Espoir</TD></TR>");
                    sRetString += String.Format("<TR><TD Colspan='5' style='width:80%;text-align:center;font-weight:bold;font-size:14px'>Liste des Etudiants Courants</TD></TR>");
                    sRetString += String.Format("<TR><TD Colspan='5' style='width:80%;text-align:center;font-weight:bold;font-size:14px'>Date d'Impression: {0}</TD></TR>", DateTime.Today.Date.ToString("dd-MMM-yyyy"));
                    sRetString += String.Format("<TR><TD Colspan='5' width:'80%'><hr style='background-color:#669999;' size='3'/></TD></TR>");
                    sRetString += String.Format("<TR><TD style='text-align:left;font-weight:bold;font-size:14px'>Numéro</TD>" +
                        "<TD style='text-align:center;font-weight:bold;font-size:14px'>ID Etudiant</TD>" +
                        "<TD style='text-align:center;font-weight:bold;font-size:14px'>Nom</TD>" +
                        "<TD style='text-align:center;font-weight:bold;font-size:14px'>Prénom</TD>" +
                        "<TD style='text-align:center;font-weight:bold;font-size:14px'>Nombre de Cours</TD>");
                    sRetString += String.Format("<TR><TD Colspan='5' width:'80%'><hr style='background-color:#669999;' size='3'/></TD></TR>");

                    int numero = 1;
                    do
                    {
                        sRetString += String.Format("<TR><TD style='text-align:center;'>{0}</TD><TD style='text-align:center;'>{1}</TD>" +
                        "<TD style='text-align:center;'>{2}</TD>" +
                        "<TD style='text-align:center;'>{3}</TD>" +
                        "<TD style='text-align:center;'>{4}</TD></TR>", numero.ToString(),
                        dtTemp["EtudiantIdPlus"].ToString(), dtTemp["Nom"].ToString(),
                        dtTemp["Prenom"].ToString(), dtTemp["NombreDeCours"].ToString());
                        numero += 1;
                    }
                    while (dtTemp.Read());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                sRetString += "<br> ERREUR 2 : ProcessInfo!";
            }
        }
        return sRetString;
    }

}
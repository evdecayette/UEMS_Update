using System;

using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;

public partial class NombreEtudiantsParClasse : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            litBody.Text = ProcessInfo();
        }
    }

    String ProcessInfo()
    {
        String sRetString = String.Empty;
        int iNombreEtudiants = 0;

        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            try
            {
                sqlConn.Open();
                String sSql = String.Format("SELECT DISTINCT C.NomCours, C.NumeroCours, COUNT(P.PersonneID) AS Nombre, PF.Nom, PF.Prenom " +
                    " FROM Personnes P, CoursPris CP, CoursOfferts CO, Cours C, Professeurs PF " +
                    " WHERE P.PersonneID = CP.PersonneID AND CP.CoursOffertID = CO.CoursOffertID " +
                    " AND CO.NumeroCours = C.NumeroCours AND CO.ProfesseurID = PF.ProfesseurID " +
                    " AND C.ExamenEntree = 0 AND CO.Actif = 1 " +
                    " GROUP BY C.NumeroCours, C.NomCours, PF.Nom, PF.Prenom " +
                    " ORDER BY C.NomCours");

                SqlDataReader dtTemp = db.GetDataReader(sSql, sqlConn);
                if (dtTemp.Read())
                {
                    // First en-tête
                    sRetString += WriteEntete();
                    do
                    {
                        // Pas d'entête; Juste Ecrire une ligne de détails ou Justifications
                        sRetString += String.Format("<TR><TD>{0}</TD><TD>{1}</TD><TD>{2}</TD><td>{3}</td></TR>",
                            dtTemp["NomCours"].ToString(), dtTemp["NumeroCours"].ToString()+" - "+ dtTemp["NomCours"].ToString(), dtTemp["Nombre"].ToString(),
                            dtTemp["Prenom"].ToString() + " " + dtTemp["Nom"].ToString().ToUpper());
                        iNombreEtudiants += int.Parse(dtTemp["Nombre"].ToString());
                    }
                    while (dtTemp.Read());
                }
                else
                {
                    sRetString += String.Format("<TR><TD Colspan='4'>Pas d'Information !!!!</TD></TR>");
                }
                dtTemp.Close();

                db = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                sRetString += "<br> ERREUR - ERREUR - ERREUR !!!";
                db = null;
            }
        }
        sRetString += String.Format("<TR><TD Colspan='4' width:'100%'><hr style='background-color:#669999;' size='3' width='100%'/></TD></TR>");
        sRetString += String.Format("<TR><TD></TD><TD style='text-align:left;font-weight:bold;font-size:14px'>TOTAL:</TD><TD style='text-align:left;font-weight:bold;font-size:14px'>{0}</TD></TR>", iNombreEtudiants);
        sRetString += String.Format("<TR><TD Colspan='4'><div style=\'page-break-after:always;\'></div></TD></TR>");
        sRetString += String.Format("<TR><TD Colspan='4' width:'100%'><hr style='background-color:#669999;' size='3' width='100%'/></TD></TR>");

        sRetString += "</TABLE>";

        return sRetString;
    }

    String WriteEntete()
    {
        String sRetString = String.Format("<TABLE width=100%>");
        sRetString += String.Format("<TR><TD Colspan='4' style='width:100%;text-align:center;font-weight:bold;font-size:14px'>Université Espoir</TD></TR>");
        sRetString += String.Format("<TR><TD Colspan='4' style='width:100%;text-align:center;font-weight:bold;font-size:14px'>Nombre d'Etudiants Par Cours</TD></TR>");
        sRetString += String.Format("<TR><TD Colspan='4' style='width:100%;text-align:center;font-weight:bold;font-size:14px'>Session Courante</TD></TR>");
        sRetString += String.Format("<TR><TD Colspan='4' style='width:100%;text-align:center;font-weight:bold;font-size:14px'>Date de ce Rapport: {0}</TD></TR>", DateTime.Today.ToString("dd-MMM-yyyy"));
        sRetString += String.Format("<TR><TD Colspan='4' width:'100%'><hr style='background-color:#669999;' size='3'/></TD></TR>");
        sRetString += String.Format("<TR style='font-weight:bold'><TD align='left'>Cours</TD><TD align='left'>Numéro du Cours</TD><TD align='left'>Nombre d'Etudiants</TD><TD align='center'>Professeur</TD></TR>");
        sRetString += String.Format("<TR><TD Colspan='4' width:'100%'><hr style='background-color:#669999;' size='3'/></TD></TR>");

        return sRetString;
    }
}
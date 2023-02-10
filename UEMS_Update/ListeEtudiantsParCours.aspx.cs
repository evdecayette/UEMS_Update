using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;


public partial class ListeEtudiantsParCours : System.Web.UI.Page
{
    string ConnectionString = XCryptEngine.ConnectionStringEncryption.Decrypt(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
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
        String sNewCours = String.Empty, sOldCours = String.Empty;
        String sSessionID = String.Empty, sOldSessionID = String.Empty;
        DateTime sSessionStartDate = DateTime.Now, sSessionEndDate = DateTime.Now;

        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn0 = new SqlConnection(ConnectionString))
        {
            try
            {
                sqlConn0.Open();
                sSessionStartDate = db.GetStartDateOfCurrentSession(sqlConn0);
                sSessionEndDate = db.GetEndDateOfCurrentSession(sqlConn0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
        {
            try
            {
                sqlConn.Open();

                String sSql = String.Format("SELECT P.Prenom, P.Nom, IsNull(P.NIF, '') AS NIF, P.Telephone1, P.email, C.NomCours, C.NumeroCours, CP.NoteSurCent, CO.SessionID, CO.CoursOffertID " +
                    " FROM Personnes P, CoursPris CP, CoursOfferts CO, Cours C " +
                    " WHERE P.PersonneID = CP.PersonneID AND CP.CoursOffertID = CO.CoursOffertID AND CO.NumeroCours = C.NumeroCours " +
                    " AND C.ExamenEntree = 0 AND CO.Actif = 1 ORDER BY C.NumeroCours, CO.CoursOffertID, P.Nom");

                SqlDataReader dtTemp = db.GetDataReader(sSql, sqlConn);
                if (dtTemp.Read())
                {
                    sNewCours = dtTemp["NumeroCours"].ToString();
                    sSessionID = dtTemp["CoursOffertID"].ToString();

                    // First en-tête
                    //sRetString += WriteEntete(dtTemp["NumeroCours"].ToString(), dtTemp["NomCours"].ToString(), sSessionStartDate, sSessionEndDate);
                    do
                    {
                        sNewCours = dtTemp["NumeroCours"].ToString();
                        sSessionID = dtTemp["CoursOffertID"].ToString();

                        if (sSessionID != sOldSessionID)
                        {
                            // Nouvelle ligne : En tête pour le cours
                            //if (sOldCours != String.Empty)
                            {   // Ce n'est pas la premiere ligne
                                sRetString += String.Format("<TR><TD Colspan='6' width:'100%'><hr style='background-color:#669999;' size='1' width='100%'/></TD></TR>");
                                sRetString += String.Format("<TR><TD Colspan='6'></TD></TR>");

                                sRetString += "</TABLE>";
                                sRetString += String.Format("<div style=\'page-break-after:always;\'></div>");

                                // Nouvelle en-tête
                                sRetString += WriteEntete(dtTemp["NumeroCours"].ToString(), dtTemp["NomCours"].ToString(), sSessionStartDate, sSessionEndDate);
                            }
                            sRetString += String.Format("<TR><TD>{0}</TD><TD>{1}</TD><TD>{2}</TD><TD>{3}</TD><TD>{4}</TD><TD>{5}</TD></TR>",
                                dtTemp["Nom"].ToString(), dtTemp["Prenom"].ToString(), dtTemp["Email"].ToString(), dtTemp["Telephone1"].ToString(),
                                dtTemp["NIF"].ToString(), dtTemp["NoteSurCent"].ToString());

                            sOldSessionID = sSessionID;
                        }
                        else
                        {
                            // Pas d'entête; Juste Ecrire une ligne de détails ou Justifications
                            sRetString += String.Format("<TR><TD>{0}</TD><TD>{1}</TD><TD>{2}</TD><TD>{3}</TD><TD>{4}</TD><TD>{5}</TD></TR>",
                                dtTemp["Nom"].ToString(), dtTemp["Prenom"].ToString(), dtTemp["Email"].ToString(), dtTemp["Telephone1"].ToString(),
                                dtTemp["NIF"].ToString(), dtTemp["NoteSurCent"].ToString());
                        }

                    }
                    while (dtTemp.Read());
                }
                else
                {
                    sRetString += String.Format("<TR><TD Colspan='6'>Pas d'Information !!!!</TD></TR>");
                }
                db = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                sRetString += "<br> ERREUR - ERREUR - ERREUR !!!";
                db = null;
            }
        }
        sRetString += String.Format("<TR><TD Colspan='6' width:'100%'><hr style='background-color:#669999;' size='1' width='100%'/></TD></TR>");
        sRetString += String.Format("<TR><TD Colspan='6'><div style=\'page-break-after:always;\'></div></TD></TR>");

        sRetString += String.Format("<TR><TD Colspan='6' width:'100%'><hr style='background-color:#669999;' size='1' width='100%'/></TD></TR>");

        sRetString += "</TABLE>";
        return sRetString;
    }

    String FixDate(String sDate)
    {
        if (sDate.Trim() == String.Empty)
            return "";

        return sDate.Substring(0, sDate.IndexOf(" "));
    }

    String WriteEntete(String sNumeroCours, String sNomCours, DateTime sStartDate, DateTime sEndDate)
    {
        String sRetString = String.Format("<TABLE width=100%>");
        sRetString += String.Format("<TR><TD Colspan='6' style='width:100%;text-align:center;font-weight:bold;font-size:14px'>Université Espoir</TD></TR>");
        sRetString += String.Format("<TR><TD Colspan='6' style='width:100%;text-align:center;font-weight:bold;font-size:14px'>Liste des Etudiants Par Cours</TD></TR>");
        sRetString += String.Format("<TR><TD Colspan='6' style='width:100%;text-align:center;font-weight:bold;font-size:14px'>Session {0} - {1}</TD></TR>", sStartDate.ToString("dd-MMMM-yyyy"), sEndDate.ToString("dd-MMMM-yyyy"));
        sRetString += String.Format("<TR><TD Colspan='6' style='width:100%;text-align:center;font-weight:bold;font-size:14px'>{0} - {1}</TD></TR>", sNumeroCours, sNomCours);
        sRetString += String.Format("<TR><TD Colspan='6' width:'100%'><hr style='background-color:#669999;' size='3'/></TD></TR>");
        sRetString += String.Format("<TR style='font-weight:bold'><TD align='left' width='50px'>Nom</TD><TD align='left'>Prénom</TD><TD align='left'>Email/Courriel</TD><TD align='left'>Téléphone</TD>" +
            "<TD align='left'>NIF/Matricule</TD><TD align='left'>Note Finale</TD></TR>");
        sRetString += String.Format("<TR><TD Colspan='6' width:'100%'><hr style='background-color:#669999;' size='3'/></TD></TR>");

        return sRetString;
    }
}
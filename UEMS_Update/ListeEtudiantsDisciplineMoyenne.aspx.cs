using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;
public partial class ListeEtudiantsDisciplineMoyenne : System.Web.UI.Page
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
        String sDisciplineID = String.Empty, sOldDisciplineID;
        Int32 iCount = 0;

        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
        {
            try
            {
                sqlConn.Open();
                String sSql = String.Format("SELECT DISTINCT P.Prenom, P.Nom, P.PersonneID, P.Telephone1, P.email, P.DisciplineID, D.DisciplineNom " +
                     " FROM Personnes P, Disciplines D, CoursPris CP, CoursOfferts CO WHERE P.DisciplineID = D.DisciplineID  " +
                     " AND P.PersonneID = CP.PersonneID AND CP.CoursOffertID = CO.CoursOffertID AND CO.Actif = 1  AND P.Actif = 1 " +
                     " AND CO.NumeroCours NOT IN ('FRA001', 'ANG001', 'MAT001') " +
                     " ORDER BY P.DisciplineID, P.Nom, P.Prenom");

                SqlDataReader dtTemp = db.GetDataReader(sSql, sqlConn);

                if (dtTemp.Read())
                {
                    // First en-tête
                    sRetString += WriteEntete(dtTemp["DisciplineNom"].ToString());
                    //sDisciplineID = dtTemp["DisciplineID"].ToString();
                    sOldDisciplineID = sDisciplineID;
                    do
                    {
                        sDisciplineID = dtTemp["DisciplineID"].ToString();
                        if (sDisciplineID != sOldDisciplineID)
                        {
                            // Nouvelle ligne : En tête
                            if (sOldDisciplineID != String.Empty)
                            {   // Ce n'est pas la premiere ligne
                                sRetString += String.Format("<TR><TD Colspan='7' width:'100%'><hr style='background-color:#669999;' size='1' width='100%'/></TD></TR>");
                                sRetString += String.Format("<TR><TD Colspan='7'></TD></TR>");

                                sRetString += "</TABLE>";
                                sRetString += String.Format("<div style=\'page-break-after:always;\'></div>");

                                // Nouvelle en-tête
                                sRetString += WriteEntete(dtTemp["DisciplineNom"].ToString());
                            }
                            iCount = 1;
                            string moyenne_etudiant = db.GetMoyennePersonneID(dtTemp["PersonneID"].ToString()).ToString("F");
                            sRetString += String.Format("<TR><TD>{0}</TD><TD>{1}</TD><TD>{2}</TD><TD>{3}</TD><TD>{4}</TD><TD>{5}</TD><TD></TD></TR>", iCount,
                                dtTemp["Nom"].ToString(), dtTemp["Prenom"].ToString(), dtTemp["Email"].ToString(), dtTemp["Telephone1"].ToString(),
                                moyenne_etudiant);

                            sOldDisciplineID = sDisciplineID;
                        }
                        else
                        {
                            iCount += 1;
                            string moyenne_etudiant = db.GetMoyennePersonneID(dtTemp["PersonneID"].ToString()).ToString("F");
                            // Pas d'entête; Juste Ecrire une ligne de détails ou Justifications
                            sRetString += String.Format("<TR><TD>{0}</TD><TD>{1}</TD><TD>{2}</TD><TD>{3}</TD><TD>{4}</TD><TD>{5}</TD><TD></TD></TR>", iCount,
                                dtTemp["Nom"].ToString(), dtTemp["Prenom"].ToString(), dtTemp["Email"].ToString(), dtTemp["Telephone1"].ToString(),
                                moyenne_etudiant);
                        }

                    }
                    while (dtTemp.Read());
                }
                else
                {
                    sRetString += String.Format("<TR><TD Colspan='7'>Pas d'Information !!!!</TD></TR>");
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
        sRetString += String.Format("<TR><TD Colspan='7' width:'100%'><hr style='background-color:#669999;' size='1' width='100%'/></TD></TR>");
        sRetString += String.Format("<TR><TD Colspan='7'><div style=\'page-break-after:always;\'></div></TD></TR>");
        sRetString += String.Format("<TR><TD Colspan='7' width:'100%'><hr style='background-color:#669999;' size='1' width='100%'/></TD></TR>");
        sRetString += "</TABLE>";
        return sRetString;
    }

    String FixDate(String sDate)
    {
        return sDate.Substring(0, sDate.IndexOf(" "));
    }

    String WriteEntete(String sDiscipline)
    {
        String sRetString = String.Format("<TABLE width=100%>");
        sRetString += String.Format("<TR><TD Colspan='7' style='width:100%;text-align:center;font-weight:bold;font-size:14px'>Université Espoir</TD></TR>");
        sRetString += String.Format("<TR><TD Colspan='7' style='width:100%;text-align:center;font-weight:bold;font-size:14px'>Liste des Etudiants Par Cours</TD></TR>");
        sRetString += String.Format("<TR><TD Colspan='7' style='width:100%;text-align:center;font-weight:bold;font-size:14px'>Discipline: {0}</TD></TR>", sDiscipline);
        sRetString += String.Format("<TR><TD Colspan='7' width:'100%'><hr style='background-color:#669999;' size='3'/></TD></TR>");
        sRetString += String.Format("<TR style='font-weight:bold'><TD></TD><TD align='left' width='50px'>Nom</TD><TD align='left'>Prénom</TD><TD align='left'>Email/Courriel</TD><TD align='left'>Téléphone</TD>" +
            "<TD align='left'>Moyenne sur 100</TD><TD align='left'></TD></TR>");
        sRetString += String.Format("<TR><TD Colspan='7' width:'100%'><hr style='background-color:#669999;' size='3'/></TD></TR>");

        return sRetString;
    }
}
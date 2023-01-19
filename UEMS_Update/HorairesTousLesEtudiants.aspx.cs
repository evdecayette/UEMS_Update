using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;

public partial class HorairesTousLesEtudiants : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                string sSql = String.Format("SELECT CP.NumeroCours, CP.NotePassage, C.NomCours, C.Credits, P.EtudiantIdPlus,  " + 
                    " P.Nom, P.Prenom, H.Jours, H.HeureDebut, H.HeureFin " +
                    " FROM CoursPris CP, Cours C, Personnes P, Horaires H, CoursOfferts CO " +
                    " WHERE CP.NumeroCours = C.NumeroCours AND CP.PersonneID = P.PersonneID " +
                    " AND CP.CoursOffertID = CO.CoursOffertID AND H.HoraireID = CO.HoraireID " +
                    " AND P.Actif = 1 AND C.ExamenEntree = 0 AND C.Credits > 0 AND IsNull(CP.DateReussite, '') = ''  " +
                    " AND CP.CoursOffertID IN (SELECT CoursOffertID FROM CoursOfferts CO, Cours C  " + 
                    " WHERE CO.NumeroCours = C.NumeroCours AND CO.Actif = 1 AND Credits > 0) " +
                    " ORDER BY DateReussite, P.Nom, P.Prenom DESC");

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
        int creditsTotal = 0;

        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            try
            {
                sqlConn.Open();

                SqlDataReader dtTemp = db.GetDataReader(sSql, sqlConn);
                String EtudiantID_Old = "", EtudiantID_New = "";
                bool first_pass = true;
                if (dtTemp.Read())
                {
                    do
                    {
                        EtudiantID_New = dtTemp["EtudiantIdPlus"].ToString();
                        if (EtudiantID_New != EtudiantID_Old)
                        {
                            EtudiantID_Old = EtudiantID_New;
                            if (!first_pass)
                            {
                                sRetString += String.Format("<div style=\'page-break-after:always;\'></div>");
                                sRetString += String.Format("<TR><TD Colspan='6' width:'80%'><hr style='background-color:#669999;' size='2' width='100%'/></TD></TR>");
                                sRetString += String.Format("<TR><TD Colspan='3' style='width:80%;text-align:left;font-weight:bold;font-size:14px'></TD>");
                                sRetString += String.Format("<TD Colspan='2' style='width:80%;text-align:right;font-weight:bold;font-size:14px'>Nombre de Crédits :</TD>");
                                sRetString += String.Format("<TD style='width:80%;text-align:center;font-weight:bold;font-size:14px'>{0}</TD></TR>", creditsTotal);
                                sRetString += String.Format("<TD Colspan='6' width:'80%'><hr style='background-color:#669999;' size='2' width='100%'/></TD></TR>");
                                sRetString += "</TABLE>";
                                creditsTotal = 0;
                            }

                            sRetString += String.Format("<TABLE style='width:80%;align:center'>");
                            sRetString += String.Format("<TR><TD Colspan='6' style='width:80%;text-align:center;font-weight:bold;font-size:18px'>Université Espoir</TD></TR>");
                            sRetString += String.Format("<TR><TD Colspan='6' style='width:80%;text-align:center;font-weight:bold;font-size:18px'><u>Horaire des Cours - Session Courante</u><div></div><div style='font-color:red'>{0}({1})</div></TD></TR>",
                                dtTemp["Prenom"].ToString() + " " + dtTemp["Nom"].ToString().ToUpper() + " ", dtTemp["EtudiantIdPlus"].ToString());
                            sRetString += String.Format("<TR><TD Colspan='6' style='width:80%;text-align:center;font-weight:bold;font-size:14px'></TD></TR>");
                            sRetString += String.Format("<TR><TD Colspan='6' style='width:80%;text-align:center;font-weight:bold;font-size:14px'>Date d'Impression: {0}</TD></TR>", DateTime.Today.Date.ToString("dd-MMM-yyyy"));
                            sRetString += String.Format("<TR><TD Colspan='6' width:'80%'><hr style='background-color:#669999;' size='3'/></TD></TR>");
                            sRetString += String.Format("<TR><TD width:'25%' style='text-align:left;font-weight:bold;font-size:14px'>Nom du Cours</TD>" +
                                "<TD style='text-align:center;font-weight:bold;font-size:14px'>Numéro du Cours</TD>" +
                                "<TD style='text-align:center;font-weight:bold;font-size:14px'>Jour</TD>" +
                                "<TD style='text-align:center;font-weight:bold;font-size:14px'>Début (Heure)</TD>" +
                                "<TD width:'15%' style='text-align:center;font-weight:bold;font-size:14px'>Fin (Heure)</TD>" +
                                "<TD width:'12%' style='text-align:center;font-weight:bold;font-size:14px'>Crédits</TD></TR>");
                            sRetString += String.Format("<TR><TD Colspan='6' width:'80%'><hr style='background-color:#669999;' size='3'/></TD></TR>");
                            first_pass = false;
                        }

                        sRetString += String.Format("<TR><TD>&nbsp;&nbsp;&nbsp;&nbsp;{0}</TD><TD style='text-align:center;'>{1}</TD>" +
                        "<TD style='text-align:center;'>{2}</TD>" +
                        "<TD style='text-align:center;'>{3}</TD>" +
                        "<TD style='text-align:center;'>{4}</TD>" +
                        "<TD style='text-align:center;'>{5}</TD></TR>", dtTemp["NomCours"].ToString(), dtTemp["NumeroCours"].ToString(), dtTemp["Jours"].ToString(),
                        dtTemp["HeureDebut"].ToString(), dtTemp["HeureFin"].ToString(), dtTemp["Credits"].ToString());
                        creditsTotal += int.Parse(dtTemp["Credits"].ToString());
                    }
                    while (dtTemp.Read());
                    // Dernier Etudiant
                    sRetString += String.Format("<TR><TD Colspan='6' width:'80%'><hr style='background-color:#669999;' size='2' width='100%'/></TD></TR>");
                    sRetString += String.Format("<TR><TD Colspan='3' style='width:80%;text-align:left;font-weight:bold;font-size:14px'></TD>");
                    sRetString += String.Format("<TD Colspan='2' style='width:80%;text-align:right;font-weight:bold;font-size:14px'>Nombre de Crédits :</TD>");
                    sRetString += String.Format("<TD style='width:80%;text-align:center;font-weight:bold;font-size:14px'>{0}</TD></TR>", creditsTotal);
                    sRetString += String.Format("<TD Colspan='6' width:'80%'><hr style='background-color:#669999;' size='2' width='100%'/></TD></TR>");
                    sRetString += "</TABLE>";
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
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;

public partial class HoraireSessionCourante : System.Web.UI.Page
{
    //String sErrorMessage = "";
    String sPersonneID = "";
    String sSql = "";
    //HashSet<Notation> getNotesScheme = null;
    string ConnectionString = XCryptEngine.ConnectionStringEncryption.Decrypt(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                sPersonneID = Request.QueryString["PersonneID"];

                sSql = String.Format("SELECT CP.NumeroCours, CP.NotePassage, C.NomCours, C.Credits, P.EtudiantIdPlus, P.Nom, P.Prenom, H.Jours, H.HeureDebut, H.HeureFin " +
                    " FROM CoursPris CP, Cours C, Personnes P, Horaires H, CoursOfferts CO " +
                    " WHERE CP.NumeroCours = C.NumeroCours AND CP.PersonneID = P.PersonneID " +
                    " AND CP.CoursOffertID = CO.CoursOffertID AND H.HoraireID = CO.HoraireID AND CP.PersonneID = '{0}' " +
                    " AND C.ExamenEntree = 0 AND C.Credits > 0 AND IsNull(CP.DateReussite, '') = ''  " +
                    " AND CP.CoursOffertID IN (SELECT CoursOffertID FROM CoursOfferts CO, Cours C WHERE CO.NumeroCours = C.NumeroCours AND CO.Actif = 1 AND Credits > 0) " +
                    " ORDER BY DateReussite DESC", sPersonneID);

                litBody.Text = ProcessInfo(sSql, sPersonneID);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }

    String ProcessInfo(String sSql, String sPersonneID)
    {
        String sRetString = String.Format("<div style=\'page-break-after:always;\'></div>");    // Start with page break in order not to print the button 'print'
        int creditsTotal = 0;

        string sDisciplineDeclaree;
        using (SqlConnection sqlConn1 = new SqlConnection(ConnectionString))
        {
            try
            {
                DB_Access db1 = new DB_Access();
                sqlConn1.Open();
                sDisciplineDeclaree = db1.GetDisciplineEtudiant(sPersonneID, sqlConn1);
            }
            catch
            {
                sRetString += "<br> ERREUR 0 : ProcessInfo-Discipline!";
                return sRetString;
            }
        }

        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
        {
            try
            {
                sqlConn.Open();

                SqlDataReader dtTemp = db.GetDataReader(sSql, sqlConn);                

                if (dtTemp.Read())
                {
                    sRetString += String.Format("<TABLE style='width:80%;align:center'>");
                    sRetString += String.Format("<TR><TD Colspan='6' style='width:80%;text-align:center;font-weight:bold;font-size:14px'>Université Espoir</TD></TR>");
                    sRetString += String.Format("<TR><TD Colspan='6' style='width:80%;text-align:center;font-weight:bold;font-size:18px'><u>Horaire des Cours - Session Courante</u><div></div><div style='font-color:red'>{0}({1})</div></TD></TR>",
                        dtTemp["Prenom"].ToString() + " " + dtTemp["Nom"].ToString().ToUpper() + " ", dtTemp["EtudiantIdPlus"].ToString());
                    sRetString += String.Format("<TR><TD Colspan='6' style='width:80%;text-align:center;font-weight:bold;font-size:14px'>Discipline Déclarée : {0}</TD></TR>",
                        sDisciplineDeclaree);
                    sRetString += String.Format("<TR><TD Colspan='6' style='width:80%;text-align:center;font-weight:bold;font-size:14px'>Date d'Impression: {0}</TD></TR>", DateTime.Today.Date.ToString("dd-MMM-yyyy"));
                    sRetString += String.Format("<TR><TD Colspan='6' width:'80%'><hr style='background-color:#669999;' size='3'/></TD></TR>");
                    sRetString += String.Format("<TR><TD width:'25%' style='text-align:left;font-weight:bold;font-size:14px'>Nom du Cours</TD>" +
                        "<TD style='text-align:center;font-weight:bold;font-size:14px'>Numéro du Cours</TD>" +
                        "<TD style='text-align:center;font-weight:bold;font-size:14px'>Jour</TD>" +
                        "<TD style='text-align:center;font-weight:bold;font-size:14px'>Début (Heure)</TD>" +
                        "<TD width:'15%' style='text-align:center;font-weight:bold;font-size:14px'>Fin (Heure)</TD>" +
                        "<TD width:'12%' style='text-align:center;font-weight:bold;font-size:14px'>Crédits</TD></TR>");
                    sRetString += String.Format("<TR><TD Colspan='6' width:'80%'><hr style='background-color:#669999;' size='3'/></TD></TR>");

                    do
                    {
                        sRetString += String.Format("<TR><TD>&nbsp;&nbsp;&nbsp;&nbsp;{0}</TD><TD style='text-align:center;'>{1}</TD>" +
                        "<TD style='text-align:center;'>{2}</TD>" +
                        "<TD style='text-align:center;'>{3}</TD>" +
                        "<TD style='text-align:center;'>{4}</TD>" +
                        "<TD style='text-align:center;'>{5}</TD></TR>", dtTemp["NomCours"].ToString(), dtTemp["NumeroCours"].ToString(), dtTemp["Jours"].ToString(),
                        dtTemp["HeureDebut"].ToString(), dtTemp["HeureFin"].ToString(), dtTemp["Credits"].ToString());
                        creditsTotal += int.Parse(dtTemp["Credits"].ToString());
                    }
                    while (dtTemp.Read());
                }
                else
                {// Write an explicit message when the selected student is not registered for the current session(Decayette's Codes)
                    dtTemp.Close();
                    string sql =String.Format("Select Nom, Prenom from Personnes Where PersonneID = '{0}'",sPersonneID);
                    
                    String sNomComplet = "";
                    try
                    {
                        sqlConn.Open();
                        SqlDataReader dt = db.GetDataReader(sql, sqlConn);

                        if (dt.Read())
                        {
                            sNomComplet = dt["Prenom"].ToString() + " " + dt["Nom"].ToString().ToUpper();
                            do
                            {
                                Response.Write(string.Format("Etudiant {0} N'inscrit pas Dans la Session Courante...!", sNomComplet));
                                return sRetString;
                            }
                            while (dt.Read());
                        }
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine(ex);
                        sRetString += "<br> ERREUR 2 : ProcessInfo!";
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                sRetString += "<br> ERREUR 2 : ProcessInfo!";
                //db = null;
            }
        }

        sRetString += String.Format("<TR><TD Colspan='6' width:'80%'><hr style='background-color:#669999;' size='2' width='100%'/></TD></TR>");
        sRetString += String.Format("<TR><TD Colspan='3' style='width:80%;text-align:left;font-weight:bold;font-size:14px'></TD>");
        sRetString += String.Format("<TD Colspan='2' style='width:80%;text-align:right;font-weight:bold;font-size:14px'>Nombre de Crédits :</TD>");
        sRetString += String.Format("<TD style='width:80%;text-align:center;font-weight:bold;font-size:14px'>{0}</TD></TR>", creditsTotal);
        sRetString += String.Format("<TD Colspan='6' width:'80%'><hr style='background-color:#669999;' size='2' width='100%'/></TD></TR>");

        sRetString += "</TABLE>";
        return sRetString;
    }

}
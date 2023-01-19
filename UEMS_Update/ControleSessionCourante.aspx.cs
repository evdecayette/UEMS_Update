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

public partial class ControleSessionCourante : System.Web.UI.Page
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
        Double dMontantTotal = 0.0;
        Double FraisParCours = 0.0;
        Double FraisEntree = 0.0;
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            try
            {
                sqlConn.Open();
                FraisParCours = int.Parse(ConfigurationManager.AppSettings["NombreDeMoisParSession"].ToString()) *
                    Double.Parse(db.GetMontantObligation(ConfigurationManager.AppSettings["CodeFraisMensuel"].ToString(), sqlConn));
                FraisEntree = Double.Parse(db.GetMontantObligation(ConfigurationManager.AppSettings["CodeFraisEntree"].ToString(), sqlConn));

                String sSql = String.Format("SELECT DISTINCT P.Prenom, P.Nom, P.NIF, P.Telephone1, P.email, P.PersonneID, P.DDN, " +
                    " P.EtudiantID, COUNT(P.PersonneID) AS NombreDeCours FROM Personnes P, CoursPris CP, CoursOfferts CO " +
                    " WHERE P.PersonneID = CP.PersonneID AND CP.CoursOffertID = CO.CoursOffertID AND CO.Actif = 1 " +
                    " AND CO.NumeroCours NOT IN ('FRA001', 'ANG001', 'MAT001') " +
                    " GROUP BY P.Prenom, P.Nom, P.NIF, P.Telephone1, P.email, P.PersonneID, P.DDN, P.EtudiantID " +
                    " ORDER BY P.Nom, P.Prenom");

                SqlDataReader dtTemp = db.GetDataReader(sSql, sqlConn);

                sRetString += String.Format("<TABLE width=100%>");
                sRetString += String.Format("<TR><TD Colspan='9' style='width:100%;text-align:center;font-weight:bold;font-size:14px'>Université Espoir</TD></TR>");
                sRetString += String.Format("<TR><TD Colspan='9' style='width:100%;text-align:center;font-weight:bold;font-size:14px'>Rapport Montants Dus - Session Courante</TD></TR>");
                sRetString += String.Format("<TR><TD Colspan='9' style='width:100%;text-align:center;font-weight:bold;font-size:14px'>Date de ce Rapport: {0}</TD></TR>", DateTime.Today.ToString("dd-MMM-yyyy"));

                sRetString += String.Format("<TR><TD Colspan='9' width:'100%'><hr style='background-color:#669999;' size='3'/></TD></TR>");
                sRetString += String.Format("<TR style='font-weight:bold'><TD># Etudiant</TD><TD align='left'>Nom</TD><TD align='left'>Prénom</TD>" +
                    "<TD align='left'>Phone</TD><TD align='left'>Courriel</TD><TD align='left'>NIF</TD><TD align='left'>DDN</TD><TD># de Cours</TD><TD>Montant Du</TD></TR>");
                sRetString += String.Format("<TR><TD Colspan='9' width:'100%'><hr style='background-color:#669999;' size='3'/></TD></TR>");

                if (dtTemp.Read())
                {
                    Double dMontant = 0.0;
                    do
                    {
                        dMontant = Double.Parse(dtTemp["NombreDeCours"].ToString()) * FraisParCours + FraisEntree;
                        dMontantTotal += dMontant;
                        sRetString += String.Format("<TR><TD>{0}</TD><TD>{1}</TD><TD>{2}</TD><TD>{3}</TD><TD>{4}</TD><TD>{5}</TD><TD>{6}</TD><TD>{7}</TD><TD align='right'>{8}</TD></TR>",
                              dtTemp["EtudiantID"].ToString(), dtTemp["Nom"].ToString(), dtTemp["Prenom"].ToString(), dtTemp["telephone1"].ToString(),
                              dtTemp["email"].ToString(), dtTemp["nif"].ToString(), dtTemp["DDN"].ToString(), dtTemp["NombreDeCours"].ToString(),
                              dMontant.ToString("F"));
                    }
                    while (dtTemp.Read());
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
        sRetString += String.Format("<TR><TD Colspan='9' width:'100%'><hr style='background-color:#669999;' size='2' width='100%'/></TD></TR>");
        sRetString += String.Format("<TR><TD Colspan='8' width:'100%' align='right'><b>TOTAL :</b></TD><TD align='right'><b>{0}</b></TD></TR>", dMontantTotal.ToString("F"));
        sRetString += String.Format("<TR><TD Colspan='9' width:'100%'><hr style='background-color:#669999;' size='2' width='100%'/></TD></TR>");

        sRetString += "</TABLE>";
        return sRetString;
    }

    String FixDate(String sDate)
    {
        if (sDate.Trim() == String.Empty)
            return "";

        return sDate.Substring(0, sDate.IndexOf(" "));
    }
}
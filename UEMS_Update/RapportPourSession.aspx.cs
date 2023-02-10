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

public partial class RapportPourSession : System.Web.UI.Page
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
        Double dMontantTotal = 0.0;
        String sDateTo = "", sDateFrom = "";

        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
        {
            try
            {
                sqlConn.Open();
                String sSessionID = Request.QueryString["SessionID"];

                if (sSessionID.Trim() == String.Empty)
                {
                    return "Erreur : Username !!!";
                }

                using (SqlConnection sqlConn1 = new SqlConnection(ConnectionString))
                {
                    String sSql1 = String.Format("SELECT * FROM LesSessions WHERE SessionID = {0}", sSessionID);
                    sqlConn1.Open();
                    SqlDataReader dtTemp1 = db.GetDataReader(sSql1, sqlConn1);
                    if (dtTemp1.Read())
                    {
                        sDateFrom = dtTemp1["SessionDateDebut"].ToString();
                        sDateTo = dtTemp1["SessionDateFin"].ToString();

                        sDateFrom = sDateFrom.Substring(0, sDateFrom.IndexOf(" "));
                        sDateTo = sDateTo.Substring(0, sDateTo.IndexOf(" "));
                    }
                }

                String sSql = String.Format("SELECT M.RecuParUserName, P.Nom, P.Prenom, M.CodeObligation, M.DateMontant, M.NumeroRecu, " +
                    " M.Montant FROM MontantsRecus M, Personnes P " +
                    " WHERE M.PersonneID = P.PersonneID AND M.DateMontant >= '{0}' AND M.DateMontant <= '{1}'", sDateFrom, sDateTo);

                SqlDataReader dtTemp = db.GetDataReader(sSql, sqlConn);

                sRetString += String.Format("<TABLE width=100%>");
                sRetString += String.Format("<TR><TD Colspan='7' style='width:100%;text-align:center;font-weight:bold;font-size:14px'>Université Espoir</TD></TR>");
                sRetString += String.Format("<TR><TD Colspan='7' style='width:100%;text-align:center;font-weight:bold;font-size:14px'>Rapport d'Entrees de Fonds d'Une Session</TD></TR>");
                sRetString += String.Format("<TR><TD Colspan='7' style='width:100%;text-align:center;font-weight:bold;font-size:14px'>Dates: {0} - {1}</TD></TR>", sDateFrom, sDateTo);

                sRetString += String.Format("<TR><TD Colspan='7' width:'100%'><hr style='background-color:#669999;' size='3'/></TD></TR>");
                sRetString += String.Format("<TR style='font-weight:bold'><TD align='left' width='50px'>Utilisateur</TD><TD align='left' width='50px'>Nom</TD><TD align='left'>Prénom</TD>" +
                    "<TD align='left'>Code</TD><TD align='left'>Date</TD><TD align='left'>Numéro Reçu</TD><TD>Montant</TD></TR>");
                sRetString += String.Format("<TR><TD Colspan='7' width:'100%'><hr style='background-color:#669999;' size='3'/></TD></TR>");

                if (dtTemp.Read())
                {
                    Double dMontant = 0.0;
                    do
                    {
                        dMontant = Double.Parse(dtTemp["Montant"].ToString());
                        dMontantTotal += dMontant;
                        sRetString += String.Format("<TR><TD>{0}</TD><TD>{1}</TD><TD>{2}</TD><TD>{3}</TD><TD>{4}</TD><TD>{5}</TD><TD align='right'>{6}</TD></TR>",
                              dtTemp["RecuParUsername"].ToString(), dtTemp["Nom"].ToString(), dtTemp["Prenom"].ToString(), dtTemp["CodeObligation"].ToString(),
                              dtTemp["DateMontant"].ToString(), dtTemp["NumeroRecu"].ToString(), dMontant.ToString("F"));
                    }
                    while (dtTemp.Read());
                }
                db = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                sRetString += "<br> ERREUR - ERREUR - ERREUR !!!";
            }
        }
        db = null;

        sRetString += String.Format("<TR><TD Colspan='7' width:'100%'><hr style='background-color:#669999;' size='2' width='100%'/></TD></TR>");
        sRetString += String.Format("<TR><TD Colspan='6' width:'100%' align='right'><b>TOTAL :</b></TD><TD align='right'><b>{0}</b></TD></TR>", dMontantTotal.ToString("F"));
        sRetString += String.Format("<TR><TD Colspan='7' width:'100%'><hr style='background-color:#669999;' size='2' width='100%'/></TD></TR>");

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
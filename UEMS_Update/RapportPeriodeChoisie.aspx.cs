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

public partial class RapportPeriodeChoisie : System.Web.UI.Page
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
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            try
            {
                sqlConn.Open();

                String sDateFrom = Request.Cookies.Get("DateFrom").Value.ToString();
                String sDateTo = Request.Cookies.Get("DateTo").Value.ToString();

                if (sDateFrom.Trim() == String.Empty || sDateTo.Trim() == String.Empty)
                {
                    return "Erreur : Dates Manquantes !!!";
                }

                String sSql = String.Format("SELECT RecuParUserName, P.Nom, P.Prenom, M.CodeObligation, M.DateMontant, M.NumeroRecu, " +
                    " M.Montant FROM MontantsRecus M, Personnes P " +
                    " WHERE M.PersonneID = P.PersonneID AND DateMontant >= '{0}' AND DateMontant <= '{1}'",
                    sDateFrom, sDateTo);

                SqlDataReader dtTemp = db.GetDataReader(sSql, sqlConn);

                sRetString += String.Format("<TABLE width=100%>");
                sRetString += String.Format("<TR><TD Colspan='7' style='width:100%;text-align:center;font-weight:bold;font-size:14px'>Université Espoir</TD></TR>");
                sRetString += String.Format("<TR><TD Colspan='7' style='width:100%;text-align:center;font-weight:bold;font-size:14px'>Rapport d'Entrees de Fonds</TD></TR>");
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
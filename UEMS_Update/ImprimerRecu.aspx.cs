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

public partial class ImprimerRecu : System.Web.UI.Page
{
    String sMontantRecuID = String.Empty;
    String sPersonneID = String.Empty;
    string ConnectionString = XCryptEngine.ConnectionStringEncryption.Decrypt(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Get PersonneID 
            try
            {
                sMontantRecuID = Request.QueryString["MontantRecuID"];
                Request.QueryString["MontantRecuID"].Remove(0);
                
                sPersonneID = Request.QueryString["PersonneID"];
                Request.QueryString["PersonneID"].Remove(0);
            }
            catch (Exception Excep)
            {
                Debug.WriteLine(Excep.Message);
            }

            //
            if (sMontantRecuID != String.Empty)
            {
                litBody.Text = ProcessInfo();
            }
        }
    }

    String ProcessInfo()
    {
        String sRetString = String.Empty;
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
        {
            sRetString += String.Format("<table style='font-size:14px;padding:15px'>");

            try
            {
                //SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);

                String sSql = String.Format("SELECT P.Nom, P.Prenom, P.NIF, P.Telephone1, M.Montant, M.DateMontant, O.Description,  M.MontantRecuID, M.NumeroRecu FROM MontantsRecus M, Personnes P, Obligations O " +
                    " WHERE M.PersonneID = P.PersonneID AND M.CodeObligation = O.Code AND M.MontantRecuID = {0} AND M.PersonneID = '{1}'", sMontantRecuID, sPersonneID);

                SqlDataReader dtTemp = db.GetDataReader(sSql, sqlConn);

                if (dtTemp.Read())
                {
                    sRetString += String.Format("<TR><TD style='font-weight:bold;width:40%'>Numéro du Reçu</TD><TD align='left'>{0}</TD></TR>", dtTemp["NumeroRecu"].ToString());
                    sRetString += String.Format("<TR><TD colspan='2'></TD></TR>");
                    sRetString += String.Format("<TR><TD style='font-weight:bold;'>Nom</TD><TD align='left'>{0}</TD></TR>", dtTemp["Nom"].ToString());
                    sRetString += String.Format("<TR><TD colspan='2'></TD></TR>");
                    sRetString += String.Format("<TR><TD style='font-weight:bold;'>Prénom</TD><TD align='left'>{0}</TD></TR>", dtTemp["Prenom"].ToString());
                    sRetString += String.Format("<TR><TD colspan='2'></TD></TR>");
                    sRetString += String.Format("<TR><TD style='font-weight:bold;'>NIF/Matricule</TD><TD align='left'>{0}</TD></TR>", dtTemp["NIF"].ToString());
                    sRetString += String.Format("<TR><TD colspan='2'></TD></TR>");
                    sRetString += String.Format("<TR><TD style='font-weight:bold;'>Montant</TD><TD align='left'>{0}</TD></TR>", Double.Parse(dtTemp["Montant"].ToString()).ToString("F"));
                    sRetString += String.Format("<TR><TD colspan='2'></TD></TR>");
                    sRetString += String.Format("<TR><TD style='font-weight:bold;'>Date</TD><TD align='left'>{0}</TD></TR>", dtTemp["DateMontant"].ToString());
                    sRetString += String.Format("<TR><TD colspan='2'></TD></TR>");
                    sRetString += String.Format("<TR><TD style='font-weight:bold;'>Description</TD><TD align='left'>{0}</TD></TR>", dtTemp["Description"].ToString());
                }
                else
                {
                    sRetString += String.Format("<TR><TD>Erreur ! Erreur ! Erreur!</TD></TR>");
                }
                db = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                sRetString += "<TR><TD>Erreur ! Erreur ! (Voir Un Tech) !</TD></TR>";
                db = null;
            }
        }
        sRetString += "</TABLE>";

        return sRetString;
    }

    String FixDate(String sDate)
    {
        return sDate.Substring(0, sDate.IndexOf(" "));
    }

}
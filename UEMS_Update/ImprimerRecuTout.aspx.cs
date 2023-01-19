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

public partial class ImprimerRecuTout : System.Web.UI.Page
{
    String sPersonneID = String.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Get PersonneID 
            try
            {
                sPersonneID = Request.QueryString["PersonneID"];
                Request.QueryString["PersonneID"] = "";
            }
            catch (Exception Excep)
            {
                Debug.WriteLine(Excep.Message);
            }

            //
            if (sPersonneID != String.Empty)
            {
                litBody.Text = ProcessInfo();
            }
        }
    }

    String ProcessInfo()
    {
        String sRetString = String.Empty;
        Boolean bFirstPass = true;
        Double dTotal = 0.0;
        sRetString += String.Format("<div style='border: 1px solid; border-radius: 20px;padding:15px'>");   // Make round circle for Nom, Prenom
        sRetString += String.Format("<table style='font-size:14px;padding:15px;width:100%'>");

        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {            
            try
            {
                sqlConn.Open();
                //SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);

                String sSql = String.Format("SELECT P.Nom, P.Prenom, P.NIF, P.Telephone1, M.Montant, M.DateMontant, O.Description,  M.MontantRecuID, M.NumeroRecu FROM MontantsRecus M, Personnes P, Obligations O " +
                    " WHERE M.PersonneID = P.PersonneID AND M.CodeObligation = O.Code AND M.PersonneID = '{0}'", sPersonneID);

                SqlDataReader dtTemp = db.GetDataReader(sSql, sqlConn);

                while (dtTemp.Read())
                {
                    if (bFirstPass)
                    {
                        bFirstPass = false;
                        // Information sur l'Étudiant
                        sRetString += String.Format("<TR><TD style='font-weight:bold;width:20%;'>Nom</TD><TD>{0}</TD></TR>", dtTemp["Nom"].ToString());
                        sRetString += String.Format("<TR><TD colspan='2'></TD></TR>");
                        sRetString += String.Format("<TR><TD style='font-weight:bold;'>Prénom</TD><TD>{0}</TD></TR>", dtTemp["Prenom"].ToString());
                        sRetString += String.Format("<TR><TD colspan='2'></TD></TR>");
                        sRetString += String.Format("<TR><TD style='font-weight:bold;'>NIF/Matricule</TD><TD>{0}</TD></TR>", dtTemp["NIF"].ToString());
                        sRetString += String.Format("<TR><TD colspan='2'></TD></TR>");

                        // Close this table and open new one
                        sRetString += "</TABLE>";
                        sRetString += String.Format("</div>");
                        sRetString += String.Format("<div style='height:50px;'><br/></div>");

                        sRetString += String.Format("<table class='table table-striped table-bordered table-hover'>");
                        // Premiere ligne de reçus
                        sRetString += String.Format("<thead><th width:30%'>Numéro Reçu</th><th>Date</th><th width:30%'>Description</th><th>Montant (HTG)</th></thead><tbody>");
                        sRetString += String.Format("<TR class='odd gradeX'><TD>{0}</TD>", dtTemp["NumeroRecu"].ToString());
                        sRetString += String.Format("<TD>{0}</TD>", FixDate(dtTemp["DateMontant"].ToString()));
                        sRetString += String.Format("<TD>{0}</TD>", dtTemp["Description"].ToString());
                        sRetString += String.Format("<TD align='right'>{0}</TD></TR>", Double.Parse(dtTemp["Montant"].ToString()).ToString("F"));
                    }
                    else
                    {
                        sRetString += String.Format("<TR class='odd gradeX'><TD>{0}</TD>", dtTemp["NumeroRecu"].ToString());
                        sRetString += String.Format("<TD>{0}</TD>", FixDate(dtTemp["DateMontant"].ToString()));
                        sRetString += String.Format("<TD>{0}</TD>", dtTemp["Description"].ToString());
                        sRetString += String.Format("<TD align='right'>{0}</TD></TR>", Double.Parse(dtTemp["Montant"].ToString()).ToString("F"));
                        dTotal += Double.Parse(dtTemp["Montant"].ToString());
                    }
                }
                if (!bFirstPass) // we went inside loop
                {
                    // Total

                    EtudiantInfo EI = new EtudiantInfo();
                    EI.GetInfoEtudiant(sPersonneID);
                    sRetString += String.Format("<TR class='odd gradeX'><TD Colspan='3' align='right' style='font-weight:bold;'>TOTAL</TD>");
                    sRetString += String.Format("<TD align='right' style='font-weight:bold;'>{0}</TD></TR>", dTotal.ToString("F"));

                    sRetString += String.Format("<TR class='odd gradeX'><TD Colspan='3' align='right' style='font-weight:bold;'>Balance Due</TD>");
                    sRetString += String.Format("<TD align='right' style='font-weight:bold;font-color:red;'>{0}</TD></TR>", EI.BalanceDue.ToString("F"));

                    EI = null;

                    sRetString += @"</tbody>";
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
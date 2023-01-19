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

public partial class ImprimerTousLesReleves : System.Web.UI.Page
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
        String ancienneValeur = String.Empty, nouvelleValeur = String.Empty;
        Boolean premierEtudiant = true;

        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            try
            {
                sqlConn.Open();
                String sSql = String.Format("SELECT CP.NumeroCours, CP.NoteSurCent, C.NomCours, P.Nom, P.Prenom, ISNULL(NIF, '') AS NIF, " +
                    " ISNULL(CONVERT(nvarchar, CP.DateReussite), '') AS DateReussite, P.PersonneID " +
                    " FROM Cours C, CoursPris CP, Personnes P " +
                    " WHERE C.NumeroCours = CP.NumeroCours AND CP.PersonneID = P.PersonneID ORDER BY Nom, Prenom, CP.NumeroCours");

                SqlDataReader dtTemp = db.GetDataReader(sSql, sqlConn);
                while (dtTemp.Read())
                {
                    nouvelleValeur = dtTemp["PersonneID"].ToString();
                    if (ancienneValeur != nouvelleValeur)
                    {
                        if (!premierEtudiant)
                        {
                            sRetString += "</TABLE>";
                            sRetString += "</div>";
                            sRetString += String.Format("<div style=\'page-break-after:always;\'></div>");
                        }
                        premierEtudiant = false;
                        sRetString += Entete(dtTemp["Nom"].ToString(), dtTemp["Prenom"].ToString(), dtTemp["NIF"].ToString());
                        ancienneValeur = nouvelleValeur;
                    }

                    sRetString += String.Format("<TR><TD style='font-weight:bold;'>{0}</TD><TD>{1}</TD><TD>{2}</TD><TD>{3}</TD></TR>",
                        dtTemp["NumeroCours"].ToString(), dtTemp["NomCours"].ToString(), dtTemp["NoteSurCent"].ToString(), dtTemp["DateReussite"].ToString());
                }
                sRetString += "</TABLE>";
                sRetString += "</div>";

                db = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                sRetString += "<TR><TD>Erreur ! Erreur ! (Voir Un Tech) !</TD></TR>";

            }
        }
        db = null; 
        
        return sRetString;
    }

    String Entete(String sNom, String sPrenom, String sNIF)
    {
        String sRetString = String.Empty;
        
        sRetString += String.Format("<p style='text-align:center;font-size:20px'>Université Espoir</p>");
        sRetString += String.Format("<p style='text-align:center;'><img src='images/logo.jpg' alt='Logo UEspoir'></p>");
        sRetString += String.Format("<p style='text-align:center;font-size:14px'>Relevé de notes NON Officiel</p>");
        sRetString += String.Format("<div style='border: 1px solid; border-radius: 20px;'>");
        sRetString += String.Format("<table style='font-size:14px;padding:15px'>");
        sRetString += String.Format("<TR><TD style='font-weight:bold;'>Nom</TD><TD align='left'>{0}</TD></TR>", sNom);
        sRetString += String.Format("<TR><TD style='font-weight:bold;'>Prénom</TD><TD align='left'>{0}</TD></TR>", sPrenom);
        sRetString += String.Format("<TR><TD style='font-weight:bold;'>NIF/Matricule</TD><TD align='left'>{0}</TD></TR>", sNIF);
        //sRetString += String.Format("<TR><TD colspan='4'></TD></TR>");
        sRetString += "</table>";
        sRetString += "</div>";
        sRetString += "<br/>";

        sRetString += String.Format("<div style='border: 1px solid; border-radius: 20px;'>");
        sRetString += String.Format("<table style='font-size:14px;padding:15px'>");
        sRetString += String.Format("<TR><TD style='font-weight:bold;width:20%'>Numéro Cours</TD><TD style='font-weight:bold;width:40%'>Description</TD><TD style='font-weight:bold;width:20%'>Note sur Cent</TD><TD style='font-weight:bold;width:20%'>Date Réussite</TD></TR>");

        return sRetString;
    }

    String FixDate(String sDate)
    {
        if (sDate != "")
        {
            return sDate.Substring(0, sDate.IndexOf(" "));
        }
        else
            return sDate;
    }

}





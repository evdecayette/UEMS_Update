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

public partial class RecevoirPaiements : System.Web.UI.Page
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
                Debug.WriteLine(sPersonneID);
                lblEtudiantInfo.ForeColor = System.Drawing.Color.Blue;
                lblEtudiantInfo.Font.Bold = true;
            }
            catch (Exception Excep)
            {
                Debug.WriteLine(Excep.Message);
            }

            //
            if (sPersonneID != String.Empty)
            {
                txtPersonneID.Text = sPersonneID;
                // Build Strings
                lblEtudiantInfo.Text = InfoEtudiant();
                LitPaiement.Text = BuildPaiementsDus();
                LitDejaPaye.Text = BuildPaiementsRecus();
                RemplirListeDesObligations();
            }
            else
            {
                lblError.Text = "ERREUR: Voir un technicien";
            }
        }
    }

    string InfoEtudiant()
    {
        String returnedString = String.Empty;
        try
        {
            EtudiantInfo EI = new EtudiantInfo();
            EI.GetInfoEtudiant(txtPersonneID.Text);
            returnedString = EI.Prenom + " " + EI.Nom + " (Discipline: " + EI.Discipline + ")";
            lblBalance.Text = EI.BalanceDue.ToString("F");
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
        }
        return returnedString;
    }

    string BuildPaiementsRecus()
    {
        String returnedString = String.Empty;
        String localPersonneID = txtPersonneID.Text;
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            returnedString += String.Format("<table class='table table-striped table-bordered table-hover'>");
            returnedString += String.Format("<thead><tr><th><a href='ImprimerRecuTout.aspx?PersonneID={0}'>Imprimer Tout</a></th><th>Description</th><th>Montant</th><th>Date</th></thead><tbody>", localPersonneID);

            // Loop through all records
            try
            {
                sqlConn.Open();
                string sSql = String.Format("SELECT O.Description, MR.Montant, DateMontant, MR.MontantRecuID, MR.DateCreee FROM MontantsRecus MR, Obligations O " +
                    " WHERE MR.CodeObligation = O.Code AND PersonneID = '{0}' ORDER BY DateMontant DESC", localPersonneID);

                SqlDataReader dt = db.GetDataReader(sSql, sqlConn);
                DateTime DateCreee;
                if (dt != null)
                {
                    while (dt.Read())
                    {
                        returnedString += String.Format(@"<tr class='odd gradeX'>");        //1 Row for each Course
                        returnedString += String.Format(@"<td><a href='ImprimerRecu.aspx?MontantRecuID={0}&PersonneID={1}'><img src='images/print.png' style='width:25px;height:25px;'></a></td>" +
                            "<td class='right'>{2}</td><td class='right'>{3}</td><td>{4}</td>", dt["MontantRecuID"].ToString(), localPersonneID, dt["Description"].ToString(),
                            Double.Parse(dt["Montant"].ToString()).ToString("F"), FixDate(dt["DateMontant"].ToString()));

                        DateCreee = DateTime.Parse(dt["DateCreee"].ToString());

                        if (DateTime.Today.Year == DateCreee.Year && DateTime.Today.Month == DateCreee.Month && DateTime.Today.Day == DateCreee.Day)
                        {
                            returnedString += String.Format(@"<td><a href='EffacerMontantRecu.aspx?MontantRecuID={0}&PersonneID={1}'>Effacer</a></td>",
                                dt["MontantRecuID"].ToString(), localPersonneID);
                        }
                        else
                        {
                            returnedString += String.Format(@"<td></td>");
                        }
                        returnedString += String.Format(@"</tr>");
                    }
                }

            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                db = null;
            }
        }
        returnedString += @"</tbody></table>";
        db = null;
        return returnedString;
    }

    string BuildPaiementsDus()
    {
        String returnedString = String.Empty;
        returnedString += String.Format("<table class='table table-striped table-bordered table-hover'>");
        returnedString += @"<thead><tr><th>Description</th><th>Montant</th><th>Date</th></thead><tbody>";

        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            try
            {
                sqlConn.Open();
                string sSql = String.Format("SELECT O.Description, MD.Montant, DateMontant FROM MontantsDus MD, Obligations O WHERE MD.CodeObligation = O.Code AND PersonneID = '{0}' ORDER BY DateMontant DESC", txtPersonneID.Text);

                SqlDataReader dt = db.GetDataReader(sSql, sqlConn);
                if (dt != null)
                {
                    while (dt.Read())
                    {
                        returnedString += String.Format(@"<tr class='odd gradeX'>");        //1 Row for each Course
                        returnedString += String.Format(@"<td>{0}</td><td class='right'>{1}</td><td class='right'>{2}</td>", dt["Description"].ToString(),
                            Double.Parse(dt["Montant"].ToString()).ToString("F"), FixDate(dt["DateMontant"].ToString()));
                        returnedString += String.Format(@"</tr>");
                    }
                }
                db = null;
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;

            }
        }
        returnedString += @"</tbody></table>";
        db = null;
        return returnedString;
    }

    void RemplirListeDesObligations()
    {
        try
        {
            DB_Access db = new DB_Access();
            using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
            {
                sqlConn.Open();
                String sSql = @"SELECT 'Choisissez une Description' AS Description, 'AAA' AS Code UNION " +
                " SELECT Description, Code FROM Obligations ORDER BY Code, Description";

                SqlDataAdapter da = new SqlDataAdapter(sSql, sqlConn);
                DataTable dTable = new DataTable();
                da.Fill(dTable);

                ddlObigations.DataSource = dTable;
                ddlObigations.DataValueField = "Code";
                ddlObigations.DataTextField = "Description";
                ddlObigations.DataBind();
            }
        }
        catch (Exception ex)
        {
            lblError.Text = "ERREUR: " + ex.Message;
        }
    }
    protected void btnSauvegarder_Click(object sender, EventArgs e)
    {
        lblError.Text = "";

        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            try
            {
                sqlConn.Open();
                if (ddlObigations.SelectedValue == "AAA")
                {
                    lblError.Text = "Choisissez une Description!";
                    return;
                }

                if (txtMontant.Text.Trim() == String.Empty || !db.ChiffreSeulement(txtMontant.Text.Trim()))
                {
                    lblError.Text = "Montant: Format Invalide or Vide!";
                    return;
                }

                if (!db.EstQueDateOK(txtDate.Text.Trim()))
                {
                    lblError.Text = "Date: Invalide!";
                    return;
                }
                sPersonneID = txtPersonneID.Text;

                // Tout bagay ok,
                // Si Facture <> FM, FI, FE alors Creer Record dans MontantsDus
                String FI = ConfigurationManager.AppSettings["CodeFraisInscription"].ToString();
                String FE = ConfigurationManager.AppSettings["CodeFraisEntree"].ToString();
                String FM = ConfigurationManager.AppSettings["CodeFraisMensuel"].ToString();
                String ObligationLabel = ddlObigations.SelectedValue.ToString();
                if (ObligationLabel != FI && ObligationLabel != FE && ObligationLabel != FM)
                {
                    db.FacturerPourObligation(ObligationLabel, sPersonneID, sqlConn);
                }

                String sSql = String.Format("INSERT INTO MontantsRecus (PersonneID, Montant, DateMontant, CodeObligation, RecuParUserName) VALUES ('{0}', {1}, '{2}', '{3}', '{4}')",
                     sPersonneID, txtMontant.Text.Trim(), txtDate.Text.Trim(), ObligationLabel, db.GetWindowsUser());
                try
                {
                    SqlCommand myCommand = new SqlCommand(sSql, sqlConn);
                    SqlCommand SqlCmdNewID = new SqlCommand("SELECT @@IDENTITY", sqlConn);
                    myCommand.ExecuteNonQuery();
                    String MontantRecuID = SqlCmdNewID.ExecuteScalar().ToString();

                    // Reset ecran
                    LitPaiement.Text = BuildPaiementsDus();
                    LitDejaPaye.Text = BuildPaiementsRecus();
                    lblBalance.Text = db.BalanceEtudiant(txtPersonneID.Text, sqlConn).ToString("F");
                    ClearObligationInfo();
                    //   ImprimerRecu(MontantRecuID, txtPersonneID.Text);
                    Response.Redirect("RecevoirPaiements.aspx?PersonneID=" + sPersonneID);
                }
                catch (Exception ex)
                {
                    lblError.Text = ex.Message;
                }
            }
            catch
            {
                lblError.Text = "ERREUR ...";
            }
        }
        db = null;
    }

    void ClearObligationInfo()
    {
        // Clear info
        lblError.Text = "";
        //txtDate.Text = String.Empty;
        ddlObigations.ClearSelection();
        txtMontant.Text = String.Empty;
        txtMontant.Focus();
    }

    void ImprimerRecu(String MontantRecuID, String PersonneID)
    {
        Response.Redirect(String.Format("ImprimerRecu.aspx?MontantRecuID={0}&PersonneID={1}", MontantRecuID, PersonneID));
    }

    String FixDate(String sDate)
    {
        return sDate.Substring(0, sDate.IndexOf(" "));
    }
    protected void ddlObigations_SelectedIndexChanged(object sender, EventArgs e)
    {
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            try
            {
                sqlConn.Open();
                txtMontant.Text = db.GetMontantObligation(ddlObigations.SelectedValue.ToString(), sqlConn);
            }
            catch
            {
                lblError.Text = "ERREUR ...";
            }
        }
        db = null;
    }
    protected void btnBackToInfoEtudiant_Click(object sender, EventArgs e)
    {
        Response.Redirect("InfoEtudiant.aspx?PersonneID=" + txtPersonneID.Text.ToString());
    }
}
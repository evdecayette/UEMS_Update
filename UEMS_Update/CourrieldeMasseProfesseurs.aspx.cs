using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Text;

public partial class CourrieldeMasseProfesseurs : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            String sGroup = ConfigurationManager.AppSettings["AdminGroup"].ToString();
            FillListWithProfesseurs(true);

        }
    }

    protected void FillListWithProfesseurs(bool bYesNo)
    {
        lblError.Text = String.Empty;

        SqlDataAdapter da;
        DataTable dTable;
        if (bYesNo)
        {
            try
            {
                SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
                String sSql = string.Format("SELECT ProfesseurID, UPPER(Nom) + ',  ' + Prenom AS NomComplet, ISNULL(LOWER(Email), '') AS Email " +
                    " FROM Professeurs WHERE Actif = 1 ORDER BY Nom, Prenom");

                da = new SqlDataAdapter(sSql, myConnection);
                dTable = new DataTable();
                da.Fill(dTable);

                gvStudents.DataSource = dTable;
                gvStudents.DataBind();
                myConnection.Close();
                myConnection = null;
            }
            catch (Exception ex)
            {
                lblError.Text = "ERREUR: " + ex.Message;
            }
            finally
            {
                dTable = null;
                da = null;
            }
        }
        else
        {
            gvStudents.DataSource = null;
            gvStudents.DataBind();
        }
    }

    protected void chkTousLesProfesseurs_CheckedChanged(object sender, EventArgs e)
    {
        bool bStatus = chkTousLesProfesseurs.Checked;
        foreach (GridViewRow dr in gvStudents.Rows)
        {
            CheckBox ctlCheck = (CheckBox)dr.FindControl("chkProfesseur");
            ctlCheck.Checked = bStatus;
        }
    }

    protected void btnEnvoyer_Click(object sender, EventArgs e)
    {
        String sqlSelect = String.Empty;

        //CHeck 
        String sSujet = txtEmailSubject.Text.Trim(), sMessage = txtEmailBody.InnerText.Trim();
        String sClasseChoisie = "0";
        if (sSujet == String.Empty)
        {
            lblError.Text = "ERROR: Sujet Vide!";
            return;
        }
        else if (sMessage == String.Empty)
        {
            lblError.Text = "ERROR: Message Vide!";
            return;
        }


        // Encode the string input
        //StringBuilder sbMessage = new StringBuilder(HttpUtility.HtmlEncode(sMessage));
        //StringBuilder sbSujet = new StringBuilder(HttpUtility.HtmlEncode(sSujet));
        // Selectively allow <b> and <i>
        //sbMessage.Replace("&lt;b&gt;", "<b>");
        //sbMessage.Replace("&lt;/b&gt;", "</b>");
        //sbMessage.Replace("&lt;i&gt;", "<i>");
        //sbMessage.Replace("&lt;/i&gt;", "</i>");

        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {            
            try
            {
                sqlConn.Open();
                String sSql = "INSERT EmailEnvoye (Message, Sujet, CoursOffertID, CreeParUserName, DateEnvoyee, AuProfesseur) " +
                    " VALUES (@Message, @Sujet, @CoursOffertID, @CreeParUserName, @DateEnvoyee, @AuProfesseur)";

                SqlParameter ParamMessage = new SqlParameter("@Message", SqlDbType.VarChar);
                ParamMessage.Value = sMessage;
                SqlParameter ParamSujet = new SqlParameter("@Sujet", SqlDbType.VarChar);
                ParamSujet.Value = sSujet;
                SqlParameter ParamCoursOffertID = new SqlParameter("@CoursOffertID", SqlDbType.Int);
                ParamCoursOffertID.Value = sClasseChoisie;
                SqlParameter ParamUserName = new SqlParameter("@CreeParUserName", SqlDbType.VarChar);
                ParamUserName.Value = db.GetWindowsUser();
                SqlParameter ParamDateEnvoyee = new SqlParameter("@DateEnvoyee", SqlDbType.Date);
                ParamDateEnvoyee.Value = DateTime.Today.Date;
                SqlParameter ParamAuProfesseur = new SqlParameter("@AuProfesseur", SqlDbType.Int);
                ParamAuProfesseur.Value = 1;

                if (!db.IssueCommandWithParams(sSql, sqlConn, ParamMessage, ParamSujet, ParamCoursOffertID, ParamUserName, ParamDateEnvoyee, ParamAuProfesseur))
                {
                    lblError.Text = "Message envoyé non sauvegardé dans la base de Données!";
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "ERREUR: " + ex.Message;
            }

            lblError.Text = "Patientez svp ...";
            int iEnvoye = 0;

            foreach (GridViewRow dr in gvStudents.Rows)
            {
                CheckBox ctlCheck = (CheckBox)dr.FindControl("chkProfesseur");
                if (!ctlCheck.Checked)
                {
                    continue;
                }

                Label ctlEmail = (Label)dr.FindControl("lblEmail");
                String adressEmail = ctlEmail.Text.Trim();
                if (adressEmail == String.Empty)           // cheke men adrès vid: Erè
                {
                    ((Label)dr.FindControl("lblNomComplet")).Enabled = false;
                    continue;
                }

                try
                {
                    if (db.SendMailToUser(adressEmail, sMessage, sSujet, null))
                    {
                        iEnvoye++;
                        Thread.Sleep(10);       //Release thread so screen is updated
                    }
                }
                catch (Exception ex)
                {
                    lblError.Text = lblError.Text + "  ERREUR: " + ex.Message;
                }
            }

            lblError.Text = iEnvoye + " message(s) envoyé(s)!";
        }
        db = null;
    }
}
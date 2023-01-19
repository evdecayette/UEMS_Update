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
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Diagnostics;

public partial class CourrieldeMasseEtudiants : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            String sGroup = ConfigurationManager.AppSettings["AdminGroup"].ToString();
            RemplirListeDeCoursDejaOfferts();
            chkIncludeNote.Enabled = false;
            chkToutesLesClasses.Checked = true;
            FillListWithStudents(false);
        }
    }

    void RemplirListeDeCoursDejaOfferts()
    {
        lblError.Text = String.Empty;
        try
        {
            SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
            String sSql = @"SELECT 0 AS CoursOffertID, 'AAA' AS NumeroCours, 'Choisissez Un Cours' AS Description UNION " +
                " SELECT CO.CoursOffertID AS ID, C.NumeroCours AS Cours, C.NumeroCours + ' - ' + NomCours + ' - ' + H.Jours + ' * ' +" + 
                " H.HeureDebut + ' - ' + H.HeureFin + ' *' AS Description " +
                " FROM CoursOfferts CO, Cours C, Horaires H  WHERE CO.NumeroCours = C.NumeroCours AND CO.HoraireID = H.HoraireID " +
                " AND SessionId IN (SELECT SessionID From LesSessions WHERE SessionCourante = 1) ORDER BY NumeroCours";

            SqlDataAdapter da = new SqlDataAdapter(sSql, myConnection);
            DataTable dTable = new DataTable();
            da.Fill(dTable);

            ddlCoursOfferts.DataSource = dTable;
            ddlCoursOfferts.DataValueField = "CoursOffertID";
            ddlCoursOfferts.DataTextField = "Description";
            ddlCoursOfferts.DataBind();
            myConnection.Close();
            myConnection = null;

        }
        catch (Exception ex)
        {
            lblError.Text = "ERREUR: " + ex.Message;
        }
    }

    protected void ddlCoursOfferts_SelectedIndexChanged(object sender, EventArgs e)
    {
        string sCoursOffertID = ddlCoursOfferts.SelectedValue.ToString();
        if (sCoursOffertID == "0")
        {
            btnEnvoyer.Enabled = false;
            FillListWithStudents(false);
            chkIncludeNote.Enabled = false;
        }
        else
        {
            btnEnvoyer.Enabled = true;
            FillListWithStudents(true);
            chkIncludeNote.Enabled = true;
            chkToutesLesClasses.Checked = false;
        }
    }

    protected void FillListWithStudents(bool bYesNo)
    {
        lblError.Text = String.Empty;

        SqlDataAdapter da;
        DataTable dTable;
        String sSql = "";

        if (bYesNo)
        {
            sSql = string.Format("SELECT CP.PersonneID, UPPER(Nom) + ',  ' + Prenom AS NomComplet, ISNULL(LOWER(Email), '') AS Email " +
                " FROM Personnes P, CoursPris CP  " +
                " WHERE P.PersonneID = CP.PersonneID AND CP.CoursOffertID = {0} ORDER BY Nom", ddlCoursOfferts.SelectedValue.ToString());
        }
        else
        {
            sSql = string.Format("SELECT PersonneID, UPPER(Nom) + ',  ' + Prenom AS NomComplet, ISNULL(LOWER(Email), '') AS Email " +
                            " FROM Personnes WHERE Actif = 1 ORDER BY Nom");
        }
        try
        {
            SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);

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

    protected void chkToutesLesClasses_CheckedChanged(object sender, EventArgs e)
    {
        chkIncludeNote.Checked = false;
        chkIncludeNote.Enabled = false;
        if (!chkToutesLesClasses.Checked)
        {
            gvStudents.DataSource = null;
            gvStudents.DataBind();

            ddlCoursOfferts.ClearSelection();          
        }
        else
        {
            FillListWithStudents(false);
        }
    }

    protected void ResetAll()
    {
        gvStudents.DataSource = null;
        gvStudents.DataBind();
        ddlCoursOfferts.ClearSelection();
        txtEmailSubject.Text = String.Empty;
        txtEmailBody.InnerText = String.Empty;
        chkToutesLesClasses.Checked = false;
        lblError.Text = String.Empty;
    }

    protected void btnEnvoyer_Click(object sender, EventArgs e)
    {
        String sqlSelect = String.Empty;
        //Check 
        String sSujet = txtEmailSubject.Text.Trim(), sMessage = txtEmailBody.InnerText.Trim(), sAttachements = "";
        List<Attachment> allAttachments = new List<Attachment>();
        try
        {
            if (uploadAttachements.HasFile)
            {
                //sAttachements = txtAttachements.FileName;
                /* Beginning of Attachment1 process - Check the first open file dialog for a attachment */
                if (uploadAttachements.PostedFile != null)
                {
                    /* Get a reference to PostedFile object */
                    HttpPostedFile attFile = uploadAttachements.PostedFile;
                    /* Get size of the file */
                    int attachFileLength = attFile.ContentLength;
                    /* Make sure the size of the file is > 0  */
                    if (attachFileLength > 0)
                    {
                        /* Get the file name */
                        sAttachements = System.IO.Path.GetFileName(uploadAttachements.PostedFile.FileName);
                        /* Delete old version if exists */
                        FileInfo fileToDelete = new FileInfo(Server.MapPath(ConfigurationManager.AppSettings["uploadfolder"].ToString() + "\\" + sAttachements));
                        if (fileToDelete.Exists)
                        {
                            fileToDelete.Delete();
                        }
                        /* Save the file on the server */
                        uploadAttachements.PostedFile.SaveAs(Server.MapPath(ConfigurationManager.AppSettings["uploadfolder"].ToString() + "\\" + sAttachements));
                        Attachment att = new Attachment(Server.MapPath(ConfigurationManager.AppSettings["uploadfolder"].ToString() + "\\" + sAttachements));
                        allAttachments.Add(att);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            lblError.Text = "ERREUR: " + ex.Message;
            return;
        }

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

        if (!chkToutesLesClasses.Checked)
        {
            // Si Option <Toutes les Classes> n'est pas choisie, une classe est obligatoire pour envoyer le courriel
            if (ddlCoursOfferts.SelectedValue.ToString() == "0")
            {
                lblError.Text = @"ERROR: Choix de Classes ou de Toutes les Classes Obligatoire si Option <Toutes les Classes> n'est pas selectionné !";
                return;
            }
        }

        // Encode the string input
        //StringBuilder sbMessage = new StringBuilder(HttpUtility.HtmlEncode(sMessage));
        //StringBuilder sbSujet = new StringBuilder(HttpUtility.HtmlEncode(sSujet));
        // Selectively allow <b> and <i>
        //sbMessage.Replace("&lt;b&gt;", "<b>");
        //sbMessage.Replace("&lt;/b&gt;", "</b>");
        //sbMessage.Replace("&lt;i&gt;", "<i>");
        //sbMessage.Replace("&lt;/i&gt;", "</i>");

        sClasseChoisie = ddlCoursOfferts.SelectedValue.ToString();
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            try
            {
                sqlConn.Open();
                String sSql = "INSERT EmailEnvoye (Message, Sujet, CoursOffertID, CreeParUserName, DateEnvoyee) " +
                    " VALUES (@Message, @Sujet, @CoursOffertID, @CreeParUserName, @DateEnvoyee)";

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

                if (!db.IssueCommandWithParams(sSql, sqlConn, ParamMessage, ParamSujet, ParamCoursOffertID, ParamUserName, ParamDateEnvoyee))
                {
                    lblError.Text = "Message envoyé non sauvegardé dans la base de Données!";
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "ERREUR: " + ex.Message;
            }

            lblError.Text = "Patientez svp ...";
            Thread.Sleep(500);

            int iEnvoye = 0;
            String sNoteEtudiant = "";
            String[] nameCoursLong;
            String nameCours = "";

            if (ddlCoursOfferts.SelectedIndex > 0)
            {
                nameCoursLong = ddlCoursOfferts.SelectedItem.Text.Split('-');
                nameCours = "Classe: " + nameCoursLong[1] + "<br/>";
            }

            foreach (GridViewRow dr in gvStudents.Rows)
            {
                Label ctlEmail = (Label)dr.FindControl("lblEmail");

                if (ctlEmail.Text.Trim() == String.Empty)
                {
                    ((Label)dr.FindControl("lblNomComplet")).Enabled = false;
                    continue;
                }

                if (chkIncludeNote.Checked)
                {
                    // Get Note and Add to body of text
                    String sPersonneID = gvStudents.DataKeys[dr.DataItemIndex].Value.ToString();
                    String sCoursOffertID = ddlCoursOfferts.SelectedValue.ToString();

                    sNoteEtudiant = "<br/><b>" + nameCours + "Note : " + db.getNoteForEdutiant(sPersonneID, sCoursOffertID, sqlConn) + "</b>";
                }

                try
                {
                    if (db.SendMailToUser(ctlEmail.Text, sMessage + sNoteEtudiant, sSujet, allAttachments))
                    {
                        iEnvoye++;
                        Thread.Sleep(10);       //Release thread so screen is updated
                    }
                }
                catch (Exception ex)
                {
                    lblError.Text = lblError.Text + "   ERREUR: " + ex.Message;
                }
            }
            ResetAll();

            lblError.Text = iEnvoye + " message(s) envoyé(s)!";
        }
        db = null;
        try
        {
            /* Delete file: if not, trying to send it again by the same process will not work */
            FileInfo fileToDelete = new FileInfo(Server.MapPath(ConfigurationManager.AppSettings["uploadfolder"].ToString() + "\\" + sAttachements));
            if (fileToDelete.Exists)
            {
                fileToDelete.Delete();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Erreur fichier courriel pas effacé!!!!! -- " + ex.Message);
        }
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ResetAll();
    }
}
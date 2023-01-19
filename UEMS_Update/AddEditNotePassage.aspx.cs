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

public partial class AddEditNotePassage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            String sGroup = ConfigurationManager.AppSettings["AdminGroup"].ToString();
            DB_Access db = new DB_Access();
            using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
            {
                try
                {
                    sqlConn.Open();

                    if (db.isCurrentWindowsInThisGroup(sGroup) || (db.IsCustomAdminMember(db.GetWindowsUser(), sqlConn)))
                    {
                        RemplirCoursOffertsDropDown();
                    }
                    else
                    {
                        lblError.Text = "PERMISSION : Vous n'avez pas la permission d'accéder à cette PAGE!!! ";
                        lblError.ForeColor = System.Drawing.Color.Red;
                        txtNotePassage.Enabled = false;
                        btnSauvegarder.Enabled = false;
                    }
                }
                catch
                {
                    lblError.Text = "ERREUR: Base de Données!";
                }
            }
        }
    }

    void RemplirCoursOffertsDropDown()
    {
        lblError.Text = String.Empty;

        try
        {
            DB_Access db = new DB_Access();
            using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
            {
                sqlConn.Open();
                String sSql = @"SELECT 'Choisissez un Cours' AS NomCours, -1 AS CoursOffertID, 'AAA000' AS NumeroCours " +
                " UNION SELECT CO.NumeroCours + ' - ' + C.NomCours, CoursOffertID AS NomCours, CO.NumeroCours FROM CoursOfferts CO, Cours C  " +
                " WHERE CO.NumeroCours = C.NumeroCours AND CO.Actif = 1 ORDER BY NumeroCours ";

                SqlDataAdapter da = new SqlDataAdapter(sSql, sqlConn);
                DataTable dTable = new DataTable();
                da.Fill(dTable);

                ddlCoursOfferts.DataSource = dTable;
                ddlCoursOfferts.DataValueField = "CoursOffertID";
                ddlCoursOfferts.DataTextField = "NomCours";

                ddlCoursOfferts.DataBind();
            }
        }
        catch (Exception ex)
        {
            lblError.Text = "ERREUR: " + ex.Message;
        }
    }
    protected void ddlCoursOfferts_SelectedIndexChanged(object sender, EventArgs e)
    {
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            sqlConn.Open();
            txtNotePassage.Text = db.GetScalar(String.Format("SELECT NotePassage FROM CoursOfferts WHERE CoursOffertID = {0}", 
                ddlCoursOfferts.SelectedValue.ToString()), sqlConn).ToString();
            txtNotePassage_Hidden.Text = txtNotePassage.Text;
            txtNotePassage.Focus();
        }
        db = null;
    }
    protected void btnSauvegarder_Click(object sender, EventArgs e)
    {
        lblError.ForeColor = System.Drawing.Color.Red;
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            string errorMessage = "";
            String csCoursOffertID, csNotePassage;
            try
            {
                txtNotePassage.Text = txtNotePassage.Text.Trim();


                if (txtNotePassage.Text == String.Empty)
                {
                    lblError.Text = "NOTE de Cours est obligatoire !";
                    return;
                }
                if (!db.ChiffreSeulement(txtNotePassage.Text))
                {
                    lblError.Text = "NOTE de Cours doit être un nombre !";
                    return;
                }

                //Double dAncienneNote = Double.Parse(txtNotePassage_Hidden.Text.Trim());
                //Double dNouvelleNote = Double.Parse(txtNotePassage.Text);
                //if (dNouvelleNote > dAncienneNote)
                //{
                //    lblError.Text = "Nouvelle Note ne peut pas être supérieure à l'Ancienne Note";
                //    txtNotePassage.Text = txtNotePassage_Hidden.Text;
                //    return;
                //}

                csNotePassage = txtNotePassage.Text;
                if (ddlCoursOfferts.SelectedValue.ToString() == "AAA001")
                {
                    lblError.Text = "Choisissez le Cours d'abord";
                    return;
                }
                csCoursOffertID = ddlCoursOfferts.SelectedValue.ToString();
                sqlConn.Open();
                if (!db.UpdateNotePassage(ref errorMessage, csCoursOffertID, csNotePassage, sqlConn))
                {
                    lblError.Text = "ERREUR " + errorMessage;
                }
                else
                {
                    lblError.Text = "Sauvegarder avec Succès! ";
                    lblError.ForeColor = System.Drawing.Color.Blue;
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "ERREUR: " + ex.Message;
            }
        }
        db = null;
    }
}
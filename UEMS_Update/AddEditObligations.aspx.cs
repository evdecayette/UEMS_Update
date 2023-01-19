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

public partial class AdminPages_AddEditObligations : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            string sObligationID = string.Empty;
            String sGroup = ConfigurationManager.AppSettings["AdminGroup"].ToString();
            DB_Access db = new DB_Access();
            using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
            {
                try
                {
                    sqlConn.Open();
                    if (db.isCurrentWindowsInThisGroup(sGroup) || (db.IsCustomAdminMember(db.GetWindowsUser(), sqlConn)))
                    {
                        try
                        {
                            sObligationID = Request.Cookies.Get("ObligationID").Value.ToString();
                            // Enlever le cookie
                            Response.Cookies.Remove("ObligationID");        // First method

                            HttpCookie cookie = new HttpCookie("ObligationID", string.Empty); // Second method, in case
                            Response.Cookies.Set(cookie);
                        }
                        catch (Exception Excep)
                        {
                            Debug.WriteLine(Excep.Message);
                        }

                        //
                        if (sObligationID == string.Empty)     // Pas de correction - Nouvelle Entrée
                        {
                            txtCode.Focus();
                        }
                        else
                        {
                            RemplirInfoPourCorrection(sObligationID);
                        }
                        RemplirListeObligationsExistantes();
                    }
                    else
                    {
                        lblError.Text = "PERMISSION : Vous n'avez pas la permission d'accéder à cette PAGE!!! ";
                        btnCancel.Enabled = false;
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

    protected void RemplirInfoPourCorrection(String sObligationID)
    {
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            try
            {
                sqlConn.Open();
                String sSql = String.Format("SELECT * FROM Obligations WHERE ObligationID = {0}", sObligationID);

                SqlDataReader dtTemp = db.GetDataReader(sSql, sqlConn);
                if (dtTemp.Read())
                {
                    // Fill info
                    txtCode.Text = dtTemp["Code"].ToString();
                    txtCode.Enabled = false;
                    txtMontant.Text = Double.Parse(dtTemp["Montant"].ToString()).ToString("F");
                    txtDescription.Text = dtTemp["Description"].ToString();
                    txtAutreDescription.Text = dtTemp["AutreDescription"].ToString();
                }
                db = null;
            }
            catch (Exception ex)
            {
                lblError.Text = "ERROR : Base de Données - Cette Obligation n'est pas trouvée - Voir Technicien !!!";
                lblError.ForeColor = System.Drawing.Color.Red;
                db = null;
                String messsage = ex.Message;
            }
        }
    }

    protected void RemplirListeObligationsExistantes()
    {
        lblError.Text = String.Empty;

        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            try
            {
                sqlConn.Open();
                String sSql = "SELECT 0 AS ObligationID, 'A' AS Code, 0 AS Montant, 'NOUVELLE OBLIGATION' AS Description, '' AS AutreDescription " +
                    " UNION SELECT ObligationID, Code, Montant, Description, AutreDescription FROM Obligations";

                SqlDataAdapter da = new SqlDataAdapter(sSql, sqlConn);
                DataTable dTable = new DataTable();
                da.Fill(dTable);

                lstObligationsExistantes.DataSource = dTable;
                lstObligationsExistantes.DataValueField = "ObligationID";
                lstObligationsExistantes.DataTextField = "Description";

                lstObligationsExistantes.DataBind();
            }
            catch (Exception ex)
            {
                lblError.Text = "ERREUR: " + ex.Message;
                lblError.ForeColor = System.Drawing.Color.Red;
            }
        }
    }


    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ClearInfo();
        btnSauvegarder.Text = "Sauvegarder";
    }

    void ClearInfo()
    {
        // Clear info
        lblError.Text = "";
        txtCode.Text = String.Empty;
        txtCode.Enabled = true;
        txtMontant.Text = String.Empty;
        txtDescription.Text = String.Empty;
        txtAutreDescription.Text = String.Empty;
        lstObligationsExistantes.SelectedIndex = -1;
        txtCode.Focus();
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
                string sSql = String.Empty;

                txtCode.Text = txtCode.Text.Trim();
                txtMontant.Text = txtMontant.Text.Trim();
                txtDescription.Text = txtDescription.Text.Trim();
                txtAutreDescription.Text = txtAutreDescription.Text.Trim();

                if (txtCode.Text == String.Empty)
                {
                    lblError.Text = "Le Code est Obligatoire !!!";
                    txtCode.Focus();
                    return;
                }
                if (txtCode.Text.Length != 2)
                {
                    lblError.Text = "Le Code est Obligatoirement 2 Lettres !!!";
                    txtCode.Focus();
                    return;
                }

                txtCode.Text = txtCode.Text.ToUpper();

                if (txtMontant.Text == String.Empty)
                {
                    lblError.Text = "Le Montant est Obligatoire !!!";
                    txtMontant.Focus();
                    return;
                }
                if (!estMonaie(txtMontant.Text))
                {
                    lblError.Text = "Le Format du Montant est xxxxx.xx !!!";
                    txtMontant.Focus();
                    return;
                }
                if (txtDescription.Text == String.Empty)
                {
                    lblError.Text = "La Description est Obligatoire !!!";
                    txtDescription.Focus();
                    return;
                }

                SqlParameter paramObligationID = new SqlParameter("@ObligationID", SqlDbType.Int);
                paramObligationID.Value = Int16.Parse(lstObligationsExistantes.SelectedValue.ToString());

                SqlParameter paramCode = new SqlParameter("@Code", SqlDbType.Text);
                paramCode.Value = txtCode.Text;

                SqlParameter paramMontant = new SqlParameter("@Montant", SqlDbType.Float);
                paramMontant.Value = float.Parse(txtMontant.Text);

                SqlParameter paramDescription = new SqlParameter("@Description", SqlDbType.Text);
                paramDescription.Value = txtDescription.Text;

                SqlParameter paramAutreDescription = new SqlParameter("@AutreDescription", SqlDbType.Text);
                paramAutreDescription.Value = txtAutreDescription.Text;

                if (btnSauvegarder.Text.ToLower() == "mise a jour")
                {

                    sSql = String.Format("UPDATE Obligations SET Montant = @Montant, Description = @Description, AutreDescription = @AutreDescription " +
                      " WHERE ObligationID = @ObligationID");
                    if (db.IssueCommandWithParams(sSql, sqlConn, paramMontant, paramDescription, paramAutreDescription, paramObligationID))
                    {
                        ClearInfo();
                        lblError.Text = "SUCCES Sauvegarder !!!";
                        lblError.ForeColor = System.Drawing.Color.Blue;
                        btnSauvegarder.Text = "Sauvegarder";
                        RemplirListeObligationsExistantes();
                    }
                    else
                    {
                        lblError.Text = "ERROR: PAS de Mise à Jour - VOIR un technicien !!!";
                        lblError.ForeColor = System.Drawing.Color.Red;
                    }
                }
                else if (btnSauvegarder.Text.ToLower() == "sauvegarder")
                {
                    // Everything is ok: Save!
                    sSql = String.Format("INSERT INTO Obligations (Code, Montant, Description, AutreDescription) " +
                        " VALUES (@Code, @Montant, @Description, @AutreDescription)");
                    if (db.IssueCommandWithParams(sSql, sqlConn, paramCode, paramMontant, paramDescription, paramAutreDescription))
                    {
                        ClearInfo();
                        lblError.Text = "SUCCES Sauvegarder !!!";
                        lblError.ForeColor = System.Drawing.Color.Blue;
                        RemplirListeObligationsExistantes();
                    }
                    else
                    {
                        lblError.Text = "ERROR: PAS de sauvegarde - VOIR un technicien !!!";
                        lblError.ForeColor = System.Drawing.Color.Red;
                    }
                }
                else
                {
                    lblError.Text = "Erreur NON Spécifiée (VOIR un Technicien) - Pas de Sauvegarde";
                    txtCode.Focus();
                    return;
                }

                db = null;
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                lblError.ForeColor = System.Drawing.Color.Red;
                db = null;
            }
        }
    }

    protected bool estMonaie(String aString)
    {
        char aChar;
        bool retValue = false;
        for (int i = 0; i < aString.Length; i++)
        {
            aChar = Convert.ToChar(aString[i]);
            if (aChar == 46)
                retValue = true;
            else if (aChar >= 48 && aChar <= 57)
                retValue = true;
            else
                return false;
        }
        return retValue;
    }

    protected void lstObligationsExistantes_SelectedIndexChanged(object sender, EventArgs e)
    {
        lblError.Text = "";
        int iObligation = Int16.Parse(lstObligationsExistantes.SelectedValue.ToString());
        if (iObligation > 0)
        {
            btnSauvegarder.Text = "Mise A Jour";
            RemplirInfoPourCorrection(lstObligationsExistantes.SelectedValue.ToString());
        }
        else
        {
            ClearInfo();
            btnSauvegarder.Text = "Sauvegarder";
        }
    }
}
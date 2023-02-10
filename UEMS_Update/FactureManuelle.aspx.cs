using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;

public partial class FactureManuelle : System.Web.UI.Page
{
    String sPersonneID = String.Empty;
    List<Obligation> LesObligations = new List<Obligation>();
    string ConnectionString = XCryptEngine.ConnectionStringEncryption.Decrypt(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get PersonneID 
        try
        {
            sPersonneID = Request.QueryString["PersonneID"];
            txtPersonneID.Text = sPersonneID;
            Debug.WriteLine(sPersonneID);
        }
        catch (Exception Excep)
        {
            Debug.WriteLine(Excep.Message);
        }

        //
        if (sPersonneID != String.Empty)     // Pas de correction - Nouvelle Entrée
        {
            BuildStringInfoGenerale();
            RemplirListeDesObligations();
        }
    }

    void BuildStringInfoGenerale()
    {
        String returnedString = String.Empty;
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
        {
            // Loop through all records
            try
            {
                sqlConn.Open();
                string sSql = String.Format("SELECT Nom, Prenom, DDN, NIF, Telephone1, email FROM Personnes WHERE PersonneID = '{0}'", sPersonneID);

                SqlDataReader dt = db.GetDataReader(sSql, sqlConn);
                if (dt != null)
                {
                    if (dt.Read())
                    {
                        txtNomComplet_Header.Text = dt["Nom"].ToString() + ", " + dt["Prenom"].ToString() + "  (Phone: " + dt["Telephone1"].ToString() + "  Email: "
                            + dt["email"].ToString() + ")";                         // Header
                        txtNomComplet_Header.ForeColor = System.Drawing.Color.Red;
                    }
                }
                db = null;
            }
            catch (Exception ex)
            {
                returnedString = ex.Message;
                db = null;
            }
        }
    }

    protected void btnSauvegarder_Click(object sender, EventArgs e)
    {
        lblError.Text = "";
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
        {
            try
            {
                sqlConn.Open();
                if (ddlObligations.SelectedValue == "AAA")
                {
                    lblError.Text = "Choisissez une Description!";
                    return;
                }

                if (txtMontant.Text.Trim() == String.Empty || !db.ChiffreSeulement(txtMontant.Text.Trim()))
                {
                    lblError.Text = "Montant: Format Invalide or Vide!";
                    return;
                }

                String sSql = String.Format("INSERT INTO MontantsDus (PersonneID, Montant, DateMontant, CodeObligation, Description) VALUES ('{0}', {1}, '{2}', '{3}', '{4}')",
                     txtPersonneID.Text, DateTime.Today.ToString("yyyy-mm-dd"), txtMontant.Text.Trim(), ddlObligations.SelectedValue.ToString(), ddlObligations.Text);
                try
                {
                    //myConnection.Open();
                    SqlCommand myCommand = new SqlCommand(sSql, sqlConn);
                    SqlCommand SqlCmdNewID = new SqlCommand("SELECT @@IDENTITY", sqlConn);
                    myCommand.ExecuteNonQuery();
                    String MontantRecuID = SqlCmdNewID.ExecuteScalar().ToString();

                    txtMontant.Text = "";
                }
                catch (Exception ex)
                {
                    lblError.Text = ex.Message;
                }
            }
            catch
            {
                lblError.Text = "Erreur ...";
            }
        }
    }

    public class Obligation
    {
        private string _Code;
        public string Code
        {
            get { return _Code; }
            set { _Code = value; }
        }

        private string _Description;
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        private Double _Montant;
        public Double Montant
        {
            get { return _Montant; }
            set { _Montant = value; }
        }
    }

    void RemplirListeDesObligations()
    {
        try
        {
            DB_Access db = new DB_Access();
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                String sSql = @"SELECT Description, Code, Montant FROM Obligations ORDER BY Code, Description";

                lblError.Text = "";

                try
                {
                    sqlConn.Open();
                    SqlDataReader dt = db.GetDataReader(sSql, sqlConn);
                    while (dt.Read())
                    {
                        Obligation obligation = new Obligation();
                        obligation.Code = dt["Code"].ToString();
                        obligation.Description = dt["Description"].ToString();
                        obligation.Montant = Double.Parse(dt["Montant"].ToString());

                        LesObligations.Add(obligation);
                    }
                }
                catch (Exception ex)
                {
                    lblError.Text = ex.Message;
                }
                ddlObligations.DataSource = LesObligations;
                //ddlObligations.ItemType = "Obligation";
                ddlObligations.DataValueField = "Code";
                ddlObligations.DataTextField = "Description";
                ddlObligations.DataBind();
            }
        }
        catch (Exception ex)
        {
            lblError.Text = "ERREUR: " + ex.Message;
        }
    
    }

    protected void btnResumeFinance_Click(object sender, EventArgs e)
    {
        Response.Redirect("ResumeFinanceEtudiant.aspx?PersonneID=" + txtPersonneID.Text);
    }

    protected void btnReleveNotes_Click(object sender, EventArgs e)
    {
        Response.Redirect("ReleveNotesEtudiant.aspx?PersonneID=" + txtPersonneID.Text);
    }

    protected void btnProgresGraduation_Click(object sender, EventArgs e)
    {
        Response.Redirect("ProgresGraduationEtudiant.aspx?PersonneID=" + txtPersonneID.Text);
    }

    protected void btnPayments_Click(object sender, EventArgs e)
    {
        Response.Redirect("RecevoirPaiements.aspx?PersonneID=" + txtPersonneID.Text);
    }

    protected void btnEditNotePasseeEtudiant_Click(object sender, EventArgs e)
    {
        Response.Redirect("EditNotePasseeEtudiant.aspx?PersonneID=" + txtPersonneID.Text);
    }

    protected void ddlObigations_SelectedIndexChanged(object sender, EventArgs e)
    {
        List<Obligation> obligations = (List<Obligation>)ddlObligations.DataSource;
        Obligation obligation = obligations[ddlObligations.SelectedIndex];
        txtMontant.Text = obligation.Montant.ToString("F");
    }
}
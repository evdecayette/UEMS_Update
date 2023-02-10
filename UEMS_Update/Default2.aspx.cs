using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

public partial class Default2 : System.Web.UI.Page
{
    string ConnectionString = XCryptEngine.ConnectionStringEncryption.Decrypt(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            String sGroup = ConfigurationManager.AppSettings["AdminGroup"].ToString();
            RemplirListeDeCoursDejaOfferts();
            
        }
    }

    void RemplirListeDeCoursDejaOfferts()
    {
        lblError.Text = String.Empty;
        btnEnvoyer.Enabled = false;
        try
        {
            SqlConnection myConnection = new SqlConnection(ConnectionString);
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
        }
        else
        {
            btnEnvoyer.Enabled = true;
            FillListWithStudents(true);
        }
    }

    protected void FillListWithStudents(bool bYesNo)
    {
        lblError.Text = String.Empty;

        SqlDataAdapter da;
        DataTable dTable;
        if (bYesNo)
        {
            try
            {
                SqlConnection myConnection = new SqlConnection(ConnectionString);
                String sSql = string.Format("SELECT CP.PersonneID, UPPER(Nom) + ',  ' + Prenom AS NomComplet, ISNULL(LOWER(Email), '') AS Email " +
                    " FROM Personnes P, CoursPris CP  " +
                    " WHERE P.PersonneID = CP.PersonneID AND CP.CoursOffertID = {0} ORDER BY Nom", ddlCoursOfferts.SelectedValue.ToString());

                da = new SqlDataAdapter(sSql, myConnection);
                dTable = new DataTable();
                da.Fill(dTable);

                gvStudents.DataSource = dTable;
                gvStudents.DataBind();
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

    protected void chkToutesLesClasses_CheckedChanged(object sender, EventArgs e)
    {
        if (chkToutesLesClasses.Checked)
        {
            gvStudents.DataSource = null;
            gvStudents.DataBind();
            
            ddlCoursOfferts.ClearSelection();
        }
        else
        {
            
        }
    }
}
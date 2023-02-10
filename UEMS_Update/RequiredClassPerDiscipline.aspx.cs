using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class RequiredClassPerDiscipline : System.Web.UI.Page
{
    
    String sDisciplineID = String.Empty, sCursusNom = String.Empty;
    
    DB_Access db = new DB_Access();
    string ConnectionString = XCryptEngine.ConnectionStringEncryption.Decrypt(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //BuildStringCoursPris();

            sDisciplineID = Request.QueryString["DisciplineID"];
            sCursusNom = Request.QueryString["NomCursus"];

            //String Date = DateTime.Today.Date.ToString("dd-MMM-yyyy");

            try
            {
                this.ListView();
                Response.Cookies.Remove("DisciplineID");                              // First method
                HttpCookie cookie = new HttpCookie("DisciplineID", String.Empty);     // Second remove method, in case
                Response.Cookies.Set(cookie);
            }
            catch (Exception NoNeed)
            {
                Debug.WriteLine(NoNeed.Message);
                lblErreur.Text = ("Erreur: ");
            }

        }
    }

    private void ListView()
    {
        int iDisciplineID = Convert.ToInt32(sDisciplineID);
        lblCursus.Text += "<center><strong>Cursus Faculté De " + sCursusNom + "</strong></center>";
        //Label1.Text += "<center><strong>Liste Des Cours Du Programme De " + sCursusNom + "</strong></center>";
        DB_Access db0 = new DB_Access();
        String sSql = String.Format("  Select Distinct C.NomCours, C.NumeroCours, C.CoursPrerequis, C.Credits from Cours C, Cursus Cu, Disciplines D where Cu.DisciplineID = {0} and C.Actif = 1 and C.NumeroCours = Cu.NumeroCours", iDisciplineID);

        switch (iDisciplineID)
        {

            case 1:// statement sequence
                backupMethode(sSql, iDisciplineID);
                break;

            case 2: // statement sequence
                backupMethode(sSql, iDisciplineID);
                break;
            case 3: // statement sequence
                backupMethode(sSql, iDisciplineID);
                break;
            case 4: // statement sequence
                backupMethode(sSql, iDisciplineID);
                break;
            case 5: // statement sequence
                backupMethode(sSql, iDisciplineID);
                break;
            default:
                return;
        }
    }
    protected void backupMethode(String sql, int DisciplineID)
    {
        DB_Access db = new DB_Access();
        DataTable dt = new DataTable();
        int iDisciplineID = Convert.ToInt32(DisciplineID);
        //Think with a switch for the case possible:
       
            try
            {
                SqlConnection con = new SqlConnection(ConnectionString);
                con.Open();
                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
                ContentLiteralID.DataSource = dt;
                ContentLiteralID.DataBind();
                con.Close();
            }
            catch (Exception ex)
            {
                lblErreur.Text = ("Erreur: " + ex.Message);
            }
        
    }
}
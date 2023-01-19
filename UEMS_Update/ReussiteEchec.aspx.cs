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
using System.Threading;

public partial class ReussiteEchec : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            RemplirGridEtudiants();
        }
    }

    void RemplirGridEtudiants()
    {
        lblError.Text = String.Empty;

        try
        {
            SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
            String sSql = @"SELECT C.PersonneID, Nom, Prenom, Telephone1, IsNull(NIF, '') AS NIF, IsNull(DDN,'') AS DDN, CoursPrisID FROM Personnes P, CoursPris C " + 
                " WHERE P.PersonneID = C.PersonneID AND NumeroCours = 'FRA001' AND NoteSurCent = 0.0 AND CoursOffertID in " + 
                " (SELECT CoursOffertID FROM CoursOfferts C, LesSessions L WHERE C.SessionID = L.SessionID AND L.SessionCourante = 1) ORDER BY Nom, Prenom";

            SqlDataAdapter da = new SqlDataAdapter(sSql, myConnection);
            DataTable dTable = new DataTable();
            da.Fill(dTable);

            gvEtudiants.DataSource = dTable;
            gvEtudiants.DataBind();
            // Section importante pour BootStrap
            if (gvEtudiants.Rows.Count > 0)
            {
                //Adds THEAD and TBODY Section.
                gvEtudiants.HeaderRow.TableSection = TableRowSection.TableHeader;

                //Adds TH element in Header Row.  
                gvEtudiants.UseAccessibleHeader = true;

                //Adds TFOOT section. 
                gvEtudiants.FooterRow.TableSection = TableRowSection.TableFooter;
            }

            myConnection.Close();
            myConnection = null;
        }
        catch (Exception ex)
        {
            lblError.Text = "ERREUR: " + ex.Message;
        }
    }

    Double FormatNote(String sNote)
    {
        try
        {
            Double dd = Double.Parse(sNote);
            return dd;
        }
        catch
        {
            return 0.0;
        }
    }

    protected void btnSauvegarder_Click(object sender, EventArgs e)
    {
        int iSaved = 0, iNotSaved = 0;

        lblError.Text = "Patientez svp ...";
        Thread.Sleep(10);

        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            try
            {
                sqlConn.Open();
            }
            catch
            {
                lblError.Text = "ERREUR: Connexion Base de Données!";
            }

            foreach (GridViewRow dr in gvEtudiants.Rows)
            {
                TextBox ctlNoteControle = (TextBox)dr.FindControl("txtNote");
                Label ctlCoursPrisControle = (Label)dr.FindControl("lblCoursPrisID");
                Double dNote = FormatNote(ctlNoteControle.Text.Trim());
                Int16 iCoursPrisID = Int16.Parse(ctlCoursPrisControle.Text);

                if (ctlNoteControle.Text.Trim() == String.Empty)
                    continue;

                try
                {
                    String sSql = "UPDATE CoursPris SET DateReussite = CONVERT (date, GETDATE()), NoteSurCent = @NoteSurCent, CreeParUserName = @CreeParUserName WHERE CoursPrisID = @CoursPrisID";

                    SqlParameter ParamNoteSurCent = new SqlParameter("@NoteSurCent", SqlDbType.Money);
                    ParamNoteSurCent.Value = dNote;
                    SqlParameter ParamCoursPrisID = new SqlParameter("@CoursPrisID", SqlDbType.Int);
                    ParamCoursPrisID.Value = iCoursPrisID;
                    SqlParameter paramUserName = new SqlParameter("@CreeParUserName", SqlDbType.VarChar);
                    paramUserName.Value = db.GetWindowsUser();

                    if (db.IssueCommandWithParams(sSql, sqlConn, ParamNoteSurCent, paramUserName, ParamCoursPrisID))
                    {
                        iSaved++;
                        ctlNoteControle.Text = String.Empty; // Clear saved ones
                    }
                    else
                    {
                        iNotSaved++;
                    }
                }
                catch (Exception ex)
                {
                    lblError.Text = "ERREUR: " + ex.Message;
                }
            }
        }
        RemplirGridEtudiants();
        lblError.Text = iSaved + " Notes Mises à Jour! et " + iNotSaved + " Notes NON Sauvegardés!";
        db = null;
    }

    //protected void btnCancel_Click(object sender, EventArgs e)
    //{
    //    RemplirGridEtudiants();
    //    lblError.Text = "";
    //}
}
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

public partial class SetWaiver : System.Web.UI.Page
{
    String sPersonneID = "";
    string ConnectionString = XCryptEngine.ConnectionStringEncryption.Decrypt(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
    protected void Page_Load(object sender, EventArgs e)
    {
        sPersonneID = Request.QueryString["PersonneID"];

        if (!IsPostBack)
        {
            
            RemplirGridEtudiants(sPersonneID);
            try
            {
                //// Enlever le cookie
                Response.Cookies.Remove("PersonneID");                              // First method
                HttpCookie cookie = new HttpCookie("PersonneID", String.Empty);     // Second remove method, in case
                Response.Cookies.Set(cookie);
            }
            catch (Exception NoNeed)
            {
                Debug.WriteLine(NoNeed.Message);
            }
        }
    }

    public void RemplirGridEtudiants(String sPersonneID) //unNumeroCours)
    {
        lblError.Text = String.Empty;

        try
        {
            //int iSessionID = int.Parse(ddlSessions.SelectedValue.ToString());
            SqlConnection myConnection = new SqlConnection(ConnectionString);

            //String sSql = String.Format("SELECT P.PersonneID,  P.Prenom, P.Nom, P.NIF, P.Telephone1, P.email, CP.NoteSurCent, CP.CoursPrisID, CO.NumeroCours, " + 
            //    " CO.SessionID  FROM Personnes P, CoursPris CP, CoursOfferts CO, Cours C  " + 
            // " WHERE P.PersonneID = CP.PersonneID AND CP.CoursOffertID = CO.CoursOffertID " + 
            // " AND CO.NumeroCours = C.NumeroCours  AND C.NumeroCours = '{0}'  AND CO.SessionID = {1} AND C.ExamenEntree = 0 " +
            //    " ORDER BY P.Nom, P.Prenom", unNumeroCours, iSessionID);

            String sSql = String.Format("SELECT DISTINCT P.PersonneID, P.Nom, P.Prenom, C.NomCours, CP.NumeroCours,CP.NotePassage,CP.NoteSurCent, CP.Waiver FROM Cours C, CoursPris CP, Personnes P WHERE CP.PersonneID=P.PersonneID and CP.PersonneID= '{0}' and CP.NumeroCours = C.NumeroCours and CP.NoteSurCent < CP.NotePassage and CP.NoteSurCent > 0", sPersonneID);
            SqlDataAdapter da = new SqlDataAdapter(sSql, myConnection);
            DataTable dTable = new DataTable();
            da.Fill(dTable);
            
            gvInfoSurCours.DataSource = dTable;
            gvInfoSurCours.DataBind();
            // Section importante pour BootStrap
            if (gvInfoSurCours.Rows.Count > 0)
            {
                //Adds THEAD and TBODY Section.
                gvInfoSurCours.HeaderRow.TableSection = TableRowSection.TableHeader;

                //Adds TH element in Header Row.  
                gvInfoSurCours.UseAccessibleHeader = true;

                //Adds TFOOT section. 
                gvInfoSurCours.FooterRow.TableSection = TableRowSection.TableFooter;
            }
            myConnection.Close();
            myConnection = null;
        }
        catch (Exception ex)
        {
            lblError.Text = "ERREUR: " + ex.Message;
        }
    }

    protected void gvInfoSurCours_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gvInfoSurCours.EditIndex = -1;
        RemplirGridEtudiants(sPersonneID);
    }

    protected void gvInfoSurCours_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gvInfoSurCours.EditIndex = e.NewEditIndex;
        RemplirGridEtudiants(sPersonneID);
    }

    protected void gvInfoSurCours_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        int iSaved = 0, iNotSaved = 0;
        DB_Access db = new DB_Access();
        Label lblNumeroCours = (Label)gvInfoSurCours.Rows[e.RowIndex].FindControl("lblNumeroCours");
        TextBox txtWaiver = (TextBox)gvInfoSurCours.Rows[e.RowIndex].FindControl("txtWaiver");
        int iwaiver = Convert.ToInt32(txtWaiver.Text);
        using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
        {
            try
            {
                sqlConn.Open();
                String sSql =String.Format("UPDATE CoursPris SET Waiver = @Waiver WHERE PersonneID='{0}' and NumeroCours = '{1}'",sPersonneID, lblNumeroCours.Text);

                SqlParameter ParamWaiver = new SqlParameter("@Waiver", SqlDbType.Money);
                ParamWaiver.Value = iwaiver;
                if (db.IssueCommandWithParams(sSql, sqlConn, ParamWaiver))
                {
                    iSaved++;
                    txtWaiver.Enabled = false;
                    gvInfoSurCours.EditIndex = -1;
                    RemplirGridEtudiants(sPersonneID);
                    //ctlNoteControle.Text = String.Empty; // Clear saved ones
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
}
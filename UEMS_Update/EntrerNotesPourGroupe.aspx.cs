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

public partial class EntrerNotesPourGroupe : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            RemplirSessionsActives();
        }
    }
    void RemplirSessionsActives()
    {
        lblError.Text = String.Empty;

        try
        {
            SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
            String sSql = @"SELECT 'Choisissez une Session ' AS SessionDescription, 0 AS SessionID, -1 AS SessionCourante, '2999-12-12' AS SessionDateDebut UNION " +
	            " SELECT SessionDescription + ' (' + CONVERT(VARCHAR, SessionDateDebut, 3) + ' - ' + CONVERT(VARCHAR, SessionDateFin, 3) + ')' AS SessionDescription, " +
	            " SessionID, SessionCourante, SessionDateDebut FROM LesSessions WHERE Actif = 1 ORDER BY SessionDateDebut DESC";
            SqlDataAdapter da = new SqlDataAdapter(sSql, myConnection);
            DataTable dTable = new DataTable();
            da.Fill(dTable);

            ddlSessions.DataSource = dTable;
            ddlSessions.DataValueField = "SessionID";
            ddlSessions.DataTextField = "SessionDescription";
            ddlSessions.DataBind();
            myConnection.Close();
            myConnection = null;
        }
        catch (Exception ex)
        {
            lblError.Text = "ERREUR: " + ex.Message;
        }
    }
    void RemplirListeDeCours(int iSessionID)
    {
        lblError.Text = String.Empty;
        try
        {
            using (SqlConnection myConnection0 = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString))
            {
                int okEntrerNotes = int.Parse(ConfigurationManager.AppSettings["NombreJoursAvantNotesFinales"].ToString());

                DB_Access db = new DB_Access();
                myConnection0.Open();
                if (db.GetIdOfSessionCourante(myConnection0) == iSessionID)
                {
                    DateTime dt = db.GetEndDateOfCurrentSession(myConnection0);
                    TimeSpan tsp = dt.Subtract(DateTime.Today);

                    if (tsp.TotalDays > okEntrerNotes)
                    {
                        lblError.Text = ConfigurationManager.AppSettings["NoteTropTot"].ToString();
                        return;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            lblError.Text = ConfigurationManager.AppSettings["NoteTropTot"].ToString();
        }

        try
        {
            SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
            //String sSql = String.Format(@"SELECT 'Choisissez un Cours ' AS NomCours, 'AAA' AS NumeroCours UNION " + 
            //    " SELECT C.NomCours, CO.NumeroCours FROM Cours C, CoursOfferts CO WHERE C.NumeroCours = CO.NumeroCours " +  
            //    " AND ExamenEntree = 0 AND CO.NumeroCours <> 'NOP' AND CO.SessionID = {0} ORDER BY NumeroCours", iSessionID);

            String sSql = String.Format(@"SELECT 'Choisissez un Cours ' AS NomCours, 'AAA' AS NumeroCours, 0 AS CoursOffertID UNION " +
                 " SELECT C.NomCours + ' - ' + Jours AS NomCours, CO.NumeroCours, CO.CoursOffertID " +
                 " FROM Cours C, CoursOfferts CO, Horaires H WHERE C.NumeroCours = CO.NumeroCours " +
                 " AND CO.HoraireID = H.HoraireID AND ExamenEntree = 0 AND CO.NumeroCours <> 'NOP' AND CO.SessionID = {0} " +
                 " ORDER BY NumeroCours", iSessionID);

            SqlDataAdapter da = new SqlDataAdapter(sSql, myConnection);
            DataTable dTable = new DataTable();
            da.Fill(dTable);

            ddlCours.DataSource = dTable;
            ddlCours.DataValueField = "CoursOffertID";
            ddlCours.DataTextField = "NomCours";
            ddlCours.DataBind();
            myConnection.Close();
            myConnection = null;
        }
        catch (Exception ex)
        {
            lblError.Text = "ERREUR: " + ex.Message;
        }
    }
    protected void ddlSessions_SelectedIndexChanged(object sender, EventArgs e)
    {
        int iSessionID = int.Parse(ddlSessions.SelectedValue.ToString());
        if (iSessionID != -1)      // RemplirListeDeCours() uses it
        {
            RemplirListeDeCours(iSessionID);
            gvEtudiants.DataSource = null;
            gvEtudiants.DataBind();
        }
    }
    protected void ddlCours_SelectedIndexChanged(object sender, EventArgs e)
    {
        //String sNumeroCours = ddlCours.SelectedValue.ToString();
        //if (sNumeroCours != "AAA")      // RemplirListeDeCours() uses it
        //{
        //    RemplirGridEtudiants(sNumeroCours);
        //}
        String sCoursOffertID = ddlCours.SelectedValue.ToString();
        if (sCoursOffertID != "AAA")      // RemplirListeDeCours() uses it
        {
            RemplirGridEtudiants(sCoursOffertID);
        }
        
    }
    void RemplirGridEtudiants(String sCoursOffertID) //unNumeroCours)
    {
        lblError.Text = String.Empty;

        try
        {
            int iSessionID = int.Parse(ddlSessions.SelectedValue.ToString());
            SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);

            //String sSql = String.Format("SELECT P.PersonneID,  P.Prenom, P.Nom, P.NIF, P.Telephone1, P.email, CP.NoteSurCent, CP.CoursPrisID, CO.NumeroCours, " + 
            //    " CO.SessionID  FROM Personnes P, CoursPris CP, CoursOfferts CO, Cours C  " + 
            // " WHERE P.PersonneID = CP.PersonneID AND CP.CoursOffertID = CO.CoursOffertID " + 
            // " AND CO.NumeroCours = C.NumeroCours  AND C.NumeroCours = '{0}'  AND CO.SessionID = {1} AND C.ExamenEntree = 0 " +
            //    " ORDER BY P.Nom, P.Prenom", unNumeroCours, iSessionID);

            String sSql = String.Format("SELECT P.PersonneID,  P.Prenom, P.Nom, P.NIF, P.Telephone1, P.email, CP.NoteSurCent, CP.CoursPrisID, CP.CoursOffertID, CO.NumeroCours, " +
                " CO.SessionID  FROM Personnes P, CoursPris CP, CoursOfferts CO, Cours C  " +
                " WHERE P.PersonneID = CP.PersonneID AND CP.CoursOffertID = CO.CoursOffertID " +
                " AND CO.NumeroCours = C.NumeroCours  AND CP.CoursOffertID = '{0}'  AND CO.SessionID = {1} AND C.ExamenEntree = 0 " +
                " ORDER BY P.Nom, P.Prenom", sCoursOffertID, iSessionID);
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
                foreach (GridViewRow dr in gvEtudiants.Rows)
                {
                    TextBox ctlNoteControle = (TextBox)dr.FindControl("txtNote");
                    Label ctlCoursPrisControle = (Label)dr.FindControl("lblCoursPrisID");
                    Double dNote = FormatNote(ctlNoteControle.Text.Trim());
                    Int16 iCoursPrisID = Int16.Parse(ctlCoursPrisControle.Text);

                    if (ctlNoteControle.Text.Trim() == String.Empty || ctlCoursPrisControle.Text.Trim() == "0"
                        || ctlNoteControle.Enabled == false || ctlNoteControle.Text.Trim() == "0")
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
            catch
            {
                lblError.Text = "ERREUR: Base de Données!";
            }
            String sNumeroCours = ddlCours.SelectedValue.ToString();
            RemplirGridEtudiants(sNumeroCours);
        }
        lblError.Text = iSaved + " Notes Mises à Jour! et " + iNotSaved + " Notes NON Sauvegardés!";
        db = null;
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
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx");
        //String sNumeroCours = ddlCours.SelectedValue.ToString();
        //RemplirGridEtudiants(sNumeroCours);
        //lblError.Text = "";
    }
    protected void gvEtudiants_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            TextBox ctlNote = (TextBox)e.Row.FindControl("txtNote");
            if (ctlNote == null)
                return;

            double dNoteSurCent = double.Parse(ctlNote.Text);
            if (dNoteSurCent > 0.0)             // Nòt sa antre deja, pa deranje l
            {
                ((TextBox)e.Row.FindControl("txtNote")).Enabled = false;
            }
        }
        catch (Exception ex)
        {
            string err = ex.Message;
        }
    }
}
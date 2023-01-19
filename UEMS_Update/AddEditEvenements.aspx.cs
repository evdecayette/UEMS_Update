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
public partial class AddEditEvenements : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string sEventID = string.Empty;
            try
            {                
                sEventID = Request.QueryString["eventid"]; 
            }
            catch (Exception Excep)
            {
                Debug.WriteLine(Excep.Message);
            }

            //
            if (sEventID != string.Empty && sEventID != null)     // Pas de correction - Nouvelle Entrée
            {
                RemplirInfoPourCorrection(sEventID);
            }
            ContentLiteralID.Text = BuildString();
        }
    }
    protected void RemplirInfoPourCorrection(string sEventID)
    {
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            

            try
            {
                sqlConn.Open();
                string sSql = string.Format("SELECT [EvenementID], CAST([EvenementDate] AS DATE) AS [EvenementDate], [EvenementDescription] " +
                    " FROM [Evenements] WHERE EvenementID = @EvenementID ORDER BY [EvenementDate] DESC ", sEventID);
                SqlParameter paramID = new SqlParameter("@EvenementID", SqlDbType.Int);
                paramID.Value = int.Parse(sEventID);

                SqlDataReader dt = db.GetDataReaderWithParams(sSql, sqlConn, paramID);
                if (dt != null)
                {
                    if (dt.Read())
                    {
                        //DateTime date = DateTime.Parse(dt["EvenementDate"].ToString());
                        Hidden_ID.Text = dt["EvenementID"].ToString();
                        txtDate.Text = DateTime.Parse(dt["EvenementDate"].ToString()).ToString("yyyy-MM-dd");     // date.Day.ToString() + "/" + date.Month.ToString() + "/" + date.Year.ToString();
                        txtDescription.Text = dt["EvenementDescription"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                db = null;
            }
        }
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ClearInfo();
    }
    String BuildString()
    {
        String returnedString = String.Empty;

        //returnedString += @"<div class='panel-body'>";
        returnedString += @"<table width='100%' class='table table-striped table-bordered table-hover' id='dataTables-Events'>";
        returnedString += @"<thead><tr><th></th><th>ID</th><th>Date</th><th>Description</th><th>Créé Par</th></thead><tbody>";

        // Loop through all records
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {            
            try
            {
                sqlConn.Open();
                string sSql = "SELECT [EvenementID], CAST([EvenementDate] AS DATE) AS [EvenementDate], [EvenementDescription] FROM [Evenements] ORDER BY [EvenementDate] DESC ";

                SqlDataReader dt = db.GetDataReader(sSql, sqlConn);
                if (dt != null)
                {
                    while (dt.Read())
                    {
                        returnedString += String.Format(@"<tr class='odd gradeX'><td><a href='AddEditEvenements.aspx?eventid={0}'>Edit</a></td>", dt["EvenementID"].ToString());
                        returnedString += String.Format(@"<td>{0}</td>", dt["EvenementID"].ToString());
                        returnedString += String.Format(@"<td>{0}</td><td>{1}</td>", dt["EvenementDate"].ToString(),
                            dt["EvenementDescription"].ToString());
                    }
                }
                db = null;
            }
            catch (Exception ex)
            {
                returnedString = ex.Message;
                return returnedString;
            }
            finally
            {
                db = null;
            }
        }
        returnedString += @"</tbody></table>";

        return returnedString;
    }
    protected void btnSauvegarder_Click(object sender, EventArgs e)
    {
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {            
            try
            {
                sqlConn.Open();
                if (txtDate.Text.Trim() == String.Empty)
                {
                    WriteErrorMessageToLabel("La Date est Obligatoire !!!", true);
                    txtDate.Focus();
                    return;
                }
                if (txtDescription.Text.Trim() == String.Empty)
                {
                    WriteErrorMessageToLabel("La Remarque est Obligatoire !!!", true);
                    txtDescription.Focus();
                    return;
                }

                string sEventID = string.Empty;
                try
                {
                    sEventID = Request.QueryString["eventid"];
                }
                catch (Exception Excep)
                {
                    Debug.WriteLine(Excep.Message);
                }

                SqlParameter paramDate = new SqlParameter("@EvenementDate", SqlDbType.Date);
                paramDate.Value = db.ScrubStringForDB(txtDate.Text);

                SqlParameter paramRemarque = new SqlParameter("@EvenementDescription", SqlDbType.Text);
                paramRemarque.Value = db.ScrubStringForDB(txtDescription.Text);

                //SqlParameter paramCreeParUsername = new SqlParameter("@CreeParUsername", SqlDbType.VarChar);
                //paramCreeParUsername.Value = db.GetWindowsUser();

                string sSql = string.Empty;
                if (sEventID != string.Empty && sEventID != null)     // Pas de correction - Nouvelle Entrée
                {
                    SqlParameter paramID = new SqlParameter("@EvenementID", SqlDbType.Int);
                    paramID.Value = int.Parse(sEventID);

                    sSql = String.Format("UPDATE Evenements SET EvenementDate = @EvenementDate, " +
                        "EvenementDescription = @EvenementDescription " +
                        "WHERE EvenementID = @EvenementID ");
                    if (db.IssueCommandWithParams(sSql, sqlConn, paramID, paramDate, paramRemarque))
                    {
                        WriteErrorMessageToLabel("Succès Sauvegarder !!!", false);
                        ClearInfo();
                    }
                    else
                    {
                        WriteErrorMessageToLabel("ERROR: PAS de Changement - (VOIR un technicien) !!!", true);
                    }
                }
                else
                {
                    sSql = String.Format("INSERT INTO Evenements (EvenementDate, EvenementDescription) " +
                        " VALUES (@EvenementDate, @EvenementDescription)");
                    if (db.IssueCommandWithParams(sSql, sqlConn, paramDate, paramRemarque))
                    {
                        WriteErrorMessageToLabel("Succès Sauvegarder !!!", false);
                        ClearInfo();
                    }
                    else
                    {
                        WriteErrorMessageToLabel("ERROR: PAS de sauvegarde - (VOIR un technicien) !!!", true);
                    }
                }
                ContentLiteralID.Text = BuildString();
                txtDescription.Focus();
                db = null;
            }
            catch (Exception ex)
            {
                WriteErrorMessageToLabel(ex.Message, true);
                db = null;
            }
        }
    }
    void ClearInfo()
    {
        txtDate.Text = String.Empty;
        txtDescription.Text = String.Empty;
    }
    protected void WriteErrorMessageToLabel(String Message, Boolean Error)
    {
        lblError.Text = Message;
        if (Error)
        {
            lblError.ForeColor = System.Drawing.Color.Red;
        }
        else
        {
            lblError.ForeColor = System.Drawing.Color.Green;
        }
    }
    protected void gvListEvenements_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            // Format the Date
            //TextBox ctlNote = (TextBox)e.Row.FindControl("EvenementDate");
            //((TextBox)e.Row.FindControl("EvenementDate")).Text = Utils.FixDate(ctlNote.Text);

        }
        catch (Exception ex)
        {
            string err = ex.Message;
        }
    }
    protected void gvListEvenements_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
}
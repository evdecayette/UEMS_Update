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

public partial class EditNotePasseeEtudiant : System.Web.UI.Page
{
    string ConnectionString = XCryptEngine.ConnectionStringEncryption.Decrypt(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            String sPersonneID = Request.QueryString["PersonneID"];
            txtPersonneID.Text = sPersonneID;
            BuildStringInfoGenerale(sPersonneID);
            RemplirGridCours(sPersonneID);
        }
    }

    void RemplirGridCours(String sPersonneID)
    {
        lblError.Text = String.Empty;

        try
        {
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            String sSql = String.Format("SELECT NoteSurCent, CP.NumeroCours + ' - ' + NomCours AS NumeroCours, CoursOffertID, NotePassage, " +  
                "CoursPrisID FROM CoursPris CP, Cours C WHERE CP.NumeroCours = C.NumeroCours AND PersonneID = '{0}' AND NoteSurCent = 0 AND Waiver = 0", sPersonneID);

            SqlDataAdapter da = new SqlDataAdapter(sSql, myConnection);
            DataTable dTable = new DataTable();
            da.Fill(dTable);

            gvNotesManquantes.DataSource = dTable;
            gvNotesManquantes.DataBind();
            myConnection.Close();
            myConnection = null;
        }
        catch (Exception ex)
        {
            lblError.Text = "ERREUR: " + ex.Message;
        }
    }


    void BuildStringInfoGenerale(string sPersonneID)
    {
        String returnedString = String.Empty;
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
        {
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
        int iSaved = 0, iNotSaved = 0;

        lblError.Text = "Patientez svp ...";
        Thread.Sleep(10);
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
        {
            try
            {
                sqlConn.Open();

                foreach (GridViewRow dr in gvNotesManquantes.Rows)
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
                lblError.Text = "ERREUR : Base de Données!";
            }
        }
        //RemplirGridCours(txtPersonneID.Text);
        Response.Redirect("EditNotePasseeEtudiant.aspx?PersonneID=" + txtPersonneID.Text.ToString());
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
    }
}
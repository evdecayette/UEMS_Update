using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web.UI.WebControls;

public partial class AddEditCursus : System.Web.UI.Page
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
                        RemplirDisciplines();
                        RemplirListeDeCours();
                        RemplirListeDeCoursDejaOfferts();
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
                    lblError.Text = "Erreur : Disciplines ou Sessions!";
                }
            }
        }
    }

    protected void RemplirDisciplines()
    {
        lblError.Text = String.Empty;
        try
        {
            SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);

            String sSql = @"SELECT 0 AS DisciplineID, 'AA' AS DisciplineNom, ' Choisissez Discipline   ' AS Description " +
                "UNION SELECT DisciplineID, DisciplineNom, ' ' + DisciplineNom + '   ' AS Description FROM Disciplines ORDER BY DisciplineNom";
            SqlDataAdapter da = new SqlDataAdapter(sSql, myConnection);
            DataTable dTable = new DataTable();
            da.Fill(dTable);

            ddlDisciplines.DataSource = dTable;
            ddlDisciplines.DataBind();
            myConnection.Close();
            myConnection = null;

            RemplirListeDeCours();
            RemplirListeDeCoursDejaOfferts();
        }
        catch (Exception ex)
        {
            lblError.Text = "ERREUR: " + ex.Message;
        }
    }
    protected void RemplirListeDeCoursDejaOfferts()
    {
        lblError.Text = String.Empty;
        int Discipline = int.Parse(ddlDisciplines.SelectedValue.ToString());
        if (Discipline == 0)
            return;
        try
        {
            SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
            String sSql = String.Format("SELECT C.NumeroCours, C.NumeroSession, CO.NomCours, C.CursusID, C.DisciplineID " +
                " FROM Cursus C, Cours CO WHERE C.NumeroCours = CO.NumeroCours AND  DisciplineID = {0} ORDER BY NumeroSession ", ddlDisciplines.SelectedValue.ToString());
            SqlDataAdapter da = new SqlDataAdapter(sSql, myConnection);
            DataTable dTable = new DataTable();
            da.Fill(dTable);

            gvCoursDejaOfferts.DataSource = dTable;
            gvCoursDejaOfferts.DataBind();

            // Section importante pour BootStrap
            if (gvCoursDejaOfferts.Rows.Count > 0)
            {
                //Adds THEAD and TBODY Section.
                gvCoursDejaOfferts.HeaderRow.TableSection = TableRowSection.TableHeader;

                //Adds TH element in Header Row.  
                gvCoursDejaOfferts.UseAccessibleHeader = true;

                //Adds TFOOT section. 
                gvCoursDejaOfferts.FooterRow.TableSection = TableRowSection.TableFooter;
            }

            myConnection.Close();
            myConnection = null;
        }
        catch (Exception ex)
        {
            lblError.Text = "ERREUR: Base de Données à vérifier avant tout!";
            Debug.WriteLine(ex.Message);
        }
    }
    protected void RemplirListeDeCours()
    {
        lblError.Text = String.Empty;
        int Discipline = int.Parse(ddlDisciplines.SelectedValue.ToString());
        if (Discipline == 0)
            return;
        try
        {
            SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
            String sSql = String.Format("SELECT NomCours, NumeroCours FROM Cours " +
                    " WHERE NumeroCours <> 'NOP' AND Actif = 1 AND ExamenEntree = 0 " +
                    " AND NumeroCours NOT IN (SELECT NumeroCours FROM Cursus WHERE DisciplineID = {0})" +
                    " ORDER BY NumeroCours ", Discipline);
            SqlDataAdapter da = new SqlDataAdapter(sSql, myConnection);
            DataTable dTable = new DataTable();
            da.Fill(dTable);

            gvCoursDisponibles.DataSource = dTable;
            gvCoursDisponibles.DataBind();

            // Section importante pour BootStrap
            if (gvCoursDisponibles.Rows.Count > 0)
            {
                //Adds THEAD and TBODY Section.
                gvCoursDisponibles.HeaderRow.TableSection = TableRowSection.TableHeader;

                //Adds TH element in Header Row.  
                gvCoursDisponibles.UseAccessibleHeader = true;

                //Adds TFOOT section. 
                gvCoursDisponibles.FooterRow.TableSection = TableRowSection.TableFooter;
            }

            myConnection.Close();
            myConnection = null;
        }
        catch (Exception ex)
        {
            lblError.Text = "ERREUR: Possiblement Base de Données! ";
            Debug.WriteLine(ex.Message);
        }
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        // ToDo : Reset everything but don't leave page
        Response.Redirect("Default.aspx");
    }
    protected void btnSauvegarder_Click(object sender, EventArgs e)
    {
        btnSauvegarder.Enabled = false;

        lblError.Text = String.Empty;
        String erreurString = String.Empty;
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            try
            {
                sqlConn.Open();
                SqlParameter ParamNumeroCours = new SqlParameter("@NumeroCours", SqlDbType.Text);
                SqlParameter ParamNumeroSession = new SqlParameter("@NumeroSession", SqlDbType.Int);
                SqlParameter ParamDisciplineID = new SqlParameter("@DisciplineID", SqlDbType.Int);
                SqlParameter paramUserName = new SqlParameter("@CreeParUserName", SqlDbType.Text);
                paramUserName.Value = db.GetWindowsUser();

                for (int iRow = 0; iRow < gvCoursDisponibles.Rows.Count; iRow++)
                {
                    if (gvCoursDisponibles.Rows[iRow].Enabled)
                    {
                        if (((CheckBox)gvCoursDisponibles.Rows[iRow].FindControl("chkCours")).Checked == true)
                        {
                            // Insérer dans table                   
                            try
                            {
                                String sSql = "INSERT INTO Cursus (NumeroCours, NumeroSession, DisciplineID, CreeParUserName) " +
                                    " VALUES (@NumeroCours, @NumeroSession, @DisciplineID, @CreeParUserName)";
                                ParamNumeroCours.Value = ((Label)gvCoursDisponibles.Rows[iRow].FindControl("lblNumeroCours")).Text;
                                int iPeriode = int.Parse(((DropDownList)gvCoursDisponibles.Rows[iRow].FindControl("ddlPeriode")).SelectedValue.ToString());
                                ParamDisciplineID.Value = ddlDisciplines.SelectedValue.ToString();
                                if (iPeriode <= 0)
                                {
                                    continue;
                                }
                                ParamNumeroSession.Value = iPeriode;
                                if (!db.IssueCommandWithParams(sSql, sqlConn, ParamNumeroCours, ParamNumeroSession, ParamDisciplineID, paramUserName))
                                {
                                    lblError.Text += String.Format("Erreur ...Erreur ... Vérifiez Valeur Période Pour {0}!", ParamNumeroCours.Value);
                                    erreurString += lblError.Text;
                                    continue;
                                }
                                gvCoursDisponibles.Rows[iRow].Enabled = false;
                            }
                            catch (Exception ex)
                            {
                                lblError.Text = "ERREUR: 1. ";
                                Debug.WriteLine(ex.Message);
                            }
                        }
                    }
                }
            }
            catch
            {
                lblError.Text = "Erreur: 2. Base de Données Inaccessible!";
            }
            db = null;
            btnSauvegarder.Enabled = true;
            RemplirListeDeCoursDejaOfferts();
        }
    }
    protected void ddlDisciplines_SelectedIndexChanged(object sender, EventArgs e)
    {
        RemplirListeDeCours();
        RemplirListeDeCoursDejaOfferts();
    }
    protected void gvCoursDisponibles_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            // Fill the lign detail dropdown box
            if (e.Row.RowType == DataControlRowType.Pager || e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    DropDownList ddl = (DropDownList)e.Row.FindControl("ddlPeriode");
                    if (ddl != null)
                    {
                        int nbreSemestre = int.Parse(ConfigurationManager.AppSettings["NombreDeSemestre"].ToString());
                        for (int i = 0; i < nbreSemestre + 1; i++)
                        {
                            if (i == 0)
                                ddl.Items.Add(new ListItem("Choisissez Période", "0"));
                            else
                            {
                                ListItem item = new ListItem("Session # " + i.ToString(), i.ToString());
                                ddl.Items.Add(item);
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Print(ex.Message);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Configuration;

public partial class AdminPages_ChoisirCoursPourSession : System.Web.UI.Page
{
    string ConnectionString = XCryptEngine.ConnectionStringEncryption.Decrypt(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            String sGroup = ConfigurationManager.AppSettings["AdminGroup"].ToString();
            DB_Access db = new DB_Access();
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                try
                {
                    sqlConn.Open();
                    if (db.isCurrentWindowsInThisGroup(sGroup) || (db.IsCustomAdminMember(db.GetWindowsUser(), sqlConn)))
                    {
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
                    lblError.Text = "ERREUR = Base de Données!";
                }
            }
        }
    }

    void RemplirListeDeCours()
    {
        lblError.Text = String.Empty;
        //btnSauvegarder.Enabled = false;
        try
        {
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            //String sSql = @"SELECT NomCours, NumeroCours FROM Cours " +
            //    " WHERE NumeroCours <> 'NOP' AND NumeroCours NOT IN (SELECT CO.NumeroCours FROM CoursOfferts CO, LesSessions LS WHERE CO.SessionID = LS.SessionID AND LS.SessionCourante = 1) ORDER BY NumeroCours";
            String sSql = @"SELECT NomCours + '  (' + CoursPreRequis + ')' AS NomCours, NumeroCours FROM Cours " +
                    " WHERE NumeroCours <> 'NOP' AND Actif = 1 AND ExamenEntree = 0 ORDER BY NumeroCours";

            SqlDataAdapter da = new SqlDataAdapter(sSql, myConnection);
            DataTable dTable = new DataTable();
            da.Fill(dTable);

            gvCoursDisponibles.DataSource = dTable;
            gvCoursDisponibles.DataBind();
            myConnection.Close();
            myConnection = null;
        }
        catch (Exception ex)
        {
            lblError.Text = "ERREUR: " + ex.Message;
        }
    }

    void RemplirListeDeCoursDejaOfferts()
    {
        lblError.Text = String.Empty;
        //btnSauvegarder.Enabled = false;
        try
        {
            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {
                String sSql = @"SELECT C.NumeroCours, C.NumeroCours + ' - ' + NomCours + ' - ' + H.Jours + ' * ' + H.HeureDebut + ' - ' + H.HeureFin + ' *  (' + CoursPreRequis + ') ' + Isnull(H.Remarque, '') AS Description " +
                    " FROM CoursOfferts CO, Cours C, Horaires H  WHERE CO.NumeroCours = C.NumeroCours AND CO.HoraireID = H.HoraireID AND CO.Actif = 1  AND C.ExamenEntree = 0 " +
                    " AND SessionId IN (SELECT SessionID From LesSessions WHERE SessionCourante = 1) ORDER BY C.NumeroCours";

                SqlDataAdapter da = new SqlDataAdapter(sSql, myConnection);
                DataTable dTable = new DataTable();
                da.Fill(dTable);

                gvCoursDejaOfferts.DataSource = dTable;
                gvCoursDejaOfferts.DataBind();
            }
        }
        catch (Exception ex)
        {
            lblError.Text = "ERREUR: " + ex.Message;
        }
    }
    protected void gvCoursDisponibles_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            // Fill the lign detail dropdown box
            if (e.Row.RowType == DataControlRowType.Pager || e.Row.RowType == DataControlRowType.DataRow)
            {
                SqlConnection myConnection = new SqlConnection(ConnectionString);
                String sSql = @" SELECT 'Choisissez le Professeur' AS NomComplet, 0 AS ProfesseurID, 'AAA' AS Prenom " +
                    " UNION SELECT Prenom + ' ' + Nom AS NomComplet, ProfesseurID, Prenom FROM Professeurs WHERE Actif = 1 ORDER BY Prenom";

                SqlDataAdapter da = new SqlDataAdapter(sSql, myConnection);
                DataTable dTable = new DataTable();
                da.Fill(dTable);

                sSql = @" SELECT 'Choisissez Heure' AS Heures, 0 AS HoraireID " +
                    " UNION SELECT HeureDebut + '-' + HeureFin + '(' + Jours + ')' AS Heures, HoraireID FROM Horaires";
                SqlDataAdapter daHeure = new SqlDataAdapter(sSql, myConnection);
                DataTable dTableHeure = new DataTable();
                daHeure.Fill(dTableHeure);

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                    DropDownList ddlEdit = (DropDownList)e.Row.FindControl("ddlProfesseurs");
                    if (ddlEdit != null)
                    {
                        ((DropDownList)e.Row.FindControl("ddlProfesseurs")).DataSource = dTable;
                        ((DropDownList)e.Row.FindControl("ddlProfesseurs")).DataBind();
                    }

                    DropDownList ddlEdit2 = (DropDownList)e.Row.FindControl("ddlHoraire");
                    if (ddlEdit2 != null)
                    {
                        ((DropDownList)e.Row.FindControl("ddlHoraire")).DataSource = dTableHeure;
                        ((DropDownList)e.Row.FindControl("ddlHoraire")).DataBind();
                    }
                }

                myConnection.Close();
                myConnection = null;
            }
        }
        catch (Exception ex)
        {
            Debug.Print(ex.Message);
        }
    }
    
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }

    protected void btnSauvegarder_Click(object sender, EventArgs e)
    {
        btnSauvegarder.Enabled = false;
        lblError.Text = String.Empty;
        String erreurString = String.Empty;
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
        {
            try
            {
                sqlConn.Open();
                SqlParameter ParamNumeroCours = new SqlParameter("@NumeroCours", SqlDbType.Text);
                SqlParameter ParamProfesseurID = new SqlParameter("@ProfesseurID", SqlDbType.Int);
                SqlParameter ParamHoraireID = new SqlParameter("@HoraireID", SqlDbType.Int);
                SqlParameter ParamSessionID = new SqlParameter("@SessionID", SqlDbType.Int);
                using (SqlConnection sqlConn1 = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        sqlConn1.Open();
                        ParamSessionID.Value = db.GetIdOfSessionCourante(sqlConn1);
                    }
                    catch(Exception ex)
                    {
                        lblError.Text = "ERREUR 1 Sauvegarder: ChoisirCoursPourSession";
                        Debug.WriteLine(ex.Message);
                    }
                }
                SqlParameter ParamNotePassage = new SqlParameter("@NotePassage", SqlDbType.Float);
                SqlParameter paramUserName = new SqlParameter("@CreeParUserName", SqlDbType.Text);
                paramUserName.Value = db.GetWindowsUser();

                for (int iRow = 0; iRow < gvCoursDisponibles.Rows.Count; iRow++)
                {
                    if (gvCoursDisponibles.Rows[iRow].Enabled)
                    {
                        if (((CheckBox)gvCoursDisponibles.Rows[iRow].FindControl("chkCours")).Checked == true)
                        {
                            // Insérer dans table CoursOfferts pour la Session Courante                    
                            try
                            {
                                String sSql = "INSERT INTO CoursOfferts (NumeroCours, ProfesseurID, HoraireID, SessionID, NotePassage, CreeParUserName) " +
                                    " VALUES (@NumeroCours, @ProfesseurID, @HoraireID, @SessionID, @NotePassage, @CreeParUserName)";

                                ParamNumeroCours.Value = ((Label)gvCoursDisponibles.Rows[iRow].FindControl("lblNumeroCours")).Text;
                                ParamProfesseurID.Value = ((DropDownList)gvCoursDisponibles.Rows[iRow].FindControl("ddlProfesseurs")).SelectedValue.ToString();
                                // Cheke si pwofesè antre
                                if (int.Parse(ParamProfesseurID.Value.ToString()) == 0)
                                {
                                    lblError.Text += String.Format("Information manquante: Professeur");
                                    continue;
                                }

                                ParamHoraireID.Value = ((DropDownList)gvCoursDisponibles.Rows[iRow].FindControl("ddlHoraire")).SelectedValue.ToString();
                                // Cheke si Horè antre
                                if (int.Parse(ParamHoraireID.Value.ToString()) == 0)
                                {
                                    lblError.Text += String.Format("Information manquante: Horaire");
                                    continue;
                                }

                                if (((TextBox)gvCoursDisponibles.Rows[iRow].FindControl("txtNotePassage")).Text == String.Empty)
                                {
                                    lblError.Text += String.Format("Information manquante: Note de Passage");
                                    continue;
                                }
                                ParamNotePassage.Value = Double.Parse(((TextBox)gvCoursDisponibles.Rows[iRow].FindControl("txtNotePassage")).Text);
                                // cheke si Nòt pasaj antre
                                if (!db.IssueCommandWithParams(sSql, sqlConn, ParamNumeroCours, ParamProfesseurID, ParamHoraireID, ParamSessionID, ParamNotePassage, paramUserName))
                                {
                                    lblError.Text += "Erreur ...Erreur ... Vérifiez si le cours est déjà offert avec cet horaire!";
                                    erreurString += "Erreur: Création de Cours Offerts!!!";
                                    continue;
                                }
                                gvCoursDisponibles.Rows[iRow].Enabled = false;
                            }
                            catch (Exception ex)
                            {
                                lblError.Text = "ERREUR 2 Sauvegarder: ChoisirCoursPourSession";
                                Debug.WriteLine(ex.Message);
                            }
                        }
                    }
                }
            }
            catch
            {
                lblError.Text = "Erreur Base de Données!";
            }
        }
        db = null;

        //btnSauvegarder.Enabled = true;
        //RemplirListeDeCoursDejaOfferts();
        Response.Redirect("ChoisirCoursPourSession.aspx"); 
    }
}
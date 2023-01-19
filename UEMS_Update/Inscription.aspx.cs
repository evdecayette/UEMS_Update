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

public partial class Inscription : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string sPersonneID = string.Empty;
            string sEditFlag = string.Empty;

            try
            {
                sPersonneID = Request.QueryString["PersonneID"];
                txtPersonneID.Text = sPersonneID;
                // Enlever le cookie
                HttpCookie cookie = new HttpCookie("PersonneID", string.Empty); // Second method, just in case
                Response.Cookies.Set(cookie);
                Response.Cookies.Remove("PersonneID");                          // First method
            }
            catch (Exception Excep)
            {
                Debug.WriteLine(Excep.Message);
            }

            RemplirListeDisciplines();
            //
            if (sPersonneID == string.Empty)     // Pas de correction - Nouvelle Entrée
            {
                txtPrenom.Focus();
            }
            else
            {
                sEditFlag = Request.QueryString["EditFlag"]; 
                // Enlever le cookie
                Response.Cookies.Remove("EditFlag");                          // First method
                HttpCookie cookieEditFlag = new HttpCookie("EditFlag", string.Empty); // Second method, just in case
                Response.Cookies.Set(cookieEditFlag);
                if (sEditFlag == "1")
                {
                    RemplirInfoPourCorrection(sPersonneID);
                }
            }
            
        }
    }
    void SetDisciplineEtudiant(String sDisciplineID)
    {
        try
        {
            string sOldSelection = "";
            string sNewSelection = sDisciplineID;

            if (ddlDisciplines.Items.FindByValue(sNewSelection) != null)
            {
                sOldSelection = ddlDisciplines.SelectedItem.Value;
                ddlDisciplines.Items.FindByValue(sNewSelection).Selected = true;
                if (sOldSelection != sNewSelection) // Pour ne pas désélectionner le nouveau choix
                {
                    ddlDisciplines.Items.FindByValue(sOldSelection).Selected = false;
                }
            }
            ViewState["OldDiscipline"] = sDisciplineID;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }
    void ChangeDisciplineEtudiant()
    {
        try
        {
            string OldDisciplineID = ViewState["OldDiscipline"].ToString();
            string NewDisciplineID = ViewState["NewDiscipline"].ToString(); 
            if (OldDisciplineID == NewDisciplineID)
                return;     // No Changes

            try
            {
                // Update Table DisciplineChangee if Discipline Change
                DB_Access db = new DB_Access();
                using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
                {
                    sqlConn.Open();
                    string sSql = String.Format("INSERT INTO DisciplineChangee (PersonneID, DeDisciplineID, ADisciplineID, ParUserName) " +
                    " VALUES (@PersonneID, @DeDisciplineID, @ADisciplineID, @ParUsername)");

                    SqlParameter paramDeDisciplineID = new SqlParameter("@DeDisciplineID", SqlDbType.Int);
                    paramDeDisciplineID.Value = OldDisciplineID;    //dbDiscipline.GetDisciplineIDFromName(int.Parse(sOldSelection));

                    SqlParameter paramADisciplineID = new SqlParameter("@ADisciplineID", SqlDbType.Int);
                    paramADisciplineID.Value = NewDisciplineID;     //dbDiscipline.GetDisciplineIDFromName(int.Parse(sNewSelection));

                    SqlParameter paramPersonneID;
                    paramPersonneID = new SqlParameter("@PersonneID", SqlDbType.NVarChar);
                    paramPersonneID.Value = txtPersonneID.Text;

                    SqlParameter paramParUsername = new SqlParameter("@ParUsername", SqlDbType.VarChar);
                    paramParUsername.Value = db.GetWindowsUser();

                    db.IssueCommandWithParams(sSql, sqlConn, paramPersonneID, paramDeDisciplineID, paramADisciplineID,
                        paramParUsername);
                }
            }
            catch (Exception ex)
            {
                WriteErrorMessageToLabel(string.Format("Discipline: Error ({0}) !!!", ex.Message), true);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }
    void RemplirListeDisciplines()
    {
        lblError.Text = String.Empty;
        try
        {
            SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
            String sSql = @"SELECT 'Choisissez une Discipline' as DisciplineNom, -1 as DisciplineID " +
                " UNION SELECT DisciplineNom, DisciplineID FROM Disciplines";

            SqlDataAdapter da = new SqlDataAdapter(sSql, myConnection);
            DataTable dTable = new DataTable();
            da.Fill(dTable);

            ddlDisciplines.DataSource = dTable;
            ddlDisciplines.DataValueField = "DisciplineID";
            ddlDisciplines.DataTextField = "DisciplineNom";
            ddlDisciplines.DataBind();

            myConnection.Close();
            myConnection = null;
        }
        catch (Exception ex)
        {
            lblError.Text = "ERREUR: " + ex.Message;
        }
    }
    protected void RemplirInfoPourCorrection(String sPersonneID)
    {
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            try
            {
                sqlConn.Open();
                //String sSql = String.Format("SELECT * FROM Personnes WHERE PersonneID = '{0}'", sPersonneID);
                String sSql = String.Format("SELECT PersonneID,Prenom,Nom,ISNULL(Telephone1, '') AS Telephone1 ,ISNULL(Telephone2, '') AS Telephone2,ISNULL(DDN, '') AS DDN,ISNULL(AdresseRue, '') AS AdresseRue" +
                    ",ISNULL(AdresseExtra, '') AS AdresseExtra,ISNULL(Ville, '') AS Ville,ISNULL(Pays, '') AS Pays,DateCreee,ISNULL(Remarque, '') AS Remarque, Etudiant, AdminStaff, Photo" +
                    ",ISNULL(UserNameAttribue, '') AS UserNameAttribue, CreeParUsername, ISNULL(NumeroRecu, '') AS NumeroRecu,ISNULL(NIF, '') AS NIF,ISNULL(email, '') AS email" +
                    ",DisciplineID" +
                    " FROM [UEspoirDB].[dbo].[Personnes] WHERE PersonneID = '{0}'", sPersonneID);

                SqlDataReader dtTemp = db.GetDataReader(sSql, sqlConn);
                if (dtTemp.Read())
                {
                    // Fill info
                    txtPersonneID.Text = sPersonneID;
                    txtPrenom.Text = dtTemp["Prenom"].ToString();
                    txtPrenom.Enabled = false;
                    txtNom.Text = dtTemp["Nom"].ToString();
                    txtNom.Enabled = false;

                    txtDDN.TextMode = TextBoxMode.SingleLine;

                    if (dtTemp["DDN"].ToString() != String.Empty)
                    {
                        DateTime dt = DateTime.Parse(dtTemp["DDN"].ToString());
                        txtDDN.Text = (dt.Day < 10 ? "0" + dt.Day.ToString() : dt.Day.ToString()) + "-" + dt.ToString("MMM") +
                            "-" + dt.Year.ToString();
                    }
                    txtDDN.Enabled = false;
                    // 
                    txtAdresseRue.Text = dtTemp["AdresseRue"].ToString();
                    txtAdresseExtra.Text = dtTemp["AdresseExtra"].ToString();
                    txtVille.Text = dtTemp["Ville"].ToString();
                    txtPays.Text = dtTemp["Pays"].ToString();
                    txtEmail.Text = dtTemp["Email"].ToString();
                    txtTelephone1.Text = dtTemp["Telephone1"].ToString();
                    txtTelephone2.Text = dtTemp["Telephone2"].ToString();
                    txtNIF.Text = dtTemp["NIF"].ToString();
                    txtRemarque.Text = dtTemp["Remarque"].ToString();
                }
                db = null;
                SetDisciplineEtudiant(dtTemp["DisciplineID"].ToString());
            }
            catch (Exception ex)
            {
                lblError.Text = string.Format("ERROR : {0} !!!", ex.Message);
                lblError.ForeColor = System.Drawing.Color.Red;
                db = null;
            }
        }
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ClearInfo();
    }
    void ClearInfo()
    {
        // Clear info
        txtPrenom.Text = String.Empty;
        txtNom.Text = String.Empty;
        txtAdresseRue.Text = String.Empty;
        txtAdresseExtra.Text = String.Empty;
        txtVille.Text = String.Empty;
        txtPays.Text = String.Empty;
        txtEmail.Text = String.Empty;
        txtDDN.Text = String.Empty;
        txtTelephone1.Text = String.Empty;
        txtTelephone2.Text = String.Empty;
        txtNIF.Text = String.Empty;
        txtRemarque.Text = String.Empty;

        ddlDisciplines.SelectedIndex = -1;

        txtPrenom.Focus();
    }
    protected void btnSauvegarder_Click(object sender, EventArgs e)
    {
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            try
            {
                sqlConn.Open();
                if (txtPrenom.Text.Trim() == String.Empty)
                {
                    WriteErrorMessageToLabel("Le Prénom est Obligatoire !!!", true);
                    txtPrenom.Focus();
                    return;
                }

                if (txtNom.Text.Trim() == String.Empty)
                {
                    WriteErrorMessageToLabel("Le Nom est Obligatoire !!!", true);
                    txtNom.Focus();
                    return;
                }

                if (txtEmail.Text.Trim() == String.Empty)
                {
                    WriteErrorMessageToLabel("Le Courriel/Email est Obligatoire !!!", true);
                    txtEmail.Focus();
                    return;
                }

                // ajouter logique pour chequer DDN
                //if (txtEmail.Text.Trim() == String.Empty)
                //{
                //    WriteErrorMessageToLabel("Le Courriel/Email est Obligatoire !!!", true);
                //    txtEmail.Focus();
                //    return;
                //}

                int iDiscipline = Int16.Parse(ddlDisciplines.SelectedValue) > 0 ? Int16.Parse(ddlDisciplines.SelectedValue) : 0;
                if (iDiscipline == 0)
                {
                    WriteErrorMessageToLabel("Choisissez une Discipline !!!", true);
                    ddlDisciplines.Focus();
                    return;
                }

                // Everything is ok: Save!
                string sSql = String.Format("INSERT INTO Personnes (PersonneID, Prenom, Nom, AdresseRue, AdresseExtra, Ville, Pays, " +
                    "Email, DDN, Telephone1,  Telephone2, NIF, DisciplineInitiale, Remarque, CreeParUsername, DisciplineID) " +
                    " VALUES (@PersonneID, @Prenom, @Nom, @AdresseRue, @AdresseExtra, @Ville, @Pays, @Email, @DDN, @Telephone1," +
                    " @Telephone2, @NIF, @DisciplineInitiale, @Remarque, @CreeParUsername, @DisciplineID)");

                if (txtPrenom.Enabled == false && txtNom.Enabled == false)
                {
                    // Everything is ok: Update!
                    sSql = String.Format("UPDATE Personnes SET AdresseRue = @AdresseRue, AdresseExtra = @AdresseExtra, Ville = @Ville, Pays = @Pays, " +
                        " Email = @Email, Telephone1 = @Telephone1, Telephone2 = @Telephone2, NIF = @NIF, Remarque = @Remarque, CreeParUsername = @CreeParUsername, " +
                        " DisciplineID = @DisciplineID WHERE PersonneID = @PersonneID");
                }

                SqlParameter paramDisciplineID = new SqlParameter("@DisciplineID", SqlDbType.Int);
                paramDisciplineID.Value = iDiscipline;

                SqlParameter paramDisciplineInitiale = new SqlParameter("@DisciplineInitiale", SqlDbType.Text);
                paramDisciplineInitiale.Value = ddlDisciplines.SelectedItem.Text;

                SqlParameter paramPrenom = new SqlParameter("@Prenom", SqlDbType.Text);
                paramPrenom.Value = db.ScrubStringForDB(txtPrenom.Text);

                SqlParameter paramNom = new SqlParameter("@Nom", SqlDbType.Text);
                paramNom.Value = db.ScrubStringForDB(txtNom.Text);

                SqlParameter paramAdresseRue = new SqlParameter("@AdresseRue", SqlDbType.Text);
                paramAdresseRue.Value = db.ScrubStringForDB(txtAdresseRue.Text);

                SqlParameter paramAdresseExtra = new SqlParameter("@AdresseExtra", SqlDbType.Text);
                paramAdresseExtra.Value = db.ScrubStringForDB(txtAdresseExtra.Text);

                SqlParameter paramVille = new SqlParameter("@Ville", SqlDbType.Text);
                paramVille.Value = db.ScrubStringForDB(txtVille.Text);

                SqlParameter paramPays = new SqlParameter("@Pays", SqlDbType.Text);
                paramPays.Value = db.ScrubStringForDB(txtPays.Text);

                SqlParameter paramEmail = new SqlParameter("@Email", SqlDbType.Text);
                paramEmail.Value = db.ScrubStringForDB(txtEmail.Text);

                SqlParameter paramDDN = new SqlParameter("@DDN", SqlDbType.Date);
                paramDDN.Value = db.ScrubStringForDB(txtDDN.Text);

                SqlParameter paramTelephone1 = new SqlParameter("@Telephone1", SqlDbType.Text);
                paramTelephone1.Value = db.ScrubStringForDB(txtTelephone1.Text);

                SqlParameter paramTelephone2 = new SqlParameter("@Telephone2", SqlDbType.Text);
                paramTelephone2.Value = db.ScrubStringForDB(txtTelephone2.Text);

                SqlParameter paramNIF = new SqlParameter("@NIF", SqlDbType.Text);
                paramNIF.Value = db.ScrubStringForDB(txtNIF.Text);

                SqlParameter paramPersonneID;
                if (txtPrenom.Enabled == true && txtNom.Enabled == true)
                {
                    paramPersonneID = new SqlParameter("@PersonneID", SqlDbType.UniqueIdentifier);
                    paramPersonneID.Value = Guid.NewGuid();
                }
                else
                {
                    paramPersonneID = new SqlParameter("@PersonneID", SqlDbType.NVarChar);
                    paramPersonneID.Value = txtPersonneID.Text;
                }

                SqlParameter paramRemarque = new SqlParameter("@Remarque", SqlDbType.Text);
                paramRemarque.Value = db.ScrubStringForDB(txtRemarque.Text);

                SqlParameter paramCreeParUsername = new SqlParameter("@CreeParUsername", SqlDbType.VarChar);
                paramCreeParUsername.Value = db.GetWindowsUser();

                if (db.IssueCommandWithParams(sSql, sqlConn, paramPersonneID, paramPrenom, paramNom, paramAdresseRue, paramAdresseExtra,
                    paramVille, paramPays, paramEmail, paramDDN, paramTelephone1, paramTelephone2,
                    paramNIF, paramDisciplineInitiale, paramRemarque, paramCreeParUsername, paramDisciplineID))
                {
                    WriteErrorMessageToLabel("SUCCES Sauvegarder !!!", false);
                    ViewState["NewDiscipline"] = iDiscipline.ToString();                 // before Clearing
                    ClearInfo();

                    if (txtPrenom.Enabled == false && txtNom.Enabled == false)
                    {
                        // We are dealing with an update and not a new student.
                        ChangeDisciplineEtudiant();
                        Response.Redirect("Inscription.aspx"); // Pour éviter tout REFRESH avec les données courantes.
                        return;
                    }
                    SqlParameter paramPersonneID_Text = new SqlParameter("@PersonneID", SqlDbType.Text);
                    paramPersonneID_Text.Value = paramPersonneID.Value.ToString();

                    // TODO: Candidat pour transaction Database

                    // Cours hard coded pour l'instant: FRA001 et MAT001
                    SqlParameter paramNumeroCours = new SqlParameter("@NumeroCours", SqlDbType.Text);
                    SqlParameter paramCoursOffertID = new SqlParameter("@CoursOffertID", SqlDbType.Int);
                    using (SqlConnection sqlConnExamen = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
                    {
                        try
                        {
                            sqlConnExamen.Open();
                            paramNumeroCours.Value = "FRA001";
                            paramCoursOffertID.Value = db.GetCoursOffertID("FRA001", sqlConnExamen);
                            db.IssueCommandWithParams("INSERT INTO CoursPris (PersonneID, NumeroCours, CreeParUsername, CoursOffertID) " +
                                " VALUES (@PersonneID, @NumeroCours, @CreeParUsername, @CoursOffertID)", sqlConnExamen,
                                paramPersonneID_Text, paramNumeroCours, paramCreeParUsername, paramCoursOffertID);

                            paramNumeroCours.Value = "MAT001";
                            paramCoursOffertID.Value = db.GetCoursOffertID("MAT001", sqlConnExamen);
                            db.IssueCommandWithParams("INSERT INTO CoursPris (PersonneID, NumeroCours, CreeParUsername, CoursOffertID) " +
                                " VALUES (@PersonneID, @NumeroCours, @CreeParUsername, @CoursOffertID)", sqlConnExamen,
                                paramNumeroCours, paramPersonneID_Text, paramCreeParUsername, paramCoursOffertID);

                            paramNumeroCours.Value = "ANG001";
                            paramCoursOffertID.Value = db.GetCoursOffertID("ANG001", sqlConnExamen);
                            db.IssueCommandWithParams("INSERT INTO CoursPris (PersonneID, NumeroCours, CreeParUsername, CoursOffertID) " +
                                " VALUES (@PersonneID, @NumeroCours, @CreeParUsername, @CoursOffertID)", sqlConnExamen,
                                paramNumeroCours, paramPersonneID_Text, paramCreeParUsername, paramCoursOffertID);

                            // Montant du à l'inscription
                            db.IssueCommand(String.Format("INSERT INTO MontantsDus (PersonneID, CodeObligation, Montant,SessionID) VALUES ('{0}','{1}',{2},(Select SessionID from LesSessions where SessionCourante =1))",
                                paramPersonneID_Text.Value, "FI", 500), sqlConnExamen);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }
                }
                else
                {
                    WriteErrorMessageToLabel("ERROR: PAS de sauvegarde - Peut-Etre un Duplicatat de nom et prenom (VOIR un technicien) !!!", true);
                }
                txtPrenom.Focus();
                db = null;
            }
            catch (Exception ex)
            {
                WriteErrorMessageToLabel(ex.Message, true);
                db = null;
            }
        }
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

    protected void txtPrenom_TextChanged(object sender, EventArgs e)
    {
        char [] prenom = txtPrenom.Text.ToArray<char>();        
        txtPrenom.Text.ToLower();
        txtPrenom.Text = prenom[0].ToString().ToUpper() + txtPrenom.Text.Substring(1);
        txtNom.Focus();
    }

    protected void txtNom_TextChanged(object sender, EventArgs e)
    {
        char[] nom = txtNom.Text.ToArray<char>();
        txtNom.Text.ToLower();
        txtNom.Text = nom[0].ToString().ToUpper() + txtNom.Text.Substring(1);
        txtAdresseRue.Focus();
    }

    protected void ddlDisciplines_SelectedIndexChanged(object sender, EventArgs e)
    {
    }
}
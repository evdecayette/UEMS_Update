using System;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;

public partial class InfoEtudiant : System.Web.UI.Page
{
    String sPersonneID = String.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Get PersonneID 
            try
            {
                sPersonneID = Request.QueryString["PersonneID"];
                txtPersonneID.Text = sPersonneID;
                Debug.WriteLine(sPersonneID);
                //LitEtudiantFooter.Text = string.Format("<a href='RecevoirPaiements.aspx?PersonneID={0}'>Ecran Des Paiements</a>", sPersonneID);
                //sPersonneID = Request.Cookies.Get("PersonneID").Value.ToString();
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
                //Permission
                String sGroup = ConfigurationManager.AppSettings["AdminGroup"].ToString();
                DB_Access db = new DB_Access();
                using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
                {
                    sqlConn.Open();
                    if (db.isCurrentWindowsInThisGroup(sGroup) || (db.IsCustomAdminMember(db.GetWindowsUser(), sqlConn)))
                    {
                        btnReactiver.Enabled = true;
                    }
                    else
                    {
                        btnReactiver.Enabled = false;
                    }
                    //
                    if (sPersonneID != String.Empty)
                    {
                        // Build Strings Info
                        BuildAllStrings();
                    }
                    else
                    {
                        // ToDo: Message + Redirect
                        Response.Redirect("ListeEtudiants.aspx");
                    }
                }
            }
            catch (Exception Excep)
            {
                Debug.WriteLine(Excep.Message);
            }
        }
    }
    protected void BuildAllStrings()
    {
        BuildStringInfoGenerale();
        BuildStringCoursPris();
        BuildStringCoursEligibles();
        BuildStringExamenEntrees();
    }
    protected void BuildStringInfoGenerale()
    {
        String returnedString = String.Empty;
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            // Loop through all records
            try
            {
                sqlConn.Open();
                string sSql = String.Format("SELECT Nom, Prenom, DDN, NIF, Telephone1, email, Actif FROM Personnes WHERE PersonneID = '{0}'", txtPersonneID.Text.ToString());
                SqlDataReader dt = db.GetDataReader(sSql, sqlConn);
                if (dt != null)
                {
                    if (dt.Read())
                    {
                        txtNomComplet_Header.Text = dt["Nom"].ToString() + ", " + dt["Prenom"].ToString() + "  (Phone: " + dt["Telephone1"].ToString() + "  Email: "
                            + dt["email"].ToString() + ")";             // Header
                        int iActif = int.Parse(dt["Actif"].ToString());
                        if (iActif == 0)
                        {
                            txtNomComplet_Header.Text += " ---- (INACTIF)";
                            btnReactiver.Visible = true;
                            // Ajouter Oui/Non confirmation avant de Réactiver: JavaScript
                            btnReactiver.Attributes.Add("onClick", "javascript:return confirm('Voulez-vous Vraiment Réactiver Cet Etudiant ?');");
                            //
                        }
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
    protected void BuildStringCoursPris()
    {
        String returnedString = String.Empty;

        lblCoursPrisHeader.Text = "Cours Déjà Suivis";  // Put in Web.Config
        DB_Access db = new DB_Access();
        int DaysBeforeWithdrawal = 0;
        int Interval = 0;
        bool bEnlever = false;

        using (SqlConnection sqlConn0 = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            sqlConn0.Open();
            DaysBeforeWithdrawal = int.Parse(ConfigurationManager.AppSettings["EnleverCoursLimiteJours"].ToString());
            Interval = (int)db.GetStartDateOfCurrentSession(sqlConn0).Subtract(DateTime.Today).TotalDays;
            Interval = Math.Abs(Interval);
            bEnlever = Interval <= DaysBeforeWithdrawal ? true : false;
        }
        
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            
            // Loop through all records
            try
            {
                sqlConn.Open();
                string sSql = String.Format("SELECT CP.CoursPrisID, CP.CoursOffertID, CP.NumeroCours, CP.NoteSurCent, IsNull(CP.DateReussite, '1900-01-01') AS DateReussite, " +
                    " C.NomCours FROM CoursPris CP, Cours C " +
                    " WHERE CP.NumeroCours = C.NumeroCours AND CP.PersonneID = '{0}' AND C. ExamenEntree = 0", txtPersonneID.Text.ToString());

                SqlDataReader dt = db.GetDataReader(sSql, sqlConn);
                String sNoteSurCent = "";

                if (dt != null)
                {
                    returnedString += String.Format("<table class='table table-striped table-bordered table-hover'>");
                    returnedString += @"<thead><tr><th>Titre</th><th>Date Réussite</th><th>Note Obtenue</th></thead><tbody>";
                    while (dt.Read())
                    {
                        String sDate = dt["DateReussite"].ToString().Trim();
                        if (sDate == "1/1/1900 12:00:00 AM")
                            sDate = "En Cours";
                        else
                        {
                            if (sDate.Contains(" "))
                                sDate = sDate.Substring(0, sDate.IndexOf(" "));
                        }
                        double dNoteSurCent = double.Parse(dt["NoteSurCent"].ToString());
                        using (SqlConnection sqlConn2 = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
                        {
                            sqlConn2.Open();
                            if (dNoteSurCent == 0 && db.CoursAppartientASessionCourante(dt["CoursOffertID"].ToString(), sqlConn2))
                            {
                                if (!bEnlever)
                                {
                                    sNoteSurCent = "N/A";
                                }
                                else
                                {
                                    sNoteSurCent = String.Format("<a href='InfoEtudiantRemoveClass.aspx?personneid={0}&CoursPrisID={1}&NumeroCours={2}&CoursOffertID={3}'>Enlever</a>",
                                        txtPersonneID.Text.ToString(), dt["CoursPrisID"].ToString(), dt["NumeroCours"].ToString(), dt["CoursOffertID"].ToString());
                                }
                            }
                            else if (dNoteSurCent == 0)
                            {
                                sNoteSurCent = dt["NoteSurCent"].ToString();
                                sDate = "N/A";
                            }
                            else
                            {
                                sNoteSurCent = dt["NoteSurCent"].ToString();
                            }
                        }
                        returnedString += String.Format(@"<tr class='odd gradeX'>");        //1 Row for each Course
                        returnedString += String.Format(@"<td>{0}</td><td class='right'>{1}</td><td class='right'>{2}</td>",
                            dt["NomCours"].ToString() + " - " + dt["NumeroCours"].ToString(),
                            sDate, sNoteSurCent);
                        returnedString += String.Format(@"</tr>");
                    }
                    returnedString += @"</table>";
                    LitCoursPris.Text = returnedString;
                }                
            }
            catch (Exception ex)
            {
                LitCoursPris.Text = ex.Message;
                db = null;
            }
        }
        db = null;
    }
    protected void BuildStringExamenEntrees()
    {
        String returnedString = String.Empty;

        lblCoursPrisHeader.Text = "Cours Déjà Suivis";  // Put in Web.Config
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            try
            {
                sqlConn.Open();
                sPersonneID = txtPersonneID.Text.ToString();
                string sSql = String.Format("SELECT CP.CoursPrisID, CP.CoursOffertID, CP.NumeroCours, CP.NoteSurCent, CP.NotePassage, IsNull(CP.DateReussite, '1900-01-01') AS DateReussite, " +
                    " C.NomCours FROM CoursPris CP, Cours C " +
                    " WHERE CP.NumeroCours = C.NumeroCours AND CP.PersonneID = '{0}' AND C.ExamenEntree = 1", sPersonneID);

                SqlDataReader dt = db.GetDataReader(sSql, sqlConn);
                String sNoteSurCent = "";

                if (dt != null)
                {
                    returnedString += String.Format("<table class='table table-striped table-bordered table-hover'>");
                    returnedString += @"<thead><tr><th>Titre</th><th>Date Réussite</th><th>Note Obtenue</th></thead><tbody>";
                    while (dt.Read())
                    {
                        String sDate = dt["DateReussite"].ToString().Trim();
                        if (sDate == "1/1/1900 12:00:00 AM")
                            sDate = "En Cours";
                        else
                        {
                            if (sDate.Contains(" "))
                                sDate = sDate.Substring(0, sDate.IndexOf(" "));
                        }
                        double dNoteSurCent = double.Parse(dt["NoteSurCent"].ToString());
                        if (dNoteSurCent == 0 && db.CoursAppartientASessionCourante(dt["CoursOffertID"].ToString(), sqlConn))
                        {
                            //sNoteSurCent = String.Format("<a href='InfoEtudiantRemoveClass.aspx?personneid={0}&CoursPrisID={1}&NumeroCours={2}&CoursOffertID={3}'>Enlever</a>", sPersonneID, dt["CoursPrisID"].ToString(), dt["NumeroCours"].ToString(), dt["CoursOffertID"].ToString());
                        }
                        else if (dNoteSurCent == 0)
                        {
                            sNoteSurCent = dt["NoteSurCent"].ToString();
                            sDate = "N/A";
                        }
                        else
                        {
                            sNoteSurCent = dt["NoteSurCent"].ToString() + " (Minimum: " + dt["NotePassage"].ToString() + ")";
                        }
                        returnedString += String.Format(@"<tr class='odd gradeX'>");        //1 Row for each Course
                        returnedString += String.Format(@"<td>{0}</td><td class='right'>{1}</td><td class='right'>{2}</td>",
                            dt["NomCours"].ToString(), sDate, sNoteSurCent);
                        returnedString += String.Format(@"</tr>");
                    }
                    returnedString += @"</table>";
                    lblExamenEntreesHeader.Text = "Résultats Examens d'Entrée";
                    LitExamenEntrees.Text = returnedString;
                }
                db = null;
            }
            catch (Exception ex)
            {
                LitExamenEntrees.Text = ex.Message;
                db = null;
            }
        }
    }
    void BuildStringCoursEligibles()
    {
        String returnedString = String.Empty;
        lblCoursEligiblesHeader.Text = "Eligibilité ...";
        bool bEnlever = false;
        int DaysBeforeWithdrawal = 0;
        int Interval = 0;

        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn0 = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            sqlConn0.Open();
            DaysBeforeWithdrawal = int.Parse(ConfigurationManager.AppSettings["EnleverCoursLimiteJours"].ToString());
            Interval = (int)db.GetStartDateOfCurrentSession(sqlConn0).Subtract(DateTime.Today).TotalDays;
            Interval = Math.Abs(Interval);
            bEnlever = Interval <= DaysBeforeWithdrawal ? true : false;
        }
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            try
            {
                TimeSpan diffStartAndToday;
                TimeSpan diffEndAndToday;
                int diffStart;
                int diffEnd;

                using (SqlConnection sqlConn2 = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
                {
                    try
                    {
                        sqlConn2.Open();
                        // Will enable ADD classes if session will start within 2 weeks or started less than 1 week
                        diffStartAndToday = db.GetStartDateOfCurrentSession(sqlConn2) - DateTime.Today;
                        diffEndAndToday = db.GetEndDateOfCurrentSession(sqlConn2) - DateTime.Today;
                        diffStart = Math.Abs((int)diffStartAndToday.TotalDays);
                        diffEnd = Math.Abs((int)diffEndAndToday.TotalDays);
                    }
                    catch
                    {
                        LitCoursEligibles.Text = "ERREUR: BuildStringCoursEligibles - 1";
                    }
                }

                sqlConn.Open();
                sPersonneID = txtPersonneID.Text.ToString();
                string sSqlActif = String.Format("SELECT Actif FROM Personnes WHERE PersonneID = '{0}'", txtPersonneID.Text.ToString());

                SqlDataReader dtActif = db.GetDataReader(sSqlActif, sqlConn);
                if (dtActif.Read())
                {
                    int iActif = int.Parse(dtActif["Actif"].ToString());
                    if (iActif == 1)
                    {
                        //??????????????????? NEED CoursOfferts WHERE Actif = 1
                        //string sSql = String.Format("SELECT NumeroCours, NomCours FROM Cours WHERE NumeroCours <> 'NOP' AND Actif = 1 AND ExamenEntree = 0 AND "+ 
                        //    " (CoursPreRequis = 'NOP' OR CoursPreRequis in (SELECT NumeroCours FROM CoursPris " + 
                        //    " WHERE PersonneID = '{0}' AND (NoteSurCent >= NotePassage OR Waiver = 1)) )", txtPersonneID.Text.ToString());

                        String sSql = String.Format("SELECT C.NumeroCours, C.NomCours, ' (' + H.Jours + ' ' + H.HeureDebut + ' - ' + H.HeureFin + ')' AS Horaire,  " +
                            " CO.CoursOffertID FROM Cours C, CoursOfferts CO, Horaires H WHERE C.NumeroCours = CO.NumeroCours AND CO.HoraireID = H.HoraireID " +
                            " AND C.NumeroCours <> 'NOP' AND C.Actif = 1 AND CO.Actif = 1 AND ExamenEntree = 0 AND " +
                            " (CoursPreRequis = 'NOP' OR CoursPreRequis in (SELECT NumeroCours FROM CoursPris  " +
                            " WHERE PersonneID = '{0}' AND (NoteSurCent >= NotePassage OR Waiver = 1)) ) ORDER BY C.NomCours", sPersonneID);
                        using (SqlConnection sqlConn3 = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
                        {
                            bool dejaInscrit = false, dejaReussi = false;
                            try
                            {
                                sqlConn3.Open();
                                SqlDataReader dt = db.GetDataReader(sSql, sqlConn3);
                                if (dt != null)
                                {
                                    //returnedString += @"<table width='100%' class='table table-striped table-bordered table-hover' id='dataTables-Etudiants'>";
                                    returnedString += String.Format("<table class='table table-striped table-bordered table-hover'>");
                                    returnedString += @"<thead><tr><th>Numéro</th><th>Titre</th></thead><tbody>";
                                    while (dt.Read())
                                    {
                                        using (SqlConnection sqlConn4 = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
                                        {
                                            sqlConn4.Open();
                                            dejaInscrit = db.EtudiantDejaInscrit(sPersonneID, dt["NumeroCours"].ToString(), sqlConn4);
                                            dejaReussi = db.EtudiantDejaReussi(sPersonneID, dt["NumeroCours"].ToString(), sqlConn4);
                                        }
                                        if (!dejaInscrit && !dejaReussi)
                                        {
                                            returnedString += String.Format(@"<tr class='odd gradeX'>");        //1 Row for each Course
                                            returnedString += String.Format(@"<td>{0}</td><td>{1}</td>", dt["NumeroCours"].ToString(), dt["NomCours"].ToString() + dt["Horaire"].ToString());
                                            
                                            if (bEnlever)
                                            {
                                                returnedString += String.Format(@"<td class='center'><a href='InfoEtudiantAddClass.aspx?personneid={0}&CoursOffert={1}&NumeroCours={2}'><image src='images/blue-plus.png'  alt='Choisir' style='width:25px;height:25px;'></a></td></tr>", sPersonneID, dt["CoursOffertID"].ToString(), dt["NumeroCours"].ToString());
                                            }
                                            else
                                            {
                                                returnedString += String.Format(@"<td class='center'></td></tr>");

                                            }
                                            returnedString += String.Format(@"</tr>");
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                LitCoursEligibles.Text = "ERREUR: BuildStringCoursEligibles - 3";
                                return;
                            }
                        }
                        returnedString += @"</table>";
                        LitCoursEligibles.Text = returnedString;

                        db = null;
                    }
                }
            }
            catch (Exception ex)
            {
                LitCoursEligibles.Text = "ERREUR: BuildStringCoursEligibles - 2";
                db = null;
                Debug.WriteLine(ex.Message);
            }
        }
    }    
    protected void btnResumeFinance_Click(object sender, EventArgs e)
    {
        Response.Redirect("ResumeFinanceEtudiant.aspx?PersonneID=" + txtPersonneID.Text);
    }
    protected void btnReleveNotes_Click(object sender, EventArgs e)
    {
        Response.Redirect("ReleveNotesEtudiant.aspx?PersonneID=" + txtPersonneID.Text);
    }
    protected void btnProgresGraduation_Click(object sender, EventArgs e)
    {
        Response.Redirect("ProgresGraduationEtudiant.aspx?PersonneID=" + txtPersonneID.Text);
    }
    protected void btnPayments_Click(object sender, EventArgs e)
    {
        Response.Redirect("RecevoirPaiements.aspx?PersonneID=" + txtPersonneID.Text);
    }
    protected void btnEditNotePasseeEtudiant_Click(object sender, EventArgs e)
    {
        Response.Redirect("EditNotePasseeEtudiant.aspx?PersonneID=" + txtPersonneID.Text);        
    }
    protected void btnFactureManuelle_Click(object sender, EventArgs e)
    {
        Response.Redirect("FactureManuelle.aspx?PersonneID=" + txtPersonneID.Text);
    }
    protected void btnReactiver_Click(object sender, EventArgs e)
    {
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            try
            {
                sqlConn.Open();
                string sSql = "UPDATE Personnes SET Actif = 1 WHERE PersonneID = @PersonneID";
                SqlParameter paramPersonneID = new SqlParameter("@PersonneID", SqlDbType.NVarChar);
                paramPersonneID.Value = txtPersonneID.Text.ToString();

                SqlParameter paramUserName = new SqlParameter("@CreeParUserName", SqlDbType.Text);
                paramUserName.Value = db.GetWindowsUser();

                if (db.IssueCommandWithParams(sSql, sqlConn, paramPersonneID))
                {
                    sSql = "INSERT Reactives (PersonneID, CreeParUserName) VALUES (@PersonneID, @CreeParUserName)";
                    db.IssueCommandWithParams(sSql, sqlConn, paramPersonneID, paramUserName);
                    btnReactiver.Visible = false;
                    //Facturer comme Inscription
                    String sObligation = ConfigurationManager.AppSettings["CodeFraisInscription"].ToString();
                    db.FacturerPourObligation(sObligation, txtPersonneID.Text.ToString(),sqlConn);

                    BuildStringInfoGenerale();
                }

                db = null;
            }
            catch (Exception ex)
            {
                LitCoursPris.Text = ex.Message;
                db = null;
            }
        }
    }
    protected void btnEditer_Click(object sender, EventArgs e)
    {
        Response.Redirect("Inscription.aspx?EditFlag=1&PersonneID=" + txtPersonneID.Text);
    }

    protected void btnHoraireSessionCourante_Click(object sender, EventArgs e)
    {
        Response.Redirect("HoraireSessionCourante.aspx?PersonneID=" + txtPersonneID.Text);
    }
}
using AjaxControlToolkit;
using AjaxControlToolkit.HTMLEditor.ToolbarButton;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web.UI.WebControls;
using WebGrease.Activities;

public partial class MettreGroupeDansClasse : System.Web.UI.Page
{
    DB_Access db = new DB_Access();
    string ConnectionString = XCryptEngine.ConnectionStringEncryption.Decrypt(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
    protected void Page_Load(object sender, EventArgs e)
    {
        //DB_Access db = new DB_Access();
        //using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
        //{
        //    sqlConn.Open();
        //    String sql = String.Format("Select Montant,MD.SessionID from MontantsDus as MD, LesSessions as LS where MD.SessionID = LS.SessionID and LS.SessionCourante = 1 and MD.PersonneID = '{0}'", "F0E3D052-060C-4DA0-A2BD-72849772653D");
        //    SqlDataReader dtTemp = db.GetDataReader(sql, sqlConn);
        //    Double Montant = new Double();
        //    Double SessionIDCourante = new Double();
        //    string convertmontant = "";
        //    while (dtTemp.Read())
        //    {
        //        convertmontant = dtTemp["Montant"].ToString();
        //        Montant = Convert.ToDouble(convertmontant);
        //        SessionIDCourante = Convert.ToDouble(dtTemp["SessionID"].ToString());
        //    }
        //    Response.Write(Montant + "ID: " + SessionIDCourante);

        //}

        if (!IsPostBack)
        {
            //string script = "$(document).ready(function () { $('[id*=btnSauvegarder]').click(); });";
            //ClientScript.RegisterStartupScript(this.GetType(), "load", script, true);

            String sNumeroCours = string.Empty;
            String sNomCours = string.Empty;

            try
            {
                //sNumeroCours = Request.Cookies.Get("numero_cours").Value.ToString();
                //sNomCours = Request.Cookies.Get("nom_cours").Value.ToString();
                if (Request.QueryString["nuc"] != null)
                    sNumeroCours = Request.QueryString["nuc"];
                if (Request.QueryString["noc"] != null)
                    sNomCours = Request.QueryString["noc"];
            }
            catch (Exception Excep)
            {
                Debug.WriteLine(Excep.Message);
            }

            if (sNumeroCours != String.Empty)
            {
                litTitle2.Visible = false;
                RemplirListeEtudiantsEligibres(sNumeroCours, sNomCours);
            }
            else
            {
                litTitle2.Visible = true;
                RemplirListeDeCours();
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
            String sSql = @"SELECT C.NomCours + ' (' + H.Jours + ' ' + H.HeureDebut + ' - ' + H.HeureFin + ')' AS NomCours, CO.NumeroCours, CO.CoursOffertID, CO.NotePassage " +
                " FROM Cours C, CoursOfferts CO, Horaires H  " +
                " WHERE C.NumeroCours = CO.NumeroCours AND CO.HoraireID = H.HoraireID AND CO.Actif = 1 " +
                " AND C.Actif = 1 AND ExamenEntree = 0 AND CO.NumeroCours <> 'NOP' ORDER BY CO.NumeroCours";

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
            lblError.Text = "ERREUR: " + ex.Message;
        }
    }

    /// <summary>
    /// Les étudiants qui sont éligibres pour prendre un cours sont:
    /// 1. _ Ceux qui ont passé le pré-requis;
    /// 2. _ Ceux qui n'ont pas réussi le cours dans le passé, donc ils ont une note supérieur 0 mais inférieur à la note de passage
    ///    
    ///     * IsNull(DateReussite, '') <> '' est le critère pour éliminer les nouveaux inscrits au Cours
    ///     * 
    ///     * Etudians qui ont passé le pré-requis 
    ///     * UNION
    ///     * Etudiants qui n'ont pas passé le cours
    /// </summary>
    /// <param name="NumeroCours"></param>
    void RemplirListeEtudiantsEligibres(String NumeroCours, String NomCours)
    {
        lblError.Text = String.Empty;
        DateTime convertDate = new DateTime();
        lblNomCours.Text = "Choisissez les Etudiants et Cliquez Sauvegarder: " + NomCours;
        using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
        {
            try
            {
                Hashtable NanKlassLaDeja = null;
                using (SqlConnection sqlConn1 = new SqlConnection(ConnectionString))
                {
                    sqlConn1.Open();
                    NanKlassLaDeja = db.GetEtudiantsAlreadyIn(NumeroCours, sqlConn1);
                }
                // SELECT tous les étudiants qui ont pris les prérequis pour ce cours ou qui ont une note inférieur à la note de passage pour ce cours
                String sSql = String.Format(@"SELECT DISTINCT P.PersonneID, P.Nom, P.Prenom, Telephone1, P.NIF, P.DDN FROM Personnes P, CoursPris CP WHERE P.PersonneID = CP.PersonneID " +
                    " AND (NoteSurCent >= NotePassage OR Waiver = 1) AND CP.NumeroCours IN (SELECT CoursPreRequis FROM Cours WHERE NumeroCours = '{0}') " +
                    " UNION SELECT DISTINCT P.PersonneID, P.Nom, P.Prenom, Telephone1, P.NIF, P.DDN FROM Personnes P, CoursPris CP WHERE P.Actif = 1 AND P.PersonneID = CP.PersonneID AND IsNull(CP.DateReussite, '') <> '' " +
                    " AND NoteSurCent < NotePassage AND CP.NumeroCours = '{0}' ORDER BY P.Nom, P.Prenom", NumeroCours);

                List<EtudiantInfo> lstSourceEntrees = new List<EtudiantInfo>();
                sqlConn.Open();
                SqlDataReader dtTemp = db.GetDataReader(sSql, sqlConn);

                while (dtTemp.Read())
                {
                    if (!db.EtudiantDejaInscritOuReussi(dtTemp["PersonneID"].ToString(), NumeroCours, sqlConn))
                    {
                        // Extra check afin d'écarter les étudiants déjà inscrit dans ce cours cette Session
                        if (!NanKlassLaDeja.ContainsKey(dtTemp["PersonneID"].ToString()))
                        {
                            EtudiantInfo unEtudiant = new EtudiantInfo();

                            unEtudiant.PersonneID = dtTemp["PersonneID"].ToString();
                            unEtudiant.Nom = dtTemp["Nom"].ToString();
                            unEtudiant.Prenom = dtTemp["Prenom"].ToString();
                            unEtudiant.Telephone1 = dtTemp["Telephone1"].ToString();
                            unEtudiant.NIF = dtTemp["NIF"].ToString();
                            if (dtTemp["DDN"].ToString().Trim() == String.Empty)
                            {
                                unEtudiant.DDN = String.Empty;
                            }
                            else
                            {
                                convertDate = DateTime.Parse(dtTemp["DDN"].ToString());
                                unEtudiant.DDN = convertDate.ToString("dd-MMMM-yyyy");
                            }
                            lstSourceEntrees.Add(unEtudiant);
                        }
                    }
                }

                if (lstSourceEntrees != null)
                {
                    gvEtudiants.DataSource = lstSourceEntrees;
                    gvEtudiants.DataKeyNames.SetValue("PersonneID", 0);
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

                }
                db = null;

            }
            catch (Exception ex)
            {
                lblError.Text = "ERREUR: " + ex.Message;
            }
        }
        db = null;
    }

    protected void gvCoursDisponibles_SelectedIndexChanged(object sender, EventArgs e)
    {
        //lblError.Text = "";
        TimeSpan diffStartAndToday;
        TimeSpan diffEndAndToday;
        int diffStart = 0;
        int diffEnd;
        int DaysBeforeWithdrawal = 0;
        using (SqlConnection sqlConn2 = new SqlConnection(ConnectionString))
        {
            try
            {
                sqlConn2.Open();
                // Will enable ADD classes if session will start within 2 weeks or started less than 1 week
                DaysBeforeWithdrawal = int.Parse(ConfigurationManager.AppSettings["EnleverCoursLimiteJours"].ToString());
                diffStartAndToday = db.GetStartDateOfCurrentSession(sqlConn2) - DateTime.Today;
                diffEndAndToday = db.GetEndDateOfCurrentSession(sqlConn2) - DateTime.Today;
                diffStart = Math.Abs((int)diffStartAndToday.TotalDays);
                diffEnd = Math.Abs((int)diffEndAndToday.TotalDays);
            }
            catch
            {
                //LitCoursEligibles.Text = "ERREUR: BuildStringCoursEligibles - 1";
            }
        }
        //
        if (diffStart <= DaysBeforeWithdrawal)
        {
            String sNumeroCours = ((Label)gvCoursDisponibles.Rows[gvCoursDisponibles.SelectedIndex].FindControl("lblNumeroCours")).Text;
            String sNomCours = ((Label)gvCoursDisponibles.Rows[gvCoursDisponibles.SelectedIndex].FindControl("lblNomCours")).Text;
            //CheckBox chkCoursSelectionne = (CheckBox)gvCoursDisponibles.Rows[gvCoursDisponibles.SelectedIndex].FindControl("chkCours");

            // Deselektyone tout Cours ki te selektyone
            for (int iRow = 0; iRow < gvCoursDisponibles.Rows.Count; iRow++)
            {
                if (iRow != gvCoursDisponibles.SelectedIndex)
                    ((CheckBox)gvCoursDisponibles.Rows[iRow].FindControl("chkCours")).Checked = false;
            }

            Response.Redirect(String.Format("MettreGroupeDansClasse.aspx?nuc={0}&noc={1}", sNumeroCours, sNomCours));
        }
        else
        {
            lblError.Text = "C\'est trop tard, vous ne pouvez pas ajouter nouveau étudiant dans les classes";
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("MettreGroupeDansClasse.aspx");   // Just reload Page
        lblError.Text = "";
    }

    protected void btnSauvegarder_Click(object sender, EventArgs e)
    {
        btnSauvegarder.Enabled = false;

        int MoisParSession = int.Parse(ConfigurationManager.AppSettings["NombreDeMoisParSession"].ToString());

        int iSaved = 0, iNotSaved = 0;
        String sPersonneID;

        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
        {
            try
            {
                sqlConn.Open();

                String sNumeroCours = String.Empty;
                String sNomCours = String.Empty;
                int iCoursOffertID = 0;
                double dNotePassage = 0;

                if (Request.QueryString["nuc"] != null)

                    sNumeroCours = Request.QueryString["nuc"];
                if (Request.QueryString["noc"] != null)
                    sNomCours = Request.QueryString["noc"];
                iCoursOffertID = db.GetCoursOffertID(sNumeroCours, sqlConn);
                dNotePassage = db.GetNoteDePassage(iCoursOffertID, sqlConn);

                if (sNumeroCours == String.Empty || sNomCours == String.Empty || iCoursOffertID <= 0 || dNotePassage <= 0)
                {
                    lblError.Text = "ERREUR Technique: Contactez un technicien!";
                    lblErrorHead.Text = "ERREUR Technique: Contactez un technicien!";
                    return;
                }

                for (int iRow = 0; iRow < gvEtudiants.Rows.Count; iRow++)
                {
                    if (gvEtudiants.Rows[iRow].Enabled)
                    {
                        if (((CheckBox)gvEtudiants.Rows[iRow].FindControl("chkEtudiant")).Checked == true)
                        {
                            // Insérer dans table CoursPris qui est aussi CoursPris inscrits
                            sPersonneID = ((Label)gvEtudiants.Rows[iRow].FindControl("lblPersonneID")).Text;
                            try
                            {
                                ((CheckBox)gvEtudiants.Rows[iRow].FindControl("chkEtudiant")).Checked = false;
                                String sSql = "INSERT INTO CoursPris (PersonneID, NumeroCours, CoursOffertID, NotePassage, CreeParUserName) VALUES (@PersonneID, @NumeroCours, @CoursOffertID, @NotePassage, @CreeParUserName)";

                                SqlParameter ParamPersonneID = new SqlParameter("@PersonneID", SqlDbType.Text);
                                ParamPersonneID.Value = sPersonneID;

                                SqlParameter ParamNumeroCours = new SqlParameter("@NumeroCours", SqlDbType.Text);
                                ParamNumeroCours.Value = sNumeroCours;

                                SqlParameter ParamCoursOffertID = new SqlParameter("@CoursOffertID", SqlDbType.Int);
                                ParamCoursOffertID.Value = iCoursOffertID;

                                SqlParameter ParamNotePassage = new SqlParameter("@NotePassage", SqlDbType.Float);
                                ParamNotePassage.Value = dNotePassage;

                                SqlParameter paramUserName = new SqlParameter("@CreeParUserName", SqlDbType.Text);
                                paramUserName.Value = db.GetWindowsUser();

                                if (db.IssueCommandWithParams(sSql, sqlConn, ParamPersonneID, ParamNumeroCours, ParamCoursOffertID, ParamNotePassage, paramUserName))
                                {
                                    string SessionIDCourantes = string.Empty;
                                    //db.Facturer(sPersonneID, sNumeroCours, MoisParSession, sqlConn);
                                    iSaved++;
                                    //Vérifier si on a déjà facturé la personne pour la session courante
                                    double FS = Double.Parse(ConfigurationManager.AppSettings["FraisParSession"].ToString());
                                    int FC = Int32.Parse(ConfigurationManager.AppSettings["FraisParCours"].ToString());
                                    int isTrueStr = Int32.Parse(ConfigurationManager.AppSettings["isTrue"].ToString());

                                    /*
                                     * Cette commande retourne 1 si l'etudiant est déjà facturé pour la session courante et 0
                                       si l'etudiant n'a pas encore facturé.
                                    */

                                    String sql = String.Format("Select COUNT(Montant) from MontantsDus where PersonneID ='{0}' and SessionID = (Select SessionID from LesSessions where SessionCourante = 1) and Montant = {1}", sPersonneID, FS);
                                    SqlCommand cmd = new SqlCommand(sql, sqlConn);
                                    Int32 count = (Int32)cmd.ExecuteScalar();

                                    if (count == 0 && iSaved != 0)
                                    {
                                        db.IssueCommand(String.Format("INSERT INTO MontantsDus (PersonneID, CodeObligation,Montant,SessionID) VALUES ('{0}','{1}','{2}',(Select SessionID from LesSessions where SessionCourante = 1))",
                                        sPersonneID, "FS", FS), sqlConn);

                                    }
                                }
                                else
                                {
                                    iNotSaved++;
                                }
                                gvEtudiants.Rows[iRow].Enabled = false;
                            }
                            catch (Exception ex)
                            {
                                lblError.Text = "ERREUR: " + ex.Message;
                            }
                        }
                    }
                }
            }
            catch
            {
                lblError.Text = "ERREUR ...";
            }
        }
        db = null;
        btnSauvegarder.Enabled = true;
        Response.Redirect("MettreGroupeDansClasse.aspx");   // Just reload Page
    }

}
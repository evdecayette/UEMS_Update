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

public partial class AdminPages_AddEditCours : System.Web.UI.Page
{
    protected static bool inProcess = false;
    protected static String errorMessage = String.Empty;
    protected static bool processSuccess = false;
    protected static bool updateCours = false;

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
                    if (db.isCurrentWindowsInThisGroup(sGroup) || db.IsCustomAdminMember(db.GetWindowsUser(), sqlConn))
                    {
                        RemplirListeDeCours();
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
                    lblError.Text = "ERREUR : Base de Données Pas Accessible!!! ";                    
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
            SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
            String sSql = @"SELECT 'AAA000' AS NomCours, 'AAA001' AS NumeroCours,'Choisissez un Pré-Réquis si Nécessaire !' AS LesDeux " 
                + " UNION SELECT NomCours, NumeroCours, NomCours + ' (' + NumeroCours + ')' AS LesDeux FROM Cours " +
                " WHERE Actif = 1 AND NumeroCours IN (SELECT CO.NumeroCours FROM CoursOfferts CO, LesSessions LS WHERE CO.SessionID = LS.SessionID AND LS.SessionCourante = 1) ORDER BY NumeroCours";

            SqlDataAdapter da = new SqlDataAdapter(sSql, myConnection);
            DataTable dTable = new DataTable();
            da.Fill(dTable);

            ddlCoursPreRequis.DataSource = dTable;
            ddlCoursPreRequis.DataValueField = "NumeroCours";
            ddlCoursPreRequis.DataTextField = "LesDeux";
            ddlCoursPreRequis.DataBind();
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
        lblError.Text = String.Empty;
        if (btnSauvegarder.Text == "Mettre A Jour")
        {
            updateCours = true;
        }
        else
        {
            updateCours = false;
        }

        btnSauvegarder.Text = "";
        btnSauvegarder.CssClass = "button-wait";

        lblError.ForeColor = System.Drawing.Color.Red;
        Timer1.Enabled = true;
        Thread workerThread = new Thread(new ThreadStart(ProcessRecords));
        workerThread.Start();   
    }

    protected void ProcessRecords()
    {
        // Début traitement d'information
        inProcess = true;

        String erreurString = String.Empty;
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {        
            try
            {
                sqlConn.Open();
                // Nettoyage de données
                txtNumeroCours.Text = txtNumeroCours.Text.Trim().ToUpper();
                if (txtNumeroCours.Text == String.Empty)
                {
                    errorMessage = "Numéro de Cours est obligatoire !";
                    inProcess = false;
                    return;
                }
                else if (txtNumeroCours.Text.Length != 6)
                {
                    errorMessage = "Format du Numéro de Cours: AAA000";
                    inProcess = false;
                    return;
                }
                else if (!allLetters(txtNumeroCours.Text.Substring(0, 3)))
                {
                    errorMessage = "Format du Numéro de Cours: AAA000 - Partie Alphabétique Manquant";
                    inProcess = false;
                    return;
                }
                else if (!allNumbers(txtNumeroCours.Text.Substring(3, 3)))
                {
                    errorMessage = "Format du Numéro de Cours: AAA000 - Partie Numérique Manquant";
                    inProcess = false;
                    return;
                }

                txtNomCours.Text = txtNomCours.Text.Trim();
                if (txtNumeroCours.Text == String.Empty)
                {
                    errorMessage = "Nom du Cours est obligatoire !";
                    inProcess = false;
                    return;
                }

                // Insérer dans table CoursOfferts pour la Session Courante                    
                String sSql = "INSERT INTO Cours (NumeroCours, NomCours, DescriptionCours, CoursPreRequis, ExamenEntree, Credits, CreeParUserName) " +
                    " VALUES (@NumeroCours, @NomCours, @DescriptionCours, @CoursPreRequis, @ExamenEntree, @Credits, @CreeParUserName)";
                SqlParameter ParamNumeroCours = new SqlParameter("@NumeroCours", SqlDbType.Text);
                ParamNumeroCours.Value = txtNumeroCours.Text;

                if (updateCours)
                {
                    sSql = "UPDATE Cours SET NomCours = @NomCours, DescriptionCours = @DescriptionCours, " +
                        " CoursPreRequis = @CoursPreRequis, ExamenEntree = @ExamenEntree, Credits = @Credits, = @CreeParUserName " +
                        " WHERE NumeroCours = @NumeroCours";
                    ParamNumeroCours.Value = ddlCoursPreRequis.SelectedValue.ToString();
                }

                SqlParameter ParamNomCours = new SqlParameter("@NomCours", SqlDbType.Text);
                ParamNomCours.Value = txtNomCours.Text;

                SqlParameter ParamDescriptionCours = new SqlParameter("@DescriptionCours", SqlDbType.Text);
                ParamDescriptionCours.Value = txtDescriptionCours.Text;

                SqlParameter ParamCoursPreRequis = new SqlParameter("@CoursPreRequis", SqlDbType.Text);
                if (ddlCoursPreRequis.SelectedValue.ToString() == "AAA001")
                {
                    ParamCoursPreRequis.Value = ConfigurationManager.AppSettings["NoPreRequis"].ToString();
                }
                else
                {
                    ParamCoursPreRequis.Value = ddlCoursPreRequis.SelectedValue.ToString();
                }

                SqlParameter ParamExamenEntree = new SqlParameter("@ExamenEntree", SqlDbType.Int);
                ParamExamenEntree.Value = chkExamenEntree.Checked ? 1 : 0;

                SqlParameter ParamCredit = new SqlParameter("@Credits", SqlDbType.Int);
                ParamCredit.Value = Int16.Parse(txtCredits.Text);

                SqlParameter paramUserName = new SqlParameter("@CreeParUserName", SqlDbType.Text);
                paramUserName.Value = db.GetWindowsUser();

                if (!db.IssueCommandWithParams(sSql, sqlConn, ParamNumeroCours, ParamNomCours, ParamDescriptionCours, ParamCoursPreRequis, ParamExamenEntree, ParamCredit, paramUserName))
                {
                    erreurString += "Erreur: Création de Cours Offerts!!!";
                }
                else
                {
                    inProcess = false;
                    processSuccess = true;

                }
            }
            catch (Exception ex)
            {
                errorMessage = "ERREUR: " + ex.Message;
                inProcess = false;
            }
        }
        db = null;

        // Fin traitement d'information
        inProcess = false;
    }

    protected bool allLetters(String aString)
    {
        char aChar;
        for (int i = 0; i < aString.Length; i++)
        {
            aChar = Convert.ToChar(aString[i]);
            if (aChar < 65 || aChar > 90)
                return false;
        }
        return true;
    }

    protected bool allNumbers(String aString)
    {
        char aChar;
        for (int i = 0; i < aString.Length; i++)
        {
            aChar = Convert.ToChar(aString[i]);
            if (aChar < 48 || aChar > 57)
                return false;
        }
        return true;
    }

    protected void Timer1_Tick(object sender, EventArgs e)
    {
        if (!inProcess) // Fin Process Records
        {
            Timer1.Enabled = false;

            btnSauvegarder.CssClass = "btn btn-outline btn-primary";
            btnSauvegarder.Text = "Sauvegarder";
            updateCours = false;

            lblError.ForeColor = System.Drawing.Color.Blue;
            errorMessage = "Succès ....";
            CleanUp();
            RemplirListeDeCours();
        }
        lblError.Text = errorMessage;
    }

    protected void CleanUp()
    {
        txtNumeroCours.Text = String.Empty;
        txtNomCours.Text = String.Empty;
        txtDescriptionCours.Text = String.Empty;
        ddlCoursPreRequis.SelectedIndex = -1;
        chkExamenEntree.Checked = false;
        txtCredits.Text = "3";
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        CleanUp();
        lblError.Text = "";
    }
    protected void ddlCoursPreRequis_SelectedIndexChanged(object sender, EventArgs e)
    {
        lblError.Text = String.Empty;
        //btnSauvegarder.Enabled = false;
        try
        {
            
            String sSql = String.Format(@"SELECT * FROM Cours WHERE Actif = 1 AND NumeroCours = '{0}'", 
                ddlCoursPreRequis.SelectedValue.ToString());
            if (ddlCoursPreRequis.SelectedIndex >= 0)
            {
                DB_Access db = new DB_Access();
                using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
                {
                    try
                    {
                        sqlConn.Open();
                        SqlDataReader dt = db.GetDataReader(sSql, sqlConn);

                        if (dt.Read())
                        {
                            txtNumeroCours.Text = dt["NumeroCours"].ToString(); ;
                            txtNomCours.Text = dt["NomCours"].ToString(); ;
                            txtCredits.Text = dt["Credits"].ToString();
                            txtDescriptionCours.Text = dt["DescriptionCours"].ToString();
                            btnSauvegarder.Text = "Mettre A Jour";
                        }
                    }
                    catch
                    {
                        lblError.Text = "Erreur: Base de Données Inaccessible!";
                    }
                }
            }

        }
        catch (Exception ex)
        {
            lblError.Text = "ERREUR: " + ex.Message;
        }
    }
}
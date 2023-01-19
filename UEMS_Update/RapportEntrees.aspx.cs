using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.InteropServices;
// Impersonation
using System.Security.Principal;
using System.Threading;
using System.Web;

using ClosedXML.Excel;

public partial class RapportEntrees : System.Web.UI.Page
{
#if DEBUG
        String sUserName = "spoteau", sDomain = "spoteau-pc", sPass = "robenson2";
#else
    String sUserName = "spoteau", sDomain = "CCPAP1", sPass = "robenson22";
#endif

    public const int LOGON32_LOGON_INTERACTIVE = 2;
    public const int LOGON32_PROVIDER_DEFAULT = 0;

    WindowsImpersonationContext impersonationContext;

    [DllImport("advapi32.dll")]
    public static extern int LogonUserA(String lpszUserName,
        String lpszDomain,
        String lpszPassword,
        int dwLogonType,
        int dwLogonProvider,
        ref IntPtr phToken);
    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int DuplicateToken(IntPtr hToken,
        int impersonationLevel,
        ref IntPtr hNewToken);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool RevertToSelf();

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern bool CloseHandle(IntPtr handle);
    protected static string Messages = String.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            RemplirSessionsDropDown();
            RemplirUsersDropDown();
            RemplirSessionsDropDownPourExcel();
        }
    }

    private bool ImpersonateValidUser(String userName, String domain, String password)
    {
        WindowsIdentity tempWindowsIdentity;
        IntPtr token = IntPtr.Zero;
        IntPtr tokenDuplicate = IntPtr.Zero;

        if (RevertToSelf())
        {
            if (LogonUserA(userName, domain, password, LOGON32_LOGON_INTERACTIVE,
                LOGON32_PROVIDER_DEFAULT, ref token) != 0)
            {
                if (DuplicateToken(token, 2, ref tokenDuplicate) != 0)
                {
                    tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
                    impersonationContext = tempWindowsIdentity.Impersonate();
                    if (impersonationContext != null)
                    {
                        CloseHandle(token);
                        CloseHandle(tokenDuplicate);
                        return true;
                    }
                }
            }
        }
        if (token != IntPtr.Zero)
            CloseHandle(token);
        if (tokenDuplicate != IntPtr.Zero)
            CloseHandle(tokenDuplicate);
        return false;
    }

    private void undoImpersonation()
    {
        impersonationContext.Undo();
    }

    void RemplirSessionsDropDown()
    {
        lblError.Text = String.Empty;

        try
        {
            SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
            String sSql = @"SELECT 'Choisissez une Session' AS SessionNomComplet, -1 AS SessionID " +
                " UNION SELECT SessionDescription + ' (' + CONVERT(nvarchar, SessionDateDebut) + ' - ' + " +
                " CONVERT(NVARCHAR, SessionDateFin) + ')' AS SessionNomComplet, SessionID FROM LesSessions";

            SqlDataAdapter da = new SqlDataAdapter(sSql, myConnection);
            DataTable dTable = new DataTable();
            da.Fill(dTable);

            ddlSessions.DataSource = dTable;
            ddlSessions.DataValueField = "SessionID";
            ddlSessions.DataTextField = "SessionNomComplet";

            ddlSessions.DataBind();
            myConnection.Close();
            myConnection = null;
        }
        catch (Exception ex)
        {
            lblError.Text = "ERREUR: " + ex.Message;
        }
    }
    /// <summary>
    /// TODO: Combiner avec Methode précédente - ddl comme parametre
    /// </summary>
    void RemplirSessionsDropDownPourExcel()
    {
        lblError.Text = String.Empty;

        try
        {
            SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
            String sSql = @"SELECT 'Choisissez une Session' AS SessionNomComplet, -1 AS SessionID " +
                " UNION SELECT SessionDescription + ' (' + CONVERT(nvarchar, SessionDateDebut) + ' - ' + CONVERT(NVARCHAR, SessionDateFin) + ')' AS SessionNomComplet, SessionID FROM LesSessions";

            SqlDataAdapter da = new SqlDataAdapter(sSql, myConnection);
            DataTable dTable = new DataTable();
            da.Fill(dTable);

            ddlSessionPourExcel.DataSource = dTable;
            ddlSessionPourExcel.DataValueField = "SessionID";
            ddlSessionPourExcel.DataTextField = "SessionNomComplet";

            ddlSessionPourExcel.DataBind();
            myConnection.Close();
            myConnection = null;
        }
        catch (Exception ex)
        {
            lblError.Text = "ERREUR: " + ex.Message;
        }
    }
    void RemplirUsersDropDown()
    {
        lblError.Text = String.Empty;

        try
        {
            SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
            String sSql = @"SELECT 'Choisissez un Utilisateur' as RecuParUserName " +
                " UNION SELECT RecuParUserName FROM MontantsRecus GROUP BY RecuParUserName ORDER BY RecuParUserName";

            SqlDataAdapter da = new SqlDataAdapter(sSql, myConnection);
            DataTable dTable = new DataTable();
            da.Fill(dTable);

            ddlUsers.DataSource = dTable;
            ddlUsers.DataValueField = "RecuParUserName";
            ddlUsers.DataTextField = "RecuParUserName";

            ddlUsers.DataBind();
            myConnection.Close();
            myConnection = null;
        }
        catch (Exception ex)
        {
            lblError.Text = "ERREUR: " + ex.Message;
        }
    }
    void RemplirCoursOffertsDropDown()
    {
        lblError.Text = String.Empty;

        try
        {
            SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
            String sSql = @"SELECT 'Choisissez un Cours' AS NomCours, -1 AS CoursOffertID, 'AAA000' AS NumeroCours " +
                " UNION SELECT CO.NumeroCours + ' - ' + C.NomCours, CoursOffertID AS NomCours, CO.NumeroCours FROM CoursOfferts CO, Cours C  " +
                " WHERE CO.NumeroCours = C.NumeroCours AND CO.Actif = 1 ORDER BY NumeroCours ";

            SqlDataAdapter da = new SqlDataAdapter(sSql, myConnection);
            DataTable dTable = new DataTable();
            da.Fill(dTable);

            ddlUsers.DataSource = dTable;
            ddlUsers.DataValueField = "CoursOffertID";
            ddlUsers.DataTextField = "NomCours";

            ddlUsers.DataBind();
            myConnection.Close();
            myConnection = null;
        }
        catch (Exception ex)
        {
            lblError.Text = "ERREUR: " + ex.Message;
        }
    }
    protected void txtDateFrom_TextChanged(object sender, EventArgs e)
    {
        if (!DateOK(txtDateFrom.Text))
        {
            txtDateFrom.Text = String.Empty;
        }
    }
    bool DatesOK()
    {
        if (DateOK(txtDateFrom.Text) && DateOK(txtDateTo.Text))
        {
            return true;
        }
        else
            return false;
    }
    bool DateOK(String sAChequer)
    {
        lblError.Text = String.Empty;

        String sCheckDate = txtDateFrom.Text.Trim();
        if (sCheckDate == String.Empty)
        {
            lblError.Text = "ERREUR: Les 2 Dates sont Obligatoires !!!";
            return false;
        }

        char[] sAllChar = sCheckDate.ToCharArray();

        foreach (char ch in sAllChar)
        {
            if (!Char.IsNumber(ch) && ch.CompareTo('/') != 0)
            {
                lblError.Text = "Cliquer pour choisir une date à partir du CALENDRIER";
                txtDateFrom.Text = string.Empty;
                return false;
            }
        }

        DateTime datechoisie = DateTime.Parse(sCheckDate);
        String dt = datechoisie.Year.ToString();
        if (dt == "1")
        {
            lblError.Text = "Date : Obligatoire";
            return false;
        }
        if (datechoisie > DateTime.Today)
        {
            lblError.Text = "Date : Ne doit pas être future !";
            return false;
        }

        return true;
    }
    protected void txtDateTo_TextChanged(object sender, EventArgs e)
    {
        if (!DateOK(txtDateTo.Text))
        {
            txtDateTo.Text = String.Empty;
            //SendClickEventToControl(Control txtDateTo);
        }
    }
    protected void btnRapportPourUser_Click(object sender, EventArgs e)
    {
        lblError.Text = "";
        if (ddlUsers.SelectedIndex == -1)
        {
            lblError.Text = "Choisissez Un Utilisateur pour cet Option!";
            return;
        }
        if (DatesOK())
        {
            HttpCookie cookie = new HttpCookie("DateFrom", txtDateFrom.Text);
            HttpCookie cookie2 = new HttpCookie("DateTo", txtDateTo.Text);

            Response.Cookies.Set(cookie);
            Response.Cookies.Set(cookie2);
            Response.Redirect("RapportPourUser.aspx?Username=" + ddlUsers.SelectedValue.ToString());
        }
    }
    protected void btnRapportPourUneSession_Click(object sender, EventArgs e)
    {
        lblError.Text = "";
        if (ddlSessions.SelectedIndex == -1)
        {
            lblError.Text = "Choisissez Une Session pour cet Option!";
            return;
        }


        Response.Redirect("RapportPourSession.aspx?SessionID=" + ddlSessions.SelectedValue.ToString());

    }
    protected void btnExcelPourClasse_Click____(object sender, EventArgs e)
    {
        //EcrireProgresEcran();
        //ThreadStart childthreat = new ThreadStart(EcrireProgresEcran);
        //Thread child = new Thread(childthreat);
        //child.Start();
        //child.Join();
    }

    public void EcrireProgresEcran()
    {
        lblError.Text = "Tableur Création En Cours ... Patientez ...";
        //Thread.Sleep(1000);

        Debug.WriteLine("OK");
        string sCurrentSessionToPrint = ddlSessionPourExcel.SelectedValue.ToString();
        int iCurrentSessionToPrint = int.Parse(sCurrentSessionToPrint);
        string sSql = string.Empty;

        if (iCurrentSessionToPrint > 0)
        {
            // Specific Session
            sSql = string.Format("SELECT DISTINCT P.Prenom, P.Nom, P.EtudiantID, IsNull(P.NIF, '') AS NIF, IsNull(P.Telephone1, '') AS Telephone1, IsNull(P.email, '') AS email,  C.NomCours,  " +
                " C.NumeroCours, CP.NoteSurCent, CO.SessionID, CO.HoraireID, H.HeureDebut, H.HeureFin, H.Jours " +
                " FROM Personnes P, CoursPris CP, CoursOfferts CO, Cours C, LesSessions LS, Horaires H  " +
                " WHERE P.PersonneID = CP.PersonneID AND CP.CoursOffertID = CO.CoursOffertID AND CO.NumeroCours = C.NumeroCours AND CO.HoraireID = H.HoraireID AND CO.SessionID =  {0} " +
                " AND C.ExamenEntree = 0 ORDER BY C.NumeroCours, CO.HoraireID, P.Nom, P.Prenom", sCurrentSessionToPrint);

        }
        else // Current Session
        {
            sSql = "SELECT DISTINCT P.Prenom, P.Nom, P.EtudiantID, IsNull(P.NIF, '') AS NIF, IsNull(P.Telephone1, '') AS Telephone1, IsNull(P.email, '') AS email,  C.NomCours,  " +
                " C.NumeroCours, CP.NoteSurCent, CO.SessionID, CO.HoraireID, H.HeureDebut, H.HeureFin, H.Jours " +
                " FROM Personnes P, CoursPris CP, CoursOfferts CO, Cours C, LesSessions LS, Horaires H  " +
                " WHERE P.PersonneID = CP.PersonneID AND CP.CoursOffertID = CO.CoursOffertID AND CO.NumeroCours = C.NumeroCours AND CO.HoraireID = H.HoraireID AND CO.Actif = 1  " +
                " AND C.ExamenEntree = 0 ORDER BY C.NumeroCours, CO.HoraireID, P.Nom, P.Prenom";
        }
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            int iError = 0;
            int nombreEtudiants = 0;

            try
            {
                sqlConn.Open();
                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("1");   // need to create a worksheet to start with before knowing the name of the class. We'll delete at the end.
                bool firstWorsheet = true;
                int sheetCount = 0;

                String numeroCoursOld = "", numeroCours = "", numeroCoursEtHoraire = "", horaire = "";
                String sSessionStartDate, sSessionEndDate;
                try
                {
                    int iRowIndex = 2;
                    SqlDataReader dt = db.GetDataReader(sSql, sqlConn);
                    if (dt != null)
                    {
                        sSessionStartDate = db.GetStartDateOfCurrentSession(sqlConn).ToString("dd-MMM-yyyy");
                        sSessionEndDate = db.GetEndDateOfCurrentSession(sqlConn).ToString("dd-MMM-yyyy");
                        dt.Read();
                        do
                        {
                            numeroCours = dt["NomCours"].ToString();
                            horaire = dt["HeureDebut"].ToString() + " - " + dt["HeureFin"].ToString() + " - " + dt["Jours"].ToString();
                            numeroCoursEtHoraire = numeroCours + horaire;
                            if (numeroCoursOld != numeroCoursEtHoraire)
                            {
                                if (!firstWorsheet)
                                {
                                    // Format worsheet before creating the new one
                                    nombreEtudiants += 5;  // add # of header lines
                                    for (int i = 1; i <= nombreEtudiants; i++)
                                    {
                                        for (int j = 1; j < 14; j++)
                                        {
                                            worksheet.Cell(i, j).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                        }
                                    }
                                    //Addition de bordure extérieure
                                    worksheet.Range("A1", "M" + nombreEtudiants.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                                    nombreEtudiants = 0;
                                }

                                firstWorsheet = false;

                                // Reset counter
                                iRowIndex = 5;
                                numeroCoursOld = numeroCoursEtHoraire;

                                sheetCount++;
                                worksheet = workbook.Worksheets.Add(sheetCount.ToString() + "-" + dt["NumeroCours"].ToString()); ;

                                // Creer Header
                                worksheet.Cell(1, 1).Value = "UNIVERSITE ESPOIR";
                                worksheet.Cell(2, 1).Value = dt["NomCours"].ToString() + " - " + dt["NumeroCours"].ToString();
                                worksheet.Cell(3, 1).Value = horaire;
                                worksheet.Cell(4, 1).Value = "Session : " + sSessionStartDate + " - " + sSessionEndDate;

                                // Names of columns
                                worksheet.Cell(5, 1).Value = "Nom";
                                worksheet.Cell(5, 2).Value = "Prénom";
                                worksheet.Cell(5, 3).Value = "Téléphone";
                                worksheet.Cell(5, 4).Value = "ID Etudiant";

                                // Cells merge horizontally
                                worksheet.Range("A1:M1").Merge();
                                worksheet.Range("A1:M1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                                worksheet.Range("A2:M2").Merge();
                                worksheet.Range("A2:M2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                                worksheet.Range("A3:M3").Merge();
                                worksheet.Range("A3:M3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                                worksheet.Range("A4:M4").Merge();
                                worksheet.Range("A4:M4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                                //ws.Style.Fill.BackgroundColor = XLColor.LightCyan;

                                // Column font weight
                                worksheet.Range("A1", "E1").Style.Font.Bold = true;
                                worksheet.Range("A2", "E2").Style.Font.Bold = true;
                                worksheet.Range("A3", "E3").Style.Font.Bold = true;
                                worksheet.Range("A4", "E4").Style.Font.Bold = true;
                                worksheet.Range("A5", "E5").Style.Font.Bold = true;

                                // Column Width
                                worksheet.Columns("A").Width = 12;
                                worksheet.Columns("B").Width = 15;
                                worksheet.Columns("C").Width = 12;
                                worksheet.Columns("D").Width = 10;

                                worksheet.Columns("E").Width = 5;
                                worksheet.Columns("F").Width = 5;
                                worksheet.Columns("G").Width = 5;
                                worksheet.Columns("H").Width = 5;
                                worksheet.Columns("I").Width = 5;
                                worksheet.Columns("J").Width = 5;
                                worksheet.Columns("K").Width = 5;
                                worksheet.Columns("L").Width = 5;
                                worksheet.Columns("M").Width = 5;

                                // Set the height of all rows in the worksheet
                                //worksheet.Rows().Height = 20;

                                // Orientation
                                worksheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;
                            }

                            // Creer entrees
                            iRowIndex++;
                            worksheet.Cell(iRowIndex, 1).Value = dt["Nom"].ToString().ToUpper();
                            worksheet.Cell(iRowIndex, 2).Value = dt["Prenom"].ToString();
                            worksheet.Cell(iRowIndex, 3).Value = dt["Telephone1"].ToString();
                            worksheet.Cell(iRowIndex, 4).Value = dt["EtudiantID"].ToString();
                            nombreEtudiants++;

                        } while (dt.Read());

                        // format last worsheet
                        nombreEtudiants += 5;  // add # of header lines
                        for (int i = 1; i <= nombreEtudiants; i++)
                        {
                            for (int j = 1; j < 14; j++)
                            {
                                worksheet.Cell(i, j).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            }
                        }
                        //Addition de bordure extérieure
                        worksheet.Range("A1", "M" + nombreEtudiants.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                        workbook.Worksheets.Delete("1");    // no need anymore

                        String fileName = "c:\\extras\\UEspoir\\UEspoir_" + DateTime.Today.Day.ToString() + "-" + DateTime.Today.Month.ToString() + "-" +
                            DateTime.Today.Year.ToString() + "-" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() +
                            DateTime.Now.Second.ToString() + ".xlsx";
                        workbook.SaveAs(fileName);
                        lblError.Text = "SpreaSheet Créé: " + fileName;

                        // Open Spreadsheet
                        Process compiler = new Process();
                        compiler.StartInfo.FileName = fileName;
                        //compiler.StartInfo.Arguments = "";
                        compiler.StartInfo.UseShellExecute = true;
                        //compiler.StartInfo.RedirectStandardOutput = true;
                        compiler.Start();
                    }
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                    lblError.Text = error;
                }
            }
            catch (Exception theException)
            {
                String errorMessage;
                errorMessage = "Error (" + iError.ToString() + "): ";
                errorMessage = String.Concat(errorMessage, theException.Message);
                lblError.Text = errorMessage;
            }
        }

    }
    protected void btnRapportPeriodeChoisie_Click(object sender, EventArgs e)
    {
        if (DatesOK())
        {
            HttpCookie cookie = new HttpCookie("DateFrom", txtDateFrom.Text);
            HttpCookie cookie2 = new HttpCookie("DateTo", txtDateTo.Text);

            Response.Cookies.Set(cookie);
            Response.Cookies.Set(cookie2);
            Response.Redirect("RapportPeriodeChoisie.aspx");
        };
    }
    protected void CreateSpreadSheetProcess_Thread()
    {
        string sCurrentSessionToPrint = ddlSessionPourExcel.SelectedValue.ToString();
        int iCurrentSessionToPrint = int.Parse(sCurrentSessionToPrint);
        string sSql = string.Empty;

        if (iCurrentSessionToPrint > 0)
        {
            // Specific Session
            sSql = string.Format("SELECT DISTINCT P.Prenom, P.Nom, P.EtudiantID, IsNull(P.NIF, '') AS NIF, IsNull(P.Telephone1, '') AS Telephone1, IsNull(P.email, '') AS email,  C.NomCours,  " +
                " C.NumeroCours, CP.NoteSurCent, CO.SessionID, CO.HoraireID, H.HeureDebut, H.HeureFin, H.Jours " +
                " FROM Personnes P, CoursPris CP, CoursOfferts CO, Cours C, LesSessions LS, Horaires H  " +
                " WHERE P.PersonneID = CP.PersonneID AND CP.CoursOffertID = CO.CoursOffertID AND CO.NumeroCours = C.NumeroCours AND CO.HoraireID = H.HoraireID AND CO.SessionID =  {0} " +
                " AND C.ExamenEntree = 0 ORDER BY C.NumeroCours, CO.HoraireID, P.Nom, P.Prenom", sCurrentSessionToPrint);

        }
        else // Current Session
        {
            sSql = "SELECT DISTINCT P.Prenom, P.Nom, P.EtudiantID, IsNull(P.NIF, '') AS NIF, IsNull(P.Telephone1, '') AS Telephone1, IsNull(P.email, '') AS email,  C.NomCours,  " +
                " C.NumeroCours, CP.NoteSurCent, CO.SessionID, CO.HoraireID, H.HeureDebut, H.HeureFin, H.Jours " +
                " FROM Personnes P, CoursPris CP, CoursOfferts CO, Cours C, LesSessions LS, Horaires H  " +
                " WHERE P.PersonneID = CP.PersonneID AND CP.CoursOffertID = CO.CoursOffertID AND CO.NumeroCours = C.NumeroCours AND CO.HoraireID = H.HoraireID AND CO.Actif = 1  " +
                " AND C.ExamenEntree = 0 ORDER BY C.NumeroCours, CO.HoraireID, P.Nom, P.Prenom";
        }
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            int iError = 0;
            int nombreEtudiants = 0;

            try
            {
                sqlConn.Open();
                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("1");   // need to create a worksheet to start with before knowing the name of the class. We'll delete at the end.
                bool firstWorsheet = true;
                int sheetCount = 0;

                String numeroCoursOld = "", numeroCours = "", numeroCoursEtHoraire = "", horaire = "";
                String sSessionStartDate, sSessionEndDate;
                try
                {
                    int iRowIndex = 2;
                    SqlDataReader dt = db.GetDataReader(sSql, sqlConn);
                    if (dt != null)
                    {
                        sSessionStartDate = db.GetStartDateOfCurrentSession(sqlConn).ToString("dd-MMM-yyyy");
                        sSessionEndDate = db.GetEndDateOfCurrentSession(sqlConn).ToString("dd-MMM-yyyy");
                        dt.Read();
                        do
                        {
                            numeroCours = dt["NomCours"].ToString();
                            horaire = dt["HeureDebut"].ToString() + " - " + dt["HeureFin"].ToString() + " - " + dt["Jours"].ToString();
                            numeroCoursEtHoraire = numeroCours + horaire;
                            if (numeroCoursOld != numeroCoursEtHoraire)
                            {
                                if (!firstWorsheet)
                                {
                                    // Format worsheet before creating the new one
                                    nombreEtudiants += 5;  // add # of header lines
                                    for (int i = 1; i <= nombreEtudiants; i++)
                                    {
                                        for (int j = 1; j < 14; j++)
                                        {
                                            worksheet.Cell(i, j).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                        }
                                    }
                                    //Addition de bordure extérieure
                                    worksheet.Range("A1", "M" + nombreEtudiants.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                                    nombreEtudiants = 0;
                                }

                                firstWorsheet = false;

                                // Reset counter
                                iRowIndex = 5;
                                numeroCoursOld = numeroCoursEtHoraire;

                                sheetCount++;
                                worksheet = workbook.Worksheets.Add(sheetCount.ToString() + "-" + dt["NumeroCours"].ToString()); ;

                                // Creer Header
                                worksheet.Cell(1, 1).Value = "UNIVERSITE ESPOIR";
                                worksheet.Cell(2, 1).Value = dt["NomCours"].ToString() + " - " + dt["NumeroCours"].ToString();
                                worksheet.Cell(3, 1).Value = horaire;
                                worksheet.Cell(4, 1).Value = "Session : " + sSessionStartDate + " - " + sSessionEndDate;

                                // Names of columns
                                worksheet.Cell(5, 1).Value = "Nom";
                                worksheet.Cell(5, 2).Value = "Prénom";
                                worksheet.Cell(5, 3).Value = "Téléphone";
                                worksheet.Cell(5, 4).Value = "ID Etudiant";

                                // Cells merge horizontally
                                worksheet.Range("A1:M1").Merge();
                                worksheet.Range("A1:M1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                                worksheet.Range("A2:M2").Merge();
                                worksheet.Range("A2:M2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                                worksheet.Range("A3:M3").Merge();
                                worksheet.Range("A3:M3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                                worksheet.Range("A4:M4").Merge();
                                worksheet.Range("A4:M4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                                //ws.Style.Fill.BackgroundColor = XLColor.LightCyan;

                                // Column font weight
                                worksheet.Range("A1", "E1").Style.Font.Bold = true;
                                worksheet.Range("A2", "E2").Style.Font.Bold = true;
                                worksheet.Range("A3", "E3").Style.Font.Bold = true;
                                worksheet.Range("A4", "E4").Style.Font.Bold = true;
                                worksheet.Range("A5", "E5").Style.Font.Bold = true;

                                // Column Width
                                worksheet.Columns("A").Width = 12;
                                worksheet.Columns("B").Width = 15;
                                worksheet.Columns("C").Width = 12;
                                worksheet.Columns("D").Width = 10;

                                worksheet.Columns("E").Width = 5;
                                worksheet.Columns("F").Width = 5;
                                worksheet.Columns("G").Width = 5;
                                worksheet.Columns("H").Width = 5;
                                worksheet.Columns("I").Width = 5;
                                worksheet.Columns("J").Width = 5;
                                worksheet.Columns("K").Width = 5;
                                worksheet.Columns("L").Width = 5;
                                worksheet.Columns("M").Width = 5;

                                // Set the height of all rows in the worksheet
                                //worksheet.Rows().Height = 20;

                                // Orientation
                                worksheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;
                            }

                            // Creer entrees
                            iRowIndex++;
                            worksheet.Cell(iRowIndex, 1).Value = dt["Nom"].ToString().ToUpper();
                            worksheet.Cell(iRowIndex, 2).Value = dt["Prenom"].ToString();
                            worksheet.Cell(iRowIndex, 3).Value = dt["Telephone1"].ToString();
                            worksheet.Cell(iRowIndex, 4).Value = dt["EtudiantID"].ToString();
                            nombreEtudiants++;

                        } while (dt.Read());

                        // format last worsheet
                        nombreEtudiants += 5;  // add # of header lines
                        for (int i = 1; i <= nombreEtudiants; i++)
                        {
                            for (int j = 1; j < 14; j++)
                            {
                                worksheet.Cell(i, j).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            }
                        }
                        //Addition de bordure extérieure
                        worksheet.Range("A1", "M" + nombreEtudiants.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                        workbook.Worksheets.Delete("1");    // no need anymore

                        String fileName = "c:\\extras\\UEspoir\\UEspoir_" + DateTime.Today.Day.ToString() + "-" + DateTime.Today.Month.ToString() + "-" +
                            DateTime.Today.Year.ToString() + "-" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() +
                            DateTime.Now.Second.ToString() + ".xlsx";
                        workbook.SaveAs(fileName);
                        lblError.Text = "SpreaSheet Créé: " + fileName;

                        // Open Spreadsheet
                        Process compiler = new Process();
                        compiler.StartInfo.FileName = fileName;
                        //compiler.StartInfo.Arguments = "";
                        compiler.StartInfo.UseShellExecute = true;
                        //compiler.StartInfo.RedirectStandardOutput = true;
                        compiler.Start();
                    }
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                    lblError.Text = error;
                }
            }
            catch (Exception theException)
            {
                String errorMessage;
                errorMessage = "Error (" + iError.ToString() + "): ";
                errorMessage = String.Concat(errorMessage, theException.Message);
                lblError.Text = errorMessage;
            }
        }
    }
    /// <summary>
    /// Get a list of all current session students for comparison with billing.
    /// Exemple: if a student has 1 class, his bill is 1000 + 1250; 2 classes, his bill is 1000 + 2*1250,
    /// we bill a maximum of 4 classes, then the 5th, 6th class is not billed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnFacture_Click(object sender, EventArgs e)
    {
        //String sSql = "SELECT PersonneID, COUNT(PersonneID) as NombreDeCours FROM CoursPris WHERE CoursOffertID IN (SELECT CoursOffertID " +
        //    " FROM CoursOfferts WHERE SessionID IN (SELECT SessionID FROM LesSessions WHERE SessionCourante = 1)) " +
        //    " AND NumeroCours NOT IN ('FRA001','MAT001','ANG001') GROUP BY PersonneID";

        //DB_Access db = new DB_Access(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString());
        ////Get List of current students with the number of classes registered
        //Hashtable htListeDesEtudiants = new Hashtable();

        //// Loop through all records
        //try
        //{
        //    SqlDataReader dt = db.GetDataReader(sSql);
        //    if (dt != null)
        //    {
        //        while (dt.Read())
        //        {
        //            htListeDesEtudiants.Add(dt["PersonneID"].ToString(), dt["NombreDeCours"].ToString());
        //        }
        //    }
        //    dt.Close();
        //    dt = null;
        //}
        //catch (Exception ex)
        //{
        //    Debug.WriteLine(ex.Message);
        //    lblError.Text = ex.Message;
        //    return;
        //}    

        //int iError = 0;
        //int nombreEtudiants = 0;

        //try
        //{
        //    var workbook = new XLWorkbook();
        //    var worksheet = workbook.Worksheets.Add("Facturation");

        //    String sPersonneIDOld = "", sPersonneID = "";
        //    String sSessionStartDate, sSessionEndDate;
        //    sSessionStartDate = db.GetStartDateOfCurrentSession().ToString("dd-MMM-yyyy");
        //    sSessionEndDate = db.GetEndDateOfCurrentSession().ToString("dd-MMM-yyyy");

        //        // Header 


        //                    // Creer Header
        //                    worksheet.Cell(1, 1).Value = "UNIVERSITE ESPOIR";
        //                    worksheet.Cell(2, 1).Value = "Rapport de Contrôle: Facturation";
        //                    worksheet.Cell(3, 1).Value = "Session : " + sSessionStartDate + " - " + sSessionEndDate;
        //                    //worksheet.Cell(3, 1).Value = 

        //                    // Names of columns
        //                    worksheet.Cell(5, 1).Value = "Nom";
        //                    worksheet.Cell(5, 2).Value = "Prénom";
        //                    worksheet.Cell(5, 3).Value = "Téléphone";
        //                    worksheet.Cell(5, 4).Value = "Email";
        //                    worksheet.Cell(5, 5).Value = "# De Cours";
        //                    worksheet.Cell(5, 6).Value = "Montant Facturé";

        //                    // Cells merge horizontally
        //                    worksheet.Range("A1:M1").Merge();
        //                    worksheet.Range("A1:M1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        //                    worksheet.Range("A2:M2").Merge();
        //                    worksheet.Range("A2:M2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        //                    worksheet.Range("A3:M3").Merge();
        //                    worksheet.Range("A3:M3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        //                    worksheet.Range("A4:M4").Merge();
        //                    worksheet.Range("A4:M4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        //                    //ws.Style.Fill.BackgroundColor = XLColor.LightCyan;

        //                    // Column font weight
        //                    worksheet.Range("A1", "E1").Style.Font.Bold = true;
        //                    worksheet.Range("A2", "E2").Style.Font.Bold = true;
        //                    worksheet.Range("A3", "E3").Style.Font.Bold = true;
        //                    worksheet.Range("A4", "E4").Style.Font.Bold = true;
        //                    worksheet.Range("A5", "E5").Style.Font.Bold = true;

        //                    // Column Width
        //                    worksheet.Columns("A").Width = 12;
        //                    worksheet.Columns("B").Width = 15;
        //                    worksheet.Columns("C").Width = 12;
        //                    worksheet.Columns("D").Width = 25;

        //                    worksheet.Columns("E").Width = 5;
        //                    worksheet.Columns("F").Width = 5;
        //                    worksheet.Columns("G").Width = 5;
        //                    worksheet.Columns("H").Width = 5;
        //                    worksheet.Columns("I").Width = 5;
        //                    worksheet.Columns("J").Width = 5;
        //                    worksheet.Columns("K").Width = 5;
        //                    worksheet.Columns("L").Width = 5;
        //                    worksheet.Columns("M").Width = 5;

        //                    // Set the height of all rows in the worksheet
        //                    //worksheet.Rows().Height = 20;

        //                    // Orientation
        //                    worksheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;


        //                           int iRowIndex = 2;

        //    sPersonneIDOld = sPersonneID;
        //    foreach (string spersonneid in htListeDesEtudiants)
        //    {
        //                // Creer entrees
        //                iRowIndex++;
        //                worksheet.Cell(iRowIndex, 1).Value = dt["Nom"].ToString().ToUpper();
        //                worksheet.Cell(iRowIndex, 2).Value = dt["Prenom"].ToString();
        //                worksheet.Cell(iRowIndex, 3).Value = dt["Telephone1"].ToString();
        //                worksheet.Cell(iRowIndex, 4).Value = dt["Email"].ToString();
        //                nombreEtudiants++;

        //    }


        //            //Addition de bordure extérieure
        //            worksheet.Range("A1", "M" + nombreEtudiants.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

        //            workbook.Worksheets.Delete("1");    // no need anymore

        //            String fileName = "c:\\extras\\UEspoir\\UEspoirFacture_" + DateTime.Today.Day.ToString() + "-" + DateTime.Today.Month.ToString() + "-" +
        //                DateTime.Today.Year.ToString() + "-" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() +
        //                DateTime.Now.Second.ToString() + ".xlsx";
        //            workbook.SaveAs(fileName);
        //            lblError.Text = "SpreaSheet Créé: " + fileName;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string error = ex.Message;
        //        lblError.Text = error;
        //    }
        //}
        //catch (Exception theException)
        //{
        //    String errorMessage;
        //    errorMessage = "Error (" + iError.ToString() + "): ";
        //    errorMessage = String.Concat(errorMessage, theException.Message);
        //    lblError.Text = errorMessage;
        //}
    }
    protected void btnHoraire_Click(object sender, EventArgs e)
    {
        CreateSpreadSheetProcessForClasseSchedule();
    }
    void CreateSpreadSheetProcessForClasseSchedule()
    {
        string sSql = string.Empty;

        sSql = "SELECT H.Jours, H.HeureDebut, H.HeureFin, H.NumeroJour, CO.HoraireID, CO.NumeroCours, CO.ProfesseurID, " +
            " P.Prenom + ' ' + P.Nom AS Professeur, C.NomCours " +
            " FROM Horaires H, CoursOfferts CO, Professeurs P, Cours C  " +
            " WHERE H.HoraireID = CO.HoraireID AND CO.ProfesseurID = P.ProfesseurID AND CO.NumeroCours = C.NumeroCours " +
            " AND CO.NumeroCours NOT IN ('FRA001', 'MAT001', 'ANG001') AND CO.CoursOffertID IN (SELECT CoursOffertID FROM CoursOfferts WHERE " +
            " SessionID IN (SELECT SessionID FROM LesSessions WHERE SessionCourante = 1) ) " +
            " ORDER BY NumeroJour, Ordre";

        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            int iError = 0;

            try
            {
                sqlConn.Open();
                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("1");   // need to create a worksheet to start with before knowing the name of the class
                                                                // We'll delete at the end
                bool firstPass = true;

                try
                {
                    int iRowIndex = 6;
                    SqlDataReader dt = db.GetDataReader(sSql, sqlConn);
                    if (dt != null)
                    {
                        dt.Read();
                        do
                        {
                            if (firstPass)
                            {
                                firstPass = false;
                                worksheet = workbook.Worksheets.Add("Horaire des Classes"); ;

                                // Creer Header
                                worksheet.Cell(1, 1).Value = "UNIVERSITE ESPOIR";
                                worksheet.Cell(2, 1).Value = "Horaire des Cours";
                                using (SqlConnection sqlConn2 = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
                                {
                                    sqlConn2.Open();
                                    worksheet.Cell(3, 1).Value = "Session : " + db.GetStartDateOfCurrentSession(sqlConn2).ToString("dd-MMM-yyyy")
                                    + " - " + db.GetEndDateOfCurrentSession(sqlConn2).ToString("dd-MMM-yyyy");
                                }
                                //worksheet.Cell(4, 1).Value = "";

                                // Names of columns
                                worksheet.Cell(5, 1).Value = "Jour";
                                worksheet.Cell(5, 2).Value = "De (Hre) ";
                                worksheet.Cell(5, 3).Value = "A (Hre)";
                                worksheet.Cell(5, 4).Value = "Numéro Cours";
                                worksheet.Cell(5, 5).Value = "Cours";
                                worksheet.Cell(5, 6).Value = "Professeur";

                                // Cells merge horizontally
                                worksheet.Range("A1:F1").Merge();
                                worksheet.Range("A1:F1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                                worksheet.Range("A2:F2").Merge();
                                worksheet.Range("A2:F2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                                worksheet.Range("A3:F3").Merge();
                                worksheet.Range("A3:F3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                                worksheet.Range("A4:F4").Merge();
                                worksheet.Range("A4:F4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                                //ws.Style.Fill.BackgroundColor = XLColor.LightCyan;

                                // Column font weight
                                worksheet.Range("A1", "F1").Style.Font.Bold = true;
                                worksheet.Range("A2", "F2").Style.Font.Bold = true;
                                worksheet.Range("A3", "F3").Style.Font.Bold = true;
                                worksheet.Range("A4", "F4").Style.Font.Bold = true;
                                worksheet.Range("A5", "F5").Style.Font.Bold = true;
                                //worksheet.Range("A5", "E6").Style.Font.Bold = true;

                                // Column Width
                                worksheet.Columns("A").Width = 15;
                                worksheet.Columns("B").Width = 12;
                                worksheet.Columns("C").Width = 12;
                                worksheet.Columns("D").Width = 15;
                                worksheet.Columns("E").Width = 40;
                                worksheet.Columns("F").Width = 30;

                                // Set the height of all rows in the worksheet
                                //worksheet.Rows().Height = 20;

                                // Orientation
                                worksheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;
                            }

                            // Creer entrees
                            worksheet.Cell(iRowIndex, 1).Value = dt["Jours"].ToString();
                            worksheet.Cell(iRowIndex, 2).Value = dt["HeureDebut"].ToString();
                            worksheet.Cell(iRowIndex, 3).Value = dt["HeureFin"].ToString();
                            worksheet.Cell(iRowIndex, 4).Value = dt["NumeroCours"].ToString();
                            worksheet.Cell(iRowIndex, 5).Value = dt["NomCours"].ToString();
                            worksheet.Cell(iRowIndex, 6).Value = dt["Professeur"].ToString();
                            iRowIndex++;
                        } while (dt.Read());

                        // format worsheet
                        iRowIndex++;
                        for (int i = 1; i <= iRowIndex; i++)
                        {
                            for (int j = 1; j <= 6; j++)
                            {
                                worksheet.Cell(i, j).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            }
                        }
                        //Addition de bordure extérieure
                        worksheet.Range("A1", "F" + iRowIndex.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                        workbook.Worksheets.Delete("1");    // no need anymore

                        String fileName = "c:\\extras\\UEspoir\\UEspoir_Horaire" + DateTime.Today.Day.ToString() + "-" + DateTime.Today.Month.ToString() + "-" +
                            DateTime.Today.Year.ToString() + "-" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() +
                            DateTime.Now.Second.ToString() + ".xlsx";
                        workbook.SaveAs(fileName);
                        lblError.Text = "SpreaSheet Créé: " + fileName;

                        if (ImpersonateValidUser(sUserName, sDomain, sPass))
                        {
                            // Open Spreadsheet
                            Process compiler = new Process();
                            compiler.StartInfo.FileName = fileName;
                            //compiler.StartInfo.Arguments = "";
                            compiler.StartInfo.UseShellExecute = true;
                            //compiler.StartInfo.RedirectStandardOutput = true;
                            compiler.Start();

                            Thread.Sleep(3000);
                            compiler.Dispose();
                        }
                        else
                        {
                            //Your impersonation failed. Therefore, this is a fail-safe mechanism here.
                            lblError.Text = string.Format("Fichier crée: {0}!", fileName);
                            lblError.ForeColor = System.Drawing.Color.Green;
                        }
                    }
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                    lblError.Text = error;
                    lblError.ForeColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception theException)
            {
                String errorMessage;
                errorMessage = "Error (" + iError.ToString() + "): ";
                errorMessage = String.Concat(errorMessage, theException.Message);
                lblError.Text = errorMessage;
                lblError.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
    protected void btnRelevesTousEtudiants_Click(object sender, EventArgs e)
    {
        Response.Redirect("ReleveNotesTousLesEtudiants.aspx");
    }
    protected void btnControle_Click(object sender, EventArgs e)
    {
        Response.Redirect("ControleSessionCourante.aspx");
    }

    protected void btnCredits_Click(object sender, EventArgs e)
    {
        Response.Redirect("EtudiantsNombreCredits.aspx");
    }

    protected void btnExcelPourClasse_Click(object sender, EventArgs e)
    {
        lblError.Text = "Tableur Création En Cours ... Patientez ...";
        //Thread.Sleep(1000);

        Debug.WriteLine("OK");
        string sCurrentSessionToPrint = ddlSessionPourExcel.SelectedValue.ToString();
        int iCurrentSessionToPrint = int.Parse(sCurrentSessionToPrint);
        string sSql = string.Empty;

        if (iCurrentSessionToPrint > 0)
        {
            // Specific Session
            sSql = string.Format("SELECT DISTINCT P.Prenom, P.Nom, P.EtudiantID, IsNull(P.NIF, '') AS NIF, IsNull(P.Telephone1, '') AS Telephone1, IsNull(P.email, '') AS email,  C.NomCours,  " +
                " C.NumeroCours, CP.NoteSurCent, CO.SessionID, CO.HoraireID, H.HeureDebut, H.HeureFin, H.Jours " +
                " FROM Personnes P, CoursPris CP, CoursOfferts CO, Cours C, LesSessions LS, Horaires H  " +
                " WHERE P.PersonneID = CP.PersonneID AND CP.CoursOffertID = CO.CoursOffertID AND CO.NumeroCours = C.NumeroCours AND CO.HoraireID = H.HoraireID AND CO.SessionID =  {0} " +
                " AND C.ExamenEntree = 0 ORDER BY C.NumeroCours, CO.HoraireID, P.Nom, P.Prenom", sCurrentSessionToPrint);

        }
        else // Current Session
        {
            sSql = "SELECT DISTINCT P.Prenom, P.Nom, P.EtudiantID, IsNull(P.NIF, '') AS NIF, IsNull(P.Telephone1, '') AS Telephone1, IsNull(P.email, '') AS email,  C.NomCours,  " +
                " C.NumeroCours, CP.NoteSurCent, CO.SessionID, CO.HoraireID, H.HeureDebut, H.HeureFin, H.Jours " +
                " FROM Personnes P, CoursPris CP, CoursOfferts CO, Cours C, LesSessions LS, Horaires H  " +
                " WHERE P.PersonneID = CP.PersonneID AND CP.CoursOffertID = CO.CoursOffertID AND CO.NumeroCours = C.NumeroCours AND CO.HoraireID = H.HoraireID AND CO.Actif = 1  " +
                " AND C.ExamenEntree = 0 ORDER BY C.NumeroCours, CO.HoraireID, P.Nom, P.Prenom";
        }
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            int iError = 0;
            int nombreEtudiants = 0;

            try
            {
                sqlConn.Open();
                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("1");   // need to create a worksheet to start with before knowing the name of the class. We'll delete at the end.
                bool firstWorsheet = true;
                int sheetCount = 0;

                String numeroCoursOld = "", numeroCours = "", numeroCoursEtHoraire = "", horaire = "";
                String sSessionStartDate, sSessionEndDate;
                try
                {
                    int iRowIndex = 2;
                    SqlDataReader dt = db.GetDataReader(sSql, sqlConn);
                    if (dt != null)
                    {
                        using (SqlConnection sqlConn2 = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
                        {
                            sqlConn2.Open();
                            sSessionStartDate = db.GetStartDateOfCurrentSession(sqlConn2).ToString("dd-MMM-yyyy");
                            sSessionEndDate = db.GetEndDateOfCurrentSession(sqlConn2).ToString("dd-MMM-yyyy");
                        }
                        dt.Read();
                        do
                        {
                            numeroCours = dt["NomCours"].ToString();
                            horaire = dt["HeureDebut"].ToString() + " - " + dt["HeureFin"].ToString() + " - " + dt["Jours"].ToString();
                            numeroCoursEtHoraire = numeroCours + horaire;
                            if (numeroCoursOld != numeroCoursEtHoraire)
                            {
                                if (!firstWorsheet)
                                {
                                    // Format worsheet before creating the new one
                                    nombreEtudiants += 5;  // add # of header lines
                                    for (int i = 1; i <= nombreEtudiants; i++)
                                    {
                                        for (int j = 1; j < 14; j++)
                                        {
                                            worksheet.Cell(i, j).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                        }
                                    }
                                    //Addition de bordure extérieure
                                    worksheet.Range("A1", "M" + nombreEtudiants.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                                    nombreEtudiants = 0;
                                }

                                firstWorsheet = false;

                                // Reset counter
                                iRowIndex = 5;
                                numeroCoursOld = numeroCoursEtHoraire;

                                sheetCount++;
                                worksheet = workbook.Worksheets.Add(sheetCount.ToString() + "-" + dt["NumeroCours"].ToString()); ;

                                // Creer Header
                                worksheet.Cell(1, 1).Value = "UNIVERSITE ESPOIR";
                                worksheet.Cell(2, 1).Value = dt["NomCours"].ToString() + " - " + dt["NumeroCours"].ToString();
                                worksheet.Cell(3, 1).Value = horaire;
                                worksheet.Cell(4, 1).Value = "Session : " + sSessionStartDate + " - " + sSessionEndDate;

                                // Names of columns
                                worksheet.Cell(5, 1).Value = "Nom";
                                worksheet.Cell(5, 2).Value = "Prénom";
                                worksheet.Cell(5, 3).Value = "Téléphone";
                                worksheet.Cell(5, 4).Value = "ID Etudiant";

                                // Cells merge horizontally
                                worksheet.Range("A1:M1").Merge();
                                worksheet.Range("A1:M1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                                worksheet.Range("A2:M2").Merge();
                                worksheet.Range("A2:M2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                                worksheet.Range("A3:M3").Merge();
                                worksheet.Range("A3:M3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                                worksheet.Range("A4:M4").Merge();
                                worksheet.Range("A4:M4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                                //ws.Style.Fill.BackgroundColor = XLColor.LightCyan;

                                // Column font weight
                                worksheet.Range("A1", "E1").Style.Font.Bold = true;
                                worksheet.Range("A2", "E2").Style.Font.Bold = true;
                                worksheet.Range("A3", "E3").Style.Font.Bold = true;
                                worksheet.Range("A4", "E4").Style.Font.Bold = true;
                                worksheet.Range("A5", "E5").Style.Font.Bold = true;

                                // Column Width
                                worksheet.Columns("A").Width = 12;
                                worksheet.Columns("B").Width = 15;
                                worksheet.Columns("C").Width = 12;
                                worksheet.Columns("D").Width = 10;

                                worksheet.Columns("E").Width = 5;
                                worksheet.Columns("F").Width = 5;
                                worksheet.Columns("G").Width = 5;
                                worksheet.Columns("H").Width = 5;
                                worksheet.Columns("I").Width = 5;
                                worksheet.Columns("J").Width = 5;
                                worksheet.Columns("K").Width = 5;
                                worksheet.Columns("L").Width = 5;
                                worksheet.Columns("M").Width = 5;

                                // Set the height of all rows in the worksheet
                                //worksheet.Rows().Height = 20;

                                // Orientation
                                worksheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;
                            }

                            // Creer entrees
                            iRowIndex++;
                            worksheet.Cell(iRowIndex, 1).Value = dt["Nom"].ToString().ToUpper();
                            worksheet.Cell(iRowIndex, 2).Value = dt["Prenom"].ToString();
                            worksheet.Cell(iRowIndex, 3).Value = dt["Telephone1"].ToString();
                            worksheet.Cell(iRowIndex, 4).Value = dt["EtudiantID"].ToString();
                            nombreEtudiants++;

                        } while (dt.Read());

                        // format last worsheet
                        nombreEtudiants += 5;  // add # of header lines
                        for (int i = 1; i <= nombreEtudiants; i++)
                        {
                            for (int j = 1; j < 14; j++)
                            {
                                worksheet.Cell(i, j).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            }
                        }
                        //Addition de bordure extérieure
                        worksheet.Range("A1", "M" + nombreEtudiants.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                        workbook.Worksheets.Delete("1");    // no need anymore

                        String fileName = "c:\\extras\\UEspoir\\UEspoir_" + DateTime.Today.Day.ToString() + "-" + DateTime.Today.Month.ToString() + "-" +
                            DateTime.Today.Year.ToString() + "-" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() +
                            DateTime.Now.Second.ToString() + ".xlsx";
                        workbook.SaveAs(fileName);
                        lblError.Text = "SpreaSheet Créé: " + fileName;

                        // Open Spreadsheet
                        Process compiler = new Process();
                        compiler.StartInfo.FileName = fileName;
                        //compiler.StartInfo.Arguments = "";
                        compiler.StartInfo.UseShellExecute = true;
                        //compiler.StartInfo.RedirectStandardOutput = true;
                        compiler.Start();
                    }
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                    lblError.Text = error;
                }
            }
            catch (Exception theException)
            {
                String errorMessage;
                errorMessage = "Error (" + iError.ToString() + "): ";
                errorMessage = String.Concat(errorMessage, theException.Message);
                lblError.Text = errorMessage;
            }
        }

    }
}
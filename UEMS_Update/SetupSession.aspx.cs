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

public partial class AdminPages_SetupSession : System.Web.UI.Page
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
                }
                catch
                {
                    lblError.Text = "ERREUR: Connexion Base de Données!";
                    return;
                }
                if (db.isCurrentWindowsInThisGroup(sGroup) || (db.IsCustomAdminMember(db.GetWindowsUser(), sqlConn)))
                {
                    calExtender1.SelectedDate = DateTime.Today.Add(TimeSpan.FromDays(7.0));
                    calExtender2.SelectedDate = DateTime.Today.Add(TimeSpan.FromDays(105.0));
                    RemplirSessionsExistantes();
                }
                else
                {
                    lblError.Text = "PERMISSION : Vous n'avez pas la permission d'accéder à cette PAGE!!! ";
                    btnCancel.Enabled = false;
                    btnNext.Enabled = false;
                    txtDateEnd.Enabled = false;
                    txtDateStart.Enabled = false;
                    txtDescription.Enabled = false;
                }
            }
        }
    }

    protected void RemplirSessionsExistantes()
    {
        String returnedString = String.Empty;
        String sessionCourante = String.Empty, sDateDebut, sDateFin;

        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            try
            {
                sqlConn.Open();
            }
            catch
            {
                lblError.Text = "ERREUR: Connexion Base de Données!";
                return;
            }
            try
            {
                string sSql = String.Format("SELECT SessionDescription, SessionDateDebut, SessionDateFin, " +
                    " IsNull(SessionExtra, '') AS SessionExtra, Actif, SessionCourante FROM LesSessions");

                SqlDataReader dt = db.GetDataReader(sSql, sqlConn);
                if (dt != null)
                {
                    returnedString += String.Format("<table class='table table-striped table-bordered table-hover'>");
                    returnedString += @"<tr class='odd gradeX'><th class='center' colspan='6'>Sessions Existantes</th></tr>";
                    returnedString += @"<tr><th>Courante</th><th>Description</th><th>Début</th><th>Fin</th><th>Actif</th></tr>";
                    while (dt.Read())
                    {
                        sessionCourante = dt["SessionCourante"].ToString() == "1" ? "*OUI*" : "";
                        //sDateDebut = dt["SessionDateDebut"].ToString().Substring(0, dt["SessionDateDebut"].ToString().IndexOf(" "));
                        //sDateFin = dt["SessionDateFin"].ToString().Substring(0, dt["SessionDateFin"].ToString().IndexOf(" "));
                        sDateDebut = DateTime.Parse(dt["SessionDateDebut"].ToString()).ToString("dd MMMM yyyy");
                        sDateFin = DateTime.Parse(dt["SessionDateFin"].ToString()).ToString("dd MMMM yyyy");

                        returnedString += String.Format(@"<tr class='odd gradeX'>");        //1 Row for each
                        returnedString += String.Format(@"<td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td>" +
                            "<td>{4}</td>", sessionCourante, dt["SessionDescription"].ToString(), sDateDebut, sDateFin,
                            dt["Actif"].ToString());
                        returnedString += String.Format(@"</tr>");
                    }
                    returnedString += @"</table>";
                    litExistingSessions.Text = returnedString;
                }
                db = null;
            }
            catch (Exception ex)
            {
                litExistingSessions.Text = ex.Message;

            }
        }
        db = null;
    }

    protected void btnNext_Click(object sender, EventArgs e)
    {
        DateTime start = DateTime.Parse(txtDateStart.Text.Trim());
        DateTime end = DateTime.Parse(txtDateEnd.Text.Trim());

        lblError.Text = "";

        if (start <= DateTime.Today)
        {
            lblError.Text = "Date Début Session doit être supérieure à la date d'Aujourd'hui";
            lblError.ForeColor = System.Drawing.Color.Red;
            return;
        }
        if (end < DateTime.Today.Add(TimeSpan.FromDays(71.0)))
        {
            lblError.Text = "Date Fin Session doit être supérieure au moins 10 semaine à la date d'Aujourd'hui";
            lblError.ForeColor = System.Drawing.Color.Red;
            return;
        }
        if (txtDescription.Text.Trim() == String.Empty)
        {
            lblError.Text = "La Description est Obligatoire!!!";
            lblError.ForeColor = System.Drawing.Color.Red;
            return;
        }


        lblError.ForeColor = System.Drawing.Color.Green;
        SqlTransaction transaction = null, transaction2 = null;
        try
        {
            int SessionCouranteID = 0;
            using (SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString))
            {
                SqlCommand cmdUpdateLesSessions = new SqlCommand();
                SqlCommand cmdUpdateCoursOfferts = new SqlCommand();
                SqlCommand cmdInsertNewSession = new SqlCommand();
                SqlCommand cmdInsertNewCoursOffertFRA001 = new SqlCommand();
                SqlCommand cmdInsertNewCoursOffertMAT001 = new SqlCommand();
                SqlCommand cmdInsertNewCoursOffertANG001 = new SqlCommand();

                myConnection.Open();
                transaction = myConnection.BeginTransaction();  // Pas de sauvegarder si l'une des transactions n'est pas à succès

                //Update LesSessions
                cmdUpdateLesSessions.CommandText = "UPDATE LesSessions SET SessionCourante = 0";
                cmdUpdateLesSessions.Connection = myConnection;
                cmdUpdateLesSessions.Transaction = transaction;

                //Update CoursOfferts
                cmdUpdateCoursOfferts.CommandText = "UPDATE CoursOfferts SET Actif = 0";
                cmdUpdateCoursOfferts.Connection = myConnection;
                cmdUpdateCoursOfferts.Transaction = transaction;

                //LesSessions
                cmdInsertNewSession.CommandText = "INSERT LesSessions (SessionDescription, SessionDateDebut, SessionDateFin, SessionCourante) VALUES " +
                    " (@SessionDescription, @SessionDateDebut, @SessionDateFin, @SessionCourante)";
                cmdInsertNewSession.Parameters.Add("@SessionDescription", SqlDbType.NVarChar).Value = txtDescription.Text;
                cmdInsertNewSession.Parameters.Add("@SessionDateDebut", SqlDbType.Date).Value = start.Date;
                cmdInsertNewSession.Parameters.Add("@SessionDateFin", SqlDbType.Date).Value = end.Date;
                cmdInsertNewSession.Parameters.Add("@SessionCourante", SqlDbType.Int).Value = 1;
                cmdInsertNewSession.Connection = myConnection;
                cmdInsertNewSession.Transaction = transaction;

                try
                {
                    cmdUpdateLesSessions.ExecuteNonQuery();
                    cmdUpdateCoursOfferts.ExecuteNonQuery();
                    cmdInsertNewSession.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    lblError.Text = "ERREUR: " + ex.ToString();
                    lblError.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                transaction2 = myConnection.BeginTransaction();  // Pas de sauvegarder si l'une des transactions n'est pas à succès

                // Offer the following classes to start with (needed for new inscriptions): FRA001 - ANG001 - MAT001
                // ProfesseurID = 3   -    HoraireID = 11        -   NotePassage = 65
                // Insert 3 records to LesSessions
                DB_Access db = new DB_Access();
                using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
                {
                    try
                    {
                        sqlConn.Open();
                        SessionCouranteID = db.GetIdOfSessionCourante(sqlConn);
                    }
                    catch
                    {
                        lblError.Text = "ERREUR: Connexion Base de Données!";
                        return;
                    }
                    
                }
                db = null;
                cmdInsertNewCoursOffertFRA001.CommandText = "INSERT CoursOfferts (NumeroCours, SessionID, HoraireID) VALUES (@NumeroCours, @SessionID, @HoraireID) ";
                cmdInsertNewCoursOffertFRA001.Parameters.Add("@NumeroCours", SqlDbType.NVarChar).Value = ConfigurationManager.AppSettings["ExamenEntreeFRA"].ToString();
                cmdInsertNewCoursOffertFRA001.Parameters.Add("@SessionID", SqlDbType.Int).Value = SessionCouranteID;
                cmdInsertNewCoursOffertFRA001.Parameters.Add("@HoraireID", SqlDbType.Int).Value = int.Parse(ConfigurationManager.AppSettings["DefaultHoraireID"].ToString());

                cmdInsertNewCoursOffertFRA001.Connection = myConnection;
                cmdInsertNewCoursOffertFRA001.Transaction = transaction2;

                cmdInsertNewCoursOffertMAT001.CommandText = "INSERT CoursOfferts (NumeroCours, SessionID, HoraireID) VALUES (@NumeroCours, @SessionID, @HoraireID) ";
                cmdInsertNewCoursOffertMAT001.Parameters.Add("@NumeroCours", SqlDbType.NVarChar).Value = ConfigurationManager.AppSettings["ExamenEntreeMAT"].ToString();
                cmdInsertNewCoursOffertMAT001.Parameters.Add("@SessionID", SqlDbType.Int).Value = SessionCouranteID;
                cmdInsertNewCoursOffertMAT001.Parameters.Add("@HoraireID", SqlDbType.Int).Value = int.Parse(ConfigurationManager.AppSettings["DefaultHoraireID"].ToString());

                cmdInsertNewCoursOffertMAT001.Connection = myConnection;
                cmdInsertNewCoursOffertMAT001.Transaction = transaction2;

                cmdInsertNewCoursOffertANG001.CommandText = "INSERT CoursOfferts (NumeroCours, SessionID, HoraireID) VALUES (@NumeroCours, @SessionID, @HoraireID) ";
                cmdInsertNewCoursOffertANG001.Parameters.Add("@NumeroCours", SqlDbType.NVarChar).Value = ConfigurationManager.AppSettings["ExamenEntreeANG"].ToString();
                cmdInsertNewCoursOffertANG001.Parameters.Add("@SessionID", SqlDbType.Int).Value = SessionCouranteID;
                cmdInsertNewCoursOffertANG001.Parameters.Add("@HoraireID", SqlDbType.Int).Value = int.Parse(ConfigurationManager.AppSettings["DefaultHoraireID"].ToString());

                cmdInsertNewCoursOffertANG001.Connection = myConnection;
                cmdInsertNewCoursOffertANG001.Transaction = transaction2;

                try
                {
                    cmdInsertNewCoursOffertFRA001.ExecuteNonQuery();
                    cmdInsertNewCoursOffertMAT001.ExecuteNonQuery();
                    cmdInsertNewCoursOffertANG001.ExecuteNonQuery();

                    transaction2.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    lblError.Text = "ERREUR: " + ex.ToString();
                    lblError.ForeColor = System.Drawing.Color.Red;
                    return;
                }
            }
            //Response.Redirect(Request.RawUrl);      //reload page - recommencer
            Response.Redirect("ChoisirCoursPourSession.aspx");
        }
        catch (Exception ex)
        {
            lblError.Text += "  --  ERREUR: " + ex.ToString();
            lblError.ForeColor = System.Drawing.Color.Red;
            return;
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }
}
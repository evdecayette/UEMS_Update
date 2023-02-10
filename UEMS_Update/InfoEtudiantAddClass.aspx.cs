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

public partial class InfoEtudiantAddClass : System.Web.UI.Page
{
    String sPersonneID = String.Empty;
    String sNumeroCours = String.Empty;
    String sCoursOffert = String.Empty;
    string ConnectionString = XCryptEngine.ConnectionStringEncryption.Decrypt(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
    protected void Page_Load(object sender, EventArgs e)
    {
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
        {
            try
            {
                sqlConn.Open();
                sPersonneID = Request.QueryString["PersonneID"];
                sNumeroCours = Request.QueryString["NumeroCours"];
                sCoursOffert = Request.QueryString["CoursOffert"];

                // Enlever les cookies
                Response.Cookies.Remove("PersonneID");                          // First method
                HttpCookie cookiePersonneID = new HttpCookie("PersonneID", string.Empty); // Second method, just in case
                Response.Cookies.Set(cookiePersonneID);

                Response.Cookies.Remove("NumeroCours");                          // First method
                HttpCookie cookieNumeroCours = new HttpCookie("NumeroCours", string.Empty); // Second method, just in case
                Response.Cookies.Set(cookieNumeroCours);

                Response.Cookies.Remove("CoursOffert");                          // First method
                HttpCookie cookieCoursOffert = new HttpCookie("CourOffert", string.Empty); // Second method, just in case
                Response.Cookies.Set(cookieCoursOffert);

                string sSql = String.Format("INSERT INTO CoursPris (PersonneID, NumeroCours, CoursOffertID, NotePassage, CreeParUsername) " +
                    " VALUES (@PersonneID, @NumeroCours, @CoursOffertID, @NotePassage, @CreeParUsername)");

                SqlParameter paramPersonneID = new SqlParameter("@PersonneID", SqlDbType.Text);
                paramPersonneID.Value = sPersonneID;
                SqlParameter paramNumeroCours = new SqlParameter("@NumeroCours", SqlDbType.Text);
                paramNumeroCours.Value = sNumeroCours;

                SqlParameter ParamNotePassage = new SqlParameter("@NotePassage", SqlDbType.Float);

                SqlParameter paramCoursOffertID = new SqlParameter("@CoursOffertID", SqlDbType.Int);
                paramCoursOffertID.Value = sCoursOffert;

                SqlParameter paramCreeParUsername = new SqlParameter("@CreeParUsername", SqlDbType.VarChar);
                paramCreeParUsername.Value = db.GetWindowsUser();

                //paramCoursOffertID.Value = db.GetOneIntegerWithParams("SELECT CoursOffertID FROM CoursOfferts C, LesSessions L " +
                //    " WHERE C.SessionID = L.SessionID AND NumeroCours = '{0}' AND L.SessionCourante = 1", paramNumeroCours);
                SqlDataReader dt = db.GetDataReader(string.Format("SELECT CoursOffertID, NotePassage FROM CoursOfferts C, LesSessions L " +
                    " WHERE C.SessionID = L.SessionID AND NumeroCours = '{0}' AND L.SessionCourante = 1", sNumeroCours), sqlConn);

                if (dt != null)
                {
                    if (dt.Read())
                    {
                        int MoisParSession = int.Parse(ConfigurationManager.AppSettings["NombreDeMoisParSession"].ToString());
                        //paramCoursOffertID.Value = Int32.Parse(dt["CoursOffertID"].ToString());
                        ParamNotePassage.Value = Double.Parse(dt["NotePassage"].ToString());
                        dt.Close();
                        using (SqlConnection sqlConn1 = new SqlConnection(ConnectionString))
                        {
                            try
                            {
                                sqlConn1.Open();
                                db.IssueCommandWithParams(sSql, sqlConn1, paramPersonneID, paramNumeroCours, paramCoursOffertID, ParamNotePassage, paramCreeParUsername);
                                //db.Facturer(sPersonneID, sNumeroCours, MoisParSession, sqlConn1);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Erreur: Inner USING ..." + ex.Message);
                            }
                        }
                    }
                    else
                    {

                    }
                }
                db = null;
                Response.Redirect(String.Format("InfoEtudiant.aspx?personneid={0}", sPersonneID));
            }
            catch (Exception Excep)
            {
                Debug.WriteLine(Excep.Message);
                db = null;
            }
        }
    }
}
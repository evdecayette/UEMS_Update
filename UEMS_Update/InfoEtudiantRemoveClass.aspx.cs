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

public partial class InfoEtudiantRemoveClass : System.Web.UI.Page
{
    String sPersonneID = String.Empty;
    String sCoursPrisID = String.Empty;
    String sNumeroCours = String.Empty;
    String sCoursOffertID = String.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            try
            {
                sqlConn.Open();
                sCoursPrisID = Request.QueryString["CoursPrisID"];
                sNumeroCours = Request.QueryString["NumeroCours"];
                sPersonneID = Request.QueryString["PersonneID"];
                sCoursOffertID = Request.QueryString["CoursOffertID"];

                int iMaxClasses = Int16.Parse(ConfigurationManager.AppSettings["MaximumClassesFacturees"].ToString());
                int iNombreCours = db.NombreDeCoursCetteSession(sPersonneID, sqlConn);

                string sSqlInsert = String.Format("INSERT INTO CoursEnleves (CoursPrisID, NumeroCours, EffaceParUserName, PersonneID, CoursOffertID) " +
                    " VALUES ( @CoursPrisID, @NumeroCours, @EffaceParUserName, @PersonneID, @CoursOffertID)");

                string sSqlFactureNegative = String.Format("INSERT INTO MontantsDus (PersonneID, CodeObligation, Montant) SELECT '{0}', '{1}', " +
                                " Montant*(-1)*{2} FROM Obligations WHERE Code = '{1}'", sPersonneID, "FM", ConfigurationManager.AppSettings["NombreDeMoisParSession"].ToString());

                string sSqlDelete = String.Format("DELETE CoursPris WHERE CoursPrisID = @CoursPrisID");

                SqlParameter paramCoursPrisID = new SqlParameter("@CoursPrisID", SqlDbType.Int);
                paramCoursPrisID.Value = int.Parse(sCoursPrisID);
                SqlParameter paramCoursPrisID2 = new SqlParameter("@CoursPrisID", SqlDbType.Int);
                paramCoursPrisID2.Value = int.Parse(sCoursPrisID);

                SqlParameter paramNumeroCours = new SqlParameter("@NumeroCours", SqlDbType.Text);
                paramNumeroCours.Value = sNumeroCours;

                SqlParameter paramEffaceParUsername = new SqlParameter("@EffaceParUsername", SqlDbType.VarChar);
                paramEffaceParUsername.Value = db.GetWindowsUser();

                SqlParameter paramPersonneID = new SqlParameter("@PersonneID", SqlDbType.Text);
                paramPersonneID.Value = sPersonneID;

                SqlParameter paramCoursOffertID = new SqlParameter("@CoursOffertID", SqlDbType.Int);
                paramCoursOffertID.Value = sCoursOffertID;

                SqlTransaction transaction = null;

                SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
                SqlCommand cmdInsert = new SqlCommand();
                SqlCommand cmdFactureNegative = new SqlCommand();
                SqlCommand cmdDelete = new SqlCommand();

                myConnection.Open();
                transaction = myConnection.BeginTransaction();  // Pas de sauvegarder si l'une des transactions n'est pas à succès

                //Insert 
                cmdInsert.CommandText = sSqlInsert;
                cmdInsert.Parameters.Add(paramCoursPrisID);
                cmdInsert.Parameters.Add(paramNumeroCours);
                cmdInsert.Parameters.Add(paramEffaceParUsername);
                cmdInsert.Parameters.Add(paramPersonneID);
                cmdInsert.Parameters.Add(paramCoursOffertID);
                cmdInsert.Connection = myConnection;
                cmdInsert.Transaction = transaction;

                //Additionner facture négative
                cmdFactureNegative.CommandText = sSqlFactureNegative;
                cmdFactureNegative.Connection = myConnection;
                cmdFactureNegative.Transaction = transaction;

                //Delete 
                cmdDelete.CommandText = sSqlDelete;
                cmdDelete.Parameters.Add(paramCoursPrisID2);
                cmdDelete.Connection = myConnection;
                cmdDelete.Transaction = transaction;

                try
                {
                    cmdInsert.ExecuteNonQuery();
                    cmdDelete.ExecuteNonQuery();
                    if (iNombreCours <= iMaxClasses)
                    {
                        cmdFactureNegative.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Debug.WriteLine(ex.Message);
                }
                myConnection.Close();
                myConnection = null;
            }
            catch (Exception Excep)
            {
                Debug.WriteLine(Excep.Message);
            }
        }
        db = null;
        Response.Redirect(String.Format("InfoEtudiant.aspx?personneid={0}", sPersonneID));
    }
}
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

public partial class Enregistrement : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            RemplirGridEtudiants();
            RemplirGridCours();
        }
    }

    void RemplirGridEtudiants()
    {
        lblError.Text = String.Empty;

        try
        {
            SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
            String sSql = @"SELECT * FROM Personnes ORDER BY Nom, Prenom";

            SqlDataAdapter da = new SqlDataAdapter(sSql, myConnection);
            DataTable dTable = new DataTable();
            da.Fill(dTable);

            gvEtudiants.DataSource = dTable;
            gvEtudiants.DataBind();
            myConnection.Close();
            myConnection = null;
        }
        catch (Exception ex)
        {
            lblError.Text = "ERREUR: " + ex.Message;
        }
    }

    void RemplirGridCours()
    {
        try
        {
            SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
            String sSql = @"SELECT CoursOfferts.CoursOffertsID, Convert(nvarchar, SessionDateDebut) + '/' + Convert(nvarchar, SessionDateFin) + '(' + SessionDescription + ')' AS Periode, " + 
                " Cours.NumeroCours, CoursPreRequis  " + 
                " FROM LesSessions, CoursOfferts, Cours  " +
	            " WHERE LesSessions.SessionID = CoursOfferts.SessionID " +
	            " AND CoursOfferts.NumeroCours = Cours.NumeroCours  " +
                " AND LesSessions.SessionCourante = 1";           

            SqlDataAdapter da = new SqlDataAdapter(sSql, myConnection);

            DataTable dTable = new DataTable();
            da.Fill(dTable);

            gvCours.DataSource = dTable;
            gvCours.DataBind();
            myConnection.Close();
            myConnection = null;
        }
        catch (Exception ex)
        {
            lblError.Text += " ---- ERREUR: " + ex.Message;
        }

    }

}
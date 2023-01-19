using System;
using System.Collections;

using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

public partial class ListeCoursCourants : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try { 
        string sSql = "SELECT CoursOffertID, CO.NumeroCours, PF.ProfesseurID, PF.Nom + ', ' + PF.Prenom AS Nom, " +
                " H.HoraireID, H.Jours + ' (' + H.HeureDebut + ' - ' + H.HeureFin + ')' AS Horaire, C.NomCours, C.DescriptionCours, C.CoursPreRequis " + 
                " FROM CoursOfferts CO, Cours C, Professeurs PF, Horaires H " +
                " WHERE CO.NumeroCours = C.NumeroCours  AND PF.ProfesseurID = CO.ProfesseurID AND H.HoraireID = CO.HoraireID" +
                " AND SessionID IN (SELECT SessionID FROM LesSessions WHERE SessionCourante = 1) " +
                " AND C.ExamenEntree = 0";

            ContentLiteralID.Text = ProcessInfo(sSql);
    }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
    }

    private string ProcessInfo(string sSql)
    {
        string returnedString = "";
        Hashtable htPreRequis = new Hashtable();
        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn1 = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            try
            {
                sqlConn1.Open();
                SqlDataReader dt = db.GetDataReader("SELECT NumeroCours, NomCours FROM Cours", sqlConn1);
                if (dt != null)
                {
                    while (dt.Read())
                    {
                        if (!htPreRequis.ContainsKey(dt["NumeroCours"].ToString()))
                        {
                            htPreRequis.Add(dt["NumeroCours"].ToString(), dt["NomCours"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            try
            {
                sqlConn.Open();
                string pre_requis = "";
                SqlDataReader dt = db.GetDataReader(sSql, sqlConn);
                returnedString += "<div><h1>Liste des Cours Offerts - Session Courante</h1></div>";
                returnedString += "<table id = 'CoursCourants' class='table table-striped table-bordered table-hover' style='width:100%'>" +
                "<thead><tr><th># Cours</th><th>Nom Cours</th><th>Nom Professeur</th><th>Horaire</th><th>Cours Pré-Requis</th></tr></thead><tbody>";
                if (dt != null)
                {
                    while (dt.Read())
                    {
                        pre_requis = htPreRequis[dt["CoursPreRequis"].ToString()].ToString();
                        pre_requis = string.Format("{0} ({1})", pre_requis, dt["CoursPreRequis"].ToString());
                        returnedString += string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td>",
                            dt["NumeroCours"].ToString(),
                            dt["NomCours"].ToString(),
                            dt["Nom"].ToString(),
                            dt["Horaire"].ToString(),
                            pre_requis
                            );
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        returnedString += "</tbody></table >";
        return returnedString;
    }
}
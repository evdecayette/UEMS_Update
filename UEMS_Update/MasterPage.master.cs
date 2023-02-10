using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class MasterPage : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string ConnectionString = XCryptEngine.ConnectionStringEncryption.Decrypt(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
        DB_Access db = new DB_Access();
        using (SqlConnection con = new SqlConnection(ConnectionString))
        {
            try
            {
                con.Open();
                String sql = ("Select DisciplineID,DisciplineNom From Disciplines");
                SqlDataReader dr = db.GetDataReader(sql, con);
                if (dr.Read())
                {
                    do
                    {
                        HtmlGenericControl li = new HtmlGenericControl("li");
                        lsCusus.Controls.Add(li);                        
                        String a = String.Format("<a href='RequiredClassPerDiscipline.aspx?disciplineId={0}&NomCursus={1}'>",dr["DisciplineID"], dr["DisciplineNom"]) + String.Format("{0}", dr["DisciplineNom"].ToString()) + "</a>";                       
                        li.InnerHtml = a;
                    }
                    while (dr.Read());
                }
      
            }
            catch (Exception ex)
            {

            }
        }
    }
}

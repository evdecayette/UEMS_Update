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

public partial class EffacerMontantRecu : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        String sPersonneID = "", sMontantRecuID = "";
        // Get PersonneID 
        try
        {
            sPersonneID = Request.QueryString["PersonneID"];
            Request.QueryString["PersonneID"].Remove(0);
            sMontantRecuID = Request.QueryString["MontantRecuID"];
            Request.QueryString["MontantRecuID"].Remove(0);

            SqlConnection sqlConn = null;
            SqlCommand SqlCmd;

            sqlConn = new System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString());
            sqlConn.Open();

            SqlCmd = new SqlCommand();
            SqlCmd.Connection = sqlConn;
            SqlCmd.CommandText = String.Format("DELETE MontantsRecus WHERE MontantRecuID = '{0}' AND PersonneID = '{1}'", sMontantRecuID, sPersonneID);
            SqlCmd.ExecuteNonQuery();
            sqlConn.Close();
            sqlConn = null;

        }
        catch (Exception Excep)
        {
            Debug.WriteLine(Excep.Message);
        }
        Response.Redirect("RecevoirPaiements.aspx?PersonneID=" + sPersonneID);
    }
}
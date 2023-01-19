using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;

public partial class Testing : System.Web.UI.Page
{
    protected static bool inProcess = false;

    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void btnSauvegarder_Click(object sender, EventArgs e)
    {
        btnSauvegarder.Text = "";
        //btnSauvegarder.Enabled = false;
        btnSauvegarder.CssClass = "button-wait";

        Timer1.Enabled = true;
        Thread workerThread = new Thread(new ThreadStart(ProcessRecords));
        workerThread.Start();
    }

    protected void ProcessRecords()
    {
        inProcess = true;
        Thread.Sleep(10000);
        inProcess = false;
    }

    protected void Timer1_Tick(object sender, EventArgs e)
    {
        if (!inProcess)
        {
            btnSauvegarder.CssClass = "btn btn-outline btn-primary";
            btnSauvegarder.Text = "Sauvegarder";
        }
    }
}
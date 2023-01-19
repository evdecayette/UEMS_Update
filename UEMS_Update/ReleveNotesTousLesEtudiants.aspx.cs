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

public partial class ReleveNotesTousLesEtudiants : System.Web.UI.Page
{
    String sSql = "";
    HashSet<Notation> getNotesScheme = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //sSql = String.Format("SELECT CP.PersonneID, CP.NumeroCours, IsNull(CP.DateReussite, '9999-01-01') DateReussite, CP.NoteSurCent, " +
            //    " CP.NotePassage, C.NomCours, C.Credits, P.Nom, P.Prenom, P.EtudiantID  " +
            // " FROM CoursPris CP, Cours C, Personnes P  " +
            //    " WHERE CP.NumeroCours = C.NumeroCours AND CP.PersonneID = P.PersonneID  " +
            //  " AND C.ExamenEntree = 0 AND P.Actif = 1 " +
            //    " ORDER BY P.Nom, P.Prenom, DateReussite DESC");

            //sSql = "SELECT DISTINCT P.PersonneID, P.EtudiantIdPlus, IsNull(CP.DateReussite, '9999-01-01') DateReussite, " +
            //    " CP.NoteSurCent,  CP.NotePassage, C.NomCours, C.NumeroCours, C.Credits, P.Nom, P.Prenom, P.EtudiantID " +
            //    " FROM Personnes, CoursPris CP, Cours C, Personnes P " +
            //    " WHERE CP.NumeroCours = C.NumeroCours AND CP.PersonneID = P.PersonneID AND C.ExamenEntree = 0 " +
            //    " AND P.Actif = 1 AND P.PersonneID IN (SELECT PersonneID FROM Personnes " +
            //    " WHERE Actif = 1 AND PersonneID IN (SELECT DISTINCT PersonneID FROM CoursPris " +
            //    " WHERE IsNull(DateReussite, '') = '' AND CoursOffertID IN(SELECT CoursOffertID " +
            //    " FROM[UEspoirDB].[dbo].[CoursOfferts] WHERE Actif = 1)) ) " +
            //    " ORDER BY P.Nom, P.Prenom, DateReussite DESC";
            sSql = "SELECT DISTINCT P.PersonneID, P.EtudiantIdPlus, IsNull(CP.DateReussite, '9999-01-01') DateReussite, " +
                " CP.NoteSurCent,  CP.NotePassage, C.NomCours, C.NumeroCours, C.Credits, P.Nom, P.Prenom, P.EtudiantID " +
                " FROM Personnes, CoursPris CP, Cours C, Personnes P " +
                " WHERE CP.NumeroCours = C.NumeroCours AND CP.PersonneID = P.PersonneID AND C.ExamenEntree = 0 " +
                " AND P.Actif = 1 AND P.PersonneID IN (SELECT PersonneID FROM Personnes " +
                " WHERE Actif = 1 AND PersonneID IN (SELECT DISTINCT PersonneID FROM CoursPris " +
                " WHERE CoursOffertID IN(SELECT CoursOffertID " +
                " FROM[UEspoirDB].[dbo].[CoursOfferts] WHERE Actif = 1)) ) " +
                " ORDER BY P.Nom, P.Prenom, DateReussite DESC";
            litBody.Text = ProcessInfo(sSql);
        }
    }

    String ProcessInfo(String sSql)
    {
        String sRetString = String.Format("<div style=\'page-break-after:always;\'></div>");    // Start with page break in order not to print the button 'print'
        float noteMoyenne = 0.0f, gpa = 0.0f;
        int creditsTotal = 0;
        int iYear = 9999, iNbreDeNotes = 0;
        string credits;

        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            try
            {
                using (SqlConnection sqlConn1 = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
                {
                    try
                    {
                        sqlConn1.Open();
                        getNotesScheme = db.FillNotations(sqlConn1);                    // Get the different notations
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }

                sqlConn.Open();
                SqlDataReader dtTemp = db.GetDataReader(sSql, sqlConn);
                String DateNoteObtenue = "", DateNoteObtenueOld = "", lettreNote = "";
                String Etudiant = "", EtudiantPrecedent = "";
                Boolean firstPassEtudiant = true;
                Boolean firstPassNote = true;
                float NoteSurCent = 0, NotePassage = 0;

                if (dtTemp.Read())
                    do
                    {
                        Etudiant = dtTemp["PersonneID"].ToString();
                        if (EtudiantPrecedent != Etudiant)
                        {
                            EtudiantPrecedent = Etudiant;
                            if (!firstPassEtudiant)
                            {
                                // Close table
                                if (iNbreDeNotes > 0)
                                    gpa = gpa / iNbreDeNotes;
                                else
                                    gpa = 0.0f;
                                sRetString += String.Format("<TR><TD Colspan='7' width:'80%'><hr style='background-color:#669999;' size='2' width='100%'/></TD></TR>");
                                sRetString += String.Format("<TR><TD Colspan='4' style='width:80%;text-align:left;font-weight:bold;font-size:14px'>Moyenne Générale (GPA) : {0}</TD>", gpa.ToString("F"));
                                sRetString += String.Format("<TD Colspan='2' style='width:80%;text-align:right;font-weight:bold;font-size:14px'>Nombre de Crédits :</TD>");
                                sRetString += String.Format("<TD style='width:80%;text-align:center;font-weight:bold;font-size:14px'>{0}</TD></TR>", creditsTotal);
                                sRetString += String.Format("<TD Colspan='7' width:'80%'><hr style='background-color:#669999;' size='2' width='100%'/></TD></TR>");

                                sRetString += "</TABLE>";
                                sRetString += String.Format("<div style=\'page-break-after:always;\'></div>");      // page break
                                noteMoyenne = 0.0f;
                                gpa = 0.0f;
                                creditsTotal = 0;
                                iNbreDeNotes = 0;
                            }
                            else
                            {
                                firstPassEtudiant = false;
                            }
                            // start new table
                            sRetString += String.Format("<TABLE style='width:80%;align:center'>");
                            sRetString += String.Format("<TR><TD Colspan='7' style='width:80%;text-align:center;font-weight:bold;font-size:14px'>Université Espoir</TD></TR>");
                            sRetString += String.Format("<TR><TD Colspan='7' style='width:80%;text-align:center;font-weight:bold;font-size:14px'>Historique des Cours<div style='font-color:red'>{0}({1})</div></TD></TR>",
                                dtTemp["Prenom"].ToString() + " " + dtTemp["Nom"].ToString().ToUpper() + " ", dtTemp["EtudiantIdPlus"].ToString());
                            sRetString += String.Format("<TR><TD Colspan='7' style='width:80%;text-align:center;font-weight:bold;font-size:14px'>Date d'Impression: {0}</TD></TR>", DateTime.Today.Date.ToString("dd-MMM-yyyy"));
                            sRetString += String.Format("<TR><TD Colspan='7' width:'80%'><hr style='background-color:#669999;' size='3'/></TD></TR>");
                            sRetString += String.Format("<TR><TD width:'40%' style='text-align:left;font-weight:bold;font-size:14px'>Nom du Cours</TD>" +
                                "<TD></TD>" +
                                "<TD style='text-align:center;font-weight:bold;font-size:14px'>Cours</TD>" +
                                "<TD style='text-align:center;font-weight:bold;font-size:14px'>Note Obtenue</TD>" +
                                "<TD style='text-align:center;font-weight:bold;font-size:14px'>Lettre Equivalente</TD>" +
                                "<TD style='text-align:center;font-weight:bold;font-size:14px'>Moyenne (GPA)</TD>" +
                                "<TD width:'12%' style='text-align:center;font-weight:bold;font-size:14px'>Crédits</TD></TR>");
                            sRetString += String.Format("<TR><TD Colspan='7' width:'80%'><hr style='background-color:#669999;' size='3'/></TD></TR>");
                        }

                        DateNoteObtenue = Utils.DateFormattee(dtTemp["DateReussite"].ToString(), out iYear);
                        if (iYear == 9999)
                            continue;
                        if (DateNoteObtenue != DateNoteObtenueOld)
                        {
                            if (!firstPassNote)
                            {
                                sRetString += String.Format("<TR><TD Colspan='7' style='width:80%;text-align:left;font-weight:bold;font-size:14px'></TD></TR>");
                            }
                            sRetString += String.Format("<TR><TD Colspan='7' style='width:80%;text-align:left;font-weight:bold;font-size:14px'>{0}</TD></TR>", DateNoteObtenue);
                            DateNoteObtenueOld = DateNoteObtenue;

                            firstPassNote = false;
                        }
                        lettreNote = Utils.ObtenirLettre((int)float.Parse(dtTemp["NoteSurCent"].ToString()), getNotesScheme, out noteMoyenne);
                        NoteSurCent = float.Parse(dtTemp["NoteSurCent"].ToString()) ;
                        NotePassage = float.Parse(dtTemp["NotePassage"].ToString());
                        if (NoteSurCent >= NotePassage)
                            credits = dtTemp["Credits"].ToString();
                        else
                        {
                            credits = "0";
                        }
                        sRetString += String.Format("<TR><TD>&nbsp;&nbsp;&nbsp;&nbsp;{0}</TD><TD></TD><TD style='text-align:center;'>{1}</TD>" +
                        "<TD style='text-align:center;'>{2}</TD>" +
                        "<TD style='text-align:center;'>{3}</TD>" +
                        "<TD style='text-align:center;'>{4}</TD>" +
                        "<TD style='text-align:center;'>{5}</TD></TR>",
                          dtTemp["NomCours"].ToString(),
                          dtTemp["NumeroCours"].ToString(),
                          float.Parse(dtTemp["NoteSurCent"].ToString()).ToString("F"),
                          lettreNote, noteMoyenne.ToString("F"), credits);

                        if (noteMoyenne >= 0 && (NoteSurCent >= NotePassage))
                        {
                            gpa += noteMoyenne;
                            creditsTotal += int.Parse(credits);
                            iNbreDeNotes++;
                        }
                    }
                    while (dtTemp.Read());

                db = null;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                sRetString += "<br> ERREUR - ERREUR - ERREUR !!!";

            }

            if (iNbreDeNotes > 0)
                gpa = gpa / iNbreDeNotes;
            else
                gpa = 0.0f;
            //sRetString += String.Format("<TR><TD Colspan='7' width:'80%'><hr style='background-color:#669999;' size='2' width='100%'/></TD></TR>");
            //sRetString += String.Format("<TR><TD Colspan='7' style='width:80%;text-align:left;font-weight:bold;font-size:14px'>Moyenne Générale (GPA) : {0}</TD></TR>", gpa.ToString("F"));
            //sRetString += String.Format("<TR><TD Colspan='7' width:'80%'><hr style='background-color:#669999;' size='2' width='100%'/></TD></TR>");
        }
        sRetString += String.Format("<TR><TD Colspan='7' width:'80%'><hr style='background-color:#669999;' size='2' width='100%'/></TD></TR>");
        sRetString += String.Format("<TR><TD Colspan='4' style='width:80%;text-align:left;font-weight:bold;font-size:14px'>Moyenne Générale (GPA) : {0}</TD>", gpa.ToString("F"));
        sRetString += String.Format("<TD Colspan='2' style='width:80%;text-align:right;font-weight:bold;font-size:14px'>Nombre de Crédits :</TD>");
        sRetString += String.Format("<TD style='width:80%;text-align:center;font-weight:bold;font-size:14px'>{0}</TD></TR>", creditsTotal);
        sRetString += String.Format("<TD Colspan='7' width:'80%'><hr style='background-color:#669999;' size='2' width='100%'/></TD></TR>");
        db = null;
        sRetString += "</TABLE>";
        return sRetString;
    }

}

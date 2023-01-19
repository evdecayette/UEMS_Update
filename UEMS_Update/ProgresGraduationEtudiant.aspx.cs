using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;

public partial class ProgresGraduationEtudiant : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            String sPersonneID = Request.QueryString["PersonneID"];
            //txtPersonneID.Text = sPersonneID;
            String sSql = String.Format("SELECT CP.NumeroCours, IsNull(CP.DateReussite, '9999-01-01') DateReussite, CP.NoteSurCent, " +
                " CP.NotePassage, C.NomCours, C.Credits, P.EtudiantIdPlus, P.Nom, P.Prenom, P.DisciplineID FROM CoursPris CP, Cours C, Personnes P " +
                " WHERE CP.PersonneID = '{0}' AND CP.NumeroCours = C.NumeroCours AND CP.PersonneID = P.PersonneID  AND C.ExamenEntree = 0 " +
                " AND C.Credits > 0 ORDER BY DateReussite DESC", sPersonneID);

            ContentLiteralID.Text = ComparerReleveAvecProgramme(sSql, sPersonneID);
        }

    }

    private String ComparerReleveAvecProgramme(String sSql, String sPersonneID)
    {
        String returnedString = String.Empty;

        // Header
        returnedString += @"<div class='row'><div class='col-lg-12'><h1 class='page-header'>Progrès Vers Graduation</h1></div></div>";
        //returnedString += @"<div class='row'><div class='col-lg-12'><div class='panel panel-default'><div class='panel-heading'>Sélectionnez Une Discipline Pour Voir le Cursus</div>";
        returnedString += @"<div class='panel-body'>";
        returnedString += @"<table width='100%' class='table table-striped table-bordered table-hover' id='dataTables-Etudiants'>";
        returnedString += @"<thead><tr><th colspan=2><h4>Programme</h4></th><th colspan=4><h4>Cours Pris</h4></th></thead>";
        returnedString += @"<tr><th>Description</th><th>Numero</th><th>Note</th><th>Note de Passage</th><th>Session</th>";
        returnedString += @"<th>Remarque</th><tbody>";
        
        Dictionary<string, ReleveNotes> dictionaryNotes = new Dictionary<string, ReleveNotes>();

        string sDisciplineDeclaree;
        int iDisciplineID = 0;

        int iYear;

        // Get Discipline de l'étudiant
        using (SqlConnection sqlConn0 = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            try
            {
                DB_Access db0 = new DB_Access();
                sqlConn0.Open();
                sDisciplineDeclaree = db0.GetDisciplineEtudiant(sPersonneID, sqlConn0);
            }
            catch
            {
                returnedString += "<br> ERREUR 0 : ProcessInfo!";
                returnedString += @"</tbody></table>";
                returnedString += @"</div>";

                return returnedString;
            }
        }

        //List<string> keys = new List<string>(dictionaryNotes.Keys);

        // Get Cours Pris par l'étudiant: Relevé de Notes
        using (SqlConnection sqlConn1 = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            try
            {
                DB_Access db1 = new DB_Access();
                sqlConn1.Open();
                SqlDataReader dtTemp = db1.GetDataReader(sSql, sqlConn1);
                String PeriodeNoteObtenue = "", lettreNote = "";

                if (dtTemp.Read())
                {
                    do
                    {
                        PeriodeNoteObtenue = Utils.DateFormattee(dtTemp["DateReussite"].ToString(), out iYear);
                        if (iYear == 9999)
                            continue;

                        string numeroCours = dtTemp["NumeroCours"].ToString();
                        float noteSurCent = float.Parse(dtTemp["NoteSurCent"].ToString());
                        float notePassage = float.Parse(dtTemp["NotePassage"].ToString());
                        if (iDisciplineID == 0)
                            iDisciplineID = int.Parse(dtTemp["DisciplineID"].ToString());

                        ReleveNotes releveNote = new ReleveNotes(sPersonneID, dtTemp["Nom"].ToString().ToUpper(), dtTemp["Prenom"].ToString(), dtTemp["EtudiantIdPlus"].ToString(),
                            dtTemp["DateReussite"].ToString(), dtTemp["NomCours"].ToString(), dtTemp["NumeroCours"].ToString(), noteSurCent.ToString("F"),
                            notePassage.ToString("F"), "0", lettreNote, PeriodeNoteObtenue, "");

                        if (!dictionaryNotes.ContainsKey(numeroCours))
                        {
                            dictionaryNotes.Add(numeroCours, releveNote);
                        }
                        else
                        {
                            ReleveNotes temp = dictionaryNotes[numeroCours];
                            // Utiliser la note la plus grande
                            if (noteSurCent > float.Parse(temp.NoteSurCent))
                            {
                                // Nouvelle note est supérieure, alors remplacez
                                dictionaryNotes.Remove(numeroCours);
                                dictionaryNotes.Add(numeroCours, releveNote);
                            }
                            noteSurCent.ToString("F");
                        }
                    }
                    while (dtTemp.Read());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                returnedString += "<br> ERREUR 1 : ProcessInfo!";
                //db = null;
            }
        }

        // Get liste de Cours Programme/Discipline
        using (SqlConnection sqlConn2 = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ToString()))
        {
            try
            {
                DB_Access db2 = new DB_Access();
                sqlConn2.Open();
                sSql = String.Format("SELECT CursusID, NumeroCours, NumeroSession, Actif, DisciplineID, Remarque, Obligatoire " +
                    " FROM Cursus WHERE DisciplineID = {0}", iDisciplineID);
                SqlDataReader dtTemp = db2.GetDataReader(sSql, sqlConn2);
                bool ok = false;

                if (dtTemp.Read())
                {
                    do
                    {
                        ReleveNotes releveNote = dictionaryNotes[dtTemp["NumeroCours"].ToString()];
                        if (releveNote == null)
                        {
                            returnedString += String.Format("<TR><TD>{0}</TD><TD>{1}</TD><TD></TD><TD></TD><TD></TD><TD></TD></TR>",
                                releveNote.NomCours, releveNote.NumeroCours);
                        }
                        else
                        {
                            ok = float.Parse(releveNote.NoteSurCent) >= float.Parse(releveNote.NotePassage);
                            returnedString += String.Format("<TR><TD>{0}</TD><TD>{1}</TD><TD>{2}</TD><TD>{3}</TD><TD>{4}</TD><TD>{5}</TD></TR>",
                                releveNote.NomCours, releveNote.NumeroCours,
                                releveNote.NoteSurCent, releveNote.NotePassage, releveNote.PeriodeNoteObtenue,
                                ok ? "OK" : "");
                        }
                    }
                    while (true);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                returnedString += "<br> ERREUR 2 : ProcessInfo!";
                //db = null;
            }
        }

        returnedString += @"</tbody></table>";
        returnedString += @"</div>";
        //returnedString += @"</div></div></div>";

        return returnedString;

    }
}
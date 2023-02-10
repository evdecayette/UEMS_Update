using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
//using ClosedXML.Excel;

public partial class ReleveNotesEtudiant : System.Web.UI.Page
{
    String sPersonneID = "";
    String sSql = "";
    HashSet<Notation> getNotesScheme = null;
    string ConnectionString = XCryptEngine.ConnectionStringEncryption.Decrypt(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            sPersonneID = Request.QueryString["PersonneID"];
            txtPersonneID.Text = sPersonneID;
            sSql = String.Format("SELECT CP.NumeroCours, IsNull(CP.DateReussite, '9999-01-01') DateReussite, CP.NoteSurCent, " +
                " CP.NotePassage, C.NomCours, C.Credits, P.EtudiantIdPlus, P.Nom, P.Prenom FROM CoursPris CP, Cours C, Personnes P " +
                " WHERE CP.PersonneID = '{0}' AND CP.NumeroCours = C.NumeroCours AND CP.PersonneID = P.PersonneID  AND C.ExamenEntree = 0 " + 
                " AND C.Credits > 0 ORDER BY DateReussite DESC", sPersonneID);

            litBody.Text = ProcessInfo(sSql);            
        }
    }

    String ProcessInfo(String sSql)
    {
        String sRetString = String.Format("<div style=\'page-break-after:always;\'></div>");    // Start with page break in order not to print the button 'print'
        float noteMoyenne = 0.0f, gpa = 0.0f;
        int iYear = 9999, iNbreDeNotes = 0;
        int creditsTotal = 0;
        string credits, sPersonneID;

        Dictionary<string, ReleveNotes> dictionaryNotes = new Dictionary<string, ReleveNotes>();
        sPersonneID = txtPersonneID.Text.ToString();

        using (SqlConnection sqlConn0 = new SqlConnection(ConnectionString))
        {
            DB_Access db0 = new DB_Access();
            try
            {
                sqlConn0.Open();
                if (sPersonneID.Trim() == String.Empty)
                {
                    return "Erreur : Username !!!";
                }
                getNotesScheme = db0.FillNotations(sqlConn0);                    // Get the different notations
                sqlConn0.Close();
            }
            catch
            {
                sRetString += "<br> ERREUR 0 : ProcessInfo!";
                return sRetString;
            }
        }

        string sDisciplineDeclaree;
        using (SqlConnection sqlConn1 = new SqlConnection(ConnectionString))
        {
            try
            {
                DB_Access db1 = new DB_Access();
                sqlConn1.Open();
                sDisciplineDeclaree = db1.GetDisciplineEtudiant(sPersonneID, sqlConn1);
            }
            catch
            {
                sRetString += "<br> ERREUR 0 : ProcessInfo!";
                return sRetString;
            }
        }

        DB_Access db = new DB_Access();
        using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
        {
            try
            {
                sqlConn.Open();

                SqlDataReader dtTemp = db.GetDataReader(sSql, sqlConn);
                String PeriodeNoteObtenue = "", PeriodeNoteObtenueOld = "", lettreNote = "";

                if (dtTemp.Read())
                {
                    do
                    {
                        PeriodeNoteObtenue = Utils.DateFormattee(dtTemp["DateReussite"].ToString(), out iYear);
                        if (iYear == 9999)
                            continue;

                        lettreNote = Utils.ObtenirLettre((int)float.Parse(dtTemp["NoteSurCent"].ToString()), getNotesScheme, out noteMoyenne);
                        if (lettreNote.ToUpper() != "I" && lettreNote.ToUpper() != "E" && lettreNote.ToUpper() != "F")
                            credits = dtTemp["credits"].ToString();
                        else
                        {
                            credits = "0";
                            //continue;
                        }
                        string NomCours = dtTemp["NomCours"].ToString();
                        float noteSurCent = float.Parse(dtTemp["NoteSurCent"].ToString());
                        float notePassage = float.Parse(dtTemp["NotePassage"].ToString());


                        ReleveNotes releveNote = new ReleveNotes(sPersonneID, dtTemp["Nom"].ToString().ToUpper(), dtTemp["Prenom"].ToString(), dtTemp["EtudiantIdPlus"].ToString(),
                            dtTemp["DateReussite"].ToString(), NomCours, dtTemp["NumeroCours"].ToString(), noteSurCent.ToString("F"),
                            notePassage.ToString("F"), credits, lettreNote, PeriodeNoteObtenue, noteMoyenne.ToString("F"));

                        if (!dictionaryNotes.ContainsKey(NomCours))
                        {
                            dictionaryNotes.Add(dtTemp["NomCours"].ToString(), releveNote);
                        }
                        else
                        {
                            ReleveNotes temp = dictionaryNotes[NomCours];
                            // Utiliser la note la plus grande
                            if (noteSurCent > float.Parse(temp.NoteSurCent))
                            {
                                // Nouvelle note est supérieure, alors remplacez
                                dictionaryNotes.Remove(NomCours);
                                dictionaryNotes.Add(dtTemp["NomCours"].ToString(), releveNote);
                            }
                            noteSurCent.ToString("F");
                        }
                    }
                    while (dtTemp.Read());

                    List<string> keys = new List<string>(dictionaryNotes.Keys);
                    bool needHeader = true;

                    for (int i = 0; i < keys.Count; i++)
                    {
                        ReleveNotes releveNote = dictionaryNotes[keys[i]];

                        if (needHeader)
                        {
                            sRetString += String.Format("<TABLE style='width:80%;align:center'>");
                            sRetString += String.Format("<TR><TD Colspan='7' style='width:80%;text-align:center;font-weight:bold;font-size:14px'>Université Espoir</TD></TR>");
                            sRetString += String.Format("<TR><TD Colspan='7' style='width:80%;text-align:center;font-weight:bold;font-size:14px'>Historique des Cours<div style='font-color:red'>{0}({1})</div></TD></TR>",
                                releveNote.Prenom + " " + releveNote.Nom.ToUpper() + " ", releveNote.EtudiantIdPlus);
                            sRetString += String.Format("<TR><TD Colspan='7' style='width:80%;text-align:center;font-weight:bold;font-size:14px'>Discipline Déclarée : {0}</TD></TR>",
                                sDisciplineDeclaree);
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
                            needHeader = false;
                        }
                        Boolean firstPass = true;

                        PeriodeNoteObtenue = releveNote.PeriodeNoteObtenue;
                        if (PeriodeNoteObtenue != PeriodeNoteObtenueOld)
                        {
                            if (!firstPass)
                            {
                                sRetString += String.Format("<TR><TD Colspan='7' style='width:80%;text-align:left;font-weight:bold;font-size:14px'></TD></TR>");
                            }
                            sRetString += String.Format("<TR><TD Colspan='7' style='width:80%;text-align:left;font-weight:bold;font-size:14px'>{0}</TD></TR>", PeriodeNoteObtenue);
                            PeriodeNoteObtenueOld = PeriodeNoteObtenue;

                            firstPass = false;
                        }
                        lettreNote = Utils.ObtenirLettre((int)float.Parse(releveNote.NoteSurCent), getNotesScheme, out noteMoyenne);
                        //if (lettreNote.ToUpper() != "I" && lettreNote.ToUpper() != "E" && lettreNote.ToUpper() != "F")
                        //    credits = releveNote.Credits;
                        //else
                        //    credits = "0";
                        if (float.Parse(releveNote.NoteSurCent) >= float.Parse(releveNote.NotePassage))
                            credits = releveNote.Credits;
                        else
                        {
                            credits = "0";                            
                        }
                        sRetString += String.Format("<TR><TD>&nbsp;&nbsp;&nbsp;&nbsp;{0}</TD><TD></TD><TD style='text-align:center;'>{1}</TD>" +
                        "<TD style='text-align:center;'>{2}</TD>" +
                        "<TD style='text-align:center;'>{3}</TD>" +
                        "<TD style='text-align:center;'>{4}</TD>" +
                        "<TD style='text-align:center;'>{5}</TD></TR>",
                          releveNote.NomCours,
                          releveNote.NumeroCours,
                          releveNote.NoteSurCent,
                          lettreNote,
                          noteMoyenne.ToString("F"),
                          credits);

                        if (noteMoyenne > 0)
                        {
                            gpa += noteMoyenne;
                            creditsTotal += int.Parse(credits);
                            iNbreDeNotes++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                sRetString += "<br> ERREUR 2 : ProcessInfo!";
                //db = null;
            }
        }
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
        return sRetString;
    }

    //public static void CreateWordprocessingDocument(string fileName)
    //{

    //    string[,] data = {
    //        {"Texas", "TX"},
    //        {"California", "CA"},
    //        {"New York", "NY"},
    //        {"Massachusetts", "MA"}
    //    };

    //    using (WordprocessingDocument myDoc = WordprocessingDocument.Create(fileName, WordprocessingDocumentType.Document))
    //    {
    //        // Add a new main document part. 
    //        MainDocumentPart mainPart = myDoc.AddMainDocumentPart();
    //        //Create Document tree for simple document. 
    //        mainPart.Document = new Document();
    //        //Create Body (this element contains other elements that we want to include
    //        Body body = new Body();

    //        DocumentFormat.OpenXml.Wordprocessing.Table table = new DocumentFormat.OpenXml.Wordprocessing.Table();

    //        TableProperties props = new TableProperties();
    //        DocumentFormat.OpenXml.Wordprocessing.TableStyle tableStyle = new DocumentFormat.OpenXml.Wordprocessing.TableStyle { Val = "LightShading-Accent1" };
    //        props.TableStyle = tableStyle;
    //        //props.Append(tableStyle);
    //        table.AppendChild(props);

    //        for (var i = 0; i <= data.GetUpperBound(0); i++)
    //        {
    //            var tr = new DocumentFormat.OpenXml.Wordprocessing.TableRow();
    //            for (var j = 0; j <= data.GetUpperBound(1); j++)
    //            {
    //                var tc = new DocumentFormat.OpenXml.Wordprocessing.TableCell();
    //                tc.Append(new Paragraph(new Run(new Text(data[i, j]))));
    //                tc.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
    //                tr.Append(tc);
    //            }
    //            table.Append(tr);
    //        }
    //        body.Append(table);
    //        // Save changes to the main document part. 
    //        mainPart.Document.Save();


    //        // let's open this document in Word
    //        Process compiler = new Process();
    //        compiler.StartInfo.FileName = fileName;
    //        //compiler.StartInfo.Arguments = "";
    //        compiler.StartInfo.UseShellExecute = true;
    //        compiler.Start();
    //    }
    //}

    //void ProcessInfoForWordOLD()
    //{
    //    String sRetString = String.Empty;
    //    float noteMoyenne = 0.0f, gpa = 0.0f;
    //    int iYear = 9999, iNbreDeNotes = 0;
    //    String nomEtudiant = "";


    //    //CreateWordprocessingDocument("c:\\extras\\TestingMore.docx" );
    //    //return;


    //    // Tble tuff
    //    String sTableTitle = String.Empty;

    //    DB_Access db = new DB_Access(ConnectionString);
    //    try
    //    {
    //        SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
    //        sPersonneID = txtPersonneID.Text;
    //        if (sPersonneID.Trim() == String.Empty)
    //        {
    //            litBody.Text = "Erreur !!!";
    //            return;
    //        }
    //        getNotesScheme = db.FillNotations();                    // Get the different notations

    //        String sSql = String.Format("SELECT CP.NumeroCours, IsNull(CP.DateReussite, '9999-01-01') DateReussite, CP.NoteSurCent, " +
    //            " CP.NotePassage, C.NomCours, C.Credits, P.Nom, P.Prenom FROM CoursPris CP, Cours C, Personnes P " +
    //            " WHERE CP.PersonneID = '{0}' AND CP.NumeroCours = C.NumeroCours AND CP.PersonneID = P.PersonneID  AND C.ExamenEntree = 0 ORDER BY DateReussite DESC", sPersonneID);

    //        SqlDataReader dtTemp = db.GetDataReader(sSql);
    //        String PeriodeNoteObtenue = "", PeriodeNoteObtenueOld = "", lettreNote = "";

    //        String fileName = "c:\\extras\\UEspoir\\Releves\\" + nomEtudiant.Replace(" ", "").Trim() + DateTime.Today.Day.ToString() + "-" + DateTime.Today.Month.ToString() + "-" +
    //            DateTime.Today.Year.ToString() + "-" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() +
    //            DateTime.Now.Second.ToString() + ".docx";

    //        //Other(fileName); return;

    //        using (WordprocessingDocument myDoc = WordprocessingDocument.Create(fileName, WordprocessingDocumentType.Document))
    //        {
    //            // Add a new main document part. 
    //            MainDocumentPart mainPart = myDoc.AddMainDocumentPart();
    //            //Create Document tree for simple document. 
    //            mainPart.Document = new Document();
    //            //Create Body (this element contains other elements that we want to include
    //            Body body = new Body();

    //            //Add new style part 
    //            StyleDefinitionsPart stylePart = mainPart.AddNewPart<StyleDefinitionsPart>();
    //            // we have to set the properties
    //            RunProperties rPr = new RunProperties();
    //            //Color color = new Color() { Val = "FF0000" };     // the color is red
    //            //RunFonts rFont = new RunFonts();
    //            //rFont.Ascii = "Arial";                            // the font is Arial
    //            rPr.Append(new Color() { Val = "FF0000" });         // the color is red
    //            //rPr.Append(rFont);
    //            rPr.Append(new Bold()); // it is Bold
    //            rPr.Append(new Underline(){ Val = DocumentFormat.OpenXml.Wordprocessing.UnderlineValues.Double });
    //            rPr.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "32" }); //font size (in 1/72  of an inch)                 

    //            //creation of a style
    //            DocumentFormat.OpenXml.Wordprocessing.Style style = new DocumentFormat.OpenXml.Wordprocessing.Style();
    //            style.StyleId = "Releve_Heading";                           //this is the ID of the style
    //            style.Append(new Name() { Val = "ReleveNotes" });           //this is name
    //            style.Append(new BasedOn() { Val = "Heading1" });           // our style based on Normal style
    //            style.Append(new NextParagraphStyle() { Val = "Normal" });  // the next paragraph is Normal type
    //            style.Append(rPr);                                          //we are adding properties previously defined
    //            // we have to add style that we have created to the StylePart
    //            stylePart.Styles = new Styles();
    //            stylePart.Styles.Append(style);
    //            stylePart.Styles.Save(); // we save the style part                

    //            //// Properties for date
    //            RunProperties rPr1 = new RunProperties();
    //            rPr1.Append(new Bold()); // it is Bold
    //            rPr1.Append(new Underline() { Val = DocumentFormat.OpenXml.Wordprocessing.UnderlineValues.Single });
    //            rPr1.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" }); //font size (in 1/72  of an inch) 
    //            //creation of a style
    //            DocumentFormat.OpenXml.Wordprocessing.Style style1 = new DocumentFormat.OpenXml.Wordprocessing.Style();
    //            style1.StyleId = "Date_Heading"; //this is the ID of the style
    //            style1.Append(new Name() { Val = "Date_Releve" }); //this is name
    //            style1.Append(new BasedOn() { Val = "Heading1" }); // our style based on Normal style
    //            style1.Append(new NextParagraphStyle() { Val = "Normal" }); // the next paragraph is Normal type
    //            style1.Append(rPr1);//we are adding properties previously defined
    //            //
    //            stylePart.Styles.Append(style1);
    //            stylePart.Styles.Save(); // we save the style part                

    //            Paragraph ReleveNotesParagraph = new Paragraph();
    //            Run ReleveNotes_run = new Run();
    //            Text ReleveNotes_text = new Text("Relevé de Notes");

    //            ParagraphProperties ReleveNotes_ParagraphProperties = new ParagraphProperties();
    //            ReleveNotes_ParagraphProperties.ParagraphStyleId = new ParagraphStyleId() { Val = "Releve_Heading" }; // we set the style
    //            ReleveNotes_ParagraphProperties.Append(new Justification() { Val = JustificationValues.Center });
    //            ReleveNotesParagraph.Append(ReleveNotes_ParagraphProperties);
    //            ReleveNotes_run.Append(ReleveNotes_text);
    //            ReleveNotesParagraph.Append(ReleveNotes_run);
    //            body.Append(ReleveNotesParagraph);                

    //            //Create paragraph for date of transcript
    //            Paragraph paragraphDate = new Paragraph();
    //            Run date_run = new Run();
    //            // we want to put that text into the output document
    //            Text text_paragraph = new Text("Date d’impression : " + DateTime.Today.ToString("dd-MMM-yyyy"));

    //            // Date                          
    //            ParagraphProperties Date_ParagraphProperties = new ParagraphProperties();
    //            Date_ParagraphProperties.ParagraphStyleId = new ParagraphStyleId() { Val = "Date_Heading" };
    //            Date_ParagraphProperties.Append(new Justification() { Val = JustificationValues.Center });
    //            paragraphDate.Append(Date_ParagraphProperties);

    //            ////Append elements appropriately.
    //            date_run.Append(text_paragraph);
    //            paragraphDate.Append(date_run);
               
    //            //====================================================================================================
    //            DocumentFormat.OpenXml.Wordprocessing.Table table = new DocumentFormat.OpenXml.Wordprocessing.Table();

    //            //define table properties
    //            //TableBorders tblBorders = new TableBorders();
    //            //tblBorders.TopBorder = new TopBorder();
    //            //tblBorders.InsideHorizontalBorder = new InsideHorizontalBorder();
    //            //tblBorders.InsideHorizontalBorder.Val = BorderValues.Single;
    //            //tblBorders.InsideVerticalBorder = new InsideVerticalBorder();
    //            //tblBorders.InsideVerticalBorder.Val = BorderValues.Single;
    //            //tblPr.Append(tblBorders);

    //            // we have to set the properties
    //            RunProperties rPrForTable = new RunProperties();
    //            RunFonts rFontForTable = new RunFonts();
    //            rFontForTable.Ascii = "Arial"; // the font is Arial
    //            rPrForTable.Append(rFontForTable);
    //            rPrForTable.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "12" }); //font size (in 1/72  of an inch)                 

    //            //creation of a style
    //            DocumentFormat.OpenXml.Wordprocessing.Style styleForTable = new DocumentFormat.OpenXml.Wordprocessing.Style();
    //            styleForTable.StyleId = "StyleForTable";
    //            styleForTable.Append(new Name() { Val = "StyleForTableName" });
    //            styleForTable.Append(new BasedOn() { Val = "LightShading-Accent1" });
    //            //styleForTable.Append(new NextParagraphStyle() { Val = "TableNormal" });
    //            //styleForTable.StyleTableProperties = new StyleTableProperties() { TableStyleRowBandSize =  }; ;
    //            styleForTable.Append(rPrForTable);
    //            stylePart.Styles.Append(styleForTable);
    //            stylePart.Styles.Save(); // we save the style part

    //            TableProperties tblPr = new TableProperties();
    //            tblPr.TableStyle = new DocumentFormat.OpenXml.Wordprocessing.TableStyle { Val = "LightShading-Accent1" }; 
    //            tblPr.TableWidth = new TableWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
    //            tblPr.TableJustification = new TableJustification { Val = TableRowAlignmentValues.Center };

    //            tblPr.TableLook = new TableLook();
    //            tblPr.TableLook.Val = "04A0";
    //            tblPr.TableLook.FirstRow = DocumentFormat.OpenXml.OnOffValue.FromBoolean(true);
    //            tblPr.TableLook.FirstColumn = DocumentFormat.OpenXml.OnOffValue.FromBoolean(true);                
    //            tblPr.TableLook.LastRow = DocumentFormat.OpenXml.OnOffValue.FromBoolean(false);
    //            tblPr.TableLook.LastColumn = DocumentFormat.OpenXml.OnOffValue.FromBoolean(false);
    //            tblPr.TableLook.NoHorizontalBand = DocumentFormat.OpenXml.OnOffValue.FromBoolean(false);
    //            tblPr.TableLook.NoVerticalBand = DocumentFormat.OpenXml.OnOffValue.FromBoolean(true);

    //            //tblPr.Append(styleForTable);
    //            table.Append(tblPr); 

    //            // Table Grid
    //            TableGrid tg = new TableGrid();
    //            GridColumn gc1 = new GridColumn();
    //            gc1.Width = "2900";
    //            tg.Append(gc1);

    //            GridColumn gc2 = new GridColumn();
    //            gc2.Width = "1900";
    //            tg.Append(gc2);

    //            GridColumn gc3 = new GridColumn();
    //            gc3.Width = "1700";
    //            tg.Append(gc3);

    //            GridColumn gc4 = new GridColumn();
    //            gc4.Width = "1700";
    //            tg.Append(gc4);

    //            GridColumn gc5 = new GridColumn();
    //            gc5.Width = "1700";
    //            tg.Append(gc5);
    //            table.Append(tg);                          
                
    //            DocumentFormat.OpenXml.Wordprocessing.TableRow tr;
    //            DocumentFormat.OpenXml.Wordprocessing.TableCell tc1;
    //            DocumentFormat.OpenXml.Wordprocessing.TableCell tc2;
    //            DocumentFormat.OpenXml.Wordprocessing.TableCell tc3;
    //            DocumentFormat.OpenXml.Wordprocessing.TableCell tc4;
    //            DocumentFormat.OpenXml.Wordprocessing.TableCell tc5;
    //            TableCellProperties tcp = new TableCellProperties();

    //            if (dtTemp.Read())
    //            {
    //                nomEtudiant = dtTemp["Prenom"].ToString() + " " + dtTemp["Nom"].ToString().ToUpper();
    //                Paragraph paragraphName = new Paragraph();
    //                //Paragraph paragraph1 = new Paragraph() { RsidParagraphAddition = "004F7104", RsidParagraphProperties = "008F2986", RsidRunAdditionDefault = "004F7104" };
    //                ParagraphProperties ppName = new ParagraphProperties();
    //                ppName.Append(new Underline() { Val = DocumentFormat.OpenXml.Wordprocessing.UnderlineValues.Double });
    //                ppName.Append(new Justification() { Val = JustificationValues.Center });
    //                ppName.Append(new Bold());
    //                paragraphName.Append(ppName);

    //                Run run_paragraphName = new Run();
    //                // we want to put that text into the output document
    //                Text text_paragraphName = new Text("Etudiant : " + nomEtudiant);
    //                //Append elements appropriately.
    //                run_paragraphName.Append(text_paragraphName);
    //                paragraphName.Append(run_paragraphName);
    //                body.Append(paragraphName);
    //                body.Append(paragraphDate);

    //                //==========================================================================================
    //                Boolean firstPass = true;
    //                int iRowNumber = 0, iColumnNumber = 0;
    //                bool bOdd = false;

    //                //table heading row
    //                tr = CreerNewRow(false, JustificationValues.Center, true, 
    //                        "100000000000", "00086A34", "0080690B", "005467EB");

    //                tc1 = CreerNewCell("Nom du Cours", iRowNumber, iColumnNumber, bOdd, false, true, true, "2900", "00086A34", "0080690B", "00086A34");
    //                iColumnNumber++;
    //                tc2 = CreerNewCell("Cours", iRowNumber, iColumnNumber, bOdd, false, true, true, "1900", "00086A34", "0080690B", "00086A34");
    //                iColumnNumber++;
    //                tc3 = CreerNewCell("Note", iRowNumber, iColumnNumber, bOdd, false, true, true, "1700", "00086A34", "0080690B", "00086A34");
    //                iColumnNumber++;
    //                tc4 = CreerNewCell("Equivalente", iRowNumber, iColumnNumber, bOdd, false, true, true, "1000", "00086A34", "0080690B", "00086A34");
    //                iColumnNumber++;
    //                tc5 = CreerNewCell("Moyenne (GPA)", iRowNumber, iColumnNumber, bOdd, false, true, true, "1000", "00086A34", "0080690B", "00086A34");
    //                tr.Append(tc1); tr.Append(tc2); tr.Append(tc3); tr.Append(tc4); tr.Append(tc5);
    //                table.Append(tr);             
                                       
    //                do
    //                {
    //                    PeriodeNoteObtenue = Utils.DateFormattee(dtTemp["DateReussite"].ToString(), out iYear);
    //                    if (iYear == 9999)
    //                        continue;
    //                    if (PeriodeNoteObtenue != PeriodeNoteObtenueOld)
    //                    {
    //                        if (!firstPass)
    //                        {


    //                        }
                            
    //                        PeriodeNoteObtenueOld = PeriodeNoteObtenue;
    //                        firstPass = false;
    //                    }

    //                    lettreNote = Utils.ObtenirLettre((int)float.Parse(dtTemp["NoteSurCent"].ToString()), getNotesScheme, out noteMoyenne);
    //                    if (iNbreDeNotes >= 0)
    //                    {
    //                        iNbreDeNotes++;
    //                        gpa += noteMoyenne;
    //                    }
    //                    //reccuring row
    //                    iRowNumber = 0;
    //                    tr = CreerNewRow(false, JustificationValues.Center, true, "100000000000", "00086A34", "0080690B", "005467EB");

    //                    tc1 = CreerNewCell(dtTemp["NomCours"].ToString(), iRowNumber, iColumnNumber, bOdd, false, true, true, "0", "00086A34", "0080690B", "00086A34");
    //                    iColumnNumber++;
    //                    tc2 = CreerNewCell(dtTemp["NumeroCours"].ToString(), iRowNumber, iColumnNumber, bOdd, false, true, true, "0", "00086A34", "0080690B", "00086A34");
    //                    iColumnNumber++;
    //                    tc3 = CreerNewCell(dtTemp["NoteSurCent"].ToString(), iRowNumber, iColumnNumber, bOdd, false, true, true, "0", "00086A34", "0080690B", "00086A34");
    //                    iColumnNumber++;
    //                    tc4 = CreerNewCell(lettreNote, iRowNumber, iColumnNumber, bOdd, false, true, true, "0", "00086A34", "0080690B", "00086A34");
    //                    iColumnNumber++;
    //                    tc5 = CreerNewCell(noteMoyenne.ToString("F"), iRowNumber, iColumnNumber, bOdd, false, true, true, "0", "00086A34", "0080690B", "00086A34");
    //                    tr.Append(tc1); tr.Append(tc2); tr.Append(tc3); tr.Append(tc4); tr.Append(tc5);
    //                    table.Append(tr); 

    //                    iRowNumber++; iColumnNumber = 0;
    //                    bOdd = !bOdd;
    //                }
    //                while (dtTemp.Read());

    //                //appending table to body
    //                body.Append(table);

    //                if (iNbreDeNotes > 0)
    //                    gpa = gpa / iNbreDeNotes;
    //                else
    //                    gpa = 0.0f;
    //                body.Append(new Paragraph(new Run(new Text("Moyenne Générale (GPA) : " + gpa.ToString("F")))));

    //                // and body to the document
    //                mainPart.Document.Append(body);
    //                // Save changes to the main document part. 
    //                mainPart.Document.Save();

    //                db = null;

    //                // let's open this document in Word
    //                Process compiler = new Process();
    //                compiler.StartInfo.FileName = fileName;
    //                //compiler.StartInfo.Arguments = "";
    //                compiler.StartInfo.UseShellExecute = true;
    //                compiler.Start();
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.WriteLine(ex);
    //        db = null;
    //    }
    //}
    /// <summary>
    /// Creer une ligne(row) de la table
    /// </summary>
    /// <param name="cnfOddHorizontalBand"></param>
    /// <param name="jcVal"></param>
    /// <param name="cnfFirstRow"></param>
    /// <param name="cnfVal"></param>
    /// <param name="rsidR"></param>
    /// <param name="rsidRPr"></param>
    /// <param name="rsidTr"></param>
    /// <returns></returns>
    //DocumentFormat.OpenXml.Wordprocessing.TableRow CreerNewRow(bool cnfOddHorizontalBand, JustificationValues jcVal = JustificationValues.Center, bool cnfFirstRow = false, 
    //        String cnfVal = "000000100000",String rsidR = "00086A34", String rsidRPr = "00581F3C", String rsidTr = "005467EB")
    //{
    //    DocumentFormat.OpenXml.Wordprocessing.TableRow NewRow = new DocumentFormat.OpenXml.Wordprocessing.TableRow();

    //    NewRow.RsidTableRowMarkRevision = rsidR;
    //    //NewRow.RsidTableRowAddition = rsidTr;
    //    //NewRow.RsidTableRowDeletion = "00086A34";
    //    NewRow.RsidTableRowProperties = rsidRPr;

    //    TableRowProperties trPr = new TableRowProperties();
    //    ConditionalFormatStyle cnfStyle = new ConditionalFormatStyle();
    //    cnfStyle.Val = cnfVal;
    //    cnfStyle.FirstRow = DocumentFormat.OpenXml.OnOffValue.FromBoolean(cnfFirstRow);
    //    cnfStyle.OddHorizontalBand = DocumentFormat.OpenXml.OnOffValue.FromBoolean(cnfOddHorizontalBand);
    //    trPr.Append(cnfStyle);

    //    //jc
    //    Justification jc = new Justification() { Val = jcVal };
    //    trPr.Append(jc);
    //    NewRow.Append(trPr);

    //    return NewRow;
    //}
    /// <summary>
    /// Creer un cellule de la table 
    /// </summary>
    /// <param name="sText"></param>
    /// <param name="iRowNumber"></param>
    /// <param name="iColumnNumber"></param>
    /// <param name="bOdd"></param>
    /// <param name="cnfOddHorizontalBand"></param>
    /// <param name="cnfFirstRow"></param>
    /// <param name="cnfFirstColumn"></param>
    /// <param name="sWidth"></param>
    /// <param name="rsidR"></param>
    /// <param name="rsidRPr"></param>
    /// <param name="rsidDefault"></param>
    /// <param name="jcVal"></param>
    /// <returns></returns>
    //DocumentFormat.OpenXml.Wordprocessing.TableCell CreerNewCell(String sText, int iRowNumber, int iColumnNumber, bool bOdd, bool cnfOddHorizontalBand, bool cnfFirstRow = false, bool cnfFirstColumn = false, String sWidth = "0", String rsidR = "00086A34", 
    //        String rsidRPr = "00581F3C", String rsidDefault = "005467EB", JustificationValues jcVal = JustificationValues.Center)
    //{
    //    DocumentFormat.OpenXml.Wordprocessing.TableCell NewCell = new DocumentFormat.OpenXml.Wordprocessing.TableCell();
    //    try
    //    {
    //        TableCellProperties tcPr = new TableCellProperties();
    //        ConditionalFormatStyle cnfStyle = new ConditionalFormatStyle();
    //        cnfStyle.Val = bOdd ? "000000100000" : "001000000000";
    //        cnfStyle.FirstRow = DocumentFormat.OpenXml.OnOffValue.FromBoolean(cnfFirstRow);
    //        cnfStyle.FirstColumn = DocumentFormat.OpenXml.OnOffValue.FromBoolean(cnfFirstColumn);
    //        cnfStyle.OddHorizontalBand = DocumentFormat.OpenXml.OnOffValue.FromBoolean(cnfOddHorizontalBand);
    //        tcPr.Append(cnfStyle);

    //        Justification jc = new Justification() { Val = jcVal };
    //        tcPr.Append(jc);

    //        //Cell width
    //        TableCellWidth tcW = new TableCellWidth() { Width = sWidth, Type = TableWidthUnitValues.Dxa };
    //        tcPr.Append(tcW);

    //        //Cell borders
    //        TableCellBorders tcBorders = new TableCellBorders();
    //        tcBorders.RightBorder = new RightBorder(){Val = BorderValues.Single, Size = UInt32Value.FromUInt32(8), Color = "4F81BD", ThemeColor = ThemeColorValues.Accent1, Space = 0};
    //        tcBorders.TopBorder = new TopBorder(){Val = BorderValues.Single, Size = UInt32Value.FromUInt32(8), Color = "4F81BD", ThemeColor = ThemeColorValues.Accent1, Space = 0};
    //        tcBorders.BottomBorder = new BottomBorder(){Val = BorderValues.Single, Size = UInt32Value.FromUInt32(8), Color = "4F81BD", ThemeColor = ThemeColorValues.Accent1, Space = 0};
    //        tcPr.Append(tcBorders);

    //        TableCellVerticalAlignment tcVAlign = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Bottom };
    //        tcPr.Append(tcVAlign);
    //        NewCell.Append(tcPr);

    //        // Paragraph
    //        Paragraph tcParagraph = new Paragraph(new Run(new Text(sText)));
    //        tcParagraph.RsidParagraphMarkRevision = rsidR;
    //        tcParagraph.RsidParagraphProperties = rsidRPr;
    //        tcParagraph.RsidRunAdditionDefault = rsidDefault;

    //        ParagraphProperties tcpPr = new ParagraphProperties();
    //        tcpPr.Justification = new Justification() { Val = JustificationValues.Center };
    //        tcParagraph.Append(tcpPr);

    //        RunProperties rPr = new RunProperties();
    //        rPr.RunStyle = new RunStyle() { Val = "24" };
    //        rPr.Append(new Bold()); // it is Bold
            
    //        rPr.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
    //        tcParagraph.Append(rPr);
    //        NewCell.Append(tcParagraph);
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.WriteLine(ex.Message);
    //    }
    //    return NewCell;
    //}
    
    protected void btnWord_Click(object sender, ImageClickEventArgs e)
    {
        //ProcessInfoForWord();
        //ProcessInfoForExcel();
    }

//    void ProcessInfoForExcel()
//    {        
//        float noteMoyenne = 0.0f, gpa = 0.0f;
//        int iYear = 9999, iNbreDeNotes = 0;
//        int creditsTotal = 0;
//        string credits;
//        string sFileName = "error";

//        DB_Access db = new DB_Access();
//        using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
//        {
//            sPersonneID = txtPersonneID.Text;
//            if (sPersonneID.Trim() == String.Empty)
//            {
//                return;
//            }
//            sSql = String.Format("SELECT CP.NumeroCours, IsNull(CP.DateReussite, '9999-01-01') DateReussite, CP.NoteSurCent, " +
//            " CP.NotePassage, C.NomCours, C.Credits, P.EtudiantIdPlus, P.Nom, P.Prenom FROM CoursPris CP, Cours C, Personnes P " +
//            " WHERE CP.PersonneID = '{0}' AND CP.NumeroCours = C.NumeroCours AND CP.PersonneID = P.PersonneID  AND C.ExamenEntree = 0 ORDER BY DateReussite DESC", sPersonneID);

//            try
//            {
//                sqlConn.Open();
//            }
//            catch
//            {
//                litBody.Text = "ERREUR: Connexion Base de Données!";
//                return;
//            }
//            getNotesScheme = db.FillNotations(sqlConn);                    // Get the different notations

//            SqlDataReader dtTemp = db.GetDataReader(sSql, sqlConn);
//            String PeriodeNoteObtenue = "", PeriodeNoteObtenueOld = "", lettreNote = "";
//            int nombreDeLignes = 0;
//            bool firstPass = true;

//            try
//            {
//                var workbook = new XLWorkbook();
//                var worksheet = workbook.Worksheets.Add("1");   // need to create a worksheet to start with before knowing the name of the class. We'll delete at the end.
//                int iRowIndex = 2;

//                // Creer Header
//                worksheet.Cell(1, 1).Value = "UNIVERSITE ESPOIR";
//                worksheet.Cell(2, 1).Value = "Relevé de Notes";

//                worksheet.Cell(5, 1).Value = String.Format("Date d'Impression: {0}</TD></TR>", DateTime.Today.Date.ToString("dd-MMM-yyyy"));

//                // Names of columns
//                worksheet.Cell(6, 1).Value = "Nom du Cours";
//                worksheet.Cell(6, 2).Value = "Cours";
//                worksheet.Cell(6, 3).Value = "Note Obtenue";
//                worksheet.Cell(6, 4).Value = "Lettre Equivalente";
//                worksheet.Cell(6, 5).Value = "Moyenne (GPA)";
//                worksheet.Cell(6, 6).Value = "Crédits";
//                iRowIndex = 7;
//                // Cells merge horizontally
//                worksheet.Range("A1:G1").Merge();
//                worksheet.Range("A1:G1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

//                worksheet.Range("A2:G2").Merge();
//                worksheet.Range("A2:G2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

//                worksheet.Range("A3:G3").Merge();
//                worksheet.Range("A3:G3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

//                worksheet.Range("A4:G4").Merge();
//                worksheet.Range("A4:G4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

//                //ws.Style.Fill.BackgroundColor = XLColor.LightCyan;

//                // Column font weight
//                worksheet.Range("A1", "E1").Style.Font.Bold = true;
//                worksheet.Range("A2", "E2").Style.Font.Bold = true;
//                worksheet.Range("A3", "E3").Style.Font.Bold = true;
//                worksheet.Range("A4", "E4").Style.Font.Bold = true;
//                worksheet.Range("A5", "E5").Style.Font.Bold = true;

//                // Column Width
//                worksheet.Columns("A").Width = 12;
//                worksheet.Columns("B").Width = 15;
//                worksheet.Columns("C").Width = 12;
//                worksheet.Columns("D").Width = 10;

//                worksheet.Columns("E").Width = 5;
//                worksheet.Columns("F").Width = 5;
//                worksheet.Columns("G").Width = 5;
//                //worksheet.Columns("H").Width = 5;
//                //worksheet.Columns("I").Width = 5;
//                //worksheet.Columns("J").Width = 5;
//                //worksheet.Columns("K").Width = 5;
//                //worksheet.Columns("L").Width = 5;
//                //worksheet.Columns("M").Width = 5;

//                //if (dtTemp.Read())
//                //{
//                //    do
//                //    {
//                //        PeriodeNoteObtenue = Utils.DateFormattee(dtTemp["DateReussite"].ToString(), out iYear);
//                //        if (iYear == 9999)
//                //            continue;

//                //        if (PeriodeNoteObtenue != PeriodeNoteObtenueOld)
//                //        {
//                //            if (!firstPass)
//                //            {
//                //worksheet.Cell(3, 1).Value = dtTemp["Prenom"].ToString() + " " + dtTemp["Nom"].ToString().ToUpper();
//                //sFileName = dtTemp["Prenom"].ToString() + "_" + dtTemp["Nom"].ToString().ToUpper();
//                //worksheet.Cell(4, 1).Value = "ID Edutiant : " + dtTemp["EtudiantID"].ToString();
//                //                //sRetString += String.Format("<TR><TD Colspan='7' style='width:80%;text-align:left;font-weight:bold;font-size:14px'></TD></TR>");
//                //            }

//                //            worksheet.Cell(iRowIndex, 1).Value = PeriodeNoteObtenue;
//                //            PeriodeNoteObtenueOld = PeriodeNoteObtenue;

//                //            firstPass = false;
//                //        }
//                //        lettreNote = Utils.ObtenirLettre((int)float.Parse(dtTemp["NoteSurCent"].ToString()), getNotesScheme, out noteMoyenne);
//                //        if (lettreNote.ToUpper() != "I" && lettreNote.ToUpper() != "E" && lettreNote.ToUpper() != "F")
//                //            credits = dtTemp["credits"].ToString();
//                //        else
//                //            credits = "0";

//                //        // Creer entrees
//                //        iRowIndex++;

//                //        worksheet.Cell(iRowIndex, 2).Value = dtTemp["NomCours"].ToString();
//                //        worksheet.Cell(iRowIndex, 3).Value = dtTemp["NumeroCours"].ToString();
//                //        worksheet.Cell(iRowIndex, 4).Value = float.Parse(dtTemp["NoteSurCent"].ToString()).ToString("F");
//                //        worksheet.Cell(iRowIndex, 2).Value = lettreNote;
//                //        worksheet.Cell(iRowIndex, 2).Value = noteMoyenne.ToString("F");
//                //        worksheet.Cell(iRowIndex, 2).Value = credits;
//                //        nombreDeLignes++;

//                //        if (noteMoyenne >= 0)
//                //        {
//                //            gpa += noteMoyenne;
//                //            creditsTotal += int.Parse(credits);
//                //            iNbreDeNotes++;
//                //        }




//                //        // Format worsheet before creating the new one
//                //        nombreDeLignes += 5;  // add # of header lines
//                //        for (int i = 1; i <= nombreDeLignes; i++)
//                //        {
//                //            for (int j = 1; j < 14; j++)
//                //            {
//                //                worksheet.Cell(i, j).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
//                //            }
//                //        }
//                //        //Addition de bordure extérieure
//                //        worksheet.Range("A1", "M" + nombreDeLignes.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

//                //        // Orientation
//                //        worksheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;

//                //        //Addition de bordure extérieure
//                //        worksheet.Range("A1", "M" + nombreDeLignes.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

//                //        workbook.Worksheets.Delete("1");    // no need anymore

//                //        String fileName = "c:\\extras\\UEspoir\\" + DateTime.Today.Day.ToString() + "-" + DateTime.Today.Month.ToString() + "-" +
//                //            DateTime.Today.Year.ToString() + "-" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() +
//                //            DateTime.Now.Second.ToString() + ".xlsx";
//                //        workbook.SaveAs(fileName);

//                //        // Open Spreadsheet
//                //        Process compiler = new Process();
//                //        compiler.StartInfo.FileName = fileName;
//                //        //compiler.StartInfo.Arguments = "";
//                //        compiler.StartInfo.UseShellExecute = true;
//                //        //compiler.StartInfo.RedirectStandardOutput = true;
//                //        compiler.Start();

//                //    }
//                //    while (dtTemp.Read());
//                //}
//                dtTemp = null;


//                if (iNbreDeNotes > 0)
//                    gpa = gpa / iNbreDeNotes;
//                else
//                    gpa = 0.0f;

//                //sRetString += String.Format("<TR><TD Colspan='7' width:'80%'><hr style='background-color:#669999;' size='2' width='100%'/></TD></TR>");
//                //sRetString += String.Format("<TR><TD Colspan='4' style='width:80%;text-align:left;font-weight:bold;font-size:14px'>Moyenne Générale (GPA) : {0}</TD>", gpa.ToString("F"));
//                //sRetString += String.Format("<TD Colspan='2' style='width:80%;text-align:right;font-weight:bold;font-size:14px'>Nombre de Crédits :</TD>");
//                //sRetString += String.Format("<TD style='width:80%;text-align:center;font-weight:bold;font-size:14px'>{0}</TD></TR>", creditsTotal);
//                //sRetString += String.Format("<TD Colspan='7' width:'80%'><hr style='background-color:#669999;' size='2' width='100%'/></TD></TR>");

//                sFileName = "c:\\extras\\UEspoir\\" + sFileName + "_" + DateTime.Today.Day.ToString() + "-" + DateTime.Today.Month.ToString() + "-" +
//                                DateTime.Today.Year.ToString() + "-" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() +
//                                DateTime.Now.Second.ToString() + ".xlsx";
//                workbook.SaveAs(sFileName);

//#if DEBUG
//        String sUserName = "spote", sDomain = "POTEAU-PC", sPass = "robenson2";
//#else
//                String sUserName = "spoteau", sDomain = "CCPAP1", sPass = "robenson22";
//#endif
//                if (SystemCalls.ImpersonateValidUser(sUserName, sDomain, sPass))
//                {
//                    // Open Spreadsheet
//                    Process compiler = new Process();
//                    compiler.StartInfo.FileName = sFileName;
//                    //compiler.StartInfo.Arguments = "";
//                    compiler.StartInfo.UseShellExecute = true;
//                    //compiler.StartInfo.RedirectStandardOutput = true;
//                    compiler.Start();

//                    Thread.Sleep(3000);
//                    compiler.Dispose();
//                }
//                else
//                {
//                    Debug.WriteLine("Nope Nope ......");
//                    //Your impersonation failed. Therefore, this is a fail-safe mechanism here.
//                    //lblError.Text = string.Format("Fichier crée: {0}!", fileName);
//                    //lblError.ForeColor = System.Drawing.Color.Green;
//                }
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine(ex);
//                //sRetString += "<br> ERREUR - ERREUR - ERREUR !!!";
//                dtTemp = null;
//            }

//        }
//    }
    
    
    //void ProcessInfoForWord()
    //{
    //    String sRetString = String.Empty;
    //    float noteMoyenne = 0.0f, gpa = 0.0f;
    //    int iYear = 9999, iNbreDeNotes = 0;
    //    String nomEtudiant = "";

    //    DB_Access db = new DB_Access();
    //    using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
    //    {
    //        try
    //        {
    //            using (SqlConnection sqlConn1 = new SqlConnection(ConnectionString))
    //            {
    //                try
    //                {
    //                    sqlConn1.Open();
    //                    getNotesScheme = db.FillNotations(sqlConn1);                    // Get the different notations
    //                }
    //                catch (Exception ex)
    //                {
    //                    Debug.WriteLine(ex.Message);
    //                }
    //            }

    //            sqlConn.Open();
    //            //SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
    //            sPersonneID = txtPersonneID.Text;
    //            if (sPersonneID.Trim() == String.Empty)
    //            {
    //                litBody.Text = "Erreur !!!";
    //                return;
    //            }

    //            String sSql = String.Format("SELECT CP.NumeroCours, IsNull(CP.DateReussite, '9999-01-01') DateReussite, CP.NoteSurCent, " +
    //                " CP.NotePassage, C.NomCours, C.Credits, P.Nom, P.Prenom FROM CoursPris CP, Cours C, Personnes P " +
    //                " WHERE CP.PersonneID = '{0}' AND CP.NumeroCours = C.NumeroCours AND CP.PersonneID = P.PersonneID  AND C.ExamenEntree = 0 ORDER BY DateReussite DESC", sPersonneID);

    //            SqlDataReader dtTemp = db.GetDataReader(sSql, sqlConn);
    //            String PeriodeNoteObtenue = "", PeriodeNoteObtenueOld = "", lettreNote = "";

    //            String fileName = "";

    //            // Let's create a simple DOCX document.
    //            DocumentCore docx = new DocumentCore();
    //            //DocumentCore.Serial = "put your serial here";

    //            if (dtTemp.Read())
    //            {
    //                nomEtudiant = dtTemp["Prenom"].ToString() + " " + dtTemp["Nom"].ToString().ToUpper();
    //                fileName = "c:\\extras\\UEspoir\\Releves\\" + nomEtudiant.Replace(" ", "").Trim() + "-" + DateTime.Today.Day.ToString() + "-" + DateTime.Today.Month.ToString() + "-" +
    //                DateTime.Today.Year.ToString() + "-" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() +
    //                DateTime.Now.Second.ToString() + ".docx";

    //                nomEtudiant = "Etudiant : " + nomEtudiant;
    //                //==========================================================================================
    //                Boolean firstPass = true;
    //                int iRowNumber = 0, iColumnNumber = 0;
    //                bool bOdd = false;

    //                // LOGO 

    //                // Add new section.
    //                Section section = new Section(docx);
    //                docx.Sections.Add(section);

    //                //// Let's set page size A4.
    //                section.PageSetup.PaperType = PaperType.Letter;

    //                //// Way 1: Add 1st paragraph.
    //                //section.Blocks.Add(new Paragraph(docx, "Université Espoir de Calvary Chapel Port-au-Prince"));
    //                //Paragraph par1 = section.Blocks[0] as Paragraph;
    //                //par1.ParagraphFormat.Alignment = HorizontalAlignment.Center;

    //                ////// Let's add a second line.
    //                //par1.Inlines.Add(new SpecialCharacter(docx, SpecialCharacterType.LineBreak));
    //                //par1.Inlines.Add(new Run(docx, "Relevé de Notes"));

    //                // Let's change font name, size and color.
    //                //CharacterFormat cf = new CharacterFormat() { FontName = "Verdana", Size = 16, FontColor = Color.Orange };
    //                //foreach (Inline inline in par1.Inlines)
    //                //    if (inline is Run)
    //                //        (inline as Run).CharacterFormat = cf.Clone();

    //                // Way 2 (easy): Add 2nd paragarph using another way.
    //                //docx.Content.End.Insert("Université Espoir\nde\nCalvary Chapel Port-au-Prince", new CharacterFormat() { Size = 25, FontColor = Color.Blue, Bold = true });
    //                ////SpecialCharacter lBr = new SpecialCharacter(docx, SpecialCharacterType.LineBreak);
    //                ////docx.Content.End.Insert(lBr.Content);
    //                //docx.Content.End.Insert("Relevé de Notes", new CharacterFormat() { Size = 20, FontColor = Color.DarkGreen, UnderlineStyle = UnderlineType.Single });

    //                // Titre: "Relevé de Notes"

    //                // date
    //                //("Date d’impression : " + DateTime.Today.ToString("dd-MMM-yyyy"));

    //                // Add a new section 
    //                Section sectionTable = new Section(docx);

    //                // Way 1: Add 1st paragraph.
    //                section.Blocks.Add(new Paragraph(docx, "Université Espoir de Calvary Chapel Port-au-Prince"));
    //                Paragraph par1 = section.Blocks[0] as Paragraph;
    //                par1.ParagraphFormat.Alignment = HorizontalAlignment.Center;

    //                //// Let's add a second line.
    //                par1.Inlines.Add(new SpecialCharacter(docx, SpecialCharacterType.LineBreak));
    //                par1.Inlines.Add(new Run(docx, "Relevé de Notes"));
    //                par1.Inlines.Add(new SpecialCharacter(docx, SpecialCharacterType.LineBreak));
    //                par1.Inlines.Add(new Run(docx, "Date d’impression : " +
    //                    DateTime.Today.ToString("dd-MMM-yyyy")));

    //                docx.Sections.Add(sectionTable);
    //                SautinSoft.Document.Tables.Table table = new SautinSoft.Document.Tables.Table(docx);
    //                SautinSoft.Document.Tables.TableRow row = new SautinSoft.Document.Tables.TableRow(docx);
    //                // Add columns
    //                SautinSoft.Document.Tables.TableCell cell = new SautinSoft.Document.Tables.TableCell(docx);
    //                // Set some cell formatting 
    //                cell.CellFormat.Borders.SetBorders(MultipleBorderTypes.Outside, SautinSoft.Document.BorderStyle.Single, Color.Blue, 1.0);
    //                cell.CellFormat.PreferredWidth = new TableWidth(30, TableWidthUnit.Percentage);
    //                row.Cells.Add(cell);
    //                cell.Content.Start.Insert("Nom du Cours");
    //                //
    //                cell = new SautinSoft.Document.Tables.TableCell(docx);
    //                cell.CellFormat.Borders.SetBorders(MultipleBorderTypes.Outside, SautinSoft.Document.BorderStyle.Single, Color.Blue, 1.0);
    //                cell.CellFormat.PreferredWidth = new TableWidth(20, TableWidthUnit.Percentage);
    //                row.Cells.Add(cell);
    //                cell.Content.Start.Insert("Cours");
    //                //
    //                cell = new SautinSoft.Document.Tables.TableCell(docx);
    //                cell.CellFormat.Borders.SetBorders(MultipleBorderTypes.Outside, SautinSoft.Document.BorderStyle.Single, Color.Blue, 1.0);
    //                cell.CellFormat.PreferredWidth = new TableWidth(15, TableWidthUnit.Percentage);
    //                row.Cells.Add(cell);
    //                cell.Content.Start.Insert("Note");
    //                //
    //                cell = new SautinSoft.Document.Tables.TableCell(docx);
    //                cell.CellFormat.Borders.SetBorders(MultipleBorderTypes.Outside, SautinSoft.Document.BorderStyle.Single, Color.Blue, 1.0);
    //                cell.CellFormat.PreferredWidth = new TableWidth(15, TableWidthUnit.Percentage);
    //                row.Cells.Add(cell);
    //                cell.Content.Start.Insert("Equivalente");
    //                //
    //                cell = new SautinSoft.Document.Tables.TableCell(docx);
    //                cell.CellFormat.Borders.SetBorders(MultipleBorderTypes.Outside, SautinSoft.Document.BorderStyle.Single, Color.Blue, 1.0);
    //                cell.CellFormat.PreferredWidth = new TableWidth(15, TableWidthUnit.Percentage);
    //                row.Cells.Add(cell);
    //                cell.Content.Start.Insert("Moyenne (GPA)");

    //                table.Rows.Add(row);

    //                do
    //                {
    //                    PeriodeNoteObtenue = Utils.DateFormattee(dtTemp["DateReussite"].ToString(), out iYear);
    //                    if (iYear == 9999)
    //                        continue;
    //                    if (PeriodeNoteObtenue != PeriodeNoteObtenueOld)
    //                    {
    //                        if (!firstPass)
    //                        {
    //                            // Write Header

    //                        }
    //                        PeriodeNoteObtenueOld = PeriodeNoteObtenue;
    //                        firstPass = false;
    //                    }
    //                    lettreNote = Utils.ObtenirLettre((int)float.Parse(dtTemp["NoteSurCent"].ToString()), getNotesScheme, out noteMoyenne);
    //                    if (iNbreDeNotes >= 0)
    //                    {
    //                        iNbreDeNotes++;
    //                        gpa += noteMoyenne;
    //                    }
    //                    //reccuring row
    //                    iRowNumber = 0;

    //                    row = new SautinSoft.Document.Tables.TableRow(docx);
    //                    // Add columns
    //                    cell = new SautinSoft.Document.Tables.TableCell(docx);
    //                    // Set some cell formatting 
    //                    cell.CellFormat.Borders.SetBorders(MultipleBorderTypes.Outside, SautinSoft.Document.BorderStyle.Single, Color.Blue, 1.0);
    //                    cell.CellFormat.PreferredWidth = new TableWidth(30, TableWidthUnit.Percentage);
    //                    row.Cells.Add(cell);
    //                    cell.Content.Start.Insert(dtTemp["NomCours"].ToString());
    //                    //
    //                    cell = new SautinSoft.Document.Tables.TableCell(docx);
    //                    cell.CellFormat.Borders.SetBorders(MultipleBorderTypes.Outside, SautinSoft.Document.BorderStyle.Single, Color.Blue, 1.0);
    //                    cell.CellFormat.PreferredWidth = new TableWidth(20, TableWidthUnit.Percentage);
    //                    row.Cells.Add(cell);
    //                    cell.Content.Start.Insert(dtTemp["NumeroCours"].ToString());
    //                    //
    //                    cell = new SautinSoft.Document.Tables.TableCell(docx);
    //                    cell.CellFormat.Borders.SetBorders(MultipleBorderTypes.Outside, SautinSoft.Document.BorderStyle.Single, Color.Blue, 1.0);
    //                    cell.CellFormat.PreferredWidth = new TableWidth(15, TableWidthUnit.Percentage);
    //                    row.Cells.Add(cell);
    //                    cell.Content.Start.Insert(dtTemp["NoteSurCent"].ToString());
    //                    //
    //                    cell = new SautinSoft.Document.Tables.TableCell(docx);
    //                    cell.CellFormat.Borders.SetBorders(MultipleBorderTypes.Outside, SautinSoft.Document.BorderStyle.Single, Color.Blue, 1.0);
    //                    cell.CellFormat.PreferredWidth = new TableWidth(15, TableWidthUnit.Percentage);
    //                    row.Cells.Add(cell);
    //                    cell.Content.Start.Insert(lettreNote);
    //                    //
    //                    cell = new SautinSoft.Document.Tables.TableCell(docx);
    //                    cell.CellFormat.Borders.SetBorders(MultipleBorderTypes.Outside, SautinSoft.Document.BorderStyle.Single, Color.Blue, 1.0);
    //                    cell.CellFormat.PreferredWidth = new TableWidth(15, TableWidthUnit.Percentage);
    //                    row.Cells.Add(cell);
    //                    cell.Content.Start.Insert(noteMoyenne.ToString("F"));

    //                    table.Rows.Add(row);

    //                    iRowNumber++; iColumnNumber = 0;
    //                    bOdd = !bOdd;
    //                }
    //                while (dtTemp.Read());
    //                // Add this table to the current section. 
    //                sectionTable.Blocks.Add(table);

    //                if (iNbreDeNotes > 0)
    //                    gpa = gpa / iNbreDeNotes;
    //                else
    //                    gpa = 0.0f;
    //                Paragraph pr = (new Paragraph(docx, ""));
    //                pr.Inlines.Add(new SpecialCharacter(docx, SpecialCharacterType.LineBreak));
    //                section.Blocks.Add(pr);
    //                //
    //                // ("Moyenne Générale (GPA) : " + gpa.ToString("F")))));
    //                section.Blocks.Add(new Paragraph(docx, "Moyenne Générale (GPA) : " + gpa.ToString("F")));

    //                db = null;

    //                // Save DOCX to a file
    //                docx.Save(fileName);

    //                // Open the result for demonstation purposes.
    //                System.Diagnostics.Process.Start(fileName);

    //                //// let's open this document in Word
    //                //Process compiler = new Process();
    //                //compiler.StartInfo.FileName = fileName;
    //                ////compiler.StartInfo.Arguments = "";
    //                //compiler.StartInfo.UseShellExecute = true;
    //                //compiler.Start();
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Debug.WriteLine(ex);
    //        }
    //    }
    //    db = null;
    //}

}



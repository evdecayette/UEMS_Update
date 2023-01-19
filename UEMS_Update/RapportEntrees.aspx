<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="RapportEntrees.aspx.cs" Inherits="RapportEntrees" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="css/formItems.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:ToolkitScriptManager ID="toolkit1" runat="server"></asp:ToolkitScriptManager>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <div class='row'><div class='col-lg-12'><h3 class='page-header'>Rapports Financiers</h3></div></div>
    <table>
         <tr>
            <td>
                <div class="dynamiclabel">
                <asp:TextBox ID="txtDateFrom" autocomplete="off" runat="server" placeholder="De Cette Date" Font-Size="Medium" Width="150px" TabIndex="0" AutoPostBack="True" OnTextChanged="txtDateFrom_TextChanged"></asp:TextBox>
                <label for="txtDateFrom">Date - Cliquez svp</label>
                <asp:CalendarExtender ID="calDateStart" runat="server" TargetControlID="txtDateFrom" PopupButtonID="txtDateFrom" Format="MM/dd/yyyy"></asp:CalendarExtender>
                </div>
            </td>
            <td>
                <div class="dynamiclabel">
                <asp:TextBox ID="txtDateTo" autocomplete="off" runat="server" placeholder="A Cette Date" Font-Size="Medium" Width="150px" TabIndex="1" AutoPostBack="True" OnTextChanged="txtDateTo_TextChanged"></asp:TextBox>
                <label for="txtDateTo">Date - Cliquez svp</label>
                <asp:CalendarExtender ID="calDateEnd" runat="server" TargetControlID="txtDateTo" PopupButtonID="txtDateTo" Format="MM/dd/yyyy"></asp:CalendarExtender>
                </div>
            </td>
        </tr>
        <tr>
            <td>
               <div class="dynamicbutton">
                <asp:Button ID="btnRapportPeriodeChoisie" Visible="true" runat="server" Text="Rapport Pour La Période Choisie"  width="300px" OnClick="btnRapportPeriodeChoisie_Click" />
                <%--<label for="btnTotals">Rapport Financier Pour Une Période Spécifique</label>--%>
              </div>
            </td>
            <td>
            </td>
        </tr>
        <tr style="width:100%" >
            <td>
               <div class="dynamicbutton" >           
                 <asp:Button ID="btnRapportPourUneSession" runat="server" Text="Rapport Pour Une Session" Width="300px" OnClick="btnRapportPourUneSession_Click" />
                 <%--<label for="btnRapportParCheque">Rapport Financier Pour Une Session Spécifique</label>--%>
               </div>                    
             </td>
             <td>
              <div class="dynamiSelect">  
                  <asp:DropDownList ID="ddlSessions" runat="server" Width="400px" Font-Size="Medium" Height="30px" >
                  </asp:DropDownList>
              </div> 
            </td>
        </tr>
        <tr style="width:100%" >
            <td>
               <div class="dynamicbutton" >           
                 <asp:Button ID="btnRapportPourUser" runat="server" Text="Rapport Pour Un Utilisateur" Width="300px" OnClick="btnRapportPourUser_Click" />
                 <%--<label for="btnRapportPourUser">Rapport Entrées Pour Un Utilisateur</label>--%>
               </div>                    
             </td>
             <td>
              <div class="dynamiSelect">  
                  <asp:DropDownList ID="ddlUsers" runat="server" Width="400px" Font-Size="Medium" Height="30px" >
                  </asp:DropDownList>
              </div> 
            </td>
        </tr>
        <tr>
            <td>
               <div class="dynamicbutton">
                <asp:Button ID="btnExcelPourClasse" Visible="true" runat="server" Text="Générer Tableurs pour les Classes"  width="300px" OnClick="btnExcelPourClasse_Click" />
               
              </div>
            </td>
            <td>
            <div class="dynamiSelect">  
                  <asp:DropDownList ID="ddlSessionPourExcel" runat="server" Width="400px" Font-Size="Medium" Height="30px" >
                  </asp:DropDownList>
              </div> 
<%--              <div class="dynamiSelect">  
                  <asp:DropDownList ID="ddlExcelPourClasse" runat="server" Width="400px" Font-Size="Medium" Height="30px" >
                  </asp:DropDownList>
              </div> --%>
            </td>
        </tr>
        <tr>
            <td>
               <div class="dynamicbutton">
                <asp:Button ID="btnFacture" Visible="false" runat="server" Text="Facturation - Session Courante"  width="300px" OnClick="btnFacture_Click" />
               <%--<label for="btnFacture">Facturation - Session Courante</label>--%>
              </div>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td>
               <div class="dynamicbutton">
                <asp:Button ID="btnHoraire" Visible="true" runat="server" Text="Horaire - Session Courante"  width="300px" OnClick="btnHoraire_Click" />
               <%--<label for="btnHoraire">Horaire - Session Courante</label>--%>
              </div>
            </td>
            <td>
            <div class="dynamicbutton">
                <asp:Button ID="btnRelevesTousEtudiants" Visible="true" runat="server" Text="Relevés de tous les Etudiants Actifs"  width="300px" OnClick="btnRelevesTousEtudiants_Click" />
               <%--<label for="btnHoraire">Relevés: Tous Les Etudiants!</label>--%>
              </div>
            </td>
        </tr>
        <tr>
            <td>
               <div class="dynamicbutton">
                <asp:Button ID="btnControle" Visible="true" runat="server" Text="Controle Financier - Session Courante"  width="300px" OnClick="btnControle_Click" />
               <%--<label for="btnControle">Controle Financier - Session Courante</label>--%>
              </div>
            </td>
            <td>
            <div class="dynamicbutton">
                <asp:Button ID="btnCredits" Visible="true" runat="server" Text="Nombre Crédits/Etudiants Actifs"  width="300px" OnClick="btnCredits_Click" />               
              </div>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="text-align:center">
                <asp:Label ID="lblError" runat="server" Font-Bold="True" Font-Size="Medium" ForeColor="#C00000" Width="100%"></asp:Label>
            </td>
         </tr>
    </table>
    </ContentTemplate>
</asp:UpdatePanel>

</asp:Content>


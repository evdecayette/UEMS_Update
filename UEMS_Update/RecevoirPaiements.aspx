<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="RecevoirPaiements.aspx.cs" Inherits="RecevoirPaiements" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="formItems.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
    <ContentTemplate>
       <div class='row'>
           <div class='col-lg-12'>
               <h4 class='page-header'>Recevoir Paiements de : <asp:Label ID="lblEtudiantInfo" runat="server" Text=""></asp:Label></h4><br />
            </div>
        </div>
     <div class='row'>
         <div class='col-lg-12'>
                <asp:Label runat="server" Text="Balance : " style="color:red"/><asp:Label ID="lblBalance" runat="server" Text=""></asp:Label>
                <asp:Button ID="btnBackToInfoEtudiant" runat="server" Width="150" Text="Back: Info Etudiant" class="btn btn-outline btn-primary" OnClick="btnBackToInfoEtudiant_Click" />                                      
                <hr />
            </div>
      </div>
      <div class='row'>
           <div class='col-lg-12'>
                <div class='panel panel-default'>
                   <div class='panel-heading'>
                    Les Erreurs sont affichées ici: <asp:Label ID="lblError" runat="server" Text="" Font-Bold="True" ForeColor="#CC0000"></asp:Label>
                   </div>
                    <div class="row">
                        <div class="col-lg-6">
                            <div class="panel panel-green">
                                <div class="panel-heading">
                                    Nouveau Paiement et Paiements Dus
                                </div>
                                <div class="panel-body">
                                    <table>
                                        <tr>
                                            <td>
                                                <div class="dynamicSelect">
                                                <asp:DropDownList ID="ddlObigations" runat="server" Height="30px" AutoPostBack="True" OnSelectedIndexChanged="ddlObigations_SelectedIndexChanged"></asp:DropDownList>
                                                </div>        
                                            </td>
                                            <td style="Width:10%">&#32;</td>
                                            <td style="align-content:center">                                              
                                                <div class="dynamicbutton"><asp:Button ID="btnSauvegarder" runat="server" Text="Sauvegarder"  
                                                    Width="150px"  Height="35px" OnClick="btnSauvegarder_Click"/>
                                                </div>                                                 
                                            </td>
                                        </tr>
                                        <tr>
                                            <td> 
                                                <div class="dynamiclabel">                                        
                                                 <asp:TextBox autocomplete="off" ID="txtDate" runat="server" placeholder="Date (mm/jj/aaaa)"  Height="35px"></asp:TextBox>
                                                    
                                                </div>
                                                <asp:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="txtDate" PopupButtonID="txtAddDate" Format="MM/dd/yyyy" ></asp:CalendarExtender>
                                            </td>
                                            <td>&#32;</td>
                                            <td style="align-content:center">
                                                <div class="dynamiclabel">
                                                    <asp:TextBox ID="txtMontant" runat="server" CausesValidation="True" Width="150px" placeholder="Montant"  Height="35px"></asp:TextBox>
                                                </div> 
                                                <asp:TextBox ID="txtPersonneID" runat="server" Visible="false"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                    <p><asp:Literal ID="LitPaiement" runat="server"></asp:Literal></p>
                                </div>
                                <div class="panel-footer">
                                   <p><asp:Literal ID="LitPaiementFooter" runat="server"></asp:Literal></p>
                                </div>
                            </div>  
                        </div>                    
                        <div class="col-lg-6">
                            <div class="panel panel-yellow">
                                <div class="panel-heading">
                                    Paiements Déjà Reçus
                                </div>
                                <div class="panel-body">
                                    <p><asp:Literal ID="LitDejaPaye" runat="server"></asp:Literal></p>
                                </div>
                                <div class="panel-footer">
                                    <p><asp:Literal ID="LitDejaPayeFooter" runat="server"></asp:Literal></p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
       </div>
        </ContentTemplate>
   </asp:UpdatePanel>
</asp:Content>


<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="FactureManuelle.aspx.cs" Inherits="FactureManuelle" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
        <asp:ToolkitScriptManager ID="toolkit1" runat="server">
                <Scripts><asp:ScriptReference Name="jquery"/></Scripts>
    </asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
        <ContentTemplate>
        <div class="row">
                <div class="col-lg-12">
                    <h4 class="page-header">Information Générale sur l'Etudiant: <asp:Label ID="txtNomComplet_Header" runat="server"> </asp:Label></h4>                   
                    <asp:Button ID="btnEditNotePasseeEtudiant" runat="server" Width="150" Text="Entrer Note Finale" class="btn btn-outline btn-primary" OnClick="btnEditNotePasseeEtudiant_Click" />
                    <asp:Button ID="btnReleveNotes" runat="server" Width="150" Text="Relevé de Notes" class="btn btn-outline btn-primary" OnClick="btnReleveNotes_Click"/>
                    <asp:Button ID="btnProgresGraduation" runat="server" Width="150" Text="Progrès Graduation" data-loading-text="Patientez svp..." class="btn btn-primary js-loading-button" OnClick="btnProgresGraduation_Click"/>
                    <asp:Button ID="btnPayments" runat="server" Width="150" Text="Ecran Des Paiements" class="btn btn-outline btn-primary" OnClick="btnPayments_Click"/>
                    <asp:TextBox ID="txtPersonneID" runat="server" Visible="false"></asp:TextBox>
                    <hr />
                </div>
                
            </div>
            <!-- /.col-lg-12 -->
            <div class="row">
                <!-- /.col-lg-4 -->
                <div class="col-lg-8">
                    <div class="panel panel-yellow">
                                <div class="panel-body">
                                    <table>
                                        <tr>
                                            <td>
                                                <div class="dynamicSelect">
                                                <asp:DropDownList ID="ddlObligations" runat="server" Height="30px" AutoPostBack="True" OnSelectedIndexChanged="ddlObigations_SelectedIndexChanged" ItemType="Obligation"></asp:DropDownList>
                                                </div>        
                                            </td>
                                            <td style="Width:10%">&#32;</td>
                                            <td>
                                                <div class="dynamiclabel">
                                                    <asp:TextBox ID="txtMontant" runat="server" CausesValidation="True" Width="150px" placeholder="Montant"  Height="35px"></asp:TextBox>
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
                                            <td colspan="5">
                                                <asp:Label ID="lblError" runat="server" Text="" Font-Bold="True" ForeColor="#CC0000"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </div>

                    </div>
                </div>
                <div class="col-lg-6">

                </div>
            </div>
            <!-- /.row -->

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>


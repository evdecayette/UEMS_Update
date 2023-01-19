<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="MettreGroupeDansClasse.aspx.cs" Inherits="MettreGroupeDansClasse" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:ToolkitScriptManager ID="toolkit1" runat="server"></asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
        <div class='row'>
            <div class='col-lg-12'><h1 class='page-header'>Enregistrement d'un Groupe d'Etudiants</h1></div>
        </div>
        <div class='row'>
            <div class='col-lg-12'>
                <div class='panel panel-default'>
                    <div class='panel-heading'>
                        <asp:Literal runat="server" ID="litTitle2">Choisissez le Cours pour Voir la Liste des Etudiants Eligibres</asp:Literal>
                    </div>
                </div>
            </div>
        </div>
        <div class='row'>
            <div class='col-lg-12'>
                <div class='panel panel-default'>
                    <asp:Button ID="btnSauvegarder" runat="server" Width="140" Text="Sauvegarder" class="btn btn-outline btn-primary" OnClick="btnSauvegarder_Click"/>
                    <%--<asp:Button ID="btnCancel" runat="server" Width="140" Text="Cancel" class="btn btn-outline btn-default" OnClick="btnCancel_Click"/>--%>
                    <asp:Label ID="lblErrorHead" runat="server" Font-Bold="True" ForeColor="#FF3300"></asp:Label>

                </div>
            </div>
        </div>
        <div class='row'>
            <div class='col-lg-12'>
                <asp:GridView ID="gvCoursDisponibles" runat="server" DataKeyNames="CoursOffertID" AutoGenerateColumns="False" 
                ShowFooter="false" AllowPaging="false" class='table table-striped table-bordered table-hover' 
                width='100%' EnablePersistedSelection="True" ShowHeaderWhenEmpty="True" AutoGenerateSelectButton="True" 
                 OnSelectedIndexChanged="gvCoursDisponibles_SelectedIndexChanged">                                       
                    <Columns>
                        <asp:TemplateField HeaderText="Choix" ItemStyle-CssClass="center">
                            <ItemTemplate>
                            <asp:CheckBox ID="chkCours" runat="server" Enabled="False"></asp:CheckBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField  HeaderText="Numéro">
                            <ItemTemplate>
                                <asp:Label ID="lblNumeroCours" runat="server" Text='<%# Bind("NumeroCours") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField  HeaderText="Description">
                            <ItemTemplate>
                                <asp:Label ID="lblNomCours" runat="server" Text='<%# Bind("NomCours") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField  HeaderText="Note Min">
                            <ItemTemplate>
                                <asp:Label ID="lblNotePassage" runat="server" Text='<%# Bind("NotePassage") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle BackColor="#003300" Font-Bold="True" Font-Size="Medium" ForeColor="White" />
                </asp:GridView>
            <%--</div>--%>
            <%--<div class='col-lg-7'>--%>
                <h4><asp:Label ID="lblNomCours" runat="server"></asp:Label></h4>
                <asp:GridView ID="gvEtudiants" runat="server" DataKeyNames="PersonneID" AutoGenerateColumns="False" 
                    AllowPaging="false" class='table table-striped table-bordered table-hover' 
                        width='100%' ShowHeaderWhenEmpty="True">                           
                    <RowStyle CssClass="odd gradeX" />
                    <Columns>
                        <asp:TemplateField HeaderText="Choix" ItemStyle-CssClass="center">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkEtudiant" CommandName="SelectEtudiant" runat="server"></asp:CheckBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField  HeaderText="Nom">
                            <ItemTemplate>
                                <asp:Label ID="lblNom" runat="server" Text='<%# Bind("Nom") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField  HeaderText="Prenom">
                            <ItemTemplate>
                                <asp:Label ID="lblPrenom" runat="server" Text='<%# Bind("Prenom") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>                           
                        <asp:TemplateField  HeaderText="Téléphone">
                            <ItemTemplate>
                                <asp:Label ID="lblTelephone" runat="server" Text='<%# Bind("Telephone1") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField  HeaderText="">
                            <ItemTemplate>
                                <asp:Label ID="lblPersonneID" runat="server" Text='<%# Bind("PersonneID") %>' Visible="false"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle BackColor="#003300" Font-Bold="True" Font-Size="Medium" ForeColor="White" />
                </asp:GridView>
            </div>
        </div>
        <div class='row'>
            <div class='col-lg-12'>
                <asp:Label ID="lblError" runat="server" Font-Bold="True" ForeColor="#FF3300"></asp:Label> 
            </div>
        </div>
        <div class='row'>
            <div class='col-lg-6'>
                <asp:Button ID="Button1" runat="server" Width="140" Text="Sauvegarder" class="btn btn-outline btn-primary" OnClick="btnSauvegarder_Click"/>
            </div>

            <div class='col-lg-6'>
                <asp:Button ID="btnSaveGroupe" runat="server" Width="140" Text="SauvegarderGroupe" class="btn btn-outline btn-primary"/>
            </div>

            <div class='col-lg-6'>
                <asp:Button ID="Button2" runat="server" Width="140" Text="Cancel" class="btn btn-outline btn-default" OnClick="btnCancel_Click"/>                       
            </div>
        </div>
    </ContentTemplate>
    </asp:UpdatePanel>
    <!-- For Busy Loading image -->
    <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" runat="server" style="align-content:center;">
    <!-- div class="loading" style="align-content:center;visibility:hidden" -->
        <asp:Panel Visible="false" runat="server" class="modal fade" id="myHiddenPanel" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" style="align-content:center;">
            <img src="images/loader.gif" alt="" />
        </asp:Panel>
    </div>

</asp:Content>


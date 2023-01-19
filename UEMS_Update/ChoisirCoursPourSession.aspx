<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="ChoisirCoursPourSession.aspx.cs" Inherits="AdminPages_ChoisirCoursPourSession" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class='row'><div class='col-lg-12'><h1 class='page-header'>Choisissez les Cours à Offrir</h1></div></div>
    <div class='row'>
    <div class='col-lg-8'>
        <asp:Label ID="lblError" runat="server" Font-Bold="True" ForeColor="#FF3300"></asp:Label>
    </div>
</div>
    <div class='row'>
        <div class='col-lg-3'>
            <div class='panel panel-default'>
                <div class='panel-heading'>
                     Liste des Cours Offerts
                </div>
                <div class='panel-body'>
                    <asp:GridView ID="gvCoursDejaOfferts" runat="server" DataKeyNames="NumeroCours" AutoGenerateColumns="False" 
                        ShowFooter="false" AllowPaging="false" class='table table-striped table-bordered table-hover'
                        EnablePersistedSelection="True" ShowHeaderWhenEmpty="True" width='100%' >                           
                        <Columns>
                            <asp:TemplateField  HeaderText="Cours Déjà Offerts">
                                <ItemTemplate>
                                    <asp:Label ID="lblDescription" runat="server" Text='<%# Bind("Description") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            </Columns>
                        <HeaderStyle BackColor="#003300" Font-Bold="True" Font-Size="Medium" ForeColor="White" />
                      </asp:GridView>
                </div>
            </div>
        </div>
        <div class='col-lg-9'>
            <div class='panel panel-default'>
                <div class='panel-heading'>
                     Offrez d'Autres Cours
                </div>
                <div class='panel-body'>
                   <asp:ToolkitScriptManager ID="toolkit1" runat="server"></asp:ToolkitScriptManager>
                   <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                   <ContentTemplate>
                      <asp:GridView ID="gvCoursDisponibles" runat="server" DataKeyNames="NumeroCours" AutoGenerateColumns="False" 
                        ShowFooter="false" AllowPaging="false" class='table table-striped table-bordered table-hover'
                        EnableSortingAndPagingCallbacks="True" PageSize="12" width='100%' EnablePersistedSelection="True" ShowHeaderWhenEmpty="True" OnRowDataBound="gvCoursDisponibles_RowDataBound">                           
                        <Columns>
                            <asp:TemplateField HeaderText="Choix" ItemStyle-CssClass="center">
                                <ItemTemplate>
                                <asp:CheckBox ID="chkCours" runat="server" Enabled="true"></asp:CheckBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField  HeaderText="Numéro">
                                <ItemTemplate>
                                    <asp:Label ID="lblNumeroCours" runat="server" Text='<%# Bind("NumeroCours") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField  HeaderText="Description (Pré-Requis)">
                                <ItemTemplate>
                                    <asp:Label ID="lblNomCours" runat="server" Text='<%# Bind("NomCours") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField  HeaderText="Note de Passage Sur Cent">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtNotePassage" Width="40px" runat="server"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField  HeaderText="Professeur">
                                <ItemTemplate>
                                    <asp:DropDownList ID="ddlProfesseurs" runat="server" DataValueField="ProfesseurID" DataTextField="NomComplet"></asp:DropDownList>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField  HeaderText="Heures">
                                <ItemTemplate>
                                    <asp:DropDownList ID="ddlHoraire" runat="server" DataValueField="HoraireID" DataTextField="Heures"></asp:DropDownList>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <HeaderStyle BackColor="#003300" Font-Bold="True" Font-Size="Medium" ForeColor="White" />
                      </asp:GridView>
                  </ContentTemplate>
                  </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
<div class='row'>
    <div class='col-lg-4'>
        <asp:Button ID="btnSauvegarder" runat="server" Width="140" Text="Sauvegarder" class="btn btn-outline btn-primary" OnClick="btnSauvegarder_Click"/>
    </div>
    <div class='col-lg-4'>
        <asp:Button ID="btnCancel" runat="server" Width="140" Text="Cancel" class="btn btn-outline btn-default" OnClick="btnCancel_Click"/>        
    </div>
</div>
    <div class='row'>
    <div class='col-lg-8'>
        
    </div>
</div>
<%--    <asp:SqlDataSource ID="SqlDataSourceProf" runat="server" ConnectionString="uespoir_connectionString" 
         SelectCommand="SELECT Prenom + ' ' + Nom AS NomComplet, ProfesseurID FROM Professeurs;"></asp:SqlDataSource>--%>
</asp:Content>


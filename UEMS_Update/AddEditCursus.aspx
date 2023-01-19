<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="AddEditCursus.aspx.cs" Inherits="AddEditCursus" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class='row'><div class='col-lg-12'><h1 class='page-header'>Choisissez les Cours Pour la Discipline</h1></div></div>
    <div class='row'>
        <div class='col-lg-8'>
            <asp:Label ID="lblError" runat="server" Font-Bold="True" ForeColor="#FF3300"></asp:Label>
        </div>
    </div>
    <div class='row'>
        <div class='col-lg-3'>
            <asp:DropDownList ID="ddlDisciplines" runat="server" DataValueField="DisciplineID" DataTextField="Description" Font-Size="Large" AutoPostBack="True" OnSelectedIndexChanged="ddlDisciplines_SelectedIndexChanged"></asp:DropDownList>
        </div>
        <div class='col-lg-3'>
        </div>
        <div class='col-lg-3'>
            <asp:Button ID="btnSauvegarder" runat="server" Width="140" Text="Sauvegarder" class="btn btn-outline btn-primary" OnClick="btnSauvegarder_Click"/>
        </div>
        <div class='col-lg-3'>
            <asp:Button ID="btnCancel" runat="server" Width="140" Text="Cancel" class="btn btn-outline btn-default" OnClick="btnCancel_Click"/>                    
        </div>
    </div>
    <div class='row'>
        <hr />
    </div>
    <div class='row'>
        <div class='col-lg-5'>
            <div class='panel panel-default'>
                <div class='panel-heading'>
                     <h4>Liste des Cours à Prendre</h4>
                </div>
                <div class='panel-body'>
                    <asp:GridView ID="gvCoursDejaOfferts" runat="server" DataKeyNames="NumeroCours" AutoGenerateColumns="False" 
                        ShowFooter="false" AllowPaging="False" class='table table-striped table-bordered table-hover'
                        EnablePersistedSelection="True" ShowHeaderWhenEmpty="True" width='100%' AllowCustomPaging="False" >                           
                        <Columns>
                            <asp:TemplateField  HeaderText="Cours">
                                <ItemTemplate>
                                    <asp:Label ID="lblNomCours" runat="server" Text='<%# Bind("NomCours") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField  HeaderText="Numéro">
                                <ItemTemplate>
                                    <asp:Label ID="lblNumero" runat="server" Text='<%# Bind("NumeroCours") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField  HeaderText="Période">
                                <ItemTemplate>
                                    <asp:Label ID="lblSession" runat="server" Text='<%# Bind("NumeroSession") %>'></asp:Label>                                      
                                </ItemTemplate>
                            </asp:TemplateField>
                            </Columns>
                      </asp:GridView>
                </div>
            </div>
        </div>
        <div class='col-lg-7'>
            <div class='panel panel-default'>
                <div class='panel-heading'>
                     <h4>Offrez d'Autres Cours</h4>
                </div>
                <div class='panel-body'>
                   <asp:ToolkitScriptManager ID="toolkit1" runat="server"></asp:ToolkitScriptManager>
                   <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                   <ContentTemplate>
                      <asp:GridView ID="gvCoursDisponibles" runat="server" DataKeyNames="NumeroCours" 
                          CssClass='table table-striped table-bordered table-hover' AllowPaging="False" 
                          OnRowDataBound="gvCoursDisponibles_RowDataBound" AutoGenerateColumns="False" AllowCustomPaging="False">                           
                        <Columns>
                            <asp:TemplateField HeaderText="Choix" ItemStyle-CssClass="center">
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkCours" runat="server" Enabled="true"></asp:CheckBox>
                                </ItemTemplate>
                                <ItemStyle CssClass="center" />
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
                            <asp:TemplateField  HeaderText="Période (# Session)">
                                <ItemTemplate>
                                    <asp:DropDownList ID="ddlPeriode" runat="server" DataValueField="NumeroSession" DataTextField="Periode"></asp:DropDownList>  
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>                       
                      </asp:GridView>
                  </ContentTemplate>
                  </asp:UpdatePanel>
                </div>
            </div>
        </div>
        <div class='row'>
            <div class='col-lg-6'></div>
            <div class='col-lg-6'>
                <asp:Button ID="Button1" runat="server" Width="140" Text="Sauvegarder" class="btn btn-outline btn-primary" OnClick="btnSauvegarder_Click"/>
            </div>
        </div>
    </div>
    <div class='row'>
        <hr />
    </div>
    <div class='row'>

    </div>
    <div class='row'>
        <hr />
    </div>


</asp:Content>




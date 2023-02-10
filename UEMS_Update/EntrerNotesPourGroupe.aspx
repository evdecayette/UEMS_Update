<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="EntrerNotesPourGroupe.aspx.cs" Inherits="EntrerNotesPourGroupe" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:ToolkitScriptManager ID="toolkit1" runat="server"></asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
        <div class='row'>
            <div class='col-lg-12'>
               <h1 class='page-header'>Enregistrement des Résultats d'un Examen Final</h1>
            </div>
        </div>
        <div class='row'>
            <div class='col-lg-12'><h3>La Liste est Vide s'il n'y a pas d'Etudiants INSCRITS dans le Cours Choisi</h3></div>
        </div>
        <div class='row'>
            <div class='col-lg-6'>
                <asp:DropDownList ID="ddlSessions" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSessions_SelectedIndexChanged" Height="25px" Width="250px"></asp:DropDownList>
            </div>
            <div class='col-lg-6'>
                <asp:DropDownList ID="ddlCours" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlCours_SelectedIndexChanged" Height="25px" Width="250px"></asp:DropDownList>
            </div>
        </div>
        <div class='row'>
            <div class='col-lg-12'>-</div>
        </div>
        <div class='row'>
            <div class='col-lg-12'>
                <asp:GridView ID="gvEtudiants" runat="server" DataKeyNames="PersonneID" AutoGenerateColumns="False" 
                    ShowFooter="false" AllowPaging="false" class='table table-striped table-bordered table-hover' 
                    EnablePersistedSelection="True" ShowHeaderWhenEmpty="True" width='100%' 
                    OnRowDataBound="gvEtudiants_RowDataBound"
                    OnPageIndexChanging="gvEtudiants_PageIndexChanging"
                    OnRowCancelingEdit="gvEtudiants_RowCancelingEdit"
                    OnRowEditing="gvEtudiants_RowEditing" 
                    OnRowUpdating="gvEtudiants_RowUpdating">
                            <Columns>
                                <asp:TemplateField HeaderText="Action" ItemStyle-CssClass="center">
                                    <EditItemTemplate>
                                        <asp:Button ID="ButtonUpdate" runat="server" Width="48%" class="btn btn-success m-5" CommandName="Update"  Text="Update"  />
                                        <asp:Button ID="ButtonCancel" runat="server" Width="48%" class="btn btn-warning" CommandName="Cancel"  Text="Cancel" />
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:Button ID="ButtonEdit" runat="server" CommandName="Edit" class="btn btn-info" Text="Editez la Note"  />
                                    </ItemTemplate>
                                   
                                 </asp:TemplateField>

                                <asp:TemplateField HeaderText="Note sur 100" ItemStyle-CssClass="center">
                                    <ItemTemplate>
                                    <asp:TextBox ID="txtNote" runat="server" Text='<%# Bind("NoteSurCent") %>'></asp:TextBox>
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

                                <asp:TemplateField  HeaderText="NIF/Matricule">
                                    <ItemTemplate>
                                        <asp:Label ID="lblNIF" runat="server" Text='<%# Bind("Nif") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField  HeaderText="Date Naissance">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDateNaissance" runat="server" Text='<%# Bind("Email") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField  HeaderText="">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCoursPrisID" Width="0%" runat="server" Text='<%# Bind("CoursPrisID") %>' Visible="false"></asp:Label>
                                        <asp:Label ID="lblPersonneID" Width="0%" runat="server" Text='<%# Bind("PersonneID") %>' Visible="false"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <HeaderStyle BackColor="#003300" Font-Bold="True" Font-Size="Medium" ForeColor="White" />
                        </asp:GridView>
                    </div>
            </div>
            <div class='row'>
                <div class='col-lg-6'>
                    <asp:Label ID="lblError" runat="server" Font-Bold="True" ForeColor="#FF3300"></asp:Label> 
                </div>
            </div>
            <div class='row'>
                <div class='col-lg-6'>
                    <asp:Button ID="btnSauvegarder" runat="server" Width="140" Text="Sauvegarder" class="btn btn-outline btn-primary" OnClick="btnSauvegarder_Click"/>
                </div>
                <div class='col-lg-6'>
                    <asp:Button ID="btnCancel" runat="server" Width="140" Text="Cancel" class="btn btn-outline btn-default" OnClick="btnCancel_Click"/>                       
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="EditNotePasseeEtudiant.aspx.cs" Inherits="EditNotePasseeEtudiant" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
        <!-- DataTables CSS -->
    <link href="vendor/datatables-plugins/dataTables.bootstrap.css" rel="stylesheet"/>
    <!-- DataTables Responsive CSS -->
    <link href="vendor/datatables-responsive/dataTables.responsive.css" rel="stylesheet"/>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
       <div class='row'><div class='col-lg-12'><h1 class='page-header'>Corriger Résultat Final</h1>
       </div></div><div class='row'><div class='col-lg-12'><div class='panel panel-default'><div class='panel-heading'>
       La Liste est Vide si l'Etudiant n'a pas de cours avec une Note Manquante
       </div><div class='panel-body'>

<asp:ToolkitScriptManager ID="toolkit1" runat="server"></asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table style="width:100%">
                <tr>
                    <td><h4 class="page-header">Etudiant:  <asp:Label ID="txtNomComplet_Header" runat="server"> </asp:Label></h4></td>
                    <td><asp:TextBox ID="txtPersonneID" runat="server" Visible="false"></asp:TextBox></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:GridView ID="gvNotesManquantes" runat="server" DataKeyNames="CoursPrisID" AutoGenerateColumns="False" 
                            ShowFooter="false" AllowPaging="false" class='table table-striped table-bordered table-hover'
                            EnableSortingAndPagingCallbacks="True" PageSize="12" width='100%' >                           
                            <RowStyle CssClass="odd gradeX" />
                            <Columns>
                                <asp:TemplateField HeaderText="Note sur 100" ItemStyle-CssClass="center">
                                    <ItemTemplate>
                                    <asp:TextBox ID="txtNote" runat="server" Text='<%# Bind("NoteSurCent") %>'></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField  HeaderText="Numero Cours">
                                    <ItemTemplate>
                                        <asp:Label ID="lblNom" runat="server" Text='<%# Bind("NumeroCours") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField  HeaderText="Cours Offert ID">
                                    <ItemTemplate>
                                        <asp:Label ID="lblPrenom" runat="server" Text='<%# Bind("CoursOffertID") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            
                                <asp:TemplateField  HeaderText="Minimum Note de Passage">
                                    <ItemTemplate>
                                        <asp:Label ID="lblTelephone" runat="server" Text='<%# Bind("NotePassage") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField  HeaderText="">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCoursPrisID" runat="server" Text='<%# Bind("CoursPrisID") %>' Visible="false"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <HeaderStyle BackColor="#003300" Font-Bold="True" Font-Size="Medium" ForeColor="White" />
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="lblError" runat="server" Font-Bold="True" ForeColor="#FF3300"></asp:Label> 
                    </td>
                </tr>
                <tr><td colspan="2">...</td></tr>
                <tr>
                    <td>
                        <asp:Button ID="btnSauvegarder" runat="server" Width="140" Text="Sauvegarder" class="btn btn-outline btn-primary" OnClick="btnSauvegarder_Click"/>
                    </td>
                    <td>
                        <asp:Button ID="btnCancel" runat="server" Width="140" Text="Cancel" class="btn btn-outline btn-default" OnClick="btnCancel_Click"/>                       
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>

       </div></div></div></div>

</asp:Content>


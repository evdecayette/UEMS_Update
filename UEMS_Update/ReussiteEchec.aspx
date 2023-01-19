<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="ReussiteEchec.aspx.cs" Inherits="ReussiteEchec" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
        <!-- DataTables CSS -->
    <link href="vendor/datatables-plugins/dataTables.bootstrap.css" rel="stylesheet"/>
    <!-- DataTables Responsive CSS -->
    <link href="vendor/datatables-responsive/dataTables.responsive.css" rel="stylesheet"/>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
       <div class='row'><div class='col-lg-12'><h1 class='page-header'>Enregistrement des Résultats d'Examens d'Entrée</h1>
       </div></div><div class='row'><div class='col-lg-12'><div class='panel panel-default'><div class='panel-heading'>
       La Liste est Vide s'il n'y a pas d'Etudiants qui attendent les résultats de l'Examen d'Entrée (Français)
       </div><div class='panel-body'>



<asp:ToolkitScriptManager ID="toolkit1" runat="server"></asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table style="width:100%">
                <tr>
                    <td>
                        <asp:Button ID="btnSauvegarder" runat="server" Width="140" Text="Sauvegarder" class="btn btn-outline btn-primary" OnClick="btnSauvegarder_Click"/>
                    </td>
                    <td>
                        <%--<asp:Button ID="btnCancel" runat="server" Width="140" Text="Cancel" class="btn btn-outline btn-default" OnClick="btnCancel_Click"/>                       --%>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:GridView ID="gvEtudiants" runat="server" DataKeyNames="PersonneID" AutoGenerateColumns="False" 
                            ShowFooter="false" AllowPaging="false" class='table table-striped table-bordered table-hover' 
                             width='100%' >                           
                            <RowStyle CssClass="odd gradeX" />
                            <Columns>
                                <asp:TemplateField HeaderText="Note sur 100" ItemStyle-CssClass="center">
                                    <ItemTemplate>
                                    <asp:TextBox ID="txtNote" runat="server"></asp:TextBox>
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
                                        <asp:Label ID="lblDateNaissance" runat="server" Text='<%# Bind("DDN") %>'></asp:Label>
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
                <tr>
                    <td>
                        <asp:Button ID="Button1" runat="server" Width="140" Text="Sauvegarder" class="btn btn-outline btn-primary" OnClick="btnSauvegarder_Click"/>
                    </td>
                    <td>
                        
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>





       </div></div></div></div>

</asp:Content>


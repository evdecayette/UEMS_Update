<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Enregistrement.aspx.cs" Inherits="Enregistrement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <!-- DataTables CSS -->
    <link href="vendor/datatables-plugins/dataTables.bootstrap.css" rel="stylesheet"/>
    <!-- DataTables Responsive CSS -->
    <link href="vendor/datatables-responsive/dataTables.responsive.css" rel="stylesheet"/>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class='row'><div class='col-lg-12'><h1 class='page-header'>Enregistrement à un Cours</h1></div></div>

    <div class='row'><div class='col-lg-12'><div class='panel panel-default'><div class='panel-heading'>
        Les Erreurs sont affiché en bas
    </div><div class='panel-body'>
<!-- Begin code for this page -->
    <div class='row'><div class='col-lg-8'>
        <asp:GridView ID="gvEtudiants" runat="server" DataKeyNames="PersonneID" AutoGenerateColumns="False"  width='100%' class='table table-striped table-bordered table-hover' > 
            <Columns>
                <asp:TemplateField HeaderText="Nom">
                    <ItemTemplate >
                        <asp:Label ID="lblNom" runat="server" class="form-group" Text='<%# Bind("Nom") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Prenom">
                    <ItemTemplate>
                        <asp:Label ID="lblPayerA" class="form-group" runat="server" Text='<%# Bind("Prenom") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Date Naissance">
                    <ItemTemplate>
                        <asp:Label ID="lblDate" runat="server" class="form-group" Text='<%# Bind("DDN") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Telephone">
                    <ItemTemplate>
                        <asp:Label ID="lblDate" runat="server" class="form-group" Text='<%# Bind("Telephone1") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="NIF/Matricule">
                    <ItemTemplate>
                        <asp:Label ID="lblDate" runat="server" class="form-group" Text='<%# Bind("NIF") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
       </asp:GridView>
    </div>
    <div class='col-lg-4'>
                <asp:GridView ID="gvCours" runat="server" DataKeyNames="NumeroCours" AutoGenerateColumns="False"  width='100%' class='table table-striped table-bordered table-hover' > 
            <Columns>
                <asp:TemplateField HeaderText="Numero Cours">
                    <ItemTemplate >
                        <asp:Label ID="lblNom" runat="server" class="form-group" Text='<%# Bind("NumeroCours") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Prenom">
                    <ItemTemplate>
                        <asp:Label ID="lblPayerA" class="form-group" runat="server" Text='<%# Bind("CoursPreRequis") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Date Naissance">
                    <ItemTemplate>
                        <asp:Label ID="lblDate" runat="server" class="form-group" Text='<%# Bind("Periode") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
       </asp:GridView>

    </div>






        

            <div class="form-group">
                <asp:Label ID="lblError" runat="server" Text=""></asp:Label>
            </div>     


<!-- End code for this page -->
                </div>
            </div>
       </div>
    </div>

</asp:Content>

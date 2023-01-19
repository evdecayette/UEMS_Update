<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="AddEditObligations.aspx.cs" Inherits="AdminPages_AddEditObligations" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class='row'><div class='col-lg-12'><h3 class='page-header'>Ajouter / Editer Nouvelles Obligations</h3></div></div>
    <div class='row'>
      <div class='col-lg-12'>
        <div class='panel panel-default'>
            <div class='panel-heading'>Exemples: Frais d'Inscriptin - Mensualité - et. (Les Erreurs sont affiché en bas)</div>
                <div class='panel-body'>
<!-- Begin code for this page -->
                    <div class="row">
                        <div class="col-lg-6">
                            <div class="form-group">
                                <asp:TextBox ID="txtCode" runat="server" Width="200" class="form-control" placeholder="Entrer le Code de 2 Lettres"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <asp:TextBox ID="txtMontant" runat="server" Width="200" class="form-control" placeholder="Entrer le Montant"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <asp:TextBox ID="txtDescription" runat="server"  class="form-control" placeholder="Entrer la Description"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <asp:TextBox ID="txtAutreDescription" runat="server"  class="form-control" placeholder="Entrer autre Explication" TextMode="MultiLine"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-lg-6">
                            <div class="form-group">
                                <asp:ListBox ID="lstObligationsExistantes" Height="200" runat="server" class="form-control" OnSelectedIndexChanged="lstObligationsExistantes_SelectedIndexChanged" AutoPostBack="True"></asp:ListBox>
                            </div>
                        </div>
                        <!-- /.col-lg-6 (nested) -->
                    </div>
                    <!-- /.row (nested) -->
                    <div class="row">
                        <div class="col-lg-6">
                            <div class="form-group">
                                <asp:Label ID="lblError" runat="server" Font-Bold="True" ForeColor="#FF3300"></asp:Label> 
                            </div>
                        </div>
                        <!-- /.col-lg-6 (nested) -->
                    </div>
                    <!-- /.row (nested) -->
                    <div class="row">
                        <div class="col-lg-2">                                
                            <asp:Button ID="btnSauvegarder" runat="server" Width="140" Text="Sauvegarder" class="btn btn-outline btn-primary" OnClick="btnSauvegarder_Click"/>
                        </div>
                        <div class="col-lg-2">
                            <asp:Button ID="btnCancel" runat="server" Width="140" Text="Cancel" class="btn btn-outline btn-default" OnClick="btnCancel_Click"/>
                        </div>
                        <!-- /.col-lg-6 (nested) -->
                    </div>
                    <!-- /.row (nested) -->
<!-- End code for this page -->
                </div>
            </div>
       </div>
    </div>

</asp:Content>


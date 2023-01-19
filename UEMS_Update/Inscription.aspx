<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Inscription.aspx.cs" Inherits="Inscription" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:ToolkitScriptManager ID="toolkit1" runat="server"></asp:ToolkitScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
        <ContentTemplate>
        <div class='row'><div class='col-lg-12'><h1 class='page-header'>Ajouter Un Nouveau Etudiant</h1></div></div>
            <div class='row'><div class='col-lg-12'>
                <div class='panel panel-default'>
                    <div class='panel-heading'>
                        Tout Nouveau Etudiant est mis dans les Cours FRA001, MAT001 et ANG001 (Examens d'Entrée Français, Math et Anglais)
                    </div>
                        <div class='panel-body'>
                            <div class="row">
                                <div class="col-lg-4">
                                    <div class="form-group">
                                        <asp:TextBox ID="txtPrenom" runat="server" autocomplete="off" class="form-control" placeholder="Entrer les Prénoms" MaxLength="40" AutoPostBack="True" OnTextChanged="txtPrenom_TextChanged"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <asp:TextBox ID="txtNom" runat="server" autocomplete="off" class="form-control" placeholder="Entrer le Nom de Famille" MaxLength="40" AutoPostBack="True" OnTextChanged="txtNom_TextChanged"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <asp:TextBox ID="txtAdresseRue" runat="server" autocomplete="off" class="form-control" placeholder="Entrer La Rue et le Numéro"  MaxLength="48"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <asp:TextBox ID="txtAdresseExtra" runat="server" autocomplete="off" class="form-control" placeholder="Adresse Extra" MaxLength="48"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <asp:TextBox ID="txtVille" runat="server" autocomplete="off" class="form-control" placeholder="Ville" MaxLength="48"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <asp:TextBox ID="txtPays" runat="server" autocomplete="off" class="form-control" placeholder="Pays - Haiti par default si laissé vide" MaxLength="30"></asp:TextBox>
                                    </div>
                                </div>
                                <!-- /.col-lg-4 (nested) -->
                                <div class="col-lg-4">
                                    <div class="form-group">
                                        <asp:TextBox ID="txtEmail" runat="server" autocomplete="off" class="form-control" placeholder="Entrer l'Adresse Email" TextMode="Email"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <asp:TextBox ID="txtDDN" runat="server" autocomplete="off" class="form-control" placeholder="Entrer la Date de Naissance" TextMode="Date" ToolTip="mm/jj/aaaa"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <asp:TextBox ID="txtTelephone1" runat="server" autocomplete="off" class="form-control" placeholder="Entrer le # de téléphone principal" TextMode="Phone"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <asp:TextBox ID="txtTelephone2" runat="server" autocomplete="off" class="form-control" placeholder="Autre # de téléphone" TextMode="Phone"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <asp:TextBox ID="txtNIF" runat="server" autocomplete="off" class="form-control" placeholder="Entrer le # du NIF ou Matricule Fiscale" MaxLength="48"></asp:TextBox>
                                    </div>

                                </div>
                                <!-- /.col-lg-4 (nested) -->
                                <!-- /.col-lg-4 (nested) -->
                                <div class="col-lg-4">
                                    <div class="form-group">
                                        <label>Discipline Initiale</label>
                                        <asp:DropDownList ID="ddlDisciplines" runat="server" class="form-control" AutoPostBack="True" CausesValidation="True" OnSelectedIndexChanged="ddlDisciplines_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                    <div class="form-group">
                                        <label>Remarque</label>                       
                                        <asp:TextBox ID="txtRemarque" runat="server"  class="form-control" TextMode="MultiLine"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <asp:TextBox ID="txtPersonneID" runat="server"  Visible="false"></asp:TextBox>
                                    </div>
                                </div>
                                <!-- /.col-lg-4 (nested) -->
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

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>


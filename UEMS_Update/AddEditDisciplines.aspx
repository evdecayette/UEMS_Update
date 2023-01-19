<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="AddEditDisciplines.aspx.cs" Inherits="AdminPages_AddEditDisciplines" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class='row'><div class='col-lg-12'><h1 class='page-header'>Ajouter Un Nouveau Cours</h1></div></div>
    <div class='row'><div class='col-lg-12'>
        <div class='panel panel-default'>
            <div class='panel-heading'>
                Choix d'Offrir ce Cours cette Session (Ceci n'est pas automatique)
            </div>
                <div class='panel-body'>
                    <div class="row">
                        <div class="col-lg-4">
                            <div class="form-group">
                                <asp:TextBox ID="txtPrenom" runat="server"  class="form-control" placeholder="Entrer les Prénoms"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <asp:TextBox ID="txtNom" runat="server"  class="form-control" placeholder="Entrer le Nom de Famille"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <asp:TextBox ID="txtAdresseRue" runat="server"  class="form-control" placeholder="Entrer La Rue et le Numéro"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <asp:TextBox ID="txtAdresseExtra" runat="server"  class="form-control" placeholder="Adresse Extra"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <asp:TextBox ID="txtVille" runat="server"  class="form-control" placeholder="Ville"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <asp:TextBox ID="txtPays" runat="server"  class="form-control" placeholder="Pays - Haiti par default si laissé vide"></asp:TextBox>
                            </div>
                        </div>
                        <!-- /.col-lg-4 (nested) -->
                        <div class="col-lg-4">
                            <div class="form-group">
                                <asp:TextBox ID="txtEmail" runat="server"  class="form-control" placeholder="Entrer l'Adresse Email"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <asp:TextBox ID="txtDDN" runat="server"  class="form-control" placeholder="Entrer la Date de Naissance"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <asp:TextBox ID="txtTelephone1" runat="server"  class="form-control" placeholder="Entrer le # de téléphone principal"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <asp:TextBox ID="txtTelephone2" runat="server"  class="form-control" placeholder="Autre # de téléphone"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <asp:TextBox ID="txtNIF" runat="server"  class="form-control" placeholder="Entrer le # du NIF ou Matricule Fiscale"></asp:TextBox>
                            </div>

                        </div>
                        <!-- /.col-lg-4 (nested) -->
                    </div>
                    <!-- /.row (nested) -->                                            

                </div>
        </div>
    </div>
</div>
</asp:Content>


<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="InfoEtudiant.aspx.cs" Inherits="InfoEtudiant" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

        <div class="row">
                <div class="col-lg-12">
                    <h4 class="page-header">Information Générale sur l'Etudiant: <asp:Label ID="txtNomComplet_Header" runat="server"> </asp:Label></h4>                   
                    <asp:Button ID="btnEditNotePasseeEtudiant" runat="server" Width="150" Text="Entrer Note Finale" class="btn btn-outline btn-primary" OnClick="btnEditNotePasseeEtudiant_Click" />
                    <asp:Button ID="btnReleveNotes" runat="server" Width="150" Text="Relevé de Notes" class="btn btn-outline btn-primary" OnClick="btnReleveNotes_Click"/>
                    <asp:Button ID="btnHoraireSessionCourante" runat="server" Width="150" Text="Horaire Session Courante" class="btn btn-outline btn-primary" OnClick="btnHoraireSessionCourante_Click" />
                    <asp:Button ID="btnProgresGraduation" runat="server" Width="150" Text="Progrès Graduation" data-loading-text="Patientez svp..." class="btn btn-primary js-loading-button" OnClick="btnProgresGraduation_Click"/>
                    <asp:Button ID="btnFactureManuelle" runat="server" Width="150" Text="Facture Manuelle" class="btn btn-outline btn-primary" OnClick="btnFactureManuelle_Click" />
                    <asp:Button ID="btnPayments" runat="server" Width="150" Text="Ecran Des Paiements" class="btn btn-outline btn-primary" OnClick="btnPayments_Click"/>
                    <asp:Button ID="btnEditer" runat="server" Width="150" Text="Editer Etudiant" class="btn btn-outline btn-primary" OnClick="btnEditer_Click" />
                    <asp:Button ID="btnReactiver" runat="server" Width="150" Text="Réactiver Etudiant" class="btn btn-outline btn-primary" Visible="false" OnClick="btnReactiver_Click"/>
                    <asp:TextBox ID="txtPersonneID" runat="server" Visible="false"></asp:TextBox>
                    <hr />
                </div>              
            </div>
            <!-- /.col-lg-12 -->
            <div class="row">
                <div class="col-lg-5">
                    <div class="panel panel-yellow">
                        <div class="panel-heading">
                            <asp:Label ID="lblCoursPrisHeader" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="panel-body">
                            <p><asp:Literal ID="LitCoursPris" runat="server"></asp:Literal></p>
                        </div>
                        <div class="panel-footer">
                            <p><asp:Literal ID="LitCoursPrisFooter" runat="server"></asp:Literal></p>
                        </div>
                    </div>
                    <div class="panel panel-yellow">
                        <div class="panel-heading">
                            <asp:Label ID="lblExamenEntreesHeader" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="panel-body">
                            <p><asp:Literal ID="LitExamenEntrees" runat="server"></asp:Literal></p>
                        </div>
                        <div class="panel-footer">
                            <p><asp:Literal ID="lblExamenEntreesFooter" runat="server"></asp:Literal></p>
                        </div>
                    </div>
                </div>
                <div class="col-lg-6">
                    <div class="panel panel-red">
                        <div class="panel-heading">
                            <asp:Label ID="lblCoursEligiblesHeader" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="panel-body">
                            <p><asp:Literal ID="LitCoursEligibles" runat="server"></asp:Literal></p>
                        </div>
                        <div class="panel-footer">
                            <p><asp:Literal ID="LitCoursEligiblesFooter" runat="server"></asp:Literal></p>
                        </div>
                    </div>
                </div>
            </div>
            <!-- /.row -->


</asp:Content>


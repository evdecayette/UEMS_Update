<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="AddEditCours.aspx.cs" Inherits="AdminPages_AddEditCours" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="css/formItems.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:ToolkitScriptManager ID="toolkit1" runat="server"></asp:ToolkitScriptManager>
    <asp:Timer runat="server" ID="Timer1" Interval="500" Enabled="false" ontick="Timer1_Tick" />
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="Timer1" EventName="Tick" />
        </Triggers>
            <ContentTemplate>
                    <div class='row'><div class='col-lg-12'><h3 class='page-header'>Ajouter Un Nouveau Cours</h3></div></div>
                    <div class='row'>
                        <div class='col-lg-12'>
                          <div class='panel panel-default'>
                            <div class='panel-heading'>
                                Créer Un Nouveau Cours
                            </div>
                            <div class='panel-body'>
                                <div class="row">
                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <asp:TextBox ID="txtNumeroCours" runat="server" Width="110px" class="form-control" placeholder="Numéro du Cours (AAA000)"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <asp:TextBox ID="txtNomCours" runat="server" Width="200px" class="form-control" placeholder="Nom Cours"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <asp:TextBox ID="txtCredits" runat="server" Text="3" class="form-control" placeholder="Credits" Width="90px" TextMode="Number" MaxLength="1"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <asp:TextBox ID="txtDescriptionCours" runat="server"  class="form-control" placeholder="Description du Cours"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-lg-6">
                                        <div class="form-group">
                                            <asp:DropDownList ID="ddlCoursPreRequis" runat="server" class="form-control" Height="30px" AutoPostBack="True" OnSelectedIndexChanged="ddlCoursPreRequis_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-5">
                                        <div class="form-group">
                                            <asp:CheckBox ID="chkExamenEntree" runat="server" Text=".&#32;&#32;Examen d'Entrée ou de Placement?" class="form-control" />
                                            <%--<label for="chkExamenEntree">.&#32;&#32;Examen d'Entrée ou de Placement</label>--%>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-10">
                                         <div class="form-group">
                                            <asp:Label ID="lblError" runat="server" Font-Bold="True" ForeColor="#FF3300"></asp:Label> 
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <asp:Button ID="btnSauvegarder" runat="server" Width="140" Height="60px" Text="Sauvegarder" class="btn btn-outline btn-primary" OnClick="btnSauvegarder_Click" />                                            
                                        </div>
                                    </div>
                                    <div class="col-lg-4">
                                        <asp:Button ID="btnCancel" runat="server" Width="140" Height="60px" Text="Cancel" class="btn btn-outline btn-default" OnClick="btnCancel_Click"/>
                                    </div>
                                    </div>
                                </div>                                                                 
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>


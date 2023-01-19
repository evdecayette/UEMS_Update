<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="SetupSession.aspx.cs" Inherits="AdminPages_SetupSession" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="../css/formItems.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:ToolkitScriptManager ID="toolkit1" runat="server">
        <Scripts>
            <asp:ScriptReference Name="jquery"/>
        </Scripts>
    </asp:ToolkitScriptManager>

    <div class='row'><div class='col-lg-12'><h1 class='page-header'>Fermer la Session Courante et Setup une Nouvelle Session</h1></div></div>
        <div class='row'>
            <div class='col-lg-12'>
            <div class='panel panel-default'>
                <div class='panel-heading'>
                    Etape # 1 (Cliquez les calendriers pour choisir les dates de la nouvelle Session)
                </div>
                    <div class='panel-body'>
                        <div class="row">
                            <div class="col-lg-6">
                                <div class="dynamicdate">
                                    <asp:TextBox autocomplete="off" ID="txtDateStart" runat="server" placeholder="Date (mm/jj/aaaa)"  Height="35px"></asp:TextBox>
                                    <label for="txtDateStart"><span></span>Date: Début de la Nouvelle Session</label>
                                    <asp:CalendarExtender ID="calExtender1" runat="server" TargetControlID="txtDateStart" PopupButtonID="txtAddDateStart" Format="MM/dd/yyyy" ></asp:CalendarExtender>
                                </div>
                                <div class="dynamicdate">
                                    <asp:TextBox autocomplete="off" ID="txtDateEnd" runat="server" placeholder="Date (mm/jj/aaaa)"  Height="35px"></asp:TextBox>                                                   
                                   <label for="txtDateEnd"><span></span>Date: Fin de la Nouvelle Session</label>
                                    <asp:CalendarExtender ID="calExtender2" runat="server" TargetControlID="txtDateEnd" PopupButtonID="txtAddDateEnd" Format="MM/dd/yyyy" ></asp:CalendarExtender>
                                </div>
      
                                <div class="dynamiclabel">
                                    <asp:TextBox ID="txtDescription" runat="server" placeholder="Description"  Height="35px"></asp:TextBox>                                                   
                                   <label for="txtDescription"><span></span>Description Obligatoire</label>                                    
                                </div>

                            </div>
                            <div class="col-lg-6">
                                <asp:Literal ID="litExistingSessions" runat="server"></asp:Literal>
                            </div>
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
                                <asp:Button ID="btnNext" runat="server" Width="140" Text="Suivant" class="btn btn-outline btn-primary" OnClick="btnNext_Click"/>
                            </div>
                            <div class="col-lg-2">
                                <asp:Button ID="btnCancel" runat="server" Width="140" Text="Annuler" class="btn btn-outline btn-default" OnClick="btnCancel_Click"/>
                            </div>                 
                        </div>
                    </div>
                </div>
            </div>
        </div>
</asp:Content>


<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="AddEditEvenements.aspx.cs" Inherits="AddEditEvenements" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <!-- DataTables CSS -->
    <link href="vendor/datatables-plugins/dataTables.bootstrap.css" rel="stylesheet"/>
    <!-- DataTables Responsive CSS -->
    <link href="vendor/datatables-responsive/dataTables.responsive.css" rel="stylesheet"/>
</asp:Content>

<asp:Content ID="ContentID" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:ToolkitScriptManager ID="toolkit1" runat="server"></asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
    <ContentTemplate>
        <div class='row'>
            <div class='col-lg-12'>                     <!-- Start of Big Row with 1 Column: Header -->
                <!-- <div class='panel panel-default'> -->
                    <div class='panel-heading'>
                        <h2>Ajout au Calendrier de UEspoir Management System</h2>
                    </div>
                <!-- </div> -->
            </div>
        </div>
        <!-- New Row -->
        <div class='row'>                              <!-- Start of Big Row with 2 Columns -->
            <!-- Start of First Column -->
            <div class='col-lg-4'>        
                <div class="form-group">
                    <asp:TextBox ID="Hidden_ID" runat="server" class="form-control" Visible="false"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Date</label>
                    <asp:TextBox ID="txtDate" runat="server" class="form-control" TextMode="Date" ToolTip="mm/jj/aaaa"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Remarque</label>
                    <asp:TextBox ID="txtDescription" runat="server" class="form-control" Height="300" TextMode="MultiLine"></asp:TextBox>
                </div>
                <div class="form-group">
                    <asp:Button ID="btnSauvegarder" runat="server" Width="140" Text="Sauvegarder" class="btn btn-outline btn-primary" OnClick="btnSauvegarder_Click" />
                </div>
                <div class="form-group">
                    <asp:Button ID="btnCancel" runat="server" Width="140" Text="Cancel" class="btn btn-outline btn-default" OnClick="btnCancel_Click" />
                </div>
                <div class="form-group">
                    <asp:Label ID="lblError" runat="server" Font-Bold="True" ForeColor="#FF3300"></asp:Label>
                </div>
            </div>                                      
            <!-- End of First Column -->
            <!-- Start of Second Column with a list of existing events-->
            <div class='col-lg-8'>                                    
                <asp:Literal ID="ContentLiteralID" runat="server">
                    <!-- Content built in Code -->
                </asp:Literal> 
            </div>                                      
            <!-- End of Second Column -->
        </div>                                        <!-- End of Big Row with 2 Columns -->
    </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
   
    <%--<div class='panel-body'>
        
    </div>--%>

<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
            <div class="row">
                <div class="col-lg-12">
                    <h2 class="page-header">Calendrier d'Evénements pour l'Année Scolaire</h2>
                </div>
                <!-- /.col-lg-12 -->
            </div>
            <!-- /.row -->
            <div class="row">
                <div class="col-lg-4">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <asp:Label ID="Label1_header" runat="server" Text=""></asp:Label> 
                        </div>
                        <div class="panel-body">
                            <p><asp:Literal ID="Literal1" runat="server"></asp:Literal></p>
                        </div>
                        <div class="panel-footer">
                            !
                        </div>
                    </div>
                </div>
                <!-- /.col-lg-4 -->
                <div class="col-lg-4">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <asp:Label ID="Label2_header" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="panel-body">
                            <p><asp:Literal ID="Literal2" runat="server"></asp:Literal></p>
                        </div>
                        <div class="panel-footer">
                            !
                        </div>
                    </div>
                </div>
                <!-- /.col-lg-4 -->
                <div class="col-lg-4">
                    <div class="panel panel-success">
                        <div class="panel-heading">
                            <asp:Label ID="Label3_header" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="panel-body">
                            <p><asp:Literal ID="Literal3" runat="server"></asp:Literal></p>
                        </div>
                        <div class="panel-footer">
                            !
                        </div>
                    </div>
                </div>
                <!-- /.col-lg-4 -->
            </div>
            <!-- /.row -->
            <div class="row">
                <div class="col-lg-4">
                    <div class="panel panel-green">
                        <div class="panel-heading">
                            <asp:Label ID="Label4_header" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="panel-body">
                            <p><asp:Literal ID="Literal4" runat="server"></asp:Literal></p>
                        </div>
                        <div class="panel-footer">
                            !
                        </div>
                    </div>
                    <!-- /.col-lg-4 -->
                </div>
                <div class="col-lg-4">
                    <div class="panel panel-yellow">
                        <div class="panel-heading">
                            <asp:Label ID="Label5_header" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="panel-body">
                            <p><asp:Literal ID="Literal5" runat="server"></asp:Literal></p>
                        </div>
                        <div class="panel-footer">
                            !
                        </div>
                    </div>
                    <!-- /.col-lg-4 -->
                </div>
                <div class="col-lg-4">
                    <div class="panel panel-red">
                        <div class="panel-heading">
                            <asp:Label ID="Label6_header" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="panel-body">
                            <p><asp:Literal ID="Literal6" runat="server"></asp:Literal></p>
                        </div>
                        <div class="panel-footer">
                            !
                        </div>
                    </div>
                    <!-- /.col-lg-4 -->
                </div>
            </div>
            <!-- /.row -->

    <!-- new row -->
    <div class="row">
        <div class="col-lg-12" >
             <div class="form-group">
                <asp:Button ID="btnEditAdd" runat="server" Width="200" Height="60px" Text="Editer/Ajouter Evénements" class="btn btn-outline btn-primary" OnClick="btnEditAdd_Click"/>                                            
            </div>
        </div>
    </div>
    <!-- end row -->
</asp:Content>


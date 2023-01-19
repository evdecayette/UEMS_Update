<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="CourrieldeMasseEtudiants.aspx.cs" Inherits="CourrieldeMasseEtudiants" ValidateRequest="false"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="css/formItems.css" rel="stylesheet" />
    <!-- add HTML TextArea editor stuff -->
    <%--<script type="text/javascript" src="http://js.nicedit.com/nicEdit-latest.js"></script>--%> 
    <script type="text/javascript" src="js/nicEdit-latest.js"></script> 
    <script type="text/javascript">
        //<![CDATA[
        bkLib.onDomLoaded(function () { nicEditors.allTextAreas() });
        //]]>
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
        <div class="container-fluid">  
        <div class="container">
            <div class='row'>
                <div class='col-lg-12'>
                    <div class='panel panel-default'>
                        <div class='panel-heading'>Cliquez -Toutes les Classes- ou Choisissez une Classe Spécifique</div>
                        <div class='panel-body'>
                            <div class="row">
                                <div class="col-lg-4"> 
                                    <div class="form-group">                                   
                                        <asp:CheckBox ID="chkToutesLesClasses" runat="server" Text="Toutes Les Classes" OnCheckedChanged="chkToutesLesClasses_CheckedChanged" AutoPostBack="True" />
                                    </div>
                                </div>
                                <div class="col-lg-4">
                                   <div class="form-group">
                                       <asp:Button ID="btnEnvoyer" runat="server" Width="140" Text="Envoyer Email" class="btn btn-outline btn-primary" OnClick="btnEnvoyer_Click"/>
                                   </div>
                                </div>
                                <div class="col-lg-4">
                                    <div class="form-group">
                                        <asp:Button ID="btnCancel" runat="server" Width="140" Text="Cancel" class="btn btn-outline btn-default" OnClick="btnCancel_Click" />  
                                    </div>                 
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-12">                                   
                                        <asp:Label ID="lblError" runat="server" Font-Bold="True" ForeColor="#FF3300"></asp:Label>                                     
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-5">  
                                    <div class="dynamicSelect">                                                                     
                                      <asp:DropDownList ID="ddlCoursOfferts" Height="30px" Width="100%" runat="server" DataValueField="CoursOffertID" DataTextField="Description" AutoPostBack="True" OnSelectedIndexChanged="ddlCoursOfferts_SelectedIndexChanged"></asp:DropDownList>
                                      <asp:CheckBox ID="chkIncludeNote" runat="server" Text="Ajouter Note Pour la Classe"  />
                                    </div>
                                        <asp:GridView ID="gvStudents" runat="server" DataKeyNames="PersonneID" AutoGenerateColumns="False" 
                                            ShowFooter="false" AllowPaging="false" class='table table-striped table-bordered table-hover'
                                            EnablePersistedSelection="True" ShowHeaderWhenEmpty="True" width='100%' 
                                            style="height:400px; overflow:auto" HeaderStyle-CssClass="FixedHeader">                           
                                            <Columns>
                                                <asp:TemplateField  HeaderText="Nom Complet">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblNomComplet" runat="server" Text='<%# Bind("NomComplet") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField  HeaderText="Email">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblEmail" runat="server" Text='<%# Bind("Email") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <HeaderStyle BackColor="#003300" Font-Bold="True" Font-Size="Medium" ForeColor="White" />
                                        </asp:GridView>
                                </div>
                                <div class="col-lg-7"> 
                                    <div class="form-group">
                                        <asp:TextBox ID="txtEmailSubject" runat="server"  class="form-control" placeholder="Objet (Sujet)"></asp:TextBox>
                                    </div>
                                    <div>                                        
                                        <asp:FileUpload ID="uploadAttachements" AllowMultiple="False" class="form-control" placeholder="Attache(s)" runat="server" />
                                    </div>
                                    <div class="form-group">                                        
                                        <textarea id="txtEmailBody" name="txtEmailBody" style="width: 100%; height:300px" runat="server"></textarea>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-6">                                             
                                  
                                </div>

                            </div>

                        </div>
                    </div>
                </div>
            </div>
            </div>
        </div>
</asp:Content>

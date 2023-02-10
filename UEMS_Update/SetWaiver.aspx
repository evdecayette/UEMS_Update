<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="SetWaiver.aspx.cs" Inherits="SetWaiver" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
     <asp:ToolkitScriptManager ID="toolkit1" runat="server"></asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class='row'>
            <div class='col-lg-12'>
                <asp:GridView ID="gvInfoSurCours" runat="server" DataKeyNames="PersonneID" AutoGenerateColumns="False" 
                    ShowFooter="false" AllowPaging="false" class='table table-striped table-bordered table-hover' 
                    EnablePersistedSelection="True" ShowHeaderWhenEmpty="True" width='100%'
                    OnRowCancelingEdit="gvInfoSurCours_RowCancelingEdit"
                    OnRowEditing="gvInfoSurCours_RowEditing" 
                    OnRowUpdating="gvInfoSurCours_RowUpdating">
                            <Columns>
                                <asp:TemplateField HeaderText="Action" ItemStyle-CssClass="center">
                                    <EditItemTemplate>
                                        <asp:Button ID="ButtonUpdate" runat="server" Width="48%" class="btn btn-success m-5" CommandName="Update"  Text="Update"  />
                                        <asp:Button ID="ButtonCancel" runat="server" Width="48%" class="btn btn-warning" CommandName="Cancel"  Text="Cancel" />
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:Button ID="btnWaiver" runat="server" CommandName="Edit" class="btn btn-info" Text="Set Waiver"  />
                                    </ItemTemplate>
                                   
                                 </asp:TemplateField>

                                <asp:TemplateField HeaderText="Cours" ItemStyle-CssClass="center">
                                    <ItemTemplate>
                                    <asp:Label ID="lblNomCours" runat="server" Text='<%# Bind("NomCours") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField  HeaderText="Numéro Cours">
                                    <ItemTemplate>
                                        <asp:Label ID="lblNumeroCours" runat="server" Text='<%# Bind("NumeroCours") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField  HeaderText="Note Sur 100">
                                    <ItemTemplate>
                                        <asp:Label ID="lblNoteSurCent" runat="server" Text='<%# Bind("NoteSurCent") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            
                                <asp:TemplateField  HeaderText="Waiver">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtWaiver" runat="server" Text='<%# Bind("Waiver") %>'></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <%-- <asp:TemplateField  HeaderText="">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCoursPrisID" Width="0%" runat="server" Text='<%# Bind("CoursPrisID") %>' Visible="false"></asp:Label>
                                        <asp:Label ID="lblPersonneID" Width="0%" runat="server" Text='<%# Bind("PersonneID") %>' Visible="false"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>--%>
                            </Columns>
                            <HeaderStyle BackColor="#003300" Font-Bold="True" Font-Size="Medium" ForeColor="White" />
                        </asp:GridView>
                    </div>
            </div>
         <div class='row'>
            <div class='col-lg-6'>
                <asp:Label ID="lblError" runat="server" Font-Bold="True" ForeColor="#FF3300"></asp:Label> 
            </div>
         </div>
          </ContentTemplate>
        </asp:UpdatePanel>
</asp:Content>


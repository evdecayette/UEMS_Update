<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="AddEditNotePassage.aspx.cs" Inherits="AddEditNotePassage" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="css/formItems.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:ToolkitScriptManager ID="toolkit1" runat="server"></asp:ToolkitScriptManager>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <div class='row'><div class='col-lg-12'><h3 class='page-header'>Changer Note de Passage</h3></div></div>
        <table>
          <tr>
             <td>
              <div class="dynamiSelect">  
                  <asp:DropDownList ID="ddlCoursOfferts" runat="server" Width="400px" Font-Size="Medium" Height="30px" TabIndex="0" AutoPostBack="True" OnSelectedIndexChanged="ddlCoursOfferts_SelectedIndexChanged" >
                  </asp:DropDownList>
              </div> 
            </td>
            <td>
                <div class="dynamiclabel">
                    <asp:TextBox ID="txtNotePassage" runat="server" Font-Size="Medium" Width="50px" TabIndex="1" ></asp:TextBox>
                    <asp:TextBox ID="txtNotePassage_Hidden" runat="server" Visible ="false" ></asp:TextBox>
                </div>
            </td>
            <td>
               <div class="dynamicbutton" >           
                 <asp:Button ID="btnSauvegarder" runat="server" Text="Sauvegarder" Width="300px" OnClick="btnSauvegarder_Click" />
                 <label for="btnSauvegarder">Sauvegarder</label>
               </div>                    
            </td>
          </tr>

          <tr>
            <td colspan="2" style="text-align:center">
                <asp:Label ID="lblError" runat="server" Font-Bold="True" Font-Size="Medium" ForeColor="#C00000" Width="100%"></asp:Label>
            </td>
         </tr>
       </table>
    </ContentTemplate>
</asp:UpdatePanel>

</asp:Content>
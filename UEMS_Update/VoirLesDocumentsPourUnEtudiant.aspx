<%@ Page Title="" EnableEventValidation="true" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="VoirLesDocumentsPourUnEtudiant.aspx.cs" Inherits="VoirLesDocumentsPourUnEtudiant" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <style>
     .center-content {
         text-align: center;
        }

     .center-content td {
         text-align: center;
       }

    .gridview-pager {
        text-align: center;
    }

    .gridview-pager a {
        display: inline-block;
        padding: 5px 10px;
        margin: 0 5px;
        background-color: #555;
        border: 1px solid #ccc;
        text-decoration: none;
        color: seagreen;
    }

    .header-style {
        text-align: center;
        font-weight: bold;
        background-color: #e0e0e0; /* Header background color */
    }
    
</style>
    <div class="p-2">
        <asp:Label ID="lblInfoEtudiant" runat="server" Text="Dossiers Pour L'Etudiant: "></asp:Label>
    </div>
   
    <asp:GridView ID="GridView1" class="table table-hover table-striped" runat="server" 
        AutoGenerateColumns="False" width="100%" DataKeyNames="documentId"
        ShowFooter="True" AllowPaging="True" ShowHeaderWhenEmpty="True"
        EmptyDataText="No Records Founds"
        OnPageIndexChanging="GridView1_PageIndexChanging"
        OnRowEditing="GridView1_RowEditing" 
        OnRowUpdating="GridView1_RowUpdating"
        OnRowCommand="GridView1_RowCommand"
        OnRowCancelingEdit="GridView1_RowCancelingEdit"
        OnRowDataBound="GridView1_RowDataBound">
        <PagerSettings Mode="NextPreviousFirstLast" FirstPageText="first" PreviousPageText="Preview" NextPageText="Next" LastPageText="Last" PageButtonCount="10" />
        <PagerStyle HorizontalAlign="Center" />
        <HeaderStyle HorizontalAlign="Center" />
    <Columns>
        <asp:TemplateField HeaderText="Image">
            <ItemTemplate >
                <asp:Image ID="imgDocument" runat="server" Width="20px" ImageUrl='<%# Eval("ImageData") %>'/>
            </ItemTemplate>
            <EditItemTemplate>
                 <asp:FileUpload ID="fileUploadID" Width="120px" Font-Size="Smaller" runat="server"></asp:FileUpload>
            </EditItemTemplate>
            <FooterTemplate>
                 <asp:FileUpload ID="fileUploadID" Font-Size="Smaller" Width="120px" runat="server"></asp:FileUpload>
            </FooterTemplate>       
            <ControlStyle Font-Size="Smaller" />
            <HeaderStyle HorizontalAlign="Center" />
            <ItemStyle Height="15px" Width="20px" HorizontalAlign="Center" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Description" SortExpression="Description">
         <ItemTemplate>
             <asp:label ID="lblDescription" runat="server" Text='<%# Eval("description") %>'/>
         </ItemTemplate>        
        <EditItemTemplate>
            <asp:DropDownList ID="ddlDescription" runat="server"></asp:DropDownList>
        </EditItemTemplate>
         <FooterTemplate>
            <asp:DropDownList ID="ddlDescription" runat="server"></asp:DropDownList>
        </FooterTemplate>
            <HeaderStyle HorizontalAlign="Center" />
        </asp:TemplateField>
       <asp:TemplateField HeaderText="Actions">
            <ItemTemplate>
                <asp:Button ID="btnPreview" Width="45%" Class="btn btn-primary" runat="server" CommandName="Preview" CommandArgument='<%# Eval("documentId") %>' Text="Preview"/>
                <asp:Button ID="btnEdit" Width="45%" Class="btn btn-warning center" runat="server" CommandName="Edit" Text="Edit" />
            </ItemTemplate>

            <EditItemTemplate>
                <asp:Button ID="btnUpdate" Class="btn btn-success center pb-2" width="45%" runat="server" CommandArgument='<%# Eval("documentId") %>' CommandName="Update"  Text="Update" />
                <asp:Button ID="btnCancel" Class="btn btn-warning center pt-2" width="45%" runat="server" CommandName="Cancel"  Text="Cancel" />
            </EditItemTemplate>
            <FooterTemplate>
                <asp:Button ID="btnAdd" Class="btn btn-info center pt-2" width="90%" runat="server" CommandName="addNew" CommandArgument='<%# Eval("documentId") %>'  Text="Nouveau Dossier" />
            </FooterTemplate>
            <HeaderStyle HorizontalAlign="Center" />
        </asp:TemplateField>
    </Columns>  
    <EmptyDataTemplate>
    <tr>
        <td colspan="4" class="empty-message">
            No records found
        </td>
    </tr>
    <tr class="footer-row">
        <td>
            <asp:FileUpload ID="fileUploadEmptyFooter" Width="120px" runat="server" Font-Size="Smaller"></asp:FileUpload>      
        </td>
        <td>
            <asp:DropDownList ID="ddlDescriptionEmptyFooter" runat="server"></asp:DropDownList>
        </td>
        <td colspan="2">
            <asp:Button ID="btnAdd" Class="btn btn-info center pt-2" width="90%" runat="server" Onclick="btnAdd_Click"  Text="Nouveau Dossier" />
        </td>
    </tr>
    </EmptyDataTemplate>
</asp:GridView>
    <div>
        <asp:Label runat="server" ID="lblError" class="colors"></asp:Label>
        
    </div>
</asp:Content>


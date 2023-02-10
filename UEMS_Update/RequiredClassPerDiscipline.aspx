<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="RequiredClassPerDiscipline.aspx.cs" Inherits="RequiredClassPerDiscipline" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="row pt-4">
    <div class="col-lg-12">
        <p style="text-align:center; font-size:25px;margin-bottom:5px"><strong>Université Espoir </strong></p>
        <asp:label ID="lblCursus" runat="server" style="text-align:center; font-size:20px;margin-bottom:5px"></asp:label>
        
        <hr />
    </div>
</div>
<div class="row" style="margin-left:10px;margin-right:10px">
   <%-- <div class="col-sm-1">
    </div>--%>
  <div class="col-sm-12 p-1">
    <div class="card">
      <div class="card-body">
        <%--<h5 class="card-title" style="text-align:Center"><strong>Liste Des Cours Du Programme De Gestion</strong></h5>
          <asp:label ID="Label1" runat="server" style="text-align:center; font-size:20px;margin-bottom:5px"></asp:label>--%>
            <asp:label runat="server" Text="" id="lblErreur"></asp:label>
            <asp:ListView ID="ContentLiteralID" runat="server" GroupPlaceholderID="groupplacehlder" ItemPlaceholderID="itemplaceolder">
                <LayoutTemplate>
                    <table class='table table-striped table-bordered table-hover' border="0">
                        <tr class="bg-body table-secondary">
                            <th>Cours</th>
                            <th style="text-align:center">Numéro</th>
                            <th style="text-align:center">Pré-Requis</th>
                            <th style="text-align:center">Crédits</th>                           
                            <%--<th style="text-align:center">Notes de Passage</th>--%>
                        </tr>
                        <tr id="groupplacehlder" runat="server"></tr>
                    </table>
                </LayoutTemplate>
                <GroupTemplate>
                    <tr>
                       <tr id="itemplaceolder" runat="server"></tr>
                    </tr>
                </GroupTemplate>
                <ItemTemplate>
                    <td style="width:30%"><%# Eval("NomCours") %></td>
                    <td style="width:20%;text-align:center"><%# Eval("NumeroCours") %></td>
                    <td style="width:20%;text-align:center"><%# Eval("CoursPreRequis") %></td>
                    <td style="width:10%;text-align:center"><%# Eval("Credits") %></td>                    
                    <%--<td style="width:10%;text-align:center"><%# Eval("NoteDePassage") %></td>--%>
                </ItemTemplate>
            </asp:ListView>
      </div>
    </div>
     
  </div>
</div>
</asp:Content>


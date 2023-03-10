<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeFile="ReleveNotesEtudiant.aspx.cs" Inherits="ReleveNotesEtudiant" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="PrintStyleFormat.css" type='text/css' rel="stylesheet" media="print"/>  
    <base target='_self'/> 
</head>
<body style="margin-bottom:0;margin-left:0;margin-right:0;margin-top:0;" ms_positioning="GridLayout">
    <form method="post" runat="server">
        <table style="width:100%">
            <tr>
                <td>
                    <input class="print" id='Button1' type='button' value='Print' onclick='window.print();'/>
                    <asp:ImageButton ID="btnWord" AlternateText="MS-Excel" ImageUrl="~/images/msexcel.png" runat="server" style='width:25px;height:25px;' OnClick="btnWord_Click"/>
                    <asp:TextBox ID="txtPersonneID" runat="server" Visible="false"></asp:TextBox>
                </td>
            </tr>
        </table>
        <asp:Literal ID="litBody"  runat="server"></asp:Literal>
    </form>
</body>
</html>
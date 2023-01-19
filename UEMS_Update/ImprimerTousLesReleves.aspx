<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ImprimerTousLesReleves.aspx.cs" Inherits="ImprimerTousLesReleves" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
        <link href="PrintStyleFormat.css" type='text/css' media="print" rel="stylesheet"/>  
    <base target='_self'/>
    <script>
        function printDiv(divName) {
            var printContents = document.getElementById(divName).innerHTML;
            var originalContents = document.body.innerHTML;

            document.body.innerHTML = printContents;
            window.print();
            document.body.innerHTML = originalContents;
        }
    </script>
</head>
<body style="margin-bottom:10;margin-left:10;margin-right:10;margin-top:10;" ms_positioning="GridLayout">
<form method="post">
    <input class="print" id='Button1' type='button' value='Print' onclick="printDiv('AImprimer')" />
    <div id="AImprimer">
        <asp:Literal ID="litBody"  runat="server"></asp:Literal>
    </div>  
</form>
</body>
</html>
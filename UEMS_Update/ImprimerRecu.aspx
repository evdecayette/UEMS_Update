<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ImprimerRecu.aspx.cs" Inherits="ImprimerRecu" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
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
        <div style='height:50px;'>
			<br/>
		</div>
		<div>
			<hr style='background-color:#669999;' size='3'>
			<p style='text-align:center;font-size:20px'>Université Espoir</p>
			<p style='text-align:center;'><img src='images/logo.jpg' alt='Imprimer Reçu'></p>
			<p style='text-align:center;font-size:14px'>Reçu Paiement</p>
			<hr style='background-color:#669999;' size='3'>
		</div>
		<div style='height:50px;'>
			<br/>
		</div>		
		<div style='border: 1px solid; border-radius: 20px;'>
            <asp:Literal ID="litBody"  runat="server"></asp:Literal>
		</div>
    </div>  
</form>
</body>
</html>

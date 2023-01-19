<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ImprimerRecuTout.aspx.cs" Inherits="ImprimerRecuTout" %>

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

        <!-- Bootstrap Core CSS -->
    <link href="vendor/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <!-- MetisMenu CSS -->
    <link href="vendor/metisMenu/metisMenu.min.css" rel="stylesheet" />
    <!-- Custom CSS -->
    <link href="dist/css/sb-admin-2.css" rel="stylesheet"/>
    <!-- Custom Fonts -->
    <link href="vendor/font-awesome/css/font-awesome.min.css" rel="stylesheet" type="text/css" />

    <!-- HTML5 Shim and Respond.js IE8 support of HTML5 elements and media queries -->
    <!-- WARNING: Respond.js doesn't work if you view the page via file:// -->
    <!--[if lt IE 9]>
        <script src="https://oss.maxcdn.com/libs/html5shiv/3.7.0/html5shiv.js"></script>
        <script src="https://oss.maxcdn.com/libs/respond.js/1.4.2/respond.min.js"></script>
    <![endif]-->
</head>
<body style="margin-bottom:10px;margin-left:10px;margin-right:10px;margin-top:10px;" ms_positioning="GridLayout">
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
			<p style='text-align:center;font-size:14px'>Historique de Paiements Reçus</p>
			<hr style='background-color:#669999;' size='3'>
		</div>
        <asp:Literal ID="litBody"  runat="server"></asp:Literal>
    </div>  
</form>
</body>
</html>

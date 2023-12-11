<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="PreviewPage.aspx.cs" Inherits="PreviewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <div class="row">
        <asp:Button ID="btnPrint" runat="server" Text="Print Image" OnClientClick="printImage(); return false;" />
    </div>
    <div class="row">
       <asp:Image ID="Image1" runat="server" />
    </div>
  <script type="text/javascript">
    function printImage() {
        var printWindow = window.open();
        printWindow.document.write('<html><head><title>Print Image</title></head><body>');
        printWindow.document.write('<img src="' + document.getElementById('<%= Image1.ClientID %>').src + '" style="width:100%;" onload="window.print(); window.onafterprint = function(){ window.close(); }"/>');
        printWindow.document.write('</body></html>');
        printWindow.document.close();

        // Use CSS to hide date information in the print window
        printWindow.document.styleSheets[0].insertRule('@media print { body::after { content: none !important; } }', 0);

        // Trigger the print
        printWindow.print();

        // Close the print window after printing
        printWindow.close();
    }
  </script>


</asp:Content>


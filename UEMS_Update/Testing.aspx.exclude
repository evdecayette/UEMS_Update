<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Testing.aspx.cs" Inherits="Testing" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style>
        .button-wait {
            background-image: url(images/loader.gif); /* 16px x 16px */
            background-color: transparent; /* make the button transparent */
            background-repeat: no-repeat;  /* make the background image appear only once */
            /*background-position: 0px 0px;   equivalent to 'top left' */
            background-position: center;
            border: none;           /* assuming we don't want any borders */
            cursor: pointer;        /* make the cursor like hovering over an <a> element */
            height: 16px;           /* make this the size of your image */
            padding-left: 16px;     /* make text start to the right of the image */
            vertical-align: middle; /* align the text vertically centered */
            border-radius: 4px;
            border: 1px solid blue;
        }
    </style>
    <script>

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:ToolkitScriptManager ID="toolkit1" runat="server"></asp:ToolkitScriptManager>
    <asp:Timer runat="server" ID="Timer1" Interval="500" Enabled="false" ontick="Timer1_Tick" />
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="Timer1" EventName="Tick" />
    </Triggers>
        <ContentTemplate>
<%--            <asp:Panel Visible="true" runat="server" ID="myHiddenPanel">
                <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="false" style="align-content:center;">
                    <img src="images/loader.gif" alt="" />
                </div>
            </asp:Panel>
            <div class="modal fade" id="Div1" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="false" style="align-content:center;">
                <img src="images/loader.gif" alt="" />
            </div>--%>
            <div class="form-group">
                <asp:Button ID="btnSauvegarder" runat="server" Width="140" Height="60px" Text="Sauvegarder" class="btn btn-outline btn-primary" OnClick="btnSauvegarder_Click" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>


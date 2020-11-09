<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UserInterface.aspx.cs" Inherits="User" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="homepage.css" rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/css2?family=Orbitron&display=swap" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css2?family=Architects+Daughter&display=swap" rel="stylesheet" />
<script type = "text/javascript" >
    function DisableBackButton() {
        window.history.forward();
    }
setTimeout("DisableBackButton()", 0);
window.onunload = function() {
    null
}; 
</script>
</head>
<body>
    <form id="Anketa" runat="server">
        <div class="homepageTitle">
            <asp:Label ID="Label1" runat="server" Text="RANDOM ANKETA"></asp:Label>
        </div>
        <div class="container">
            <asp:Label ID="notifyLabel" runat="server" Text="Vaš glas je zabilježen u bazi ! Hvala na glasanju !" Visible="False"></asp:Label>
            <asp:Label ID="naslovAnkete" runat="server"></asp:Label>
            <div class="listaOdgovora">
                <asp:RadioButtonList ID="RadioButtonList1" runat="server">
                </asp:RadioButtonList>
            </div>
            <asp:Button ID="voteButton" runat="server" Text="Pošalji moj odabir !" OnClick="VoteButton_Click" BackColor="Transparent" BorderColor="Black" ForeColor="Black" BorderWidth="1" />
        </div>
    </form>
</body>
</html>

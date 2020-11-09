<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Admin.aspx.cs" Inherits="Admin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Admin</title>
    <link href="admin.css" rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/css2?family=Orbitron&display=swap" rel="stylesheet" />
<script type = "text/javascript" >
    function DisableBackButton() {window.history.forward();}setTimeout("DisableBackButton()", 0);window.onunload = function() {null}; 
</script>
</head>
<body>
    <asp:Label ID="title" runat="server" Text="ADMIN PANEL"></asp:Label>
    <hr />
    <form id="listaAnketa" runat="server">
    <div id="startingDiv" runat="server">
    <asp:PlaceHolder ID="dodajButton" runat="server"></asp:PlaceHolder>
    <br /> <br />
    <asp:Label ID="popisanketalabel" runat="server" Text="Popis svih anketa:"></asp:Label>
    <br />
    <asp:Label ID="info" runat="server" Text="(Odaberite anketu za prikaz detalja ili eventualne izmjene !)"></asp:Label>
    <br /><br />
    <asp:Label ID="legenda" runat="server" Text="žuta boja - anketu je moguće uređivati" ForeColor="Yellow" Font-Size="10"></asp:Label>
    <br /><br />
    </div>
    <asp:Label ID="notifyLabel" runat="server" Visible="False"></asp:Label>   
    </form>
    <form id="pregledPojedinacneAnkete" runat="server">  
    </form>
</body>
</html>

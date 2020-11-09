<%@ Application Language="C#" %>
<%@ Import Namespace="Anketa_vipdata" %>
<%@ Import Namespace="System.Web.Optimization" %>
<%@ Import Namespace="System.Web.Routing" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e)
    {
        RouteConfig.RegisterRoutes(RouteTable.Routes);
        BundleConfig.RegisterBundles(BundleTable.Bundles);

        RouteTable.Routes.MapPageRoute("admin", "AdminPanel", "~/Admin.aspx");
        RouteTable.Routes.MapPageRoute("user", "RandomAnketa", "~/UserInterface.aspx");

    }

</script>

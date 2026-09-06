<%@ Page Language="C#" Async="true" AutoEventWireup="true" CodeBehind="oauth_callback.aspx.cs" Inherits="hephaestus_web.oauth_callback" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Hephaestus - Autenticação Google</title>
    <link rel="stylesheet" href="assets/vendors/css/vendor.bundle.base.css" />
    <link rel="stylesheet" href="assets/css/style.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container-scroller">
            <div class="container-fluid page-body-wrapper full-page-wrapper">
                <div class="content-wrapper full-page-wrapper d-flex align-items-center auth login-bg">
                    <div class="card col-lg-4 mx-auto"><div class="card-body px-5 py-5 text-center">
                        <h3 class="card-title">Autenticação Google</h3>
                        <asp:Label ID="lblStatus" runat="server" Text="A concluir o início de sessão…" />
                        <div class="mt-4"><asp:HyperLink ID="lnkLogin" runat="server" NavigateUrl="~/login.aspx" Visible="false">Voltar ao login</asp:HyperLink></div>
                    </div></div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>

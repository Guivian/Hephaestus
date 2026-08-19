<%@ Page Title="Erro interno do servidor" Language="C#" AutoEventWireup="true" CodeBehind="error_500.aspx.cs" Inherits="hephaestus_web.error_500" %>
<!DOCTYPE html>
<html lang="pt">

    <head runat="server">
        <meta charset="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1" />
        <title>500 · Erro interno | Hephaestus</title>
        <link rel="stylesheet" href="assets/vendors/mdi/css/materialdesignicons.min.css" />
        <link rel="stylesheet" href="assets/vendors/css/vendor.bundle.base.css" />
        <link rel="stylesheet" href="assets/css/style.css" />
        <link rel="stylesheet" href="assets/css/hephaestus-pages.css" />
        <link rel="stylesheet" href="assets/css/reporting-pages.css" />
    </head>

    <body class="error-page-body">
        <form id="form1" runat="server">
            <main class="error-shell"><a href="dashboard.aspx" class="error-brand"><img src="assets/images/logo.svg?v=4" alt="Hephaestus" /></a>
                <section class="error-card error-500" aria-labelledby="errorTitle">
                    <div class="error-code" aria-hidden="true">500</div><span class="error-icon"><i class="mdi mdi-server-network-off"></i></span>
                    <h1 id="errorTitle">Ocorreu um erro interno</h1>
                    <p>Não foi possível concluir o pedido. Tente novamente dentro de alguns instantes.</p>
                    <div class="error-status"><span></span>
                        <div><strong>Serviço temporariamente indisponível</strong><small>Os seus dados e alterações guardadas permanecem seguros.</small></div>
                    </div>
                    <div class="error-actions"><button type="button" class="btn btn-primary" onclick="location.reload()"><i class="mdi mdi-refresh mr-1"></i> Tentar novamente</button><a href="dashboard.aspx" class="btn btn-outline-secondary"><i class="mdi mdi-home-outline mr-1"></i> Ir para o dashboard</a></div><small>Se o problema persistir, contacte o administrador da plataforma.</small>
                </section>
                <footer>Hephaestus Helpdesk · Código do erro: 500</footer>
            </main>
        </form>
    </body>

</html>
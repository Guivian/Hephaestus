<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/hephaestus.Master" AutoEventWireup="true" CodeBehind="dashboard.aspx.cs" Inherits="hephaestus_web.dashboard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="hephaestus-dashboard">
        <section class="dashboard-hero mb-4" aria-labelledby="dashboardTitle">
            <div>
                <span class="dashboard-eyebrow">Visão geral</span>
                <h2 id="dashboardTitle" class="mb-1">Bem-vindo, <span id="dashboardUserName">Utilizador</span></h2>
                <p class="text-muted mb-0"><i class="mdi mdi-calendar-today mr-1"></i><span id="dashboardCurrentDate">Hoje</span> · Consulte rapidamente o que precisa da sua atenção.</p>
            </div>
        </section>

        <section class="row dashboard-kpis" aria-label="Indicadores principais">
            <div class="col-sm-6 col-xl-3 grid-margin stretch-card">
                <article class="card dashboard-kpi dashboard-kpi-primary">
                    <div class="card-body"><div class="dashboard-kpi-header"><span class="dashboard-kpi-icon"><i class="mdi mdi-ticket-outline"></i></span><span id="kpiOpenTrend" class="dashboard-kpi-trend">—</span></div><div id="kpiOpen" class="dashboard-kpi-value">—</div><h3 class="dashboard-kpi-label">Tickets abertos</h3><p class="dashboard-kpi-help">Pedidos ativos relevantes para si</p></div>
                </article>
            </div>
            <div class="col-sm-6 col-xl-3 grid-margin stretch-card">
                <article class="card dashboard-kpi dashboard-kpi-warning">
                    <div class="card-body"><div class="dashboard-kpi-header"><span class="dashboard-kpi-icon"><i class="mdi mdi-alert-circle-outline"></i></span><span id="kpiAttentionTrend" class="dashboard-kpi-trend">—</span></div><div id="kpiAttention" class="dashboard-kpi-value">—</div><h3 class="dashboard-kpi-label">Requerem atenção</h3><p class="dashboard-kpi-help">Pendentes, atrasados ou por atribuir</p></div>
                </article>
            </div>
            <div class="col-sm-6 col-xl-3 grid-margin stretch-card" data-permission="tasks-view">
                <article class="card dashboard-kpi dashboard-kpi-info">
                    <div class="card-body"><div class="dashboard-kpi-header"><span class="dashboard-kpi-icon"><i class="mdi mdi-calendar-clock"></i></span><span id="kpiTodayTrend" class="dashboard-kpi-trend">—</span></div><div id="kpiToday" class="dashboard-kpi-value">—</div><h3 class="dashboard-kpi-label">Tarefas para hoje</h3><p class="dashboard-kpi-help">Intervenções planeadas para o dia</p></div>
                </article>
            </div>
            <div class="col-sm-6 col-xl-3 grid-margin stretch-card">
                <article class="card dashboard-kpi dashboard-kpi-success">
                    <div class="card-body"><div class="dashboard-kpi-header"><span class="dashboard-kpi-icon"><i class="mdi mdi-check-circle-outline"></i></span><span id="kpiResolvedTrend" class="dashboard-kpi-trend">—</span></div><div id="kpiResolved" class="dashboard-kpi-value">—</div><h3 class="dashboard-kpi-label">Resolvidos</h3><p class="dashboard-kpi-help">Tickets concluídos no período</p></div>
                </article>
            </div>
        </section>

        <div class="row">
            <div class="col-xl-8 grid-margin stretch-card">
                <section class="card w-100" aria-labelledby="attentionTitle">
                    <div class="card-body p-0">
                        <div class="dashboard-card-header"><div><h4 id="attentionTitle" class="card-title mb-1">Tickets que requerem atenção</h4><p class="text-muted small mb-0">Pedidos prioritários, pendentes ou sem atribuição</p></div><a href="tickets_listing.aspx" class="btn btn-outline-secondary btn-sm">Ver todos</a></div>
                        <div class="table-responsive">
                            <table class="table dashboard-table mb-0">
                                <thead><tr><th>Ticket</th><th>Assunto</th><th>Prioridade</th><th>Estado</th><th>Responsável</th><th>Aberto em</th><th></th></tr></thead>
                                <tbody id="attentionTicketRows">
                                    <tr class="dashboard-empty-row"><td colspan="7"><div class="dashboard-empty"><i class="mdi mdi-ticket-outline"></i><strong>Sem tickets para apresentar</strong><span>Os tickets que necessitem de atenção serão apresentados aqui.</span></div></td></tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </section>
            </div>
            <div class="col-xl-4 grid-margin stretch-card" data-permission="tasks-view">
                <section class="card w-100" aria-labelledby="agendaTitle">
                    <div class="card-body">
                        <div class="dashboard-card-header px-0 pt-0"><div><h4 id="agendaTitle" class="card-title mb-1">Agenda de hoje</h4><p class="text-muted small mb-0">Próximas intervenções planeadas</p></div><a href="technician_calendar.aspx" class="btn btn-outline-secondary dashboard-icon-button" title="Abrir calendário"><i class="mdi mdi-calendar"></i></a></div>
                        <div id="todayAgenda" class="dashboard-agenda">
                            <div class="dashboard-empty"><i class="mdi mdi-calendar-blank"></i><strong>Agenda sem intervenções</strong><span>As tarefas planeadas para hoje aparecerão aqui.</span></div>
                        </div>
                    </div>
                </section>
            </div>
        </div>

        <div class="row" data-permission="management-view">
            <div class="col-xl-7 grid-margin stretch-card">
                <section class="card w-100" aria-labelledby="statusChartTitle">
                    <div class="card-body">
                        <div class="dashboard-card-header px-0 pt-0"><div><h4 id="statusChartTitle" class="card-title mb-1">Tickets por estado</h4><p class="text-muted small mb-0">Distribuição dos pedidos no período selecionado</p></div><select id="dashboardPeriod" class="form-control form-control-sm dashboard-period"><option value="30">Últimos 30 dias</option><option value="90">Últimos 90 dias</option><option value="365">Último ano</option></select></div>
                        <div id="ticketStatusChart" class="dashboard-chart" role="img" aria-label="Gráfico de tickets por estado">
                            <div class="dashboard-empty"><i class="mdi mdi-chart-donut"></i><strong>Sem dados estatísticos</strong><span>O gráfico será apresentado quando existirem tickets no período.</span></div>
                        </div>
                        <div id="ticketStatusLegend" class="dashboard-chart-legend"></div>
                    </div>
                </section>
            </div>
            <div class="col-xl-5 grid-margin stretch-card">
                <section class="card w-100" aria-labelledby="workloadTitle">
                    <div class="card-body">
                        <div class="dashboard-card-header px-0 pt-0"><div><h4 id="workloadTitle" class="card-title mb-1">Carga por técnico</h4><p class="text-muted small mb-0">Intervenções abertas por responsável</p></div><i class="mdi mdi-account-group text-muted h4 mb-0"></i></div>
                        <div id="technicianWorkload" class="dashboard-workload">
                            <div class="dashboard-empty"><i class="mdi mdi-account-clock"></i><strong>Sem carga registada</strong><span>A distribuição de tarefas será apresentada aqui.</span></div>
                        </div>
                    </div>
                </section>
            </div>
        </div>

        <div class="row">
            <div class="col-xl-7 grid-margin stretch-card">
                <section class="card w-100" aria-labelledby="recentTitle">
                    <div class="card-body p-0">
                        <div class="dashboard-card-header"><div><h4 id="recentTitle" class="card-title mb-1">Tickets recentes</h4><p class="text-muted small mb-0">Últimos pedidos disponíveis para o seu perfil</p></div><a href="tickets_listing.aspx" class="text-primary small">Histórico completo <i class="mdi mdi-arrow-right"></i></a></div>
                        <div id="recentTickets" class="dashboard-recent-list">
                            <div class="dashboard-empty"><i class="mdi mdi-history"></i><strong>Sem tickets recentes</strong><span>Os pedidos mais recentes serão apresentados aqui.</span></div>
                        </div>
                    </div>
                </section>
            </div>
            <div class="col-xl-5 grid-margin stretch-card">
                <section class="card w-100" aria-labelledby="activityTitle">
                    <div class="card-body">
                        <div class="dashboard-card-header px-0 pt-0"><div><h4 id="activityTitle" class="card-title mb-1">Atividade recente</h4><p class="text-muted small mb-0">Comentários, anexos e alterações</p></div><i class="mdi mdi-bell-outline text-muted h4 mb-0"></i></div>
                        <div id="recentActivity" class="dashboard-activity">
                            <div class="dashboard-empty"><i class="mdi mdi-timeline-text-outline"></i><strong>Sem atividade recente</strong><span>As últimas alterações serão apresentadas aqui.</span></div>
                        </div>
                    </div>
                </section>
            </div>
        </div>

        <section class="card mb-4" aria-labelledby="quickActionsTitle">
            <div class="card-body">
                <div class="dashboard-card-header px-0 pt-0"><div><h4 id="quickActionsTitle" class="card-title mb-1">Ações rápidas</h4><p class="text-muted small mb-0">Acesso direto às operações mais utilizadas</p></div></div>
                <div class="dashboard-quick-actions">
                    <a href="#" class="dashboard-action" data-permission="ticket-create"><span class="dashboard-action-icon text-success"><i class="mdi mdi-plus-circle-outline"></i></span><span><strong>Abrir ticket</strong><small>Registar um novo pedido</small></span><i class="mdi mdi-chevron-right"></i></a>
                    <a href="tickets_listing.aspx" class="dashboard-action"><span class="dashboard-action-icon text-primary"><i class="mdi mdi-format-list-bulleted"></i></span><span><strong>Consultar tickets</strong><small>Pesquisar pedidos e histórico</small></span><i class="mdi mdi-chevron-right"></i></a>
                    <a href="technician_calendar.aspx" class="dashboard-action" data-permission="calendar-view"><span class="dashboard-action-icon text-warning"><i class="mdi mdi-calendar-clock"></i></span><span><strong>Calendário</strong><small>Consultar intervenções planeadas</small></span><i class="mdi mdi-chevron-right"></i></a>
                    <a href="#" class="dashboard-action" data-permission="users-manage"><span class="dashboard-action-icon text-info"><i class="mdi mdi-account-card-details"></i></span><span><strong>Gerir utilizadores</strong><small>Contas, funções e acessos</small></span><i class="mdi mdi-chevron-right"></i></a>
                    <a href="statistics_metrics.aspx" class="dashboard-action" data-permission="reports-view"><span class="dashboard-action-icon text-danger"><i class="mdi mdi-file-chart"></i></span><span><strong>Relatórios</strong><small>Estatísticas e exportações</small></span><i class="mdi mdi-chevron-right"></i></a>
                </div>
            </div>
        </section>

        <section class="card dashboard-admin-summary mb-4" data-permission="admin-view" aria-labelledby="adminSummaryTitle">
            <div class="card-body">
                <div class="dashboard-card-header px-0 pt-0"><div><h4 id="adminSummaryTitle" class="card-title mb-1">Resumo administrativo</h4><p class="text-muted small mb-0">Contas e elementos operacionais que requerem acompanhamento</p></div><span class="badge badge-outline-info">Administração</span></div>
                <div class="row">
                    <div class="col-sm-6 col-lg-3 mb-3 mb-lg-0"><div class="admin-metric"><i class="mdi mdi-account-multiple"></i><span><strong id="adminTotalUsers">—</strong><small>Utilizadores</small></span></div></div>
                    <div class="col-sm-6 col-lg-3 mb-3 mb-lg-0"><div class="admin-metric"><i class="mdi mdi-worker"></i><span><strong id="adminActiveTechnicians">—</strong><small>Técnicos ativos</small></span></div></div>
                    <div class="col-sm-6 col-lg-3 mb-3 mb-sm-0"><div class="admin-metric"><i class="mdi mdi-account-off"></i><span><strong id="adminInactiveUsers">—</strong><small>Contas inativas</small></span></div></div>
                    <div class="col-sm-6 col-lg-3"><div class="admin-metric"><i class="mdi mdi-ticket-account"></i><span><strong id="adminUnassignedTickets">—</strong><small>Tickets por atribuir</small></span></div></div>
                </div>
            </div>
        </section>
    </div>

    <script>
        (function () {
            'use strict';
            var dateElement = document.getElementById('dashboardCurrentDate');
            if (dateElement) {
                dateElement.textContent = new Date().toLocaleDateString('pt-PT', { weekday: 'long', day: '2-digit', month: 'long', year: 'numeric' });
            }
        }());
    </script>
</asp:Content>

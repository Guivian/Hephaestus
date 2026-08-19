<%@ Page Title="Estatísticas e Métricas" Language="C#" MasterPageFile="~/hephaestus.Master" AutoEventWireup="true" CodeBehind="statistics_metrics.aspx.cs" Inherits="hephaestus_web.statistics_metrics" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .reports-page .report-kpi .report-kpi-icon {
            width: 42px !important;
            height: 42px !important;
            padding: 0 !important;
            margin: 0 0 1rem 0 !important;
            border-radius: 9px !important;
            display: flex !important;
            align-items: center !important;
            justify-content: center !important;
            line-height: 1 !important;
        }

        .reports-page .report-kpi-blue .report-kpi-icon {
            background: #0090e7 !important;
        }

        .reports-page .report-kpi-green .report-kpi-icon {
            background: #00d25b !important;
        }

        .reports-page .report-kpi-purple .report-kpi-icon {
            background: #8f5fe8 !important;
        }

        .reports-page .report-kpi-orange .report-kpi-icon {
            background: #ffab00 !important;
        }

        .reports-page .report-kpi .report-kpi-icon .mdi {
            width: 42px !important;
            height: 42px !important;
            margin: 0 !important;
            padding: 0 !important;
            color: #fff !important;
            font-size: 21px !important;
            line-height: 42px !important;
            text-align: center !important;
            display: block !important;
        }

        .reports-page .report-kpi .report-kpi-icon .mdi::before {
            display: inline-block !important;
            margin: 0 !important;
            line-height: 42px !important;
            vertical-align: top !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="reports-page">
        <div class="d-flex flex-wrap align-items-center justify-content-between mb-4">
            <div class="d-flex align-items-center"><span class="reports-title-icon mr-3"><i class="mdi mdi-chart-areaspline"></i></span>
                <div>
                    <h2 class="mb-1">Estatísticas e Métricas</h2>
                    <p class="text-muted mb-0">Acompanhe a eficiência do suporte e a distribuição do trabalho.</p>
                </div>
            </div>
            <a class="btn btn-outline-primary reports-export-link" href="data_export.aspx"><i class="mdi mdi-download mr-1"></i> Exportar dados</a>
        </div>
        <section class="card mb-4 reports-filter-card" aria-label="Filtros do relatório">
            <div class="card-body">
                <div class="row align-items-end">
                    <div class="col-md-3">
                        <div class="form-group mb-md-0"><label for="metricPeriod">Período</label><select id="metricPeriod" class="form-control">
                                <option value="30">Últimos 30 dias</option>
                                <option value="90">Últimos 90 dias</option>
                                <option value="365">Último ano</option>
                            </select></div>
                    </div>
                    <div class="col-md-3">
                        <div class="form-group mb-md-0"><label for="metricType">Tipo</label><select id="metricType" class="form-control">
                                <option>Tickets e tarefas</option>
                                <option>Tickets</option>
                                <option>Tarefas</option>
                            </select></div>
                    </div>
                    <div class="col-md-3">
                        <div class="form-group mb-md-0"><label for="metricTechnician">Técnico</label><select id="metricTechnician" class="form-control">
                                <option>Todos os técnicos</option>
                            </select></div>
                    </div>
                    <div class="col-md-3"><button type="button" id="applyMetrics" class="btn btn-primary btn-block"><i class="mdi mdi-filter mr-1"></i> Aplicar filtros</button></div>
                </div>
            </div>
        </section>
        <section class="row" aria-label="Indicadores principais">
            <div class="col-sm-6 col-xl-3 grid-margin stretch-card">
                <article class="card report-kpi report-kpi-blue">
                    <div class="card-body">
                        <div class="report-kpi-icon"><i class="mdi mdi-ticket-outline" aria-hidden="true"></i></div><strong>—</strong><span>Tickets abertos</span><small>Sem dados disponíveis</small>
                    </div>
                </article>
            </div>
            <div class="col-sm-6 col-xl-3 grid-margin stretch-card">
                <article class="card report-kpi report-kpi-green">
                    <div class="card-body">
                        <div class="report-kpi-icon"><i class="mdi mdi-check-circle-outline" aria-hidden="true"></i></div><strong>—</strong><span>Tickets concluídos</span><small>Sem dados disponíveis</small>
                    </div>
                </article>
            </div>
            <div class="col-sm-6 col-xl-3 grid-margin stretch-card">
                <article class="card report-kpi report-kpi-purple">
                    <div class="card-body">
                        <div class="report-kpi-icon"><i class="mdi mdi-wrench" aria-hidden="true"></i></div><strong>—</strong><span>Intervenções</span><small>Sem dados disponíveis</small>
                    </div>
                </article>
            </div>
            <div class="col-sm-6 col-xl-3 grid-margin stretch-card">
                <article class="card report-kpi report-kpi-orange">
                    <div class="card-body">
                        <div class="report-kpi-icon"><i class="mdi mdi-timer-sand" aria-hidden="true"></i></div><strong>—</strong><span>Tempo médio de resolução</span><small>Sem dados disponíveis</small>
                    </div>
                </article>
            </div>
        </section>
        <div class="row">
            <div class="col-xl-8 grid-margin stretch-card">
                <section class="card w-100">
                    <div class="card-body">
                        <div class="report-card-heading">
                            <div>
                                <h4 class="card-title mb-1">Evolução de tickets</h4>
                                <p class="text-muted small mb-0">Abertos e concluídos ao longo do período</p>
                            </div><span class="badge badge-outline-primary">30 dias</span>
                        </div>
                        <div class="report-chart">
                            <div class="dashboard-empty"><i class="mdi mdi-chart-line"></i><strong>Sem dados estatísticos</strong><span>Os dados serão apresentados quando estiverem disponíveis.</span></div>
                        </div>
                    </div>
                </section>
            </div>
            <div class="col-xl-4 grid-margin stretch-card">
                <section class="card w-100">
                    <div class="card-body">
                        <div class="report-card-heading">
                            <div>
                                <h4 class="card-title mb-1">Tickets por estado</h4>
                                <p class="text-muted small mb-0">Distribuição atual</p>
                            </div>
                        </div>
                        <div class="report-chart report-chart-donut">
                            <div class="dashboard-empty"><i class="mdi mdi-chart-line"></i><strong>Sem dados estatísticos</strong><span>Os dados serão apresentados quando estiverem disponíveis.</span></div>
                        </div>
                    </div>
                </section>
            </div>
        </div>
        <div class="row">
            <div class="col-xl-7 grid-margin stretch-card">
                <section class="card w-100">
                    <div class="card-body">
                        <div class="report-card-heading">
                            <div>
                                <h4 class="card-title mb-1">Intervenções por técnico</h4>
                                <p class="text-muted small mb-0">Número de tarefas realizadas no período</p>
                            </div>
                        </div>
                        <div class="report-chart">
                            <div class="dashboard-empty"><i class="mdi mdi-chart-line"></i><strong>Sem dados estatísticos</strong><span>Os dados serão apresentados quando estiverem disponíveis.</span></div>
                        </div>
                    </div>
                </section>
            </div>
            <div class="col-xl-5 grid-margin stretch-card">
                <section class="card w-100">
                    <div class="card-body p-0">
                        <div class="report-card-heading px-4 pt-4">
                            <div>
                                <h4 class="card-title mb-1">Desempenho da equipa</h4>
                                <p class="text-muted small mb-0">Resumo por técnico</p>
                            </div>
                        </div>
                        <div class="table-responsive">
                            <table class="table report-table mb-0">
                                <thead>
                                    <tr>
                                        <th>Técnico</th>
                                        <th>Concluídas</th>
                                        <th>Tempo médio</th>
                                        <th>SLA</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td colspan="4">
                                            <div class="dashboard-empty"><i class="mdi mdi-account-group-outline"></i><strong>Sem dados de desempenho</strong><span>Não existem registos para apresentar.</span></div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </section>
            </div>
        </div>
    </div>
    <script>
        (function() {
            'use strict';
            var button = document.getElementById('applyMetrics');
            if (button) {
                button.addEventListener('click', function() {
                    button.innerHTML = '<i class="mdi mdi-check mr-1"></i> Filtros aplicados';
                    setTimeout(function() {
                        button.innerHTML = '<i class="mdi mdi-filter mr-1"></i> Aplicar filtros';
                    }, 1600);
                });
            }
        }());
    </script>
</asp:Content>
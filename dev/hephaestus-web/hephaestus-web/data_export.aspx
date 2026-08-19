<%@ Page Title="Exportação de Dados" Language="C#" MasterPageFile="~/hephaestus.Master" AutoEventWireup="true" CodeBehind="data_export.aspx.cs" Inherits="hephaestus_web.data_export" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="reports-page export-page">
        <div class="d-flex flex-wrap align-items-center justify-content-between mb-4">
            <div class="d-flex align-items-center"><span class="reports-title-icon mr-3"><i class="mdi mdi-database-export"></i></span>
                <div>
                    <h2 class="mb-1">Exportação de Dados</h2>
                    <p class="text-muted mb-0">Prepare relatórios estatísticos ou históricos com os filtros pretendidos.</p>
                </div>
            </div><a href="statistics_metrics.aspx" class="btn btn-outline-secondary reports-export-link"><i class="mdi mdi-chart-line mr-1"></i> Ver métricas</a>
        </div>
        <div class="row">
            <div class="col-xl-8 grid-margin stretch-card">
                <section class="card w-100 export-builder">
                    <div class="card-body">
                        <div class="export-step"><span>1</span>
                            <div>
                                <h4>Escolha os dados</h4>
                                <p>Selecione o conteúdo a incluir no ficheiro.</p>
                            </div>
                        </div>
                        <div class="row mb-4">
                            <div class="col-md-6"><label class="export-choice active"><input type="radio" name="dataset" checked /><i class="mdi mdi-ticket-confirmation"></i><span><strong>Histórico de tickets</strong><small>Tickets e respetivos dados operacionais</small></span></label></div>
                            <div class="col-md-6"><label class="export-choice"><input type="radio" name="dataset" /><i class="mdi mdi-chart-box"></i><span><strong>Relatório estatístico</strong><small>Métricas e desempenho da equipa</small></span></label></div>
                        </div>
                        <div class="export-step"><span>2</span>
                            <div>
                                <h4>Defina os filtros</h4>
                                <p>Os filtros aplicados serão incluídos no relatório.</p>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group"><label for="exportPeriod">Período</label><select id="exportPeriod" class="form-control">
                                        <option>Últimos 30 dias</option>
                                        <option>Últimos 90 dias</option>
                                        <option>Último ano</option>
                                        <option>Personalizado</option>
                                    </select></div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group"><label for="exportType">Tipo</label><select id="exportType" class="form-control">
                                        <option>Todos</option>
                                        <option>Support (SUP)</option>
                                        <option>Service (SVC)</option>
                                        <option>Intervention (INT)</option>
                                    </select></div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group"><label for="exportState">Estado</label><select id="exportState" class="form-control">
                                        <option>Todos os estados</option>
                                        <option>Open</option>
                                        <option>Assigned</option>
                                        <option>Pending</option>
                                        <option>WIP</option>
                                        <option>Resolved</option>
                                        <option>Closed</option>
                                    </select></div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group"><label for="exportTechnician">Técnico</label><select id="exportTechnician" class="form-control">
                                        <option>Todos os técnicos</option>
                                    </select></div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group"><label for="exportPriority">Prioridade</label><select id="exportPriority" class="form-control">
                                        <option>Todas</option>
                                        <option>P1</option>
                                        <option>P2</option>
                                        <option>P3</option>
                                        <option>P4</option>
                                    </select></div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group"><label for="exportUser">Utilizador</label><input id="exportUser" class="form-control" type="search" placeholder="Nome do utilizador" /></div>
                            </div>
                        </div>
                        <div class="export-step mt-2"><span>3</span>
                            <div>
                                <h4>Selecione o formato</h4>
                                <p>Escolha o formato adequado ao destino dos dados.</p>
                            </div>
                        </div>
                        <div class="export-formats"><label class="export-format active"><input type="radio" name="format" value="CSV" checked /><i class="mdi mdi-file-delimited text-success"></i><strong>CSV</strong><small>Análise em folhas de cálculo</small></label><label class="export-format"><input type="radio" name="format" value="PDF" /><i class="mdi mdi-file-pdf text-danger"></i><strong>PDF</strong><small>Relatório pronto a partilhar</small></label><label class="export-format"><input type="radio" name="format" value="XML" /><i class="mdi mdi-code-tags text-warning"></i><strong>XML</strong><small>Integração entre sistemas</small></label></div>
                    </div>
                </section>
            </div>
            <div class="col-xl-4 grid-margin">
                <aside class="card export-summary">
                    <div class="card-body">
                        <h4 class="card-title">Resumo da exportação</h4>
                        <dl>
                            <div>
                                <dt>Conteúdo</dt>
                                <dd id="summaryDataset">Histórico de tickets</dd>
                            </div>
                            <div>
                                <dt>Período</dt>
                                <dd id="summaryPeriod">Últimos 30 dias</dd>
                            </div>
                            <div>
                                <dt>Formato</dt>
                                <dd id="summaryFormat"><span class="badge badge-outline-success">CSV</span></dd>
                            </div>
                            <div>
                                <dt>Registos estimados</dt>
                                <dd>—</dd>
                            </div>
                        </dl>
                        <div class="export-notice"><i class="mdi mdi-information-outline"></i><span>O ficheiro incluirá a data de geração e a identificação dos filtros aplicados.</span></div><button type="button" id="generateExport" class="btn btn-primary btn-block btn-lg mt-4"><i class="mdi mdi-download mr-1"></i> Gerar exportação</button>
                        <p id="exportFeedback" class="export-feedback" role="status" aria-live="polite"></p>
                    </div>
                </aside>
            </div>
        </div>
    </div>
    <script>
        (function() {
            'use strict';
            var choices = document.querySelectorAll('.export-choice'),
                formats = document.querySelectorAll('.export-format');

            function select(items, current) {
                for (var i = 0; i < items.length; i++) items[i].classList.remove('active');
                current.classList.add('active');
            }
            for (var i = 0; i < choices.length; i++) choices[i].addEventListener('click', function() {
                select(choices, this);
                document.getElementById('summaryDataset').textContent = this.querySelector('strong').textContent;
            });
            for (var j = 0; j < formats.length; j++) formats[j].addEventListener('click', function() {
                select(formats, this);
                var v = this.querySelector('input').value,
                    color = v === 'CSV' ? 'success' : v === 'PDF' ? 'danger' : 'warning';
                document.getElementById('summaryFormat').innerHTML = '<span class="badge badge-outline-' + color + '">' + v + '</span>';
            });
            document.getElementById('exportPeriod').addEventListener('change', function() {
                document.getElementById('summaryPeriod').textContent = this.value;
            });
            document.getElementById('generateExport').addEventListener('click', function() {
                var b = this,
                    f = document.getElementById('exportFeedback');
                b.disabled = true;
                b.innerHTML = '<span class="spinner-border spinner-border-sm mr-1"></span> A preparar...';
                setTimeout(function() {
                    b.disabled = false;
                    b.innerHTML = '<i class="mdi mdi-download mr-1"></i> Gerar exportação';
                    f.textContent = 'Não existem dados disponíveis para exportar.';
                }, 900);
            });
        }());
    </script>
</asp:Content>
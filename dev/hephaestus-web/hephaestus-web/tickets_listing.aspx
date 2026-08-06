<%@ Page Title="Histórico de Tickets" Language="C#" MasterPageFile="~/hephaestus.Master" AutoEventWireup="true" CodeBehind="tickets_listing.aspx.cs" Inherits="hephaestus_web.tickets_listing" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="tickets-page">
        <div class="d-flex flex-wrap align-items-center justify-content-between mb-4">
            <div class="d-flex align-items-center">
                <span class="page-title-icon mr-3"><i class="mdi mdi-ticket-confirmation"></i></span>
                <div><h3 class="mb-1">Histórico de Tickets</h3><p class="text-muted mb-0">Consulte, filtre e ordene todos os pedidos registados</p></div>
            </div>
            <div class="listing-actions d-flex"><button type="button" class="btn btn-outline-light mr-2" title="Exportação disponível na fase de backend"><i class="mdi mdi-download mr-1"></i> Exportar</button><button type="button" class="btn btn-success"><i class="mdi mdi-plus mr-1"></i> Abrir Ticket</button></div>
        </div>
        <div class="card filter-card mb-4">
            <div class="card-body">
                <div class="d-flex justify-content-between align-items-center mb-3"><h4 class="card-title mb-0"><i class="mdi mdi-filter-variant mr-2 text-primary"></i>Filtros cumulativos</h4><button type="button" id="clearFilters" class="btn btn-link text-muted btn-sm">Limpar filtros</button></div>
                <div class="row">
                    <div class="col-lg-3 col-md-6"><div class="form-group"><label for="referenceFilter">Código / título</label><div class="input-group"><div class="input-group-prepend"><span class="input-group-text"><i class="mdi mdi-magnify"></i></span></div><input id="referenceFilter" class="form-control" type="search" placeholder="SUP001 ou assunto" /></div></div></div>
                    <div class="col-lg-2 col-md-3"><div class="form-group"><label for="typeFilter">Tipo</label><select id="typeFilter" class="form-control"><option value="">Todos</option><option value="SUP">Support</option><option value="SVC">Service</option></select></div></div>
                    <div class="col-lg-2 col-md-3"><div class="form-group"><label for="statusFilter">Estado</label><select id="statusFilter" class="form-control"><option value="">Todos</option><option>Open</option><option>Assigned</option><option>Pending</option><option>WIP</option><option>Resolved</option><option>Closed</option></select></div></div>
                    <div class="col-lg-3 col-md-6"><div class="form-group"><label for="technicianFilter">Técnico</label><select id="technicianFilter" class="form-control"><option value="">Todos</option><option>Por atribuir</option></select></div></div>
                    <div class="col-lg-2 col-md-3"><div class="form-group"><label for="priorityFilter">Prioridade</label><select id="priorityFilter" class="form-control"><option value="">Todas</option><option>P1</option><option>P2</option><option>P3</option><option>P4</option></select></div></div>
                    <div class="col-lg-3 col-md-4"><div class="form-group mb-lg-0"><label for="dateFromFilter">Abertura desde</label><input id="dateFromFilter" class="form-control" type="date" /></div></div>
                    <div class="col-lg-3 col-md-4"><div class="form-group mb-lg-0"><label for="dateToFilter">Abertura até</label><input id="dateToFilter" class="form-control" type="date" /></div></div>
                    <div class="col-lg-3 col-md-4"><div class="form-group mb-lg-0"><label for="userFilter">Utilizador</label><input id="userFilter" class="form-control" type="search" placeholder="Nome do requerente" /></div></div>
                    <div class="col-lg-3 d-flex align-items-end"><button type="button" id="applyFilters" class="btn btn-primary btn-block mb-3 mb-lg-0"><i class="mdi mdi-filter mr-1"></i> Aplicar filtros</button></div>
                </div>
                <div id="activeFilters" class="mt-3" aria-live="polite"></div>
            </div>
        </div>
        <div class="card">
            <div class="card-body p-0">
                <div class="d-flex flex-wrap justify-content-between align-items-center px-4 py-3 border-bottom">
                    <div><h4 class="card-title mb-1">Tickets</h4><span id="resultSummaryTop" class="results-summary"></span></div>
                    <div class="d-flex align-items-center"><label for="pageSize" class="text-muted small mb-0 mr-2">Por página</label><select id="pageSize" class="form-control form-control-sm page-size-select"><option>15</option><option>25</option><option>50</option><option>100</option></select></div>
                </div>
                <div class="table-responsive">
                    <table class="table ticket-table mb-0">
                        <thead><tr><th data-sort="code">Ticket <i class="mdi mdi-swap-vertical"></i></th><th data-sort="title">Título</th><th data-sort="type">Tipo</th><th data-sort="status">Estado</th><th data-sort="technician">Técnico</th><th data-sort="priority">Prioridade</th><th data-sort="date">Data de abertura</th><th data-sort="user">Utilizador</th><th class="no-sort"></th></tr></thead>
                        <tbody id="ticketRows"></tbody>
                    </table>
                </div>
                <div class="d-flex flex-wrap justify-content-between align-items-center px-4 py-3 border-top"><span id="resultSummaryBottom" class="results-summary mb-2 mb-md-0"></span><nav aria-label="Paginação dos tickets"><ul id="pagination" class="pagination pagination-sm mb-0"></ul></nav></div>
            </div>
        </div>
    </div>
    <script>
        (function () {
            'use strict';
            var tickets = window.hephaestusTickets || [];
            var filtered = tickets.slice(), page = 1, sortKey = 'date', sortDirection = -1;
            var controls = { search: document.getElementById('referenceFilter'), type: document.getElementById('typeFilter'), status: document.getElementById('statusFilter'), technician: document.getElementById('technicianFilter'), priority: document.getElementById('priorityFilter'), from: document.getElementById('dateFromFilter'), to: document.getElementById('dateToFilter'), user: document.getElementById('userFilter') };
            Array.from(new Set(tickets.map(function (ticket) { return ticket.technician; }).filter(function (name) { return name && name !== 'Por atribuir'; }))).sort().forEach(function (name) {
                var option = document.createElement('option'); option.value = name; option.textContent = name; controls.technician.appendChild(option);
            });
            function escapeHtml(value) { var div = document.createElement('div'); div.textContent = value; return div.innerHTML; }
            function statusClass(status) { return status.toLowerCase().replace(' ', '-'); }
            function formatDate(value) { return new Date(value + 'T12:00:00').toLocaleDateString('pt-PT', { day: '2-digit', month: 'short', year: 'numeric' }); }
            function currentValues() { var values = {}; Object.keys(controls).forEach(function (key) { values[key] = controls[key].value.trim(); }); return values; }
            function applyFilters() {
                var v = currentValues(), query = v.search.toLowerCase();
                filtered = tickets.filter(function (t) {
                    return (!query || t.code.toLowerCase().includes(query) || t.title.toLowerCase().includes(query)) && (!v.type || t.type === v.type) && (!v.status || t.status === v.status) && (!v.technician || t.technician === v.technician) && (!v.priority || t.priority === v.priority) && (!v.from || t.date >= v.from) && (!v.to || t.date <= v.to) && (!v.user || t.user.toLowerCase().includes(v.user.toLowerCase()));
                });
                page = 1; renderFilters(); render();
            }
            function renderFilters() {
                var v = currentValues(), labels = { search: 'Pesquisa', type: 'Tipo', status: 'Estado', technician: 'Técnico', priority: 'Prioridade', from: 'Desde', to: 'Até', user: 'Utilizador' }, html = '';
                Object.keys(v).forEach(function (key) { if (v[key]) html += '<span class="active-filter">' + labels[key] + ': ' + escapeHtml(v[key]) + '<button type="button" data-clear="' + key + '" aria-label="Remover filtro ' + labels[key] + '"><i class="mdi mdi-close"></i></button></span>'; });
                document.getElementById('activeFilters').innerHTML = html || '<span class="text-muted small">Nenhum filtro aplicado.</span>';
            }
            function render() {
                filtered.sort(function (a, b) { var av = a[sortKey], bv = b[sortKey]; return av === bv ? 0 : (av > bv ? 1 : -1) * sortDirection; });
                var pageSize = +document.getElementById('pageSize').value, totalPages = Math.max(1, Math.ceil(filtered.length / pageSize));
                if (page > totalPages) page = totalPages;
                var start = (page - 1) * pageSize, pageItems = filtered.slice(start, start + pageSize), html = '';
                pageItems.forEach(function (t) {
                    html += '<tr><td><a href="#" class="ticket-code" data-ticket="' + t.code + '">' + t.code + '</a></td><td><div class="ticket-title" title="' + escapeHtml(t.title) + '">' + escapeHtml(t.title) + '</div><div class="ticket-subtitle">Pedido de helpdesk</div></td><td><span class="type-icon type-' + t.type.toLowerCase() + '"><i class="mdi ' + (t.type === 'SUP' ? 'mdi-lifebuoy' : 'mdi-tools') + '"></i></span>' + (t.type === 'SUP' ? 'Support' : 'Service') + '</td><td><span class="badge status-badge status-' + statusClass(t.status) + '">' + t.status + '</span></td><td>' + escapeHtml(t.technician) + '</td><td><span class="priority priority-' + t.priority.toLowerCase() + '">' + t.priority + '</span></td><td>' + formatDate(t.date) + '</td><td>' + escapeHtml(t.user) + '</td><td><button type="button" class="btn btn-outline-secondary btn-icon btn-sm" title="Abrir ' + t.code + '" data-ticket="' + t.code + '"><i class="mdi mdi-chevron-right"></i></button></td></tr>';
                });
                if (!html) html = '<tr><td colspan="9" class="empty-state"><i class="mdi mdi-magnify d-block h2"></i><div class="text-white mb-1">Nenhum ticket encontrado</div><small>Ajuste ou remova alguns filtros.</small></td></tr>';
                document.getElementById('ticketRows').innerHTML = html;
                var first = filtered.length ? start + 1 : 0, last = Math.min(start + pageSize, filtered.length), summary = 'A mostrar ' + first + '–' + last + ' de ' + filtered.length + ' tickets';
                document.getElementById('resultSummaryTop').textContent = filtered.length + ' resultado(s)'; document.getElementById('resultSummaryBottom').textContent = summary;
                renderPagination(totalPages);
            }
            function renderPagination(totalPages) {
                var html = '<li class="page-item ' + (page === 1 ? 'disabled' : '') + '"><button type="button" class="page-link" data-page="' + (page - 1) + '" aria-label="Anterior"><i class="mdi mdi-chevron-left"></i></button></li>';
                var from = Math.max(1, page - 2), to = Math.min(totalPages, from + 4); from = Math.max(1, to - 4);
                if (from > 1) html += '<li class="page-item"><button type="button" class="page-link" data-page="1">1</button></li>' + (from > 2 ? '<li class="page-item disabled"><span class="page-link">…</span></li>' : '');
                for (var p = from; p <= to; p++) html += '<li class="page-item ' + (p === page ? 'active' : '') + '"><button type="button" class="page-link" data-page="' + p + '">' + p + '</button></li>';
                if (to < totalPages) html += (to < totalPages - 1 ? '<li class="page-item disabled"><span class="page-link">…</span></li>' : '') + '<li class="page-item"><button type="button" class="page-link" data-page="' + totalPages + '">' + totalPages + '</button></li>';
                html += '<li class="page-item ' + (page === totalPages ? 'disabled' : '') + '"><button type="button" class="page-link" data-page="' + (page + 1) + '" aria-label="Seguinte"><i class="mdi mdi-chevron-right"></i></button></li>';
                document.getElementById('pagination').innerHTML = html;
            }
            document.getElementById('applyFilters').addEventListener('click', applyFilters);
            [controls.search, controls.user].forEach(function (control) { control.addEventListener('keydown', function (e) { if (e.key === 'Enter') { e.preventDefault(); applyFilters(); } }); });
            document.getElementById('clearFilters').addEventListener('click', function () { Object.keys(controls).forEach(function (key) { controls[key].value = ''; }); applyFilters(); });
            document.getElementById('activeFilters').addEventListener('click', function (e) { var button = e.target.closest('[data-clear]'); if (button) { controls[button.getAttribute('data-clear')].value = ''; applyFilters(); } });
            document.getElementById('pageSize').addEventListener('change', function () { page = 1; render(); });
            document.getElementById('pagination').addEventListener('click', function (e) { var button = e.target.closest('[data-page]'); if (button && !button.parentElement.classList.contains('disabled')) { page = +button.getAttribute('data-page'); render(); } });
            document.querySelector('.ticket-table thead').addEventListener('click', function (e) { var th = e.target.closest('[data-sort]'); if (th) { var key = th.getAttribute('data-sort'); sortDirection = sortKey === key ? -sortDirection : 1; sortKey = key; render(); } });
            document.getElementById('ticketRows').addEventListener('click', function (e) { var target = e.target.closest('[data-ticket]'); if (target) { e.preventDefault(); window.alert('A abrir o ticket ' + target.getAttribute('data-ticket') + '. A ligação ao detalhe será concluída na integração com o backend.'); } });
            renderFilters(); render();
        }());
    </script>
</asp:Content>

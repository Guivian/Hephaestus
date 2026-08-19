<%@ Page Title="Abrir Ticket" Language="C#" MasterPageFile="~/hephaestus.Master" AutoEventWireup="true" CodeBehind="ticket_create.aspx.cs" Inherits="hephaestus_web.ticket_create" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="ticket-workspace ticket-create-page">
        <div class="d-flex flex-wrap align-items-center justify-content-between mb-4">
            <div class="d-flex align-items-center"><span class="page-title-icon mr-3"><i class="mdi mdi-ticket-account"></i></span>
                <div>
                    <h3 class="mb-1">Abrir novo ticket</h3>
                    <p class="text-muted mb-0">Registe um pedido de suporte ou de serviço</p>
                </div>
            </div>
            <a href="tickets_listing.aspx" class="btn btn-outline-light mt-3 mt-md-0"><i class="mdi mdi-arrow-left mr-1"></i> Voltar à listagem</a>
        </div>
        <div class="row">
            <div class="col-xl-8">
                <div class="card">
                    <div class="card-body">
                        <div class="workspace-section-heading"><span>1</span>
                            <div>
                                <h4>Tipo de pedido</h4>
                                <p>Esta escolha define a referência e as prioridades disponíveis.</p>
                            </div>
                        </div>
                        <div class="row ticket-type-options mb-4">
                            <div class="col-md-6"><label class="ticket-type-card active" for="typeSupport"><input id="typeSupport" name="ticketType" type="radio" value="SUP" checked /><span class="type-icon type-sup"><i class="mdi mdi-lifebuoy"></i></span><span><strong>Suporte</strong><small>Avaria, erro ou problema técnico</small><em>Referência SUPXXX</em></span><i class="mdi mdi-check-circle selection-check"></i></label></div>
                            <div class="col-md-6"><label class="ticket-type-card" for="typeService"><input id="typeService" name="ticketType" type="radio" value="SVC" /><span class="type-icon type-svc"><i class="mdi mdi-tools"></i></span><span><strong>Serviço</strong><small>Instalação, atualização, recolha ou pedido planeado</small><em>Referência SVCXXX</em></span><i class="mdi mdi-check-circle selection-check"></i></label></div>
                        </div>
                        <div class="workspace-section-heading"><span>2</span>
                            <div>
                                <h4>Detalhes do pedido</h4>
                                <p>Descreva claramente o que necessita.</p>
                            </div>
                        </div>
                        <div class="form-group"><label for="ticketTitle">Título <span class="text-danger">*</span></label><input id="ticketTitle" class="form-control" maxlength="200" required placeholder="Ex.: Computador não inicia após atualização" />
                            <div class="d-flex justify-content-between mt-1"><small class="text-muted">Um resumo curto ajuda a encaminhar o pedido.</small><small id="titleCount" class="text-muted">0/200</small></div>
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group"><label for="ticketEquipment">Equipamento <span class="text-danger">*</span></label><input id="ticketEquipment" class="form-control" maxlength="150" required placeholder="Identificação do equipamento" /><small class="text-muted">Nome, código patrimonial ou identificação do equipamento.</small></div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group"><label for="ticketLocation">Localidade <span class="text-danger">*</span></label><select id="ticketLocation" class="form-control" required>
                                        <option value="">Selecione uma localidade</option>
                                        <option>Lisboa</option>
                                        <option>Porto</option>
                                        <option>Remoto</option>
                                        <option>Outra localização</option>
                                    </select><small class="text-muted">Local onde será prestado o suporte ou serviço.</small></div>
                            </div>
                        </div>
                        <div class="form-group"><label for="ticketDescription">Descrição <span class="text-danger">*</span></label><textarea id="ticketDescription" class="form-control" rows="7" required placeholder="Indique o equipamento ou serviço, sintomas, mensagens de erro e quando começou..."></textarea></div>
                        <div class="form-group mb-0"><label>Prioridade sugerida <span class="text-danger">*</span></label>
                            <div id="priorityOptions" class="priority-options"></div><small class="text-muted d-block mt-2"><i class="mdi mdi-information-outline mr-1"></i>A prioridade será confirmada pelo técnico responsável.</small>
                        </div>
                    </div>
                </div>
                <div class="card mt-4">
                    <div class="card-body">
                        <div class="workspace-section-heading"><span>3</span>
                            <div>
                                <h4>Anexos</h4>
                                <p>Adicione imagens, documentos ou registos que ajudem no diagnóstico.</p>
                            </div>
                        </div>
                        <label class="upload-zone" for="ticketFiles"><input id="ticketFiles" type="file" multiple /><i class="mdi mdi-cloud-upload-outline"></i><strong>Arraste ficheiros para aqui ou clique para selecionar</strong><small>Máximo sugerido: 10 MB por ficheiro</small></label>
                        <div id="selectedFiles" class="selected-files"></div>
                    </div>
                </div>
                <div class="d-flex justify-content-end mt-4"><a href="tickets_listing.aspx" class="btn btn-outline-secondary mr-2">Cancelar</a><button id="submitTicket" type="button" class="btn btn-success"><i class="mdi mdi-check mr-1"></i> Criar ticket</button></div>
            </div>
            <div class="col-xl-4 mt-4 mt-xl-0">
                <div class="card sticky-summary">
                    <div class="card-body">
                        <h4 class="card-title"><i class="mdi mdi-clipboard-text-outline text-primary mr-2"></i>Resumo</h4>
                        <div class="summary-reference"><small>Nova referência</small><strong id="summaryReference">SUP — automática</strong></div>
                        <dl class="ticket-summary-list">
                            <dt>Tipo</dt>
                            <dd id="summaryType">Suporte</dd>
                            <dt>Prioridade</dt>
                            <dd id="summaryPriority">Por selecionar</dd>
                            <dt>Equipamento</dt>
                            <dd id="summaryEquipment">Por indicar</dd>
                            <dt>Localidade</dt>
                            <dd id="summaryLocation">Por indicar</dd>
                            <dt>Estado inicial</dt>
                            <dd><span class="badge status-badge status-open">Open</span></dd>
                            <dt>Criado por</dt>
                            <dd>Utilizador autenticado</dd>
                        </dl>
                        <div class="workspace-note"><i class="mdi mdi-email-outline"></i>
                            <p>Receberá uma notificação por e-mail sempre que o ticket for atualizado.</p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script>
        (function() {
            'use strict';
            var priorities = {
                SUP: [
                    ['P1', 'Crítico', 'Serviço indisponível ou impacto total'],
                    ['P2', 'Alto', 'Impacto significativo, sem alternativa'],
                    ['P3', 'Médio', 'Impacto limitado ou com alternativa'],
                    ['P4', 'Baixo', 'Questão menor ou sem urgência']
                ],
                SVC: [
                    ['P1', 'Urgente', 'Necessário com máxima brevidade'],
                    ['P2', 'Prioritário', 'Necessário em curto prazo'],
                    ['P3', 'Normal', 'Pedido com planeamento regular'],
                    ['P4', 'Planeado', 'Sem urgência, data flexível']
                ]
            };
            var current = 'SUP';

            function renderPriorities() {
                document.getElementById('priorityOptions').innerHTML = priorities[current].map(function(p, i) {
                    return '<label class="priority-option ' + (i === 2 ? 'active' : '') + '"><input type="radio" name="priority" value="' + p[0] + '" ' + (i === 2 ? 'checked' : '') + '><span class="priority priority-' + p[0].toLowerCase() + '">' + p[0] + '</span><strong>' + p[1] + '</strong><small>' + p[2] + '</small></label>';
                }).join('');
                updateSummary();
            }

            function updateSummary() {
                var selected = document.querySelector('input[name="priority"]:checked');
                document.getElementById('summaryReference').textContent = current + ' — automática';
                document.getElementById('summaryType').textContent = current === 'SUP' ? 'Suporte' : 'Serviço';
                document.getElementById('summaryPriority').textContent = selected ? selected.value + ' · ' + selected.parentElement.querySelector('strong').textContent : 'Por selecionar';
            }
            document.querySelectorAll('input[name="ticketType"]').forEach(function(r) {
                r.addEventListener('change', function() {
                    current = this.value;
                    document.querySelectorAll('.ticket-type-card').forEach(function(c) {
                        c.classList.remove('active');
                    });
                    this.parentElement.classList.add('active');
                    renderPriorities();
                });
            });
            document.getElementById('priorityOptions').addEventListener('change', function(e) {
                if (e.target.name === 'priority') {
                    document.querySelectorAll('.priority-option').forEach(function(c) {
                        c.classList.remove('active');
                    });
                    e.target.parentElement.classList.add('active');
                    updateSummary();
                }
            });
            document.getElementById('ticketTitle').addEventListener('input', function() {
                document.getElementById('titleCount').textContent = this.value.length + '/200';
            });
            document.getElementById('ticketEquipment').addEventListener('input', function() {
                document.getElementById('summaryEquipment').textContent = this.value.trim() || 'Por indicar';
            });
            document.getElementById('ticketLocation').addEventListener('change', function() {
                document.getElementById('summaryLocation').textContent = this.value || 'Por indicar';
            });
            document.getElementById('ticketFiles').addEventListener('change', function() {
                document.getElementById('selectedFiles').innerHTML = Array.from(this.files).map(function(f) {
                    return '<span><i class="mdi mdi-file-outline"></i>' + f.name + '<small>' + Math.max(1, Math.round(f.size / 1024)) + ' KB</small></span>';
                }).join('');
            });
            document.getElementById('submitTicket').addEventListener('click', function() {
                var title = document.getElementById('ticketTitle'),
                    equipment = document.getElementById('ticketEquipment'),
                    location = document.getElementById('ticketLocation'),
                    description = document.getElementById('ticketDescription');
                if (!title.value.trim() || !equipment.value.trim() || !location.value || !description.value.trim()) {
                    [title, equipment, location, description].forEach(function(c) {
                        c.classList.toggle('is-invalid', !c.value.trim());
                    });
                    return;
                }
                window.alert('Protótipo front-end validado. A gravação e geração da referência serão ligadas ao backend C#.');
            });
            var requestedType = new URLSearchParams(window.location.search).get('type');
            if (requestedType === 'SVC') {
                current = 'SVC';
                document.getElementById('typeService').checked = true;
                document.querySelectorAll('.ticket-type-card').forEach(function(c) {
                    c.classList.remove('active');
                });
                document.getElementById('typeService').parentElement.classList.add('active');
            }
            renderPriorities();
        }());
    </script>
</asp:Content>
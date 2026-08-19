<%@ Page Title="Anexos do Ticket" Language="C#" MasterPageFile="~/hephaestus.Master" AutoEventWireup="true" CodeBehind="ticket_attachments.aspx.cs" Inherits="hephaestus_web.ticket_attachments" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="ticket-workspace detail-page">
        <nav class="workspace-breadcrumb mb-3"><a href="tickets_listing.aspx">Tickets e tarefas</a><i class="mdi mdi-chevron-right"></i><a href="ticket_detail.aspx">—</a><i class="mdi mdi-chevron-right"></i><span>Anexos</span></nav>
        <div class="d-flex flex-wrap justify-content-between align-items-center mb-4">
            <div>
                <div class="d-flex align-items-center"><span class="page-title-icon mr-3"><i class="mdi mdi-paperclip"></i></span>
                    <div>
                        <h3 class="mb-1">Anexos de —</h3>
                        <p class="text-muted mb-0">Ficheiros partilhados no ticket e nas tarefas associadas</p>
                    </div>
                </div>
            </div><a class="btn btn-outline-light mt-3 mt-md-0" href="ticket_detail.aspx"><i class="mdi mdi-arrow-left mr-1"></i> Voltar ao detalhe</a>
        </div>
        <div class="workspace-tabs mb-4"><a href="ticket_detail.aspx"><i class="mdi mdi-information-outline"></i>Detalhe</a><a class="active" href="ticket_attachments.aspx"><i class="mdi mdi-paperclip"></i>Anexos <span>0</span></a><a href="ticket_history.aspx"><i class="mdi mdi-message-text-outline"></i>Comentários / Histórico <span>0</span></a></div>
        <div class="row">
            <div class="col-xl-8">
                <div class="card mb-4">
                    <div class="card-body">
                        <div class="d-flex justify-content-between align-items-center mb-3">
                            <h4 class="card-title mb-0">Adicionar ficheiros</h4><span class="text-muted small">Associar a —</span>
                        </div><label class="upload-zone" for="attachmentFiles"><input id="attachmentFiles" type="file" multiple /><i class="mdi mdi-cloud-upload-outline"></i><strong>Arraste ficheiros para aqui ou clique para selecionar</strong><small>Imagens, PDF e documentos · máximo sugerido 10 MB</small></label>
                        <div id="attachmentQueue" class="selected-files"></div>
                        <div id="uploadActions" class="text-right mt-3 d-none"><button id="clearFiles" class="btn btn-outline-secondary mr-2" type="button">Limpar</button><button class="btn btn-success" type="button"><i class="mdi mdi-upload mr-1"></i> Carregar ficheiros</button></div>
                    </div>
                </div>
                <div class="card">
                    <div class="card-body p-0">
                        <div class="attachment-toolbar">
                            <div>
                                <h4 class="card-title mb-1">Ficheiros (0)</h4><small class="text-muted">0 bytes utilizados</small>
                            </div>
                            <div class="input-group attachment-search"><input id="fileSearch" class="form-control" placeholder="Pesquisar ficheiro" />
                                <div class="input-group-append"><span class="input-group-text"><i class="mdi mdi-magnify"></i></span></div>
                            </div>
                        </div>
                        <div id="attachmentList" class="attachment-list">
                        </div>
                        <div id="noFiles" class="empty-state"><i class="mdi mdi-file-search-outline d-block h2"></i>Ainda não existem anexos associados.</div>
                    </div>
                </div>
            </div>
            <aside class="col-xl-4 mt-4 mt-xl-0">
                <div class="card">
                    <div class="card-body">
                        <h4 class="card-title">Regras de anexos</h4>
                        <ul class="workspace-guidelines">
                            <li><i class="mdi mdi-shield-check text-success"></i><span>Evite incluir palavras-passe ou dados confidenciais.</span></li>
                            <li><i class="mdi mdi-file-check-outline text-primary"></i><span>A ficha de intervenção é gerada automaticamente por tarefa.</span></li>
                            <li><i class="mdi mdi-history text-warning"></i><span>Cada adição ou remoção fica registada no histórico.</span></li>
                        </ul>
                    </div>
                </div>
            </aside>
        </div>
    </div>
    <script>
        (function() {
            var input = document.getElementById('attachmentFiles'),
                queue = document.getElementById('attachmentQueue'),
                actions = document.getElementById('uploadActions');

            function render() {
                queue.innerHTML = Array.from(input.files).map(function(f) {
                    return '<span><i class="mdi mdi-file-outline"></i>' + f.name + '<small>' + Math.max(1, Math.round(f.size / 1024)) + ' KB</small></span>';
                }).join('');
                actions.classList.toggle('d-none', !input.files.length);
            }
            input.addEventListener('change', render);
            document.getElementById('clearFiles').addEventListener('click', function() {
                input.value = '';
                render();
            });
            document.getElementById('fileSearch').addEventListener('input', function() {
                var q = this.value.toLowerCase(),
                    shown = 0;
                document.querySelectorAll('.attachment-row').forEach(function(row) {
                    var visible = row.dataset.name.toLowerCase().includes(q);
                    row.classList.toggle('d-none', !visible);
                    if (visible) shown++;
                });
                document.getElementById('noFiles').classList.toggle('d-none', shown !== 0);
            });
        }());
    </script>
</asp:Content>
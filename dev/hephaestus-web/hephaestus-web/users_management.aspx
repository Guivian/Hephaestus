<%@ Page Title="Gestão de Utilizadores" Language="C#" MasterPageFile="~/hephaestus.Master" AutoEventWireup="true" CodeBehind="users_management.aspx.cs" Inherits="hephaestus_web.users_management" %>
<asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="admin-users-page">
        <div class="d-flex flex-wrap align-items-center justify-content-between mb-4">
            <div class="d-flex align-items-center"><span class="page-title-icon mr-3"><i class="mdi mdi-account-multiple"></i></span>
                <div>
                    <h3 class="mb-1">Gestão de Utilizadores</h3>
                    <p class="text-muted mb-0">Administre utilizadores, técnicos e níveis de acesso</p>
                </div>
            </div>
            <div class="admin-page-actions d-flex"><a class="btn btn-success" href="user_form.aspx"><i class="mdi mdi-account-plus mr-1"></i>Nova conta</a></div>
        </div>
        <div class="row">
            <div class="col-xl-3 col-sm-6 grid-margin">
                <div class="card admin-kpi">
                    <div class="card-body"><small>Total de contas</small><strong id="kpiTotal">0</strong><span class="text-muted small">Todos os perfis</span></div>
                </div>
            </div>
            <div class="col-xl-3 col-sm-6 grid-margin">
                <div class="card admin-kpi kpi-success">
                    <div class="card-body"><small>Contas ativas</small><strong id="kpiActive">0</strong><span class="text-success small">Acesso permitido</span></div>
                </div>
            </div>
            <div class="col-xl-3 col-sm-6 grid-margin">
                <div class="card admin-kpi kpi-info">
                    <div class="card-body"><small>Técnicos</small><strong id="kpiTech">0</strong><span class="text-muted small">Lisboa e Porto</span></div>
                </div>
            </div>
            <div class="col-xl-3 col-sm-6 grid-margin">
                <div class="card admin-kpi kpi-warning">
                    <div class="card-body"><small>Contas inativas</small><strong id="kpiInactive">0</strong><span class="text-warning small">Podem ser reativadas</span></div>
                </div>
            </div>
        </div>
        <div class="card filter-card mb-4">
            <div class="card-body">
                <div class="row align-items-end">
                    <div class="col-lg-4 col-md-6">
                        <div class="form-group mb-lg-0"><label for="userSearch">Nome ou email</label>
                            <div class="input-group">
                                <div class="input-group-prepend"><span class="input-group-text"><i class="mdi mdi-magnify"></i></span></div><input id="userSearch" class="form-control" type="search" placeholder="Pesquisar conta" />
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-2 col-md-3">
                        <div class="form-group mb-lg-0"><label for="roleFilter">Perfil</label><select id="roleFilter" class="form-control">
                                <option value="">Todos</option>
                                <option>Admin</option>
                                <option>Gestor</option>
                                <option>Técnico</option>
                                <option>Utilizador</option>
                            </select></div>
                    </div>
                    <div class="col-lg-2 col-md-3">
                        <div class="form-group mb-lg-0"><label for="locationFilter">Localidade</label><select id="locationFilter" class="form-control">
                                <option value="">Todas</option>
                                <option>Lisboa</option>
                                <option>Porto</option>
                                <option value="—">Sem localidade</option>
                            </select></div>
                    </div>
                    <div class="col-lg-2 col-md-3">
                        <div class="form-group mb-lg-0"><label for="stateFilter">Estado</label><select id="stateFilter" class="form-control">
                                <option value="">Todos</option>
                                <option value="true">Ativa</option>
                                <option value="false">Inativa</option>
                            </select></div>
                    </div>
                    <div class="col-lg-2 col-md-3"><button id="clearAdminFilters" type="button" class="btn btn-outline-secondary btn-block">Limpar</button></div>
                </div>
            </div>
        </div>
        <div class="card">
            <div class="card-body p-0">
                <div class="d-flex justify-content-between align-items-center px-4 py-3 border-bottom">
                    <div>
                        <h4 class="card-title mb-1">Contas registadas</h4><span id="adminResults" class="results-summary"></span>
                    </div><span class="text-muted small"><i class="mdi mdi-database-outline mr-1"></i>A aguardar integração com o backend</span>
                </div>
                <div class="table-responsive">
                    <table class="table admin-user-table mb-0">
                        <thead>
                            <tr>
                                <th>Utilizador</th>
                                <th>Perfil</th>
                                <th>Localidade</th>
                                <th>2FA</th>
                                <th>Estado</th>
                                <th>Último acesso</th>
                                <th class="text-right">Ações</th>
                            </tr>
                        </thead>
                        <tbody id="adminUserRows"></tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
    <div id="adminFeedback" class="admin-feedback" aria-live="polite"></div>
    <div class="modal fade" id="accountStateModal" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content ticket-modal">
                <div class="modal-header">
                    <h5 id="stateModalTitle" class="modal-title">Alterar estado da conta</h5><button type="button" class="close text-white" data-dismiss="modal"><span>×</span></button>
                </div>
                <div class="modal-body">
                    <p id="stateModalText" class="text-muted mb-0"></p>
                </div>
                <div class="modal-footer"><button type="button" class="btn btn-outline-light" data-dismiss="modal">Cancelar</button><button id="confirmStateChange" type="button" class="btn btn-warning">Confirmar</button></div>
            </div>
        </div>
    </div>
    <script>
        (function() {
            var users = [];
            var pending = null,
                search = document.getElementById('userSearch'),
                role = document.getElementById('roleFilter'),
                location = document.getElementById('locationFilter'),
                state = document.getElementById('stateFilter');

            function esc(v) {
                var d = document.createElement('div');
                d.textContent = v;
                return d.innerHTML
            }

            function initials(n) {
                return n.split(' ').slice(0, 2).map(function(x) {
                    return x[0]
                }).join('').toUpperCase()
            }

            function cls(r) {
                return {
                    'Admin': 'admin',
                    'Gestor': 'manager',
                    'Técnico': 'technician',
                    'Utilizador': 'user'
                } [r]
            }

            function render() {
                document.getElementById('kpiTotal').textContent = users.length;
                document.getElementById('kpiActive').textContent = users.filter(function(u) {
                    return u.active
                }).length;
                document.getElementById('kpiTech').textContent = users.filter(function(u) {
                    return u.role === 'Técnico'
                }).length;
                document.getElementById('kpiInactive').textContent = users.filter(function(u) {
                    return !u.active
                }).length;
                var q = search.value.toLowerCase();
                var list = users.filter(function(u) {
                    return (!q || (u.name + ' ' + u.email).toLowerCase().indexOf(q) > -1) && (!role.value || u.role === role.value) && (!location.value || u.location === location.value) && (!state.value || String(u.active) === state.value)
                });
                document.getElementById('adminResults').textContent = list.length + ' de ' + users.length + ' contas';
                document.getElementById('adminUserRows').innerHTML = list.length ? list.map(function(u) {
                    return '<tr><td><div class="user-identity"><span class="user-avatar">' + initials(u.name) + '</span><span><strong>' + esc(u.name) + '</strong><small>' + esc(u.email) + '</small></span></div></td><td><span class="role-badge role-' + cls(u.role) + '">' + esc(u.role) + '</span></td><td>' + esc(u.location) + '</td><td><i class="mdi ' + (u.twofa ? 'mdi-shield-check text-success' : 'mdi-shield-off text-muted') + ' mr-1"></i>' + (u.twofa ? 'Ativo' : 'Inativo') + '</td><td><span class="account-status ' + (u.active ? 'account-active' : 'account-inactive') + '">' + (u.active ? 'Ativa' : 'Inativa') + '</span></td><td class="text-muted small">' + u.last + '</td><td><div class="admin-row-actions"><a class="btn btn-primary btn-sm" href="user_form.aspx?id=' + u.id + '" title="Editar"><i class="mdi mdi-pencil"></i></a><button class="btn btn-info btn-sm" data-reset="' + u.id + '" title="Redefinir palavra-passe"><i class="mdi mdi-lock-reset"></i></button><button class="btn btn-' + (u.active ? 'warning' : 'success') + ' btn-sm" data-state="' + u.id + '" title="' + (u.active ? 'Desativar' : 'Reativar') + '"><i class="mdi ' + (u.active ? 'mdi-account-off' : 'mdi-account-check') + '"></i></button></div></td></tr>'
                }).join('') : '<tr><td colspan="7" class="empty-state"><i class="mdi mdi-account-search d-block display-4 mb-2"></i>Nenhuma conta corresponde aos filtros.</td></tr>'
            }

            function feedback(t) {
                var e = document.getElementById('adminFeedback');
                e.textContent = t;
                e.classList.add('show');
                setTimeout(function() {
                    e.classList.remove('show')
                }, 2600)
            }
            [search, role, location, state].forEach(function(e) {
                e.addEventListener(e === search ? 'input' : 'change', render)
            });
            document.getElementById('clearAdminFilters').onclick = function() {
                search.value = role.value = location.value = state.value = '';
                render()
            };
            document.getElementById('adminUserRows').onclick = function(e) {
                var reset = e.target.closest('[data-reset]'),
                    button = e.target.closest('[data-state]');
                if (reset) {
                    feedback('Pedido de redefinição preparado para integração com o backend.')
                }
                if (button) {
                    pending = +button.dataset.state;
                    var u = users.filter(function(x) {
                        return x.id === pending
                    })[0];
                    document.getElementById('stateModalTitle').textContent = (u.active ? 'Desativar' : 'Reativar') + ' conta';
                    document.getElementById('stateModalText').textContent = (u.active ? 'A conta de ' + u.name + ' deixará de ter acesso à plataforma.' : 'A conta de ' + u.name + ' voltará a ter acesso à plataforma.');
                    $('#accountStateModal').modal('show')
                }
            };
            document.getElementById('confirmStateChange').onclick = function() {
                var u = users.filter(function(x) {
                    return x.id === pending
                })[0];
                u.active = !u.active;
                $('#accountStateModal').modal('hide');
                feedback('Conta ' + (u.active ? 'reativada' : 'desativada') + ' nesta demonstração.');
                render()
            };
            render();
        }());
    </script>
</asp:Content>
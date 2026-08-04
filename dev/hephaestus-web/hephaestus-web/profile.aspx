<%@ Page Title="" Language="C#" MasterPageFile="~/hephaestus.Master" AutoEventWireup="true" CodeBehind="profile.aspx.cs" Inherits="hephaestus_web.profile" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="d-flex align-items-center mb-4">
        <h3><asp:Label ID="lblPrimaryTitle" runat="server" Text="Definições de Conta" CssClass="page-title m-0"></asp:Label></h3>
        <h3><asp:Label ID="lblSecondaryTitle" runat="server" Text="Editar Informações de Conta" CssClass="page-title m-0" Visible="false"></asp:Label></h3>
    </div>

    <div class="row">
        <div class="col-12 grid-margin stretch-card">
            <div class="card">
                <div class="card-body">
                    <p class="card-description"> Informações do Utilizador </p>
                    <div class="forms-sample">
                        <div class="form-group mb-4">
                            <label class="d-block mb-2">Tipo de Conta</label>
                            <asp:Label ID="lblTipoConta" runat="server" CssClass="badge badge-outline-primary" style="font-size: 0.9rem; padding: 0.5rem 1rem;" Text="Administrador"></asp:Label>
                        </div>

                        <div class="form-group">
                            <label for="txtNome">Nome</label>
                            <asp:TextBox ID="txtNome" runat="server" CssClass="form-control readonly-field" Text="João Silva"></asp:TextBox>
                        </div>
                        
                        <div class="form-group">
                            <label for="txtEmail">Email</label>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control readonly-field" Text="joao.silva@example.com"></asp:TextBox>
                        </div>
                        
                        <div class="form-group">
                            <label for="txtLocalidade">Localidade</label>
                            <asp:TextBox ID="txtLocalidade" runat="server" CssClass="form-control readonly-field" Text="Lisboa, Portugal"></asp:TextBox>
                        </div>
                        
                        <div class="form-group">
                            <label class="d-block">Autenticação de 2 Fatores (2FA)</label>
                            <label class="custom-toggle-wrapper mt-1" id="wrapper2FA" runat="server">
                                <input type="checkbox" id="chk2FA" runat="server" />
                                <div class="toggle-switch-ui"></div>
                                <div class="toggle-status-text">
                                    <span class="lbl-on">Ativado</span>
                                    <span class="lbl-off">Desativado</span>
                                </div>
                            </label>
                        </div>
                        
                        <div class="form-group">
                            <label class="d-block">Estado da Conta</label>
                            <label class="custom-toggle-wrapper mt-1" id="wrapperEstado" runat="server">
                                <input type="checkbox" id="chkAtiva" runat="server" checked />
                                <div class="toggle-switch-ui"></div>
                                <div class="toggle-status-text">
                                    <span class="lbl-on">Ativa</span>
                                    <span class="lbl-off">Inativa</span>
                                </div>
                            </label>
                        </div>
                        
                        <div class="mt-4 d-flex justify-content-between">
                            <div>
                                <button type="button" class="btn btn-outline-danger" data-toggle="modal" data-target="#deactivateModal">Desativar conta</button>
                            </div>
                            <div class="text-right">
                                <asp:Button ID="btnEditar" runat="server" Text="Editar" CssClass="btn btn-primary" OnClick="btnEditar_Click" />
                                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn btn-danger mr-2" OnClick="btnCancelar_Click" Visible="false" />
                                <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-success" OnClick="btnGuardar_Click" Visible="false" />
                            </div>
                        </div>
                        
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="deactivateModal" tabindex="-1" role="dialog" aria-hidden="true">
      <div class="modal-dialog" role="document">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Confirmar Desativação</h5>
            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
              <span aria-hidden="true">&times;</span>
            </button>
          </div>
          <div class="modal-body">
            <div class="alert alert-warning" role="alert">
                <strong>Aviso:</strong> Esta é uma ação imediata. A sua sessão será terminada instantaneamente e apenas poderá reativar a sua conta se entrar em contacto direto com um Administrador ou Superior Hierárquico.
            </div>
            <p class="mt-3">Para confirmar a desativação da sua conta, por favor escreva <strong>D3S4T1V4R C0NT4</strong> abaixo.</p>
            <asp:TextBox ID="txtConfirmDeactivate" runat="server" CssClass="form-control" autocomplete="off"></asp:TextBox>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancelar</button>
            <asp:Button ID="btnConfirmDeactivate" runat="server" Text="Desativar Conta" CssClass="btn btn-danger" OnClick="btnConfirmDeactivate_Click" />
          </div>
        </div>
      </div>
    </div>
</asp:Content>

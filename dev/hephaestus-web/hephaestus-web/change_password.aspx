<%@ Page Title="" Language="C#" MasterPageFile="~/hephaestus.Master" AutoEventWireup="true" CodeBehind="change_password.aspx.cs" Inherits="hephaestus_web.change_password" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="d-flex align-items-center mb-4">
        <h3><span class="page-title m-0">Alterar Palavra-passe</span></h3>
    </div>

    <div class="row">
        <div class="col-md-6 grid-margin stretch-card">
            <div class="card">
                <div class="card-body">
                    <p class="card-description"> Crie uma nova palavra-passe segura. </p>
                    <div class="forms-sample">
                        <div class="form-group">
                            <label for="txtPasswordAtual">Palavra-passe Atual</label>
                            <asp:TextBox ID="txtPasswordAtual" runat="server" CssClass="form-control" TextMode="Password" placeholder="Palavra-passe atual"></asp:TextBox>
                        </div>
                        
                        <div class="form-group">
                            <label for="txtNovaPassword">Nova Palavra-passe</label>
                            <asp:TextBox ID="txtNovaPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Nova palavra-passe"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label for="txtConfirmarPassword">Confirmar Nova Palavra-passe</label>
                            <asp:TextBox ID="txtConfirmarPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Confirme a nova palavra-passe"></asp:TextBox>
                        </div>
                        
                        <div class="mt-4">
                            <asp:Button ID="btnGuardarPassword" runat="server" Text="Alterar Palavra-passe" CssClass="btn btn-primary mr-2" />
                            <a href="profile.aspx" class="btn btn-light">Cancelar</a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<%@ Page Language="C#" Async="true" AutoEventWireup="true" CodeBehind="two_factor_authentication.aspx.cs" Inherits="hephaestus_web.two_factor_authentication" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

    <head runat="server">
        <meta charset="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
        <title>Hephaestus - 2FA</title>
        <link rel="stylesheet" href="../../assets/vendors/mdi/css/materialdesignicons.min.css" />
        <link rel="stylesheet" href="../../assets/vendors/css/vendor.bundle.base.css" />
        <link rel="stylesheet" href="../../assets/css/style.css" />
        <link rel="shortcut icon" />
    </head>

    <body>
        <form id="form1" runat="server">
            <div class="container-scroller">
                <div class="container-fluid page-body-wrapper full-page-wrapper">
                    <div class="row w-100 m-0">
                        <div class="content-wrapper full-page-wrapper d-flex align-items-center auth login-bg">
                            <div class="card col-lg-4 mx-auto">
                                <div class="card-body px-5 py-5">
                                    <h3 class="card-title text-left mb-3">Confirmar Código de Autenticação</h3>
                                    <div>
                                        <div class="form-group">
                                            <label class="d-block text-left">Código de 6 dígitos</label>
                                            <div class="d-flex justify-content-between">
                                                <input name="digit1" type="text" inputmode="numeric" class="form-control p_input text-center digit-input" maxlength="1" style="width: 14%; font-size: 1.5rem; padding: 10px;" />
                                                <input name="digit2" type="text" inputmode="numeric" class="form-control p_input text-center digit-input" maxlength="1" style="width: 14%; font-size: 1.5rem; padding: 10px;" />
                                                <input name="digit3" type="text" inputmode="numeric" class="form-control p_input text-center digit-input" maxlength="1" style="width: 14%; font-size: 1.5rem; padding: 10px;" />
                                                <input name="digit4" type="text" inputmode="numeric" class="form-control p_input text-center digit-input" maxlength="1" style="width: 14%; font-size: 1.5rem; padding: 10px;" />
                                                <input name="digit5" type="text" inputmode="numeric" class="form-control p_input text-center digit-input" maxlength="1" style="width: 14%; font-size: 1.5rem; padding: 10px;" />
                                                <input name="digit6" type="text" inputmode="numeric" class="form-control p_input text-center digit-input" maxlength="1" style="width: 14%; font-size: 1.5rem; padding: 10px;" />
                                            </div>
                                        </div>
                                        <div class="text-center">
                                            <asp:Button ID="btnConfirm" runat="server" CssClass="btn btn-primary btn-block enter-btn" Text="Confirmar" OnClick="btnConfirm_Click" />
                                        </div>
                                        <asp:Label ID="lblError" runat="server" CssClass="text-danger d-block text-center mt-3" />
                                        <p class="sign-up"><asp:LinkButton ID="btnResend" runat="server" OnClick="btnResend_Click">Reenviar código</asp:LinkButton></p>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <script src="../../assets/vendors/js/vendor.bundle.base.js"></script>
            <script src="../../assets/js/off-canvas.js"></script>
            <script src="../../assets/js/hoverable-collapse.js"></script>
            <script src="../../assets/js/misc.js"></script>
            <script src="../../assets/js/settings.js"></script>
            <script src="../../assets/js/todolist.js"></script>
            <script>
                document.addEventListener('DOMContentLoaded', function() {
                    const inputs = document.querySelectorAll('.digit-input');
                    inputs.forEach((input, index) => {
                        input.addEventListener('input', function(e) {
                            this.value = this.value.replace(/[^0-9]/g, '');
                            if (this.value !== '') {
                                if (index < inputs.length - 1) {
                                    inputs[index + 1].focus();
                                }
                            }
                        });
                        input.addEventListener('keydown', function(e) {
                            if (e.key === 'Backspace') {
                                if (this.value === '') {
                                    if (index > 0) {
                                        inputs[index - 1].focus();
                                        inputs[index - 1].value = '';
                                    }
                                }
                            }
                        });
                        input.addEventListener('paste', function(e) {
                            e.preventDefault();
                            const pastedData = (e.clipboardData || window.clipboardData).getData('text');
                            const digits = pastedData.replace(/[^0-9]/g, '').slice(0, 6);
                            for (let i = 0; i < digits.length; i++) {
                                if (i < inputs.length) {
                                    inputs[i].value = digits[i];
                                }
                            }
                            if (digits.length > 0) {
                                const focusIndex = Math.min(digits.length, inputs.length - 1);
                                inputs[focusIndex].focus();
                            }
                        });
                    });
                });
            </script>
        </form>
    </body>

</html>

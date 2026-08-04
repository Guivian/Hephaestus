<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="hephaestus_web.login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <meta charset="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
        <title>Hephaestus - Entrar</title>
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
                                    <h3 class="card-title text-left mb-3">Iniciar Sessão</h3>
                                    <div>
                                        <div class="form-group">
                                            <label>Email</label>
                                            <input type="text" class="form-control p_input" />
                                        </div>
                                        <div class="form-group">
                                            <label>Palavra-Passe</label>
                                            <input type="password" class="form-control p_input" />
                                        </div>
                                        <div class="form-group d-flex align-items-center justify-content-between">
                                            <div class="form-check">
                                                <label class="form-check-label">
                                                    <input type="checkbox" class="form-check-input" /> Manter email
                                                </label>
                                            </div>
                                            <a href="#" class="forgot-pass">Esqueceu a palavra-passe?</a>
                                        </div>
                                        <div class="text-center">
                                            <button type="submit" class="btn btn-primary btn-block enter-btn">Entrar</button>
                                        </div>
                                        <div class="d-flex">
                                            <button type="button" class="gsi-material-button w-100 btn-block" style="max-width: 100%;">
                                                <div class="gsi-material-button-state"></div>
                                                <div class="gsi-material-button-content-wrapper justify-content-center">
                                                    <div class="gsi-material-button-icon">
                                                        <svg version="1.1" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 48 48" xmlns:xlink="http://www.w3.org/1999/xlink" style="display: block;">
                                                            <path fill="#EA4335" d="M24 9.5c3.54 0 6.71 1.22 9.21 3.6l6.85-6.85C35.9 2.38 30.47 0 24 0 14.62 0 6.51 5.38 2.56 13.22l7.98 6.19C12.43 13.72 17.74 9.5 24 9.5z"></path>
                                                            <path fill="#4285F4" d="M46.98 24.55c0-1.57-.15-3.09-.38-4.55H24v9.02h12.94c-.58 2.96-2.26 5.48-4.78 7.18l7.73 6c4.51-4.18 7.09-10.36 7.09-17.65z"></path>
                                                            <path fill="#FBBC05" d="M10.53 28.59c-.48-1.45-.76-2.99-.76-4.59s.27-3.14.76-4.59l-7.98-6.19C.92 16.46 0 20.12 0 24c0 3.88.92 7.54 2.56 10.78l7.97-6.19z"></path>
                                                            <path fill="#34A853" d="M24 48c6.48 0 11.93-2.13 15.89-5.81l-7.73-6c-2.15 1.45-4.92 2.3-8.16 2.3-6.26 0-11.57-4.22-13.47-9.91l-7.98 6.19C6.51 42.62 14.62 48 24 48z"></path>
                                                            <path fill="none" d="M0 0h48v48H0z"></path>
                                                        </svg>
                                                    </div>
                                                    <span class="gsi-material-button-contents" style="flex-grow: 0;">Entrar com Google</span>
                                                    <span style="display: none;">Entrar com Google</span>
                                                </div>
                                            </button>
                                        </div>
                                        <p class="sign-up">Não tem conta? <a href="register.aspx">Registo</a></p>
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
        </form>
    </body>
</html>

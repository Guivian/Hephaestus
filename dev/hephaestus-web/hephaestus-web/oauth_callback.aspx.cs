using System;

namespace hephaestus_web
{
    public partial class oauth_callback : System.Web.UI.Page
    {
        protected async void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            var code = Request.QueryString["code"];
            if (string.IsNullOrWhiteSpace(code))
            {
                ShowError("O Google não devolveu um código de autenticação válido.");
                return;
            }

            var result = await ApiClient.ExchangeGoogleCodeAsync(code);
            if (!result.IsSuccess || result.Data == null)
            {
                ShowError(result.Error);
                return;
            }

            WebSession.Store(result.Data);
            Response.Redirect("dashboard.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void ShowError(string message)
        {
            lblStatus.CssClass = "text-danger";
            lblStatus.Text = Server.HtmlEncode(message);
            lnkLogin.Visible = true;
        }
    }
}

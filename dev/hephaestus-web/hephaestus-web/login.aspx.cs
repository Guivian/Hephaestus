using System;
using System.Net;

namespace hephaestus_web
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && WebSession.IsAuthenticated)
                Response.Redirect("dashboard.aspx");
        }

        protected async void btnLogin_Click(object sender, EventArgs e)
        {
            lblError.Text = string.Empty;
            var email = txtEmail.Text.Trim();
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblError.Text = "Introduza o e-mail e a palavra-passe.";
                return;
            }

            var result = await ApiClient.StartLoginAsync(email, txtPassword.Text);
            if (!result.IsSuccess || result.Data == null)
            {
                lblError.Text = result.Error;
                return;
            }

            if (result.StatusCode == HttpStatusCode.Accepted && result.Data.RequiresTwoFactor)
            {
                Session["TwoFactorChallengeId"] = result.Data.ChallengeId;
                RedirectWithoutAborting("two_factor_authentication.aspx");
                return;
            }

            WebSession.Store(result.Data);
            RedirectWithoutAborting("dashboard.aspx");
        }

        private void RedirectWithoutAborting(string url)
        {
            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}

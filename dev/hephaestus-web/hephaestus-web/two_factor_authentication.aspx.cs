using System;

namespace hephaestus_web
{
    public partial class two_factor_authentication : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && !(Session["TwoFactorChallengeId"] is Guid))
                Response.Redirect("login.aspx");
        }

        protected async void btnConfirm_Click(object sender, EventArgs e)
        {
            if (!(Session["TwoFactorChallengeId"] is Guid challengeId))
            {
                Response.Redirect("login.aspx");
                return;
            }

            var code = string.Concat(Request.Form["digit1"], Request.Form["digit2"],
                Request.Form["digit3"], Request.Form["digit4"], Request.Form["digit5"], Request.Form["digit6"]);
            if (code.Length != 6 || !int.TryParse(code, out _))
            {
                lblError.Text = "Introduza os seis números do código.";
                return;
            }

            var result = await ApiClient.VerifyTwoFactorAsync(challengeId, code);
            if (!result.IsSuccess || result.Data == null)
            {
                lblError.Text = result.Error;
                return;
            }

            Session.Remove("TwoFactorChallengeId");
            WebSession.Store(result.Data);
            Response.Redirect("dashboard.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected async void btnResend_Click(object sender, EventArgs e)
        {
            if (!(Session["TwoFactorChallengeId"] is Guid challengeId))
            {
                Response.Redirect("login.aspx");
                return;
            }

            var result = await ApiClient.ResendTwoFactorAsync(challengeId);
            if (!result.IsSuccess || result.Data == null)
            {
                lblError.Text = result.Error;
                return;
            }

            Session["TwoFactorChallengeId"] = result.Data.ChallengeId;
            lblError.CssClass = "text-success d-block text-center mt-3";
            lblError.Text = "Foi enviado um novo código.";
        }
    }
}

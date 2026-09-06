using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace hephaestus_web
{
    public partial class hephaestus : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!WebSession.IsAuthenticated || !WebSession.EnsureFreshAccessToken())
            {
                WebSession.Clear();
                Response.Redirect("login.aspx");
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            var refreshToken = WebSession.RefreshToken;
            if (!string.IsNullOrWhiteSpace(refreshToken))
                ApiClient.LogoutAsync(refreshToken).GetAwaiter().GetResult();

            WebSession.Clear();
            Response.Redirect("login.aspx");
        }
    }
}

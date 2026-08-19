using System;
namespace hephaestus_web { public partial class error_404 : System.Web.UI.Page { protected void Page_Load(object sender, EventArgs e) { Response.StatusCode = 404; Response.TrySkipIisCustomErrors = true; } } }

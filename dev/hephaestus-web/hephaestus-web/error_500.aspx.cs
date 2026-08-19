using System;
namespace hephaestus_web { public partial class error_500 : System.Web.UI.Page { protected void Page_Load(object sender, EventArgs e) { Response.StatusCode = 500; Response.TrySkipIisCustomErrors = true; } } }

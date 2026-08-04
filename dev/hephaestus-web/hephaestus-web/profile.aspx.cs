using System;

namespace hephaestus_web
{
    public partial class profile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            lblPrimaryTitle.Visible = false;
            lblSecondaryTitle.Visible = true;
            btnEditar.Visible = false;
            btnGuardar.Visible = true;
            btnCancelar.Visible = true;
            
            txtNome.CssClass = "form-control";
            txtEmail.CssClass = "form-control";
            txtLocalidade.CssClass = "form-control";
            
            wrapper2FA.Style.Add("pointer-events", "auto");
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            lblPrimaryTitle.Visible = true;
            lblSecondaryTitle.Visible = false;
            btnEditar.Visible = true;
            btnGuardar.Visible = false;
            btnCancelar.Visible = false;
            
            txtNome.CssClass = "form-control readonly-field";
            txtEmail.CssClass = "form-control readonly-field";
            txtLocalidade.CssClass = "form-control readonly-field";
            
            wrapper2FA.Style.Add("pointer-events", "none");
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            lblPrimaryTitle.Visible = true;
            lblSecondaryTitle.Visible = false;
            btnEditar.Visible = true;
            btnGuardar.Visible = false;
            btnCancelar.Visible = false;
            
            txtNome.CssClass = "form-control readonly-field";
            txtEmail.CssClass = "form-control readonly-field";
            txtLocalidade.CssClass = "form-control readonly-field";
            
            wrapper2FA.Style.Add("pointer-events", "none");
            
            // A lógica para repor os valores originais da Base de Dados vem daqui
        }

        protected void btnConfirmDeactivate_Click(object sender, EventArgs e)
        {
            if (txtConfirmDeactivate.Text == "D3S4T1V4R C0NT4")
            {
                // Desativa a conta na UI (e futuramente na BD)
                chkAtiva.Checked = false;
                
                // Limpa o campo de confirmação para não manter o texto guardado
                txtConfirmDeactivate.Text = string.Empty;
            }
        }
    }
}
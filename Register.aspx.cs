using System;
using System.Web;
using OrbitechWeb.Services;

namespace OrbitechWeb
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void RegisterBtn_Click(object sender, EventArgs e)
        {
            string username = UsernameInput.Text.Trim();
            string email = EmailInput.Text.Trim();
            string password = PasswordInput.Text.Trim();
            string confirmPassword = ConfirmPasswordInput.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                ErrorPanel.Visible = true;
                ErrorLiteral.Text = "All fields are required.";
                return;
            }

            if (password != confirmPassword)
            {
                ErrorPanel.Visible = true;
                ErrorLiteral.Text = "Passwords do not match.";
                return;
            }

            if (password.Length < 6)
            {
                ErrorPanel.Visible = true;
                ErrorLiteral.Text = "Password must be at least 6 characters.";
                return;
            }

            bool regSuccess = false;
            string errorMsg = "";

            try
            {
                var service = new OrbitechService();
                regSuccess = service.RegisterUser(username, password, email);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }

            if (regSuccess)
            {
                Response.Redirect("~/Login.aspx?registered=true", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            else
            {
                ErrorPanel.Visible = true;
                if (!string.IsNullOrEmpty(errorMsg))
                    ErrorLiteral.Text = "An error occurred: " + errorMsg;
                else
                    ErrorLiteral.Text = "Username already exists. Please choose a different username.";
            }
        }
    }
}

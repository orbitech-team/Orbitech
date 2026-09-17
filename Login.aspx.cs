using System;
using System.Web;
using OrbitechWeb.Services;

namespace OrbitechWeb
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Username"] != null)
            {
                string username = Session["Username"].ToString();
                ErrorPanel.Visible = false;
                SuccessPanel.Visible = true;
                var msg = new System.Web.UI.WebControls.Literal();
                msg.Text = "You are already logged in as <strong>" + username + "</strong>. " +
                           "<a href='" + ResolveUrl("~/Logout.aspx") + "' style='color:var(--primary);font-weight:600'>Click here to logout</a> " +
                           "and sign in with a different account.";
                SuccessPanel.Controls.Clear();
                SuccessPanel.Controls.Add(msg);
            }
        }

        protected void LoginBtn_Click(object sender, EventArgs e)
        {
            string username = UsernameInput.Text.Trim();
            string password = PasswordInput.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ErrorPanel.Visible = true;
                ErrorLiteral.Text = "Please enter both username and password.";
                return;
            }

            bool loginSuccess = false;
            string errorMsg = "";

            try
            {
                var service = new OrbitechService();
                loginSuccess = service.ValidateUser(username, password);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }

            if (loginSuccess)
            {
                Session.Clear();
                Session["Username"] = username;
                bool isAdmin = username.StartsWith("admin", StringComparison.OrdinalIgnoreCase);
                Session["IsAdmin"] = isAdmin;

                if (isAdmin)
                {
                    Response.Redirect("~/AdminProducts.aspx", false);
                }
                else
                {
                    Response.Redirect("~/Default.aspx", false);
                }
                Context.ApplicationInstance.CompleteRequest();
            }
            else
            {
                ErrorPanel.Visible = true;
                if (!string.IsNullOrEmpty(errorMsg))
                    ErrorLiteral.Text = "An error occurred while logging in: " + errorMsg;
                else
                    ErrorLiteral.Text = "Invalid username or password. Please try again.";
            }
        }
    }
}

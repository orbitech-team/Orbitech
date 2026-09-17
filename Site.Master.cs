using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OrbitechWeb
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                UpdateAccountDisplay();
            }
            catch
            {
            }
        }

        private void UpdateAccountDisplay()
        {
            // Build the admin link HTML once
            string adminLinkHtml = "<a href='" + ResolveUrl("~/AdminProducts.aspx") + "' style='color:var(--primary);font-weight:700'>Admin Panel</a>";
            string adminHeaderHtml = "<a href='" + ResolveUrl("~/AdminProducts.aspx") + "' style='font-size:var(--text-sm);font-weight:600;color:var(--primary);margin-right:12px'>Admin Panel</a>";
            string adminDrawerHtml = "<a href='" + ResolveUrl("~/AdminProducts.aspx") + "' style='color:var(--primary);font-weight:600'>Admin Panel</a>";

            if (Session["Username"] != null)
            {
                string username = Session["Username"].ToString();
                bool isAdmin = (Session["IsAdmin"] != null && (bool)Session["IsAdmin"]);

                // Account panel — username + logout (this already works)
                if (AccountPanel != null)
                {
                    AccountPanel.Controls.Clear();

                    var greeting = new Literal();
                    greeting.Text = "<a href='" + ResolveUrl("~/Login.aspx") + "' style='text-decoration:none;color:inherit;display:inline-flex;align-items:center;gap:6px'>" +
                        "<svg width='20' height='20' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round'><circle cx='12' cy='8' r='4'/><path d='M4 21c0-4 4-7 8-7s8 3 8 7'/></svg>" +
                        "<span style='font-size:var(--text-sm);font-weight:600'>" + username + "</span></a>";

                    var logout = new Literal();
                    logout.Text = "<a href='" + ResolveUrl("~/Logout.aspx") + "' style='font-size:var(--text-sm);font-weight:600;color:var(--primary);margin-left:12px'>Logout</a>";

                    AccountPanel.Controls.Add(greeting);
                    AccountPanel.Controls.Add(logout);
                }

                // Admin links — inject HTML directly, no Visible property needed
                if (AdminNavLiteral != null)
                    AdminNavLiteral.Text = isAdmin ? adminLinkHtml : "";
                if (AdminHeaderLiteral != null)
                    AdminHeaderLiteral.Text = isAdmin ? adminHeaderHtml : "";
                if (AdminDrawerLiteral1 != null)
                    AdminDrawerLiteral1.Text = isAdmin ? adminDrawerHtml : "";
            }
            else
            {
                // Not logged in — empty all admin literals
                if (AdminNavLiteral != null)
                    AdminNavLiteral.Text = "";
                if (AdminHeaderLiteral != null)
                    AdminHeaderLiteral.Text = "";
                if (AdminDrawerLiteral1 != null)
                    AdminDrawerLiteral1.Text = "";
            }
        }
    }
}

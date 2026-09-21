using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OrbitechWeb.Models;
using OrbitechWeb.Services;

namespace OrbitechWeb
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // No try/catch on the account display on purpose: if something
            // breaks, you WANT the yellow screen telling you where.
            UpdateAccountDisplay();
            UpdateCartBadge();
        }

        // Safe admin check: works whether Login.aspx.cs stored a native
        // bool or the string "True" in Session["IsAdmin"].
        private bool IsAdminUser()
        {
            object role = Session["IsAdmin"];
            if (role == null) return false;

            try
            {
                return Convert.ToBoolean(role);
            }
            catch
            {
                return role.ToString().Trim()
                    .Equals("true", StringComparison.OrdinalIgnoreCase);
            }
        }

        private void UpdateAccountDisplay()
        {
            // Admin links: top nav + mobile drawer only. The HEADER admin
            // link was removed (crowded the header, out of place) —
            // AdminHeaderLiteral is now always empty.
            string adminLinkHtml = "<a href='" + ResolveUrl("~/AdminProducts.aspx") + "' style='color:var(--primary);font-weight:700'>Admin Panel</a>";
            string adminDrawerHtml = "<a href='" + ResolveUrl("~/AdminProducts.aspx") + "' style='color:var(--primary);font-weight:600'>Admin Panel</a>";

            // ---------- LOGGED IN ----------
            if (Session["Username"] != null)
            {
                string username = Session["Username"].ToString();
                bool isAdmin = IsAdminUser();

                if (AccountLiteral != null)
                {
                    string greetingHtml = "<a href='" + ResolveUrl("~/Profile.aspx") + "' class='account-trigger'>" +
                        "<svg width='20' height='20' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round'><circle cx='12' cy='8' r='4'/><path d='M4 21c0-4 4-7 8-7s8 3 8 7'/></svg>" +
                        "<span>" + username + "</span></a>";

                    string sepHtml = "<span class='account-sep'></span>";
                    string profileHtml = "<a href='" + ResolveUrl("~/Profile.aspx") + "' class='account-link'>My Profile</a>";
                    string logoutHtml = "<a href='" + ResolveUrl("~/Logout.aspx") + "' class='account-link'>Logout</a>";

                    AccountLiteral.Text = greetingHtml + sepHtml + profileHtml + sepHtml + logoutHtml;
                }

                if (AdminNavLiteral != null)
                    AdminNavLiteral.Text = isAdmin ? adminLinkHtml : "";
                if (AdminHeaderLiteral != null)
                    AdminHeaderLiteral.Text = ""; // removed from header by design
                if (AdminDrawerLiteral1 != null)
                    AdminDrawerLiteral1.Text = isAdmin ? adminDrawerHtml : "";
            }
            // ---------- LOGGED OUT ----------
            else
            {
                if (AccountLiteral != null)
                {
                    AccountLiteral.Text = "<a href='" + ResolveUrl("~/Login.aspx") + "' class='account-trigger' aria-label='Account' title='Login / Register'>" +
                        "<svg width='20' height='20' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round'><circle cx='12' cy='8' r='4'/><path d='M4 21c0-4 4-7 8-7s8 3 8 7'/></svg></a>";
                }

                if (AdminNavLiteral != null)
                    AdminNavLiteral.Text = "";
                if (AdminHeaderLiteral != null)
                    AdminHeaderLiteral.Text = "";
                if (AdminDrawerLiteral1 != null)
                    AdminDrawerLiteral1.Text = "";
            }
        }

        // Live cart count badge on the cart icon. Sums item QUANTITIES
        // (3 of one product = 3). Hidden entirely when the cart is
        // empty — no floating "0" bubble.
        private void UpdateCartBadge()
        {
            if (CartCountBadge == null) return;

            try
            {
                if (Session["Username"] != null)
                {
                    OrbitechService service = new OrbitechService();
                    List<CartItem> items = service.GetCartItems(Session["Username"].ToString());
                    int count = items.Sum(i => i.Quantity);

                    CartCountBadge.InnerText = count.ToString();
                    CartCountBadge.Visible = count > 0;
                }
                else
                {
                    CartCountBadge.Visible = false;
                }
            }
            catch
            {
                // The badge is cosmetic — it must never break a page.
                CartCountBadge.Visible = false;
            }
        }
    }
}

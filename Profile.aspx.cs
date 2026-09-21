using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using OrbitechWeb.Models;
using OrbitechWeb.Services;

namespace OrbitechWeb
{
    public partial class Profile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Gate 1: auth before anything touches data
            if (Session["Username"] == null)
            {
                Response.Redirect("~/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
            {
                LoadAccountCard();

                // One page, two states: the session role decides the view
                bool isAdmin = Session["IsAdmin"] != null && Convert.ToBoolean(Session["IsAdmin"]);
                if (isAdmin)
                {
                    LoadAdminView();
                }
                else
                {
                    LoadCustomerView();
                }
            }
        }

        // Common to both views: avatar initial, name, role badge, member since
        private void LoadAccountCard()
        {
            string username = Session["Username"].ToString();

            UsernameLiteral.Text = username;
            AvatarLiteral.Text = username.Substring(0, 1).ToUpper();
            RoleLiteral.Text = "<span class='role-badge'>Customer</span>";
            if (Session["IsAdmin"] != null && Convert.ToBoolean(Session["IsAdmin"]))
            {
                RoleLiteral.Text = "<span class='role-badge role-badge--admin'>Admin</span>";
            }

            OrbitechService service = new OrbitechService();
            ProfileStats stats = service.GetProfileStats(username);
            MemberSinceLiteral.Text = "Member since " + stats.MemberSince.ToString("MMMM yyyy");
        }

        private void LoadCustomerView()
        {
            CustomerViewPanel.Visible = true;
            AdminViewPanel.Visible = false;

            string username = Session["Username"].ToString();
            OrbitechService service = new OrbitechService();

            ProfileStats stats = service.GetProfileStats(username);
            CustOrdersLiteral.Text = stats.TotalOrders.ToString();
            CustSpentLiteral.Text = stats.TotalSpent.ToString("N2");
            CustFavsLiteral.Text = stats.FavouriteCount.ToString();

            // Reuses the exact method the invoice work already built
            List<Order> orders = service.GetOrdersForUser(username);

            if (orders.Count == 0)
            {
                CustNoOrdersPanel.Visible = true;
            }
            else
            {
                // Show the 5 most recent; the query is already date-DESC
                int show = Math.Min(5, orders.Count);
                List<Order> recent = orders.GetRange(0, show);
                CustOrdersRepeater.DataSource = recent;
                CustOrdersRepeater.DataBind();
            }
        }

        private void LoadAdminView()
        {
            AdminViewPanel.Visible = true;
            CustomerViewPanel.Visible = false;

            OrbitechService service = new OrbitechService();

            AdminDashboardStats stats = service.GetAdminDashboardStats();
            AdminProductsLiteral.Text = stats.TotalProducts.ToString();
            AdminOrdersLiteral.Text = stats.TotalOrders.ToString();
            AdminUsersLiteral.Text = stats.TotalUsers.ToString();
            AdminRevenueLiteral.Text = stats.TotalRevenue.ToString("N2");

            AdminOrdersRepeater.DataSource = service.GetRecentOrders(5);
            AdminOrdersRepeater.DataBind();

            List<Product> lowStock = service.GetLowStockProducts();
            if (lowStock.Count == 0)
            {
                NoLowStockPanel.Visible = true;
            }
            else
            {
                LowStockRepeater.DataSource = lowStock;
                LowStockRepeater.DataBind();
            }
        }
    }
}

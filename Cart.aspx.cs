using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OrbitechWeb.Models;
using OrbitechWeb.Services;

namespace OrbitechWeb
{
    public partial class Cart : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if this is an "add to cart" request from Shop.aspx or ProductDetails.aspx
            // URL format: Cart.aspx?action=add&id=5
            string action = Request.QueryString["action"];
            string idParam = Request.QueryString["id"];

            if (action == "add" && !string.IsNullOrEmpty(idParam))
            {
                AddToCartFromUrl(idParam);
                return; // Redirects after adding
            }

            // Must be logged in to view the cart
            if (Session["Username"] == null)
            {
                LoginPromptPanel.Visible = true;
                CartPanel.Visible = false;
                EmptyCartPanel.Visible = false;
                return;
            }

            if (!IsPostBack)
            {
                LoadCart();
            }
        }

        private void AddToCartFromUrl(string idParam)
        {
            int productId;
            if (!int.TryParse(idParam, out productId))
            {
                Response.Redirect("~/Cart.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
        return;
            }

            // Must be logged in to add items
            if (Session["Username"] == null)
            {
                // Redirect to login, then back to shop after
                Response.Redirect("~/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
        return;
            }

            string username = Session["Username"].ToString();
            OrbitechService service = new OrbitechService();
            service.AddToCart(username, productId, 1);

            // Redirect to cart page (without the action params) so it loads the cart
            Response.Redirect("~/Cart.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
}


        private void LoadCart()
        {
            string username = Session["Username"].ToString();

            OrbitechService service = new OrbitechService();
            List<CartItem> items = service.GetCartItems(username);

            if (items.Count == 0)
            {
                EmptyCartPanel.Visible = true;
                CartPanel.Visible = false;
                LoginPromptPanel.Visible = false;
                return;
            }

            // Show cart, hide empty/login panels
            CartPanel.Visible = true;
            EmptyCartPanel.Visible = false;
            LoginPromptPanel.Visible = false;

            // Bind items to Repeater
            CartRepeater.DataSource = items;
            CartRepeater.DataBind();

            // Calculate totals
            decimal subtotal = 0;
            int totalItems = 0;
            foreach (var item in items)
            {
                subtotal += item.LineTotal;
                totalItems += item.Quantity;
            }

            decimal vat = subtotal * 0.15m;
            decimal total = subtotal + vat;

            ItemCountLiteral.Text = totalItems.ToString();
            SubtotalLiteral.Text = subtotal.ToString("N2");
            VatLiteral.Text = vat.ToString("N2");
            TotalLiteral.Text = total.ToString("N2");
        }

        protected void CartRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string username = Session["Username"].ToString();
            OrbitechService service = new OrbitechService();

            if (e.CommandName == "Remove")
            {
                int cartItemId = Convert.ToInt32(e.CommandArgument);
                service.RemoveFromCart(cartItemId);
            }
            else if (e.CommandName == "Increase")
            {
                // CommandArgument format: "cartItemId|currentQuantity"
                string[] args = e.CommandArgument.ToString().Split('|');
                int cartItemId = Convert.ToInt32(args[0]);
                int currentQty = Convert.ToInt32(args[1]);
                service.UpdateCartQuantity(cartItemId, currentQty + 1);
            }
            else if (e.CommandName == "Decrease")
            {
                string[] args = e.CommandArgument.ToString().Split('|');
                int cartItemId = Convert.ToInt32(args[0]);
                int currentQty = Convert.ToInt32(args[1]);
                service.UpdateCartQuantity(cartItemId, currentQty - 1);
            }

            // Reload cart after any change
            LoadCart();
        }
    }
}

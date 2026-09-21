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
            // Handle "add" links from Shop.aspx (Cart.aspx?action=add&id=5)
            string action = Request.QueryString["action"];
            string idParam = Request.QueryString["id"];

            if (action == "add" && !string.IsNullOrEmpty(idParam))
            {
                AddToCartFromUrl(idParam);
                return;
            }

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

            if (Session["Username"] == null)
            {
                Response.Redirect("~/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            OrbitechService service = new OrbitechService();
            service.AddToCart(Session["Username"].ToString(), productId, 1);

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

            CartPanel.Visible = true;
            EmptyCartPanel.Visible = false;
            LoginPromptPanel.Visible = false;

            CartRepeater.DataSource = items;
            CartRepeater.DataBind();

            // PART A: totals now come from the SAME method the checkout
            // uses, so both pages always show identical numbers.
            string promo = Session["PromoCode"] as string;
            OrderTotals totals = service.CalculateOrderTotals(items, promo);

            int totalItems = 0;
            foreach (CartItem item in items)
            {
                totalItems += item.Quantity;
            }

            ItemCountLiteral.Text = totalItems.ToString();
            SubtotalLiteral.Text = totals.Subtotal.ToString("N2");
            ShippingLiteral.Text = totals.Shipping == 0 ? "FREE" : "R" + totals.Shipping.ToString("N2");
            VatLiteral.Text = totals.Tax.ToString("N2");
            TotalLiteral.Text = totals.Total.ToString("N2");

   


            // Show the discount line only when a valid code is applied
            if (totals.PromoApplied)
            {
                DiscountLinePanel.Visible = true;
                DiscountLiteral.Text = totals.Discount.ToString("N2");
            }
            else
            {
                DiscountLinePanel.Visible = false;
            }
        }

        // PART A: Apply button. Stores the code in Session so it
        // survives the redirect to Checkout.aspx.
        // PART A: Apply button. Stores the code in Session so it
        // survives the redirect to Checkout.aspx.
        protected void ApplyPromoBtn_Click(object sender, EventArgs e)
        {
            string code = PromoInput.Text.Trim();

            OrbitechService service = new OrbitechService();
            List<CartItem> items = service.GetCartItems(Session["Username"].ToString());
            OrderTotals totals = service.CalculateOrderTotals(items, code);

            if (totals.PromoApplied)
            {
                Session["PromoCode"] = code.ToUpper();
            }
            else
            {
                Session["PromoCode"] = null;
            }

            // Pre-fill the box with the currently applied code
            PromoInput.Text = Session["PromoCode"] as string ?? "";

            // RELOAD FIRST — LoadCart no longer touches the message panel,
            // so it can't wipe anything we set below.
            LoadCart();

            // THEN set the message. Nothing runs after this during the
            // request, so this is what the user sees when the page renders.
            PromoMessagePanel.Visible = true;
            if (totals.PromoApplied)
            {
                PromoMessageLiteral.Text = "<span style='color:var(--emerald);font-size:var(--text-sm);font-weight:600'>Promo code " + totals.PromoCode + " applied!</span>";
            }
            else if (string.IsNullOrEmpty(code))
            {
                PromoMessageLiteral.Text = "<span style='color:#dc2626;font-size:var(--text-sm)'>Enter a promo code first.</span>";
            }
            else
            {
                PromoMessageLiteral.Text = "<span style='color:#dc2626;font-size:var(--text-sm)'>Invalid promo code.</span>";
            }
        }


        // PART A: Sends the user to Checkout.aspx with the cart intact.
        protected void CheckoutBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Checkout.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
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
                string[] args = e.CommandArgument.ToString().Split('|');
                service.UpdateCartQuantity(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]) + 1);
            }
            else if (e.CommandName == "Decrease")
            {
                string[] args = e.CommandArgument.ToString().Split('|');
                service.UpdateCartQuantity(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]) - 1);
            }

            LoadCart();
        }
    }
}

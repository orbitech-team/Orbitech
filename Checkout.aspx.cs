using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OrbitechWeb.Models;
using OrbitechWeb.Services;

namespace OrbitechWeb
{
    public partial class Checkout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Username"] == null)
            {
                Response.Redirect("~/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            // PART A: If we just placed an order, the order id is in
            // the query string (Checkout.aspx?placed=12). Show success.
            if (!IsPostBack)
            {
                string placedId = Request.QueryString["placed"];
                if (!string.IsNullOrEmpty(placedId))
                {
                    ShowSuccess(placedId);
                    return;
                }

                LoadCheckout();
            }
        }

        private void LoadCheckout()
        {
            string username = Session["Username"].ToString();
            OrbitechService service = new OrbitechService();

            List<CartItem> items = service.GetCartItems(username);

            if (items.Count == 0)
            {
                // Nothing to check out with. Send back to the cart.
                Response.Redirect("~/Cart.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            CheckoutPanel.Visible = true;
            SuccessPanel.Visible = false;

            CheckoutRepeater.DataSource = items;
            CheckoutRepeater.DataBind();

            // Same calculation method as the cart page. The promo the
            // user applied on the cart carries over via Session.
            string promo = Session["PromoCode"] as string;
            OrderTotals totals = service.CalculateOrderTotals(items,promo);

            SubtotalLiteral.Text = totals.Subtotal.ToString("N2");
            ShippingLiteral.Text = totals.Shipping == 0 ? "FREE" : "R" + totals.Shipping.ToString("N2");
            VatLiteral.Text = totals.Tax.ToString("N2");
            TotalLiteral.Text = totals.Total.ToString("N2");

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

        // PART A: The button that finalizes everything. Calls PlaceOrder,
        // which runs inside a database transaction on the service side.
        protected void PlaceOrderBtn_Click(object sender, EventArgs e)
        {
            string username = Session["Username"].ToString();
            string promo = Session["PromoCode"] as string;

            OrbitechService service = new OrbitechService();

            // Guard: cart may have been emptied since the page loaded
            List<CartItem> items = service.GetCartItems(username);
            if (items.Count == 0)
            {
                Response.Redirect("~/Cart.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            int orderId = service.PlaceOrder(username, promo);

            if (orderId > 0)
            {
                // Order succeeded. The promo code is now used up.
                Session["PromoCode"] = null;

                // Redirect to the success view (keeps the URL shareable
                // and prevents double-placing if the user refreshes).
                Response.Redirect("~/Checkout.aspx?placed=" + orderId, false);
                Context.ApplicationInstance.CompleteRequest();
            }
            else
            {
                ErrorPanel.Visible = true;
                ErrorLiteral.Text = "Something went wrong placing your order. Your cart was not changed, please try again.";
            }
        }

        // PART A: Success view. Pulls the saved order (with its invoice
        // number) back from the database. Member B will build the full
        // InvoiceView page using the same GetOrderById method.
        private void ShowSuccess(string placedId)
        {
            int orderId;
            if (!int.TryParse(placedId, out orderId))
            {
                Response.Redirect("~/Cart.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            OrbitechService service = new OrbitechService();
            Order order = service.GetOrderById(orderId);

            if (order == null)
            {
                Response.Redirect("~/Cart.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            // SECURITY: only show this order to its owner. A user must
            // never see another user's order by editing the URL.
            if (order.Username != Session["Username"].ToString())
            {
                Response.Redirect("~/Cart.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            CheckoutPanel.Visible = false;
            SuccessPanel.Visible = true;

            SuccessLiteral.Text =
                "<p style='color:var(--fg-mute); margin-bottom:var(--s4);'>Thank you for your purchase. Your invoice has been generated.</p>" +
                "<div style='display:grid; gap:var(--s2); text-align:left; background:var(--bg); padding:var(--s5); border-radius:var(--r); font-size:var(--text-sm);'>" +
                "<span><strong>Invoice number:</strong> " + order.InvoiceNumber + "</span>" +
                "<span><strong>Order total:</strong> R" + order.Total.ToString("N2") + "</span>" +
                "<span><strong>Status:</strong> " + order.Status + "</span>" +
                "<span><strong>Date:</strong> " + order.OrderDate.ToString("dd MMM yyyy, HH:mm") + "</span>" +
                "</div>";
        }
    }
}

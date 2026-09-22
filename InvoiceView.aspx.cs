using System;
using System.Collections.Generic;
using OrbitechWeb.Models;
using OrbitechWeb.Services;

namespace OrbitechWeb
{
    public partial class InvoiceView : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadInvoice();
            }
        }

        private void LoadInvoice()
        {
            // A user must be logged in before any invoice data is requested.
            if (Session["Username"] == null)
            {
                string returnUrl = Server.UrlEncode(Request.RawUrl);
                Response.Redirect("~/Login.aspx?returnUrl=" + returnUrl, false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            int orderId;
            if (!int.TryParse(Request.QueryString["id"], out orderId) || orderId <= 0)
            {
                ShowNotFound("No valid order was specified.");
                return;
            }

            try
            {
                // Program to the service interface, like the other service-driven pages.
                IOrbitechService service = new OrbitechService();
                Order order = service.GetOrderById(orderId);

                if (order == null)
                {
                    ShowNotFound("We couldn't find that invoice.");
                    return;
                }

                string currentUser = Convert.ToString(Session["Username"]);
                bool isAdmin = GetIsAdmin();
                bool isOwner = string.Equals(
                    order.Username,
                    currentUser,
                    StringComparison.OrdinalIgnoreCase);

                // A customer may only view their own invoice. Admins may view any invoice.
                if (!isOwner && !isAdmin)
                {
                    ShowNotFound("That invoice doesn't belong to your account.");
                    return;
                }

                BindInvoice(order);
            }
            catch (Exception ex)
            {
                // Keep the technical SQL/service message out of the browser, but preserve
                // it in Visual Studio's Output window for debugging.
                System.Diagnostics.Trace.TraceError("InvoiceView failed for order {0}: {1}", orderId, ex);
                ShowNotFound("We couldn't load this invoice right now. Please try again.");
            }
        }

        private bool GetIsAdmin()
        {
            object value = Session["IsAdmin"];
            if (value == null)
            {
                return false;
            }

            bool result;
            return bool.TryParse(value.ToString(), out result) && result;
        }

        private void BindInvoice(Order order)
        {
            NotFoundPanel.Visible = false;
            InvoiceCardPanel.Visible = true;

            InvoiceNumberLiteral.Text = Server.HtmlEncode(order.InvoiceNumber ?? string.Empty);
            InvoiceDateLiteral.Text = order.OrderDate.ToString("dd MMM yyyy");
            StatusLiteral.Text = Server.HtmlEncode(order.Status ?? string.Empty);
            CustomerNameLiteral.Text = Server.HtmlEncode(order.Username ?? string.Empty);

            InvoiceLinesRepeater.DataSource = order.Items ?? new List<OrderItem>();
            InvoiceLinesRepeater.DataBind();

            SubtotalLiteral.Text = order.Subtotal.ToString("N2");

            DiscountRowPanel.Visible = order.Discount > 0;
            DiscountLiteral.Text = order.Discount.ToString("N2");

            ShippingLiteral.Text = order.Shipping > 0
                ? "R" + order.Shipping.ToString("N2")
                : "Free";

            VatLiteral.Text = order.Tax.ToString("N2");
            TotalLiteral.Text = order.Total.ToString("N2");
        }

        private void ShowNotFound(string message)
        {
            InvoiceCardPanel.Visible = false;
            NotFoundPanel.Visible = true;
            NotFoundMessage.Text = Server.HtmlEncode(message);
        }
    }
}

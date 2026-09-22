using System;
using OrbitechWeb.Models;
using OrbitechWeb.Services;

namespace OrbitechWeb
{
    public partial class AdminsReport : System.Web.UI.Page
    {
        private readonly IOrbitechService service = new OrbitechService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!EnsureAdmin())
            {
                return;
            }

            if (!IsPostBack)
            {
                LoadSalesReport();
                LoadSatisfactionReport();
                LoadComplaints();
            }
        }

        // Uses the same session convention as Login.aspx, Profile.aspx and AdminProducts.aspx.
        private bool EnsureAdmin()
        {
            bool isAdmin = Session["Username"] != null
                && Session["IsAdmin"] != null
                && Convert.ToBoolean(Session["IsAdmin"]);

            if (!isAdmin)
            {
                Response.Redirect("~/Login.aspx?returnUrl=" + Server.UrlEncode(Request.RawUrl), false);
                Context.ApplicationInstance.CompleteRequest();
                return false;
            }

            return true;
        }

        protected void DdlRange_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSalesReport();
        }

        private void LoadSalesReport()
        {
            try
            {
                int days = Convert.ToInt32(DdlRange.SelectedValue);
                SalesSummary sales = service.GetSalesSummary(days);

                LitRevenue.Text = sales.TotalRevenue.ToString("N2");
                LitOrderCount.Text = sales.OrderCount.ToString();
                LitAvgOrder.Text = sales.AvgOrderValue.ToString("N2");

                MonthlyRepeater.DataSource = sales.MonthlySales;
                MonthlyRepeater.DataBind();
                TopProductsRepeater.DataSource = sales.TopProducts;
                TopProductsRepeater.DataBind();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Sales report error: {0}", ex);
                ShowError("Could not load the sales report.");
            }
        }

        private void LoadSatisfactionReport()
        {
            try
            {
                FeedbackSummary summary = service.GetFeedbackSummary();
                LitSatResponses.Text = summary.TotalResponses.ToString();
                LitSatDelivery.Text = summary.TotalResponses > 0
                    ? summary.AvgDeliveryRating.ToString("0.0") : "—";
                LitSatProduct.Text = summary.TotalResponses > 0
                    ? summary.AvgSatisfactionRating.ToString("0.0") : "—";
                LitSatPercent.Text = summary.TotalResponses > 0
                    ? summary.PercentSatisfied.ToString("0") + "%" : "—";

                DistributionRepeater.DataSource = summary.SatisfactionDistribution;
                DistributionRepeater.DataBind();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Satisfaction report error: {0}", ex);
                ShowError("Could not load the satisfaction report.");
            }
        }

        private void LoadComplaints()
        {
            try
            {
                var complaints = service.GetComplaints();
                ComplaintsRepeater.DataSource = complaints;
                ComplaintsRepeater.DataBind();
                NoComplaintsPanel.Visible = complaints.Count == 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Complaints report error: {0}", ex);
                ShowError("Could not load the complaints report.");
            }
        }

        private void ShowError(string message)
        {
            ErrorPanel.Visible = true;
            ErrorLiteral.Text = Server.HtmlEncode(message);
        }
    }
}

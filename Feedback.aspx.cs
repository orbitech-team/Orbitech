using System;
using OrbitechWeb.Models;
using OrbitechWeb.Services;

namespace OrbitechWeb
{
    public partial class Feedback : System.Web.UI.Page
    {
        private readonly IOrbitechService service = new OrbitechService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadReport();
            }
        }

        private void LoadReport()
        {
            try
            {
                FeedbackSummary summary = service.GetFeedbackSummary();

                LitAvgDeliveryDays.Text = summary.AvgDeliveryDays > 0
                    ? summary.AvgDeliveryDays.ToString("0.0") + " days" : "—";
                LitAvgDeliveryRating.Text = summary.TotalResponses > 0
                    ? summary.AvgDeliveryRating.ToString("0.0") + " / 5" : "—";
                LitAvgSatisfaction.Text = summary.TotalResponses > 0
                    ? summary.AvgSatisfactionRating.ToString("0.0") + " / 5" : "—";
                LitPctSatisfied.Text = summary.TotalResponses > 0
                    ? summary.PercentSatisfied.ToString("0") + "%" : "—";

                RecentFeedbackRepeater.DataSource = service.GetRecentFeedback(5);
                RecentFeedbackRepeater.DataBind();
                NoReviewsPanel.Visible = summary.TotalResponses == 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Feedback report error: {0}", ex);
                ShowStatus(false, "The feedback report could not be loaded.");
            }
        }

        protected void BtnSubmitFeedback_Click(object sender, EventArgs e)
        {
            if (Session["Username"] == null)
            {
                Response.Redirect("~/Login.aspx?returnUrl=" + Server.UrlEncode(Request.RawUrl), false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!Page.IsValid)
            {
                ShowStatus(false, "Please correct the highlighted fields and try again.");
                return;
            }

            int orderId;
            int deliveryRating;
            int satisfactionRating;
            if (!int.TryParse(TxtOrderID.Text.Trim(), out orderId)
                || !int.TryParse(RblDelivery.SelectedValue, out deliveryRating)
                || !int.TryParse(RblSatisfaction.SelectedValue, out satisfactionRating))
            {
                ShowStatus(false, "Please enter a valid order and ratings.");
                return;
            }

            bool isComplaint = ChkComplaint.Checked;
            string category = isComplaint ? DdlComplaintCategory.SelectedValue : null;
            if (isComplaint && string.IsNullOrWhiteSpace(category))
            {
                ShowStatus(false, "Please choose a complaint category when logging a complaint.");
                return;
            }

            try
            {
                // Prevent customers from submitting feedback against another user's order.
                Order order = service.GetOrderById(orderId);
                string username = Convert.ToString(Session["Username"]);
                bool isAdmin = Session["IsAdmin"] != null
                    && Convert.ToBoolean(Session["IsAdmin"]);

                if (order == null)
                {
                    ShowStatus(false, "That order could not be found.");
                    return;
                }

                if (!isAdmin && !string.Equals(order.Username, username,
                    StringComparison.OrdinalIgnoreCase))
                {
                    ShowStatus(false, "That order does not belong to your account.");
                    return;
                }

                string result = service.SubmitFeedback(
                    orderId,
                    deliveryRating,
                    satisfactionRating,
                    TxtComments.Text.Trim(),
                    isComplaint,
                    category);

                bool success = result != null
                    && result.StartsWith("SUCCESS", StringComparison.OrdinalIgnoreCase);
                string message = success && result.StartsWith("SUCCESS: ", StringComparison.OrdinalIgnoreCase)
                    ? result.Substring("SUCCESS: ".Length)
                    : result;

                ShowStatus(success, message ?? "Feedback could not be submitted.");

                if (success)
                {
                    TxtOrderID.Text = string.Empty;
                    TxtComments.Text = string.Empty;
                    ChkComplaint.Checked = false;
                    DdlComplaintCategory.SelectedIndex = 0;
                    RblDelivery.SelectedValue = "5";
                    RblSatisfaction.SelectedValue = "5";
                    LoadReport();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Feedback submission error: {0}", ex);
                ShowStatus(false, "Feedback could not be submitted right now.");
            }
        }

        private void ShowStatus(bool success, string message)
        {
            StatusPanel.Visible = true;
            StatusPanel.CssClass = "alert " + (success ? "alert-success" : "alert-error");
            StatusLiteral.Text = Server.HtmlEncode(message);
        }
    }
}

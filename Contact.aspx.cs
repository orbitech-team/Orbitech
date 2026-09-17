// ============================================================
// Contact.aspx.cs — Code-Behind for Contact.aspx
// ============================================================
// TUTORIAL STEP: This code handles the contact form submission.
// It reads the form fields, calls the WCF service to save the
// message to the CONTACT table, and shows a success/error message.
//
// PLACE THIS FILE IN: OrbitechWeb/Contact.aspx.cs
// ============================================================

using System;
using OrbitechWeb.Services;

namespace OrbitechWeb
{
    public partial class Contact : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // No special initialization needed
        }

        // TUTORIAL STEP: This method runs when the "Send message" button is clicked.
        protected void SubmitBtn_Click(object sender, EventArgs e)
        {
            string name = CName.Text.Trim();
            string email = CEmail.Text.Trim();
            string subject = CSubject.SelectedValue;
            string message = CMessage.Text.Trim();

            // --- VALIDATION ---
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(message))
            {
                ShowStatus("Please fill in your email and message.", false);
                return;
            }

            // --- CALL WCF SERVICE ---
            // Save the contact message to the database
            OrbitechService service = new OrbitechService();
            bool success = service.SaveContactMessage(name, email, subject, message);

            if (success)
            {
                // Clear the form
                CName.Text = "";
                CEmail.Text = "";
                CMessage.Text = "";

                ShowStatus("Message sent successfully! We'll get back to you within 4 business hours.", true);
            }
            else
            {
                ShowStatus("Failed to send message. Please try again or email us directly.", false);
            }
        }

        // Helper: Show a status message (green for success, red for error)
        private void ShowStatus(string message, bool success)
        {
            StatusPanel.Visible = true;
            StatusPanel.CssClass = success ? "alert-success" : "alert-error";
            StatusPanel.Style["padding"] = "var(--s4) var(--s5)";
            StatusPanel.Style["border-radius"] = "var(--r-sm)";
            StatusPanel.Style["margin-bottom"] = "var(--s5)";
            StatusPanel.Style["font-size"] = "var(--text-sm)";

            if (success)
            {
                StatusPanel.Style["background"] = "#f0fdf4";
                StatusPanel.Style["color"] = "#16a34a";
                StatusPanel.Style["border"] = "1px solid #bbf7d0";
            }
            else
            {
                StatusPanel.Style["background"] = "#fef2f2";
                StatusPanel.Style["color"] = "#dc2626";
                StatusPanel.Style["border"] = "1px solid #fecaca";
            }

            StatusLiteral.Text = message;
        }
    }
}

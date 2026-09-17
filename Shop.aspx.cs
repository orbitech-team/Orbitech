// ============================================================
// Shop.aspx.cs — Code-Behind for Shop.aspx
// ============================================================
// TUTORIAL STEP: This code runs on the server when the Shop page loads.
// It:
//   1. Reads the "cat" (category) and "search" query parameters from the URL
//   2. Calls the WCF service to get products from the database
//   3. Binds those products to the Repeater control on the page
//
// PLACE THIS FILE IN: OrbitechWeb/Shop.aspx.cs
// ============================================================

using System;
using System.Collections.Generic;
using OrbitechWeb.Models;
using OrbitechWeb.Services;

namespace OrbitechWeb
{
    public partial class Shop : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // TUTORIAL STEP: !IsPostBack means this code only runs on the first page load,
            // not when a button is clicked (postback). This prevents reloading data unnecessarily.
            if (!IsPostBack)
            {
                LoadProducts();
            }
        }

        // TUTORIAL STEP: This method connects to the WCF service and loads products.
        private void LoadProducts()
        {
            // Create a connection to the WCF service
            // Since the service is in the same project, we can instantiate it directly.
            // (In a production app with a separate service host, you'd use a client proxy.)
            OrbitechService service = new OrbitechService();

            List<Product> products;
            string pageTitle = "All Devices";

            // Check for search term in the URL (?search=xxx)
            string searchTerm = Request.QueryString["search"];

            // Check for category in the URL (?cat=Phone)
            string category = Request.QueryString["cat"];

            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Search mode: user typed something in the search bar
                products = service.SearchProducts(searchTerm);
                pageTitle = "Search: \"" + searchTerm + "\"";
            }
            else if (!string.IsNullOrEmpty(category))
            {
                // Category filter mode: user clicked a category link
                products = service.GetProductsByCategory(category);
                pageTitle = category + "s";
            }
            else
            {
                // Default: show all products
                products = service.GetAllProducts();
            }

            // Update the page title
            PageTitleLabel.Text = pageTitle;

            // Update the product count text
            ProductCountLiteral.Text = "Showing " + products.Count + " product" + (products.Count != 1 ? "s" : "");

            // Bind the products to the Repeater control
            // The Repeater will generate one product card per product
            ProductRepeater.DataSource = products;
            ProductRepeater.DataBind();

            // Show "no results" panel if no products were found
            NoResultsPanel.Visible = (products.Count == 0);
        }
    }
}

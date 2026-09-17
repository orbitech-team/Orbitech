// ============================================================
// Default.aspx.cs — Code-Behind for Default.aspx (Home Page)
// ============================================================
// TUTORIAL STEP: This code loads the top 8 products from the database
// to show in the "Trending Tech" section on the homepage.
//
// PLACE THIS FILE IN: OrbitechWeb/Default.aspx.cs
// ============================================================

using System;
using System.Collections.Generic;
using OrbitechWeb.Models;
using OrbitechWeb.Services;

namespace OrbitechWeb
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadFeaturedProducts();
            }
        }

        // TUTORIAL STEP: Load featured products from the database.
        // We get all products and take the first 8 for the homepage.
        private void LoadFeaturedProducts()
        {
            OrbitechService service = new OrbitechService();
            List<Product> allProducts = service.GetAllProducts();

            // Take the first 8 products for the featured section
            List<Product> featured = new List<Product>();
            int count = 0;
            foreach (Product p in allProducts)
            {
                if (count >= 8) break;
                featured.Add(p);
                count++;
            }

            FeaturedRepeater.DataSource = featured;
            FeaturedRepeater.DataBind();
        }
    }
}

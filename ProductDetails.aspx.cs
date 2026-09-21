
using System;
using OrbitechWeb.Models;
using OrbitechWeb.Services;

namespace OrbitechWeb
{
    public partial class ProductDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadProduct();
            }
        }

        private void LoadProduct()
        {
            // Read the product ID from the URL (?id=5)
            string idParam = Request.QueryString["id"];

            if (string.IsNullOrEmpty(idParam))
            {
                // No ID provided — show "not found"
                ProductPanel.Visible = false;
                NotFoundPanel.Visible = true;
                return;
            }

            int productId;
            if (!int.TryParse(idParam, out productId))
            {
                // ID is not a valid number — show "not found"
                ProductPanel.Visible = false;
                NotFoundPanel.Visible = true;
                return;
            }

            // Call the WCF service to get the product
            OrbitechService service = new OrbitechService();
            Product product = service.GetProductById(productId);

            if (product == null)
            {
                // Product not found in database
                ProductPanel.Visible = false;
                NotFoundPanel.Visible = true;
                return;
            }

            // Product found — show the product panel and populate fields
            ProductPanel.Visible = true;
            NotFoundPanel.Visible = false;

            // Store product ID for the Add to Cart button
            ProductIdHidden.Value = product.ProductID.ToString();

            // Set page title
            Title = product.Name + " — OrbiTech";

            // Populate breadcrumb
            BreadCrumbLiteral.Text = product.Name;

            // Populate product image
            ProductImage.ImageUrl = product.ImageURL;

            // Populate category badge
            CategoryLiteral.Text = "&#9889; " + product.CategoryName;

            // Populate product name
            ProductNameLiteral.Text = product.Name;

            // Populate grade stars
            GradeStarsLiteral.Text = product.Grade == "A" ? "&#9733;&#9733;&#9733;&#9733;&#9733;" :
                                      product.Grade == "B" ? "&#9733;&#9733;&#9733;&#9733;&#9734;" :
                                      "&#9733;&#9733;&#9733;&#9734;&#9734;";
            GradeLiteral.Text = product.Grade;

            // Populate stock status
            StockLiteral.Text = product.Quantity > 0 ? "In stock &middot; Local Warranty" : "Out of stock";

            // Populate description
            DescriptionLiteral.Text = product.Description;

            // Populate price
            PriceLiteral.Text = product.Price.ToString("N2");

            // Populate brand, colour, condition
            BrandLiteral.Text = product.Brand;
            ColourLiteral.Text = string.IsNullOrEmpty(product.Colour) ? "Various" : product.Colour;
            ConditionLiteral.Text = product.Condition;
        }
        protected void AddToCartBtn_Click(object sender, EventArgs e)
        {
            // Must be logged in
            if (Session["Username"] == null)
            {
                Response.Redirect("~/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            int productId;
            if (!int.TryParse(ProductIdHidden.Value, out productId))
            {
                return;
            }

            int quantity = 1;
            int.TryParse(QtyInput.Text, out quantity);
            if (quantity < 1) quantity = 1;

            string username = Session["Username"].ToString();
            OrbitechService service = new OrbitechService();
            service.AddToCart(username, productId, quantity);

            Response.Redirect("~/Cart.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

    }
}

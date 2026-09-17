using System;
using System.Collections.Generic;
using System.Web;
using OrbitechWeb.Models;
using OrbitechWeb.Services;

namespace OrbitechWeb
{
    public partial class AdminProducts : System.Web.UI.Page
    {
        private int editingProductId = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            // SINGLE SECURITY CHECK: Session-based only.
            // The old code had a second check using FormsAuthenticationTicket
            // which we never set up — it always returned null and bounced
            // admins back to Login. Removed it.
            if (Session["Username"] == null || Session["IsAdmin"] == null || (bool)Session["IsAdmin"] != true)
            {
                Response.Redirect("~/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
            {
                LoadCategories();
                LoadProducts();
            }
        }

        private void LoadProducts()
        {
            OrbitechService service = new OrbitechService();
            List<Product> products = service.GetAllProducts();

            AdminProductRepeater.DataSource = products;
            AdminProductRepeater.DataBind();

            AdminCountLiteral.Text = products.Count + " products in database";
        }

        private void LoadCategories()
        {
            OrbitechService service = new OrbitechService();
            List<Category> categories = service.GetAllCategories();

            // FIXED: was "Name" — the Category model property is "CategoryName"
            DdlCategory.DataTextField = "CategoryName";
            DdlCategory.DataValueField = "CategoryID";
            DdlCategory.DataSource = categories;
            DdlCategory.DataBind();
        }

        protected void ShowAddBtn_Click(object sender, EventArgs e)
        {
            FormTitleLiteral.Text = "Add New Product";
            editingProductId = 0;

            TxtName.Text = "";
            TxtDescription.Text = "";
            TxtPrice.Text = "";
            TxtQuantity.Text = "";
            TxtImageURL.Text = "";
            TxtBrand.Text = "";
            TxtColour.Text = "";
            DdlCondition.SelectedIndex = 0;
            DdlGrade.SelectedIndex = 0;

            EditFormPanel.Visible = true;
        }

        protected void EditProduct_Click(object sender, EventArgs e)
        {
            int productId = Convert.ToInt32(((System.Web.UI.WebControls.LinkButton)sender).CommandArgument);

            OrbitechService service = new OrbitechService();
            Product product = service.GetProductById(productId);

            if (product != null)
            {
                // FIXED: Set ViewState directly instead of the private field.
                // The old code set editingProductId (private field), but OnInit
                // runs BEFORE the click event, so ViewState was never populated.
                // SaveProductBtn_Click checks ViewState, not the private field,
                // so it always thought it was adding a new product.
                ViewState["EditingProductId"] = productId;
                FormTitleLiteral.Text = "Edit: " + product.Name;

                TxtName.Text = product.Name;
                TxtDescription.Text = product.Description;
                TxtPrice.Text = product.Price.ToString();
                TxtQuantity.Text = product.Quantity.ToString();
                TxtImageURL.Text = product.ImageURL;
                TxtBrand.Text = product.Brand;
                TxtColour.Text = product.Colour;

                DdlCategory.SelectedValue = product.CategoryID.ToString();

                foreach (System.Web.UI.WebControls.ListItem item in DdlCondition.Items)
                {
                    item.Selected = (item.Value == product.Condition);
                }

                foreach (System.Web.UI.WebControls.ListItem item in DdlGrade.Items)
                {
                    item.Selected = (item.Value == product.Grade);
                }

                EditFormPanel.Visible = true;
            }
        }


        protected void SaveProductBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TxtName.Text) || string.IsNullOrEmpty(TxtDescription.Text) ||
                string.IsNullOrEmpty(TxtPrice.Text) || string.IsNullOrEmpty(TxtQuantity.Text) ||
                string.IsNullOrEmpty(TxtImageURL.Text) || string.IsNullOrEmpty(TxtBrand.Text))
            {
                ShowFormError("Please fill in all required fields (marked with *).");
                return;
            }

            decimal price;
            if (!decimal.TryParse(TxtPrice.Text, out price))
            {
                ShowFormError("Price must be a valid number (e.g., 2499.00).");
                return;
            }

            int quantity;
            if (!int.TryParse(TxtQuantity.Text, out quantity))
            {
                ShowFormError("Quantity must be a whole number.");
                return;
            }

            Product product = new Product
            {
                Name = TxtName.Text.Trim(),
                Description = TxtDescription.Text.Trim(),
                Price = price,
                Quantity = quantity,
                ImageURL = TxtImageURL.Text.Trim(),
                CategoryID = Convert.ToInt32(DdlCategory.SelectedValue),
                Brand = TxtBrand.Text.Trim(),
                Colour = TxtColour.Text.Trim(),
                Condition = DdlCondition.SelectedValue,
                Grade = DdlGrade.SelectedValue
            };

            OrbitechService service = new OrbitechService();
            bool success;

            if (ViewState["EditingProductId"] != null && (int)ViewState["EditingProductId"] > 0)
            {
                product.ProductID = (int)ViewState["EditingProductId"];
                success = service.UpdateProduct(product);
                ShowStatus(success ? "Product updated successfully!" : "Failed to update product.", success);
            }
            else
            {
                success = service.AddProduct(product);
                ShowStatus(success ? "Product added successfully!" : "Failed to add product.", success);
            }

            EditFormPanel.Visible = false;
            ViewState["EditingProductId"] = null;
            LoadProducts();
        }

        protected void DeleteProduct_Click(object sender, EventArgs e)
        {
            int productId = Convert.ToInt32(((System.Web.UI.WebControls.LinkButton)sender).CommandArgument);

            OrbitechService service = new OrbitechService();
            bool success = service.DeleteProduct(productId);

            ShowStatus(success ? "Product deleted successfully!" : "Failed to delete product.", success);
            LoadProducts();
        }

        protected void CancelBtn_Click(object sender, EventArgs e)
        {
            EditFormPanel.Visible = false;
            ViewState["EditingProductId"] = null;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (editingProductId > 0)
            {
                ViewState["EditingProductId"] = editingProductId;
            }
        }

        private void ShowStatus(string message, bool success)
        {
            StatusPanel.Visible = true;
            StatusPanel.CssClass = success ? "alert alert-success" : "alert alert-error";
            StatusLiteral.Text = message;
        }

        private void ShowFormError(string message)
        {
            FormErrorPanel.Visible = true;
            FormErrorLiteral.Text = message;
        }
    }
}

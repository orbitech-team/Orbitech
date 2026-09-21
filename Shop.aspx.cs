using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using OrbitechWeb.Models;
using OrbitechWeb.Services;

namespace OrbitechWeb
{
    public partial class Shop : System.Web.UI.Page
    {
        private List<int> _favouritedIds = new List<int>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Username"] != null)
            {
                OrbitechService service = new OrbitechService();
                _favouritedIds = service.GetFavouritedProductIds(Session["Username"].ToString());
            }

            if (!IsPostBack)
            {
                LoadProducts();
            }
        }

        private void LoadProducts()
        {
            OrbitechService service = new OrbitechService();

            List<Product> products;
            string pageTitle = "All Devices";

            string searchTerm = Request.QueryString["search"];
            string category = Request.QueryString["cat"];

            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = service.SearchProducts(searchTerm);
                pageTitle = "Search: \"" + searchTerm + "\"";
            }
            else if (!string.IsNullOrEmpty(category))
            {
                products = service.GetProductsByCategory(category);
                pageTitle = category + "s";
            }
            else
            {
                products = service.GetAllProducts();
            }

            PageTitleLabel.Text = pageTitle;
            ProductCountLiteral.Text = "Showing " + products.Count + " product" + (products.Count != 1 ? "s" : "");

            ProductRepeater.DataSource = products;
            ProductRepeater.DataBind();

            // No-results message: names the term when searching (Phase 1 bonus)
            if (products.Count == 0)
            {
                NoResultsPanel.Visible = true;
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    NoResultsMessage.Text = "No results for \"" + Server.HtmlEncode(searchTerm)
                        + "\" &mdash; try a brand like <strong>Samsung</strong> or a type like <strong>tablet</strong>.";
                }
                else
                {
                    NoResultsMessage.Text = "No products found. Try a different category or search term.";
                }
            }
            else
            {
                NoResultsPanel.Visible = false;
            }
        }

        // fill the heart on favourited cards, outline otherwise
        protected void ProductRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                LinkButton favButton = (LinkButton)e.Item.FindControl("FavButton");
                int productId = Convert.ToInt32(DataBinder.Eval(e.Item.DataItem, "ProductID"));
                favButton.Text = _favouritedIds.Contains(productId) ? "\u2665" : "\u2661";
            }
        }

        // PHASE 2: heart click - toggle favourite (login required)
        protected void ProductRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "FavToggle")
            {
                if (Session["Username"] == null)
                {
                    Response.Redirect("~/Login.aspx");
                    return;
                }

                int productId = Convert.ToInt32(e.CommandArgument);
                string username = Session["Username"].ToString();

                OrbitechService service = new OrbitechService();

                if (_favouritedIds.Contains(productId))
                {
                    service.RemoveFavourite(username, productId);
                    _favouritedIds.Remove(productId);
                }
                else
                {
                    service.AddFavourite(username, productId);
                    _favouritedIds.Add(productId);
                }

                LoadProducts(); // re-bind so the heart visually updates
            }
        }
    }
}

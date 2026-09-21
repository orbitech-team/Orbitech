using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using OrbitechWeb.Models;
using OrbitechWeb.Services;

namespace OrbitechWeb
{
    public partial class _Default : System.Web.UI.Page
    {
        // PHASE 2: which products the logged-in user has favourited
        private List<int> _favouritedIds = new List<int>();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Load favourites on EVERY load (postbacks too) so the heart
            // state is accurate when the toggle handler runs.
            if (Session["Username"] != null)
            {
                OrbitechService service = new OrbitechService();
                _favouritedIds = service.GetFavouritedProductIds(Session["Username"].ToString());
            }

            if (!IsPostBack)
            {
                LoadFeaturedProducts();
            }
        }

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

        //fill the heart on favourited cards, outline otherwise
        protected void ProductRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                LinkButton favButton = e.Item.FindControl("FavButton") as LinkButton;

                // Guard: markup/code mismatch would make this null
                if (favButton == null) return;

                // Guard: ProductID should always be an int, but never trust data blindly
                object rawId = DataBinder.Eval(e.Item.DataItem, "ProductID");
                if (rawId == null || rawId == DBNull.Value) return;

                int productId = Convert.ToInt32(rawId);
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

                LoadFeaturedProducts(); // re-bind so the heart visually updates
            }
        }
    }
}

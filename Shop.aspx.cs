using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OrbitechWeb.Models;
using OrbitechWeb.Services;

namespace OrbitechWeb
{
    public partial class Shop : System.Web.UI.Page
    {
        // FAVOURITES: which products the logged-in user has favourited
        private List<int> _favouritedIds = new List<int>();

        // FILTERS: every query key that counts as a filter (drives the "Clear all" link)
        private static readonly string[] FilterKeys = { "search", "cat", "condition", "brand", "grade", "min", "max" };

        protected void Page_Load(object sender, EventArgs e)
        {
            // FAVOURITES: load on EVERY load (postbacks too) so the
            // heart state is correct when the toggle handler runs.
            if (Session["Username"] != null)
            {
                OrbitechService service = new OrbitechService();
                _favouritedIds = service.GetFavouritedProductIds(Session["Username"].ToString());
            }

            // FILTERS: rebuild the sidebar links
            BuildFilterLinks();

            if (!IsPostBack)
            {
                // Reflect current sort + price in the controls
                string sort = Request.QueryString["sort"];
                if (!string.IsNullOrEmpty(sort))
                    SortDropDown.SelectedValue = sort;

                MinPriceInput.Text = Request.QueryString["min"] ?? "";
                MaxPriceInput.Text = Request.QueryString["max"] ?? "";

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

            // ---- FILTERING: in memory, composes with search/category ----
            products = ApplyFilters(products);

            // ---- SORTING: in memory, applied after filtering ----
            ApplySort(products, Request.QueryString["sort"]);

            PageTitleLabel.Text = pageTitle;
            ProductCountLiteral.Text = "Showing " + products.Count + " product" + (products.Count != 1 ? "s" : "");

            ProductRepeater.DataSource = products;
            ProductRepeater.DataBind();

            // No-results message: names the term when searching
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

        // FAVOURITES: fill the heart on favourited cards, outline otherwise
        protected void ProductRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                LinkButton favButton = e.Item.FindControl("FavButton") as LinkButton;
                if (favButton == null) return;

                object rawId = DataBinder.Eval(e.Item.DataItem, "ProductID");
                if (rawId == null || rawId == DBNull.Value) return;

                int productId = Convert.ToInt32(rawId);
                favButton.Text = _favouritedIds.Contains(productId) ? "\u2665" : "\u2661";
            }
        }

        // FAVOURITES: heart click - toggle favourite (login required)
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

        // ---------- filters ----------

        private List<Product> ApplyFilters(List<Product> products)
        {
            string condition = Request.QueryString["condition"];
            string brand = Request.QueryString["brand"];
            string grade = Request.QueryString["grade"];
            bool hasMin = !string.IsNullOrEmpty(Request.QueryString["min"]);
            bool hasMax = !string.IsNullOrEmpty(Request.QueryString["max"]);
            decimal minPrice, maxPrice;
            decimal.TryParse(Request.QueryString["min"], out minPrice);
            decimal.TryParse(Request.QueryString["max"], out maxPrice);

            List<Product> result = products;

            if (!string.IsNullOrEmpty(condition))
                result = result.Where(p => string.Equals(p.Condition, condition, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrEmpty(brand))
                result = result.Where(p => string.Equals(p.Brand, brand, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrEmpty(grade))
                result = result.Where(p => string.Equals(p.Grade, grade, StringComparison.OrdinalIgnoreCase)).ToList();

            if (hasMin)
                result = result.Where(p => p.Price >= minPrice).ToList();

            if (hasMax)
                result = result.Where(p => p.Price <= maxPrice).ToList();

            return result;
        }

        private void ApplySort(List<Product> products, string sort)
        {
            switch (sort)
            {
                case "price-asc":
                    products.Sort((a, b) => a.Price.CompareTo(b.Price));
                    break;
                case "price-desc":
                    products.Sort((a, b) => b.Price.CompareTo(a.Price));
                    break;
                case "name-asc":
                    products.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
                    break;
                    // "" / Featured: leave the service's natural order
            }
        }

        protected void SortDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            var qs = HttpUtility.ParseQueryString(Request.QueryString.ToString() ?? "");
            if (string.IsNullOrEmpty(SortDropDown.SelectedValue))
                qs.Remove("sort");
            else
                qs.Set("sort", SortDropDown.SelectedValue);
            Response.Redirect("Shop.aspx" + (qs.Count > 0 ? "?" + qs.ToString() : ""), false);
        }

        protected void ApplyPriceBtn_Click(object sender, EventArgs e)
        {
            string min = MinPriceInput.Text.Trim();
            string max = MaxPriceInput.Text.Trim();
            decimal value;

            var qs = HttpUtility.ParseQueryString(Request.QueryString.ToString() ?? "");

            if (min.Length > 0 && decimal.TryParse(min, out value))
                qs.Set("min", value.ToString());
            else
                qs.Remove("min");

            if (max.Length > 0 && decimal.TryParse(max, out value))
                qs.Set("max", value.ToString());
            else
                qs.Remove("max");

            Response.Redirect("Shop.aspx" + (qs.Count > 0 ? "?" + qs.ToString() : ""), false);
        }

        // ---------- filter link rendering ----------

        private string BuildFilterUrl(string key, string value)
        {
            var qs = HttpUtility.ParseQueryString(Request.QueryString.ToString() ?? "");
            if (string.IsNullOrEmpty(value))
                qs.Remove(key);
            else
                qs.Set(key, value);
            return "Shop.aspx" + (qs.Count > 0 ? "?" + qs.ToString() : "");
        }

        private string FilterLink(string key, string value, string label, bool active)
        {
            string cls = active ? "filter-link is-active" : "filter-link";
            return "<a class='" + cls + "' href='" + BuildFilterUrl(key, value) + "'>" +
                   HttpUtility.HtmlEncode(label) + "</a>";
        }

        private void BuildFilterLinks()
        {
            string cat = Request.QueryString["cat"];
            string condition = Request.QueryString["condition"];
            string grade = Request.QueryString["grade"];
            string brand = Request.QueryString["brand"];

            // ---- Category ----
            CategoryFiltersLiteral.Text =
                FilterLink("cat", null, "All Products", string.IsNullOrEmpty(cat)) +
                FilterLink("cat", "Phone", "Cellphones", cat == "Phone") +
                FilterLink("cat", "Tablet", "Tablets", cat == "Tablet") +
                FilterLink("cat", "Smartwatch", "Smartwatches", cat == "Smartwatch");

            // ---- Condition ----
            ConditionFiltersLiteral.Text =
                FilterLink("condition", null, "Any condition", string.IsNullOrEmpty(condition)) +
                FilterLink("condition", "New", "Brand New", condition == "New") +
                FilterLink("condition", "Refurbished", "Certified Pre-Owned", condition == "Refurbished");

            // ---- Grade ----
            GradeFiltersLiteral.Text =
                FilterLink("grade", null, "Any grade", string.IsNullOrEmpty(grade)) +
                FilterLink("grade", "A", "Grade A", grade == "A") +
                FilterLink("grade", "B", "Grade B", grade == "B") +
                FilterLink("grade", "C", "Grade C", grade == "C");

            // ---- Brand: built dynamically from the catalogue ----
            OrbitechService service = new OrbitechService();
            List<string> brands = service.GetAllProducts()
                .Select(p => p.Brand)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(b => b)
                .ToList();

            StringBuilder html = new StringBuilder();
            html.Append(FilterLink("brand", null, "All brands", string.IsNullOrEmpty(brand)));
            foreach (string b in brands)
            {
                html.Append(FilterLink("brand", b, b,
                    string.Equals(brand, b, StringComparison.OrdinalIgnoreCase)));
            }
            BrandFiltersLiteral.Text = html.ToString();

            // ---- Clear all (only shown when a filter is active) ----
            bool anyFilter = FilterKeys.Any(k => !string.IsNullOrEmpty(Request.QueryString[k]));
            ClearFiltersLiteral.Text = anyFilter
                ? "<a class='filter-link filter-link--clear' href='Shop.aspx'>&#10005; Clear all filters</a>"
                : "";
        }
    }
}

using System;
using System.Collections.Generic;
using OrbitechWeb.Models;
using OrbitechWeb.Services;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OrbitechWeb
{
    public partial class Favourite : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Same guard as Cart: favourites need a logged-in user
            if (Session["Username"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadFavourites();
            }
        }

        private void LoadFavourites()
        {
            string username = Session["Username"].ToString();

            // Create the client the same way Cart.aspx.cs does -
            // keep your existing using/namespace for the service client.
            OrbitechService client = new OrbitechService();
            List<Product> favourites = client.GetFavourites(username);

            FavouritesRepeater.DataSource = favourites;
            FavouritesRepeater.DataBind();

            EmptyPanel.Visible = (favourites.Count == 0);
        }

        protected void FavouritesRepeater_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "RemoveFav")
            {
                string username = Session["Username"].ToString();
                int productId = Convert.ToInt32(e.CommandArgument);

                OrbitechService client = new OrbitechService();
                client.RemoveFavourite(username, productId);

                LoadFavourites(); // refresh the grid so the card disappears
            }
        }
    }
}
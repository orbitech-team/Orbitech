<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Favourite.aspx.cs" Inherits="OrbitechWeb.Favourite" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <meta name="description" content="Your saved OrbiTech devices." />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

  <section class="page-head">
    <div class="container">
      <div class="crumbs"><a href='<%= ResolveUrl("~/Default.aspx") %>'>Home</a> <span class="sep">&rsaquo;</span> <span>Favourites</span></div>
      <h1>Your Favourites</h1>
      <p>Devices you've saved for later. Tap the heart again anywhere to remove them.</p>
    </div>
  </section>

  <section class="section">
    <div class="container">
      <div class="shop-grid">
        <asp:Repeater ID="FavouritesRepeater" runat="server" OnItemCommand="FavouritesRepeater_ItemCommand">
          <ItemTemplate>
            <article class="product-card">
              <div class="img-wrap">
                <span class='<%# Eval("Condition").ToString() == "New" ? "badge" : "badge badge--refurb" %>'>
                  <%# Eval("Condition") %>
                </span>
                <img src='<%# Eval("ImageURL") %>' alt='<%# Eval("Name") %>' />
              </div>
              <div class="stock">
                <span class="dot"></span>
                <%# Eval("Brand") %>
              </div>
              <a href='<%# ResolveUrl("~/ProductDetails.aspx?id=") + Eval("ProductID") %>' class="name">
                <%# Eval("Name") %>
              </a>
              <div class="price">
                <span class="now">R<%# Eval("Price", "{0:N2}") %></span>
              </div>
              <div class="stars">
                <span class="count">Grade <%# Eval("Grade") %></span>
              </div>
              <a href='<%# ResolveUrl("~/Cart.aspx?action=add&id=") + Eval("ProductID") %>' class="btn">Order now &rarr;</a>
              <div style="margin-top: 8px; text-align: center;">
                <asp:LinkButton ID="RemoveFavButton" runat="server"
                    CommandName="RemoveFav"
                    CommandArgument='<%# Eval("ProductID") %>'
                    style="font-size: 12px; color: var(--fg-mute); text-decoration: underline;">
                  Remove from favourites
                </asp:LinkButton>
              </div>
            </article>
          </ItemTemplate>
        </asp:Repeater>
      </div>

      <asp:Panel ID="EmptyPanel" runat="server" Visible="false" style="padding: var(--s9); text-align: center;">
        <p style="font-size: var(--text-lg); color: var(--fg-mute);">
          No favourites yet &mdash; tap the &hearts; on any product in the
          <a href='<%= ResolveUrl("~/Shop.aspx") %>'>shop</a>.
        </p>
      </asp:Panel>
    </div>
  </section>

</asp:Content>

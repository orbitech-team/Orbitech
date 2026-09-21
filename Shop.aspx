<%@ Page Title="Shop Tech — OrbiTech" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Shop.aspx.cs" Inherits="OrbitechWeb.Shop" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <meta name="description" content="Browse OrbiTech's premium selection of smartphones, tablets, and smartwatches." />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

  <section class="page-head">
    <div class="container">
      <div class="crumbs"><a href='<%= ResolveUrl("~/Default.aspx") %>'>Home</a> <span class="sep">&rsaquo;</span> <span>Shop &middot; Portable Tech</span></div>
      <h1>
        <asp:Label ID="PageTitleLabel" runat="server" Text="All Devices" />
      </h1>
      <p>Browse our curated selection of smartphones, tablets, and smartwatches. From the latest flagship releases to certified pre-owned value deals.</p>
    </div>
  </section>

  <section class="section">
    <div class="container">
      <div class="shop-layout">

        <aside class="filters" aria-label="Filters">
          <div class="filter-block">
            <h3>Category</h3>
            <label><a href='<%= ResolveUrl("~/Shop.aspx") %>' style="text-decoration:none; color:inherit;">All Products</a></label>
            <label><a href='<%= ResolveUrl("~/Shop.aspx?cat=Phone") %>' style="text-decoration:none; color:inherit;">Cellphones</a></label>
            <label><a href='<%= ResolveUrl("~/Shop.aspx?cat=Tablet") %>' style="text-decoration:none; color:inherit;">Tablets</a></label>
            <label><a href='<%= ResolveUrl("~/Shop.aspx?cat=Smartwatch") %>' style="text-decoration:none; color:inherit;">Smartwatches</a></label>
          </div>
          <div class="filter-block">
            <h3>Price range</h3>
            <div style="display:flex; justify-content:space-between; font-family:var(--ff-mono); font-size:11px; color:var(--fg-mute)"><span>R199</span><span>R26,000</span></div>
            <div class="range-bar" aria-hidden="true"></div>
          </div>
          <div class="filter-block">
            <h3>Condition</h3>
            <label><a href='<%= ResolveUrl("~/Shop.aspx?condition=New") %>' style="text-decoration:none; color:inherit;">Brand New</a></label>
            <label><a href='<%= ResolveUrl("~/Shop.aspx?condition=Refurbished") %>' style="text-decoration:none; color:inherit;">Certified Pre-Owned</a></label>
          </div>
        </aside>

        <asp:UpdatePanel ID="ShopUpdatePanel" runat="server">
          <ContentTemplate>

            <div class="shop-toolbar">
              <span class="count">
                <asp:Literal ID="ProductCountLiteral" runat="server" Text="Loading products..." />
              </span>
            </div>

            <div class="shop-grid">
              <asp:Repeater ID="ProductRepeater" runat="server"
                  OnItemCommand="ProductRepeater_ItemCommand"
                  OnItemDataBound="ProductRepeater_ItemDataBound">
                <ItemTemplate>
                  <article class="product-card">
                    <div class="img-wrap">
                      <span class='<%# Eval("Condition").ToString() == "New" ? "badge" : "badge badge--refurb" %>'>
                        <%# Eval("Condition") %>
                      </span>

                      <asp:LinkButton ID="FavButton" runat="server"
                          CommandName="FavToggle"
                          CommandArgument='<%# Eval("ProductID") %>'
                          CssClass="fav-btn" />

                      <img src='<%# Eval("ImageURL") %>' alt='<%# Eval("Name") %>' />
                    </div>
                    <div class="stock">
                      <span class="dot"></span>
                      <%# Convert.ToInt32(Eval("Quantity")) > 0 ? "In stock" : "Out of stock" %>
                      &middot; <%# Eval("Brand") %>
                    </div>
                    <a href='<%# ResolveUrl("~/ProductDetails.aspx?id=") + Eval("ProductID") %>' class="name">
                      <%# Eval("Name") %>
                    </a>
                    <div class="price">
                      <span class="now">R<%# Eval("Price", "{0:N2}") %></span>
                    </div>
                    <div class="stars">
                      <%# Eval("Grade").ToString() == "A" ? "&#9733;&#9733;&#9733;&#9733;&#9733;" : Eval("Grade").ToString() == "B" ? "&#9733;&#9733;&#9733;&#9733;&#9734;" : "&#9733;&#9733;&#9733;&#9734;&#9734;" %>
                      <span class="count">Grade <%# Eval("Grade") %></span>
                    </div>
                    <a href='<%# ResolveUrl("~/Cart.aspx?action=add&id=") + Eval("ProductID") %>' class="btn">Order now &rarr;</a>
                  </article>
                </ItemTemplate>
              </asp:Repeater>
            </div>

            <asp:Panel ID="NoResultsPanel" runat="server" Visible="false" style="padding: var(--s9); text-align: center;">
              <asp:Literal ID="NoResultsMessage" runat="server" />
            </asp:Panel>

          </ContentTemplate>
        </asp:UpdatePanel>

      </div>
    </div>
  </section>

</asp:Content>

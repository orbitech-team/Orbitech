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

        <!-- FILTERS SIDEBAR: all blocks are rendered by code-behind
             into Literals so every link preserves the other active
             filters (e.g. switching brand keeps your category). -->
        <aside class="filters" aria-label="Filters">
          <div class="filter-block">
            <h3>Category</h3>
            <asp:Literal ID="CategoryFiltersLiteral" runat="server" />
          </div>
          <div class="filter-block">
            <h3>Price range (R)</h3>
            <div class="price-filter">
              <asp:TextBox ID="MinPriceInput" runat="server" placeholder="Min" inputmode="numeric" />
              <span class="dash">&ndash;</span>
              <asp:TextBox ID="MaxPriceInput" runat="server" placeholder="Max" inputmode="numeric" />
            </div>
            <asp:Button ID="ApplyPriceBtn" runat="server" Text="Apply price" CssClass="btn btn--ghost btn--sm" OnClick="ApplyPriceBtn_Click" />
          </div>
          <div class="filter-block">
            <h3>Condition</h3>
            <asp:Literal ID="ConditionFiltersLiteral" runat="server" />
          </div>
          <div class="filter-block">
            <h3>Brand</h3>
            <asp:Literal ID="BrandFiltersLiteral" runat="server" />
          </div>
          <div class="filter-block">
            <h3>Grade</h3>
            <asp:Literal ID="GradeFiltersLiteral" runat="server" />
          </div>
          <asp:Literal ID="ClearFiltersLiteral" runat="server" />
        </aside>

        <asp:UpdatePanel ID="ShopUpdatePanel" runat="server">
          <ContentTemplate>

            <div class="shop-toolbar">
              <span class="count">
                <asp:Literal ID="ProductCountLiteral" runat="server" Text="Loading products..." />
              </span>
              <label class="sort-wrap">Sort:
                <asp:DropDownList ID="SortDropDown" runat="server" AutoPostBack="true" OnSelectedIndexChanged="SortDropDown_SelectedIndexChanged">
                  <asp:ListItem Value="" Text="Featured" />
                  <asp:ListItem Value="price-asc" Text="Price: Low to High" />
                  <asp:ListItem Value="price-desc" Text="Price: High to Low" />
                  <asp:ListItem Value="name-asc" Text="Name: A to Z" />
                </asp:DropDownList>
              </label>
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
              <p style="font-size: var(--text-lg); color: var(--fg-mute);">
                <asp:Literal ID="NoResultsMessage" runat="server" Text="No products found. Try a different filter combination." />
              </p>
            </asp:Panel>

          </ContentTemplate>
        </asp:UpdatePanel>

      </div>
    </div>
  </section>

</asp:Content>

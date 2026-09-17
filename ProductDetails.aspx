<%-- ============================================================
     ProductDetails.aspx — Single Product Detail Page
     ============================================================
     TUTORIAL STEP: This page replaces product.html. Instead of hardcoded
     product info, it loads a single product from the database using the
     "id" query parameter (e.g., ProductDetails.aspx?id=5).

     PLACE THIS FILE IN: OrbitechWeb/ProductDetails.aspx
     ============================================================ --%>

<%@ Page Title="Product Details — OrbiTech" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProductDetails.aspx.cs" Inherits="OrbitechWeb.ProductDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <meta name="description" content="View product details at OrbiTech." />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

  <div class="container">
    <div class="crumbs" style="padding-top: var(--s5)">
      <a href='<%= ResolveUrl("~/Default.aspx") %>'>Home</a>
      <span class="sep">&rsaquo;</span>
      <a href='<%= ResolveUrl("~/Shop.aspx") %>'>Shop</a>
      <span class="sep">&rsaquo;</span>
      <span><asp:Literal ID="BreadCrumbLiteral" runat="server" Text="Product" /></span>
    </div>

    <!-- TUTORIAL STEP: This Panel is shown if the product is found -->
    <asp:Panel ID="ProductPanel" runat="server" Visible="false">
      <section class="product-detail">
        <div class="gallery">
          <figure class="gallery-main">
            <asp:Image ID="ProductImage" runat="server" AlternateText="Product Image" style="width: 100%; border-radius: var(--r);" />
          </figure>
        </div>

        <div class="pdp-info">
          <span class="pdp-cat">
            <asp:Literal ID="CategoryLiteral" runat="server" />
          </span>
          <h1><asp:Literal ID="ProductNameLiteral" runat="server" /></h1>

          <div class="rating-row">
            <span style="color: var(--amber); font-size: 16px">
              <asp:Literal ID="GradeStarsLiteral" runat="server" />
            </span>
            <span>Grade <asp:Literal ID="GradeLiteral" runat="server" /></span>
            <span style="color: var(--rule-strong)">|</span>
            <span style="color: var(--emerald); display:inline-flex; align-items:center; gap:6px">
              <span style="width:6px;height:6px;background:var(--emerald);border-radius:999px;display:inline-block"></span>
              <asp:Literal ID="StockLiteral" runat="server" />
            </span>
          </div>

          <p class="desc"><asp:Literal ID="DescriptionLiteral" runat="server" /></p>

          <div class="price-row">
            <span class="now">R<asp:Literal ID="PriceLiteral" runat="server" /></span>
          </div>

          <div class="option-block">
            <div class="label">Brand</div>
            <div style="font-weight:600; font-size: var(--text-base)">
              <asp:Literal ID="BrandLiteral" runat="server" />
            </div>
          </div>

          <div class="option-block">
            <div class="label">Colour</div>
            <div style="font-weight:600; font-size: var(--text-base)">
              <asp:Literal ID="ColourLiteral" runat="server" />
            </div>
          </div>

          <div class="option-block">
            <div class="label">Condition</div>
            <div style="font-weight:600; font-size: var(--text-base)">
              <asp:Literal ID="ConditionLiteral" runat="server" />
            </div>
          </div>

          <div class="pdp-cta">
            <asp:HiddenField ID="ProductIdHidden" runat="server" />
            <div class="qty">
              <button type="button" data-act="-" aria-label="Decrease">&minus;</button>
              <asp:TextBox ID="QtyInput" runat="server" Text="1" inputmode="numeric" aria-label="Quantity" style="width:40px;text-align:center;border:1px solid var(--rule);border-radius:var(--r-sm)" />
              <button type="button" data-act="+" aria-label="Increase">+</button>
            </div>
            <asp:Button ID="AddToCartBtn" runat="server" Text="Add to cart &rarr;" CssClass="btn btn--indigo" Style="flex:1; min-width:160px" OnClick="AddToCartBtn_Click" />
          </div>


          <div class="pdp-features">
            <div class="pf"><span class="ic">&#9889;</span><span>Free SA-wide delivery</span></div>
            <div class="pf"><span class="ic">&#8634;</span><span>14-day hassle-free returns</span></div>
            <div class="pf"><span class="ic">&#9733;</span><span>12-month OrbiTech warranty</span></div>
            <div class="pf"><span class="ic">&#10003;</span><span>ICASA Approved</span></div>
          </div>
        </div>
      </section>
    </asp:Panel>

    <!-- TUTORIAL STEP: This Panel is shown if the product ID is missing or invalid -->
    <asp:Panel ID="NotFoundPanel" runat="server" Visible="false" style="padding: var(--s9); text-align:center;">
      <h1 style="font-size: var(--text-2xl); margin-bottom: var(--s5);">Product Not Found</h1>
      <p style="color: var(--fg-mute); margin-bottom: var(--s7);">The product you're looking for doesn't exist or has been removed.</p>
      <a href='<%= ResolveUrl("~/Shop.aspx") %>' class="btn btn--indigo">Back to Shop &rarr;</a>
    </asp:Panel>

  </div>

</asp:Content>

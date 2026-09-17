<%-- ============================================================
     Default.aspx — Home Page (converted from index.html)
     ============================================================
     TUTORIAL STEP: This page replaces index.html. The main change is
     the <%@ Page %> directive at the top and the ContentPlaceHolder tags.
     Most of the HTML is the same — only the page directive and form/links
     have been adjusted for ASP.NET.

     PLACE THIS FILE IN: OrbitechWeb/Default.aspx
     ============================================================ --%>

<%@ Page Title="OrbiTech — Your Tech, In Orbit" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="OrbitechWeb._Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <meta name="description" content="OrbiTech is South Africa's premier B2C e-commerce platform for new and certified pre-owned smartphones, tablets, and smartwatches." />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

  <!-- HERO: bento grid -->
  <section class="hero">
    <div class="container">
      <div class="bento">

        <article class="bento-card bento-card--lg">
          <div class="sparkle"></div>
          <div>
            <span class="eyebrow">&#9889; Featured Release</span>
            <h2>iPhone 15 Pro<br/>Titanium Power</h2>
            <p>Experience the next generation of mobile performance. Aerospace-grade titanium design, A17 Pro chip, and the most advanced camera system yet.</p>
            <a href='<%= ResolveUrl("~/Shop.aspx?cat=Phone") %>' class="btn btn--paper">Explore iPhones
              <svg width="14" height="10" viewBox="0 0 14 10" fill="none" aria-hidden="true"><path d="M1 5h12m0 0L9 1m4 4L9 9" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"/></svg>
            </a>
            <div class="dots"><span class="active"></span><span></span><span></span></div>
          </div>
          <img class="product" src="https://images.unsplash.com/photo-1696446701796-da61225697cc?w=900&q=80&auto=format&fit=crop" alt="iPhone 15 Pro" />
        </article>

        <article class="bento-card bento-card--purple">
          <div class="sparkle"></div>
          <span class="eyebrow">Smartwatches</span>
          <h3 style="font-size:var(--text-xl); line-height:1.15">Apple Watch<br/>Series 9</h3>
          <a href='<%= ResolveUrl("~/Shop.aspx?cat=Smartwatch") %>' class="shop-now">Shop Wearables
            <svg width="12" height="10" viewBox="0 0 14 10" fill="none" aria-hidden="true"><path d="M1 5h12m0 0L9 1m4 4L9 9" stroke="currentColor" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round"/></svg>
          </a>
          <img class="product" src="https://images.unsplash.com/photo-1544117518-30dd5ff7a4b0?w=600&q=80&auto=format&fit=crop" alt="Apple Watch" />
        </article>

        <article class="bento-card bento-card--teal">
          <div class="sparkle"></div>
          <span class="eyebrow">Tablets</span>
          <h3 style="font-size:var(--text-xl); line-height:1.15">iPad Pro<br/>M2 Chip</h3>
          <a href='<%= ResolveUrl("~/Shop.aspx?cat=Tablet") %>' class="shop-now">View iPads
            <svg width="12" height="10" viewBox="0 0 14 10" fill="none" aria-hidden="true"><path d="M1 5h12m0 0L9 1m4 4L9 9" stroke="currentColor" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round"/></svg>
          </a>
          <img class="product" src="https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=600&q=80&auto=format&fit=crop" alt="iPad Pro" />
        </article>

        <div class="bento-row">
          <article class="bento-card bento-card--orange">
            <div class="sparkle"></div>
            <span class="eyebrow">Value Added</span>
            <h3 style="font-size:var(--text-lg); line-height:1.2">Instant<br/>Trade-In</h3>
            <a href="#" class="shop-now">Get Value
              <svg width="12" height="10" viewBox="0 0 14 10" fill="none" aria-hidden="true"><path d="M1 5h12m0 0L9 1m4 4L9 9" stroke="currentColor" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round"/></svg>
            </a>
            <img class="product" src="https://images.unsplash.com/photo-1556656793-062ff9878258?w=500&q=80&auto=format&fit=crop" alt="Trade-in" />
          </article>

          <article class="bento-card bento-card--green">
            <div class="sparkle"></div>
            <span class="eyebrow">Certified</span>
            <h3 style="font-size:var(--text-lg); line-height:1.2">Refurbished<br/>Excellence</h3>
            <a href='<%= ResolveUrl("~/Shop.aspx") %>' class="shop-now">Browse Deals
              <svg width="12" height="10" viewBox="0 0 14 10" fill="none" aria-hidden="true"><path d="M1 5h12m0 0L9 1m4 4L9 9" stroke="currentColor" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round"/></svg>
            </a>
            <img class="product" src="https://images.unsplash.com/photo-1512499617640-c74ae3a79d37?w=500&q=80&auto=format&fit=crop" alt="Refurbished Phone" />
          </article>

          <article class="bento-card bento-card--black">
            <div class="sparkle"></div>
            <span class="eyebrow">Student Portal</span>
            <h3 style="font-size:var(--text-lg); line-height:1.2">Exclusive<br/>Discounts</h3>
            <a href='<%= ResolveUrl("~/Login.aspx") %>' class="shop-now">Verify Now
              <svg width="12" height="10" viewBox="0 0 14 10" fill="none" aria-hidden="true"><path d="M1 5h12m0 0L9 1m4 4L9 9" stroke="currentColor" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round"/></svg>
            </a>
            <img class="product" src="https://images.unsplash.com/photo-1522202176988-66273c2fd55f?w=500&q=80&auto=format&fit=crop" alt="Students" />
          </article>
        </div>

      </div>
    </div>
  </section>

  <!-- TRENDING PRODUCTS — dynamically loaded from database -->
  <section class="section" style="padding-top: var(--s5)">
    <div class="container">
      <div class="section-head">
        <h2>Trending Tech</h2>
        <a href='<%= ResolveUrl("~/Shop.aspx") %>' class="view-all">View all
          <svg width="14" height="10" viewBox="0 0 14 10" fill="none"><path d="M1 5h12m0 0L9 1m4 4L9 9" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"/></svg>
        </a>
      </div>

      <!-- TUTORIAL STEP: Featured products loaded from database.
           The Repeater generates product cards from the top 8 products. -->
      <div class="product-grid" style="display:grid; grid-template-columns:repeat(4, 1fr); gap: var(--s5);">
        <asp:Repeater ID="FeaturedRepeater" runat="server">
          <ItemTemplate>
            <article class="product-card">
              <div class="img-wrap">
                <span class='<%# Eval("Condition").ToString() == "New" ? "badge" : "badge badge--refurb" %>'>
                  <%# Eval("Condition") %>
                </span>
                <img src='<%# Eval("ImageURL") %>' alt='<%# Eval("Name") %>' />
              </div>
              <div class="stock"><span class="dot"></span><%# Eval("Brand") %></div>
              <a href='<%# ResolveUrl("~/ProductDetails.aspx?id=") + Eval("ProductID") %>' class="name"><%# Eval("Name") %></a>
              <div class="price"><span class="now">R<%# Eval("Price", "{0:N2}") %></span></div>
              <a href='<%# ResolveUrl("~/ProductDetails.aspx?id=") + Eval("ProductID") %>' class="btn">View details &rarr;</a>
            </article>
          </ItemTemplate>
        </asp:Repeater>
      </div>
    </div>
  </section>

  <!-- VALUE PROPS -->
  <section class="section">
    <div class="container">
      <div style="display:grid; grid-template-columns:repeat(3, 1fr); gap: var(--s5);">
        <div style="padding:var(--s6); background:var(--bg); border-radius:var(--r);">
          <div style="font-size:28px; margin-bottom:var(--s3)">&#9889;</div>
          <h3 style="font-size:var(--text-md); margin-bottom:var(--s2)">Free SA Delivery</h3>
          <p style="font-size:var(--text-sm); color:var(--fg-mute)">On all orders over R1000. Delivered in 2-4 business days.</p>
        </div>
        <div style="padding:var(--s6); background:var(--bg); border-radius:var(--r);">
          <div style="font-size:28px; margin-bottom:var(--s3)">&#8634;</div>
          <h3 style="font-size:var(--text-md); margin-bottom:var(--s2)">14-Day Returns</h3>
          <p style="font-size:var(--text-sm); color:var(--fg-mute)">Changed your mind? Return any device within 14 days, hassle-free.</p>
        </div>
        <div style="padding:var(--s6); background:var(--bg); border-radius:var(--r);">
          <div style="font-size:28px; margin-bottom:var(--s3)">&#9733;</div>
          <h3 style="font-size:var(--text-md); margin-bottom:var(--s2)">12M Warranty</h3>
          <p style="font-size:var(--text-sm); color:var(--fg-mute)">Every device comes with a 12-month OrbiTech warranty.</p>
        </div>
      </div>
    </div>
  </section>

</asp:Content>

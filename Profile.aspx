<%@ Page Title="My Profile — OrbiTech" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="OrbitechWeb.Profile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <meta name="description" content="Your OrbiTech profile and account overview." />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

  <section class="page-head">
    <div class="container">
      <div class="crumbs"><a href='<%= ResolveUrl("~/Default.aspx") %>'>Home</a> <span class="sep">&rsaquo;</span> <span>Profile</span></div>
      <h1>My Profile</h1>
      <p>Your account, orders and activity at a glance.</p>
    </div>
  </section>

  <section class="section">
    <div class="container">

      <!-- ACCOUNT CARD (both views) -->
      <div class="profile-card">
        <div class="profile-id">
          <div class="avatar"><asp:Literal ID="AvatarLiteral" runat="server" /></div>
          <div>
            <div class="profile-name"><asp:Literal ID="UsernameLiteral" runat="server" /></div>
            <div class="profile-role"><asp:Literal ID="RoleLiteral" runat="server" /></div>
            <div class="profile-since"><asp:Literal ID="MemberSinceLiteral" runat="server" /></div>
          </div>
        </div>
        <div class="profile-actions">
          <a href='<%= ResolveUrl("~/Cart.aspx") %>' class="btn btn--ghost">My Cart</a>
          <!-- PHASE 2 teammate's page: link goes live after their merge -->
          <a href='<%= ResolveUrl("~/Favourites.aspx") %>' class="btn btn--ghost">My Favourites</a>
          <a href='<%= ResolveUrl("~/Logout.aspx") %>' class="btn btn--ghost">Log out</a>
        </div>
      </div>

      <!-- ================= CUSTOMER VIEW ================= -->
      <asp:Panel ID="CustomerViewPanel" runat="server" Visible="false">
        <h2 class="profile-h2">My Activity</h2>
        <div class="stat-tiles">
          <div class="stat-tile">
            <p class="stat-label">Orders placed</p>
            <p class="stat-num"><asp:Literal ID="CustOrdersLiteral" runat="server" /></p>
          </div>
          <div class="stat-tile">
            <p class="stat-label">Total spent</p>
            <p class="stat-num">R<asp:Literal ID="CustSpentLiteral" runat="server" /></p>
          </div>
          <div class="stat-tile">
            <p class="stat-label">Favourites</p>
            <p class="stat-num"><asp:Literal ID="CustFavsLiteral" runat="server" /></p>
          </div>
        </div>

        <h2 class="profile-h2">Recent Orders</h2>
        <asp:Panel ID="CustNoOrdersPanel" runat="server" Visible="false" style="color: var(--fg-mute); padding: var(--s5) 0;">
          <p>You haven't placed any orders yet. <a href='<%= ResolveUrl("~/Shop.aspx") %>'>Browse the catalog</a> to get started.</p>
        </asp:Panel>

        <asp:Repeater ID="CustOrdersRepeater" runat="server">
          <ItemTemplate>
            <div class="order-row">
              <div class="order-main">
                <span class="order-inv"><%# Eval("InvoiceNumber") %></span>
                <span class="order-date"><%# Eval("OrderDate", "{0:dd MMM yyyy}") %></span>
              </div>
              <span class="order-status"><%# Eval("Status") %></span>
              <span class="order-total">R<%# Eval("Total", "{0:N2}") %></span>
              <a href='<%# ResolveUrl("~/InvoiceView.aspx") %>?id=<%# Eval("OrderID") %>' class="order-invoice-link" style="font-size: var(--text-sm); color: var(--primary); font-weight: 600; white-space: nowrap;">View invoice &rarr;</a>
            </div>
          </ItemTemplate>
        </asp:Repeater>
      </asp:Panel>

      <!-- ================= ADMIN VIEW ================= -->
      <asp:Panel ID="AdminViewPanel" runat="server" Visible="false">
        <h2 class="profile-h2">Store Overview</h2>
        <div class="stat-tiles">
          <div class="stat-tile">
            <p class="stat-label">Products</p>
            <p class="stat-num"><asp:Literal ID="AdminProductsLiteral" runat="server" /></p>
          </div>
          <div class="stat-tile">
            <p class="stat-label">Orders</p>
            <p class="stat-num"><asp:Literal ID="AdminOrdersLiteral" runat="server" /></p>
          </div>
          <div class="stat-tile">
            <p class="stat-label">Registered users</p>
            <p class="stat-num"><asp:Literal ID="AdminUsersLiteral" runat="server" /></p>
          </div>
          <div class="stat-tile">
            <p class="stat-label">Total revenue</p>
            <p class="stat-num">R<asp:Literal ID="AdminRevenueLiteral" runat="server" /></p>
          </div>
        </div>

        <div class="profile-actions" style="margin-top: var(--s5);">
          <a href='<%= ResolveUrl("~/AdminProducts.aspx") %>' class="btn btn--indigo">Manage Products</a>
          <!-- MEMBER C's page: link activates once their Reports work merges -->
          <a href='<%= ResolveUrl("~/AdminsReport.aspx") %>' class="btn btn--ghost">Full Reports</a>
        </div>

        <h2 class="profile-h2">Latest Orders (all customers)</h2>
        <asp:Repeater ID="AdminOrdersRepeater" runat="server">
          <ItemTemplate>
            <div class="order-row">
              <div class="order-main">
                <span class="order-inv"><%# Eval("InvoiceNumber") %></span>
                <span class="order-date"><%# Eval("OrderDate", "{0:dd MMM yyyy, HH:mm}") %></span>
              </div>
              <span class="order-user"><%# Eval("Username") %></span>
              <span class="order-status"><%# Eval("Status") %></span>
              <span class="order-total">R<%# Eval("Total", "{0:N2}") %></span>
              <a href='<%# ResolveUrl("~/InvoiceView.aspx") %>?id=<%# Eval("OrderID") %>' class="order-invoice-link" style="font-size: var(--text-sm); color: var(--primary); font-weight: 600; white-space: nowrap;">View invoice &rarr;</a>
            </div>
          </ItemTemplate>
        </asp:Repeater>

        <h2 class="profile-h2">Low Stock Alerts</h2>
        <asp:Panel ID="NoLowStockPanel" runat="server" Visible="false" style="color: var(--fg-mute); padding: var(--s5) 0;">
          <p>No products are low on stock right now.</p>
        </asp:Panel>
        <div class="lowstock-list">
          <asp:Repeater ID="LowStockRepeater" runat="server">
            <ItemTemplate>
              <a class="lowstock-row" href='<%= ResolveUrl("~/ProductDetails.aspx") %>?id=<%# Eval("ProductID") %>'>
                <div class="pic"><img src='<%# Eval("ImageURL") %>' alt='<%# Eval("Name") %>' /></div>
                <div class="info">
                  <span class="name"><%# Eval("Name") %></span>
                  <span class="variant"><%# Eval("Brand") %> &middot; R<%# Eval("Price", "{0:N2}") %></span>
                </div>
                <span class="stock-warn">Only <%# Eval("Quantity") %> left</span>
              </a>
            </ItemTemplate>
          </asp:Repeater>
        </div>
      </asp:Panel>

    </div>
  </section>

</asp:Content>

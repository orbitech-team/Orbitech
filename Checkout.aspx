<%@ Page Title="Checkout — OrbiTech" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Checkout.aspx.cs" Inherits="OrbitechWeb.Checkout" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <meta name="description" content="Secure OrbiTech checkout." />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

  <section class="page-head">
    <div class="container">
      <div class="crumbs"><a href='<%= ResolveUrl("~/Cart.aspx") %>'>Cart</a> <span class="sep">&rsaquo;</span> <span>Checkout</span></div>
      <h1>Checkout</h1>
      <p>Review your order and place it. You'll receive an invoice immediately.</p>
    </div>
  </section>

  <section class="section">
    <div class="container">

      <asp:Panel ID="CheckoutPanel" runat="server" Visible="false">
        <div class="cart-layout">
          <div>
            <div class="cart-list">
              <!-- Read-only summary of everything being bought -->
              <asp:Repeater ID="CheckoutRepeater" runat="server">
                <ItemTemplate>
                  <article class="cart-row">
                    <div class="pic"><img src='<%# Eval("ProductImage") %>' alt='<%# Eval("ProductName") %>' /></div>
                    <div class="info">
                      <div class="name"><%# Eval("ProductName") %></div>
                    </div>
                    <div class="qty" style="font-family:var(--ff-mono); font-size:var(--text-sm)">x <%# Eval("Quantity") %></div>
                    <span class="subtotal">R<%# Eval("LineTotal", "{0:N2}") %></span>
                  </article>
                </ItemTemplate>
              </asp:Repeater>
            </div>
          </div>

          <aside class="cart-summary">
            <h3>Order Summary</h3>

            <asp:Panel ID="ErrorPanel" runat="server" CssClass="alert alert-error" Visible="false">
              <asp:Literal ID="ErrorLiteral" runat="server" />
            </asp:Panel>

            <div class="cart-line"><span>Subtotal</span><span style="font-family:var(--ff-display); font-weight:600; color:var(--ink)">R<asp:Literal ID="SubtotalLiteral" runat="server" /></span></div>

            <asp:Panel ID="DiscountLinePanel" runat="server" Visible="false" CssClass="cart-line">
              <span>Student discount (15%)</span>
              <span style="color: var(--emerald); font-weight: 600">&minus;R<asp:Literal ID="DiscountLiteral" runat="server" /></span>
            </asp:Panel>

            <!-- Each transaction rule gets its own visible line -->
            <div class="cart-line"><span>Shipping (SA-wide)</span><span style="color: var(--emerald); font-weight: 600"><asp:Literal ID="ShippingLiteral" runat="server" /></span></div>
            <div class="cart-line"><span>VAT (15%)</span><span style="font-family:var(--ff-display); font-weight:600; color:var(--ink)">R<asp:Literal ID="VatLiteral" runat="server" /></span></div>
            <div class="cart-line is-total"><span>Total</span><span>R<asp:Literal ID="TotalLiteral" runat="server" /></span></div>

            <asp:Button ID="PlaceOrderBtn" runat="server" Text="Place Order &rarr;" CssClass="btn btn--indigo btn--block" OnClick="PlaceOrderBtn_Click" OnClientClick="return confirm('Confirm and place this order?');" />
            <p style="margin-top: var(--s5); font-size: 11px; font-family: var(--ff-mono); color: var(--fg-mute); text-align: center; line-height: 1.6">By placing this order you agree to OrbiTech's 14-day return policy and 12-month warranty terms.</p>
          </aside>
        </div>
      </asp:Panel>

      <!-- Success panel: replaces the whole form once the order is placed -->
      <asp:Panel ID="SuccessPanel" runat="server" Visible="false" style="text-align:center; padding: var(--s9) 0; max-width: 560px; margin: 0 auto;">
        <div style="width:64px;height:64px;background:var(--emerald);border-radius:999px;display:grid;place-items:center;margin:0 auto var(--s5);font-size:28px;color:#fff">&#10003;</div>
        <h2 style="font-size: var(--text-2xl); margin-bottom: var(--s4);">Order placed!</h2>
        <asp:Literal ID="SuccessLiteral" runat="server" />
        <div style="margin-top: var(--s6); display:flex; gap: var(--s3); justify-content:center; flex-wrap:wrap">
          <a href='<%= ResolveUrl("~/Shop.aspx") %>' class="btn btn--ghost">Continue shopping</a>
          <a href='<%= ResolveUrl("~/Default.aspx") %>' class="btn btn--indigo">Back to home</a>
        </div>
      </asp:Panel>

    </div>
  </section>

</asp:Content>

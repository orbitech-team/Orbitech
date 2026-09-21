<%@ Page Title="Cart — OrbiTech" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Cart.aspx.cs" Inherits="OrbitechWeb.Cart" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <meta name="description" content="Your OrbiTech shopping cart." />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

  <section class="page-head">
    <div class="container">
      <div class="crumbs"><a href='<%= ResolveUrl("~/Default.aspx") %>'>Home</a> <span class="sep">&rsaquo;</span> <span>Shopping cart</span></div>
      <h1>Your Cart</h1>
      <p>Review your tech selection. All devices are ICASA approved and come with a 12-month OrbiTech warranty.</p>
    </div>
  </section>

  <section class="section">
    <div class="container">

      <asp:Panel ID="LoginPromptPanel" runat="server" Visible="false" style="text-align:center; padding: var(--s9) 0;">
        <h2 style="font-size: var(--text-xl); margin-bottom: var(--s4);">Please log in to view your cart</h2>
        <p style="color: var(--fg-mute); margin-bottom: var(--s6);">Your cart is tied to your account so you can access it from any device.</p>
        <a href='<%= ResolveUrl("~/Login.aspx") %>' class="btn btn--indigo">Login / Register</a>
      </asp:Panel>

      <asp:Panel ID="EmptyCartPanel" runat="server" Visible="false" style="text-align:center; padding: var(--s9) 0;">
        <h2 style="font-size: var(--text-xl); margin-bottom: var(--s4);">Your cart is empty</h2>
        <p style="color: var(--fg-mute); margin-bottom: var(--s6);">Browse our catalog and add some tech to your cart.</p>
        <a href='<%= ResolveUrl("~/Shop.aspx") %>' class="btn btn--indigo">Browse Products</a>
      </asp:Panel>

      <asp:Panel ID="CartPanel" runat="server" Visible="false">
        <div class="cart-layout">
          <div>
            <div class="cart-list">
              <asp:Repeater ID="CartRepeater" runat="server" OnItemCommand="CartRepeater_ItemCommand">
                <ItemTemplate>
                  <article class="cart-row">
                    <div class="pic"><img src='<%# Eval("ProductImage") %>' alt='<%# Eval("ProductName") %>' /></div>
                    <div class="info">
                      <div class="name"><%# Eval("ProductName") %></div>
                      <div class="variant"><%# Eval("Brand") %> &middot; <%# Eval("Condition") %></div>
                    </div>
                    <div class="qty">
                      <asp:LinkButton ID="DecreaseBtn" runat="server" CommandName="Decrease" CommandArgument='<%# Eval("CartItemID") + "|" + Eval("Quantity") %>'>&minus;</asp:LinkButton>
                      <input type="text" value='<%# Eval("Quantity") %>' inputmode="numeric" aria-label="Quantity" readonly style="width:40px;text-align:center;border:1px solid var(--rule);border-radius:var(--r-sm)" />
                      <asp:LinkButton ID="IncreaseBtn" runat="server" CommandName="Increase" CommandArgument='<%# Eval("CartItemID") + "|" + Eval("Quantity") %>'>+</asp:LinkButton>
                    </div>
                    <span class="subtotal">R<%# Eval("LineTotal", "{0:N2}") %></span>
                    <asp:LinkButton ID="RemoveBtn" runat="server" CommandName="Remove" CommandArgument='<%# Eval("CartItemID") %>' CssClass="remove" aria-label="Remove" OnClientClick="return confirm('Remove this item from your cart?');">&#10005;</asp:LinkButton>
                  </article>
                </ItemTemplate>
              </asp:Repeater>
            </div>

            <div style="margin-top: var(--s5); display: flex; gap: var(--s3); flex-wrap: wrap">
              <a href='<%= ResolveUrl("~/Shop.aspx") %>' class="btn btn--ghost">&larr; Continue shopping</a>
            </div>
          </div>

          <aside class="cart-summary">
            <h3>Order Summary</h3>

            <!-- Promo code: now a real server control so the button can trigger C# -->
            <div class="promo-input">
              <asp:TextBox ID="PromoInput" runat="server" placeholder="Student ID / Promo" />
              <asp:Button ID="ApplyPromoBtn" runat="server" Text="Apply" OnClick="ApplyPromoBtn_Click" />
            </div>
            <asp:Panel ID="PromoMessagePanel" runat="server" Visible="false" style="margin-top: var(--s2);">
              <asp:Literal ID="PromoMessageLiteral" runat="server" />
            </asp:Panel>

            <div class="cart-line"><span>Subtotal &middot; <asp:Literal ID="ItemCountLiteral" runat="server" /> items</span><span style="font-family:var(--ff-display); font-weight:600; color:var(--ink)">R<asp:Literal ID="SubtotalLiteral" runat="server" /></span></div>

            <!-- Discount line: hidden completely unless a valid promo is applied -->
            <asp:Panel ID="DiscountLinePanel" runat="server" Visible="false" CssClass="cart-line">
              <span>Student discount (15%)</span>
              <span style="color: var(--emerald); font-weight: 600">&minus;R<asp:Literal ID="DiscountLiteral" runat="server" /></span>
            </asp:Panel>

            <div class="cart-line"><span>Shipping (SA-wide)</span><span style="color: var(--emerald); font-weight: 600"><asp:Literal ID="ShippingLiteral" runat="server" /></span></div>
            <div class="cart-line"><span>VAT (15%)</span><span style="font-family:var(--ff-display); font-weight:600; color:var(--ink)">R<asp:Literal ID="VatLiteral" runat="server" /></span></div>
            <div class="cart-line is-total"><span>Total</span><span>R<asp:Literal ID="TotalLiteral" runat="server" /></span></div>

            <asp:Button ID="CheckoutBtn" runat="server" Text="Proceed to Secure Checkout &rarr;" CssClass="btn btn--indigo btn--block" OnClick="CheckoutBtn_Click" />

            <div style="display: flex; justify-content: center; gap: var(--s3); margin-top: var(--s5); flex-wrap: wrap">
              <span style="font-family: var(--ff-mono); font-size: 11px; color: var(--fg-mute); padding: 6px 10px; background: var(--paper); border-radius: 4px">PayFast</span>
              <span style="font-family: var(--ff-mono); font-size: 11px; color: var(--fg-mute); padding: 6px 10px; background: var(--paper); border-radius: 4px">OZOW</span>
              <span style="font-family: var(--ff-mono); font-size: 11px; color: var(--fg-mute); padding: 6px 10px; background: var(--paper); border-radius: 4px">VISA</span>
              <span style="font-family: var(--ff-mono); font-size: 11px; color: var(--fg-mute); padding: 6px 10px; background: var(--paper); border-radius: 4px">MASTERCARD</span>
            </div>
            <p style="margin-top: var(--s5); font-size: 11px; font-family: var(--ff-mono); color: var(--fg-mute); text-align: center; line-height: 1.6">SSL Secured Checkout. Powered by local South African payment gateways.</p>
          </aside>
        </div>
      </asp:Panel>

    </div>
  </section>

</asp:Content>

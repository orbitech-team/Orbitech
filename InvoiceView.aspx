<%-- ============================================================
     InvoiceView.aspx — Order Invoice Page
     ============================================================
     Shows a single order's invoice: line items, subtotal, discount,
     shipping, VAT and total, matching the numbers first shown to the
     customer on Checkout.aspx. Reached via ?id=<OrderID>, matching
     IOrbitechService.GetOrderById(int orderId) — linked from a row
     in Profile.aspx's order history.

     PLACE THIS FILE IN: OrbitechWeb/InvoiceView.aspx
     ============================================================ --%>

<%@ Page Title="Invoice — OrbiTech" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="InvoiceView.aspx.cs" Inherits="OrbitechWeb.InvoiceView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <meta name="description" content="View and print your OrbiTech order invoice." />
  <style>
    .invoice-wrap {
      max-width: 760px;
      margin: 0 auto;
      background: var(--paper);
      border-radius: var(--r-lg);
      box-shadow: var(--shadow);
      padding: var(--s8);
    }
    .invoice-head {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      flex-wrap: wrap;
      gap: var(--s5);
      border-bottom: 1px solid var(--rule-strong);
      padding-bottom: var(--s6);
      margin-bottom: var(--s6);
    }
    .invoice-head .brand-mark { font-size: var(--text-xl); }
    .invoice-meta { text-align: right; font-size: var(--text-sm); color: var(--fg-mute); }
    .invoice-meta strong { display: block; color: var(--ink); font-size: var(--text-lg); font-family: var(--ff-mono); }
    .invoice-status {
      display: inline-block;
      margin-top: var(--s2);
      padding: 4px 12px;
      border-radius: 999px;
      font-size: 11px;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.04em;
      background: var(--primary-soft);
      color: var(--primary);
    }
    .invoice-parties {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: var(--s5);
      margin-bottom: var(--s7);
      font-size: var(--text-sm);
    }
    .invoice-parties h4 { font-size: var(--text-xs); text-transform: uppercase; letter-spacing: 0.05em; color: var(--fg-mute); margin-bottom: var(--s2); }
    .invoice-table { width: 100%; border-collapse: collapse; margin-bottom: var(--s6); }
    .invoice-table th {
      text-align: left;
      font-size: var(--text-xs);
      text-transform: uppercase;
      letter-spacing: 0.04em;
      color: var(--fg-mute);
      border-bottom: 1px solid var(--rule-strong);
      padding: var(--s3) var(--s2);
    }
    .invoice-table td { padding: var(--s3) var(--s2); border-bottom: 1px solid var(--rule); font-size: var(--text-sm); }
    .invoice-table .line-item { display: flex; align-items: center; gap: var(--s3); }
    .invoice-table .line-item img { width: 44px; height: 44px; object-fit: cover; border-radius: var(--r-sm); background: var(--bg); }
    .invoice-table td.num, .invoice-table th.num { text-align: right; font-family: var(--ff-mono); }
    .invoice-summary { max-width: 320px; margin-left: auto; font-size: var(--text-sm); }
    .invoice-summary .row { display: flex; justify-content: space-between; padding: var(--s2) 0; }
    .invoice-summary .row.is-total { border-top: 1px solid var(--rule-strong); margin-top: var(--s3); padding-top: var(--s4); font-size: var(--text-lg); font-weight: 700; color: var(--ink); }
    .invoice-actions { margin-top: var(--s7); display: flex; gap: var(--s3); justify-content: flex-end; flex-wrap: wrap; }
    @media print {
      .site-header, .nav-bar, footer, .invoice-actions { display: none !important; }
      .invoice-wrap { box-shadow: none; }
    }
  </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

  <section class="page-head">
    <div class="container">
      <div class="crumbs"><a href='<%= ResolveUrl("~/Profile.aspx") %>'>Profile</a> <span class="sep">&rsaquo;</span> <span>Invoice</span></div>
      <h1>Invoice</h1>
      <p>Your order confirmation and full billing breakdown.</p>
    </div>
  </section>

  <section class="section">
    <div class="container">

      <!-- Shown when the invoice number is missing, not found, or doesn't belong to this user -->
      <asp:Panel ID="NotFoundPanel" runat="server" Visible="false" style="max-width:560px; margin:0 auto; text-align:center; padding: var(--s9) 0;">
        <p style="font-size: var(--text-lg); color: var(--fg-mute);">
          <asp:Literal ID="NotFoundMessage" runat="server" Text="We couldn't find that invoice." />
        </p>
        <a href='<%= ResolveUrl("~/Profile.aspx") %>' class="btn btn--ghost" style="margin-top: var(--s5)">Back to profile</a>
      </asp:Panel>

      <asp:Panel ID="InvoiceCardPanel" runat="server" Visible="false">
        <div class="invoice-wrap">

          <div class="invoice-head">
            <div>
              <a href='<%= ResolveUrl("~/Default.aspx") %>' class="brand"><span class="brand-mark">O</span> OrbiTech</a>
              <p style="color: var(--fg-mute); font-size: var(--text-xs); margin-top: var(--s2);">Sandton City Office Tower, Johannesburg, 2196</p>
            </div>
            <div class="invoice-meta">
              Invoice
              <strong><asp:Literal ID="InvoiceNumberLiteral" runat="server" /></strong>
              <asp:Literal ID="InvoiceDateLiteral" runat="server" />
              <br />
              <span class="invoice-status"><asp:Literal ID="StatusLiteral" runat="server" /></span>
            </div>
          </div>

          <div class="invoice-parties">
            <div>
              <h4>Billed to</h4>
              <div><asp:Literal ID="CustomerNameLiteral" runat="server" /></div>
            </div>
            <div style="text-align:right">
              <h4>Payment</h4>
              <div>OrbiTech Secure Checkout</div>
            </div>
          </div>

          <table class="invoice-table">
            <thead>
              <tr>
                <th>Item</th>
                <th class="num">Qty</th>
                <th class="num">Unit price</th>
                <th class="num">Line total</th>
              </tr>
            </thead>
            <tbody>
              <asp:Repeater ID="InvoiceLinesRepeater" runat="server">
                <ItemTemplate>
                  <tr>
                    <td>
                      <div class="line-item">
                        <img src='<%# Eval("ProductImage") %>' alt='<%# Eval("ProductName") %>' />
                        <span><%# Eval("ProductName") %></span>
                      </div>
                    </td>
                    <td class="num"><%# Eval("Quantity") %></td>
                    <td class="num">R<%# Eval("UnitPrice", "{0:N2}") %></td>
                    <td class="num">R<%# Eval("LineTotal", "{0:N2}") %></td>
                  </tr>
                </ItemTemplate>
              </asp:Repeater>
            </tbody>
          </table>

          <div class="invoice-summary">
            <div class="row"><span>Subtotal</span><span>R<asp:Literal ID="SubtotalLiteral" runat="server" /></span></div>

            <asp:Panel ID="DiscountRowPanel" runat="server" Visible="false" CssClass="row">
              <span>Student discount (15%)</span>
              <span style="color: var(--emerald)">&minus;R<asp:Literal ID="DiscountLiteral" runat="server" /></span>
            </asp:Panel>

            <div class="row"><span>Shipping</span><span><asp:Literal ID="ShippingLiteral" runat="server" /></span></div>
            <div class="row"><span>VAT (15%)</span><span>R<asp:Literal ID="VatLiteral" runat="server" /></span></div>
            <div class="row is-total"><span>Total</span><span>R<asp:Literal ID="TotalLiteral" runat="server" /></span></div>
          </div>

          <div class="invoice-actions">
            <a href='<%= ResolveUrl("~/Profile.aspx") %>' class="btn btn--ghost">Back to profile</a>
            <asp:Button ID="PrintBtn" runat="server" Text="Print invoice" CssClass="btn btn--indigo" OnClientClick="window.print(); return false;" />
          </div>

        </div>
      </asp:Panel>

    </div>
  </section>

</asp:Content>

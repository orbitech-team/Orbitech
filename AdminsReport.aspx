<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdminsReport.aspx.cs" Inherits="OrbitechWeb.AdminsReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta name="description" content="OrbiTech Admin — Sales, satisfaction and complaint reports." />
  <style>
    .admin-head { padding: var(--s7) 0; background: var(--ink); color: var(--paper); }
    .admin-head h1 { color: var(--paper); font-size: var(--text-2xl); }
    .admin-head p { opacity: 0.7; font-size: var(--text-sm); margin-top: var(--s2); }

    .report-card { background: var(--paper); border: 1px solid var(--rule); border-radius: var(--r-lg);
                   padding: var(--s7); box-shadow: var(--shadow-sm); margin-bottom: var(--s7); }
    .report-card h2 { font-size: var(--text-lg); margin-bottom: var(--s2); }
    .report-card .sub { color: var(--fg-mute); font-size: var(--text-sm); margin-bottom: var(--s5); }

    .report-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)); gap: var(--s4); margin-bottom: var(--s6); }
    .report-tile { background: var(--bg); border-radius: var(--r); padding: var(--s5); text-align: center; }
    .report-tile .big { font-family: var(--ff-display); font-size: var(--text-xl); font-weight: 700; color: var(--ink); }
    .report-tile .lbl { font-family: var(--ff-mono); font-size: 10px; text-transform: uppercase; letter-spacing: 0.08em; color: var(--fg-mute); margin-top: var(--s2); }

    .report-table { width: 100%; border-collapse: collapse; font-size: var(--text-sm); background: var(--paper); border-radius: var(--r); overflow: hidden; }
    .report-table th { background: var(--bg); padding: var(--s3) var(--s4); text-align: left; font-family: var(--ff-display);
                       font-weight: 700; font-size: var(--text-xs); text-transform: uppercase; letter-spacing: 0.05em; color: var(--ink-mute); }
    .report-table td { padding: var(--s3) var(--s4); border-top: 1px solid var(--rule); vertical-align: middle; }
    .report-table tr:hover td { background: var(--bg-soft); }

    .dist-row { display: grid; grid-template-columns: 46px 40px 1fr 46px; align-items: center; gap: var(--s3); margin-bottom: var(--s2); font-size: var(--text-sm); }
    .dist-bar { background: var(--bg); border-radius: 999px; height: 12px; overflow: hidden; }
    .dist-bar > div { background: var(--primary); height: 100%; border-radius: 999px; }

    .badge-complaint { background: #fef2f2; color: #dc2626; padding: 4px 12px; border-radius: 999px;
                       font-size: var(--text-xs); font-weight: 600; border: 1px solid #fecaca; white-space: nowrap; }

    .range-pick { display: flex; justify-content: space-between; align-items: center; gap: var(--s4);
                  flex-wrap: wrap; margin-bottom: var(--s6); }
    .range-pick label { font-size: var(--text-sm); font-weight: 600; color: var(--ink-soft); }
    .range-pick select { padding: 8px 12px; border: 1px solid var(--rule-strong); border-radius: var(--r-sm);
                         font-family: var(--ff-body); background: var(--paper); font-size: var(--text-sm); }
    .alert { padding: var(--s4) var(--s5); border-radius: var(--r-sm); margin-bottom: var(--s5); font-size: var(--text-sm); }
    .alert-error { background: #fef2f2; color: #dc2626; border: 1px solid #fecaca; }
    .stars { color: #f59e0b; }
  </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="admin-head">
    <div class="container">
      <h1>Reports</h1>
      <p>Sales, customer satisfaction and customer complaints.</p>
    </div>
  </div>

  <section class="section">
    <div class="container">

      <asp:Panel ID="ErrorPanel" runat="server" CssClass="alert alert-error" Visible="false">
        <asp:Literal ID="ErrorLiteral" runat="server" />
      </asp:Panel>

      <!-- Date range filter (applies to the SALES report) -->
      <div class="range-pick">
        <label for="<%= DdlRange.ClientID %>">Sales period:</label>
        <div style="display:flex; gap:var(--s3); align-items:center;">
          <asp:DropDownList ID="DdlRange" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DdlRange_SelectedIndexChanged">
            <asp:ListItem Text="Last 7 days" Value="7" />
            <asp:ListItem Text="Last 30 days" Value="30" Selected="True" />
            <asp:ListItem Text="Last 90 days" Value="90" />
            <asp:ListItem Text="Last 12 months" Value="365" />
          </asp:DropDownList>
          <a class="btn btn--ghost" href='<%= ResolveUrl("~/AdminProducts.aspx") %>'>Back to products</a>
        </div>
      </div>

      <!-- ================================================================
           REPORT 1: SALES
           ================================================================ -->
      <div class="report-card">
        <h2>Sales Report</h2>
        <p class="sub">Revenue and orders for the selected period, with a monthly breakdown and the best-selling products.</p>

        <div class="report-grid">
          <div class="report-tile">
            <div class="big">R<asp:Literal ID="LitRevenue" runat="server" Text="0.00" /></div>
            <div class="lbl">Total revenue</div>
          </div>
          <div class="report-tile">
            <div class="big"><asp:Literal ID="LitOrderCount" runat="server" Text="0" /></div>
            <div class="lbl">Orders</div>
          </div>
          <div class="report-tile">
            <div class="big">R<asp:Literal ID="LitAvgOrder" runat="server" Text="0.00" /></div>
            <div class="lbl">Average order value</div>
          </div>
        </div>

        <div style="display:grid; grid-template-columns: 1fr 1fr; gap: var(--s6); align-items:start;">
          <div>
            <h3 style="font-size: var(--text-sm); text-transform:uppercase; letter-spacing:.05em; color: var(--ink-mute); margin-bottom: var(--s3);">Monthly sales</h3>
            <asp:Repeater ID="MonthlyRepeater" runat="server">
              <HeaderTemplate>
                <table class="report-table">
                  <thead><tr><th>Month</th><th>Orders</th><th>Revenue</th></tr></thead>
                  <tbody>
              </HeaderTemplate>
              <ItemTemplate>
                <tr>
                  <td style="font-family: var(--ff-mono); font-size: 12px;"><%# Eval("MonthLabel") %></td>
                  <td><%# Eval("OrderCount") %></td>
                  <td style="font-weight:600">R<%# Eval("Revenue", "{0:N2}") %></td>
                </tr>
              </ItemTemplate>
              <FooterTemplate>
                  </tbody>
                </table>
              </FooterTemplate>
            </asp:Repeater>
          </div>

          <div>
            <h3 style="font-size: var(--text-sm); text-transform:uppercase; letter-spacing:.05em; color: var(--ink-mute); margin-bottom: var(--s3);">Top products (by revenue)</h3>
            <asp:Repeater ID="TopProductsRepeater" runat="server">
              <HeaderTemplate>
                <table class="report-table">
                  <thead><tr><th>Product</th><th>Units</th><th>Revenue</th></tr></thead>
                  <tbody>
              </HeaderTemplate>
              <ItemTemplate>
                <tr>
                  <td style="font-weight:600"><%# Eval("ProductName") %></td>
                  <td><%# Eval("QuantitySold") %></td>
                  <td style="font-weight:600">R<%# Eval("Revenue", "{0:N2}") %></td>
                </tr>
              </ItemTemplate>
              <FooterTemplate>
                  </tbody>
                </table>
              </FooterTemplate>
            </asp:Repeater>
          </div>
        </div>
      </div>

      <!-- ================================================================
           REPORT 2: CUSTOMER SATISFACTION
           ================================================================ -->
      <div class="report-card">
        <h2>Customer Satisfaction Report</h2>
        <p class="sub">Ratings submitted by customers after receiving their products (Feedback.aspx).</p>

        <div class="report-grid">
          <div class="report-tile">
            <div class="big"><asp:Literal ID="LitSatDelivery" runat="server" Text="—" /> <span class="stars">&#9733;</span></div>
            <div class="lbl">Avg. delivery efficiency (of 5)</div>
          </div>
          <div class="report-tile">
            <div class="big"><asp:Literal ID="LitSatProduct" runat="server" Text="—" /> <span class="stars">&#9733;</span></div>
            <div class="lbl">Avg. product satisfaction (of 5)</div>
          </div>
          <div class="report-tile">
            <div class="big"><asp:Literal ID="LitSatPercent" runat="server" Text="—" /></div>
            <div class="lbl">Satisfied customers (4-5 stars)</div>
          </div>
          <div class="report-tile">
            <div class="big"><asp:Literal ID="LitSatResponses" runat="server" Text="0" /></div>
            <div class="lbl">Total feedback responses</div>
          </div>
        </div>

        <h3 style="font-size: var(--text-sm); text-transform:uppercase; letter-spacing:.05em; color: var(--ink-mute); margin-bottom: var(--s3);">Satisfaction distribution</h3>
        <asp:Repeater ID="DistributionRepeater" runat="server">
          <ItemTemplate>
            <div class="dist-row">
              <span class="stars"><%# Eval("Stars") %>&#9733;</span>
              <span style="font-family: var(--ff-mono); font-size: 12px;"><%# Eval("Count") %></span>
              <div class="dist-bar"><div style='width:<%# Eval("Percent") %>%'></div></div>
              <span style="font-family: var(--ff-mono); font-size: 12px; text-align:right;"><%# Eval("Percent") %>%</span>
            </div>
          </ItemTemplate>
        </asp:Repeater>
      </div>

      <!-- ================================================================
           REPORT 3: CUSTOMER COMPLAINTS
           ================================================================ -->
      <div class="report-card">
        <h2>Customer Complaints</h2>
        <p class="sub">All feedback logged as a complaint by customers. Newest first.</p>

        <asp:Repeater ID="ComplaintsRepeater" runat="server">
          <HeaderTemplate>
            <table class="report-table">
              <thead>
                <tr>
                  <th>Complaint #</th>
                  <th>Order</th>
                  <th>Category</th>
                  <th>Comment</th>
                  <th>Ratings</th>
                  <th>Date</th>
                </tr>
              </thead>
              <tbody>
          </HeaderTemplate>
          <ItemTemplate>
            <tr>
              <td style="font-family: var(--ff-mono); font-size: 12px;">#<%# Eval("FeedbackID") %></td>
              <td><%# Eval("OrderID") %></td>
              <td><span class="badge-complaint"><%# Eval("ComplaintCategory") %></span></td>
              <td><%# Eval("Comments") %></td>
              <td class="stars" style="white-space:nowrap">D: <%# Eval("DeliveryRating") %>&#9733; &middot; P: <%# Eval("SatisfactionRating") %>&#9733;</td>
              <td style="font-family: var(--ff-mono); font-size: 12px;"><%# Eval("FeedbackDate") %></td>
            </tr>
          </ItemTemplate>
          <FooterTemplate>
              </tbody>
            </table>
          </FooterTemplate>
        </asp:Repeater>

        <asp:Panel ID="NoComplaintsPanel" runat="server" Visible="false"
                   style="color: var(--fg-mute); font-size: var(--text-sm); padding: var(--s5) 0; text-align:center;">
          No complaints logged — excellent!
        </asp:Panel>
      </div>

    </div>
  </section>
</asp:Content>

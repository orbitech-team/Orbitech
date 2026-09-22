<%@ Page Title="Feedback" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Feedback.aspx.cs" Inherits="OrbitechWeb.Feedback" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta name="description" content="OrbiTech delivery performance and customer satisfaction reports." />
  <style>
    /* TUTORIAL STEP: Report tiles + rating form styles */
    .report-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: var(--s4); margin: var(--s6) 0; }
    .report-tile { background: var(--paper); border: 1px solid var(--rule); border-radius: var(--r-lg); padding: var(--s6); text-align: center; box-shadow: var(--shadow-sm); }
    .report-tile .big { font-family: var(--ff-display); font-size: var(--text-2xl); font-weight: 700; color: var(--ink); }
    .report-tile .lbl { font-family: var(--ff-mono); font-size: 11px; text-transform: uppercase; letter-spacing: 0.08em; color: var(--fg-mute); margin-top: var(--s2); }
    .report-tile .stars { color: #f59e0b; letter-spacing: 2px; }

    .feedback-layout { display: grid; grid-template-columns: 1fr 1fr; gap: var(--s7); align-items: start; }
    @media (max-width: 860px) { .feedback-layout { grid-template-columns: 1fr; } }

    .fcard { background: var(--paper); border: 1px solid var(--rule); border-radius: var(--r-lg); padding: var(--s7); box-shadow: var(--shadow-sm); }
    .fcard h3 { font-size: var(--text-lg); margin-bottom: var(--s5); }
    .ffield { margin-bottom: var(--s5); }
    .ffield > span.fl { display: block; font-size: var(--text-sm); font-weight: 600; margin-bottom: var(--s2); color: var(--ink-soft); }
    .ffield input[type=text], .ffield textarea {
      width: 100%; padding: 10px 14px; border: 1px solid var(--rule-strong);
      border-radius: var(--r-sm); font-size: var(--text-sm); font-family: var(--ff-body); background: var(--bg);
    }
    .ffield textarea { resize: vertical; min-height: 90px; }
    .rblstars { display: flex; gap: var(--s3); }
    .rblstars label { cursor: pointer; }
    .rblstars input[type=radio] { margin-right: 6px; }
    .hint { font-size: 11px; color: var(--fg-mute); font-family: var(--ff-mono); }
    .val-msg { color: #dc2626; font-size: var(--text-xs); display: block; margin-top: 4px; }
    .alert { padding: var(--s4) var(--s5); border-radius: var(--r-sm); margin-bottom: var(--s5); font-size: var(--text-sm); }
    .alert-error { background: #fef2f2; color: #dc2626; border: 1px solid #fecaca; }
    .alert-success { background: #f0fdf4; color: #16a34a; border: 1px solid #bbf7d0; }

    .review-list { display: flex; flex-direction: column; gap: var(--s4); }
    .review { background: var(--paper); border: 1px solid var(--rule); border-radius: var(--r); padding: var(--s5); }
    .review .meta { font-family: var(--ff-mono); font-size: 11px; color: var(--fg-mute); display: flex; justify-content: space-between; margin-bottom: var(--s2); }
    .review .msg { font-size: var(--text-sm); color: var(--ink-soft); }
    .review .stars { color: #f59e0b; }
  </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     <section class="page-head">
    <div class="container">
      <div class="crumbs"><a href='<%= ResolveUrl("~/Default.aspx") %>'>Home</a> <span class="sep">&rsaquo;</span> <span>Delivery &amp; satisfaction</span></div>
      <h1>Delivery &amp; Satisfaction Report</h1>
      <p>See how efficiently OrbiTech delivers, and how satisfied customers are after receiving their products. Ratings come straight from customers who ordered.</p>
    </div>
  </section>

  <section class="section">
    <div class="container">

      <!-- ================================================================
           PART 1: DISPLAY — delivery efficiency + customer satisfaction
           TUTORIAL STEP: These tiles are filled by GetFeedbackSummary()
           in the code-behind (Page_Load).
           ================================================================ -->
      <div class="report-grid">
        <div class="report-tile">
          <div class="big"><asp:Literal ID="LitAvgDeliveryDays" runat="server" Text="—" /></div>
          <div class="lbl">Avg. delivery time (business days)</div>
        </div>
        <div class="report-tile">
          <div class="big"><asp:Literal ID="LitAvgDeliveryRating" runat="server" Text="—" /></div>
          <div class="lbl">Delivery efficiency rating (of 5)</div>
        </div>
        <div class="report-tile">
          <div class="big"><asp:Literal ID="LitAvgSatisfaction" runat="server" Text="—" /></div>
          <div class="lbl">Product satisfaction rating (of 5)</div>
        </div>
        <div class="report-tile">
          <div class="big"><asp:Literal ID="LitPctSatisfied" runat="server" Text="—" /></div>
          <div class="lbl">Customers satisfied (rated 4 or 5)</div>
        </div>
      </div>

      <div class="feedback-layout">

        <!-- ================================================================
             PART 2: SUBMIT — the customer's own rating for an order
             TUTORIAL STEP: Submit button calls SubmitFeedback()
             via the WCF service. RadioButtonLists collect 1-5 ratings.
             ================================================================ -->
        <div class="fcard">
          <h3>Rate your delivery</h3>

          <asp:Panel ID="StatusPanel" runat="server" CssClass="alert" Visible="false">
            <asp:Literal ID="StatusLiteral" runat="server" />
          </asp:Panel>

          <div class="ffield">
            <span class="fl">Order number *</span>
            <asp:TextBox ID="TxtOrderID" runat="server" placeholder="e.g., 5 (from your confirmation email)" MaxLength="9" />
            <asp:RequiredFieldValidator ID="RfvOrderID" runat="server" ControlToValidate="TxtOrderID"
              ErrorMessage="Please enter your order number." CssClass="val-msg" Display="Dynamic" />
            <asp:CompareValidator ID="CvOrderID" runat="server" ControlToValidate="TxtOrderID" Operator="DataTypeCheck"
              Type="Integer" ErrorMessage="Order number must be a number." CssClass="val-msg" Display="Dynamic" />
          </div>

          <div class="ffield">
            <span class="fl">How efficient was the delivery? *</span>
            <asp:RadioButtonList ID="RblDelivery" runat="server" RepeatLayout="Flow" CssClass="rblstars">
              <asp:ListItem Value="1" Text="1 &#9733;" />
              <asp:ListItem Value="2" Text="2 &#9733;" />
              <asp:ListItem Value="3" Text="3 &#9733;" />
              <asp:ListItem Value="4" Text="4 &#9733;" />
              <asp:ListItem Value="5" Text="5 &#9733;" Selected="True" />
            </asp:RadioButtonList>
            <span class="hint">1 = very slow &middot; 5 = arrived fast &amp; intact</span>
          </div>

          <div class="ffield">
            <span class="fl">How satisfied are you with the product? *</span>
            <asp:RadioButtonList ID="RblSatisfaction" runat="server" RepeatLayout="Flow" CssClass="rblstars">
              <asp:ListItem Value="1" Text="1 &#9733;" />
              <asp:ListItem Value="2" Text="2 &#9733;" />
              <asp:ListItem Value="3" Text="3 &#9733;" />
              <asp:ListItem Value="4" Text="4 &#9733;" />
              <asp:ListItem Value="5" Text="5 &#9733;" Selected="True" />
            </asp:RadioButtonList>
            <span class="hint">1 = very dissatisfied &middot; 5 = very satisfied</span>
          </div>

          <div class="ffield">
            <span class="fl">Comments</span>
            <asp:TextBox ID="TxtComments" runat="server" TextMode="MultiLine" placeholder="Tell us how the delivery and product worked out for you..." />
          </div>

          <div class="ffield">
            <asp:CheckBox ID="ChkComplaint" runat="server" Text="Log this as a complaint" />
          </div>

          <div class="ffield">
            <span class="fl">Complaint category (only if logging a complaint)</span>
            <asp:DropDownList ID="DdlComplaintCategory" runat="server">
              <asp:ListItem Text="— select (if complaining) —" Value="" />
              <asp:ListItem Text="Damaged on arrival" Value="Damaged on arrival" />
              <asp:ListItem Text="Late delivery" Value="Late delivery" />
              <asp:ListItem Text="Wrong item received" Value="Wrong item received" />
              <asp:ListItem Text="Poor communication" Value="Poor communication" />
              <asp:ListItem Text="Other" Value="Other" />
            </asp:DropDownList>
          </div>

          <asp:Button ID="BtnSubmitFeedback" runat="server" Text="Submit Feedback" CssClass="btn btn--indigo" OnClick="BtnSubmitFeedback_Click" />
        </div>

        <!-- ================================================================
             PART 3: Recent customer comments
             TUTORIAL STEP: Bound to GetRecentFeedback(5) in the code-behind.
             ================================================================ -->
        <div class="fcard">
          <h3>What customers are saying</h3>
          <div class="review-list">
            <asp:Repeater ID="RecentFeedbackRepeater" runat="server">
              <ItemTemplate>
                <div class="review">
                  <div class="meta">
                    <span>Order #<%# Eval("OrderID") %> &middot; <%# Eval("FeedbackDate") %></span>
                    <span class="stars">
                      Delivery <%# Eval("DeliveryRating") %>&#9733; &middot; Product <%# Eval("SatisfactionRating") %>&#9733;
                    </span>
                  </div>
                  <div class="msg"><%# Eval("Comments") %></div>
                </div>
              </ItemTemplate>
            </asp:Repeater>
            <asp:Panel ID="NoReviewsPanel" runat="server" Visible="false" style="color: var(--fg-mute); font-size: var(--text-sm);">
              No customer comments yet — be the first to leave feedback!
            </asp:Panel>
          </div>
        </div>

      </div>
    </div>
  </section>
</asp:Content>

<%-- ============================================================
     Contact.aspx — Contact Page (converted from contact.html)
     ============================================================
     TUTORIAL STEP: This page replaces contact.html. The key change is
     the contact form now uses ASP.NET server controls and the code-behind
     calls the WCF service to save the message to the CONTACT table.

     PLACE THIS FILE IN: OrbitechWeb/Contact.aspx
     ============================================================ --%>

<%@ Page Title="Contact Us — OrbiTech" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="OrbitechWeb.Contact" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <meta name="description" content="Get in touch with the OrbiTech team for device inquiries, trade-in valuations, or student portal verification." />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

  <section class="page-head">
    <div class="container">
      <div class="crumbs"><a href='<%= ResolveUrl("~/Default.aspx") %>'>Home</a> <span class="sep">&rsaquo;</span> <span>Contact &amp; support</span></div>
      <h1>Talk to the OrbiTech Team.</h1>
      <p>Whether you're looking for a trade-in value, need help with your order, or want to verify your student status, we're here to help South African tech lovers.</p>
    </div>
  </section>

  <section>
    <div class="container">
      <div class="contact-grid">

        <!-- ==================== CONTACT INFO ==================== -->
        <div class="contact-info">
          <div class="info-block"><div class="ic">&#9993;</div><div><div class="label">EMAIL US</div><div class="value"><a href="mailto:support@orbitech.co.za">support@orbitech.co.za</a></div></div></div>
          <div class="info-block"><div class="ic">&#9742;</div><div><div class="label">CALL US</div><div class="value"><a href="tel:+27115550123">+27 (11) 555-0123</a></div></div></div>
          <div class="info-block"><div class="ic">&#128172;</div><div><div class="label">LIVE CHAT</div><div class="value"><a href="#">Start Chat with Support</a></div></div></div>
          <div class="info-block"><div class="ic">&#11618;</div><div><div class="label">VISIT OUR LAB</div><div class="value">Sandton City Office Tower<br />Johannesburg, 2196</div></div></div>
          <div class="info-block"><div class="ic">&#9201;</div><div><div class="label">SUPPORT HOURS</div><div class="value">Mon &mdash; Fri &middot; 08:00 &mdash; 18:00 SAST<br />Sat &middot; 09:00 &mdash; 13:00 SAST</div></div></div>

          <div style="margin-top: var(--s7); padding: var(--s6); background: linear-gradient(135deg, var(--primary), var(--accent)); color: var(--paper); border-radius: var(--r-lg); position: relative; overflow: hidden">
            <div style="position: absolute; inset: 0; background-image: radial-gradient(circle at 80% 20%, rgba(255,255,255,0.18) 0, transparent 40%); pointer-events: none"></div>
            <div style="position: relative">
              <h3 style="color: var(--paper); font-size: var(--text-xl); margin-bottom: var(--s3)">Bulk Student Orders?</h3>
              <p style="color: rgba(255,255,255,0.85); font-size: var(--text-sm); line-height: 1.6; margin-bottom: var(--s4)">University departments or student organizations ordering 10+ units qualify for special procurement rates.</p>
              <a href="mailto:bulk@orbitech.co.za" class="btn btn--paper">Contact Procurement &rarr;</a>
            </div>
          </div>
        </div>

        <!-- ==================== CONTACT FORM ==================== -->
        <!-- TUTORIAL STEP: The form now uses ASP.NET server controls.
             When submitted, the code-behind calls the WCF service to
             save the message to the CONTACT table in the database. -->
        <div class="contact-form">
          <h2 style="font-size: var(--text-xl); margin-bottom: var(--s2)">Send us a message</h2>
          <p style="color: var(--fg-soft); font-size: var(--text-sm); margin-bottom: var(--s5)">Our local support team in Johannesburg will get back to you within 4 business hours.</p>

          <!-- Success/Error message -->
          <asp:Panel ID="StatusPanel" runat="server" Visible="false" style="padding: var(--s4) var(--s5); border-radius: var(--r-sm); margin-bottom: var(--s5); font-size: var(--text-sm);">
            <asp:Literal ID="StatusLiteral" runat="server" />
          </asp:Panel>

          <div class="field-row">
            <div class="field"><label for="CName">Name</label><asp:TextBox ID="CName" runat="server" placeholder="Thabo" /></div>
            <div class="field"><label for="CEmail">Email</label><asp:TextBox ID="CEmail" runat="server" TextMode="Email" placeholder="thabo@example.co.za" /></div>
          </div>

          <div class="field">
            <label for="CSubject">What is this about?</label>
            <asp:DropDownList ID="CSubject" runat="server">
              <asp:ListItem Text="Trade-In Valuation Inquiry" />
              <asp:ListItem Text="Student Portal Verification" />
              <asp:ListItem Text="Order Tracking & Support" />
              <asp:ListItem Text="Warranty Claim" />
              <asp:ListItem Text="Something else" />
            </asp:DropDownList>
          </div>

          <div class="field">
            <label for="CMessage">Your message</label>
            <asp:TextBox ID="CMessage" runat="server" TextMode="MultiLine" placeholder="Tell us how we can assist you today..." />
          </div>

          <asp:Button ID="SubmitBtn" runat="server" Text="Send message &rarr;" CssClass="btn btn--indigo btn--block" OnClick="SubmitBtn_Click" style="padding: 16px; font-size: var(--text-base); margin-top: var(--s2)" />
        </div>

      </div>
    </div>
  </section>

</asp:Content>

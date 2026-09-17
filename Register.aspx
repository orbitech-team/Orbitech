<%-- ============================================================
     Register.aspx — New User Registration Page
     ============================================================
     TUTORIAL STEP: This is a NEW page. It provides a registration form
     where new customers can create an account. The code-behind calls
     the WCF service's RegisterUser method.

     PLACE THIS FILE IN: OrbitechWeb/Register.aspx
     ============================================================ --%>

<%@ Page Title="Register — OrbiTech" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="OrbitechWeb.Register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <meta name="description" content="Create your OrbiTech account." />
  <style>
    .auth-wrap {
      max-width: 460px;
      margin: var(--s9) auto;
      padding: var(--s8);
      background: var(--paper);
      border-radius: var(--r-lg);
      box-shadow: var(--shadow);
    }
    .auth-wrap h1 {
      font-size: var(--text-2xl);
      text-align: center;
      margin-bottom: var(--s2);
    }
    .auth-wrap .subtitle {
      text-align: center;
      color: var(--fg-mute);
      font-size: var(--text-sm);
      margin-bottom: var(--s7);
    }
    .auth-field { margin-bottom: var(--s5); }
    .auth-field label {
      display: block;
      font-size: var(--text-sm);
      font-weight: 600;
      margin-bottom: var(--s2);
      color: var(--ink-soft);
    }
    .auth-field input {
      width: 100%;
      padding: 14px 18px;
      border: 1px solid var(--rule-strong);
      border-radius: var(--r-sm);
      font-size: var(--text-base);
      font-family: var(--ff-body);
      background: var(--bg);
    }
    .auth-field input:focus {
      outline: none;
      border-color: var(--primary);
      box-shadow: 0 0 0 3px var(--primary-soft);
    }
    .auth-btn {
      width: 100%;
      padding: 16px;
      font-size: var(--text-base);
      margin-top: var(--s4);
    }
    .auth-footer {
      text-align: center;
      margin-top: var(--s6);
      font-size: var(--text-sm);
      color: var(--fg-mute);
    }
    .auth-footer a { color: var(--primary); font-weight: 600; }
    .alert {
      padding: var(--s4) var(--s5);
      border-radius: var(--r-sm);
      margin-bottom: var(--s5);
      font-size: var(--text-sm);
    }
    .alert-error { background: #fef2f2; color: #dc2626; border: 1px solid #fecaca; }
    .alert-success { background: #f0fdf4; color: #16a34a; border: 1px solid #bbf7d0; }
  </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

  <div class="auth-wrap">
    <h1>Create Account</h1>
    <p class="subtitle">Join OrbiTech and start shopping for premium tech</p>

    <!-- Error message panel -->
    <asp:Panel ID="ErrorPanel" runat="server" CssClass="alert alert-error" Visible="false">
      <asp:Literal ID="ErrorLiteral" runat="server" />
    </asp:Panel>

    <div class="auth-field">
      <label for="UsernameInput">Username</label>
      <asp:TextBox ID="UsernameInput" runat="server" placeholder="Choose a username (max 25 chars)" MaxLength="25" />
    </div>

    <div class="auth-field">
      <label for="EmailInput">Email Address</label>
      <asp:TextBox ID="EmailInput" runat="server" placeholder="you@example.co.za" TextMode="Email" />
    </div>

    <div class="auth-field">
      <label for="PasswordInput">Password</label>
      <asp:TextBox ID="PasswordInput" runat="server" TextMode="Password" placeholder="Choose a password (min 6 chars)" />
    </div>

    <div class="auth-field">
      <label for="ConfirmPasswordInput">Confirm Password</label>
      <asp:TextBox ID="ConfirmPasswordInput" runat="server" TextMode="Password" placeholder="Re-enter your password" />
    </div>

    <asp:Button ID="RegisterBtn" runat="server" Text="Create Account &rarr;" CssClass="btn btn--indigo auth-btn" OnClick="RegisterBtn_Click" />

    <div class="auth-footer">
      Already have an account? <a href='<%= ResolveUrl("~/Login.aspx") %>'>Login here</a>
    </div>
  </div>

</asp:Content>

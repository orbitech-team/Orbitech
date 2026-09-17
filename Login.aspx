<%@ Page Title="Login — OrbiTech" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="OrbitechWeb.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <meta name="description" content="Login to your OrbiTech account." />
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
    <h1>Welcome Back</h1>
    <p class="subtitle">Sign in to your OrbiTech account</p>

    <asp:Panel ID="SuccessPanel" runat="server" CssClass="alert alert-success" Visible="false">
    </asp:Panel>

    <asp:Panel ID="ErrorPanel" runat="server" CssClass="alert alert-error" Visible="false">
      <asp:Literal ID="ErrorLiteral" runat="server" />
    </asp:Panel>

    <div class="auth-field">
      <label for="UsernameInput">Username</label>
      <asp:TextBox ID="UsernameInput" runat="server" placeholder="Enter your username" />
    </div>

    <div class="auth-field">
      <label for="PasswordInput">Password</label>
      <asp:TextBox ID="PasswordInput" runat="server" TextMode="Password" placeholder="Enter your password" />
    </div>

    <asp:Button ID="LoginBtn" runat="server" Text="Sign In &rarr;" CssClass="btn btn--indigo auth-btn" OnClick="LoginBtn_Click" />

    <div class="auth-footer">
      Don't have an account? <a href='<%= ResolveUrl("~/Register.aspx") %>'>Register here</a>
    </div>
  </div>

</asp:Content>

<%-- ============================================================
     AdminProducts.aspx — Admin Product Management Page
     ============================================================
     TUTORIAL STEP: This is a NEW page. It's the admin panel where an
     admin user can:
       - View all products in a table
       - Add a new product
       - Edit an existing product
       - Delete a product

     This page requires admin login (checked in code-behind).
     Non-admin users are redirected to the login page.

     PLACE THIS FILE IN: OrbitechWeb/AdminProducts.aspx
     ============================================================ --%>

<%@ Page Title="Admin — Product Management" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdminProducts.aspx.cs" Inherits="OrbitechWeb.AdminProducts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <meta name="description" content="OrbiTech Admin — Manage products." />
  <style>
    /* TUTORIAL STEP: Admin-specific styles for the management table and form */
    .admin-head {
      padding: var(--s7) 0;
      background: var(--ink);
      color: var(--paper);
    }
    .admin-head h1 { color: var(--paper); font-size: var(--text-2xl); }
    .admin-head p { opacity: 0.7; font-size: var(--text-sm); margin-top: var(--s2); }
    .admin-actions {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin: var(--s6) 0 var(--s5);
      flex-wrap: wrap;
      gap: var(--s4);
    }
    .admin-actions .count {
      font-family: var(--ff-mono);
      font-size: var(--text-sm);
      color: var(--fg-mute);
    }
    .product-table {
      width: 100%;
      border-collapse: collapse;
      font-size: var(--text-sm);
      background: var(--paper);
      border-radius: var(--r);
      overflow: hidden;
      box-shadow: var(--shadow-sm);
    }
    .product-table th {
      background: var(--bg);
      padding: var(--s4) var(--s5);
      text-align: left;
      font-family: var(--ff-display);
      font-weight: 700;
      font-size: var(--text-xs);
      text-transform: uppercase;
      letter-spacing: 0.05em;
      color: var(--ink-mute);
    }
    .product-table td {
      padding: var(--s4) var(--s5);
      border-top: 1px solid var(--rule);
      vertical-align: middle;
    }
    .product-table tr:hover td { background: var(--bg-soft); }
    .product-table img {
      width: 50px;
      height: 50px;
      object-fit: cover;
      border-radius: var(--r-sm);
    }
    .product-table .del-btn {
      background: #fef2f2;
      color: #dc2626;
      padding: 6px 14px;
      border-radius: 999px;
      font-size: var(--text-xs);
      font-weight: 600;
      border: 1px solid #fecaca;
      cursor: pointer;
    }
    .product-table .del-btn:hover { background: #fee2e2; }
    .product-table .edit-btn {
      background: var(--primary-soft);
      color: var(--primary);
      padding: 6px 14px;
      border-radius: 999px;
      font-size: var(--text-xs);
      font-weight: 600;
      border: 1px solid var(--primary-line);
      cursor: pointer;
      text-decoration: none;
    }
    .product-table .edit-btn:hover { background: var(--primary); color: var(--paper); }

    /* Modal/overlay for Add/Edit form */
    .modal-overlay {
      display: none;
      position: fixed;
      inset: 0;
      background: rgba(15, 23, 42, 0.6);
      z-index: 1000;
      place-items: center;
      padding: var(--s5);
    }
    .modal-overlay.is-open { display: grid; }
    .modal {
      background: var(--paper);
      border-radius: var(--r-lg);
      padding: var(--s8);
      max-width: 600px;
      width: 100%;
      max-height: 90vh;
      overflow-y: auto;
      box-shadow: var(--shadow-lg);
    }
    .modal h2 { font-size: var(--text-xl); margin-bottom: var(--s5); }
    .form-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: var(--s4);
    }
    .form-field { margin-bottom: var(--s4); }
    .form-field label {
      display: block;
      font-size: var(--text-sm);
      font-weight: 600;
      margin-bottom: var(--s2);
      color: var(--ink-soft);
    }
    .form-field input, .form-field select, .form-field textarea {
      width: 100%;
      padding: 10px 14px;
      border: 1px solid var(--rule-strong);
      border-radius: var(--r-sm);
      font-size: var(--text-sm);
      font-family: var(--ff-body);
      background: var(--bg);
    }
    .form-field textarea { resize: vertical; min-height: 80px; }
    .form-field-full { grid-column: 1 / -1; }
    .modal-actions {
      display: flex;
      gap: var(--s3);
      margin-top: var(--s6);
    }
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

  <div class="admin-head">
    <div class="container">
      <h1>Product Management</h1>
      <p>Add, edit, and remove products from the OrbiTech database.</p>
    </div>
  </div>

  <section class="section">
    <div class="container">

      <!-- Status messages -->
      <asp:Panel ID="StatusPanel" runat="server" CssClass="alert" Visible="false">
        <asp:Literal ID="StatusLiteral" runat="server" />
      </asp:Panel>

      <div class="admin-actions">
        <span class="count">
          <asp:Literal ID="AdminCountLiteral" runat="server" Text="Loading..." />
        </span>
        <!-- TUTORIAL STEP: Button to show the Add Product form -->
        <asp:Button ID="ShowAddBtn" runat="server" Text="+ Add New Product" CssClass="btn btn--indigo" OnClick="ShowAddBtn_Click" />
      </div>

      <!-- TUTORIAL STEP: Add/Edit Product Form Panel (hidden by default) -->
      <asp:Panel ID="EditFormPanel" runat="server" Visible="false" style="background: var(--paper); border-radius: var(--r-lg); padding: var(--s8); box-shadow: var(--shadow); margin-bottom: var(--s7);">
        <h2 style="font-size: var(--text-xl); margin-bottom: var(--s5);">
          <asp:Literal ID="FormTitleLiteral" runat="server" Text="Add New Product" />
        </h2>

        <asp:Panel ID="FormErrorPanel" runat="server" CssClass="alert alert-error" Visible="false">
          <asp:Literal ID="FormErrorLiteral" runat="server" />
        </asp:Panel>

        <div class="form-grid">
          <div class="form-field form-field-full">
            <label>Product Name *</label>
            <asp:TextBox ID="TxtName" runat="server" placeholder="e.g., Samsung Galaxy S24 Ultra" />
          </div>

          <div class="form-field form-field-full">
            <label>Description *</label>
            <asp:TextBox ID="TxtDescription" runat="server" TextMode="MultiLine" placeholder="Enter product description..." />
          </div>

          <div class="form-field">
            <label>Price (R) *</label>
            <asp:TextBox ID="TxtPrice" runat="server" placeholder="e.g., 2499.00" />
          </div>

          <div class="form-field">
            <label>Quantity *</label>
            <asp:TextBox ID="TxtQuantity" runat="server" placeholder="e.g., 10" />
          </div>

          <div class="form-field form-field-full">
            <label>Image URL *</label>
            <asp:TextBox ID="TxtImageURL" runat="server" placeholder="https://..." />
          </div>

          <div class="form-field">
            <label>Category *</label>
            <asp:DropDownList ID="DdlCategory" runat="server" />
          </div>

          <div class="form-field">
            <label>Brand *</label>
            <asp:TextBox ID="TxtBrand" runat="server" placeholder="e.g., Samsung" />
          </div>

          <div class="form-field">
            <label>Colour</label>
            <asp:TextBox ID="TxtColour" runat="server" placeholder="e.g., Black" />
          </div>

          <div class="form-field">
            <label>Condition *</label>
            <asp:DropDownList ID="DdlCondition" runat="server">
              <asp:ListItem Text="New" Value="New" />
              <asp:ListItem Text="Refurbished" Value="Refurbished" />
            </asp:DropDownList>
          </div>

          <div class="form-field">
            <label>Grade *</label>
            <asp:DropDownList ID="DdlGrade" runat="server">
              <asp:ListItem Text="A" Value="A" />
              <asp:ListItem Text="B" Value="B" />
              <asp:ListItem Text="C" Value="C" />
            </asp:DropDownList>
          </div>
        </div>

        <div class="modal-actions">
          <!-- TUTORIAL STEP: Save button triggers SaveProductBtn_Click -->
          <asp:Button ID="SaveProductBtn" runat="server" Text="Save Product" CssClass="btn btn--indigo" OnClick="SaveProductBtn_Click" />
          <!-- Cancel button hides the form -->
          <asp:Button ID="CancelBtn" runat="server" Text="Cancel" CssClass="btn btn--ghost" OnClick="CancelBtn_Click" />
        </div>
      </asp:Panel>

      <!-- TUTORIAL STEP: Products table (generated from database) -->
      <asp:Repeater ID="AdminProductRepeater" runat="server">
        <HeaderTemplate>
          <table class="product-table">
            <thead>
              <tr>
                <th>Image</th>
                <th>Name</th>
                <th>Brand</th>
                <th>Category</th>
                <th>Price</th>
                <th>Qty</th>
                <th>Condition</th>
                <th>Grade</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
        </HeaderTemplate>
        <ItemTemplate>
          <tr>
            <td><img src='<%# Eval("ImageURL") %>' alt='<%# Eval("Name") %>' /></td>
            <td style="font-weight:600"><%# Eval("Name") %></td>
            <td><%# Eval("Brand") %></td>
            <td><%# Eval("CategoryName") %></td>
            <td>R<%# Eval("Price", "{0:N2}") %></td>
            <td><%# Eval("Quantity") %></td>
            <td><%# Eval("Condition") %></td>
            <td><%# Eval("Grade") %></td>
            <td>
              <div style="display:flex; gap:8px;">
                <!-- Edit button passes product ID back to the server -->
                <asp:LinkButton ID="EditBtn" runat="server" CssClass="edit-btn"
                    CommandArgument='<%# Eval("ProductID") %>'
                    OnCommand="EditProduct_Click"
                    Text="Edit" />
                <!-- Delete button asks for confirmation -->
                <asp:LinkButton ID="DeleteBtn" runat="server" CssClass="del-btn"
                    CommandArgument='<%# Eval("ProductID") %>'
                    OnCommand="DeleteProduct_Click"
                    OnClientClick="return confirm('Are you sure you want to delete this product?');"
                    Text="Delete" />
              </div>
            </td>
          </tr>
        </ItemTemplate>
        <FooterTemplate>
            </tbody>
          </table>
        </FooterTemplate>
      </asp:Repeater>

    </div>
  </section>

</asp:Content>

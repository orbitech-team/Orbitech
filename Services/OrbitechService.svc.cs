using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel;
using OrbitechWeb.Models;
using OrbitechWeb.Utils;
using FeedbackModel = OrbitechWeb.Models.Feedback;

namespace OrbitechWeb.Services
{
    public class OrbitechService : IOrbitechService
    {
        private string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["OrbitechDB"].ConnectionString;
            }
        }

        public bool ValidateUser(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = "SELECT u_PasswordHash FROM ORBI_USER WHERE u_Username = @Username";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);

                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                {
                    return false;
                }

                string storedHash = result.ToString();
                return PasswordHelper.VerifyPassword(password, storedHash);
            }
        }

        public bool RegisterUser(string username, string password, string email)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string checkQuery = "SELECT COUNT(*) FROM ORBI_USER WHERE u_Username = @Username";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@Username", username);

                conn.Open();
                int existing = (int)checkCmd.ExecuteScalar();
                if (existing > 0)
                {
                    return false;
                }

                string hashedPassword = PasswordHelper.HashPassword(password);

                string insertQuery = "INSERT INTO ORBI_USER (u_Username, u_PasswordHash, , u_RegisteredDate) VALUES (@Username, @PasswordHash, GETDATE())";
                SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                insertCmd.Parameters.AddWithValue("@Username", username);
                insertCmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                int rows = insertCmd.ExecuteNonQuery();
                return rows > 0;
            }
        }



        public List<Product> GetProducts()
        {
            return GetAllProducts();
        }

        public List<Product> GetAllProducts()
        {
            List<Product> products = new List<Product>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT p.p_ID, p.p_Name, p.p_Description, p.p_Price, p.p_Quantity, 
                                p.p_ImageURL, p.c_ID, c.c_Name, p.p_Brand, p.p_Colour, p.p_Condition, p.p_Grade
                                FROM ORBI_PRODUCT p
                                INNER JOIN CATEGORY c ON p.c_ID = c.c_ID
                                ORDER BY p.p_ID";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    products.Add(MapProduct(reader));
                }
            }

            return products;
        }

        public List<Product> GetProductsByCategory(string categoryName)
        {
            List<Product> products = new List<Product>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT p.p_ID, p.p_Name, p.p_Description, p.p_Price, p.p_Quantity, 
                                p.p_ImageURL, p.c_ID, c.c_Name, p.p_Brand, p.p_Colour, p.p_Condition, p.p_Grade
                                FROM ORBI_PRODUCT p
                                INNER JOIN CATEGORY c ON p.c_ID = c.c_ID
                                WHERE c.c_Name = @CategoryName
                                ORDER BY p.p_ID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CategoryName", categoryName);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    products.Add(MapProduct(reader));
                }
            }

            return products;
        }

        public List<Product> SearchProducts(string searchTerm)
        {
            List<Product> products = new List<Product>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                // One parameter, wildcards baked in once, reused by every LIKE.
                // OR'd across name, brand, description, condition, grade AND category
                // name (via the JOIN). Ranked: name matches first, then alphabetical.
                string query = @"SELECT p.p_ID, p.p_Name, p.p_Description, p.p_Price, p.p_Quantity,
                        p.p_ImageURL, p.c_ID, c.c_Name, p.p_Brand, p.p_Colour, p.p_Condition, p.p_Grade
                        FROM ORBI_PRODUCT p
                        INNER JOIN CATEGORY c ON p.c_ID = c.c_ID
                        WHERE p.p_Name LIKE @Term
                           OR p.p_Brand LIKE @Term
                           OR p.p_Description LIKE @Term
                           OR p.p_Condition LIKE @Term
                           OR p.p_Grade LIKE @Term
                           OR c.c_Name LIKE @TermF
                        ORDER BY CASE WHEN p.p_Name LIKE @Term THEN 0 ELSE 1 END, p.p_Name";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Term", "%" + searchTerm + "%");

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    products.Add(MapProduct(reader));
                }
            }

            return products;
        }


        public Product GetProduct(int productId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT p.p_ID, p.p_Name, p.p_Description, p.p_Price, p.p_Quantity, 
                                p.p_ImageURL, p.c_ID, c.c_Name, p.p_Brand, p.p_Colour, p.p_Condition, p.p_Grade
                                FROM ORBI_PRODUCT p
                                INNER JOIN CATEGORY c ON p.c_ID = c.c_ID
                                WHERE p.p_ID = @ProductID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProductID", productId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return MapProduct(reader);
                }
            }

            return null;
        }

        public Product GetProductById(int productId)
        {
            return GetProduct(productId);
        }

        public List<Category> GetAllCategories()
        {
            return GetCategories();
        }


        public bool AddProduct(Product product)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = @"INSERT INTO ORBI_PRODUCT 
                                (p_Name, p_Description, p_Price, p_Quantity, p_ImageURL, c_ID, p_Brand, p_Colour, p_Condition, p_Grade)
                                VALUES (@Name, @Description, @Price, @Quantity, @ImageURL, @CategoryID, @Brand, @Colour, @Condition, @Grade)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", product.Name);
                cmd.Parameters.AddWithValue("@Description", product.Description);
                cmd.Parameters.AddWithValue("@Price", product.Price);
                cmd.Parameters.AddWithValue("@Quantity", product.Quantity);
                cmd.Parameters.AddWithValue("@ImageURL", product.ImageURL);
                cmd.Parameters.AddWithValue("@CategoryID", product.CategoryID);
                cmd.Parameters.AddWithValue("@Brand", product.Brand);
                cmd.Parameters.AddWithValue("@Colour", string.IsNullOrEmpty(product.Colour) ? (object)DBNull.Value : product.Colour);
                cmd.Parameters.AddWithValue("@Condition", product.Condition);
                cmd.Parameters.AddWithValue("@Grade", product.Grade);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
        }

        public bool UpdateProduct(Product product)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = @"UPDATE ORBI_PRODUCT SET 
                                p_Name = @Name, p_Description = @Description, p_Price = @Price, 
                                p_Quantity = @Quantity, p_ImageURL = @ImageURL, c_ID = @CategoryID,
                                p_Brand = @Brand, p_Colour = @Colour, p_Condition = @Condition, p_Grade = @Grade
                                WHERE p_ID = @ProductID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProductID", product.ProductID);
                cmd.Parameters.AddWithValue("@Name", product.Name);
                cmd.Parameters.AddWithValue("@Description", product.Description);
                cmd.Parameters.AddWithValue("@Price", product.Price);
                cmd.Parameters.AddWithValue("@Quantity", product.Quantity);
                cmd.Parameters.AddWithValue("@ImageURL", product.ImageURL);
                cmd.Parameters.AddWithValue("@CategoryID", product.CategoryID);
                cmd.Parameters.AddWithValue("@Brand", product.Brand);
                cmd.Parameters.AddWithValue("@Colour", string.IsNullOrEmpty(product.Colour) ? (object)DBNull.Value : product.Colour);
                cmd.Parameters.AddWithValue("@Condition", product.Condition);
                cmd.Parameters.AddWithValue("@Grade", product.Grade);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
        }

        public bool DeleteProduct(int productId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = "DELETE FROM ORBI_PRODUCT WHERE p_ID = @ProductID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProductID", productId);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
        }

        public List<Category> GetCategories()
        {
            List<Category> categories = new List<Category>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = "SELECT c_ID, c_Name FROM CATEGORY ORDER BY c_ID";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    categories.Add(new Category
                    {
                        CategoryID = Convert.ToInt32(reader["c_ID"]),
                        CategoryName = reader["c_Name"].ToString()
                    });
                }
            }

            return categories;
        }

        public bool SaveContact(string name, string email, string subject, string message)
        {
            return SaveContactMessage(name, email, subject, message);
        }

        public bool SaveContactMessage(string name, string email, string subject, string message)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = @"INSERT INTO CONTACT (cn_Name, cn_Email, cn_Subject, cn_Message) 
                                VALUES (@Name, @Email, @Subject, @Message)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", string.IsNullOrEmpty(name) ? (object)DBNull.Value : name);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Subject", subject);
                cmd.Parameters.AddWithValue("@Message", message);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
        }

        private Product MapProduct(SqlDataReader reader)
        {
            return new Product
            {
                ProductID = Convert.ToInt32(reader["p_ID"]),
                Name = reader["p_Name"].ToString(),
                Description = reader["p_Description"].ToString(),
                Price = Convert.ToDecimal(reader["p_Price"]),
                Quantity = Convert.ToInt32(reader["p_Quantity"]),
                ImageURL = reader["p_ImageURL"].ToString(),
                CategoryID = Convert.ToInt32(reader["c_ID"]),
                CategoryName = reader["c_Name"].ToString(),
                Brand = reader["p_Brand"].ToString(),
                Colour = reader["p_Colour"] != DBNull.Value ? reader["p_Colour"].ToString() : "",
                Condition = reader["p_Condition"] != DBNull.Value ? reader["p_Condition"].ToString() : "",
                Grade = reader["p_Grade"] != DBNull.Value ? reader["p_Grade"].ToString() : ""
            };
        }

        // ============================================================
        // CART METHODS — Per-customer cart using CART and CART_ITEM tables
        // ============================================================

        private int GetUserId(string username)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = "SELECT u_ID FROM ORBI_USER WHERE u_Username = @Username";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);

                conn.Open();
                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
                return -1;
            }
        }

        private int GetOrCreateCart(int userId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                // Check if user already has a cart
                string checkQuery = "SELECT ct_ID FROM CART WHERE u_ID = @UserID";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@UserID", userId);

                conn.Open();
                object result = checkCmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }

                // Create a new cart for this user
                string insertQuery = "INSERT INTO CART (u_ID) VALUES (@UserID); SELECT CAST(SCOPE_IDENTITY() AS INT)";
                SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                insertCmd.Parameters.AddWithValue("@UserID", userId);

                return (int)insertCmd.ExecuteScalar();
            }
        }

        public List<CartItem> GetCartItems(string username)
        {
            List<CartItem> items = new List<CartItem>();

            int userId = GetUserId(username);
            if (userId == -1) return items;

            int cartId = GetOrCreateCart(userId);

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT ci.ci_ID, ci.p_ID, p.p_Name, p.p_ImageURL, p.p_Price, 
                                ci.ci_Quantity, p.p_Brand, p.p_Condition
                                FROM CART_ITEM ci
                                INNER JOIN ORBI_PRODUCT p ON ci.p_ID = p.p_ID
                                WHERE ci.ct_ID = @CartID
                                ORDER BY ci.ci_ID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CartID", cartId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    decimal price = Convert.ToDecimal(reader["p_Price"]);
                    int qty = Convert.ToInt32(reader["ci_Quantity"]);

                    items.Add(new CartItem
                    {
                        CartItemID = Convert.ToInt32(reader["ci_ID"]),
                        ProductID = Convert.ToInt32(reader["p_ID"]),
                        ProductName = reader["p_Name"].ToString(),
                        ProductImage = reader["p_ImageURL"].ToString(),
                        ProductPrice = price,
                        Quantity = qty,
                        LineTotal = price * qty,
                        Brand = reader["p_Brand"].ToString(),
                        Condition = reader["p_Condition"] != DBNull.Value ? reader["p_Condition"].ToString() : ""
                    });
                }
            }

            return items;
        }

        public bool AddToCart(string username, int productId, int quantity)
        {
            int userId = GetUserId(username);
            if (userId == -1) return false;

            int cartId = GetOrCreateCart(userId);

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                // Check if this product is already in the cart
                string checkQuery = "SELECT ci_ID, ci_Quantity FROM CART_ITEM WHERE ct_ID = @CartID AND p_ID = @ProductID";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@CartID", cartId);
                checkCmd.Parameters.AddWithValue("@ProductID", productId);

                conn.Open();
                SqlDataReader reader = checkCmd.ExecuteReader();

                if (reader.Read())
                {
                    // Product already in cart — update quantity
                    int existingItemId = Convert.ToInt32(reader["ci_ID"]);
                    int existingQty = Convert.ToInt32(reader["ci_Quantity"]);
                    reader.Close();

                    string updateQuery = "UPDATE CART_ITEM SET ci_Quantity = @NewQty WHERE ci_ID = @ItemID";
                    SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                    updateCmd.Parameters.AddWithValue("@NewQty", existingQty + quantity);
                    updateCmd.Parameters.AddWithValue("@ItemID", existingItemId);

                    return updateCmd.ExecuteNonQuery() > 0;
                }
                else
                {
                    reader.Close();

                    // Product not in cart — insert new row
                    string insertQuery = "INSERT INTO CART_ITEM (ct_ID, p_ID, ci_Quantity) VALUES (@CartID, @ProductID, @Quantity)";
                    SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@CartID", cartId);
                    insertCmd.Parameters.AddWithValue("@ProductID", productId);
                    insertCmd.Parameters.AddWithValue("@Quantity", quantity);

                    return insertCmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool RemoveFromCart(int cartItemId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = "DELETE FROM CART_ITEM WHERE ci_ID = @ItemID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ItemID", cartItemId);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool UpdateCartQuantity(int cartItemId, int quantity)
        {
            if (quantity <= 0)
            {
                // If quantity is 0 or less, remove the item
                return RemoveFromCart(cartItemId);
            }

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = "UPDATE CART_ITEM SET ci_Quantity = @Quantity WHERE ci_ID = @ItemID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.Parameters.AddWithValue("@ItemID", cartItemId);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public int GetCartCount(string username)
        {
            int userId = GetUserId(username);
            if (userId == -1) return 0;

            int cartId = GetOrCreateCart(userId);

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = "SELECT ISNULL(SUM(ci_Quantity), 0) FROM CART_ITEM WHERE ct_ID = @CartID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CartID", cartId);

                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }
        public bool AddFavourite(string username, int productId)
        {
            int userId = GetUserId(username);
            if (userId == -1) return false;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = "INSERT INTO FAVOURITE (u_ID, p_ID) VALUES (@UserID, @ProductID)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@ProductID", productId);

                try
                {
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
                catch (SqlException ex)
                {
                    // 2627/2601 = UNIQUE constraint violation => user already favourited
                    // this product. The DATABASE caught the duplicate - that is the
                    // UQ_Favourite_UserProduct constraint doing its job.
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        return false;
                    }
                    throw;
                }
            }
        }

        public bool RemoveFavourite(string username, int productId)
        {
            int userId = GetUserId(username);
            if (userId == -1) return false;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = "DELETE FROM FAVOURITE WHERE u_ID = @UserID AND p_ID = @ProductID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@ProductID", productId);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
        }

        public bool IsFavourite(string username, int productId)
        {
            int userId = GetUserId(username);
            if (userId == -1) return false;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = "SELECT COUNT(*) FROM FAVOURITE WHERE u_ID = @UserID AND p_ID = @ProductID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@ProductID", productId);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        public List<Product> GetFavourites(string username)
        {
            List<Product> products = new List<Product>();

            int userId = GetUserId(username);
            if (userId == -1) return products;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT p.p_ID, p.p_Name, p.p_Description, p.p_Price, p.p_Quantity,
                        p.p_ImageURL, p.c_ID, c.c_Name, p.p_Brand, p.p_Colour, p.p_Condition, p.p_Grade
                        FROM FAVOURITE f
                        INNER JOIN ORBI_PRODUCT p ON f.p_ID = p.p_ID
                        INNER JOIN CATEGORY c ON p.c_ID = c.c_ID
                        WHERE f.u_ID = @UserID
                        ORDER BY f.fav_Date DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    products.Add(MapProduct(reader));
                }
            }

            return products;
        }

        public List<int> GetFavouritedProductIds(string username)
        {
            List<int> ids = new List<int>();

            int userId = GetUserId(username);
            if (userId == -1) return ids;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = "SELECT p_ID FROM FAVOURITE WHERE u_ID = @UserID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ids.Add(Convert.ToInt32(reader["p_ID"]));
                }
            }

            return ids;
        }



        // PART A: Valid promo codes. Later you could swap this for a
        // PROMO_CODE database table - the rest of the code would not change.
        private bool IsValidPromo(string code)
        {
            return code != null && code.Trim().ToUpper() == "STUDENT15";
        }

        // PART A: The core checkout action. Everything below runs inside
        // a SqlTransaction - if ANY step fails, the whole thing rolls back.
        // That prevents half-completed orders (e.g. order created but stock
        // not decremented).
        public int PlaceOrder(string username, string promoCode)
        {
            int userId = GetUserId(username);
            if (userId == -1) return -1;

            int cartId = GetOrCreateCart(userId);
            List<CartItem> items = GetCartItems(username);
            if (items.Count == 0) return -1;

            OrderTotals totals = CalculateOrderTotals(items, promoCode);

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlTransaction tx = conn.BeginTransaction();
                try
                {
                    // STEP 1: Insert the order header row
                    int orderId;
                    string insertOrder = @"INSERT INTO ORBI_ORDER
                        (u_ID, o_Subtotal, o_Discount, o_Shipping, o_Tax, o_Total, o_PromoCode, o_Status)
                        VALUES (@UserID, @Subtotal, @Discount, @Shipping, @Tax, @Total, @Promo, 'Placed');
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    using (SqlCommand cmd = new SqlCommand(insertOrder, conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        cmd.Parameters.AddWithValue("@Subtotal", totals.Subtotal);
                        cmd.Parameters.AddWithValue("@Discount", totals.Discount);
                        cmd.Parameters.AddWithValue("@Shipping", totals.Shipping);
                        cmd.Parameters.AddWithValue("@Tax", totals.Tax);
                        cmd.Parameters.AddWithValue("@Total", totals.Total);
                        cmd.Parameters.AddWithValue("@Promo", (object)totals.PromoCode ?? DBNull.Value);
                        orderId = (int)cmd.ExecuteScalar();
                    }

                    // STEP 2: One ORDER_ITEM row per cart item + stock decrement.
                    // We SNAPSHOT the price (oi_UnitPrice) instead of joining back later.
                    foreach (CartItem item in items)
                    {
                        string insertItem = @"INSERT INTO ORDER_ITEM
                            (o_ID, p_ID, oi_Quantity, oi_UnitPrice)
                            VALUES (@OrderID, @ProductID, @Qty, @UnitPrice)";
                        using (SqlCommand cmd = new SqlCommand(insertItem, conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@OrderID", orderId);
                            cmd.Parameters.AddWithValue("@ProductID", item.ProductID);
                            cmd.Parameters.AddWithValue("@Qty", item.Quantity);
                            cmd.Parameters.AddWithValue("@UnitPrice", item.ProductPrice);
                            cmd.ExecuteNonQuery();
                        }

                        // Decrease stock so Reports' "stock on hand" is accurate.
                        string updateStock = "UPDATE ORBI_PRODUCT SET p_Quantity = p_Quantity - @Qty WHERE p_ID = @ProductID";
                        using (SqlCommand cmd = new SqlCommand(updateStock, conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@Qty", item.Quantity);
                            cmd.Parameters.AddWithValue("@ProductID", item.ProductID);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // STEP 3: Generate the invoice row.
                    // Format: INV-2026-0001 (year + order id padded to 4 digits).
                    string invoiceNumber = "INV-" + DateTime.Now.Year + "-" + orderId.ToString("D4");
                    string insertInvoice = "INSERT INTO INVOICE (o_ID, inv_Number) VALUES (@OrderID, @InvoiceNumber)";
                    using (SqlCommand cmd = new SqlCommand(insertInvoice, conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@OrderID", orderId);
                        cmd.Parameters.AddWithValue("@InvoiceNumber", invoiceNumber);
                        cmd.ExecuteNonQuery();
                    }

                    // STEP 4: Empty the customer's cart.
                    string clearCart = "DELETE FROM CART_ITEM WHERE ct_ID = @CartID";
                    using (SqlCommand cmd = new SqlCommand(clearCart, conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@CartID", cartId);
                        cmd.ExecuteNonQuery();
                    }

                    // All 4 steps succeeded. Make it permanent.
                    tx.Commit();
                    return orderId;
                }
                catch
                {
                    // Something failed above. Undo EVERYTHING so we never
                    // leave a half-created order in the database.
                    tx.Rollback();
                    return -1;
                }
            }
        }

        // PART A: Full order incl. invoice number. Member B's invoice
        // pages will reuse this same method.
        public Order GetOrderById(int orderId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT o.o_ID, o.u_ID, o.o_Date, o.o_Subtotal, o.o_Discount,
                                o.o_Shipping, o.o_Tax, o.o_Total, o.o_PromoCode, o.o_Status,
                                i.inv_Number, u.u_Username
                                FROM ORBI_ORDER o
                                INNER JOIN INVOICE i ON o.o_ID = i.o_ID
                                INNER JOIN ORBI_USER u ON o.u_ID = u.u_ID
                                WHERE o.o_ID = @OrderID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@OrderID", orderId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.Read()) return null;

                Order order = new Order
                {
                    OrderID = Convert.ToInt32(reader["o_ID"]),
                    UserID = Convert.ToInt32(reader["u_ID"]),
                    Username = reader["u_Username"].ToString(),
                    OrderDate = Convert.ToDateTime(reader["o_Date"]),
                    Subtotal = Convert.ToDecimal(reader["o_Subtotal"]),
                    Discount = Convert.ToDecimal(reader["o_Discount"]),
                    Shipping = Convert.ToDecimal(reader["o_Shipping"]),
                    Tax = Convert.ToDecimal(reader["o_Tax"]),
                    Total = Convert.ToDecimal(reader["o_Total"]),
                    PromoCode = reader["o_PromoCode"] != DBNull.Value ? reader["o_PromoCode"].ToString() : "",
                    Status = reader["o_Status"].ToString(),
                    InvoiceNumber = reader["inv_Number"].ToString()
                };

                reader.Close();
                order.Items = GetOrderItems(orderId);
                return order;
            }
        }

        public List<OrderItem> GetOrderItems(int orderId)
        {
            List<OrderItem> items = new List<OrderItem>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT oi.oi_ID, oi.p_ID, oi.oi_Quantity, oi.oi_UnitPrice,
                                p.p_Name, p.p_ImageURL
                                FROM ORDER_ITEM oi
                                INNER JOIN ORBI_PRODUCT p ON oi.p_ID = p.p_ID
                                WHERE oi.o_ID = @OrderID
                                ORDER BY oi.oi_ID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@OrderID", orderId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    decimal unitPrice = Convert.ToDecimal(reader["oi_UnitPrice"]);
                    int qty = Convert.ToInt32(reader["oi_Quantity"]);

                    items.Add(new OrderItem
                    {
                        OrderItemID = Convert.ToInt32(reader["oi_ID"]),
                        ProductID = Convert.ToInt32(reader["p_ID"]),
                        ProductName = reader["p_Name"].ToString(),
                        ProductImage = reader["p_ImageURL"].ToString(),
                        Quantity = qty,
                        UnitPrice = unitPrice,
                        LineTotal = unitPrice * qty
                    });
                }
            }
            return items;
        }

        // PART A: Member B needs this for MyInvoices.aspx.
        public List<Order> GetOrdersForUser(string username)
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT o.o_ID, i.inv_Number, o.o_Date, o.o_Total, o.o_Status
                                FROM ORBI_ORDER o
                                INNER JOIN INVOICE i ON o.o_ID = i.o_ID
                                INNER JOIN ORBI_USER u ON o.u_ID = u.u_ID
                                WHERE u.u_Username = @Username
                                ORDER BY o.o_Date DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    orders.Add(new Order
                    {
                        OrderID = Convert.ToInt32(reader["o_ID"]),
                        InvoiceNumber = reader["inv_Number"].ToString(),
                        OrderDate = Convert.ToDateTime(reader["o_Date"]),
                        Total = Convert.ToDecimal(reader["o_Total"]),
                        Status = reader["o_Status"].ToString()
                    });
                }
            }
            return orders;
        }


        public OrderTotals CalculateOrderTotals(List<CartItem> items, string promoCode)
        {
            decimal subtotal = 0;

            foreach (CartItem item in items)
            {
                subtotal += item.LineTotal;
            }
            OrderTotals orderTotal = new OrderTotals();

            orderTotal.Subtotal = subtotal;

            // --- Rule 3: Promo code discount ---
            if (!string.IsNullOrEmpty(promoCode) && IsValidPromo(promoCode))
            {
                orderTotal.PromoCode = promoCode.ToUpper();
                orderTotal.PromoApplied = true;
                orderTotal.Discount = Math.Round(subtotal * 0.15m, 2);
            }

            // --- Rule 2: Free shipping over R1000 (after discount), else R100 flat ---
            decimal afterDiscount = subtotal - orderTotal.Discount;
            orderTotal.Shipping = afterDiscount >= 1000m ? 0m : 100m;

            // --- Rule 1: VAT 15% on the discounted amount ---
            orderTotal.Tax = Math.Round(afterDiscount * 0.15m, 2);

            // Grand total: goods - discount + shipping + VAT
            orderTotal.Total = afterDiscount + orderTotal.Shipping + orderTotal.Tax;

            return orderTotal;
        }

        // ============================================================
        // PHASE 3: PROFILE METHODS
        // ============================================================

        // PHASE 3: Customer activity stats. FavouriteCount is wrapped in
        // try/catch because the FAVOURITE table is built by a teammate in
        // parallel (Phase 2). Until their merge lands, the count reads 0
        // instead of crashing the whole profile page.
        public ProfileStats GetProfileStats(string username)
        {
            ProfileStats stats = new ProfileStats();
            stats.Username = username;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                // Personal orders + lifetime spend (one query, two aggregates)
                string orderQuery = @"SELECT COUNT(*) AS OrderCount,
                                        ISNULL(SUM(o_Total), 0) AS TotalSpent,
                                        MIN(u_RegisteredDate) AS MemberSince
                                        FROM ORBI_ORDER o
                                        INNER JOIN ORBI_USER u ON o.u_ID = u.u_ID
                                        WHERE u.u_Username = @Username";
                using (SqlCommand cmd = new SqlCommand(orderQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            stats.TotalOrders = Convert.ToInt32(reader["OrderCount"]);
                            stats.TotalSpent = Convert.ToDecimal(reader["TotalSpent"]);
                            if (reader["MemberSince"] != DBNull.Value)
                                stats.MemberSince = Convert.ToDateTime(reader["MemberSince"]);
                        }
                    }
                }

                // Favourites count - table owned by Phase 2, may not exist yet
                try
                {
                    string favQuery = @"SELECT COUNT(*) FROM FAVOURITE f
                                        INNER JOIN ORBI_USER u ON f.u_ID = u.u_ID
                                        WHERE u.u_Username = @Username";
                    using (SqlCommand cmd = new SqlCommand(favQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        stats.FavouriteCount = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
                catch
                {
                    // FAVOURITE table not created yet - count stays 0.
                    // This catch disappears once Phase 2 merges.
                    stats.FavouriteCount = 0;
                }
            }
            return stats;
        }

        // PHASE 3: The four admin KPI tiles. Four scalar queries, one method,
        // so the page makes one service call instead of four.
        public AdminDashboardStats GetAdminDashboardStats()
        {
            AdminDashboardStats stats = new AdminDashboardStats();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                stats.TotalProducts = Convert.ToInt32(
                    new SqlCommand("SELECT COUNT(*) FROM ORBI_PRODUCT", conn).ExecuteScalar());

                stats.TotalOrders = Convert.ToInt32(
                    new SqlCommand("SELECT COUNT(*) FROM ORBI_ORDER", conn).ExecuteScalar());

                stats.TotalUsers = Convert.ToInt32(
                    new SqlCommand("SELECT COUNT(*) FROM ORBI_USER", conn).ExecuteScalar());

                stats.TotalRevenue = Convert.ToDecimal(
                    new SqlCommand("SELECT ISNULL(SUM(o_Total), 0) FROM ORBI_ORDER", conn).ExecuteScalar());
            }
            return stats;
        }

        // PHASE 3: Latest N orders across ALL users (admin view).
        // SELECT TOP with a parameter - SQL Server allows @Count in TOP,
        // which keeps the limit from being string-concatenated.
        public List<Order> GetRecentOrders(int count)
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT TOP (@Count)
                                o.o_ID, u.u_Username, i.inv_Number,
                                o.o_Date, o.o_Total, o.o_Status
                                FROM ORBI_ORDER o
                                INNER JOIN ORBI_USER u ON o.u_ID = u.u_ID
                                INNER JOIN INVOICE i ON o.o_ID = i.o_ID
                                ORDER BY o.o_Date DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Count", count);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    orders.Add(new Order
                    {
                        OrderID = Convert.ToInt32(reader["o_ID"]),
                        Username = reader["u_Username"].ToString(),
                        InvoiceNumber = reader["inv_Number"].ToString(),
                        OrderDate = Convert.ToDateTime(reader["o_Date"]),
                        Total = Convert.ToDecimal(reader["o_Total"]),
                        Status = reader["o_Status"].ToString()
                    });
                }
            }
            return orders;
        }

        // PHASE 3: Low-stock alert list for the admin dashboard.
        // Threshold 3 - tune to taste, but keep it a constant here,
        // one place to change.
        public List<Product> GetLowStockProducts()
        {
            List<Product> products = new List<Product>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT p_ID, p_Name, p_Price, p_Quantity, p_ImageURL,
                                p_Brand, p_Condition, p_Grade
                                FROM ORBI_PRODUCT
                                WHERE p_Quantity <= 3
                                ORDER BY p_Quantity ASC";

                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        ProductID = Convert.ToInt32(reader["p_ID"]),
                        Name = reader["p_Name"].ToString(),
                        Price = Convert.ToDecimal(reader["p_Price"]),
                        Quantity = Convert.ToInt32(reader["p_Quantity"]),
                        ImageURL = reader["p_ImageURL"].ToString(),
                        Brand = reader["p_Brand"].ToString(),
                        Condition = reader["p_Condition"].ToString(),
                        Grade = reader["p_Grade"].ToString()
                    });
                }
            }
            return products;
        }




        // === MEMBER C: FEEDBACK + REPORTS ===

        public string SubmitFeedback(int orderId, int deliveryRating, int satisfactionRating,
            string comments, bool isComplaint, string complaintCategory)
        {
            if (deliveryRating < 1 || deliveryRating > 5
                || satisfactionRating < 1 || satisfactionRating > 5)
            {
                return "Ratings must be between 1 and 5.";
            }

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                string orderQuery = "SELECT COUNT(*) FROM ORBI_ORDER WHERE o_ID = @OrderID";
                using (SqlCommand orderCmd = new SqlCommand(orderQuery, conn))
                {
                    orderCmd.Parameters.AddWithValue("@OrderID", orderId);
                    if (Convert.ToInt32(orderCmd.ExecuteScalar()) == 0)
                    {
                        return "The selected order does not exist.";
                    }
                }

                string duplicateQuery = "SELECT COUNT(*) FROM FEEDBACK WHERE o_ID = @OrderID";
                using (SqlCommand duplicateCmd = new SqlCommand(duplicateQuery, conn))
                {
                    duplicateCmd.Parameters.AddWithValue("@OrderID", orderId);
                    if (Convert.ToInt32(duplicateCmd.ExecuteScalar()) > 0)
                    {
                        return "Feedback has already been submitted for this order.";
                    }
                }

                string insertQuery = @"INSERT INTO FEEDBACK
                        (o_ID, f_DeliveryRating, f_SatisfactionRating, f_Comments,
                         f_IsComplaint, f_ComplaintCategory, f_DateSubmitted)
                    VALUES
                        (@OrderID, @DeliveryRating, @SatisfactionRating, @Comments,
                         @IsComplaint, @ComplaintCategory, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@OrderID", orderId);
                    cmd.Parameters.AddWithValue("@DeliveryRating", deliveryRating);
                    cmd.Parameters.AddWithValue("@SatisfactionRating", satisfactionRating);
                    cmd.Parameters.AddWithValue("@Comments",
                        string.IsNullOrWhiteSpace(comments) ? (object)DBNull.Value : comments);
                    cmd.Parameters.AddWithValue("@IsComplaint", isComplaint);
                    cmd.Parameters.AddWithValue("@ComplaintCategory",
                        string.IsNullOrWhiteSpace(complaintCategory)
                            ? (object)DBNull.Value : complaintCategory);

                    return cmd.ExecuteNonQuery() > 0
                        ? "SUCCESS: Feedback submitted successfully."
                        : "Feedback could not be submitted.";
                }
            }
        }

        public FeedbackSummary GetFeedbackSummary()
        {
            FeedbackSummary summary = new FeedbackSummary();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                string aggregateQuery = @"SELECT
                        COUNT(*) AS TotalResponses,
                        ISNULL(AVG(CAST(f_DeliveryRating AS DECIMAL(5,2))), 0) AS AvgDeliveryRating,
                        ISNULL(AVG(CAST(f_SatisfactionRating AS DECIMAL(5,2))), 0) AS AvgSatisfactionRating,
                        ISNULL(SUM(CASE WHEN f_SatisfactionRating >= 4 THEN 1 ELSE 0 END) * 100.0
                            / NULLIF(COUNT(*), 0), 0) AS PercentSatisfied
                    FROM FEEDBACK";

                using (SqlCommand cmd = new SqlCommand(aggregateQuery, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        summary.TotalResponses = Convert.ToInt32(reader["TotalResponses"]);
                        summary.AvgDeliveryRating = Convert.ToDouble(reader["AvgDeliveryRating"]);
                        summary.AvgSatisfactionRating = Convert.ToDouble(reader["AvgSatisfactionRating"]);
                        summary.PercentSatisfied = Convert.ToDouble(reader["PercentSatisfied"]);
                    }
                }

                string distributionQuery = @"SELECT f_SatisfactionRating AS Stars, COUNT(*) AS RatingCount
                    FROM FEEDBACK
                    GROUP BY f_SatisfactionRating";

                Dictionary<int, int> counts = new Dictionary<int, int>();
                using (SqlCommand cmd = new SqlCommand(distributionQuery, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        counts[Convert.ToInt32(reader["Stars"])] = Convert.ToInt32(reader["RatingCount"]);
                    }
                }

                for (int star = 1; star <= 5; star++)
                {
                    int count = counts.ContainsKey(star) ? counts[star] : 0;
                    summary.SatisfactionDistribution.Add(new SatisfactionBucket
                    {
                        Stars = star,
                        Count = count,
                        Percent = summary.TotalResponses > 0
                            ? Math.Round(count * 100.0 / summary.TotalResponses, 1) : 0
                    });
                }
            }

            return summary;
        }

        public List<FeedbackModel> GetRecentFeedback(int count)
        {
            List<FeedbackModel> feedback = new List<FeedbackModel>();
            int safeCount = Math.Max(1, Math.Min(count, 100));

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT TOP (@Count)
                        f_ID, o_ID, f_DeliveryRating, f_SatisfactionRating,
                        f_Comments, f_IsComplaint, f_ComplaintCategory, f_DateSubmitted
                    FROM FEEDBACK
                    ORDER BY f_DateSubmitted DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Count", safeCount);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        feedback.Add(MapFeedback(reader));
                    }
                }
            }

            return feedback;
        }

        public List<FeedbackModel> GetComplaints()
        {
            List<FeedbackModel> complaints = new List<FeedbackModel>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT f_ID, o_ID, f_DeliveryRating, f_SatisfactionRating,
                        f_Comments, f_IsComplaint, f_ComplaintCategory, f_DateSubmitted
                    FROM FEEDBACK
                    WHERE f_IsComplaint = 1
                    ORDER BY f_DateSubmitted DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        complaints.Add(MapFeedback(reader));
                    }
                }
            }

            return complaints;
        }

        private FeedbackModel MapFeedback(SqlDataReader reader)
        {
            return new FeedbackModel
            {
                FeedbackID = Convert.ToInt32(reader["f_ID"]),
                OrderID = Convert.ToInt32(reader["o_ID"]),
                DeliveryRating = Convert.ToInt32(reader["f_DeliveryRating"]),
                SatisfactionRating = Convert.ToInt32(reader["f_SatisfactionRating"]),
                Comments = reader["f_Comments"] == DBNull.Value
                    ? string.Empty
                    : reader["f_Comments"].ToString(),
                IsComplaint = Convert.ToBoolean(reader["f_IsComplaint"]),
                ComplaintCategory = reader["f_ComplaintCategory"] == DBNull.Value
                    ? string.Empty
                    : reader["f_ComplaintCategory"].ToString(),
                FeedbackDate = Convert
                    .ToDateTime(reader["f_DateSubmitted"])
                    .ToString("dd MMM yyyy")
            };
        }


        public SalesSummary GetSalesSummary(int days)
        {
            SalesSummary summary = new SalesSummary();
            int safeDays = Math.Max(1, Math.Min(days, 3650));

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                // Use the saved order total so revenue exactly matches checkout/invoices.
                string totalsQuery = @"SELECT
                        ISNULL(SUM(o_Total), 0) AS TotalRevenue,
                        COUNT(*) AS OrderCount,
                        ISNULL(AVG(o_Total), 0) AS AvgOrderValue
                    FROM ORBI_ORDER
                    WHERE o_Date >= DATEADD(DAY, -@Days, GETDATE())
                      AND o_Status <> 'Cancelled'";

                using (SqlCommand cmd = new SqlCommand(totalsQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Days", safeDays);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            summary.TotalRevenue = Convert.ToDecimal(reader["TotalRevenue"]);
                            summary.OrderCount = Convert.ToInt32(reader["OrderCount"]);
                            summary.AvgOrderValue = Convert.ToDecimal(reader["AvgOrderValue"]);
                        }
                    }
                }

                string monthlyQuery = @"SELECT
                        CONVERT(char(7), o_Date, 120) AS MonthLabel,
                        COUNT(*) AS OrderCount,
                        ISNULL(SUM(o_Total), 0) AS Revenue
                    FROM ORBI_ORDER
                    WHERE o_Date >= DATEADD(DAY, -@Days, GETDATE())
                      AND o_Status <> 'Cancelled'
                    GROUP BY CONVERT(char(7), o_Date, 120)
                    ORDER BY MonthLabel DESC";

                using (SqlCommand cmd = new SqlCommand(monthlyQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Days", safeDays);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            summary.MonthlySales.Add(new MonthlySale
                            {
                                MonthLabel = reader["MonthLabel"].ToString(),
                                OrderCount = Convert.ToInt32(reader["OrderCount"]),
                                Revenue = Convert.ToDecimal(reader["Revenue"])
                            });
                        }
                    }
                }

                string productsQuery = @"SELECT TOP 10
                        p.p_Name AS ProductName,
                        SUM(oi.oi_Quantity) AS QuantitySold,
                        SUM(oi.oi_Quantity * oi.oi_UnitPrice) AS Revenue
                    FROM ORBI_PRODUCT p
                    INNER JOIN ORDER_ITEM oi ON p.p_ID = oi.p_ID
                    INNER JOIN ORBI_ORDER o ON oi.o_ID = o.o_ID
                    WHERE o.o_Date >= DATEADD(DAY, -@Days, GETDATE())
                      AND o.o_Status <> 'Cancelled'
                    GROUP BY p.p_Name
                    ORDER BY Revenue DESC";

                using (SqlCommand cmd = new SqlCommand(productsQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Days", safeDays);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            summary.TopProducts.Add(new TopProduct
                            {
                                ProductName = reader["ProductName"].ToString(),
                                QuantitySold = Convert.ToInt32(reader["QuantitySold"]),
                                Revenue = Convert.ToDecimal(reader["Revenue"])
                            });
                        }
                    }
                }
            }

            return summary;
        }

    }
}

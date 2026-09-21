using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel;
using OrbitechWeb.Models;
using OrbitechWeb.Utils;

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

                string insertQuery = "INSERT INTO ORBI_USER (u_Username, u_PasswordHash) VALUES (@Username, @PasswordHash)";
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
                           OR c.c_Name LIKE @Term
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

    }
}

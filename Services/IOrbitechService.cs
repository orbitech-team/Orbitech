using System.Collections.Generic;
using System.ServiceModel;
using OrbitechWeb.Models;

namespace OrbitechWeb.Services
{
    [ServiceContract]
    public interface IOrbitechService
    {
        [OperationContract]
        bool ValidateUser(string username, string password);

        [OperationContract]
        bool RegisterUser(string username, string password, string email);

        [OperationContract]
        List<Product> GetProducts();

        [OperationContract]
        List<Product> GetAllProducts();

        [OperationContract]
        List<Product> GetProductsByCategory(string categoryName);

        [OperationContract]
        List<Product> SearchProducts(string searchTerm);

        [OperationContract]
        Product GetProduct(int productId);

        [OperationContract]

        Product GetProductById(int productId);

        [OperationContract]
        List<Category> GetAllCategories();

        [OperationContract]
        bool AddProduct(Product product);

        [OperationContract]
        bool UpdateProduct(Product product);

        [OperationContract]
        bool DeleteProduct(int productId);

        [OperationContract]
        List<Category> GetCategories();

        [OperationContract]
        bool SaveContact(string name, string email, string subject, string message);

        [OperationContract]
        bool SaveContactMessage(string name, string email, string subject, string message);

        [OperationContract]
        List<CartItem> GetCartItems(string username);

        [OperationContract]
        bool AddToCart(string username, int productId, int quantity);

        [OperationContract]
        bool RemoveFromCart(int cartItemId);

        [OperationContract]
        bool UpdateCartQuantity(int cartItemId, int quantity);

        [OperationContract]
        int GetCartCount(string username);



    }
}

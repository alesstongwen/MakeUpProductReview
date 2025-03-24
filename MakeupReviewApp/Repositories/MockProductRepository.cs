using MakeupReviewApp.Models;

namespace MakeupReviewApp.Repositories
{
    public class MockProductRepository
    {
        private static List<Product> _products = new List<Product>
        {
            new Product { Id = 1, Name = "Lipstick", Brand = "Maybelline", Category = "Lips", ImageUrl = "https://encrypted-tbn1.gstatic.com/shopping?q=tbn:ANd9GcQyORtW8X5BeQunwWme0aNG0mikutHPVkIYeLM4QclF3Wn-monVKCkI8bU_qyjI-a5XK4nwdt9yj6BX3VRcngZ44AxyY4Be8kb_zY6Hpe6YhEfx1bk4eyMSm5YD7Cxu&usqp=CAc", Price = 10.99M, Description = "Matte finish lipstick." },
            new Product { Id = 2, Name = "Foundation", Brand = "Fenty Beauty", Category = "Face", ImageUrl = "https://media.glamour.com/photos/59b7ea7ebcac137d68ef4541/master/w_2560%2Cc_limit/fentybeautyfoundation-fentybeauty-instagram-story.jpg", Price = 29.99M, Description = "Full coverage foundation." }
        };

        public List<Product> GetAllProducts()
        {
            return _products;
        }

        public Product GetProductById(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }
    }
}

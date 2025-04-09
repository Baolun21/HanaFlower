using BookShop.Data.Models;

namespace BookShop.Data.Dtos.Cart
{
    public class CartItem
    {
        public Products? Product { get; set; }
        public int Quantity { get; set; }
    }
}

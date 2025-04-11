namespace BookShop.Data.Dtos.Cart
{
    public class DetailOrder
    {
        public int MaKH { get; set; }
        public string? DiaChiGiao { get; set; }
        public string? SDT { get; set; }
        public string? email { get; set; }
        public string? note { get; set; }
        public decimal TongTien { get; set; }
        public int LoaiGiaoDich { get; set; } = 0; // 0. Tien mat, 1. Chuyen khoan
        public List<CartItem>? CartItems { get; set; }
    }
}

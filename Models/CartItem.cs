namespace HSU.PTWeb.AnhPH.BookStore.Models
{
    // Model CartItem đại diện cho 1 mục trong giỏ hàng (lưu tạm ở session)
    public class CartItem
    {
        // Khóa ngoại tới Product
        public int ProductId { get; set; }

        // Tên sản phẩm để hiển thị trong giỏ
        public string ProductName { get; set; }

        // Tương thích: Title cũ
        public string Title { get => ProductName; set => ProductName = value; }

        // Giá hiện tại của sản phẩm
        public decimal Price { get; set; }

        // Số lượng sản phẩm trong giỏ
        public int Quantity { get; set; }

        // Ảnh hiển thị
        public string ImageUrl { get; set; }

        // Tương thích: Image cũ
        public string Image { get => ImageUrl; set => ImageUrl = value; }
    }
}

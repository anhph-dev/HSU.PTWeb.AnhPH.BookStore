namespace HSU.PTWeb.AnhPH.BookStore.ViewModels
{
    // ViewModel cho trang danh sách đơn và chi tiết đơn
    public class OrderListItemViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
    }

    public class OrderDetailItemViewModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
    }

    public class OrderDetailsViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public List<OrderDetailItemViewModel> Items { get; set; } = new List<OrderDetailItemViewModel>();
    }
}

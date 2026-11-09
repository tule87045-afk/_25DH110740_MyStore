using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace _25DH110740_MyStore.Models.ViewModel
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Nhập họ tên"), StringLength(100), Display(Name = "Họ tên")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "Nhập tên đăng nhập"), StringLength(50, MinimumLength = 4, ErrorMessage = "Tên đăng nhập từ 4-50 ký tự"), Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Nhập mật khẩu"), StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự"), DataType(DataType.Password), Display(Name = "Mật khẩu")]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp"), DataType(DataType.Password), Display(Name = "Xác nhận mật khẩu")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Nhập email"), EmailAddress(ErrorMessage = "Email không hợp lệ"), Display(Name = "Email")]
        public string CustomerEmail { get; set; }
        [Required(ErrorMessage = "Nhập số điện thoại"), RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Số điện thoại không hợp lệ"), Display(Name = "Số điện thoại")]
        public string CustomerPhone { get; set; }
        [Required(ErrorMessage = "Nhập địa chỉ"), StringLength(250), Display(Name = "Địa chỉ")]
        public string CustomerAddress { get; set; }
    }

    public class LoginVM
    {
        [Required(ErrorMessage = "Nhập tên đăng nhập"), Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Nhập mật khẩu"), DataType(DataType.Password), Display(Name = "Mật khẩu")]
        public string Password { get; set; }
        [Display(Name = "Ghi nhớ đăng nhập")]
        public bool RememberMe { get; set; }
    }

    public class ProfileVM
    {
        public string Username { get; set; }
        [Required(ErrorMessage = "Nhập họ tên"), Display(Name = "Họ tên")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "Nhập email"), EmailAddress(ErrorMessage = "Email không hợp lệ"), Display(Name = "Email")]
        public string CustomerEmail { get; set; }
        [Required(ErrorMessage = "Nhập số điện thoại"), RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Số điện thoại không hợp lệ"), Display(Name = "Số điện thoại")]
        public string CustomerPhone { get; set; }
        [Required(ErrorMessage = "Nhập địa chỉ"), Display(Name = "Địa chỉ")]
        public string CustomerAddress { get; set; }
    }

    public class ChangePasswordVM
    {
        [Required(ErrorMessage = "Nhập mật khẩu hiện tại"), DataType(DataType.Password), Display(Name = "Mật khẩu hiện tại")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "Nhập mật khẩu mới"), StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự"), DataType(DataType.Password), Display(Name = "Mật khẩu mới")]
        public string NewPassword { get; set; }
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp"), DataType(DataType.Password), Display(Name = "Xác nhận mật khẩu mới")]
        public string ConfirmPassword { get; set; }
    }

    public class ProfileInfoVM
    {
        public ProfileVM Profile { get; set; }
        public ChangePasswordVM Password { get; set; }
    }

    public class HomeProductVM
    {
        public string SearchTerm { get; set; }
        public List<Product> FeaturedProducts { get; set; }
        public PagedList.IPagedList<Product> NewProducts { get; set; }
    }

    public class ProductListVM
    {
        public Category Category { get; set; }
        public string SortOrder { get; set; }
        public PagedList.IPagedList<Product> Products { get; set; }
    }

    public class ProductDetailVM
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public List<Product> RelatedProducts { get; set; }
        public PagedList.IPagedList<Product> TopProducts { get; set; }
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public string Category { get; set; }
        public int CategoryId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get { return UnitPrice * Quantity; } }
    }

    public class Cart
    {
        private readonly List<CartItem> items = new List<CartItem>();
        public List<CartItem> Items { get { return items; } }
        public decimal Total { get { return items.Sum(i => i.TotalPrice); } }
        public int Count { get { return items.Sum(i => i.Quantity); } }
        public List<IGrouping<string, CartItem>> GroupedItems { get { return items.GroupBy(i => i.Category).ToList(); } }
        public PagedList.IPagedList<Product> SimilarProducts { get; set; }
    }

    // Lưu giỏ hàng trong Session
    public class CartService
    {
        private const string Key = "Cart";
        private readonly System.Web.HttpSessionStateBase session;
        public CartService(System.Web.HttpSessionStateBase session) { this.session = session; }

        public Cart GetCart()
        {
            var cart = session[Key] as Cart;
            if (cart == null) { cart = new Cart(); session[Key] = cart; }
            return cart;
        }

        public void Add(Product p, int qty)
        {
            if (qty < 1) qty = 1;
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(i => i.ProductId == p.ProductId);
            if (item == null)
                cart.Items.Add(new CartItem { ProductId = p.ProductId, ProductName = p.ProductName, ImageUrl = p.ImageUrl, Category = p.Category != null ? p.Category.CategoryName : "Khác", CategoryId = p.CategoryId, UnitPrice = p.Price, Quantity = qty });
            else item.Quantity += qty;
        }

        public void Remove(int productId) { GetCart().Items.RemoveAll(i => i.ProductId == productId); }

        public void UpdateQuantity(int productId, int qty)
        {
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null) return;
            if (qty < 1) cart.Items.Remove(item); else item.Quantity = qty;
        }

        public void Clear() { session[Key] = null; }
    }

    public class CheckoutVM
    {
        public List<CartItem> Items { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Total { get { return Subtotal + ShippingFee; } }
        [Required, Display(Name = "Người nhận")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "Nhập số điện thoại"), Display(Name = "Số điện thoại")]
        public string CustomerPhone { get; set; }
        [Required(ErrorMessage = "Nhập địa chỉ giao hàng"), Display(Name = "Địa chỉ giao hàng")]
        public string ShippingAddress { get; set; }
        public string NewShippingAddress { get; set; }
        [Required(ErrorMessage = "Chọn hình thức giao hàng")]
        public string DeliveryMethod { get; set; }
        [Required(ErrorMessage = "Chọn hình thức thanh toán")]
        public string PaymentMethod { get; set; }
        public string SelectedIds { get; set; }
    }

    public class PersonalMenuVM
    {
        public bool IsLoggedIn { get; set; }
        public string Username { get; set; }
        public int CartCount { get; set; }
    }

    public class MyOrderVM
    {
        public string SearchTerm { get; set; }
        public string Status { get; set; }
        public List<Order> Orders { get; set; }
    }

    public class DashboardVM
    {
        public int ProductCount { get; set; }
        public int CustomerCount { get; set; }
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
        public List<KeyValuePair<string, int>> OrdersByStatus { get; set; }
        public List<KeyValuePair<string, decimal>> RevenueByMonth { get; set; }
        public List<Product> TopProducts { get; set; }
    }
}

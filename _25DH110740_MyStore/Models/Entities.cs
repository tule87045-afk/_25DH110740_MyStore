using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _25DH110740_MyStore.Models
{
    public class User
    {
        [Key, StringLength(50)]
        public string Username { get; set; }
        [Required, StringLength(200)]
        public string PasswordHash { get; set; }
        [StringLength(20)]
        public string Role { get; set; }   // "Customer" | "Admin"
    }

    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }
        [Required, StringLength(50)]
        public string Username { get; set; }
        [Required, StringLength(100)]
        public string CustomerName { get; set; }
        [StringLength(100)]
        public string CustomerEmail { get; set; }
        [StringLength(20)]
        public string CustomerPhone { get; set; }
        [StringLength(250)]
        public string CustomerAddress { get; set; }
        [ForeignKey("Username")]
        public virtual User User { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }

    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Nhập tên danh mục"), StringLength(100), Display(Name = "Tên danh mục")]
        public string CategoryName { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }

    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Nhập tên sản phẩm"), StringLength(200), Display(Name = "Tên sản phẩm")]
        public string ProductName { get; set; }
        [StringLength(100), Display(Name = "Thương hiệu")]
        public string Brand { get; set; }
        [Display(Name = "Mô tả")]
        public string ProductDescription { get; set; }
        [Range(0, 100000000, ErrorMessage = "Giá không hợp lệ"), Display(Name = "Giá")]
        public decimal Price { get; set; }
        [StringLength(250), Display(Name = "Ảnh (đường dẫn)")]
        public string ImageUrl { get; set; }
        [Display(Name = "Tồn kho")]
        public int Stock { get; set; }
        [Display(Name = "Đã bán")]
        public int Sold { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }

    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        [StringLength(30)]
        public string PaymentStatus { get; set; }   // Chờ thanh toán, Đang xử lý, Đang vận chuyển, Đã giao, Đã hủy
        [StringLength(30)]
        public string PaymentMethod { get; set; }
        [StringLength(30)]
        public string DeliveryMethod { get; set; }
        [StringLength(250)]
        public string ShippingAddress { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }

    public class OrderDetail
    {
        [Key]
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        [NotMapped]
        public decimal TotalPrice { get { return Quantity * UnitPrice; } }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }

    public static class OrderStatus
    {
        public const string Pending = "Chờ thanh toán";
        public const string Processing = "Đang xử lý";
        public const string Shipping = "Đang vận chuyển";
        public const string Delivered = "Đã giao";
        public const string Cancelled = "Đã hủy";
        public static readonly string[] All = { Pending, Processing, Shipping, Delivered, Cancelled };
    }
}

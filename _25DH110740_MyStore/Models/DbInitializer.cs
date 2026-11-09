using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using _25DH110740_MyStore.Helpers;

namespace _25DH110740_MyStore.Models
{
    // Tự tạo DB và nạp dữ liệu mẫu ở lần chạy đầu tiên
    public class DbInitializer : CreateDatabaseIfNotExists<_25DH110740_MyStoreContext>
    {
        protected override void Seed(_25DH110740_MyStoreContext db)
        {
            db.Users.Add(new User { Username = "admin", PasswordHash = PasswordHelper.Hash("admin123"), Role = "Admin" });
            db.Users.Add(new User { Username = "khach", PasswordHash = PasswordHelper.Hash("123456"), Role = "Customer" });
            db.Customers.Add(new Customer { Username = "khach", CustomerName = "Nguyễn Văn Khách", CustomerEmail = "khach@example.com", CustomerPhone = "0901234567", CustomerAddress = "12 Lê Lợi, Quận 1, TP.HCM" });

            var cats = new[] { "Vợt cầu lông", "Cầu lông", "Giày cầu lông", "Quần áo", "Túi & Balo", "Phụ kiện" }
                .Select(n => new Category { CategoryName = n }).ToList();
            db.Categories.AddRange(cats);
            db.SaveChanges();
            System.Func<string, int> C = n => cats.First(c => c.CategoryName == n).CategoryId;

            var items = new List<Product>
            {
                P(C("Vợt cầu lông"),"Vợt Yonex Astrox 99 Pro","Yonex",3450000,"racket1.svg",45,"Vợt thiên công, đầu nặng, khung Namd + Rotational Generator System giúp đập cầu uy lực. Trọng lượng 4U, cán G5."),
                P(C("Vợt cầu lông"),"Vợt Yonex Nanoflare 800 Pro","Yonex",3290000,"racket2.svg",38,"Vợt thiên tốc độ, đầu nhẹ, phản hồi nhanh cho lối chơi phòng thủ phản công."),
                P(C("Vợt cầu lông"),"Vợt Li-Ning Axforce 80","Li-Ning",2890000,"racket3.svg",30,"Vợt cân bằng công thủ, khung carbon cao cấp, độ bền cao."),
                P(C("Vợt cầu lông"),"Vợt Victor Thruster K 9900","Victor",2750000,"racket1.svg",25,"Vợt đầu nặng dành cho người chơi lực tay tốt, thiên công mạnh mẽ."),
                P(C("Vợt cầu lông"),"Vợt Kumpoo Power Control E70","Kumpoo",1250000,"racket2.svg",60,"Vợt tầm trung dễ chơi, phù hợp người mới và trung cấp."),
                P(C("Vợt cầu lông"),"Vợt Apacs Z Ziggler","Apacs",1590000,"racket3.svg",40,"Vợt nhẹ, linh hoạt, giá tốt cho người chơi phong trào."),
                P(C("Cầu lông"),"Ống cầu lông Yonex AS-50 (12 quả)","Yonex",780000,"shuttle1.svg",120,"Cầu lông lông vịt tiêu chuẩn thi đấu, độ bay ổn định, bền."),
                P(C("Cầu lông"),"Ống cầu Hải Yến S70 (12 quả)","Hải Yến",320000,"shuttle2.svg",200,"Cầu tập luyện phổ thông, giá hợp lý cho câu lạc bộ."),
                P(C("Cầu lông"),"Ống cầu Victor Gold No.1","Victor",690000,"shuttle1.svg",90,"Cầu lông vịt chất lượng cao, độ bền tốt."),
                P(C("Cầu lông"),"Cầu nhựa Lining Training","Li-Ning",150000,"shuttle2.svg",150,"Cầu nhựa bền bỉ, phù hợp tập luyện ngoài trời."),
                P(C("Giày cầu lông"),"Giày Yonex Power Cushion 65Z3","Yonex",2650000,"shoe1.svg",35,"Giày đệm Power Cushion êm, bám sân tốt, hạn chế chấn thương cổ chân."),
                P(C("Giày cầu lông"),"Giày Lining Saga","Li-Ning",1450000,"shoe2.svg",50,"Giày nhẹ, thoáng khí, thiết kế trẻ trung."),
                P(C("Giày cầu lông"),"Giày Kawasaki K-163","Kawasaki",890000,"shoe1.svg",70,"Giày giá mềm, đế cao su chống trơn trượt."),
                P(C("Quần áo"),"Áo cầu lông Yonex Team","Yonex",450000,"shirt1.svg",100,"Áo thun thấm hút mồ hôi, co giãn 4 chiều."),
                P(C("Quần áo"),"Quần cầu lông Lining Pro","Li-Ning",320000,"shirt2.svg",80,"Quần short thể thao nhẹ, có túi, thoải mái khi di chuyển."),
                P(C("Quần áo"),"Váy cầu lông nữ Victor","Victor",420000,"shirt2.svg",55,"Váy kèm quần trong, chất liệu khô nhanh."),
                P(C("Túi & Balo"),"Túi vợt Yonex 6 cây","Yonex",1890000,"bag1.svg",25,"Túi đựng 6 vợt, ngăn giày riêng, chống nước."),
                P(C("Túi & Balo"),"Balo Victor BR3020","Victor",990000,"bag1.svg",40,"Balo gọn nhẹ, 2 ngăn vợt, ngăn laptop."),
                P(C("Túi & Balo"),"Bao vợt Li-Ning 2 ngăn","Li-Ning",590000,"bag1.svg",60,"Bao vợt tiện dụng, đựng 3-4 cây."),
                P(C("Phụ kiện"),"Quấn cán Yonex AC102 (3 cuộn)","Yonex",90000,"grip1.svg",300,"Quấn cán thấm hút mồ hôi, bám tay tốt."),
                P(C("Phụ kiện"),"Dây cước Yonex BG65","Yonex",120000,"grip1.svg",250,"Dây đan vợt bền, cảm giác cầu tốt."),
                P(C("Phụ kiện"),"Băng cổ tay Lining","Li-Ning",60000,"grip1.svg",200,"Băng thấm mồ hôi, co giãn tốt."),
                P(C("Phụ kiện"),"Vớ cầu lông Yonex (3 đôi)","Yonex",180000,"shirt1.svg",150,"Vớ dày đệm gót, thoáng khí."),
                P(C("Phụ kiện"),"Lưới cầu lông di động","Hải Yến",850000,"grip1.svg",20,"Lưới và trụ di động, dễ lắp đặt cho sân tại nhà.")
            };
            var rnd = new System.Random(7);
            foreach (var p in items) p.Sold = rnd.Next(5, 120);
            db.Products.AddRange(items);
            db.SaveChanges();
            base.Seed(db);
        }

        private static Product P(int cat, string name, string brand, decimal price, string img, int stock, string desc)
        {
            return new Product { CategoryId = cat, ProductName = name, Brand = brand, Price = price, ImageUrl = "~/Content/images/products/" + img, Stock = stock, ProductDescription = desc };
        }
    }
}

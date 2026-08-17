using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataAccess.Data
{
    // 1. ApplicationDbContext類別名稱，是自訂的資料庫上下文類別，繼承Entity Framework Core 的 DbContext 父類別
    public class ApplicationDbContext : DbContext
    {
        //2. public ApplicationDbContext 是建構子名稱constructor，方法method要作為「建構子（初始化物件用的函式）」時，名稱必須與類別名稱一樣
        //3. DbContextOptions<ApplicationDbContext>是型別，透過泛型指定專屬型別，讓 DI 容器能精準注入對應的設定物件（防止多 DB 時混淆），變數名取為 options
        // : base(options) 是將設定檔傳給父類別 (DbContext) 去執行底層初始化，所以{}為空
        /* 
        目的：透過依賴注入 (Dependency Injection)，將連線設定與類別解耦，提升程式碼的彈性與可測試性
        */
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // DbSet 的屬性名稱 Categories，就是 SQL 中的 Table 名稱；Category 指的是 Models 裡面的Category.cs 檔案
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        //Data Seeding (資料種子) 是指在應用程式建立資料庫時，自動寫入預設（初始）資料的操作
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //請先把官方原本預設該做的基本功做完，接著再加自己的規則
            //base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Science", DisplayOrder = 2 },
                new Category { Id = 3, Name = "History", DisplayOrder = 3 }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Title = "Store of the Jungle",
                    Author = "Alba Solver",
                    Description = "A thrilling adventure deep in the heart of an uncharted jungle, where ancient secrets and hidden treasures await those brave enough to seek them.",
                    ISBN = "JNG7777770001",
                    ListPrice = 45,
                    Price = 40,
                    Price50 = 35,
                    Price100 = 30,
                    CategoryId = 1
                },
                new Product
                {
                    Id = 2,
                    Title = "Money and Time",
                    Author = "Bob Roarman",
                    Description = "An insightful exploration of wealth, ambition, and the relentless passage of time. A story that challenges what it truly means to be rich.",
                    ISBN = "MNT8888880001",
                    ListPrice = 55,
                    Price = 50,
                    Price50 = 45,
                    Price100 = 40,
                    CategoryId = 3
                },
                new Product
                {
                    Id = 3,
                    Title = "Secret of the Lake",
                    Author = "Jessica Alban",
                    Description = "A mysterious tale set around a remote lake where strange occurrences lead a young detective to uncover a decades-old secret buried beneath the water.",
                    ISBN = "LKE9999990001",
                    ListPrice = 35,
                    Price = 30,
                    Price50 = 28,
                    Price100 = 25,
                    CategoryId = 1
                },
                new Product
                {
                    Id = 4,
                    Title = "Moon and Planets",
                    Author = "Kristen Lober",
                    Description = "A captivating journey through the cosmos, blending science and imagination as humanity reaches beyond the stars for the very first time.",
                    ISBN = "SPC1010100001",
                    ListPrice = 65,
                    Price = 60,
                    Price50 = 55,
                    Price100 = 50,
                    CategoryId = 2
                },
                new Product
                {
                    Id = 5,
                    Title = "Forest and Dawn",
                    Author = "Laura Goldberg",
                    Description = "At the edge of an ancient forest, dawn reveals more than just light. A poetic novel about renewal, loss, and the quiet strength found in nature.",
                    ISBN = "FRD1111110001",
                    ListPrice = 30,
                    Price = 27,
                    Price50 = 24,
                    Price100 = 20,
                    CategoryId = 3
                },
                new Product
                {
                    Id = 6,
                    Title = "The Whispering Grove",
                    Author = "Elara Vance",
                    Description = "Deep within a forgotten grove, the trees hold memories of those who came before. A hauntingly beautiful story of legacy and belonging.",
                    ISBN = "WGR1212120001",
                    ListPrice = 42,
                    Price = 38,
                    Price50 = 34,
                    Price100 = 30,
                    CategoryId = 1
                },
                new Product
                {
                    Id = 7,
                    Title = "The Forgotten Cipher",
                    Author = "Anya Ravenwood",
                    Description = "A brilliant cryptographer stumbles upon an ancient code that could rewrite history. But cracking it means confronting a conspiracy centuries in the making.",
                    ISBN = "CIP1313130001",
                    ListPrice = 50,
                    Price = 45,
                    Price50 = 40,
                    Price100 = 35,
                    CategoryId = 2
                },
                new Product
                {
                    Id = 8,
                    Title = "The Silent Orchard",
                    Author = "Elara Vance",
                    Description = "When silence falls over a once-thriving orchard, one woman returns to her childhood home to uncover the truth behind its abandonment and her family's past.",
                    ISBN = "ORC1414140001",
                    ListPrice = 38,
                    Price = 34,
                    Price50 = 30,
                    Price100 = 26,
                    CategoryId = 3
                }
            );
        }
    }
}
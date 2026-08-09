using Microsoft.EntityFrameworkCore;

namespace BulkyBookWeb.Data
{
    // 1. ApplicationDbContext類別名稱，是自訂的資料庫上下文類別，繼承Entity Framework Core 的 DbContext 父類別
    public class ApplicationDbContext : DbContext
    {
        //2. public ApplicationDbContext 是建構子名稱，當一個方法要作為「建構子（初始化物件用的函式）」時，名稱必須與類別名稱一樣
        //3. DbContextOptions<ApplicationDbContext>是型別，透過泛型指定專屬型別，讓 DI 容器能精準注入對應的設定物件（防止多 DB 時混淆），變數名取為 options
        // : base(options) 是將設定檔傳給父類別 (DbContext) 去執行底層初始化，所以{}為空
        /* 
        目的：透過依賴注入 (Dependency Injection)，將連線設定與類別解耦，提升程式碼的彈性與可測試性
        */
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) {}
    }
}

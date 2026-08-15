using BulkyBook.Business.Services.IServices;
using BulkyBook.Business.Services;
using Microsoft.EntityFrameworkCore;
using BulkyBook.DataAccess.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//幫網站裝上「接待員（Controller）」和「View」，讓網站能夠根據使用者的要求去選取資料並顯示畫面
builder.Services.AddControllersWithViews();

//做DB 的整合與管理
//專門用來簡化讀取 ConnectionStrings 區塊的程式碼
//var test = builder.Configuration.GetConnectionString("SQLConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLConnection"));
});
//向系統註冊 「依賴注入（Dependency Injection, DI）」 的規則
//如果 Controller 或其他地方跟你要 ICategoryService（介面），請你自動建立一個 CategoryService（實體類別）給它
//AddScoped 決定了這個物件要「活多久」。在 Web 開發中，Scoped(生命週期) 代表一次 HTTP 請求（Per HTTP Request），
//當網頁回應（Response）回傳給使用者後，這個實體就會被系統自動銷毀釋放記憶體
builder.Services.AddScoped<ICategoryService, CategoryService>();

//根據設定，把網站應用程式實體(app object)建造出來
var app = builder.Build();

// Configure the HTTP request pipeline.
//pipeline 裡的每一道關卡叫作 Middleware（中間件）。請求會像流水一樣由上至下依次經過每一個關卡。
//比對launchSettings.json裡的「環境變數」設定並做判斷
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //啟用安全的傳輸標頭（HSTS），防止中間人攻擊
    app.UseHsts();
}

//如果使用者輸入 http://，自動將他轉址到安全的 https://
app.UseHttpsRedirection();
//開啟「路由解析」功能，用來分析使用者輸入的網址
app.UseRouting();
//檢查使用者是否有存取該頁面的權限
app.UseAuthorization();

app.MapStaticAssets(); //1. 在應用程式啟動時，系統就會授權伺服器讀取並提供 wwwroot 資料夾裡面的 CSS、JavaScript 與圖片檔案。如果把這行註解/刪除，網頁上的 CSS 樣式與圖片就會全部失效載入不出來。效能比舊版(UseStaticFiles())更換更高、速度更快

app.MapControllerRoute(
    name: "MyArea", //辨識路由的「內部標籤」
    //路由網址的比對篩選器，{area}是系統關鍵字，是一個「佔位符 / 變數，容器」，用來接收網址第一段文字
    // :exists 是將使用者輸入的網址塞進路由規則中的 {area} 變數，確認是否有 Controller 帶有對應的 [Area] 標記
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

//設定當使用者輸入網址時該怎麼對應到後台程式
app.MapControllerRoute(
    name: "default",
    //{id?}問號代表選填，網址後面可以帶 ID 參數（例如文章編號）
    //如果使用者輸入 https://localhost/，系統會自動導向到 `Home` 控制器的 `Index` 畫面（首頁）
    pattern: "{controller=Home}/{action=Index}/{id?}",
    defaults: new { area = "Customer" }) //如果使用者在網址列完全沒有輸入 Area 名稱，請自動把 Area 當作 "Customer"
    .WithStaticAssets(); // 2. 將 Controller 產生的網頁畫面與這些優化過的靜態資產綁定在一起



app.Run();

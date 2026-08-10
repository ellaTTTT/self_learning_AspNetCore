# self_learning_AspNetCore

1. 透過依賴注入（DI）來初始化 Entity Framework Core (EF Core) 的 DbContext
```public class 1ApplicationDbContext : DbContext
{
    public 2ApplicationDbContext(DbContextOptions<3ApplicationDbContext> options): base(options) {}
}
```
宣告 `public class ApplicationDbContext : DbContext`，就像是在學校創立名為「三年 A 班」的班級，並且讓它繼承校規框架（`DbContext`）。而用來記載教室地址的規格文件，型別就叫做 `DbContextOptions`，即所謂的「班級手冊」。

開學當天，三年 A 班導師準備上任並執行初始化手續，這道手續就是第二個 `ApplicationDbContext` 建構子。學校規定這道手續的名稱必須與班級名稱一模一樣，教務處才能辨識這是專屬於三年 A 班的報到流程。

教務處（`Program.cs` 中的 DI 容器）會預先準備好這本手冊，並在封面上特別蓋上「三年 A 班專用」的大字印章，這是第三個 `<ApplicationDbContext>` 泛型標籤的作用。印章非常關鍵，因為教務處的櫃子裡同時存放著全校幾十個班級的手冊，如果封面上沒有蓋上 `<ApplicationDbContext>` 這個專屬標籤，當三年 A 班導師來領取資料時，教務處雖然知道他要拿的是「班級手冊」（`DbContextOptions`），卻無法判斷該把哪一個班級的手冊拿給他。

在執行報到手續的瞬間，教務處比對了封面上的專屬標籤，把那本資料庫地址與密碼的實體手冊，以名為 `options` 的變數遞交到導師手中。導師拿到手冊後，自己完全不需要翻開閱讀或撰寫任何內容，而是直接透過 `: base(options)` 將這本手冊呈交給學校的底層系統( `DbContext`)。學校底層系統接過手冊並讀取裡面的密碼與地址後，便會自動完成教室開門與資料庫連線的所有底層工作。

- 1 與 2 綁定（C# 語法規定）：建構子的名稱（2）必須與類別名稱（1）完全相同，否則 C# 會以為這是一個普通函式而報錯。
- 1 與 3 綁定（.NET 依賴注入規定）：泛型 <> 裡面的型別（3）必須填入這個類別自己（1），這樣系統啟動時，才能拿到專屬於這個類別的連線設定檔。
- 步驟如下
    ```
    1. 安裝 EF Core NuGet 套件-> 安裝環境
    安裝「專案需要的套件/函式庫」。預設的 .NET 專案不包含資料庫驅動，安裝套件就像是幫專案安裝「SQL Server 擴充模組」與「自動化工具」。

    2. 建立 Data Model / Entity (例：Category.cs) -> C# 建立欄位
    資料庫串接主要有兩種開發模式：
    - Code First（程式碼優先 - 目前主流）：
    直接在 C# 寫 Category.cs 類別，EF Core 會根據你的 C# 類別屬性（Property），透過 Migration（移轉指令） 自動幫你在資料庫建立出對應的資料表與欄位。
    - Db First（資料庫優先）：
    先在資料庫把 Table 和欄位手動建好，再用指令反向產出 C# 的 Model 類別。

    在 .NET Core 中，預設最推薦的是 Code First，所以欄位確實可以在 C# 程式碼裡面定義。

    3. 建立 ApplicationDbContext.cs -> ORM，把語法做轉換
    是 ORM 在程式裡的總指揮中心。它不僅把 C# 的 LINQ 語法轉成 SQL 語法，還負責紀錄哪些資料被修改(Change Tracking)，並在執行 SaveChanges() 時一次寫回資料庫。

    4. 在 appsettings.json 設定連線字串 -> 連線到 DB 的設定。

    5. 在 Program.cs 註冊 DbContext 服務 -> 做整合與管理
    利用 .NET 的依賴注入（Dependency Injection, DI）與控制反轉（IoC）機制。這能確保：
    - 指定 ApplicationDbContext 使用的資料庫驅動（如 SQL Server）。
    - 管理生命週期（預設為 Scoped，即每個 HTTP Request 建立一個獨立實例，Request 結束時自動釋放連線資源）

    6. 執行 EF Core 移轉與建立資料庫 (Migrations)：
    在工具 -> NuGet 套件管理員 -> 套件管理器設定(Package Manager Console) 執行：`add-migration AddCategoryToDB` 以及 `update-database`
    執行 Migration 會比對模型變化，自動產生 SQL DDL 腳本並作用於資料庫，實體建立出真正的 Data Table 與 Schema，Add-Migration <名稱>出來的遷移檔會是 日期_名稱.cs
    ```

2. 使用者是與MVC的何者做對接?
    點擊畫面上的按鈕，本質上是向伺服器發送了一封「新郵件（HTTP 請求）」，而伺服器裡專門打開郵件並處理邏輯的人，永遠是 Controller，View（.cshtml）只是一份靜態的 HTML 模板檔案，它在伺服器裡不會自己執行、也不會自己聽網路請求，必須由 Controller 來呼叫它、餵資料給它
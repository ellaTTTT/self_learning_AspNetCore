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
3. MVC 介紹
    M (Model - 模型 / 商業邏輯層) - 
    職責： 負責處理所有與商業邏輯、資料規則、資料存取相關的事情。
    包含： 計算、驗證規則（例如：類別名稱不能重複）、與資料庫互動、第三方 API 接軌等。

    V (View - 視圖 / 呈現層) - 
    職責： 只負責把資料畫成 UI（畫面） 展示給使用者看，不包含任何邏輯。

    C (Controller - 控制器 / 協調層) - 
    職責： 充當交通指揮官（Conductor） 或 接線生。
    它只負責：
    接收來自前端的 HTTP 請求（Request）。
    呼叫商業邏輯層（Model/Service） 來處理資料。
    根據處理結果，決定要回傳哪一個 View 或轉址（Response/Redirect）。

    👉 總結：Controller 應該只做「路由與協調」，而不做「業務計算與邏輯驗證」。
4. 非同步端點（Async Endpoint）
「端點（Endpoint）」在 Web 開發中通常是指一個可以被呼叫的方法或 API 網址。

    - 同步（Synchronous）：點餐後站在櫃檯死等
    當程式去資料庫抓資料時，負責處理請求的執行緒（Thread）會卡在那裡「完全不能動」，必須等資料庫回應才能處理下一件事。這就像你在餐廳點餐後，只能站在櫃檯前面盯著廚師看，不能回座位，也不能接下一個顧客。
    - 非同步（Asynchronous）：點餐後拿到「取餐嗶嗶呼叫器」
    當程式發起資料庫查詢時，執行緒（Thread）會先把這筆查詢交給後台背景去等，自己立刻騰出手去處理其他使用者的請求。等到資料庫抓完資料了，再透過呼叫器通知程式回來把資料拿走。

    在 .NET Web 開發中，幾乎所有涉及「資料庫讀寫」或「跨網路請求」的方法，都強烈建議寫成非同步，伺服器才能同時處理大量使用者。
5. Task 是 C# 內建的一個類別（型態），代表一個「正在進行中或未來才會完成的任務（憑證/呼叫器）」
6. async 跟 await？
    這兩個關鍵字通常成對出現，各自扮演不同的角色：
    - async（標示）： 寫在方法聲明前。它的作用是告訴編譯器：「這個方法內部會包含非同步操作」，並且允許你在方法體內使用 await。它也會自動幫你將回傳值包裝成 Task。
    - await（等待與解包）： 寫在非同步任務的前面。它的作用是「暫停這個方法的執行，並把控制權（執行緒）還給伺服器」。只有「這個動作有真的去跟資料庫（網路/硬碟）連線」才需要寫這個；或是看方法後面有沒有 Async，以及它是不是回傳 Task 就會需要使用 await

    當程式看到 await _context.Categories.ToListAsync(); 時，它會發送 SQL 給資料庫，然後執行緒立刻離開去服務其他使用者。
    等資料庫把資料抓回來時，系統會重新安排一個執行緒回來，自動把 Task<IEnumerable<Category>> 拆封（Unwrap），直接拿到裡面的 IEnumerable<Category> 資料庫結果。

    簡而言之：
    async 是「開關/宣告」，await 是「非同步等待並取出包裹裡的資料」。兩者配合能讓伺服器在等待資料庫時不卡死，大幅提升網站的併發處理能力。
7. 
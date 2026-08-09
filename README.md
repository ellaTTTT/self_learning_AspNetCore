# self_learning_AspNetCore

關於
```public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) {}
}
```
宣告 `public class ApplicationDbContext : DbContext`，就像是在學校創立了一個名為「三年 A 班」的班級，並且讓它繼承校規框架（`DbContext`）。而用來記載教室地址的規格文件，型別就叫做 `DbContextOptions`，即所謂的「班級手冊」。

開學當天，三年 A 班導師準備上任並執行初始化手續，這道手續就是第二個 `ApplicationDbContext` 建構子。學校規定這道手續的名稱必須與班級名稱一模一樣，教務處才能辨識這是專屬於三年 A 班的報到流程。

教務處（`Program.cs` 中的 DI 容器）會預先準備好這本手冊，並在封面上特別蓋上「三年 A 班專用」的大字印章，這是第三個 `<ApplicationDbContext>` 泛型標籤的作用。印章非常關鍵，因為教務處的櫃子裡同時存放著全校幾十個班級的手冊，如果封面上沒有蓋上 `<ApplicationDbContext>` 這個專屬標籤，當三年 A 班導師來領取資料時，教務處雖然知道他要拿的是「班級手冊」（`DbContextOptions`），卻無法判斷該把哪一個班級的手冊拿給他。

在執行報到手續的瞬間，教務處比對了封面上的專屬標籤，精準地把那本寫滿資料庫地址與密碼的實體手冊，以名為 `options` 的變數遞交到導師手中。導師拿到手冊後，自己完全不需要翻開閱讀或撰寫任何內容，而是直接透過 `: base(options)` 將這本手冊呈交給學校的底層系統（父類別 `DbContext`）。學校底層系統接過手冊並讀取裡面的密碼與地址後，便會自動完成教室開門與資料庫連線的所有底層工作。
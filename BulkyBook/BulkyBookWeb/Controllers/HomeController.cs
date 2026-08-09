using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BulkyBookWeb.Controllers
{
    public class HomeController : Controller //繼承 Microsoft.AspNetCore.Mvc (第2行)所提供的 Controller 類別
    {
        //ASP.NET Core 中的一個介面（Interface），這個 HTTP 動作執行完畢後，要回傳給瀏覽器的結果
        //是一個通用的回傳型態，用來規範 Controller 裡面的函式（Method）最後可以「吐」出什麼東西給使用者。
        public IActionResult Index()
        {
            // 自動去找 Views/Home/Index.cshtml 這個網頁畫面檔案並渲染出來
            return View();
        }

        public IActionResult Privacy()
        {
            // 它會自動去找 Views/Home/Privacy.cshtml 這個網頁畫面檔案並渲染出來
            return View();
        }
    }
}

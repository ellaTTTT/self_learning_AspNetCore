using BulkyBookWeb.Data;
using BulkyBookWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Controllers
{
    public class CategoryController : Controller
    {
        //是「欄位」（Field），也就是變數（Variable），因為沒有()，是存放物件的儲存空間（欄位）
        //_context是變數名稱，私有欄位（Private Field）： 慣例以小寫底線開頭
        private readonly ApplicationDbContext _context;

        //context 作用範圍只在這個建構子執行期間，執行完畢後就會消失
        //其中 ApplicationDbContext 是看類別名稱，不是看檔名/物件名稱
        //是接收外部物件並放入空間的入口（建構子），當這個 Controller 被建立時會自動執行的「初始化方法」
        public CategoryController(ApplicationDbContext context)
        {
            _context = context; // 把傳進來的 context 賦值給私有欄位 _context，其型態是 ApplicationDbContext
        }
        public IActionResult Index()
        {
            // _context has-a Categories（分類資料表物件），所以可以透過 _context.Categories.ToList() 來抓取資料
            var categories = _context.Categories.ToList();
            return View("Index", categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        //宣告這個方法只處理 HTTP POST 請求
        //網頁發送請求尋找名為 Create(不區分大小寫)的 Action 時，框架就會自動找到 CreatePost() 這個方法，
        //讓對外的網址依然維持乾淨的 /Controller/Create
        [HttpPost]
        [ValidateAntiForgeryToken] //避免CSRF（Cross-Site Request Forgery，跨站請求偽造）
        [ActionName("Create")]
        //自動把前端傳過來的文字資料打包成一個 C# 的 Category 物件，並傳進 category 這個變數裡
        public IActionResult CreatePost(Category category)
        {
            //c：代表資料庫 Categories 資料表中的每一筆舊資料，category：代表使用者這次剛提交上來的新資料
            //向資料庫詢問「是否有任何Any一筆資料符合括號內的條件(不分大小寫)？」(有就回傳 true，沒有就回傳 false)
            //此時ModelState就會有錯誤值，進而使下方if條件判斷無法通過
            //這是屬於伺服器端的驗證(Server-Side Validation)，是資料的最後一道防線，就算惡意跳過前端頁面，Controller 依然能擋下錯誤資料
            if (!String.IsNullOrEmpty(category.Name) && _context.Categories.Any(c => c.Name.ToLower() == category.Name.ToLower()))
            {
                ModelState.AddModelError("", "Category name already exist!");
            }
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category); //Categories可寫可不寫
                _context.SaveChanges(); // 必須呼叫這行 DB 才會執行 INSERT 指令
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index"); //原return RedirectToAction("Index", "Category"); 但因為同controller所以可以省略後者

            }
            return View();
        }

        //根據主鍵(Primary Key / ID)，抓出特定資料
        public IActionResult Update(int? id)
        {
            if(id==0 || id==null)
            {
                return NotFound();
            }

            var category = _context.Categories.Find(id);
            if(category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Update")]
        public IActionResult UpdatePost(Category category)
        {
            if (!String.IsNullOrEmpty(category.Name) && 
                _context.Categories.Any(c => c.Name.ToLower() == category.Name.ToLower() && c.Id != category.Id))
            {
                ModelState.AddModelError("", "Category name already exist!");
            }
            if (ModelState.IsValid)
            {
                _context.Categories.Update(category);
                _context.SaveChanges();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");

            }
            return View();
        }


        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }

            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        //遵循最小知識原則（Principle of Least Knowledge）：刪除只需要知道 ID，後端就不應該要求呼叫者提供比 ID 更多的資訊。
        public IActionResult DeletePost(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            _context.Categories.Remove(category);
            _context.SaveChanges();
            /* 
            回列表頁時，在前端畫面上顯示一次性的成功提示訊息（Alert / Toast 畫面通知）
            ViewBag 與 ViewData 的生命週期只限於「同一次 HTTP 請求」。一旦發生了 Redirect（瀏覽器發送新的請求），ViewBag 裡面的資料就會全部消失。
            只有 TempData 能把「 Category created successfully 」這段文字，跨越 Redirect 帶到下一個頁面（Index 頁面）。 
            */
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
    }
}

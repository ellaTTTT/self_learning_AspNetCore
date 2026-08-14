using BulkyBook.Business.Services.IServices;
using BulkyBook.DataAccess.Data;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

//變動前，CategoryController 直接相依於 ApplicationDbContext，控制器內部佈滿了 Entity Framework Core 的資料庫指令（如 _context.Categories.ToList() 或 _context.SaveChanges()）。
//這意味著 Controller 不僅要處理網頁的 HTTP 請求、頁面跳轉與模型驗證，還要一手包辦資料庫的查詢與寫入細節，導致 Controller 的職責過於龐大且與資料庫底層高度綁定。
//變動之後獨立出了業務邏輯層，建立了 CategoryService 與其介面 ICategoryService。現在 Controller 改為注入 ICategoryService 介面，只需呼叫 _categoryService.GetAllCategoriesAsync() 這類方法，便能將所有資料讀寫與業務規則完全外包給 CategoryService 處理
//同時也在 Program.cs 透過 builder.Services.AddScoped<ICategoryService, CategoryService>() 完成服務註冊，交由系統自動管理物件的生命週期。

namespace BulkyBookWeb.Controllers
{
    public class CategoryController : Controller
    {
        //是「欄位」（Field），也就是變數（Variable），因為沒有()，是存放物件的儲存空間（欄位）
        //_categoryService是變數名稱，私有欄位（Private Field）： 慣例以小寫底線開頭
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        public async Task<IActionResult> Index()
        {
            //呼叫 Service 提供的非同步方法，Controller 完全不知道資料庫怎麼實作的
            var categories = _categoryService.GetAllCategoriesAsync();
            return View("Index", categories);
        }

        public async Task<IActionResult> Create()
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
        public async Task<IActionResult> CreatePost(Category category)
        {
            //c：代表資料庫 Categories 資料表中的每一筆舊資料，category：代表使用者這次剛提交上來的新資料
            //向資料庫詢問「是否有任何Any一筆資料符合括號內的條件(不分大小寫)？」(有就回傳 true，沒有就回傳 false)
            //此時ModelState就會有錯誤值，進而使下方if條件判斷無法通過
            //這是屬於伺服器端的驗證(Server-Side Validation)，是資料的最後一道防線，就算惡意跳過前端頁面，Controller 依然能擋下錯誤資料
            if (!String.IsNullOrEmpty(category.Name) && await _categoryService.IsCategoryNameUniqueAsync(category.Name))
            {
                ModelState.AddModelError("", "Category name already exist!");
            }
            if (ModelState.IsValid)
            {
                //_context.Categories.Add(category); //Categories可寫可不寫
                //_context.SaveChanges(); // 必須呼叫這行 DB 才會執行 INSERT 指令
                await _categoryService.CreateCategoryAsync(category);
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index"); //原return RedirectToAction("Index", "Category"); 但因為同controller所以可以省略後者

            }
            return View();
        }

        //根據主鍵(Primary Key / ID)，抓出特定資料
        public async Task<IActionResult> Update(int? id)
        {
            if(id==0 || id==null)
            {
                return NotFound();
            }

            var category = _categoryService.GetCategoryByIdAsync(id.Value);
            if(category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Update")]
        public async Task<IActionResult> UpdatePost(Category category)
        {
            if (!String.IsNullOrEmpty(category.Name) && 
               await _categoryService.IsCategoryNameUniqueAsync(category.Name, category.Id))
            {
                ModelState.AddModelError("", "Category name already exist!");
            }
            if (ModelState.IsValid)
            {
                await _categoryService.UpdateCategoryAsync(category);
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");

            }
            return View();
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }

            var category = _categoryService.GetCategoryByIdAsync(id.Value);
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
        public async Task<IActionResult> DeletePost(int id)
        {
            await _categoryService.DeleteCategoryAsync(id);
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

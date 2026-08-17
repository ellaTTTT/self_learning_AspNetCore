using BulkyBook.Business.Services.IServices;
using BulkyBook.Models;
using BulkyBook.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;

//變動前，ProductController 直接相依於 ApplicationDbContext，控制器內部佈滿了 Entity Framework Core 的資料庫指令（如 _context.Products.ToList() 或 _context.SaveChanges()）。
//這意味著 Controller 不僅要處理網頁的 HTTP 請求、頁面跳轉與模型驗證，還要一手包辦資料庫的查詢與寫入細節，導致 Controller 的職責過於龐大且與資料庫底層高度綁定。
//變動之後獨立出了業務邏輯層，建立了 ProductService 與其介面 IProductService。現在 Controller 改為注入 IProductService 介面，只需呼叫 _productService.GetAllProductsAsync() 這類方法，便能將所有資料讀寫與業務規則完全外包給 ProductService 處理
//同時也在 Program.cs 透過 builder.Services.AddScoped<IProductService, ProductService>() 完成服務註冊，交由系統自動管理物件的生命週期。

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductController : Controller
    {
        //是「欄位」（Field），也就是變數（Variable），因為沒有()，是存放物件的儲存空間（欄位）
        //_productService是變數名稱，私有欄位（Private Field）： 慣例以小寫底線開頭
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<IActionResult> Index()
        {
            //呼叫 Service 提供的非同步方法，Controller 完全不知道資料庫怎麼實作的
            //var products = await _productService.GetAllProductsAsync();
            return View("Index");
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
        //自動把前端傳過來的文字資料打包成一個 C# 的 Product 物件，並傳進 product 這個變數裡
        public async Task<IActionResult> CreatePOST(Product product)
        {
            if (ModelState.IsValid)
            {
                //_context.Products.Add(product); //Products可寫可不寫
                //_context.SaveChanges(); // 必須呼叫這行 DB 才會執行 INSERT 指令
                await _productService.CreateProductAsync(product);
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index"); //原return RedirectToAction("Index", "Product"); 但因為同controller所以可以省略後者

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

            var product = await _productService.GetProductByIdAsync(id.Value);
            if(product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Update")]
        public async Task<IActionResult> UpdatePOST(Product product)
        {
            if (ModelState.IsValid)
            {
                await _productService.UpdateProductAsync(product);
                TempData["success"] = "Product updated successfully";
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

            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        //遵循最小知識原則（Principle of Least Knowledge）：刪除只需要知道 ID，後端就不應該要求呼叫者提供比 ID 更多的資訊。
        public async Task<IActionResult> DeletePOST(int id)
        {
            await _productService.DeleteProductAsync(id);
            /* 
            回列表頁時，在前端畫面上顯示一次性的成功提示訊息（Alert / Toast 畫面通知）
            ViewBag 與 ViewData 的生命週期只限於「同一次 HTTP 請求」。一旦發生了 Redirect（瀏覽器發送新的請求），ViewBag 裡面的資料就會全部消失。
            只有 TempData 能把「 Product created successfully 」這段文字，跨越 Redirect 帶到下一個頁面（Index 頁面）。 
            */
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction("Index");
        }

        #region API CALLS
        public async Task<IActionResult> GetAll()
        {
            // 1. 非同步呼叫 Business 層的 ProductService，從資料庫取出所有產品清單
            // 傳入 true，代表向 Service 要求「產品 + 分類」的完整資料
            var products = await _productService.GetAllProductsAsync(true);
            // 2. 將查詢到的資料包裝成 JSON 格式並回傳給前端（HTTP 200）
            // 不是用來跳轉畫面的 Action，而是一個 Web API 端點，專門提供資料給前端的 JavaScript 讀取
            return Json(new {data=products});
        }
        #endregion
    }
}
using BulkyBookWeb.Data;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Controllers
{
    public class CategoryController : Controller
    {
        //是「欄位」（Field），也就是變數（Variable），因為沒有()，_context是變數名稱
        //私有欄位（Private Field）： 慣例以小寫底線開頭
        private readonly ApplicationDbContext _context;

        //context 作用範圍只在這個建構子執行期間，執行完畢後就會消失
        //ApplicationDbContext 是看類別名稱，不是看檔名/物件名稱
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
    }
}

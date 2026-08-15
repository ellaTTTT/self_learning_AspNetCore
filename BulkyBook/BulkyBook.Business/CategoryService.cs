using BulkyBook.Business.Services.IServices;
using BulkyBook.DataAccess.Data;
using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.Business.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        // async 跟 await 通常是成對出現的，各自扮演不同的角色
        // async 寫在方法聲明前。它的作用是告訴編譯器：「這個方法內部會包含非同步操作」，並且允許你在方法內使用 await。它也會自動幫你將回傳值包裝成 Task
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            //await （等待與解包）暫停這個方法的執行，並把控制權（執行緒）還給伺服器，會寫在「非同步動作（Async Operation）」前面，例如下方的ToListAsync()
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            //下方的指令會做打包發送給 SQL Server 資料庫，並等待資料庫處理完畢回傳結果涉及網路傳輸與硬碟讀寫，
            //是需要非同步的地方，因此需要寫await
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task DeleteCategoryAsync(int id) {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category {id} not found.");
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        //當你在介面 ICategoryService 增加了一個新方法合約時，實作它的 CategoryService 就強制必須要寫出這個方法，否則程式無法編譯。
        //throw new NotImplementedException()（拋出尚未實作例外），只是 VS 自動產生的「佔位符」，代表已經把這個方法掛上去了，但等下才要來寫裡面的邏輯（SQL 查詢）
        //是一個非同步方法，執行完畢後會回傳一個布林值（true 表示名稱唯一、可用；false 表示名稱重複、不可用）
        public async Task<bool> IsCategoryNameUniqueAsync(string name, int? categoryId = null)
        {
            if (categoryId.HasValue)
            {
                // true修改既有分類(Update)，c.Id != categoryId.Value 是為了排除自己，是否有「名稱相同」，且「ID 不是自己」的資料
                return !await _context.Categories.AnyAsync(c => c.Name.ToLower() == name.ToLower() && c.Id != categoryId.Value);
            }
            else // false新增分類(Create)
            {
                // 有沒有任何一筆資料的名稱跟新名稱一樣，有的話就回傳true，但經過反向會得到false
                return !await _context.Categories.AnyAsync(c => c.Name.ToLower() == name.ToLower());
            }
        }
    }
}
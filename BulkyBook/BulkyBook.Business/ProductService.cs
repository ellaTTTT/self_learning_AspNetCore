using BulkyBook.Business.Services.IServices;
using BulkyBook.DataAccess.Data;
using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.Business.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        // async 跟 await 通常是成對出現的，各自扮演不同的角色
        // async 寫在方法聲明前。它的作用是告訴編譯器：「這個方法內部會包含非同步操作」，並且允許你在方法內使用 await。它也會自動幫你將回傳值包裝成 Task
        public async Task<IEnumerable<Product>> GetAllProductsAsync(bool includeCategory=false)
        {
            if(includeCategory)
            {
                // 貪婪載入（Eager Loading）：透過 SQL JOIN 將關聯的 Category 資料表一起抓出來
                return await _context.Products.Include(u => u.Category).ToListAsync();
            }
            else
            {
                //await （等待與解包）暫停這個方法的執行，並把控制權（執行緒）還給伺服器，
                //會寫在「非同步動作（Async Operation）」前面，例如下方的ToListAsync()
                // 只抓 Products 資料表本身的欄位，Category 物件會是 null
                return await _context.Products.ToListAsync();

            }
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            _context.Products.Add(product);
            //下方的指令會做打包發送給 SQL Server 資料庫，並等待資料庫處理完畢回傳結果涉及網路傳輸與硬碟讀寫，
            //是需要非同步的地方，因此需要寫await
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task DeleteProductAsync(int id) {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product {id} not found.");
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
    }
}
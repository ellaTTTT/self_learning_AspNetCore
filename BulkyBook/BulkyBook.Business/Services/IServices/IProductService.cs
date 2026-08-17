using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.Business.Services.IServices
{
    public interface IProductService
    {
        // Task 是 C# 內建的一個類別（型態），代表一個「正在進行中或未來才會完成的任務（憑證/呼叫器）」
        // Task 表示非同步：先給你一張取餐憑證，等等會兌換成 Product，但也有可能是 null(有?代表的意思)
        // 不寫 Task 表示同步：立刻就要給我一份實體的
        Task<Product?> GetProductByIdAsync(int id);

        // IEnumerable<Product> 是一串 Product 物件的集合，只允許呼叫者讀取（跑迴圈），不允許呼叫者隨便對這串資料進行新增或刪除
        Task<IEnumerable<Product>> GetAllProductsAsync(bool includeCategory=false);
        Task<Product> CreateProductAsync(Product product);

        // 只負責去執行動作，不需要拿回任何資料。
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
    }
}

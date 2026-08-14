using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.Business.Services.IServices
{
    public interface ICategoryService
    {
        // Task 是 C# 內建的一個類別（型態），代表一個「正在進行中或未來才會完成的任務（憑證/呼叫器）」
        // Task 表示非同步：先給你一張取餐憑證，等等會兌換成 Category，但也有可能是 null(有?代表的意思)
        // 不寫 Task 表示同步：立刻就要給我一份實體的
        Task<Category?> GetCategoryByIdAsync(int id);

        // IEnumerable<Category> 是一串 Category 物件的集合，只允許呼叫者讀取（跑迴圈），不允許呼叫者隨便對這串資料進行新增或刪除
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> CreateCategoryAsync(Category category);

        // 只負責去執行動作，不需要拿回任何資料。
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(int id);

        Task<bool> IsCategoryNameUniqueAsync(string name, int? categoryId = null);
    }
}

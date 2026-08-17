using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BulkyBook.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public string ISBN { get; set; } = string.Empty;

        [Required]
        public string Author { get; set ; } = string.Empty;

        [Required]
        [Display(Name = "List Price")]
        [Range(1, 1000)]
        public double ListPrice { get; set; }  //因為double跟int, bool, char等都是實質型別，預設就有初始值，所以不需要有string.Empty;

        [Required]
        [Display(Name = "Price for 1-50")]
        [Range(1, 1000)]
        public double Price { get; set; }

        [Required]
        [Display(Name = "Price for 50+")]
        [Range(1, 1000)]
        public double Price50 { get; set; }

        [Required]
        [Display(Name = "Price for 100+")]
        [Range(1, 1000)]
        public double Price100 { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]// 指向外鍵屬性名稱
        //導覽屬性 (Navigation Property)，是物件導向關聯，讓你不必手動寫 SQL JOIN，但資料庫裡不存在此欄位
        //告訴 EF底下這個 Category 物件，請用上面那個 CategoryId 欄位當作外鍵來對應
        public Category Category { get; set; }

        [ValidateNever]
        [Display(Name = "Product Image")]
        public string? ImageUrl { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Models
{
    // 這是用來描述「Category」的資料模型，會對應到(conver to)資料庫中的一個資料表/欄位。
    public class Category
    {
        //在 C# 網頁開發中，大部分工具和框架只認「屬性property（有 { get; set; }）」，不認普通變數。
        //資料庫在讀寫資料時，需要透過 { get; set; } 來作為資料傳輸的橋樑。
        //開放讓外部程式讀取（get）與修改（set）
        /*
        又稱為自動實作屬性(Auto-Implemented Property)，只要寫 { get; set; }（或 { get; }），它就是 Property
        public int Id { get; private set; }表示外部可以 get（讀取）但只有 Category 自己內部可以 set（修改） 
        */
        //[Key] //表示這個欄位是資料表的主鍵，是一個state annotation(狀態註解)，用於 primary key 的名稱不是 Id 時
        public int Id { get; set; } //建立一個property

        [Required]
        [StringLength(100)]
        [Display(Name = "Categoty Name")]
        public string Name { get; set; } = string.Empty; //表示 Name 預設值為空字串，避免 null 的問題(防止「剛建立好、但還沒給值」的空檔)

        [Display(Name = "Display Order")]
        [ValidateNever] //在模型驗證時略過該欄位
        [Range(0, 100, ErrorMessage ="Range must be between 0 and 100!")]
        public int? DisplayOrder { get; set; }
    }
}

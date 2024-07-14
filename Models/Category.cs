using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PieShop.Models
{
    public class Category
    {
        [DisplayName("Category Id")]
        public int CategoryId { get; set; }
        [DisplayName("Category Name")]
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<Item>? Items { get; set; }
    }
}

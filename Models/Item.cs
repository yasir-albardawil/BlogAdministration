using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PieShop.Models
{
    public class Item
    {
        [Key]
        
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [DisplayName("Short Description")]
        public string? ShortDescription { get; set; }
        [Required]
        [DisplayName("Long Description")]
        public string? LongDescription { get; set; }
        [DisplayName("Allergy Information")]
        public string? AllergyInformation { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        [DisplayName("Image Url")]
        public string? ImageUrl { get; set; }
        [Required]
        [DisplayName("Image Thumbnai lUrl")]
        public string? ImageThumbnailUrl { get; set; }
        [DisplayName("Is Item Of The Week")]
        public bool IsPieOfTheWeek { get; set; }
        [DisplayName("In Stock")]
        public bool InStock { get; set; }

        [DisplayName("Category Id")]
        public int CategoryId { get; set; }
        [Required]
        public Category Category { get; set; } = default!;
    }
}
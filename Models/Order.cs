using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PieShop.Models
{
    public class Order
    {

        [BindNever]
        [DisplayName("Order Id")]
        public int OrderId { get; set; }
        public List<OrderDetail>? OrderDetails { get; set; }
        [Required(ErrorMessage = "First name shuld not be empty")]
        [DisplayName("First Name")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [DisplayName("Last Name")]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;
        [Required]
        [DisplayName("Address Line 1")]
        [StringLength(100)]
        public string AddressLine1 { get; set; } = string.Empty;
        [DisplayName("Address Line 2")]

        public string? AddressLine2 { get; set; }
        [DisplayName("Zip Code")]
        public string ZipCode { get; set; } = string.Empty;
        [Required]
        [DisplayName("City")]
        public string City { get; set; } = string.Empty;
        [DisplayName("State")]
        public string? State { get; set; }
        [DisplayName("Country")]
        public string Country { get; set; } = string.Empty;
        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required]
        [DisplayName("Email")]
        [RegularExpression(@"(?:[a-z0-9!#$%&'*+/=?^_{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])", 
            ErrorMessage = "The email address is not entered in a correct format")]
            public string Email { get; set; } = string.Empty;

        [BindNever]
        [DisplayName("Order Total")]

        public decimal OrderTotal { get; set; }
        [BindNever]
        [DisplayName("Order Placed")]
        public DateTime OrderPlaced { get; set; }
    }
}

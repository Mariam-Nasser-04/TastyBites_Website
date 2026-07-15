using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartResturant.Models
{
    public class Item
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "You have to provide a valid name")]
        [MinLength(5, ErrorMessage = "Name mustn't be less than 5 characters.")]
        [MaxLength(50, ErrorMessage = "Name mustn't exceed 50 characters.")]
        [DisplayName("Item Name")]
        public string Name { get; set; }
        [Range(1, 10000, ErrorMessage = "Salary must be between 1 EGP and 10000 EGP.")]
        public decimal Price { get; set; }

        public string? ImageName { get; set; }

        [NotMapped]
        [Display(Name = "Upload Image")]
        public IFormFile? ImageFile { get; set; }

        [DisplayName("Category")]
        [Range(1, int.MaxValue, ErrorMessage = "Choose a valid Category.")]
        public int CategoryId { get; set; }

        [ValidateNever]
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Category Category { get; set; }


    }
}

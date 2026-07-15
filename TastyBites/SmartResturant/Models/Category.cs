using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartResturant.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "You have to provide a valid name")]
        [MinLength(5, ErrorMessage = "Full name mustn't be less than 5 characters.")]
        [MaxLength(50, ErrorMessage = "Full name mustn't exceed 50 characters.")]
        [DisplayName("Category Name")]
        public string Name { get; set; }

        public string? ImageName { get; set; }

        [NotMapped]
        [Display(Name = "Upload Image")]
        public IFormFile? ImageFile { get; set; }

        //Navigation Property
        [ValidateNever]
        public List<Item> Items { get; set; }


    }
}

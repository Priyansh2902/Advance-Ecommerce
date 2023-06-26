using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BulkyWebRazor_Temp.Models
{
    public class Category
    {
       
            [Key]
            public int Id { get; set; }

            [Required]
            [MaxLength(50)]
            [DisplayName("Category Name")]
            public string Name { get; set; }

            [DisplayName("Display Order")]
            [Range(1, 200, ErrorMessage = "Display Order Must be Between 1-200!")]
            public int DisplayOrder { get; set; }


        
    }
}

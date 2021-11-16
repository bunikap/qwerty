using System.ComponentModel.DataAnnotations;
namespace qwerty.Models
{
    public class Permission
    {
        public int Id { get; set; }


        [Required]
        // [StringLength(50)]
        public string permission { get; set; }
         public int visible{get;set;}
        // [NotMapped]
        // public int num{get;set;}
    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
namespace qwerty.Models
{
    public class Permission
    {
        public int Id { get; set; }


        [Required]
        // [StringLength(50)]
        public string permission { get; set; }
        // [NotMapped]
        // public int num{get;set;}
    }
}


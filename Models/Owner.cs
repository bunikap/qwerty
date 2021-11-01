using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace qwerty.Models
{
    public class Owner
    {

        public int Id { get; set; }
        [Display(Name = "User")]
        public string own { get; set; }
        public string Department { get; set; }

        public int PermissionId { get; set; }
        [Display(Name = "Permission")]

        public Permission Permissionn { get; set; }

        // [NotMapped]
        // public ICollection<CheckBoxListItem> GetCheckBoxListItem { get; set; }


    }

}
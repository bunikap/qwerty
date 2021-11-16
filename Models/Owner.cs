using System.ComponentModel.DataAnnotations;

namespace qwerty.Models
{
    public class Owner
    {

        public int Id { get; set; }
        [Display(Name = "User")]
        public string own { get; set; }

        public int DepartmentId{get;set;}
          [Display(Name = "Department")]
        public Department Departmentt { get; set; }

        public int PermissionId { get; set; }
        [Display(Name = "Permission")]

        public Permission Permissionn { get; set; }

        public int visible{get;set;}

        // [NotMapped]
        // public ICollection<CheckBoxListItem> GetCheckBoxListItem { get; set; }


    }

}
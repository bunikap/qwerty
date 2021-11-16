using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace qwerty.Models
{
    public class UserPer
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public Owner Ownerss { get; set; }

        public int PermissionsId { get; set; }
        public Permission Permissions { get; set; }
        [NotMapped]
        public IList<string> SelectedPermission { get; set; }
        [NotMapped]
        public IList<SelectListItem> AvailablePermission { get; set; }
        [NotMapped]
        public IEnumerable<string> name { get; set; }
        [NotMapped]
        public IEnumerable<string> namePermission { get; set; }
         public int visible{get;set;}


        public UserPer()
        {
            SelectedPermission = new List<string>();
            AvailablePermission = new List<SelectListItem>();
        }
    }
}
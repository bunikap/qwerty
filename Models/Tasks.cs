using System;
using System.ComponentModel.DataAnnotations;
namespace qwerty.Models
{
    public class Tasks
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }
        public string Detail { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime SDate { get; set; }

        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime DDate { get; set; }

        public int OwnersId { get; set; }


        public Owner Owners { get; set; }

        public int ApproveId { get; set; }


        public Owner Approve { get; set; }




        [Display(Name = "Status")]
        public int StatusId { get; set; }




        [Display(Name = "Status")]
        public Status stat { get; set; }

        public int visible { get; set; }



    }
}


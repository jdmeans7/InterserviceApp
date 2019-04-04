using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterserviceApp.Models
{
    public class is_StaffClass
    {
        [Key]
        [Required]
        public int id { get; set; }

        [Required]
        [Display(Name = "Badge ID")]
        public int badgeID { get; set; }
        public is_staffDetails Staff { get; set; }

        [Required]
        [Display(Name = "Class ID")]
        public int classID { get; set; }
        public is_Class Class { get; set; }

        [Display(Name = "Approved")]
        public Boolean approved { get; set; }

        [Required]
        [Display(Name = "End Date")]
        public DateTime endDate { get; set; }

        [Required]
        [Display(Name = "Status")]
        public Boolean status { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterserviceApp.Models
{
    public class is_Class
    {
        [Key]
        [Required]
        public int classID { get; set; }

        [Display(Name = "Date", Description = "Date of Class")]
        [Required]
        public DateTime date { get; set; }
        [Required]

        [Display(Name = "Start Time")]
        public TimeSpan? startTime { get; set; }

        [Display(Name = "Room")]
        [Required]
        public string room { get; set; }
        [Required]

        [Display(Name = "Capacity")]
        public string capacity { get; set; }

        [Display(Name = "Justification")]
        [Required]
        public string justification { get; set; }
        [Required]

        [Display(Name = "Fees")]
        public double fees { get; set; }

        public int courseID { get; set; }
        public virtual is_Course Course { get; set; }

        public virtual ICollection<is_StaffClass> staffClasses { get; set; }
    }
}
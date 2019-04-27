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

        //[Required]
        [Display(Name = "Date", Description = "Date of Class")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-dd-yyyy}")]
        public DateTime date { get; set; }

        //[Required]
        [Display(Name = "Start Time")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = @"{0:hh\:mm}")]
        public TimeSpan? startTime { get; set; }

        //[Required]
        [Display(Name = "Room")]
        public string room { get; set; }

        //[Required]
        [Display(Name = "Capacity")]
        public string capacity { get; set; }

        //[Required]
        [Display(Name = "Justification")]
        public string justification { get; set; }

        //[Required]
        [Display(Name = "Fees")]
        public double fees { get; set; }

        [Display(Name = "Approved")]
        public bool? approved { get; set; }

        [Display(Name = "Hyperlink")]
        public string hyperlink { get; set; }

        public bool? blackboard { get; set; }

        public int courseID { get; set; }
        public virtual is_Course Course { get; set; }

        public virtual ICollection<is_StaffClass> staffClasses { get; set; }
    }
}
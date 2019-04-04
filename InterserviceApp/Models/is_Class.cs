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
        public int classID { get; set; }

        [Display(Name = "Date", Description = "Date of Class")]
        public DateTime date { get; set; }

        [Display(Name = "Start Time")]
        public TimeSpan? startTime { get; set; }

        [Display(Name = "Room")]
        public string room { get; set; }

        [Display(Name = "Capacity")]
        public string capacity { get; set; }

        [Display(Name = "Justification")]
        public string justification { get; set; }

        [Display(Name = "Fees")]
        public double fees { get; set; }

        public int courseID { get; set; }
        public virtual is_Course Course { get; set; }

        public virtual ICollection<is_StaffClass> staffClasses { get; set; }
    }
}
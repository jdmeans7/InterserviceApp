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
        [Required]
        public DateTime date { get; set; }
        [Required]
        public TimeSpan? startTime { get; set; }
        [Required]
        public string room { get; set; }
        [Required]
        public string capacity { get; set; }
        [Required]
        public string justification { get; set; }
        [Required]
        public double fees { get; set; }

        public int courseID { get; set; }
        public virtual is_Course Course { get; set; }

        public virtual ICollection<is_StaffClass> staffClasses { get; set; }
    }
}
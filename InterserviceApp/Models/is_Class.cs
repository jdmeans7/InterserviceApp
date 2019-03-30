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
        public DateTime date { get; set; }
        public TimeSpan? startTime { get; set; }
        public string room { get; set; }
        public string capacity { get; set; }
        public string justification { get; set; }
        public double fees { get; set; }

        public int courseID { get; set; }
        public virtual is_Course Course { get; set; }

        public virtual ICollection<is_StaffClass> staffClasses { get; set; }
    }
}
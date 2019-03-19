using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterserviceApp.Models
{
    public class is_Course
    {
        [Key]
        public int courseID { get; set; }
        public string courseCode { get; set; }
        public string desc { get; set; }

        public ICollection<is_Class> Classes { get; set; }
    }
}
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
        [Required]
        public int courseID { get; set; }
        [Required]
        [Range(0,10000)]
        public string courseCode { get; set; }
        [Required]
        public string desc { get; set; }
        public bool required { get; set; }
        public ICollection<is_Class> Classes { get; set; }
    }
}
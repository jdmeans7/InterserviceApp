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
        public int badgeID { get; set; }
        [Required]
        public is_staffDetails Staff { get; set; }

        [Required]
        public int classID { get; set; }
        public is_Class Class { get; set; }

        
        public Boolean approved { get; set; }
        [Required]
        public DateTime endDate { get; set; }
        [Required]
        public Boolean status { get; set; }
    }
}
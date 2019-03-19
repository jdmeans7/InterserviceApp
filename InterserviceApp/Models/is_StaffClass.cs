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
        public int id { get; set; }

        
        public int badgeID { get; set; }
        public is_staffDetails Staff { get; set; }

        public int classID { get; set; }
        public is_Class Class { get; set; }

        public Boolean approved { get; set; }
        public DateTime endDate { get; set; }
        public Boolean status { get; set; }
    }
}
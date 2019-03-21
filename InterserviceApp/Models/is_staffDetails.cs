using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterserviceApp.Models
{
    public class is_staffDetails
    {
        [Key]
        public int detailsID { get; set; }
        public int badgeID { get; set; }
        public string fName { get; set; }
        public string lName { get; set; }
        public string dept { get; set; }
        public string phone { get; set; }

        [DisplayFormat(DataFormatString ="{0:d}")]
        public DateTime birthdate { get; set; }

        public ICollection<is_StaffClass> staffClasses { get; set; }
    }
}
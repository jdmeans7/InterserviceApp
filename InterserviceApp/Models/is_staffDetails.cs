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

        //public int detailsID { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int badgeID { get; set; }
        [Required]
        public string fName { get; set; }
        [Required]
        public string lName { get; set; }
        [Required]
        public string dept { get; set; }
        [Required]
        [Phone]
        public string phone { get; set; }

        [Required]
        [DisplayFormat(DataFormatString ="{0:d}")]
        public DateTime birthdate { get; set; }

        public ICollection<is_StaffClass> staffClasses { get; set; }
    }
}
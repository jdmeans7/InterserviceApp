﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterserviceApp.Models
{
    public class is_staffDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] //Stops auto-increment
        [Display(Name = "Badge ID")]
        public int badgeID { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string fName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string lName { get; set; }

        public string email { get; set; }

        [Required]
        [Display(Name = "Department")]
        public string dept { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string phone { get; set; }

        [Required]
        [Display(Name = "Birthdate")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime birthdate { get; set; }

        public bool? supervisor { get; set; }

        public bool? flag { get; set; }

        public ICollection<is_StaffClass> staffClasses { get; set; }
    }
}
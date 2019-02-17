namespace InterserviceApp.Models
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class InterserviceModels : DbContext
    {
        // Your context has been configured to use a 'InterserviceModels' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'InterserviceApp.Models.InterserviceModels' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'InterserviceModels' 
        // connection string in the application configuration file.
        public InterserviceModels()
            : base("name=InterserviceModels")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<is_staffDetails> is_staffDetails { get; set; }
        public virtual DbSet<is_Class> Classes { get; set; }
        public virtual DbSet<is_Course> Courses { get; set; }
        public virtual DbSet<is_StaffClass> StaffClasses { get; set; }
    }

    public class is_staffDetails
    {
        [Key]
        public int detailsID { get; set; }
        public int badgeID { get; set; }
        public string fName { get; set; }
        public string lName { get; set; }
        public string dept { get; set; }
        public string phone { get; set; }
        public DateTime birthdate { get; set; }
    }

    public class is_Class
    {
        [Key]
        public int classID { get; set; }
        public DateTime date { get; set; }
        public string startTime { get; set; }
        public string room { get; set; }
        public string capacity { get; set; }
        public string justification { get; set; }
        public string fees { get; set; }

        public int courseID { get; set; }
        public virtual is_Course Courses { get; set; }
    }

    public class is_Course
    {
        [Key]
        public int courseID { get; set; }
        public string courseCode { get; set; }
        public string desc { get; set; }
    }

    public class is_StaffClass
    {
        [Key]
        public int badgeID { get; set; }
        public int classID { get; set; }
        public Boolean approved { get; set; }
        public DateTime endDate { get; set; }
        public Boolean status { get; set; }
    }
}
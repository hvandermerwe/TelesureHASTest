using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TelesureHASTest.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }

        [Required]
        public string EmployeeNo { get; set; }

        [Required]
        public Person Person { get; set; }
    }
}

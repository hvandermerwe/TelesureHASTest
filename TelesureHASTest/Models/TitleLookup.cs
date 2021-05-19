using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TelesureHASTest.Models
{
    public class TitleLookup
    {
        [Key]
        public int TitleID { get; set; }

        [Required]
        public string Name { get; set; }
    }
}

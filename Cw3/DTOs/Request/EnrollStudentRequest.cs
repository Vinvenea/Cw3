using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.DTOs.Request
{
    public class EnrollStudentRequest
    {
        [RegularExpression("^s[0-9]+$")]
        public string IndexNumber { get; set; }
        [Required(ErrorMessage = "Musisz podac imie")]
        [MaxLength(10)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(225)]
        public string LastName { get; set; }

        public string Birthdate { get; set; }
        [Required]
        public string Studies { get; set; }
    }
}

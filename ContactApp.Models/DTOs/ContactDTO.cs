using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactAppNLayer.Models.DTOs
{
    public class ContactDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Fullname is required")]
        [StringLength(100, ErrorMessage = "Fullname cannot exceed 100 characters")]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Range(1000000000, 9999999999, ErrorMessage = "Phone number must be 10 digits")]
        public long Phone { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; }
    }
}

﻿using ContactProAltair.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContactProAltair.Models
{
    public class Contact
    {
        private DateTime _createdDate;
        private DateTime? _dateOfBirth;

        //Primary Key
        public int Id { get; set; }

        //Foreign Key
        public string? AppUserID { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength=2)]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
        public string? LastName { get; set; }

        [NotMapped]
        public string? FullName { get { return $"{FirstName} {LastName}"; } }

        [DataType(DataType.Date)]
        public DateTime CreatedDate { get => _createdDate.ToLocalTime(); set => _createdDate = value.ToUniversalTime(); }

        [Display(Name = "Birthday")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get => _dateOfBirth?.ToLocalTime(); set => _dateOfBirth = value.HasValue ? value.Value.ToUniversalTime() : null; }

        public string? Address1 { get; set; }

        public string? Address2 { get; set; }

        public string? City { get; set; }

        //States enum
        public States? State { get; set; }

        [Display(Name = "Zip Code")]
        [DataType(DataType.PostalCode)]
        public int? ZipCode { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        public byte[]? ImageData { get; set; }

        public string? ImageType { get; set; }

        //Navigation Properties - Refers to Entity Framework

        public virtual AppUser? AppUser { get; set; }
        public virtual ICollection<Category> Categories { get; set; } = new HashSet<Category>();





    }
}

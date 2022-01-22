using System.ComponentModel.DataAnnotations;

namespace SWPCarAssistent.Core.Common.Entities
{
    public class EntityContacts
    {
        [Key]
        [StringLength(50)]
        public string FullName { get; set; }

        [Required]
        [StringLength(15)]
        public string PhoneNumber { get; set; }

        public bool? Favourite { get; set; }

        public EntityContacts(string fullName, string phoneNumber, bool? favourite)
        {
            FullName = fullName;
            PhoneNumber = phoneNumber;
            Favourite = favourite;
        }
    }
}


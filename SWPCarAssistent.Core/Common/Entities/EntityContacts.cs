using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}


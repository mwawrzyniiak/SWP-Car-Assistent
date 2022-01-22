using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWPCarAssistent.Core.Common.Entities
{

    [Table("Radio")]
    public class EntityRadio
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string RadioName { get; set; }

        [Required]
        [StringLength(5)]
        public string Frequency { get; set; }
    }
}

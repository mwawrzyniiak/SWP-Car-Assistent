using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWPCarAssistent.Core.Common.Entities
{
    [Table("Radio")]
    public class Radio
    {
        public Radio() { }
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string RadioName { get; set; }

        [Required]
        [StringLength(5)]
        public string Frequency { get; set; }

        public Radio(int id, string radioName, string frequency)
        {
            Id = id;
            RadioName = radioName;
            Frequency = frequency;
        }
    }
}

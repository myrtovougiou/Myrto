using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.API.Models
{
    public class Audit
    {
        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public DateTime DateTime { get; set; }
        [Required]
        [MaxLength(128)]
        public string TableName { get; set; }
        [Required]
        [MaxLength(50)]
        public string Action { get; set; }

        public string KeyValues { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
    }
}

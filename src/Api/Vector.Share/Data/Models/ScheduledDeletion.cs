using System;
using System.ComponentModel.DataAnnotations;

namespace Vector.Share.Data.Models
{
    public class ScheduledDeletion
    {
        [Key]
        public string Identifier { get; set; }
        public DateTime DeletionTime { get; set; }
    }
}

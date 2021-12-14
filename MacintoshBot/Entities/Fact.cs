using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MacintoshBot.Entities
{
    [Table("facts")]
    public class Fact
    {

        public Fact()
        {
            CreatedTime = DateTime.Now;
        }
        
        [Column("id")]
        public int Id { get; set; }
        
        [Column("text")]
        public string Text { get; set; }
        
        [Column("created_ts")]
        public DateTime CreatedTime { get; set; }
    }
}
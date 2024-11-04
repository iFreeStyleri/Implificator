using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Implificator.DAL.Entities
{
    [Index(nameof(TgId), IsUnique = true)]
    public class User : Entity
    {
        
        [Required]
        public long TgId { get; set; }
        public uint CountSubscribe { get; set; }
        public uint CountMail { get; set; }

        public List<VIP> VIP { get; set; }
    }
}

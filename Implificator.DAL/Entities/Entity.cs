using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Implificator.DAL.Entities
{
    public abstract class Entity
    {
        [Key]
        public long Id { get; set; }
    }
}

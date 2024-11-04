using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implificator.DAL.Entities
{
    public class VIP : Entity
    {
        public DateTime ClosedDate { get; set; }
        public bool IsClosed { get; set; }

        public User User { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Implificator.DAL.Entities
{
    [Index(nameof(URL))]
    public class QRMessage
    {
        public int Id { get; set; }
        public User User { get; set; }
        public User SharedUser { get; set; }
        public string URL { get; set; }
        public bool IsPublish { get; set; }
    }
}

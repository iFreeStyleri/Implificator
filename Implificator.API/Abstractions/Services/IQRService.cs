using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implificator.Abstractions.Services
{
    public interface IQRService
    {
        Stream CreateQRCode(string url);
    }
}

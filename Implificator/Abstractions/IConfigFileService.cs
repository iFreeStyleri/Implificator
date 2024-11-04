using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implificator.Abstractions
{
    public interface IConfigFileService
    {
        public Task<string> ReadSection(string key);
    }
}

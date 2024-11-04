using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implificator.Abstractions
{
    public interface IPageCreator<T>
    {
        public Task<string> CreatePage(T page, long idChat);
    }
}

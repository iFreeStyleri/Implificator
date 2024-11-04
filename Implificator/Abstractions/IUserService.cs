using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Implificator.DAL.Entities;

namespace Implificator.Abstractions
{
    public interface IUserService
    {
        public Task<bool> HasVIP(long Id);
        public Task<User> CreateUser(long Id);
        public Task<VIP> AddVIP(long Id, int countDays);
    }
}

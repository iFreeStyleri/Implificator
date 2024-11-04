using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Implificator.Abstractions;
using Implificator.DAL;
using Implificator.DAL.Entities;
using Implificator.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Implificator.Implementations
{
    public class UserService : IUserService
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<VIP> _vipRepository;
        public UserService(IBaseRepository<User> context, IBaseRepository<VIP> vipRepository)
        {
            _userRepository = context;
            _vipRepository = vipRepository;
        }

        public async Task<bool> HasVIP(long Id)
        {
            var result = await _vipRepository.GetAll().Include(w => w.User).Where(w => w.User.TgId == Id).LastOrDefaultAsync();
            return result != null && result.ClosedDate <= DateTime.UtcNow;
        }

        public async Task<User> CreateUser(long Id)
        {
            var result = await _userRepository.GetAll().SingleOrDefaultAsync(u => u.TgId == Id);
            if (result == null)
            {
                var newUser = new User
                {
                    TgId = Id
                };
                await _userRepository.Add(newUser);
                return newUser;
            }

            return result;
        }

        public async Task<VIP> AddVIP(long id, int countDays)
        {
            var hasVIP = await HasVIP(id);
            if (!hasVIP)
            {
                var result = await _userRepository.GetAll().Include(i => i.VIP).SingleOrDefaultAsync(s => s.TgId == id);
                if (result.VIP != null && result.VIP.Count != 0)
                {
                    var VIP = new VIP
                    {
                        User = result,
                        ClosedDate = DateTime.UtcNow.AddDays(countDays),

                    };
                    await _vipRepository.Add(VIP);
                    return VIP;
                }
            }

            return null;
        }
    }
}

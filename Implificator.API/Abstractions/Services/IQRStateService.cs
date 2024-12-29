using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Implificator.Models;

namespace Implificator.Abstractions.Services
{
    public interface IQRStateService
    {
        public void SetUserData(long userId, UserData sharedUser);
        public UserData? GetUserData(long userId);
    }
}

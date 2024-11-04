using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Implificator.Abstractions
{
    public interface IQRCallback<T> : ICallback
    {
        public Stream GetQrCode(string url);
        public Task DoWorkAnother(Update update);
    }
}

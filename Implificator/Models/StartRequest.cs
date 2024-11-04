using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implificator.Models
{
    public class StartRequest
    {
        public MenuType MenuType { get; set; }
        public string Function { get; private set; }
        public long Value { get; private set; }
        public bool Created { get; private set; }
        public StartRequest(string metadata)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(metadata))
                    throw new ArgumentNullException("metadata");

                var function = metadata.Replace("/start ", "");
                Function = function.Split('-').First();
                Value = Convert.ToInt64(function.Split('-').Last());
                if (Value <= 0)
                    throw new ArgumentNullException(nameof(Value));
                if (Function == "sendRate")
                    MenuType = MenuType.Message;
                else if (Function == "ref")
                    MenuType = MenuType.References;
                else
                    throw new ArgumentException("Нераспознанная последовательность");
                Created = true;

            }
            catch
            {

            }
        }
    }
}

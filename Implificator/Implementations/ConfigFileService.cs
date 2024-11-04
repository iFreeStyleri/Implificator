using Implificator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implificator.Implementations
{
    public class ConfigFileService : IConfigFileService
    {
        private readonly string _configFile;
        public ConfigFileService(string path)
        {
            if (!path.Contains(".config"))
                throw new ArgumentException("Неправильный формат файла");
            _configFile = path;
        }
        public async Task<string> ReadSection(string key)
        {
            var sections = await File.ReadAllLinesAsync(_configFile);
            var section = sections.ToList().SingleOrDefault(s => s.StartsWith(s))?.Split('=').Last();
            if (section == null)
                throw new ArgumentException("key not found");
            return section;
        }
    }
}

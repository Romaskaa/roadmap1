using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using System.Text;
using System.Threading.Tasks;
using newproj.DTO;

namespace newproj.Infrastructure
{
    public class JsonRuleLoader
    {
        private readonly JavaScriptSerializer _serializer;

        public JsonRuleLoader()
        {
            _serializer = new JavaScriptSerializer();
        }

        public List<JsonRule> Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл с правилами не найден.", filePath);

            var json = File.ReadAllText(filePath, Encoding.UTF8);
            var rules = _serializer.Deserialize<List<JsonRule>>(json);

            return rules ?? new List<JsonRule>();
        }
    }
}

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

        public JsonRoadmap LoadRoadmap(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл с правилами не найден.", filePath);

            var json = File.ReadAllText(filePath, Encoding.UTF8);
            var roadmap = _serializer.Deserialize<JsonRoadmap>(json);

            if (roadmap != null && roadmap.Rules != null)
                return roadmap;

            var rules = _serializer.Deserialize<List<JsonRule>>(json) ?? new List<JsonRule>();
            return new JsonRoadmap
            {
                Version = "1.0",
                Rules = rules
            };
        }

        public List<JsonRule> Load(string filePath)
        {
            return LoadRoadmap(filePath).Rules ?? new List<JsonRule>();
        }
    }
}

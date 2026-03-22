using System;

namespace project.Controllers
{
    public class ProfileProperty
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public ProfileProperty()
        {
        }

        public ProfileProperty(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}

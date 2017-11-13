using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity.Attribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DefaultTableAttribute : TableAttribute
    {
        public DefaultTableAttribute(string name) : base(name)
        {
            if (!string.IsNullOrEmpty(Settings.DatabaseSchema))
                Schema = Settings.DatabaseSchema;
        }
    }
}
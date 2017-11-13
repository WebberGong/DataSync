using System;

namespace Entity.Attribute
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class NotificationWhereAttribute : System.Attribute
    {
        public NotificationWhereAttribute(string condition)
        {
            Condition = condition;
        }

        public string Condition { get; }
    }
}
using System;

namespace Screamer
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ScreamConditionAttribute : Attribute
    {
        /// <summary>
        /// How is the condition accessable in the config?
        /// </summary>
        public String Name { get; set; }

        public ScreamConditionAttribute(String name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ScreamTriggerAttribute : Attribute
    {
        /// <summary>
        /// How is the condition accessable in the config?
        /// </summary>
        public String Name { get; set; }

        public ScreamTriggerAttribute(String name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ScreamVariableAttribute : Attribute
    {
        /// <summary>
        /// How is the condition accessable in the config?
        /// </summary>
        public String Name { get; set; }

        public ScreamVariableAttribute(String name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ScreamActionAttribute : Attribute
    {
        /// <summary>
        /// How is the condition accessable in the config?
        /// </summary>
        public String Name { get; set; }

        public ScreamActionAttribute(String name)
        {
            Name = name;
        }
    }
}

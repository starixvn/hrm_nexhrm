namespace Fanex.NexHR.Core.XML
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class XMLNameAttribute : Attribute
    {
        private readonly string name;

        public XMLNameAttribute(string name)
        {
            this.name = name;
        }

        public string Name => name;
    }
}

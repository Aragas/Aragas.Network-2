using System;

namespace Aragas.Network.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PacketAttribute : Attribute
    {
        public long ID { get; }

        public PacketAttribute(long id) => ID = id;
    }
}
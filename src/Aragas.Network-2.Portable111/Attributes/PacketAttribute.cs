using System;

namespace Aragas.Network.Attributes
{
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = true)]
    public class PacketEnumAttribute : Attribute
    {
        public Type PacketType { get; }

        public PacketEnumAttribute(Type packetType) => PacketType = packetType;
    }
    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PacketAttribute : Attribute
    {
        public long ID { get; }

        public PacketAttribute(long id) => ID = id;
    }
}
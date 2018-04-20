using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Aragas.Network.Attributes;
using Aragas.Network.IO;

namespace Aragas.Network.Packets
{
    public abstract class BasePacketFactory<TPacketType, TIDType, TSerializer, TDeserializer> : IDisposable 
        where TPacketType : Packet<TIDType, TSerializer, TDeserializer> where TSerializer : PacketSerializer where TDeserializer : PacketDeserializer
    {
        public abstract TPacketType Create(TIDType packetID);
        public abstract TPacketTypeCustom Create<TPacketTypeCustom>() where TPacketTypeCustom : TPacketType;
        public abstract TPacketTypeCustom Create<TPacketTypeCustom>(Func<TPacketTypeCustom> initializer) where TPacketTypeCustom : new();

        public abstract void Dispose();
    }

    public class PacketEnumFactory<TPacketType, TIDType, TSerializer, TDeserializer> : BasePacketFactory<TPacketType, TIDType, TSerializer, TDeserializer> 
        where TPacketType : Packet<TIDType, TSerializer, TDeserializer> where TIDType : Enum where TSerializer : PacketSerializer where TDeserializer : PacketDeserializer
    {
        private static Dictionary<Type, TIDType> IDTypeFromPacketType { get; } = new Dictionary<Type, TIDType>();
        private static Dictionary<TIDType, Func<TPacketType>> Packets { get; } = new Dictionary<TIDType, Func<TPacketType>>();
        private static bool IsInizialized { get; set; }

        public PacketEnumFactory()
        {
            if (!IsInizialized)
            {
                lock (this) // Thread safe
                {
                    if (!IsInizialized)
                    {
                        IsInizialized = true;

#if !(NETSTANDARD2_0 || NET45)
                        var whereToFindPackets = PCLExt.AppDomain.AppDomain.GetAssemblies();
#else
                        var whereToFindPackets = AppDomain.CurrentDomain.GetAssemblies();
#endif
                        foreach (var packetType in whereToFindPackets.SelectMany(asm => asm.ExportedTypes.Where(type => type.GetTypeInfo().IsSubclassOf(typeof(TPacketType)))))
                        {
                            var p = ActivatorCached.CreateInstance(packetType) as TPacketType;
                            Packets.Add(p.ID, packetType != null ? (Func<TPacketType>)(() => ActivatorCached.CreateInstance(packetType) as TPacketType) : null);
                            IDTypeFromPacketType.Add(p.GetType(), p.ID);
                        }
                    }
                }
            }
        }

        public override TPacketType Create(TIDType packetID) => Packets.TryGetValue(packetID, out var packetConstructor) ? packetConstructor() : null;
        public override TPacketTypeCustom Create<TPacketTypeCustom>() => Packets.TryGetValue(IDTypeFromPacketType[typeof(TPacketTypeCustom)], out var packetConstructor) ? (TPacketTypeCustom) packetConstructor() : null;
        public override TPacketTypeCustom Create<TPacketTypeCustom>(Func<TPacketTypeCustom> initializer) => initializer();

        public override void Dispose() { }
    }

    public class PacketAttributeFactory<TPacketType, TIDType, TSerializer, TDeserializer> : BasePacketFactory<TPacketType, TIDType, TSerializer, TDeserializer>
    where TPacketType : Packet<TIDType, TSerializer, TDeserializer> where TIDType : struct where TSerializer : PacketSerializer where TDeserializer : PacketDeserializer
    {
        private static Dictionary<Type, TIDType> IDTypeFromPacketType { get; } = new Dictionary<Type, TIDType>();
        private static Dictionary<TIDType, Func<TPacketType>> Packets { get; } = new Dictionary<TIDType, Func<TPacketType>>();
        private static bool IsInizialized { get; set; }

        public PacketAttributeFactory()
        {
            if (!IsInizialized)
            {
                lock (this) // Thread safe
                {
                    if (!IsInizialized)
                    {
                        IsInizialized = true;

#if !(NETSTANDARD2_0 || NET45)
                        var whereToFindPackets = PCLExt.AppDomain.AppDomain.GetAssemblies();
#else
                        var whereToFindPackets = AppDomain.CurrentDomain.GetAssemblies();
#endif
                        var packetTypes = whereToFindPackets.Where(asm => !asm.IsDynamic).SelectMany(asm => asm.ExportedTypes.Select(type => type.GetTypeInfo()).Where(typeInfo => typeInfo.IsSubclassOf(typeof(TPacketType)))).ToList();
                        var packetTypesWithAttribute = packetTypes.Where(type => type.IsDefined(typeof(PacketAttribute), false));
                        foreach (var typeInfo in packetTypesWithAttribute)
                        {
                            var p = ActivatorCached.CreateInstance(typeInfo.AsType()) as PacketAttribute<TIDType, TSerializer, TDeserializer>;
                            Packets.Add(p.ID, () => (TPacketType)ActivatorCached.CreateInstance(typeInfo.AsType()));
                            IDTypeFromPacketType.Add(p.GetType(), p.ID);
                        }
                    }
                }
            }
        }

        public override TPacketType Create(TIDType packetID) => Packets.TryGetValue(packetID, out var packetConstructor) ? packetConstructor() : null;
        public override TPacketTypeCustom Create<TPacketTypeCustom>() => Packets.TryGetValue(IDTypeFromPacketType[typeof(TPacketTypeCustom)], out var packetConstructor) ? (TPacketTypeCustom)packetConstructor() : null;
        public override TPacketTypeCustom Create<TPacketTypeCustom>(Func<TPacketTypeCustom> initializer) => initializer();

        public override void Dispose() { }
    }

    public class DefaultPacketFactory<TPacketType, TIDType, TSerializer, TDeserializer> : BasePacketFactory<TPacketType, TIDType, TSerializer, TDeserializer> 
        where TPacketType : Packet<TIDType, TSerializer, TDeserializer> where TSerializer : PacketSerializer where TDeserializer : PacketDeserializer
    {
        private static Dictionary<Type, TIDType> IDTypeFromPacketType = new Dictionary<Type, TIDType>();
        private static Dictionary<TIDType, Func<TPacketType>> Packets = new Dictionary<TIDType, Func<TPacketType>>();
        private static bool _isInizialized = false;

        public DefaultPacketFactory()
        {
            if (!_isInizialized)
            {
                lock (this)
                {
                    if (!_isInizialized)
                    {
                        _isInizialized = true;

#if !(NETSTANDARD2_0 || NET45)
                        var whereToFindPackets = PCLExt.AppDomain.AppDomain.GetAssemblies();
#else
                        var whereToFindPackets = AppDomain.CurrentDomain.GetAssemblies();
#endif

                        var packetTypes = whereToFindPackets.Where(asm => !asm.IsDynamic).SelectMany(asm => asm.ExportedTypes.Where(type => type.GetTypeInfo().IsSubclassOf(typeof(TPacketType)))).ToList();
                        foreach (var packetType in packetTypes)
                        {
                            var p = ActivatorCached.CreateInstance(packetType) as TPacketType; // -- We need to create a packet instance to get the ID
                            Packets.Add(p.ID, packetType != null ? (Func<TPacketType>)(() => ActivatorCached.CreateInstance(packetType) as TPacketType) : null);
                            IDTypeFromPacketType.Add(p.GetType(), p.ID);
                        }
                    }
                }
            }
        }

        public override TPacketType Create(TIDType packetID) => Packets.TryGetValue(packetID, out var packetConstructor) ? packetConstructor() : null;
        public override TPacketTypeCustom Create<TPacketTypeCustom>() => Packets.TryGetValue(IDTypeFromPacketType[typeof(TPacketTypeCustom)], out var packetConstructor) ? (TPacketTypeCustom) packetConstructor() : null;
        public override TPacketTypeCustom Create<TPacketTypeCustom>(Func<TPacketTypeCustom> initializer) => initializer();

        public override void Dispose() { }
    }
}
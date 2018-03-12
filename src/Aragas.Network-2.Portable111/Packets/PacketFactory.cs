using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Aragas.Network.Attributes;
using Aragas.Network.IO;

namespace Aragas.Network.Packets
{
    public abstract class BasePacketFactory<TPacketType, TIDType, TSerializer, TDeserializer> : IDisposable where TPacketType : Packet<TIDType, TSerializer, TDeserializer> where TSerializer : PacketSerializer where TDeserializer : PacketDeserialiser
    {
        public abstract TPacketType Create(TIDType packetID);
        public abstract TPacketTypeCustom Create<TPacketTypeCustom>() where TPacketTypeCustom : TPacketType;
        public abstract TPacketTypeCustom Create<TPacketTypeCustom>(Func<TPacketTypeCustom> initializer) where TPacketTypeCustom : new();

        public abstract void Dispose();
    }

    /// <summary>
    /// Will search for Packets in the assembly where the TPacketType was defined only.
    /// Packet ID's are saved by using the key's HashCode, so it won't work with strings, but will work with default numericals.
    /// </summary>
    /// <typeparam name="TPacketType"></typeparam>
    /// <typeparam name="TIDType"></typeparam>
    /// <typeparam name="TSerializer"></typeparam>
    /// <typeparam name="TDeserializer"></typeparam>
    public class DefaultPacketFactory<TPacketType, TIDType, TSerializer, TDeserializer> : BasePacketFactory<TPacketType, TIDType, TSerializer, TDeserializer> where TPacketType : Packet<TIDType, TSerializer, TDeserializer> where TSerializer : PacketSerializer where TDeserializer : PacketDeserialiser
    {
        private static void AddIDList(IEnumerable<Assembly> whereToFindPackets)
        {
            var packetTypes = whereToFindPackets.Where(asm => !asm.IsDynamic).SelectMany(asm => asm.ExportedTypes.Where(type => type.GetTypeInfo().IsSubclassOf(typeof(TPacketType)))).ToList();
            foreach (var packetType in packetTypes)
            {
                var p = ActivatorCached.CreateInstance(packetType) as TPacketType; // -- We need to create a packet instance to get the ID
                Packets.Add(p.ID, packetType != null ? (Func<TPacketType>)(() => ActivatorCached.CreateInstance(packetType) as TPacketType) : null);
                IDTypeFromPacketType.Add(p.GetType(), p.ID);
            }
        }

        private static void AddIDListByEnum(Type packetEnumType, IEnumerable<Assembly> whereToFindPackets)
        {
            // Ensure every enum value has unique value
            
            var packetTypes = whereToFindPackets.Where(asm => !asm.IsDynamic).SelectMany(asm => asm.ExportedTypes.Where(type => type.GetTypeInfo().IsSubclassOf(typeof(TPacketType)))).ToList();
            foreach (var packetName in Enum.GetValues(packetEnumType))
            {
                var packetTypeName = $"{packetName}Packet";
                var packetType = packetTypes.FirstOrDefault(type => type.Name == packetTypeName);

                var p = ActivatorCached.CreateInstance(packetType) as PacketWithEnumName<TIDType, TSerializer, TDeserializer>;
                p.SetIntegerType((int) packetName);
                
                Packets.Add(p.ID, packetType != null ? (Func<TPacketType>)(() =>
                {
                    var packetID = packetName;
                    var packet = ActivatorCached.CreateInstance(packetType) as PacketWithEnumName<TIDType, TSerializer, TDeserializer>;
                    packet.SetIntegerType((int) packetID);
                    return packet as TPacketType;
                }) : null);
                IDTypeFromPacketType.Add(p.GetType(), p.ID);
            }
        }

        private static void AddIDListByAttribute(IEnumerable<Assembly> whereToFindPackets)
        {
            var packetTypes = whereToFindPackets.Where(asm => !asm.IsDynamic).SelectMany(asm => asm.ExportedTypes.Select(type => type.GetTypeInfo()).Where(typeInfo => typeInfo.IsSubclassOf(typeof(TPacketType)))).ToList();
            var packetTypesWithAttribute = packetTypes.Where(type => type.IsDefined(typeof(PacketAttribute), false));
            foreach (var typeInfo in packetTypesWithAttribute)
            {
                var p = ActivatorCached.CreateInstance(typeInfo.AsType()) as PacketWithAttribute<TIDType, TSerializer, TDeserializer>;
                Packets.Add(p.ID, () => (TPacketType) ActivatorCached.CreateInstance(typeInfo.AsType()));
                IDTypeFromPacketType.Add(p.GetType(), p.ID);
            }
        }


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
                        var assemblies = PCLExt.AppDomain.AppDomain.GetAssemblies();
#else
                        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
#endif

                        if (typeof(TPacketType).GetTypeInfo().IsSubclassOf(typeof(PacketWithEnumName<TIDType, TSerializer, TDeserializer>)))
                        {
                            var packetEnumAttributes = assemblies.Where(asm => !asm.IsDynamic).SelectMany(asm => asm.ExportedTypes.Select(type => type.GetTypeInfo()).Where(typeInfo => typeInfo.IsDefined(typeof(PacketEnumAttribute), false)));
                            var packetEnumAttribute = packetEnumAttributes.SingleOrDefault(attrType => attrType.GetCustomAttribute<PacketEnumAttribute>().PacketType == typeof(TPacketType));
                            if (packetEnumAttribute != null)
                                AddIDListByEnum(packetEnumAttribute.AsType(), assemblies);
                        }
                        else if (typeof(TPacketType).GetTypeInfo().IsSubclassOf(typeof(PacketWithAttribute<TIDType, TSerializer, TDeserializer>)))
                            AddIDListByAttribute(assemblies);
                        else
                            AddIDList(assemblies);
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
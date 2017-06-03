using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Aragas.Network.Attributes;
using Aragas.Network.IO;

namespace Aragas.Network.Packets
{
    public class PacketFactory<TPacketType, TIDType, TSerializer, TDeserializer> where TPacketType : Packet<TIDType, TSerializer, TDeserializer> where TSerializer : PacketSerializer where TDeserializer : PacketDeserialiser
    {
        #region Packet Build
        private static TypeInfo GetTypeFromNameAndAbstract<T>(string className, IEnumerable<Assembly> assemblies) =>
            assemblies.Select(assembly => assembly.DefinedTypes.Where(typeInfo => typeInfo.IsSubclassOf(typeof(T))).FirstOrDefault(typeInfo => typeInfo.Name == className)).FirstOrDefault();

        private static Dictionary<object, Func<object[], TPacketType>> CreateIDList(Type packetEnumType, IEnumerable<Assembly> whereToFindPackets)
        {
            var packetDictionary = new Dictionary<object, Func<object[], TPacketType>>();

            var typeNames = Enum.GetValues(packetEnumType);

            foreach (var packetName in typeNames)
            {
                var typeName = $"{packetName}Packet";
                var type = GetTypeFromNameAndAbstract<TPacketType>(typeName, whereToFindPackets);
                packetDictionary.Add((int) packetName, type != null ? (Func<object[], TPacketType>)(args => (TPacketType) ActivatorCached.CreateInstance(type.AsType(), args)) : null);
            }

            return packetDictionary;
        }
        #endregion Packet Build

        #region Packet Attribute Build
        private static IEnumerable<TypeInfo> GetTypeInfosFromAbstract<T>(IEnumerable<Assembly> assemblies) =>
            assemblies.SelectMany(assembly => assembly.DefinedTypes.Where(typeInfo => typeInfo.IsSubclassOf(typeof(T))));

        private static KeyValuePair<object, Func<object[], TPacketType>> GetPacketIDAndFunc(TypeInfo typeInfo) =>
            new KeyValuePair<object, Func<object[], TPacketType>>(typeInfo.GetCustomAttribute<PacketAttribute>().ID, args => (TPacketType) ActivatorCached.CreateInstance(typeInfo.AsType(), args));

        public static Dictionary<object, Func<object[], TPacketType>> CreateIDListByAttribute(IEnumerable<Assembly> whereToFindPackets)
        {
            var typeInfos = GetTypeInfosFromAbstract<TPacketType>(whereToFindPackets);
            var typeInfosWithAttribute = typeInfos.Where(typeInfo => typeInfo.IsDefined(typeof(PacketAttribute), false));
            return typeInfosWithAttribute.Select(GetPacketIDAndFunc).ToDictionary(pair => pair.Key, pair => pair.Value);
        }
        #endregion Packet Attribute Build

        private Dictionary<object, Func<object[], TPacketType>> Packets { get; } = new Dictionary<object, Func<object[], TPacketType>>();

        public PacketFactory(Type packetEnumType = null)
        {
            if (packetEnumType != null)
            {
                foreach (var keyValuePair in CreateIDList(packetEnumType, new[] { typeof(TPacketType).GetTypeInfo().Assembly }))
                    Packets.Add(keyValuePair.Key, keyValuePair.Value);
            }

            foreach (var keyValuePair in CreateIDListByAttribute(new[] { typeof(TPacketType).GetTypeInfo().Assembly }))
                if (!Packets.ContainsKey(keyValuePair.Key))
                    Packets.Add(keyValuePair.Key, keyValuePair.Value);
        }

        public TPacketType Create(object packetID, params object[] args) => Packets[packetID](args);
    }
}
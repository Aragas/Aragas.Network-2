using System.IO;

namespace Aragas.Network.IO
{
    public abstract class StreamDeserializer : PacketDeserializer
    {
        public static TDeserializer Create<TDeserializer>(byte [] data) where TDeserializer : StreamDeserializer, new()
            => new TDeserializer() { Stream = new MemoryStream(data) };

        public Stream Stream { get; internal set; }

        private StreamDeserializer() { }
        protected StreamDeserializer(byte[] data) { Stream = new MemoryStream(data); }
        protected StreamDeserializer(Stream stream) { Stream = stream; }

        public override int BytesLeft() => (int) (Stream.Length - Stream.Position);
        
        public override void Dispose()
        {
            Stream.Dispose();
        }
    }
}
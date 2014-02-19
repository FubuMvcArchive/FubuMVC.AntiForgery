using System;
using System.IO;

namespace FubuMVC.AntiForgery
{
    public class BinaryAntiForgerySerializer : IAntiForgerySerializer
    {
        private readonly IAntiForgeryEncoder _encoder;

        public BinaryAntiForgerySerializer(IAntiForgeryEncoder encoder)
        {
            _encoder = encoder;
        }

        public virtual AntiForgeryData Deserialize(string serializedToken)
        {
            try
            {
                using (var stream = new MemoryStream(_encoder.Decode(serializedToken)))
                using (var reader = new BinaryReader(stream))
                {
                    return new AntiForgeryData
                    {
                        Salt = reader.ReadString(),
                        Value = reader.ReadString(),
                        CreationDate = new DateTime(reader.ReadInt64()),
                        Username = reader.ReadString()
                    };
                }
            }
            catch (Exception ex)
            {
                //TODO log this failure somehow so we can warn admins about tampering.
                return new AntiForgeryData();
            }
        }

        public virtual string Serialize(AntiForgeryData token)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(token.Salt);
                writer.Write(token.Value);
                writer.Write(token.CreationDate.Ticks);
                writer.Write(token.Username);

                return _encoder.Encode(stream.ToArray());
            }
        }
    }
}
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;


namespace Insight.Database
{
    /// <summary>
    /// Serializes objects to JSON using the DataContractJsonSerializer or an overridden serializer (usually Newtonsoft.JSON).
    /// </summary>
    internal class DataContractJsonObjectSerializer : DbObjectSerializer
    {
        /// <inheritdoc/>
        public override bool CanSerialize(Type type, DbType dbType)
        {
            return base.CanSerialize(type, dbType) || dbType == DbType.Object;
        }

        /// <inheritdoc/>
        public override object SerializeObject(Type type, object value)
        {
            if (value == null)
                return null;

            // serialize the parameters
            using (MemoryStream stream = new MemoryStream())
            {
                new DataContractJsonSerializer(type).WriteObject(stream, value);

                return Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
            }
        }

        /// <inheritdoc/>
        public override object DeserializeObject(Type type, object encoded)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes((string)encoded)))
            {
                return serializer.ReadObject(stream);
            }
        }
    }
}

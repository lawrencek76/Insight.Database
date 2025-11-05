#nullable enable
using System;
using System.Data;
using System.Text.Json;

namespace Insight.Database
{
    /// <summary>
    /// Serializes objects to JSON using the DataContractJsonSerializer or an overridden serializer (usually Newtonsoft.JSON).
    /// </summary>
    public class JsonObjectSerializer : IDbObjectSerializer
    {
		/// <summary>
		/// The singleton Serializer.
		/// </summary>
		public static DbObjectSerializer Serializer => _serializer;

		private static DbObjectSerializer _serializer;
		public Func<Type, object, Exception, object>? OnSerializeError { get; set; }

		/// <summary>
		/// Gets or sets a JSON Serializer to replace the DefaultJsonSerializer.
		/// </summary>
		public static DbObjectSerializer? OverrideSerializer { get; set; }

		static JsonObjectSerializer()
		{
			_serializer = new DataContractJsonObjectSerializer();
		}
		
		/// <inheritdoc/>
		public bool CanSerialize(Type type, DbType dbType)
        {
			if(OverrideSerializer != null)
				return OverrideSerializer.CanSerialize(type, dbType);
			return _serializer.CanSerialize(type, dbType) || dbType == DbType.Object;
        }

        /// <inheritdoc/>
        public object SerializeObject(Type type, object value)
        {
            if (OverrideSerializer != null)
                return OverrideSerializer.SerializeObject(type, value);

			return _serializer.SerializeObject(type, value);
		}

        /// <inheritdoc/>
        public object DeserializeObject(Type type, object encoded)
        {
            if (OverrideSerializer != null)
                return OverrideSerializer.DeserializeObject(type, encoded);

            return _serializer.DeserializeObject(type, encoded);
		}

		/// <inheritdoc/>
		public bool CanDeserialize(Type sourceType, Type targetType)
		{
			if (OverrideSerializer != null)
				return OverrideSerializer.CanDeserialize(sourceType, targetType);
			return _serializer.CanDeserialize(sourceType, targetType);
		}

		/// <inheritdoc/>
		public DbType GetSerializedDbType(Type type, DbType dbType)
		{
			if (OverrideSerializer != null)
				return OverrideSerializer.GetSerializedDbType(type, dbType);

			return _serializer.GetSerializedDbType(type, dbType);
		}

		public static void UseSystemTextJsonSerializer(JsonSerializerOptions? options = null)
		{
			_serializer = new SystemTextJsonObjectSerializer(options);
		}
	}
}

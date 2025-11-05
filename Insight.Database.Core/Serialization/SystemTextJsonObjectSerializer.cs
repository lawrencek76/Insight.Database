#nullable enable
using Insight.Database.MissingExtensions;

using System;
using System.Data;
using System.Text.Json;

namespace Insight.Database
{
	internal class SystemTextJsonObjectSerializer : DbObjectSerializer
	{
		private readonly JsonSerializerOptions _options;
		public Func<Type, object, Exception, object>? OnSerializeError { get; set; }

		public SystemTextJsonObjectSerializer(JsonSerializerOptions? options = null)
		{
			_options = options ?? JsonSerializerOptions.Default;
		}

		/// <inheritdoc/>
		public override bool CanSerialize(Type type, DbType dbType) =>
			base.CanSerialize(type, dbType) || dbType == DbType.Object;

		/// <inheritdoc/>
		public override object? SerializeObject(Type type, object value) =>
			value is not null ? JsonSerializer.Serialize(value, type, _options) : null;

		/// <inheritdoc/>
		override public object? DeserializeObject(Type type, object encoded)
		{
			try
			{
				if (encoded is null)
				{
					return null;
				}
				var jsonString = encoded as string;
				if (string.IsNullOrEmpty(jsonString))
				{
					jsonString = "{}";
				}
				return JsonSerializer.Deserialize(jsonString!, type, _options)!;
			}
			catch (Exception ex)
			{
				if (OnSerializeError != null)
				{
					return OnSerializeError(type, encoded, ex);
				}
				throw;
			}
		}


	}
}

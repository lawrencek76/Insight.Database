using System;
using System.Collections.Generic;
using System.Data;

using Microsoft.Data;
using Microsoft.Data.SqlClient;

namespace Insight.Database.Providers.MsSqlClient
{
	internal static class SqlTypeMapper
	{
		private static readonly IReadOnlyDictionary<string, SqlDbType> Map =
			new Dictionary<string, SqlDbType>(StringComparer.OrdinalIgnoreCase)
			{
				["bigint"] = SqlDbType.BigInt,
				["binary"] = SqlDbType.Binary,
				["bit"] = SqlDbType.Bit,
				["char"] = SqlDbType.Char,
				["date"] = SqlDbType.Date,
				["datetime"] = SqlDbType.DateTime,
				["datetime2"] = SqlDbType.DateTime2,
				["datetimeoffset"] = SqlDbType.DateTimeOffset,
				["decimal"] = SqlDbType.Decimal,
				["filestream"] = SqlDbType.VarBinary,
				["float"] = SqlDbType.Float,
				["image"] = SqlDbType.Image,
				["int"] = SqlDbType.Int,
				["money"] = SqlDbType.Money,
				["nchar"] = SqlDbType.NChar,
				["ntext"] = SqlDbType.NText,
				["numeric"] = SqlDbType.Decimal,
				["nvarchar"] = SqlDbType.NVarChar,
				["real"] = SqlDbType.Real,
				["rowversion"] = SqlDbType.Timestamp,
				["smalldatetime"] = SqlDbType.DateTime,
				["smallint"] = SqlDbType.SmallInt,
				["smallmoney"] = SqlDbType.SmallMoney,
				["sql_variant"] = SqlDbType.Variant,
				["text"] = SqlDbType.Text,
				["time"] = SqlDbType.Time,
				["timestamp"] = SqlDbType.Timestamp,
				["tinyint"] = SqlDbType.TinyInt,
				["uniqueidentifier"] = SqlDbType.UniqueIdentifier,
				["varbinary"] = SqlDbType.VarBinary,
				["varchar"] = SqlDbType.VarChar,
				["xml"] = SqlDbType.Xml,
				["json"] = SqlDbTypeExtensions.Json,
				["vector"] = SqlDbTypeExtensions.Vector,
			};

		/// <summary>
		/// Gets the corresponding SqlDbType for a given SQL Server data type name.
		/// </summary>
		public static bool TryGetSqlDbType(string sqlType, out SqlDbType sqlDbType)
			=> Map.TryGetValue(sqlType, out sqlDbType);

		/// <summary>
		/// Gets the SqlDbType for a given SQL Server type or throws if not found.
		/// </summary>
		public static SqlDbType GetSqlDbType(string sqlType)
		{
			if (sqlType == null)
				throw new ArgumentNullException(nameof(sqlType));

			if (Map.TryGetValue(sqlType, out var result))
				return result;

			throw new ArgumentOutOfRangeException(
				nameof(sqlType),
				$"Unrecognized SQL Server type name '{sqlType}'."
			);
		}
	}
}

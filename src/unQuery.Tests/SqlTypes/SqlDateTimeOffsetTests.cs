﻿using NUnit.Framework;
using System;
using System.Data;
using System.Linq;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlDateTimeOffsetTests : TestFixture
	{
		private readonly DateTimeOffset testValue = new DateTimeOffset(new DateTime(2012, 11, 10, 1, 2, 3, 003), TimeSpan.FromHours(2));

		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(SqlDateTimeOffset.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlDateTimeOffset.GetTypeHandler().CreateParamFromValue("Test", null));
		}

		[Test]
		public void CreateMetaData()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlDateTimeOffset.GetTypeHandler().CreateMetaData("Test"));

			SqlTypeHandler col = new SqlDateTimeOffset(testValue, null, ParameterDirection.Input);
			Assert.Throws<TypePropertiesMustBeSetExplicitlyException>(() => col.CreateMetaData("Test"));

			col = new SqlDateTimeOffset(testValue, 5, ParameterDirection.Input);
			var meta = col.CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.DateTimeOffset, meta.SqlDbType);
			Assert.AreEqual(5, meta.Scale);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			TestHelper.AssertSqlParameter((new SqlDateTimeOffset(testValue, 6, ParameterDirection.Input)).GetParameter(), SqlDbType.DateTimeOffset, testValue, scale: 6);
			TestHelper.AssertSqlParameter((new SqlDateTimeOffset(null, 4, ParameterDirection.Input)).GetParameter(), SqlDbType.DateTimeOffset, DBNull.Value, scale: 4);
			TestHelper.AssertSqlParameter((new SqlDateTimeOffset(testValue, null, ParameterDirection.Input)).GetParameter(), SqlDbType.DateTimeOffset, testValue, scale: 0);
			TestHelper.AssertSqlParameter((new SqlDateTimeOffset(null, null, ParameterDirection.Input)).GetParameter(), SqlDbType.DateTimeOffset, DBNull.Value, scale: 0);
		}

		[Test]
		public void GetRawValue()
		{
			SqlType type = new SqlDateTimeOffset(testValue, 2, ParameterDirection.Input);
			Assert.AreEqual(testValue, type.GetRawValue());

			type = new SqlDateTimeOffset(null, 0, ParameterDirection.Input);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlDateTimeOffset>(Col.DateTimeOffset(testValue, 7));
			Assert.IsInstanceOf<SqlDateTimeOffset>(Col.DateTimeOffset(testValue));
		}

		[Test]
		public void Structured()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfDateTimeOffsets", new[] {
					new { A = Col.DateTimeOffset(testValue, 4) },
					new { A = Col.DateTimeOffset(null, 4) }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(DateTimeOffset), rows[0].A.GetType());
			Assert.AreEqual(testValue, rows[0].A);
			Assert.AreEqual(null, rows[1].A);
		}

		[Test]
		public void StructuredDynamicYielder()
		{
			var result = new StructuredDynamicYielder(new[] { new {
				A = Col.DateTimeOffset(testValue, 4),
				B = Col.DateTimeOffset(null, 4)
			}}).First();

			Assert.AreEqual(2, result.FieldCount);
			Assert.AreEqual(typeof(DateTimeOffset), result.GetValue(0).GetType());
			Assert.AreEqual(testValue, result.GetValue(0));
			Assert.AreEqual(DBNull.Value, result.GetValue(1));
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlDateTimeOffset)]);
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.DateTimeOffset]);
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Insight.Database;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Dynamic;
using System.Xml;
using System.Xml.Linq;
using System.Runtime.Serialization;
using System.Data.Common;
using System.Text.Json;
using System.Text.Json.Nodes;


namespace Insight.Tests.MsSqlClient
{
    [TestFixture]
    public class JsonTests : MsSqlClientBaseTest
    {
        class Result
        {
            public Data Data;
            public string String;
            public System.Text.Json.JsonDocument JsonDocument;
            public System.Text.Json.Nodes.JsonNode JsonNode;
        }

        class Data
        {
            public string Text;
        }

        #region SingleColumn Deserialization Tests

        [Test]
        public void JsonSingleColumnCanDeserializeToJsonNode()
        {
            var list = Connection().QuerySql<JsonNode>("SELECT CONVERT(json, '{}')", new { });
			
            ClassicAssert.IsNotNull(list);
            var doc = list[0];
            ClassicAssert.IsNotNull(doc);
            
        }

		[Test]
		public void JsonSingleColumnCanDeserializeToJsonDocument()
		{
			var list = Connection().QuerySql<JsonDocument>("SELECT CONVERT(json, '{}')", new { });

			ClassicAssert.IsNotNull(list);
			var obj = list[0];
			ClassicAssert.IsNotNull(obj);
			ClassicAssert.AreEqual(JsonValueKind.Object, obj.RootElement.ValueKind);
		}

		[Test]
        public void JsonSingleColumnCanDeserializeToString()
        {
            var list = Connection().QuerySql<string>("SELECT CONVERT(json, '{ \"Text\": \"foo\" }')", new { });

            ClassicAssert.IsNotNull(list);
            var s = list[0];
            ClassicAssert.IsNotNull(s);
            ClassicAssert.AreEqual("{\"Text\":\"foo\"}", s);
        }
        #endregion

        #region Json Column Deserialization Tests
        [Test]
        public void JsonColumnCanDeserializeToString()
        {
            var list = Connection().QuerySql<Result>("SELECT String=CONVERT(json, '{ \"Text\": \"foo\" }')", new { });

            ClassicAssert.IsNotNull(list);
            var result = list[0];
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsNotNull(result.String);
            ClassicAssert.AreEqual("{\"Text\":\"foo\"}", result.String);
        }

		[Test]
		public void JsonColumnCanDeserializeToJsonDocument()
		{
			var list = Connection().QuerySql<Result>("SELECT JsonDocument=CONVERT(json, '{ \"Text\": \"foo\" }')", new { });

			ClassicAssert.IsNotNull(list);
			var result = list[0];
			ClassicAssert.IsNotNull(result);
			ClassicAssert.IsNotNull(result.JsonDocument);
			ClassicAssert.AreEqual("{\"Text\":\"foo\"}", result.JsonDocument.ToString());
		}

		[Test]
        public void JsonColumnCanDeserializeToJsonNode()
        {
            var list = Connection().QuerySql<Result>("SELECT JsonNode=CONVERT(json, '{ \"Text\": \"foo\" }')", new { });

            ClassicAssert.IsNotNull(list);
            var result = list[0];
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsNotNull(result.JsonNode);
            ClassicAssert.AreEqual("{\"Text\":\"foo\"}", result.JsonNode.ToString());
        }

        [Test]
        public void JsonColumnCanDeserializeToObject()
        {
            var list = Connection().QuerySql<Result>("SELECT Data=CONVERT(json, '{ \"Text\": \"foo\" }')", new { });

            ClassicAssert.IsNotNull(list);
            var result = list[0];
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsNotNull(result.Data);
            ClassicAssert.AreEqual("foo", result.Data.Text);
        }
        #endregion

        #region Serialization Tests
        [Test]
        public void JsonDocumentCanSerializeToJsonParameter()
        {
            // create a document
            JsonDocument doc = JsonDocument.Parse("{ \"Text\": \"foo\" }");

            var list = Connection().Query<JsonDocument>("ReflectJson", new { Json = doc });
            var data = list[0];
            ClassicAssert.IsNotNull(data);
            ClassicAssert.AreEqual(doc.ToString(), data.ToString());
        }

        [Test]
        public void JsonNodeCanSerializeToJsonParameter()
        {
            // create a document
            XDocument doc = XDocument.Parse("<Data><Text>foo</Text></Data>");

            var list = Connection().Query<JsonNode>("ReflectJson", new { Json = doc });
            var data = list[0];
            ClassicAssert.IsNotNull(data);
            ClassicAssert.AreEqual(doc.ToString(), data.ToString());
        }

        [Test]
        public void ObjectCanSerializeToXmlParameter()
        {
            // create a document
            Data d = new Data()
            {
                Text = "foo"
            };

            var list = Connection().Query<Result>("ReflectXmlAsData", new { Xml = d });
            var data = list[0];
            ClassicAssert.IsNotNull(data);
            ClassicAssert.AreEqual(d.Text, data.Data.Text);
        }

        [Test]
        public void StringCanSerializeToXmlParameter()
        {
            // create a document
            string doc = "<Data><Text>foo</Text></Data>";

            var list = Connection().Query<string>("ReflectXml", new { Xml = doc });
            var data = list[0];
            ClassicAssert.IsNotNull(data);
            ClassicAssert.AreEqual(doc, data);
        }
        #endregion
    }

    [TestFixture]
    public class JsonTVPTests : MsSqlClientBaseTest
    {
        class Result
        {
            public Data Data;
        }

        class Data
        {
            public string Text;
        }

        [Test]
        public void XmlFieldCanBeSerializedInTVP()
        {
            Result r = new Result();
            r.Data = new Data();
            r.Data.Text = "foo";

            var list = Connection().Query<Result>("ReflectXmlTable", new { p = new List<Result>() { r } });
            var item = list[0];

            ClassicAssert.AreEqual(r.Data.Text, item.Data.Text);
        }

        [Test]
        public void StringXmlCanBeSentAndReturnedAsStrings()
        {
            string s = "<xml>text</xml>";
            var input = new List<string>() { s };

            var list = Connection().Query<string>("ReflectXmlTableAsVarChar", new { p = input.Select(x => new { id = 1, data = x }).ToList() });
            var item = list[0];

            ClassicAssert.AreEqual(s, item);
        }
    }
}

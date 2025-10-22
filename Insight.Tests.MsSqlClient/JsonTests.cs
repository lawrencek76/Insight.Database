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
            //public System.Text.Json.JsonDocument jsonDocument;
            public System.Text.Json.Nodes.JsonNode jsonNode;
        }

        class Data
        {
            public string Text;
        }

        #region SingleColumn Deserialization Tests
        [Test]
        public void JsonIsDeserializedProperly()
        {
            using (var c = Connection().OpenWithTransaction())
            {
                c.ExecuteSql("CREATE TABLE JsonTest(stuff json)");
                c.ExecuteSql("INSERT INTO JsonTest VALUES(@s)", new { s = new String('x', 10000) });

                var inner = (SqlConnection)c.InnerConnection;

                var result = inner.QueryXml("SELECT * FROM JsonTest", commandType: CommandType.Text, transaction: c);
            }
        }

        [Test]
        public void JsonSingleColumnCanDeserializeToJsonDocument()
        {
            var list = Connection().QuerySql<string>("SELECT CONVERT(json, '{}')", new { });

            ClassicAssert.IsNotNull(list);
            var doc = list[0];
            ClassicAssert.IsNotNull(doc);
            
        }

        [Test]
        public void XmlSingleColumnCanDeserializeToXDocument()
        {
            var list = Connection().QuerySql<XDocument>("SELECT CONVERT(xml, '<data/>')", new { });

            ClassicAssert.IsNotNull(list);
            var doc = list[0];
            ClassicAssert.IsNotNull(doc);
            ClassicAssert.AreEqual("<data />", doc.ToString());
        }

        [Test]
        public void XmlSingleColumnCanDeserializeToString()
        {
            var list = Connection().QuerySql<string>("SELECT CONVERT(xml, '<data/>')", new { });

            ClassicAssert.IsNotNull(list);
            var s = list[0];
            ClassicAssert.IsNotNull(s);
            ClassicAssert.AreEqual("<data />", s);
        }
        #endregion

        #region Xml Column Deserialization Tests
        [Test]
        public void XmlColumnCanDeserializeToString()
        {
            var list = Connection().QuerySql<Result>("SELECT String=CONVERT(xml, '<Data><Text>foo</Text></Data>')", new { });

            ClassicAssert.IsNotNull(list);
            var result = list[0];
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsNotNull(result.String);
            ClassicAssert.AreEqual("<Data><Text>foo</Text></Data>", result.String);
        }

        //[Test]
        //public void XmlColumnCanDeserializeToXmlDocument()
        //{
        //    var list = Connection().QuerySql<Result>("SELECT XmlDocument=CONVERT(xml, '<Data><Text>foo</Text></Data>')", new { });

        //    ClassicAssert.IsNotNull(list);
        //    var result = list[0];
        //    ClassicAssert.IsNotNull(result);
        //    ClassicAssert.IsNotNull(result.jsonDocument);
        //    ClassicAssert.AreEqual("<Data><Text>foo</Text></Data>", result.jsonDocument);
        //}

        [Test]
        public void XmlColumnCanDeserializeToXDocument()
        {
            var list = Connection().QuerySql<Result>("SELECT XDocument=CONVERT(xml, '<Data><Text>foo</Text></Data>')", new { });

            ClassicAssert.IsNotNull(list);
            var result = list[0];
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsNotNull(result.jsonNode);
            ClassicAssert.AreEqual(String.Format("<Data>{0}  <Text>foo</Text>{0}</Data>", Environment.NewLine), result.jsonNode.ToString());
        }

        [Test]
        public void XmlColumnCanDeserializeToObject()
        {
            var list = Connection().QuerySql<Result>("SELECT Data=CONVERT(xml, '<XmlTests.Data xmlns=\"http://schemas.datacontract.org/2004/07/Insight.Tests.MsSqlClient\"><Text>foo</Text></XmlTests.Data>')", new { });

            ClassicAssert.IsNotNull(list);
            var result = list[0];
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsNotNull(result.Data);
            ClassicAssert.AreEqual("foo", result.Data.Text);
        }
        #endregion

        #region Serialization Tests
        [Test]
        public void XmlDocumentCanSerializeToXmlParameter()
        {
            // create a document
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Data><Text>foo</Text></Data>");

            var list = Connection().Query<XmlDocument>("ReflectXml", new { Xml = doc });
            var data = list[0];
            ClassicAssert.IsNotNull(data);
            ClassicAssert.AreEqual(doc.OuterXml, data.OuterXml);
        }

        [Test]
        public void XDocumentCanSerializeToXmlParameter()
        {
            // create a document
            XDocument doc = XDocument.Parse("<Data><Text>foo</Text></Data>");

            var list = Connection().Query<XDocument>("ReflectXml", new { Xml = doc });
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

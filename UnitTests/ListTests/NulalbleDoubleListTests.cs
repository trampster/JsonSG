using NUnit.Framework;
using JsonSrcGen;
using System.Collections.Generic;
using System;

[assembly: JsonList(typeof(double?))] 

namespace UnitTests.ListTests
{
    public class NullableDoubleListTests
    { 
        JsonSrcGen.JsonConverter _convert;

        string ExpectedJson = "[42.21,176.568,1.7976931348623157E+308,-1.7976931348623157E+308,null,0]";

        [SetUp]
        public void Setup()
        {
            _convert = new JsonConverter();
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-us");
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture;

        }

        [Test] 
        public void ToJson_CorrectString()
        {
            //arrange
            var list = new List<double?>(){42.21d, 176.568d, double.MaxValue, double.MinValue, null, 0};

            //act
            var json = _convert.ToJson(list);

            //assert
            Assert.That(json.ToString(), Is.EqualTo(ExpectedJson));
        }

        [Test]
        public void ToJson_Null_CorrectString()
        {
            //arrange
            //act
            var json = _convert.ToJson((List<double?>)null);

            //assert
            Assert.That(json.ToString(), Is.EqualTo("null"));
        }

        [Test]
        public void FromJson_EmptyList_CorrectList()
        {
            //arrange
            var list = new List<double?>();

            //act
            _convert.FromJson(list, ExpectedJson);

            //assert
            Assert.That(list.Count, Is.EqualTo(6));
            Assert.That(list[0], Is.EqualTo(42.21d));
            Assert.That(list[1], Is.EqualTo(176.568d));
            Assert.That(list[2], Is.EqualTo(double.MaxValue));
            Assert.That(list[3], Is.EqualTo(double.MinValue));
            Assert.That(list[4], Is.Null);
            Assert.That(list[5], Is.EqualTo(0));
        }

        [Test] 
        public void FromJson_PopulatedList_CorrectList()
        {
            //arrange
            var list = new List<double?>(){1, 2, 3};

            //act
            list =_convert.FromJson(list, ExpectedJson);

            //assert
            Assert.That(list.Count, Is.EqualTo(6));
            Assert.That(list[0], Is.EqualTo(42.21d));
            Assert.That(list[1], Is.EqualTo(176.568d));
            Assert.That(list[2], Is.EqualTo(double.MaxValue));
            Assert.That(list[3], Is.EqualTo(double.MinValue));
            Assert.That(list[4], Is.Null);
            Assert.That(list[5], Is.EqualTo(0));
        }

        [Test] 
        public void FromJson_JsonNull_ReturnsNull()
        {
            //arrange
            var list = new List<double?>(){1, 2, 3};

            //act
            list = _convert.FromJson(list, "null");

            //assert
            Assert.That(list, Is.Null);
        }

        [Test]
        public void FromJson_ListNull_MakesList()
        {
            //arrange
            //act
            var list = _convert.FromJson((List<double?>)null, ExpectedJson);

            //assert
            Assert.That(list.Count, Is.EqualTo(6));
            Assert.That(list[0], Is.EqualTo(42.21d));
            Assert.That(list[1], Is.EqualTo(176.568d));
            Assert.That(list[2], Is.EqualTo(double.MaxValue));
            Assert.That(list[3], Is.EqualTo(double.MinValue));
            Assert.That(list[4], Is.Null);
            Assert.That(list[5], Is.EqualTo(0));
        }
    }
}
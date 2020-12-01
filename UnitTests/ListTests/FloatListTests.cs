using NUnit.Framework;
using JsonSrcGen;
using System.Collections.Generic;
using System;

[assembly: JsonList(typeof(float))] 

namespace UnitTests.ListTests
{
    public class FloatListTests
    { 
        JsonSrcGen.JsonConverter _convert;

        string ExpectedJson = "[42.21,176.568,3.4028235E+38,-3.4028235E+38,0]";

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
            var list = new List<float>(){42.21f, 176.568f, float.MaxValue, float.MinValue, 0};

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
            var json = _convert.ToJson((List<float>)null);

            //assert
            Assert.That(json.ToString(), Is.EqualTo("null"));
        }

        [Test]
        public void FromJson_EmptyList_CorrectList()
        {
            //arrange
            var list = new List<float>();

            //act
            _convert.FromJson(list, ExpectedJson);

            //assert
            Assert.That(list.Count, Is.EqualTo(5));
            Assert.That(list[0], Is.EqualTo(42.21f));
            Assert.That(list[1], Is.EqualTo(176.568f));
            Assert.That(list[2], Is.EqualTo(float.MaxValue));
            Assert.That(list[3], Is.EqualTo(float.MinValue));
            Assert.That(list[4], Is.EqualTo(0));
        }

        [Test] 
        public void FromJson_PopulatedList_CorrectList()
        {
            //arrange
            var list = new List<float>(){1, 2, 3};

            //act
            list =_convert.FromJson(list, ExpectedJson);

            //assert
            Assert.That(list.Count, Is.EqualTo(5));
            Assert.That(list[0], Is.EqualTo(42.21f));
            Assert.That(list[1], Is.EqualTo(176.568f));
            Assert.That(list[2], Is.EqualTo(float.MaxValue));
            Assert.That(list[3], Is.EqualTo(float.MinValue));
            Assert.That(list[4], Is.EqualTo(0));
        }

        [Test] 
        public void FromJson_JsonNull_ReturnsNull()
        {
            //arrange
            var list = new List<float>(){1, 2, 3};

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
            var list = _convert.FromJson((List<float>)null, ExpectedJson);

            //assert
            Assert.That(list.Count, Is.EqualTo(5));
            Assert.That(list[0], Is.EqualTo(42.21f));
            Assert.That(list[1], Is.EqualTo(176.568f));
            Assert.That(list[2], Is.EqualTo(float.MaxValue));
            Assert.That(list[3], Is.EqualTo(float.MinValue));
            Assert.That(list[4], Is.EqualTo(0));
        }
    }
}
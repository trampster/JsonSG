using NUnit.Framework;
using JsonSrcGen;
using System.Collections.Generic;
using System;

[assembly: JsonList(typeof(long?))] 

namespace UnitTests.ListTests
{
    public class NullableLongListTests
    { 
        JsonSrcGen.JsonConverter _convert;

        string ExpectedJson = "[-9223372036854775808,-1,0,1,42,null,9223372036854775807]";

        [SetUp]
        public void Setup()
        {
            _convert = new JsonConverter();
        }

        [Test] 
        public void ToJson_CorrectString()
        {
            //arrange
            var list = new List<long?>(){long.MinValue,-1,0, 1, 42, null, long.MaxValue};

            //act
            var json = JsonConverter.ToJson(list);

            //assert
            Assert.That(json.ToString(), Is.EqualTo(ExpectedJson));
        }

        [Test]
        public void ToJson_Null_CorrectString()
        {
            //arrange
            //act
            var json = JsonConverter.ToJson((List<long?>)null);

            //assert
            Assert.That(json.ToString(), Is.EqualTo("null"));
        }

        [Test]
        public void FromJson_EmptyList_CorrectList()
        {
            //arrange
            var list = new List<long?>();

            //act
            JsonConverter.FromJson(list, ExpectedJson);

            //assert
            Assert.That(list.Count, Is.EqualTo(7));
            Assert.That(list[0], Is.EqualTo(long.MinValue));
            Assert.That(list[1], Is.EqualTo(-1));
            Assert.That(list[2], Is.EqualTo(0));
            Assert.That(list[3], Is.EqualTo(1));
            Assert.That(list[4], Is.EqualTo(42));
            Assert.That(list[5], Is.Null);
            Assert.That(list[6], Is.EqualTo(long.MaxValue));
        }

        [Test] 
        public void FromJson_PopulatedList_CorrectList()
        {
            //arrange
            var list = new List<long?>(){1, 2, 3};

            //act
            list =JsonConverter.FromJson(list, ExpectedJson);

            //assert
            Assert.That(list.Count, Is.EqualTo(7));
            Assert.That(list[0], Is.EqualTo(long.MinValue));
            Assert.That(list[1], Is.EqualTo(-1));
            Assert.That(list[2], Is.EqualTo(0));
            Assert.That(list[3], Is.EqualTo(1));
            Assert.That(list[4], Is.EqualTo(42));
            Assert.That(list[5], Is.Null);
            Assert.That(list[6], Is.EqualTo(long.MaxValue));
        }

        [Test] 
        public void FromJson_JsonNull_ReturnsNull()
        {
            //arrange
            var list = new List<long?>(){1, 2, 3};

            //act
            list = JsonConverter.FromJson(list, "null");

            //assert
            Assert.That(list, Is.Null);
        }

        [Test]
        public void FromJson_ListNull_MakesList()
        {
            //arrange
            //act
            var list = JsonConverter.FromJson((List<long?>)null, ExpectedJson);

            //assert
            Assert.That(list.Count, Is.EqualTo(7));
            Assert.That(list[0], Is.EqualTo(long.MinValue));
            Assert.That(list[1], Is.EqualTo(-1));
            Assert.That(list[2], Is.EqualTo(0));
            Assert.That(list[3], Is.EqualTo(1));
            Assert.That(list[4], Is.EqualTo(42));
            Assert.That(list[5], Is.Null);
            Assert.That(list[6], Is.EqualTo(long.MaxValue));
        }
    }
}
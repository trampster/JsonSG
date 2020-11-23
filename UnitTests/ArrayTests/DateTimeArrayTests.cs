//using NUnit.Framework;
//using JsonSrcGen;
//using System.Collections.Generic;
//using System;

//[assembly: JsonArray(typeof(DateTime))]

//namespace UnitTests.ArrayTests
//{
//    public class DateTimeArrayTests
//    {
//        JsonSrcGen.JsonConverter _convert;

//        string ExpectedJson = "[\"2017-07-25T00:00:00\",\"2017-07-25T23:59:58\",\"2017-07-25T23:59:58.196\"]";

//        [SetUp]
//        public void Setup()
//        {
//            _convert = new JsonConverter();
//        }

//        [Test]
//        public void ToJson_CorrectString()
//        {
//            //arrange
//            var array = new DateTime[] { new DateTime(2017, 7, 25), new DateTime(2017, 7, 25, 23, 59, 58), new DateTime(2017, 7, 25, 23, 59, 58).AddMilliseconds(196) };

//            //act
//            var json = JsonConverter.ToJson(array);

//            //assert
//            Assert.That(json.ToString(), Is.EqualTo(ExpectedJson));
//        }

//        [Test]
//        public void ToJson_Null_CorrectString()
//        {
//            //arrange
//            //act
//            var json = JsonConverter.ToJson((DateTime[])null);

//            //assert
//            Assert.That(json.ToString(), Is.EqualTo("null"));
//        }

//        [Test]
//        public void FromJson_EmptyArray_CorrectArray()
//        {
//            //arrange
//            var array = new DateTime[] { };

//            //act
//            JsonConverter.FromJson(array, ExpectedJson);

//            //assert
//            Assert.That(array.Length, Is.EqualTo(3));
//            Assert.That(array[0], Is.EqualTo(new DateTime(2017, 7, 25)));
//            Assert.That(array[1], Is.EqualTo(new DateTime(2017, 7, 25, 23, 59, 58)));
//            Assert.That(array[2], Is.EqualTo(new DateTime(2017, 7, 25, 23, 59, 58).AddMilliseconds(196)));
//        }

//        [Test]
//        public void FromJson_PopulatedArray_CorrectArray()
//        {
//            //arrange
//            var array = new DateTime[] { DateTime.Now, DateTime.Now, DateTime.Now };

//            //act
//            JsonConverter.FromJson(array, ExpectedJson);

//            //assert
//            Assert.That(array.Length, Is.EqualTo(3));
//            Assert.That(array[0], Is.EqualTo(new DateTime(2017, 7, 25)));
//            Assert.That(array[1], Is.EqualTo(new DateTime(2017, 7, 25, 23, 59, 58)));
//            Assert.That(array[2], Is.EqualTo(new DateTime(2017, 7, 25, 23, 59, 58).AddMilliseconds(196)));
//        }

//        [Test]
//        public void FromJson_JsonNull_ReturnsNull()
//        {
//            //arrange
//            var array = new DateTime[] { DateTime.Now, DateTime.Now, DateTime.Now };

//            //act
//            JsonConverter.FromJson(array, "null");

//            //assert
//            Assert.That(array, Is.Null);
//        }

//        [Test]
//        public void FromJson_ArrayNull_MakesArray()
//        {
//            //arrange
//            //act
//            var array = JsonConverter.FromJson((DateTime[])null, ExpectedJson);

//            //assert
//            Assert.That(array.Length, Is.EqualTo(3));
//            Assert.That(array[0], Is.EqualTo(new DateTime(2017, 7, 25)));
//            Assert.That(array[1], Is.EqualTo(new DateTime(2017, 7, 25, 23, 59, 58)));
//            Assert.That(array[2], Is.EqualTo(new DateTime(2017, 7, 25, 23, 59, 58).AddMilliseconds(196)));
//        }
//    }
//}
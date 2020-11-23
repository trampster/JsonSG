using NUnit.Framework;
using JsonSrcGen;
using System.Collections.Generic;


namespace UnitTests
{
    [Json]
    public class JsonListClass
    {
        public System.Collections.Generic.List<bool> BooleanList {get;set;} 
    }

    public class ListPropertyTests
    { 
        JsonSrcGen.JsonConverter _convert;

        string ExpectedJson = $"{{\"BooleanList\":[true,false]}}";

        [SetUp]
        public void Setup()
        {
            _convert = new JsonConverter();
        }

        [Test] 
        public void ToJson_CorrectString()
        {
            //arrange
            var jsonClass = new JsonListClass()
            {
                BooleanList = new List<bool>(){true, false}
            };

            //act
            var json =JsonConverter.ToJson(jsonClass);

            //assert
            Assert.That(json.ToString(), Is.EqualTo(ExpectedJson));
        }

        [Test]
        public void ToJson_Null_CorrectString()
        {
            //arrange
            var jsonClass = new JsonListClass()
            {
                BooleanList = null
            };

            //act
            var json =JsonConverter.ToJson(jsonClass);

            //assert
            Assert.That(json.ToString(), Is.EqualTo("{\"BooleanList\":null}"));
        }

        [Test]
        public void FromJson_EmptyList_CorrectList()
        {
            //arrange
            var jsonClass = new JsonListClass()
            {
                BooleanList = new List<bool>()
            };

            //act
           JsonConverter.FromJson(jsonClass, ExpectedJson);

            //assert
            Assert.That(jsonClass.BooleanList.Count, Is.EqualTo(2));
            Assert.That(jsonClass.BooleanList[0], Is.True);
            Assert.That(jsonClass.BooleanList[1], Is.False);
        }

        [Test]
        public void FromJson_NullList_CorrectList()
        {
            //arrange
            var jsonClass = new JsonListClass()
            {
                BooleanList = null
            };

            //act
           JsonConverter.FromJson(jsonClass, ExpectedJson);

            //assert
            Assert.That(jsonClass.BooleanList.Count, Is.EqualTo(2));
            Assert.That(jsonClass.BooleanList[0], Is.True);
            Assert.That(jsonClass.BooleanList[1], Is.False);
        }

        [Test] 
        public void FromJson_PopulatedList_CorrectList()
        {
            //arrange
            var jsonClass = new JsonListClass()
            {
                BooleanList = new List<bool>(){false, false, false}
            };

            //act
           JsonConverter.FromJson(jsonClass, ExpectedJson);

            //assert
            Assert.That(jsonClass.BooleanList.Count, Is.EqualTo(2));
            Assert.That(jsonClass.BooleanList[0], Is.True);
            Assert.That(jsonClass.BooleanList[1], Is.False);
        }
    }
}
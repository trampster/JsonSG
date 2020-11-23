using NUnit.Framework;
using JsonSrcGen;

namespace UnitTests
{
    [Json]
    public class JsonArrayClass
    {
        public bool[] BooleanArray {get;set;} 
    }

    public class ArrayPropertyTests
    { 
        JsonSrcGen.JsonConverter _convert;

        string ExpectedJson = $"{{\"BooleanArray\":[true,false]}}";

        [SetUp]
        public void Setup()
        {
            _convert = new JsonConverter();
        }

        [Test] 
        public void ToJson_CorrectString()
        {
            //arrange
            var jsonClass = new JsonArrayClass()
            {
                BooleanArray = new bool[]{true, false}
            };

            //act
            var json =JsonConverter.ToJson(jsonClass);

            //assert
            Assert.That(json.ToString(), Is.EqualTo(ExpectedJson.ToString()));
        }

        [Test]
        public void ToJson_Null_CorrectString()
        {
            //arrange
            var jsonClass = new JsonArrayClass()
            {
                BooleanArray = null
            };

            //act
            var json =JsonConverter.ToJson(jsonClass);

            //assert
            Assert.That(json.ToString(), Is.EqualTo("{\"BooleanArray\":null}"));
        }

        [Test]
        public void FromJson_EmptyArray_CorrectArray()
        {
            //arrange
            var jsonClass = new JsonArrayClass()
            {
                BooleanArray = new bool[0]
            };

            //act
           JsonConverter.FromJson(jsonClass, ExpectedJson);

            //assert
            Assert.That(jsonClass.BooleanArray.Length, Is.EqualTo(2));
            Assert.That(jsonClass.BooleanArray[0], Is.True);
            Assert.That(jsonClass.BooleanArray[1], Is.False);
        }

        [Test]
        public void FromJson_NullArray_CorrectArray()
        {
            //arrange
            var jsonClass = new JsonArrayClass()
            {
                BooleanArray = null
            };

            //act
           JsonConverter.FromJson(jsonClass, ExpectedJson);

            //assert
            Assert.That(jsonClass.BooleanArray.Length, Is.EqualTo(2));
            Assert.That(jsonClass.BooleanArray[0], Is.True);
            Assert.That(jsonClass.BooleanArray[1], Is.False);
        }

        [Test] 
        public void FromJson_PopulatedArray_CorrectArray()
        {
            //arrange
            var jsonClass = new JsonArrayClass()
            {
                BooleanArray = new bool[]{false, false, false}
            };

            //act
           JsonConverter.FromJson(jsonClass, ExpectedJson);

            //assert
            Assert.That(jsonClass.BooleanArray.Length, Is.EqualTo(2));
            Assert.That(jsonClass.BooleanArray[0], Is.True);
            Assert.That(jsonClass.BooleanArray[1], Is.False);
        }
    }
}
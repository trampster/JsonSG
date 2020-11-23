using NUnit.Framework;
using JsonSrcGen;

namespace UnitTests
{
    [Json]
    public class JsonByteClass
    {
        public byte Age {get;set;}
        public byte Height {get;set;}
        public byte Min {get;set;}
        public byte Max {get;set;}
    }

    public class BytePropertyTests
    {
        JsonSrcGen.JsonConverter _convert;
        const string ExpectedJson = "{\"Age\":42,\"Height\":176,\"Max\":255,\"Min\":0}";

        [SetUp]
        public void Setup()
        {
            _convert = new JsonConverter();
        }

        [Test]
        public void ToJson_CorrectString()
        {
            //arrange
            var jsonClass = new JsonByteClass()
            {
                Age = 42,
                Height = 176,
                Max = byte.MaxValue,
                Min = byte.MinValue
            };

            //act
            var json =JsonConverter.ToJson(jsonClass);

            //assert
            Assert.That(json.ToString(), Is.EqualTo(ExpectedJson));
        }

        [Test]
        public void FromJson_CorrectJsonClass()
        {
            //arrange
            var json = ExpectedJson;
            var jsonClass = new JsonByteClass();

            //act
           JsonConverter.FromJson(jsonClass, json);

            //assert
            Assert.That(jsonClass.Age, Is.EqualTo(42));
            Assert.That(jsonClass.Height, Is.EqualTo(176));
            Assert.That(jsonClass.Min, Is.EqualTo(byte.MinValue));
            Assert.That(jsonClass.Max, Is.EqualTo(byte.MaxValue));
        }
    }
}
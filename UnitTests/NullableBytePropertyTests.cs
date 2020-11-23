using NUnit.Framework;
using JsonSrcGen;

namespace UnitTests
{
    [Json]
    public class JsonNullableByteClass
    {
        public byte? Age {get;set;}
        public byte? Height {get;set;}
        public byte? Min {get;set;}
        public byte? Max {get;set;}
        public byte? Null {get;set;}
    }

    public class NullableBytePropertyTests
    {
        JsonSrcGen.JsonConverter _convert;
        const string ExpectedJson = "{\"Age\":42,\"Height\":176,\"Max\":255,\"Min\":0,\"Null\":null}";

        [SetUp]
        public void Setup()
        {
            _convert = new JsonConverter();
        }

        [Test]
        public void ToJson_CorrectString()
        {
            //arrange
            var jsonClass = new JsonNullableByteClass()
            {
                Age = 42,
                Height = 176,
                Max = byte.MaxValue,
                Min = byte.MinValue,
                Null = null
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
            var jsonClass = new JsonNullableByteClass();

            //act
           JsonConverter.FromJson(jsonClass, json);

            //assert
            Assert.That(jsonClass.Age, Is.EqualTo(42));
            Assert.That(jsonClass.Height, Is.EqualTo(176));
            Assert.That(jsonClass.Min, Is.EqualTo(byte.MinValue));
            Assert.That(jsonClass.Max, Is.EqualTo(byte.MaxValue));
            Assert.That(jsonClass.Null, Is.Null);
        }
    }
}
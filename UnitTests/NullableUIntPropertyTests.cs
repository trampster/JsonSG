using NUnit.Framework;
using JsonSrcGen;


namespace UnitTests
{
    [Json]
    public class JsonNullableUIntClass
    {
        public uint? Age {get;set;}
        public uint? Height {get;set;}
        public uint? Min {get;set;}
        public uint? Max {get;set;}
        public uint? Null {get;set;}
    }

    public class NullableUIntPropertyTests
    {
        JsonSrcGen.JsonConverter _convert;
        const string ExpectedJson = "{\"Age\":42,\"Height\":176,\"Max\":4294967295,\"Min\":0,\"Null\":null}";

        [SetUp]
        public void Setup()
        {
            _convert = new JsonConverter();
        }

        [Test]
        public void ToJson_CorrectString()
        {
            //arrange
            var jsonClass = new JsonNullableUIntClass()
            {
                Age = 42,
                Height = 176,
                Max = uint.MaxValue,
                Min = uint.MinValue,
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
            var jsonClass = new JsonNullableUIntClass();

            //act
           JsonConverter.FromJson(jsonClass, json);

            //assert
            Assert.That(jsonClass.Age, Is.EqualTo(42));
            Assert.That(jsonClass.Height, Is.EqualTo(176));
            Assert.That(jsonClass.Min, Is.EqualTo(uint.MinValue));
            Assert.That(jsonClass.Max, Is.EqualTo(uint.MaxValue));
            Assert.That(jsonClass.Null, Is.Null);
        }
    }
}
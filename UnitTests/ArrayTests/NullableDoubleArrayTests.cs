using NUnit.Framework;
using JsonSrcGen;
using System.Text;

[assembly: JsonArray(typeof(double?))] 

namespace UnitTests.ArrayTests
{
    public class NullableDoubleArrayTests : NullableDoubleArrayTestsBase
    {
        protected override string ToJson(double?[] json)
        {
            return _convert.ToJson(json).ToString();
        }

        protected override double?[] FromJson(double?[] value, string json)
        {
            return _convert.FromJson(value, json);
        }
    }

    public class Utf8NullableDoubleArrayTests : NullableDoubleArrayTestsBase
    {
        protected override string ToJson(double?[] json)
        {
            var jsonUtf8 = _convert.ToJsonUtf8(json); 
            return Encoding.UTF8.GetString(jsonUtf8);
        }

        protected override double?[] FromJson(double?[] value, string json)
        {
            return _convert.FromJson(value, Encoding.UTF8.GetBytes(json));
        }
    }

    public abstract class NullableDoubleArrayTestsBase
    { 
        protected JsonSrcGen.JsonConverter _convert;

        string ExpectedJson = "[42.21,176.568,1.7976931348623157E+308,-1.7976931348623157E+308,null,0]";

        [SetUp]
        public void Setup()
        {
            _convert = new JsonConverter();
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-us");
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture;

        }
        protected abstract string ToJson(double?[] json);

        [Test] 
        public void ToJson_CorrectString()
        {
            //arrange
            var array = new double?[]{42.21d, 176.568d, double.MaxValue, double.MinValue, null, 0};

            //act
            var json = ToJson(array);

            //assert
            Assert.That(json.ToString(), Is.EqualTo(ExpectedJson));
        }

        [Test]
        public void ToJson_Null_CorrectString()
        {
            //arrange
            //act
            var json = _convert.ToJson((double?[])null);

            //assert
            Assert.That(json.ToString(), Is.EqualTo("null"));
        }

        protected abstract double?[] FromJson(double?[] value, string json);

        [Test]
        public void FromJson_EmptyList_CorrectList()
        {
            //arrange
            var array = new double?[]{};

            //act
            array = FromJson(array, ExpectedJson);

            //assert
            Assert.That(array.Length, Is.EqualTo(6));
            Assert.That(array[0], Is.EqualTo(42.21d));
            Assert.That(array[1], Is.EqualTo(176.568d));
            Assert.That(array[2], Is.EqualTo(double.MaxValue));
            Assert.That(array[3], Is.EqualTo(double.MinValue));
            Assert.That(array[4], Is.Null);
            Assert.That(array[5], Is.EqualTo(0));
        }

        [Test] 
        public void FromJson_PopulatedList_CorrectList()
        {
            //arrange
            var array = new double?[]{1, 2, 3};

            //act
            array = FromJson(array, ExpectedJson);

            //assert
            Assert.That(array.Length, Is.EqualTo(6));
            Assert.That(array[0], Is.EqualTo(42.21d));
            Assert.That(array[1], Is.EqualTo(176.568d));
            Assert.That(array[2], Is.EqualTo(double.MaxValue));
            Assert.That(array[3], Is.EqualTo(double.MinValue));
            Assert.That(array[4], Is.Null);
            Assert.That(array[5], Is.EqualTo(0));
        }

        [Test] 
        public void FromJson_JsonNull_ReturnsNull()
        {
            //arrange
            var array = new double?[]{1, 2, 3};

            //act
            array = FromJson(array, "null");

            //assert
            Assert.That(array, Is.Null);
        }

        [Test]
        public void FromJson_ListNull_MakesList()
        {
            //arrange
            //act
            var array = FromJson((double?[])null, ExpectedJson);

            //assert
            Assert.That(array.Length, Is.EqualTo(6));
            Assert.That(array[0], Is.EqualTo(42.21d));
            Assert.That(array[1], Is.EqualTo(176.568d));
            Assert.That(array[2], Is.EqualTo(double.MaxValue));
            Assert.That(array[3], Is.EqualTo(double.MinValue));
            Assert.That(array[4], Is.Null);
            Assert.That(array[5], Is.EqualTo(0));
        }
    }
}
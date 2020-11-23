using NUnit.Framework;
using JsonSrcGen;
using System;

[assembly: GenerationOutputFolder("/home/daniel/Work/JsonSrcGen/Generated")]

namespace CustomConverterTests
{
    [CustomConverter(typeof(DateTime))]
    public class CustomDateTimeConverter : ICustomConverter<DateTime>
    {
        public void ToJson(IJsonBuilder builder, DateTime target)
        {
            builder.Append("\"");
            builder.Append(target.ToString());
            builder.Append("\""); 
        }

        public ReadOnlySpan<char> FromJson(ReadOnlySpan<char> json, ref DateTime value)
        {
            json = json.SkipWhitespace();
            if(json[0] != '\"')
            {
                throw new InvalidJsonException("DateTime should start with a quote", json);
            }
            json = json.Slice(1);

            var dateTimeSpan = json.ReadTo('\"');

            value = DateTime.Parse(dateTimeSpan);

            return json.Slice(dateTimeSpan.Length + 1); 
        }
    }

    [Json]
    public class CustomClass 
    {
        public DateTime DateTime{get;set;}
    }

    public class CustomDateTimeConverterTests
    {
        [Test]
        public void ToJson_CorrectJson() 
        {
            //arrange
            var dateTime = DateTime.MinValue;

            //act
            var json = JsonConverter.ToJson(new CustomClass(){DateTime=dateTime}); 

            //assert
            Assert.That(json.ToString(), Is.EqualTo($"{{\"DateTime\":\"{dateTime}\"}}"));
        }

        [Test]
        public void FromJson_CorrectDateTime() 
        {
            //arrange
            var customClass = new CustomClass();

            //act
            JsonConverter.FromJson(customClass, "{\"DateTime\":\"1/01/0001 12:00:00 AM\"}"); 

            //assert
            Assert.That(customClass.DateTime, Is.EqualTo(DateTime.MinValue));
        }
    }
}
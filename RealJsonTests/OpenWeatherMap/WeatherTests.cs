using JsonSrcGen;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System;

namespace JsonSrcGen.RealJsonTests.OpenWeatherMap
{
    /// <summary>
    /// Test data retrieved from https://api.openweathermap.org/data/2.5/onecall?lat=33.441792&lon=-94.037689&appid={apiKey}
    /// </summary>
    public class WeatherTests
    {
        string _json;
        JsonConverter _converter;

        [SetUp]
        public void Setup()
        {
            _json = File.ReadAllText(Path.Combine("OpenWeatherMap","Weather.json"));
            _converter = new JsonConverter();
        }

        [Test]
        public void FromJson_CorrectTopLevel()
        {
            // arrange
            Weather weather = new Weather();
            
            // act
            _converter.FromJson(weather, _json);

            // assert
            Assert.That(weather.Latitude, Is.EqualTo(33.44f));
            Assert.That(weather.Longitude, Is.EqualTo(-94.04f));
            Assert.That(weather.Timezone, Is.EqualTo("America/Chicago"));
            Assert.That(weather.TimezoneOffset, Is.EqualTo(-18000));
        }

        [Test]
        public void FromJson_CorrectCurrent()
        {
            // arrange
            Weather weather = new Weather();
            
            // act
            _converter.FromJson(weather, _json);

            // assert
            var current = weather.Current;
            Assert.That(current.Dt, Is.EqualTo(1603185587));
            Assert.That(current.Sunrise, Is.EqualTo(1603196699));
            Assert.That(current.Temp, Is.EqualTo(294.59f));
            Assert.That(current.FeelsLike, Is.EqualTo(296.17f));
            Assert.That(current.Pressure, Is.EqualTo(1017));
            Assert.That(current.Humidity, Is.EqualTo(88));
            Assert.That(current.DewPoint, Is.EqualTo(292.52f));
            Assert.That(current.Uvi, Is.EqualTo(5.21f));
            Assert.That(current.Clouds, Is.EqualTo(20));
            Assert.That(current.Visibility, Is.EqualTo(10000));
            Assert.That(current.WindSpeed, Is.EqualTo(2.6f));
            Assert.That(current.WindDeg, Is.EqualTo(150));
            Assert.That(current.Weather.Length, Is.EqualTo(1));
            Assert.That(current.Weather[0].Id, Is.EqualTo(801));
            Assert.That(current.Weather[0].Main, Is.EqualTo("Clouds"));
            Assert.That(current.Weather[0].Description, Is.EqualTo("few clouds"));
            Assert.That(current.Weather[0].Icon, Is.EqualTo("02n"));
        }

        [Test]
        public void FromJson_MinutelyCorrect()
        {
            // arrange
            Weather weather = new Weather();
            
            // act
            _converter.FromJson(weather, _json);

            // assert
            var minutely = weather.Minutely;

            Assert.That(minutely.Length, Is.EqualTo(61));

            // first
            Assert.That(minutely[0].Dt, Is.EqualTo(1603185600));
            Assert.That(minutely[0].Precipitation, Is.EqualTo(0));

            Assert.That(minutely[2].Dt, Is.EqualTo(1603185720));
            Assert.That(minutely[2].Precipitation, Is.EqualTo(0));

            // last
            Assert.That(minutely[60].Dt, Is.EqualTo(1603189200));
            Assert.That(minutely[60].Precipitation, Is.EqualTo(0));
        }

        [Test]
        public void FromJson_HourlyCorrect()
        {
            // arrange
            Weather weather = new Weather();
            
            // act
            _converter.FromJson(weather, _json);

            // assert
            var hourly = weather.Hourly;

            Assert.That(hourly.Length, Is.EqualTo(48));

            // first
            Assert.That(hourly[0].Dt, Is.EqualTo(1603184400));
            Assert.That(hourly[0].Temp, Is.EqualTo(294.59f));
            Assert.That(hourly[0].FeelsLike, Is.EqualTo(296.32f));
            Assert.That(hourly[0].Pressure, Is.EqualTo(1017));
            Assert.That(hourly[0].Humidity, Is.EqualTo(88));
            Assert.That(hourly[0].DewPoint, Is.EqualTo(292.52f));
            Assert.That(hourly[0].Clouds, Is.EqualTo(20));
            Assert.That(hourly[0].Visibility, Is.EqualTo(10000));
            Assert.That(hourly[0].WindSpeed, Is.EqualTo(2.39f));
            Assert.That(hourly[0].WindDeg, Is.EqualTo(184));
            Assert.That(hourly[0].Weather.Length, Is.EqualTo(1));
            Assert.That(hourly[0].Weather[0].Id, Is.EqualTo(501));
            Assert.That(hourly[0].Weather[0].Main, Is.EqualTo("Rain"));
            Assert.That(hourly[0].Weather[0].Description, Is.EqualTo("moderate rain"));
            Assert.That(hourly[0].Weather[0].Icon, Is.EqualTo("10n"));
            Assert.That(hourly[0].Pop, Is.EqualTo(0.88f));
            Assert.That(hourly[0].Rain.OneHour, Is.EqualTo(1.53f));

            // // last
            Assert.That(hourly[47].Dt, Is.EqualTo(1603353600));
            Assert.That(hourly[47].Temp, Is.EqualTo(292.79f));
            Assert.That(hourly[47].FeelsLike, Is.EqualTo(293.72f));
            Assert.That(hourly[47].Pressure, Is.EqualTo(1017));
            Assert.That(hourly[47].Humidity, Is.EqualTo(90));
            Assert.That(hourly[47].DewPoint, Is.EqualTo(291.13f));
            Assert.That(hourly[47].Clouds, Is.EqualTo(0));
            Assert.That(hourly[47].Visibility, Is.EqualTo(10000));
            Assert.That(hourly[47].WindSpeed, Is.EqualTo(2.64f));
            Assert.That(hourly[47].WindDeg, Is.EqualTo(153));
            Assert.That(hourly[47].Weather.Length, Is.EqualTo(1));
            Assert.That(hourly[47].Weather[0].Id, Is.EqualTo(800));
            Assert.That(hourly[47].Weather[0].Main, Is.EqualTo("Clear"));
            Assert.That(hourly[47].Weather[0].Description, Is.EqualTo("clear sky"));
            Assert.That(hourly[47].Weather[0].Icon, Is.EqualTo("01n"));
            Assert.That(hourly[47].Pop, Is.EqualTo(0f));
            Assert.That(hourly[47].Rain, Is.Null);
        }

        [Test]
        public void FromJson_DailyCorrect()
        {
            // arrange
            Weather weather = new Weather();
            
            // act
            _converter.FromJson(weather, _json);

            // assert
            var daily = weather.Daily;

            Assert.That(daily.Length, Is.EqualTo(8));

            // first
            Assert.That(daily[1].Dt, Is.EqualTo(1603303200));
            Assert.That(daily[1].Sunrise, Is.EqualTo(1603283148));
            Assert.That(daily[1].Sunset, Is.EqualTo(1603323315));
            Assert.That(daily[1].Temp.Day, Is.EqualTo(301.18f));
            Assert.That(daily[1].Temp.Min, Is.EqualTo(292f));
            Assert.That(daily[1].Temp.Max, Is.EqualTo(301.83f));
            Assert.That(daily[1].Temp.Night, Is.EqualTo(294.68f));
            Assert.That(daily[1].Temp.Eve, Is.EqualTo(297.13f));
            Assert.That(daily[1].Temp.Morn, Is.EqualTo(292f));
            Assert.That(daily[1].FeelsLike.Day, Is.EqualTo(302.18f));
            Assert.That(daily[1].FeelsLike.Night, Is.EqualTo(295.72f));
            Assert.That(daily[1].FeelsLike.Eve, Is.EqualTo(298.7f));
            Assert.That(daily[1].FeelsLike.Morn, Is.EqualTo(293.79f));
            Assert.That(daily[1].Pressure, Is.EqualTo(1018));
            Assert.That(daily[1].Humidity, Is.EqualTo(58));
            Assert.That(daily[1].DewPoint, Is.EqualTo(292.4f));
            Assert.That(daily[1].WindSpeed, Is.EqualTo(3.18f));
            Assert.That(daily[1].WindDeg, Is.EqualTo(155f));
            Assert.That(daily[1].Weather.Length, Is.EqualTo(1));
            Assert.That(daily[1].Weather[0].Id, Is.EqualTo(800));
            Assert.That(daily[1].Weather[0].Main, Is.EqualTo("Clear"));
            Assert.That(daily[1].Weather[0].Description, Is.EqualTo("clear sky"));
            Assert.That(daily[1].Weather[0].Icon, Is.EqualTo("01d"));
            Assert.That(daily[1].Clouds, Is.EqualTo(0));
            Assert.That(daily[1].Pop, Is.EqualTo(0));
            Assert.That(daily[1].Uvi, Is.EqualTo(4.92f));
        }
    }
}
using Bogus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DummyDataApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var Rooms = new[] { "DNR", "LVR", "BTR", "BDR" }; //부엌, 거실, 욕실, 침실

            var sensorDummy = new Faker<SensorInfo>()
                .RuleFor(o => o.DevId, f => f.PickRandom(Rooms))
                .RuleFor(o => o.CurrTime, f => f.Date.Past(0))
                .RuleFor(o => o.Temp, f => f.Random.Float(20.0f, 35.9f))
                .RuleFor(o => o.Humid, f => f.Random.Float(40.0f, 63.9f));

            while (true)
            {
                var currValue = sensorDummy.Generate();

                Console.WriteLine(JsonConvert.SerializeObject(currValue,Formatting.Indented));

                Thread.Sleep(1000);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WJ.ModelToDoc.model;

namespace WJ.ModelToDoc.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpGet("Test")]
        public string Test()
        {
            var type = typeof(User);
            var name = type.Name;
            var members = type.GetMembers();
            foreach (var mem in members)
            {
                var x = mem.Name;
            }
            var methods = type.GetMethods();
            foreach (var met in methods)
            {
                var y = met.Name;
                var TT = met.ReturnType.Name;
            }
            var atrs = type.GetCustomAttributes(typeof(TableAttribute), false);
            foreach (TableAttribute atr in atrs)
            {
                var x = atr.Name;
            }
            var pros = type.GetProperties();
            foreach (var pro in pros)
            {
                var x = pro.Name;
                var atrsx = pro.GetCustomAttributes(typeof(DisplayAttribute), false);
                foreach (DisplayAttribute atrsxx in atrsx)
                {
                    var b = atrsx;
                }
            }
            // var y = type.GetMembers(System.Reflection.BindingFlags.DeclaredOnly);
            /*
                DeclareOnly:仅获取指定类定义的方法，而不获取所继承的方法；
                Instance：获取实例方法
                NonPublic: 获取非公有方法
                Public： 获取共有方法
                Static：获取静态方法
             */
            return "反射";
        }
    }
}

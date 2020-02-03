using System; 
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Toss.Command;
using Toss.CommandHandler.Interfaces;
using Toss.Domain.Enums;

namespace Toss.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TossController : ControllerBase
    {
        private readonly ICommandHandler<TossCommand, Dictionary<string, PlayDecision>> _commandHandler;
        private readonly ILogger<TossController> _logger;

        public TossController(ICommandHandler<TossCommand, Dictionary<string, PlayDecision>> commandHandler, ILogger<TossController> logger)
        {
            _commandHandler = commandHandler;
            _logger = logger;
        }

        [HttpGet]
        [Route("decisions")]
        public IActionResult GetDecisions(string weather, string match)
        {
            try
            {
                var playWeather = getEnumValue<Weather>(weather);
                var playMatch = getEnumValue<MatchType>(match);

                Random random = new Random();
                var tossChances = new int[] { 1, 2 };

                int toss = tossChances[random.Next(0, tossChances.Length)];

                var result = _commandHandler.Handle(new TossCommand { Match = playMatch, Weather = playWeather, Randomize = toss });

                return Ok(result);
            }
            catch(ArgumentException ex)
            {
                return Ok("Please pass valid weather & match values");
            }
            catch(Exception ex)
            {
                _logger.LogCritical(ex.ToString());
                return Ok("Unexpected error encountered while getting Team Decisions");
            }
        }

        private T getEnumValue<T>(string value)
        {
            if (Enum.TryParse(typeof(T), value, true, out object returnValue))
            {
                return (T)returnValue;
            }

            throw new ArgumentException();
        }
    }
}


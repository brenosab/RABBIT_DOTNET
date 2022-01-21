using ADM_RT_CORE_LIB.Messaging.Interfaces;
using ADM_RT_CORE_LIB.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ADM_RT_UTILIDADES.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorController : ControllerBase
    {
        private readonly IMessageBus bus;

        public SensorController(IMessageBus bus)
        {
            this.bus = bus;
        }

        [HttpPost]
        public IActionResult PublishLuminosidade([FromBody] MessageData message)
        {
            try
            {
                bus.Publish(message,
                    Environment.GetEnvironmentVariable("APP_RABBIT_EXCHANGE"),
                    Environment.GetEnvironmentVariable("APP_RABBIT_ROUTINGKEY"));
                return Ok("Message published to queue");
            }
            catch (Exception ex)
            {
                return (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development") ?
                    BadRequest(ex.Message) :
                    BadRequest(ex.GetBaseException().Message);

            }
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

namespace MultiShop.RabbitMQMessageApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        [HttpPost]
        public IActionResult CreateMessage()
        {
            var connectionFactory = new ConnectionFactory()
            {
                HostName= "localhost",

            };
            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();
            return Ok("Mesajınız Kuyruga Alınmıstır.");
        }
    }
}

using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;
using RabbitMQ.Client;
using System.Text;

namespace RabbitMQ_WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }

        public void OnPost()
        {
            string message = "";

            var name = Request.Form["Name"][0];
            message += name + ".";

            var email = Request.Form["Email"][0];
            message += email + ".";

            var tours = Request.Form["Tours"][0];
            message += tours + ".";

            string book = "";
            try
            {
                book = Request.Form["Book"][0];

            }
            catch (Exception)
            {
                book = "off";

            }
            message += book + ".";

            string cancel = "";
            try
            {
                cancel = Request.Form["Cancel"][0];

            }
            catch (Exception)
            {
                cancel = "off";
            }
            message += cancel + ".";

            SendMessage(message);
        }

        private void SendMessage(string message)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "topic_logs",
                                        type: "topic");

                var routingKey = "anonymous.info";

                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "topic_logs",
                                     routingKey: routingKey,
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent '{0}':'{1}'", routingKey, message);
            }
        }



    }
}
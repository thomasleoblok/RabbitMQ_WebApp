using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;
using RabbitMQ.Client;
using System.Text;
using RabbitMQ.Client.Events;

namespace RabbitMQ_WebApp.Pages
{
    public class IndexModel : PageModel
    {
        public List<string> Response { get; set; } = new List<string>();
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
           // Thread ListenerThread = new Thread(new ThreadStart(ListenToMessage));
           // ListenerThread.Start();
            ListenToMessage();
        }

        public void OnGet()
        {
            
        }

        public void OnPost()
        {
            string message = "";
            var returnQueue = "anonymous.response";

            var correlation_id = Guid.NewGuid();
            message += correlation_id + ".";

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
                returnQueue = "bookingresponse";

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
                returnQueue = "cancel.response";

            }
            catch (Exception)
            {
                cancel = "off";
            }
            message += cancel += ".";

            message += returnQueue;

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

                string[] words = message.Split('.');
                for (int i = 0; i < words.Length; i++)
                {
                    if (i == 4 && words[i] == "on")
                    {
                        routingKey = "booking.info";
                    }
                    else if (i == 5 && words[i] == "on")
                    {
                        routingKey = "cancel.info";
                    }
                }

                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "topic_logs",
                                     routingKey: routingKey,
                                     basicProperties: null,
                                     body: body);
                //PrintToScreen($" [x] Sent '{routingKey}':'{message}'");
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;


                };
                channel.BasicConsume(queue: channel.QueueDeclare().QueueName,
                                     autoAck: false,
                                     consumer: consumer);
            }
        }

        private void ListenToMessage()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "topic_logs", type: "topic");
                var queueName = channel.QueueDeclare().QueueName;


                channel.QueueBind(queue: queueName,
                      exchange: "topic_logs",
                                  routingKey: "booking.response");

                PrintToScreen($"booking.response {DateTime.Now}");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;
                    PrintToScreen($" [x] Received '{routingKey}':'{message}'");
                };
                channel.BasicConsume(queue: queueName,
                                     autoAck: false,
                                     consumer: consumer);
            }

        }

        private void PrintToScreen(string message)
        {
            Response.Add(message);
        }
    }
}
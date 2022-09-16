using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.ExchangeDeclare(exchange: "topic_logs", type: "topic");
    var queueName = channel.QueueDeclare().QueueName;


    channel.QueueBind(queue: queueName,
          exchange: "topic_logs",
                      routingKey: "booking.response");


    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var routingKey = ea.RoutingKey;

        File.WriteAllText("C:\\Users\\thoma\\Documents\\Softwareudvikling\\2_Semester\\Systemintegration\\Kode\\RabbitMQ_WebApp\\WriteLines.txt", message);
    };
    channel.BasicConsume(queue: queueName,
                         autoAck: true,
                         consumer: consumer);
}
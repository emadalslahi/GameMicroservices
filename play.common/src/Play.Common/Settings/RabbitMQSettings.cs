 
namespace Play.Common.Service.Settings;

    public class RabbitMQSettings
    {
        public string Host { get; set; } = "localhost";

        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "user";
        public string Password { get; set; } = "password";
        /* public string VirtualHost { get; set; } = "/";
         public string ExchangeName { get; set; } = "play-catalog-exchange";
         public string QueueName { get; set; } = "play-catalog-queue";

         public bool UseSsl { get; set; } = false;
         public bool AutoDelete { get; set; } = true;
         */

    }

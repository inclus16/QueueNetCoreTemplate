using InclusService.Dto;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests.Helpers
{
    public abstract class QueueTestig
    {
        protected const string EXCHANGE_NAME = "TEST";

        protected const string QUEUE_NAME = "TEST";

        protected const string ROUTING_KEY = "TEST";


        protected void InitWorkspace(Connection connection)
        {
            ConnectionFactory factory = new ConnectionFactory
            {
                UserName = connection.UserName,
                Password = connection.Password,
                VirtualHost = connection.VirtualHost,
                HostName = connection.HostName
            };
            using (IConnection conn = factory.CreateConnection())
            {
                IModel model = conn.CreateModel();
                model.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Direct);
                model.QueueDeclare(QUEUE_NAME, false, false, false, null);
                model.QueueBind(QUEUE_NAME, EXCHANGE_NAME, ROUTING_KEY, null);
            }
        }

        protected Message GetMessage(Connection connection)
        {
            ConnectionFactory factory = new ConnectionFactory
            {
                UserName = connection.UserName,
                Password = connection.Password,
                VirtualHost = connection.VirtualHost,
                HostName = connection.HostName
            };
            using (IConnection conn = factory.CreateConnection())
            {
                IModel model = conn.CreateModel();
                BasicGetResult result = model.BasicGet(QUEUE_NAME, false);
                byte[] body = result.Body;
                return JsonConvert.DeserializeObject<Message>(Encoding.UTF8.GetString(body));
            }
        }

        protected void ReleaseWorkspace(Connection connection)
        {
            ConnectionFactory factory = new ConnectionFactory
            {
                UserName = connection.UserName,
                Password = connection.Password,
                VirtualHost = connection.VirtualHost,
                HostName = connection.HostName
            };
            using (IConnection conn = factory.CreateConnection())
            {
                IModel model = conn.CreateModel();
                model.QueueDelete(QUEUE_NAME);
                model.ExchangeDelete(EXCHANGE_NAME);
            }
        }
    }
}

using InclusService.Dto;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace InclusService.Services
{
    public class QueueAccessor
    {
        private readonly Connection ConnectionData;

        private bool IsConnected = false;

        private IConnection Connection;
        public QueueAccessor(Connection connection)
        {
            ConnectionData = connection;
        }
        public void Dispatch(Message message, string exchange,string routingKey)
        {
            if (Connection == null)
            {
                Connect();
            }
            IModel model = Connection.CreateModel();
            byte[] messageBodyBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            model.BasicPublish(exchange, routingKey, null, messageBodyBytes);
        }

        public void Watch()
        {
            if (Connection == null)
            {
                Connect();
            }
        }

        private void Connect()
        {
            ConnectionFactory factory = new ConnectionFactory
            {
                UserName = ConnectionData.UserName,
                Password = ConnectionData.Password,
                VirtualHost = ConnectionData.VirtualHost,
                HostName = ConnectionData.HostName
            };
            Connection = factory.CreateConnection();
        }
    }
}

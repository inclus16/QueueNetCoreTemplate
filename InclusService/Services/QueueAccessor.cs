using InclusService.Dto;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InclusService.Services
{
    public class QueueAccessor
    {
        private readonly Connection ConnectionData;
       

        private IConnection Connection;


        public delegate void InclomingMessageHadler(Message message);

        public event InclomingMessageHadler OnIncomingMessage;
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

        public void Watch(string queue)
        {
            if (Connection == null)
            {
                Connect();
            }
            IModel model = Connection.CreateModel();
            var consumer = new EventingBasicConsumer(model);
            consumer.Received += (ch, ea) =>
            {
                var body = ea.Body;
                model.BasicAck(ea.DeliveryTag, false);
            };
            while (Connection.IsOpen)
            {
                OnIncomingMessage?.Invoke(JsonConvert.DeserializeObject<Message>(model.BasicConsume(queue, false, consumer)));
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

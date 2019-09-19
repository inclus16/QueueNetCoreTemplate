using InclusService.Dto;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace InclusService.Services
{
    public class QueueAccessor
    {
        private readonly Connection ConnectionData;

        private IConnection Connection;

        public delegate void IncomingMessageHandler(Message message);

        public event IncomingMessageHandler OnIncomingMessage;
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
            while (Connection.IsOpen)
            {
                IModel model = Connection.CreateModel();
                BasicGetResult result = model.BasicGet(queue, false);
                byte[] body = result.Body;
                OnIncomingMessage?.Invoke(JsonConvert.DeserializeObject<Message>(Encoding.UTF8.GetString(body)));
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

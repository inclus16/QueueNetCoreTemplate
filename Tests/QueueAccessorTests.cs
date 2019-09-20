using InclusService.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tests.Helpers;
using Xunit;

namespace Tests
{
    public class QueueAccessorTests : QueueTestig
    {
        private readonly QueueAccessor Queue;

        private readonly InclusService.Dto.Connection Connection;
        public QueueAccessorTests()
        {
            Connection = new InclusService.Dto.Connection
            {
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/",
                HostName = "192.168.0.100"
            };
            Queue = new QueueAccessor(Connection);
        }

        [Fact]
        public void DispatchTest()
        {
            InitWorkspace(Connection);
            InclusService.Dto.Message sendedMessage = new InclusService.Dto.Message
            {
                Type = typeof(QueueAccessor),
                Data = "SOME_DATA"
            };
            Queue.Dispatch(sendedMessage,EXCHANGE_NAME,ROUTING_KEY) ;
            InclusService.Dto.Message receivedMessage = GetMessage(Connection);
            Assert.Equal(sendedMessage.Type.FullName, receivedMessage.Type.FullName);
            Assert.Equal(sendedMessage.Data, receivedMessage.Data);
            ReleaseWorkspace(Connection);
        }

        [Fact]
        public void WatchTest()
        {
            InitWorkspace(Connection);
            InclusService.Dto.Message sendedMessage = new InclusService.Dto.Message
            {
                Type = typeof(QueueAccessor),
                Data = "SOME_DATA"
            };
            Queue.Dispatch(sendedMessage,EXCHANGE_NAME,ROUTING_KEY);
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            Task task = new Task(()=>Queue.Watch(QUEUE_NAME),tokenSource.Token);
            Queue.OnIncomingMessage += (InclusService.Dto.Message incomingMessage) =>
            {
                Assert.Equal(sendedMessage.Type.FullName, incomingMessage.Type.FullName);
                Assert.Equal(sendedMessage.Data, incomingMessage.Data);
                tokenSource.Cancel();
            };

        }
    }
}

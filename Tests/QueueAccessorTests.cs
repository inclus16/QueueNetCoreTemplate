using InclusService.Services;
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
        public async Task WatchTest()
        {
            InitWorkspace(Connection);
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            Task watch = Task.Run(()=>Queue.Watch(QUEUE_NAME),tokenSource.Token);
            InclusService.Dto.Message sendedMessage = new InclusService.Dto.Message
            {
                Type = typeof(QueueAccessor),
                Data = "SOME_DATA"
            };
            await Task.Delay(2000);
            Queue.OnIncomingMessage+=(InclusService.Dto.Message incomingMessage)=>
            {
                Assert.Equal(sendedMessage.Type.FullName, incomingMessage.Type.FullName);
                Assert.Equal(sendedMessage.Data.ToString(), incomingMessage.Data.ToString());
                tokenSource.Cancel();
                ReleaseWorkspace(Connection);
            };
            Queue.Dispatch(sendedMessage, EXCHANGE_NAME, ROUTING_KEY);


        }
    }
}

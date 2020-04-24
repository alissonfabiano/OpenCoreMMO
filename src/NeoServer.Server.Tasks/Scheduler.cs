using NeoServer.Server.Tasks.Contracts;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace NeoServer.Server.Tasks
{


    public class Scheduler : IScheduler
    {
        private readonly ChannelWriter<ISchedulerEvent> writer;
        private readonly ChannelReader<ISchedulerEvent> reader;

        private ConcurrentDictionary<uint, byte> activeEventIds = new ConcurrentDictionary<uint, byte>();

        private uint lastEventId = 0;

        private IDispatcher dispatcher;

        public Scheduler(IDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            var channel = Channel.CreateUnbounded<ISchedulerEvent>(new UnboundedChannelOptions() { SingleReader = true });
            reader = channel.Reader;
            writer = channel.Writer;
        }

        public uint AddEvent(ISchedulerEvent evt)
        {


            if (evt.EventId == default)
            {
                evt.SetEventId(++lastEventId);
            }

            if (activeEventIds.TryAdd(evt.EventId, default))
            {
                writer.TryWrite(evt);
            }

            return evt.EventId;
        }

        public void Start(CancellationToken token)
        {
            Task.Run(async () =>
            {
                while (await reader.WaitToReadAsync())
                {
                    // Fast loop around available jobs
                    while (reader.TryRead(out var evt))
                    {
                        if (EventIsCancelled(evt.EventId))
                        {
                            continue;
                        }


                        await DispatchEvent(evt);
                    }
                }
            });
        }

        private async ValueTask DispatchEvent(ISchedulerEvent evt)
        {
            await Task.Delay(evt.ExpirationDelay);
            evt.SetToNotExpire();
            if (EventIsCancelled(evt.EventId))
            {
                return;
            }
            dispatcher.AddEvent(evt, true); //send to dispatcher
            activeEventIds.TryRemove(evt.EventId, out _);
        }

        public bool CancelEvent(uint eventId)
        {
            if (eventId == default)
            {
                return false;
            }

            return activeEventIds.TryRemove(eventId, out _);
        }

        private bool EventIsCancelled(uint eventId) => !activeEventIds.ContainsKey(eventId);


    }
}
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using Watchdog.App.Abstractions;

namespace Watchdog.Worker.Core
{
    public class WorkConsumer <T> : IInstance
    {
        public int InstanceId { get; }

        private readonly ChannelReader<T> _apiReader;
        private readonly ChannelWriter<T> _jobWriter;
        private readonly IWorkQueue<T> _workQueue;
        private readonly ILogger<WorkConsumer<T>> _logger;
        
        public WorkConsumer(
            ChannelReader<T> apiReader,
            ChannelWriter<T> jobWriter,
            IWorkQueue<T> workQueue,
            ILogger<WorkConsumer<T>> logger,
            IInstanceIdGenerator<WorkConsumer<T>> instanceIdGenerator)
        {
            Guard.Against.Null(apiReader, nameof(apiReader));
            Guard.Against.Null(workQueue, nameof(workQueue));
            Guard.Against.Null(logger, nameof(logger));
            Guard.Against.Null(instanceIdGenerator, nameof(instanceIdGenerator));

            _apiReader = apiReader;
            _jobWriter = jobWriter;
            _workQueue = workQueue;
            _logger = logger;
            InstanceId = instanceIdGenerator.GetNewId();
        }
        
        public async ValueTask BeginConsumeAsync(CancellationToken cancellationToken = default)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var deal = await _apiReader.ReadAsync(cancellationToken).ConfigureAwait(false);
                await _jobWriter.WriteAsync(deal, cancellationToken);
                await Task
                    .Run(() => _workQueue.Cleanup(deal), cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}
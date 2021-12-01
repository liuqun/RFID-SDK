using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace GDotnet.Reader.Api
{
    /// <summary>
    /// 事件调度
    /// </summary>
    public class EventDispatcher
    {
        private readonly BlockingCollection<Action> _queue;
        private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();

        public static readonly EvnetDispatcher Instance = new EvnetDispatcher();

        /// <summary>
        /// EvnetDispatcher
        /// </summary>
        private EventDispatcher()
        {
            _queue = new BlockingCollection<Action>();

            Task.Factory.StartNew(() => StartInternal(), TaskCreationOptions.LongRunning);
        }

        public void Invoke(Action executeAction)
        {
            _queue.Add(executeAction, _cancellation.Token);
        }

        public void Dispose()
        {
            _cancellation.Cancel();
        }

        private void StartInternal()
        {
            while (!_cancellation.IsCancellationRequested)
            {
                try
                {
                    var execute = _queue.Take();

                    execute();
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace OSItemIndex.AggregateService
{
    /// <summary>
    ///
    /// </summary>
    public abstract class NamedAggregateService : IHostedService, IDisposable
    {
        public readonly string Name;
        private readonly TimeSpan? _executeDelay;

        private Task _loopingTask;
        private Task _executingTask;
        private CancellationTokenSource _stoppingStartCts;
        private CancellationTokenSource _stoppingLoopCts;

        protected NamedAggregateService(string name, TimeSpan? executeDelay = null)
        {
            Name = name;
            _executeDelay = executeDelay;
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            // Create linked token to allow cancelling executing task from provided token
            _stoppingStartCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            // Store the task we're executing
            _loopingTask = LoopAsync(_stoppingStartCts.Token);

            // If the task is completed then return it, this will bubble cancellation and failure to the caller
            return _loopingTask.IsCompleted ? _loopingTask : Task.CompletedTask; // Otherwise it's running
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="cancellationToken">Indicates that the loop process should be aborted.</param>
        private async Task<Task> LoopAsync(CancellationToken cancellationToken)
        {
            _stoppingLoopCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            Log.Information("{@Name} aggregate service is starting", Name);

            _stoppingLoopCts.Token.Register(() => Log.Information("{@Name} aggregate service is stopping", Name));

            while (!_stoppingLoopCts.Token.IsCancellationRequested)
            {
                _executingTask = ExecuteAsync(_stoppingLoopCts.Token); // Store the task we're executing
                await _executingTask;

                if (_executeDelay != null)
                {
                    await Task.Delay(_executeDelay.Value, _stoppingLoopCts.Token);
                }
            }

            Log.Information("{@Name} aggregate service is stopping", Name);

            // If the task is completed then return it, this will bubble cancellation and failure to the caller
            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask; // Otherwise it's running
        }

        /// <summary>
        /// This method is called when the <see cref="IHostedService"/> starts. The implementation should return a task that represents
        /// the lifetime of the long running operation(s) being performed.
        /// </summary>
        /// <param name="stoppingToken">Triggered when <see cref="IHostedService.StopAsync(CancellationToken)"/> is called.</param>
        /// <returns>A <see cref="Task"/> that represents the long running operations.</returns>
        protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop called without start
            if (_loopingTask == null)
            {
                return;
            }

            try
            {
                // Signal cancellation to the executing method
                _stoppingStartCts.Cancel();
                _stoppingLoopCts.Cancel();
            }
            finally
            {
                // Wait until the task completes or the stop token triggers
                await Task.WhenAny(_loopingTask, Task.Delay(Timeout.Infinite, cancellationToken)).ConfigureAwait(false);
            }
        }

        public virtual void Dispose()
        {
            _stoppingStartCts?.Cancel();
            _stoppingLoopCts?.Cancel();
        }
    }
}
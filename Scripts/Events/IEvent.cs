using System.Collections.Generic;
using UnityDES.Utils;

namespace UnityDES.Events
{
    /// <summary>
    /// Mandatory interface for any simulation event.
    /// </summary>
    /// 
    /// <typeparam name="TKey">Type of the queue key</typeparam>
    public interface IEvent<TEvent, TKey> : IQueueItem<TKey>
        where TEvent : class, IEvent<TEvent, TKey>
    {
        /// <summary>
        /// Current behaviour enumerator instance.
        /// </summary>
        public IEnumerator<BehaviourResult<TEvent, TKey>> BehaviourCycle { get; }

        /// <summary>
        /// Runs the behaviour of the event and manages immediate rescheduling through the simulation instance.
        /// </summary>
        void Run(ISimulationController<TEvent, TKey> simulationController);

        /// <summary>
        /// Defines the behaviour of the event.
        /// Its logic can be split into multiple stages using yield.
        /// </summary>
        /// 
        /// <returns>
        /// <c>Null</c> value will remove the event from the queue.
        /// <c>0</c> value will skip the yield and continue processing the behaviour.
        /// Value <c>&gt;0</c> will reschedule the event accordingly always of minimum 1 tick ahead.
        /// </returns>
        public abstract IEnumerator<BehaviourResult<TEvent, TKey>> Behaviour();
    }
}

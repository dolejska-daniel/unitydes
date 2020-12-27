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
        /// Runs the behaviour of the event and manages its immediate rescheduling through the simulation instance.
        /// It also processes any simulation requests the event's behaviour could have - schedule/unschedule referenced event, etc.
        /// </summary>
        void Run(ISimulationController<TEvent, TKey> simulationController);

        /// <summary>
        /// Defines the behaviour of the event.
        /// Its logic can be split into multiple stages using yield.
        /// </summary>
        /// 
        /// <returns><see cref="BehaviourResult{TEvent, TKey}"/> describing what the event wants to do</returns>
        public abstract IEnumerator<BehaviourResult<TEvent, TKey>> Behaviour();
    }
}

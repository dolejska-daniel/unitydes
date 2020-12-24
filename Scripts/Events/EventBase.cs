using System;
using System.Collections.Generic;

namespace UnityDES.Events
{
    /// <summary>
    /// Base class for any simulation event.
    /// </summary>
    /// 
    /// <typeparam name="TKey">Type of the queue key</typeparam>
    public abstract class EventBase<TKey> : IEvent<EventBase<TKey>, TKey>
    {
        public TKey QueueKey { get; set; }

        public TKey SimulationTime { get => QueueKey; }

        public IEnumerator<BehaviourResult> BehaviourCycle { get; protected set; }

        /// <summary>
        /// Initializes BehaviourCycle property.
        /// </summary>
        public EventBase()
        {
            BehaviourCycle = Behaviour();
        }

        public abstract IEnumerator<BehaviourResult> Behaviour();

        public void Run(ISimulationController<EventBase<TKey>, TKey> simulationController)
        {
            bool unfinished;
            do
            {
                // continue processing the behaviour until its end or voluntary rescheduling
                unfinished = BehaviourCycle.MoveNext();
            }
            while (unfinished && BehaviourCycle.Current.ContinueBehaviour);

            if (unfinished && !BehaviourCycle.Current.UnscheduleEvent)
            {
                // update the event's key (must be done before rescheduling!)
                IncreaseKey(BehaviourCycle.Current.RescheduleTime);

                // reschedule the event
                if (!simulationController.Reschedule(this))
                    throw new ApplicationException("Rescheduling of existing event has failed!");

                if (BehaviourCycle.Current.ResetBehaviour)
                {
                    // create new behaviour iterator instance
                    BehaviourCycle = Behaviour();
                }
            }
            else
            {
                // the event's behaviour either completely finished or it voluntarily wants to be unscheduled
                // remove the event from the simulation
                simulationController.Unschedule(this);
            }
        }

        /// <summary>
        /// Updates the key so the event's execution happens in the future based on value of <paramref name="time"/>.
        /// </summary>
        /// 
        /// <param name="time">Time offset into the future</param>
        protected abstract void IncreaseKey(float time);
    }
}

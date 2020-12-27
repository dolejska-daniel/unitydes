using System;
using System.Collections.Generic;

namespace UnityDES.Events
{
    /// <summary>
    /// Base class for any simulation event.
    /// </summary>
    /// 
    /// <typeparam name="TEvent">Type of the derived event</typeparam>
    /// <typeparam name="TKey">Type of the queue key</typeparam>
    public abstract class EventBase<TEvent, TKey> : IEvent<TEvent, TKey>
        where TEvent : class, IEvent<TEvent, TKey>
    {
        public TKey QueueKey { get; set; }

        /// <summary>
        /// Should always be current simulation time during processing the event's behaviour.
        /// </summary>
        public TKey SimulationTime { get => QueueKey; }

        public IEnumerator<BehaviourResult<TEvent, TKey>> BehaviourCycle { get; protected set; }

        /// <summary>
        /// Initializes BehaviourCycle property.
        /// </summary>
        public EventBase()
        {
            BehaviourCycle = Behaviour();
        }

        /// <summary>
        /// Returns instance of the generic event derived from this class.
        /// This method is necessary for correct type checking.
        /// </summary>
        /// 
        /// <returns>Current instance of the generic event</returns>
        protected abstract TEvent This();

        /// <summary>
        /// Updates the key so the event's execution happens in the future based on value of <paramref name="time"/>.
        /// </summary>
        /// 
        /// <param name="time">Time offset into the future</param>
        protected abstract void IncreaseKey(float time);

        public abstract IEnumerator<BehaviourResult<TEvent, TKey>> Behaviour();

        public void Run(ISimulationController<TEvent, TKey> simulationController)
        {
            bool unfinished;
            BehaviourResult<TEvent, TKey> behaviourResult;

            do
            {
                // continue processing the behaviour until its end or voluntary rescheduling
                unfinished = BehaviourCycle.MoveNext();
                behaviourResult = BehaviourCycle.Current;

                if (behaviourResult.ScheduleReferenced)
                {
                    // the currently processed event wants to schedule new referenced event
                    simulationController.Schedule(behaviourResult.ReferencedEvent, behaviourResult.ReferencedTime);
                }
                else if (behaviourResult.UnscheduleReferenced)
                {
                    // the currently processed event wants to unschedule referenced event
                    simulationController.Unschedule(behaviourResult.ReferencedEvent);
                }

                if (behaviourResult.ResetBehaviour)
                {
                    // the currently processed event wants its to be reset
                    BehaviourCycle = Behaviour();
                }
            }
            while (unfinished && behaviourResult.ContinueBehaviour);

            if (unfinished && behaviourResult.RescheduleEvent)
            {
                // update the event's key (must be done before rescheduling!)
                IncreaseKey(behaviourResult.SelfTime);

                // reschedule the event
                if (!simulationController.Reschedule(This()))
                    throw new ApplicationException("Rescheduling of existing event has failed!");
            }
            else if (!unfinished || behaviourResult.UnscheduleEvent)
            {
                // the event's behaviour either completely finished or it voluntarily wants to be unscheduled
                // remove the event from the simulation
                if (!simulationController.Unschedule(This()))
                    throw new ApplicationException("Unscheduling of existing event has failed!");
            }
            else
            {
                throw new ApplicationException("This should never happen.");
            }
        }

        /// <inheritdoc cref="BehaviourResult{TEvent, TKey}.Continue(bool)"/>
        protected BehaviourResult<TEvent, TKey> Continue(bool reset = false)
            => BehaviourResult<TEvent, TKey>.Continue(reset);

        /// <inheritdoc cref="BehaviourResult{TEvent, TKey}.Reschedule(float, bool)"/>
        protected BehaviourResult<TEvent, TKey> Reschedule(float rescheduleTime, bool reset = true)
            => BehaviourResult<TEvent, TKey>.Reschedule(rescheduleTime, reset);

        /// <inheritdoc cref="BehaviourResult{TEvent, TKey}.Unschedule"/>
        protected BehaviourResult<TEvent, TKey> Unschedule()
            => BehaviourResult<TEvent, TKey>.Unschedule();

        /// <inheritdoc cref="BehaviourResult{TEvent, TKey}.ScheduleNew(float, TEvent, float, bool)"/>
        protected BehaviourResult<TEvent, TKey> ScheduleNew(float rescheduleTime, TEvent @event, float scheduleTime, bool reset = false)
            => BehaviourResult<TEvent, TKey>.ScheduleNew(rescheduleTime, @event, scheduleTime, reset);

        /// <inheritdoc cref="BehaviourResult{TEvent, TKey}.ScheduleNewAndContinue(TEvent, float, bool)"/>
        protected BehaviourResult<TEvent, TKey> ScheduleNewAndContinue(TEvent @event, float scheduleTime, bool reset = false)
            => BehaviourResult<TEvent, TKey>.ScheduleNewAndContinue(@event, scheduleTime, reset);
    }
}

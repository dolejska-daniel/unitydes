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

        public IEnumerator<BehaviourResult<EventBase<TKey>, TKey>> BehaviourCycle { get; protected set; }

        /// <summary>
        /// Initializes BehaviourCycle property.
        /// </summary>
        public EventBase()
        {
            BehaviourCycle = Behaviour();
        }

        /// <summary>
        /// Updates the key so the event's execution happens in the future based on value of <paramref name="time"/>.
        /// </summary>
        /// 
        /// <param name="time">Time offset into the future</param>
        protected abstract void IncreaseKey(float time);

        public abstract IEnumerator<BehaviourResult<EventBase<TKey>, TKey>> Behaviour();

        public void Run(ISimulationController<EventBase<TKey>, TKey> simulationController)
        {
            bool unfinished;
            BehaviourResult<EventBase<TKey>, TKey> behaviourResult;

            do
            {
                // continue processing the behaviour until its end or voluntary rescheduling
                unfinished = BehaviourCycle.MoveNext();
                behaviourResult = BehaviourCycle.Current;

                if (behaviourResult.ScheduleNewEvent)
                {
                    simulationController.Schedule(behaviourResult.NewEvent, behaviourResult.NewEventScheduleTime);
                }
            }
            while (unfinished && behaviourResult.ContinueBehaviour);

            if (unfinished && !behaviourResult.UnscheduleEvent)
            {
                // update the event's key (must be done before rescheduling!)
                IncreaseKey(behaviourResult.RescheduleTime);

                // reschedule the event
                if (!simulationController.Reschedule(this))
                    throw new ApplicationException("Rescheduling of existing event has failed!");

                if (behaviourResult.ResetBehaviour)
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

        /// <inheritdoc cref="BehaviourResult{TEvent, TKey}.ScheduleNew(float, TEvent, float, bool)"/>
        protected BehaviourResult<EventBase<TKey>, TKey> ScheduleNew(float rescheduleTime, EventBase<TKey> @event, float scheduleTime, bool reset = false)
            => BehaviourResult<EventBase<TKey>, TKey>.ScheduleNew(rescheduleTime, @event, scheduleTime, reset);

        /// <inheritdoc cref="BehaviourResult{TEvent, TKey}.ScheduleNewAndContinue(TEvent, float, bool)"/>
        protected BehaviourResult<EventBase<TKey>, TKey> ScheduleNewAndContinue(EventBase<TKey> @event, float scheduleTime, bool reset = false)
            => BehaviourResult<EventBase<TKey>, TKey>.ScheduleNewAndContinue(@event, scheduleTime, reset);

        /// <inheritdoc cref="BehaviourResult{TEvent, TKey}.Continue(bool)"/>
        protected BehaviourResult<EventBase<TKey>, TKey> Continue(bool reset = false)
            => BehaviourResult<EventBase<TKey>, TKey>.Continue(reset);

        /// <inheritdoc cref="BehaviourResult{TEvent, TKey}.Reschedule(float, bool)"/>
        protected BehaviourResult<EventBase<TKey>, TKey> Reschedule(float rescheduleTime, bool reset = true)
            => BehaviourResult<EventBase<TKey>, TKey>.Reschedule(rescheduleTime, reset);

        /// <inheritdoc cref="BehaviourResult{TEvent, TKey}.Unschedule"/>
        protected BehaviourResult<EventBase<TKey>, TKey> Unschedule()
            => BehaviourResult<EventBase<TKey>, TKey>.Unschedule();
    }
}

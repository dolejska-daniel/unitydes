using System;

namespace UnityDES.Events
{
    /// <summary>
    /// Structure describing result of the event's behaviour.
    /// </summary>
    public struct BehaviourResult<TEvent, TKey>
        where TEvent : class, IEvent<TEvent, TKey>
    {
        /// <summary>
        /// Reserved time constant - the behaviour should continue being processed.
        /// </summary>
        const float TIME_CONTINUE = -1f;

        /// <summary>
        /// Reserved time constant - the event should be removed from the simulation.
        /// </summary>
        const float TIME_UNSCHEDULE = -2f;

        /// <summary>
        /// Minimum number of time to pass before running the event's behaviour again.
        /// </summary>
        /// <remarks>
        /// Values <c>&lt;0</c> are reserved for other result states.
        /// </remarks>
        public float RescheduleTime { get; internal set; }

        /// <summary>
        /// Behaviour iterator should be reset before its next run.
        /// </summary>
        public bool ResetBehaviour { get; internal set; }

        /// <summary>
        /// Simulation should continue processing the event's behaviour.
        /// </summary>
        public bool ContinueBehaviour { get => RescheduleTime == TIME_CONTINUE; }

        /// <summary>
        /// Simulation should now remove the event from the simulation.
        /// </summary>
        public bool UnscheduleEvent { get => RescheduleTime == TIME_UNSCHEDULE; }

        /// <summary>
        /// Simulation should add a new event to the simulation.
        /// </summary>
        public bool ScheduleNewEvent { get => NewEvent != null; }

        /// <summary>
        /// Event to be added to the simulation.
        /// </summary>
        public TEvent NewEvent { get; internal set; }

        /// <summary>
        /// .
        /// </summary>
        public float NewEventScheduleTime { get; internal set; }

        BehaviourResult(float rescheduleTime, bool reset, TEvent @event = null, float scheduleTime = 0f)
        {
            RescheduleTime = rescheduleTime;
            ResetBehaviour = reset;
            NewEvent = @event;
            NewEventScheduleTime = scheduleTime;
        }

        /// <summary>
        /// Event should be rescheduled and its behaviour run after minimum of <paramref name="rescheduleTime"/> seconds
        /// or be reset and rescheduled (if <paramref name="reset"/> is <c>True</c>).
        /// </summary>
        /// 
        /// <param name="rescheduleTime">Minimum amount of time to skip</param>
        /// <param name="reset">Should behaviour iterator be reset?</param>
        /// <returns>Behaviour result instance reflecting rescheduling</returns>
        public static BehaviourResult<TEvent, TKey> Reschedule(float rescheduleTime, bool reset = true)
        {
            if (rescheduleTime < 0f)
                throw new ArgumentException("Event's reschedule time cannot be less than 0.");

            return new BehaviourResult<TEvent, TKey>(rescheduleTime, reset);
        }

        /// <summary>
        /// Event should be removed from the simulation.
        /// </summary>
        /// 
        /// <returns>Behaviour result instance reflecting unscheduling</returns>
        public static BehaviourResult<TEvent, TKey> Unschedule()
            => new BehaviourResult<TEvent, TKey>(TIME_UNSCHEDULE, false);

        /// <summary>
        /// Processing of the event's behaviour should continue immediately
        /// or be reset and continue immediately (if <paramref name="reset"/> is <c>True</c>).
        /// </summary>
        /// 
        /// <param name="reset">Should behaviour iterator be reset?</param>
        /// <returns>Behaviour result instance reflecting continuing</returns>
        public static BehaviourResult<TEvent, TKey> Continue(bool reset = false)
            => new BehaviourResult<TEvent, TKey>(TIME_CONTINUE, reset);

        /// <summary>
        /// Processing of the event's behaviour should continue after scheduling a new <paramref name="event"/>
        /// or be reset after scheduling the event (if <paramref name="reset"/> is <c>True</c>).
        /// </summary>
        /// 
        /// <param name="event">Event to be scheduled</param>
        /// <param name="scheduleTime">Event to be scheduled</param>
        /// <param name="reset">Should behaviour iterator be reset?</param>
        /// <returns>Behaviour result instance reflecting new event scheduling</returns>
        public static BehaviourResult<TEvent, TKey> ScheduleNewAndContinue(TEvent @event, float scheduleTime = 0, bool reset = false)
            => ScheduleNew(TIME_CONTINUE, @event, scheduleTime, reset);

        /// <summary>
        /// Event should be rescheduled and its behaviour run after minimum of <paramref name="rescheduleTime"/> seconds
        /// or be reset and rescheduled (if <paramref name="reset"/> is <c>True</c>).
        /// All that after scheduling a new <paramref name="event"/> (with that event waiting for a minimum of <paramref name="scheduleTime"/> seconds).
        /// </summary>
        /// 
        /// <param name="rescheduleTime">Minimum amount of time to skip</param>
        /// <param name="event">Event to be scheduled</param>
        /// <param name="scheduleTime">Event to be scheduled</param>
        /// <param name="reset">Should behaviour iterator be reset?</param>
        /// <returns>Behaviour result instance reflecting new event scheduling</returns>
        public static BehaviourResult<TEvent, TKey> ScheduleNew(float rescheduleTime, TEvent @event, float scheduleTime = 0, bool reset = false)
            => new BehaviourResult<TEvent, TKey>(rescheduleTime, reset, @event, scheduleTime);

        public override string ToString()
        {
            if (ContinueBehaviour)
                return "EventBehaviour: Continue" + (ResetBehaviour ? " and Reset" : "");

            if (UnscheduleEvent)
                return "EventBehaviour: Unschedule";

            return "EventBehaviour: Reschedule in " + RescheduleTime + (ResetBehaviour ? " and Reset" : "");
        }

        public static implicit operator float(BehaviourResult<TEvent, TKey> result) => result.RescheduleTime;
    }
}

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
        /// Behaviour state option.
        /// The names should be self explanatory.
        /// </summary>
        public enum State
        {
            IGNORE      = 0x00,
            CONTINUE    = 0x01,
            SCHEDULE    = 0x02,
            RESCHEDULE  = 0x04,
            UNSCHEDULE  = 0x08,
            RESET       = 0x10,
        }

        /// <summary>
        /// States allowed for event returning this result.
        /// </summary>
        public const State ALLOWED_SELF_STATES = State.CONTINUE | State.RESCHEDULE | State.UNSCHEDULE | State.RESET;

        /// <summary>
        /// States allowed for events referenced by this result.
        /// </summary>
        public const State ALLOWED_REFERENCED_STATES = State.SCHEDULE | State.UNSCHEDULE;

        /// <summary>
        /// Behaviour state of the event returning this result.
        /// </summary>
        public State SelfState { get; internal set; }

        /// <summary>
        /// Minimum amount of time to pass before running the event's behaviour again.
        /// </summary>
        public float SelfTime { get; internal set; }

        /// <summary>
        /// Behaviour state of the event referenced by this result.
        /// </summary>
        public State ReferencedState { get; internal set; }

        /// <summary>
        /// Event to be added to the simulation.
        /// </summary>
        public TEvent ReferencedEvent { get; internal set; }

        /// <summary>
        /// Minimum amount of time to pass before running the event.
        /// </summary>
        public float ReferencedTime { get; internal set; }

        /// <summary>
        /// Event retuning this result wants to continue processing its behaviour.
        /// </summary>
        public bool ContinueBehaviour { get => (SelfState & State.CONTINUE) != State.IGNORE; }

        /// <summary>
        /// Event retuning this result wants to reset its behaviour.
        /// </summary>
        public bool ResetBehaviour { get => (SelfState & State.RESET) != State.IGNORE; }

        /// <summary>
        /// Event retuning this result wants to reschedule itself.
        /// </summary>
        public bool RescheduleEvent { get => (SelfState & State.RESCHEDULE) != State.IGNORE; }

        /// <summary>
        /// Event retuning this result wants to unschedule itself.
        /// </summary>
        public bool UnscheduleEvent { get => (SelfState & State.UNSCHEDULE) != State.IGNORE; }

        /// <summary>
        /// Event retuning this result wants to schedule event it is referencing.
        /// </summary>
        public bool ScheduleReferenced { get => (ReferencedState & State.SCHEDULE) != State.IGNORE; }

        /// <summary>
        /// Event retuning this result wants to unschedule event it is referencing.
        /// </summary>
        public bool UnscheduleReferenced { get => (ReferencedState & State.UNSCHEDULE) != State.IGNORE; }

        BehaviourResult(State selfState, float selfTime = 0f, State referencedState = State.IGNORE, TEvent referencedEvent = null, float referencedTime = 0f)
        {
            selfState &= ALLOWED_SELF_STATES;
            referencedState &= ALLOWED_REFERENCED_STATES;

            SelfState = selfState;
            SelfTime = selfTime;
            ReferencedState = referencedState;
            ReferencedEvent = referencedEvent;
            ReferencedTime = referencedTime;
        }

        /// <summary>
        /// Processing of the event's behaviour should continue immediately
        /// or be reset and continue immediately (if <paramref name="reset"/> is <c>True</c>).
        /// </summary>
        /// 
        /// <param name="reset">Should behaviour iterator be reset?</param>
        /// <returns>Behaviour result instance reflecting continuing</returns>
        public static BehaviourResult<TEvent, TKey> Continue(bool reset = false)
            => new BehaviourResult<TEvent, TKey>(State.CONTINUE | (reset ? State.RESET : State.IGNORE));

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
            if (rescheduleTime <= 0f)
                throw new ArgumentException("Event's reschedule time cannot be less than or equal to 0.");

            return new BehaviourResult<TEvent, TKey>(State.RESCHEDULE | (reset ? State.RESET : State.IGNORE), rescheduleTime);
        }

        /// <summary>
        /// Event should be removed from the simulation.
        /// </summary>
        /// 
        /// <returns>Behaviour result instance reflecting unscheduling</returns>
        public static BehaviourResult<TEvent, TKey> Unschedule()
            => new BehaviourResult<TEvent, TKey>(State.UNSCHEDULE);

        /// <summary>
        /// Event should be rescheduled and its behaviour run after minimum of <paramref name="rescheduleTime"/> seconds
        /// or be reset and rescheduled (if <paramref name="selfReset"/> is <c>True</c>).
        /// All that after scheduling a new <paramref name="event"/> (with that event waiting for a minimum of <paramref name="scheduleTime"/> seconds).
        /// </summary>
        /// 
        /// <param name="rescheduleTime">Minimum amount of time to skip</param>
        /// <param name="event">Event to be scheduled</param>
        /// <param name="scheduleTime">Event to be scheduled</param>
        /// <param name="selfReset">Should behaviour iterator be reset?</param>
        /// <returns>Behaviour result instance reflecting new event scheduling</returns>
        public static BehaviourResult<TEvent, TKey> ScheduleNew(float rescheduleTime, TEvent @event, float scheduleTime = 0f, bool selfReset = false)
            => new BehaviourResult<TEvent, TKey>(State.RESCHEDULE | (selfReset ? State.RESET : State.IGNORE), rescheduleTime, State.SCHEDULE, @event, scheduleTime);

        /// <summary>
        /// Processing of the event's behaviour should continue after scheduling a new <paramref name="event"/>
        /// or be reset after scheduling the event (if <paramref name="selfReset"/> is <c>True</c>).
        /// </summary>
        /// 
        /// <param name="event">Event to be scheduled</param>
        /// <param name="scheduleTime">Event to be scheduled</param>
        /// <param name="selfReset">Should behaviour iterator be reset?</param>
        /// <returns>Behaviour result instance reflecting new event scheduling</returns>
        public static BehaviourResult<TEvent, TKey> ScheduleNewAndContinue(TEvent @event, float scheduleTime = 0f, bool selfReset = false)
            => new BehaviourResult<TEvent, TKey>(State.CONTINUE | (selfReset ? State.RESET : State.IGNORE), 0f, State.SCHEDULE, @event, scheduleTime);
    }
}

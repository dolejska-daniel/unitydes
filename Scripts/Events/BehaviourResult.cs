using System;

namespace UnityDES.Events
{
    /// <summary>
    /// Structure describing result of the event's behaviour.
    /// </summary>
    public struct BehaviourResult
    {
        /// <summary>
        /// Minimum number of time to pass before running the event's behaviour again.
        /// </summary>
        /// <remarks>
        /// Values <c>&lt;=0</c> are reserved for other result states.
        /// </remarks>
        public float RescheduleTime { get; internal set; }

        /// <summary>
        /// Behaviour iterator should be reset before its next run.
        /// </summary>
        public bool ResetBehaviour { get; internal set; }

        /// <summary>
        /// Simulation should continue processing the event's behaviour.
        /// </summary>
        public bool ContinueBehaviour { get => RescheduleTime == 0; }

        /// <summary>
        /// Simulation should now remove the event from the simulation.
        /// </summary>
        public bool UnscheduleEvent{ get => RescheduleTime < 0; }

        BehaviourResult(float rescheduleTime, bool reset)
        {
            RescheduleTime = rescheduleTime;
            ResetBehaviour = reset;
        }

        /// <summary>
        /// Event should be rescheduled and its behaviour run after minimum of <paramref name="time"/> seconds.
        /// </summary>
        /// 
        /// <param name="time">Minimum amount of time to skip</param>
        /// <param name="reset">Should behaviour iterator be reset?</param>
        /// <returns>Behaviour result instance reflecting rescheduling</returns>
        public static BehaviourResult Reschedule(float time, bool reset = false)
        {
            if (time <= 0)
                throw new ArgumentException("Event's reschedule time cannot be less or equal to 0.");

            return new BehaviourResult(time, reset);
        }

        /// <summary>
        /// Event should be removed from the simulation.
        /// </summary>
        /// 
        /// <returns>Behaviour result instance reflecting unscheduling</returns>
        public static BehaviourResult Unschedule() => new BehaviourResult(-1, false);

        /// <summary>
        /// Processing of the event's behaviour should continue immediately.
        /// </summary>
        /// 
        /// <param name="reset">Should behaviour iterator be reset?</param>
        /// <returns>Behaviour result instance reflecting continuing</returns>
        public static BehaviourResult Continue(bool reset = false) => new BehaviourResult(0, reset);

        public override string ToString()
        {
            if (ContinueBehaviour)
                return "EventBehaviour: Continue" + (ResetBehaviour ? " and Reset" : "");

            if (UnscheduleEvent)
                return "EventBehaviour: Unschedule";

            return "EventBehaviour: Reschedule in " + RescheduleTime + (ResetBehaviour ? " and Reset" : "");
        }

        public static implicit operator float(BehaviourResult result) => result.RescheduleTime;
    }
}

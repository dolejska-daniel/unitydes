using System.Collections;
using UnityDES.Events;

namespace UnityDES
{
    /// <summary>
    /// The main class controlling the simulation.
    /// </summary>
    /// 
    /// <typeparam name="TEvent">Type of the event</typeparam>
    /// <typeparam name="TKey">Type of the simulation time</typeparam>
    public interface ISimulationController<TEvent, TKey>
        where TEvent : IEvent<TEvent, TKey>
    {
        /// <summary>
        /// Current simulation time.
        /// </summary>
        TKey SimulationTime { get; }

        /// <summary>
        /// Speed of the simulation.
        /// </summary>
        int SimulationSpeed { get; set; }

        /// <summary>
        /// Runs <see cref="RunAvailableTicks"/> each rendered frame as a coroutine.
        /// </summary>
        IEnumerator RunAvailableTicksCoroutine();

        /// <summary>
        /// Runs all simulation ticks that should be processed to keep up with the real time.
        /// </summary>
        /// <remarks>
        /// This method expects to be called every rendered frame.
        /// </remarks>
        void RunAvailableTicks();

        /// <summary>
        /// Adds the event (<paramref name="event"/>) to the simulation.
        /// </summary>
        /// <remarks>
        /// The event's time will be set to the current simulation time and then offset by provided number of ticks (<paramref name="tickCount"/>).
        /// </remarks>
        /// 
        /// <param name="event">Event to be scheduled</param>
        /// <param name="tickCount">Number of ticks to be skipped (1 means run next tick)</param>
        void Schedule(TEvent @event, int tickCount = 1);

        /// <summary>
        /// Reschedules the event (<paramref name="event"/>) already inside the simulation.
        /// </summary>
        /// <remarks>
        /// This method expects the event's key to be already updated.
        /// </remarks>
        /// 
        /// <param name="event">Event to be rescheduled</param>
        /// <returns><c>True</c> if the update of the event simulation queue has been successful, <c>False</c> otherwise</returns>
        bool Reschedule(TEvent @event);

        /// <summary>
        /// Completely removes the event (<paramref name="event"/>) from the simulation.
        /// Its location within the internal queue is irrelevant.
        /// </summary>
        /// 
        /// <param name="event">Event to be removed</param>
        /// <returns><c>True</c> if the removal from the simulation has been successful, <c>False</c> otherwise</returns>
        bool Unschedule(TEvent @event);
    }
}

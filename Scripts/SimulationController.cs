using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityDES.Utils;
using UnityDES.Events;

namespace UnityDES
{
    /// <summary>
    /// The main class controlling the simulation.
    /// </summary>
    public class SimulationController : ISimulationController<EventBase<SimulationTime>, SimulationTime>
    {
        public SimulationTime SimulationTime { get; protected set; }

        public int SimulationSpeed { get; set; } = 1;

        /// <summary>
        /// Queue of the simulation events.
        /// </summary>
        protected PriorityQueue<EventBase<SimulationTime>, SimulationTime> Events;

        /// <summary>
        /// Comparer of the simulation events for the queue.
        /// </summary>
        public static readonly Comparer<EventBase<SimulationTime>> EventComparer = Comparer<EventBase<SimulationTime>>.Create(
            (a, b) => SimulationTime.Comparer.Compare(a.QueueKey, b.QueueKey));

        /// <summary>
        /// Initializes the simulation time and event queue.
        /// </summary>
        public SimulationController(int ticksPerFrame = 1)
        {
            SimulationTime = new SimulationTime(ticksPerFrame);
            Events = new PriorityQueue<EventBase<SimulationTime>, SimulationTime>(EventComparer);
        }

        public IEnumerator RunAvailableTicksCoroutine()
        {
            while (true)
            {
                RunAvailableTicks();
                yield return null;
            }
        }

        public void RunAvailableTicks()
        {
            // calculate number of ticks to be run
            var tickCount = SimulationTime.TicksPerFrame * Time.deltaTime;

            // run the ticks
            for (; tickCount > 0; tickCount--)
            {
                RunTick();
            }
        }

        public void RunTick()
        {
            // get the first event in the queue
            var @event = Events.Peek();

            // while there are events available
            while (@event.QueueKey < SimulationTime)
            {
                // ...run the event behaviour - this method takes care of rescheduling/removing the event
                @event.Run(this);
                // ...get the next event in queue
                @event = Events.Peek();
            }

            // update the simulation time
            SimulationTime.DoTick(SimulationSpeed);
        }

        public void Schedule(EventBase<SimulationTime> @event, int tickCount = 1)
        {
            // set current simulation time
            @event.QueueKey.Frame = SimulationTime.Frame;
            @event.QueueKey.Tick = SimulationTime.Tick;
            // offset the time to schedule the event
            @event.QueueKey.DoTick(tickCount);

            // enqueue the event
            Events.Enqueue(@event);
        }

        public bool Reschedule(EventBase<SimulationTime> @event)
        {
            // re-enqueue the event
            return Events.Update(@event);
        }

        public bool Unschedule(EventBase<SimulationTime> @event)
        {
            // remove the event from the queue
            return Events.Dequeue(@event);
        }
    }
}

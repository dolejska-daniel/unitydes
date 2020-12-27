using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityDES.Utils;
using UnityDES.Events;
using System;

namespace UnityDES
{
    /// <summary>
    /// The main class controlling the simulation.
    /// </summary>
    public class SimulationController<TEvent> : ISimulationController<TEvent, SimulationTime>
        where TEvent : class, IEvent<TEvent, SimulationTime>
    {
        public SimulationTime SimulationTime { get; protected set; }

        public float DeltaTime { get; protected set; }

        public int SimulationSpeed { get; set; } = 1;

        /// <summary>
        /// Queue of the simulation events.
        /// </summary>
        protected PriorityQueue<TEvent, SimulationTime> Events;

        /// <summary>
        /// Comparer of the simulation events for the queue.
        /// </summary>
        public static readonly Comparer<TEvent> EventComparer = Comparer<TEvent>.Create(
            (a, b) => SimulationTime.Comparer.Compare(a.QueueKey, b.QueueKey));

        /// <summary>
        /// Initializes the simulation time and event queue.
        /// </summary>
        public SimulationController(int ticksPerFrame = 1)
        {
            SimulationTime = new SimulationTime(ticksPerFrame);
            Events = new PriorityQueue<TEvent, SimulationTime>(EventComparer);
        }

        public IEnumerator RunAvailableTicksCoroutine()
        {
            while (true)
            {
                RunAvailableTicks(Time.deltaTime);
                yield return null;
            }
        }

        public void RunAvailableTicks() => RunAvailableTicks(Time.deltaTime);

        public void RunAvailableTicks(float deltaTime)
        {
            DeltaTime += deltaTime;

            // calculate number of ticks to be run
            var tickCount = (float)Math.Floor(SimulationTime.TicksPerFrame * DeltaTime);

            // if any ticks are run, subtract the number of ticks from the passed time
            DeltaTime -= tickCount * SimulationTime.TickLength;
            // adjust simulation speed
            tickCount *= SimulationSpeed;

            // run the calculated number of ticks
            for (; tickCount > 0; tickCount--)
            {
                RunTick();
            }
        }

        /// <summary>
        /// Runs all available events for current simulation time.
        /// Accordingly updates current simulation time after all events have been processed.
        /// </summary>
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
            SimulationTime.DoTick();
        }

        public void Schedule(TEvent @event, float scheduleTime)
            => Schedule(@event, (int)Math.Ceiling(@event.QueueKey.TicksPerFrame * scheduleTime));

        public void Schedule(TEvent @event, int tickCount = 1)
        {
            // set current simulation time
            @event.QueueKey.Frame = SimulationTime.Frame;
            @event.QueueKey.Tick = SimulationTime.Tick;
            // offset the time to schedule the event
            @event.QueueKey.DoTick(tickCount);

            // enqueue the event
            Events.Enqueue(@event);
        }

        public bool Reschedule(TEvent @event)
        {
            // re-enqueue the event
            return Events.Update(@event);
        }

        public bool Unschedule(TEvent @event)
        {
            // remove the event from the queue
            return Events.Dequeue(@event);
        }
    }
}

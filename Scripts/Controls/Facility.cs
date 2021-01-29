using System;
using System.Collections.Generic;
using UnityDES.Events;
using UnityDES.Utils;

namespace UnityDES.Controls
{
    public class Facility<TEvent, TKey>
        where TEvent : class, IEvent<TEvent, TKey>
    {
        /// <summary>
        /// Internal facility entry structure.
        /// </summary>
        public class FacilityEntry : IQueueItem<int>
        {
            /// <summary>
            /// Priority of the event in the queue.
            /// </summary>
            public int QueueKey { get; set; }

            /// <summary>
            /// Event in the queue.
            /// </summary>
            public readonly TEvent Event;

            public FacilityEntry(int priority, TEvent @event)
            {
                QueueKey = priority;
                Event = @event;
            }

            public static Comparer<FacilityEntry> Comparer = Comparer<FacilityEntry>.Create(
                (a, b) => a.QueueKey - b.QueueKey);
        }

        /// <summary>
        /// The event delegate for all facility events.
        /// </summary>
        /// 
        /// <param name="event">Event within the facility</param>
        public delegate void EventStateChanged(TEvent @event);

        /// <summary>
        /// Called during the claim call if the facility is full.
        /// This event is called only once for each event entering the facility.
        /// </summary>
        public event EventStateChanged OnQueueEnter;

        /// <summary>
        /// Called after an event leaves the initial waiting queue.
        /// This event is called only once for each event entering the facility.
        /// </summary>
        public event EventStateChanged OnQueueLeave;

        /// <summary>
        /// Called when an event enters the facility.
        /// This can happen immediatelly during the claim call, after leaving the queue or after interrupt ends.
        /// </summary>
        public event EventStateChanged OnFacilityEnter;

        /// <summary>
        /// Called when an event with higher priority interrupts event currently in the facility forcing it to wait.
        /// </summary>
        public event EventStateChanged OnFacilityInterrupt;

        /// <summary>
        /// Called after an event finally leaves the facility.
        /// This event is called only once for each event entering the facility.
        /// </summary>
        public event EventStateChanged OnFacilityLeave;

        /// <summary>
        /// Default claim priority of new events.
        /// </summary>
        const int DEFAULT_PRIORITY = 100;

        /// <summary>
        /// Complete concurrent capacity of the facility.
        /// </summary>
        public int Capacity { get; protected set; }

        /// <summary>
        /// Number of events with currently active claims.
        /// </summary>
        public int CurrentlyUsing { get => Inside.Count; }

        /// <summary>
        /// Priority queue with events waiting for facility being freed.
        /// </summary>
        protected PriorityQueue<FacilityEntry, int> WaitingQueue { get; }

        /// <summary>
        /// Priority queue with events interrupted during their active claims.
        /// </summary>
        protected PriorityQueue<FacilityEntry, int> InterruptedQueue { get; }

        /// <summary>
        /// Set of events with currently active claims.
        /// </summary>
        protected SortedSet<FacilityEntry> Inside { get; }

        /// <summary>
        /// Mapping of event instances to facility's internal entries.
        /// </summary>
        protected Dictionary<TEvent, FacilityEntry> EventEntryMapping { get; }

        /// <summary>
        /// Simulation controller.
        /// </summary>
        protected ISimulationController<TEvent, TKey> Controller { get; }

        /// <summary>
        /// Initializes the facility control object for provided simulation (<paramref name="controller"/>) with given <paramref name="capacity"/>.
        /// </summary>
        /// 
        /// <param name="controller">Controller of corresponding simulation</param>
        /// <param name="capacity">Concurrent event claim capacity of the facility</param>
        public Facility(ISimulationController<TEvent, TKey> controller, int capacity = 1)
        {
            if (capacity < 1)
                throw new ArgumentException("Capacity of the facility cannot be less than 1.");

            Controller = controller;
            Capacity = capacity;

            WaitingQueue = new PriorityQueue<FacilityEntry, int>(FacilityEntry.Comparer);
            InterruptedQueue = new PriorityQueue<FacilityEntry, int>(FacilityEntry.Comparer);
            Inside = new SortedSet<FacilityEntry>(FacilityEntry.Comparer);
            EventEntryMapping = new Dictionary<TEvent, FacilityEntry>();
        }

        /// <summary>
        /// Claims a signle capacity slot for provied <paramref name="event"/>.
        /// The event may skip the queue or even interrupt active claims having enough <paramref name="priority"/>.
        /// </summary>
        /// 
        /// <param name="event">Event claiming the facility capacity slot</param>
        /// <param name="priority">Priority of the event's claim</param>
        /// <returns>
        /// <c>Continue</c> behaviour type if claim has been successful, <c>Unschedule</c> otherwise.
        /// The event will be rescheduled by the facility when it frees up.
        /// </returns>
        public BehaviourResult<TEvent, TKey> Claim(TEvent @event, int priority = DEFAULT_PRIORITY)
        {
            if (EventEntryMapping.ContainsKey(@event))
                throw new ArgumentException("Provided event is already within the facility.");

            // the event is not within this facility, create corresponding entry
            var entry = new FacilityEntry(priority, @event);
            // create the mapping of the event to the created internal entry
            EventEntryMapping.Add(entry.Event, entry);

            if (CurrentlyUsing < Capacity)
            {
                // there is enough room in the facility
                // register active claim of the event
                Inside.Add(entry);
                // run registered event handlers
                OnFacilityEnter?.Invoke(entry.Event);

                // claim has been registered, continue with the event's behaviour
                return BehaviourResult<TEvent, TKey>.Continue();
            }

            // there is not enough room in the facility
            var existingEntry = Inside.Min;
            if (FacilityEntry.Comparer.Compare(existingEntry, entry) >= 0)
            {
                // the smallest existing entry inside the facility is still larger than the new entry
                // add this new entry to the waiting queue
                WaitingQueue.Enqueue(entry);
                // run registered event handlers
                OnQueueEnter?.Invoke(entry.Event);

                // the event is waiting, it is necessary to remove it from the simulation
                // until the claim is ready
                return BehaviourResult<TEvent, TKey>.Unschedule();
            }

            // the existing entry has lower priority than the new entry
            // unschedule the existing entry - it has just lost control over the facility
            Controller.Unschedule(existingEntry.Event);

            // remove the entry from inside the facility
            Inside.Remove(existingEntry);
            // add it to the queue of interrupted entries
            InterruptedQueue.Enqueue(existingEntry);
            // run registered event handlers
            OnFacilityInterrupt?.Invoke(existingEntry.Event);

            // add new entry inside the facility
            Inside.Add(entry);
            // run registered event handlers
            OnFacilityEnter?.Invoke(entry.Event);

            // continue processing the event's behaviour
            return BehaviourResult<TEvent, TKey>.Continue();
        }

        /// <summary>
        /// Gives up active claim of the provied <paramref name="event"/>.
        /// </summary>
        /// 
        /// <param name="event">Event giving up the active claim</param>
        /// <returns><c>Continue</c> behaviour type</returns>
        public BehaviourResult<TEvent, TKey> Free(TEvent @event)
        {
            if (!EventEntryMapping.ContainsKey(@event))
                throw new ArgumentException("Provided event is not within the facility.");

            var entry = EventEntryMapping[@event];

            // remove the entry from inside the facility
            Inside.Remove(entry);
            // run registered event handlers
            OnFacilityLeave?.Invoke(entry.Event);

            // the event may free before actually acquiring the facility
            // make sure the internal queues are clear
            InterruptedQueue.Dequeue(entry);
            WaitingQueue.Dequeue(entry);
            // remove the event mapping
            EventEntryMapping.Remove(entry.Event);

            // first try to activate interrupted events
            while (InterruptedQueue.Count > 0 && CurrentlyUsing < Capacity)
            {
                // there is both a previously interrupted event and a free slot in the facility
                var interruptedEntry = InterruptedQueue.Dequeue();

                // activate the events claim of the facility
                Inside.Add(interruptedEntry);
                // run registered event handlers
                OnFacilityEnter?.Invoke(interruptedEntry.Event);

                // schedule the event to be run this tick
                Controller.Schedule(interruptedEntry.Event, 0);
            }

            // activate waiting events
            while (WaitingQueue.Count > 0 && CurrentlyUsing < Capacity)
            {
                // there is both a waiting event and a free slot in the facility
                var waitingEntry = WaitingQueue.Dequeue();
                // run registered event handlers
                OnQueueLeave?.Invoke(waitingEntry.Event);

                // activate the events claim of the facility
                Inside.Add(waitingEntry);
                // run registered event handlers
                OnFacilityEnter?.Invoke(waitingEntry.Event);

                // schedule the event to be run this simulation tick
                Controller.Schedule(waitingEntry.Event, 0);
            }

            // continue processing the event's behaviour
            return BehaviourResult<TEvent, TKey>.Continue();
        }
    }
}

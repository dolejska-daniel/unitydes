using System.Collections.Generic;
using NUnit.Framework;
using UnityDES;
using UnityDES.Events;
using UnityDES.Controls;
using UnityDES.Utils;

namespace Controls
{
    public class FacilityTests
    {
        public class PublicEvent : SimulationTimeEvent
        {
            public PublicEvent(int ticksPerFrame = 10) : base(ticksPerFrame)
            {
            }

            public override IEnumerator<BehaviourResult<SimulationTimeEvent, SimulationTime>> Behaviour()
            {
                // reschedule to next tick
                yield return Reschedule(QueueKey.TickLength, false);

                // remove the event from the queue
                yield return Unschedule();
            }

            public new void IncreaseKey(float time) => base.IncreaseKey(time);
        }

        public class PublicFacility : Facility<SimulationTimeEvent, SimulationTime>
        {
            public new PriorityQueue<FacilityEntry, int> WaitingQueue { get => base.WaitingQueue; }

            public new PriorityQueue<FacilityEntry, int> InterruptedQueue { get => base.InterruptedQueue; }

            public new SortedSet<FacilityEntry> Inside { get => base.Inside; }

            public new Dictionary<SimulationTimeEvent, FacilityEntry> EventEntryMapping { get => base.EventEntryMapping; }

            public PublicFacility(ISimulationController<SimulationTimeEvent, SimulationTime> controller, int capacity = 1) : base(controller, capacity)
            {
            }
        }

        [Test]
        public void Claim_Free()
        {
            var controller = new SimulationController<SimulationTimeEvent>(4);
            var facility = new PublicFacility(controller);

            var event1 = new PublicEvent(4);

            Assert.AreEqual(1, facility.Capacity);
            Assert.AreEqual(0, facility.CurrentlyUsing);
            Assert.AreEqual(0, facility.EventEntryMapping.Count);
            Assert.AreEqual(0, facility.WaitingQueue.Count);
            Assert.AreEqual(0, facility.InterruptedQueue.Count);

            var behaviour = facility.Claim(event1);

            Assert.AreEqual(1, facility.Capacity);
            Assert.AreEqual(1, facility.CurrentlyUsing);
            Assert.AreEqual(1, facility.EventEntryMapping.Count);
            Assert.AreEqual(0, facility.WaitingQueue.Count);
            Assert.AreEqual(0, facility.InterruptedQueue.Count);

            Assert.IsTrue(behaviour.ContinueBehaviour);
            Assert.IsFalse(behaviour.UnscheduleEvent);
        }

        [Test]
        public void Claim_Full()
        {
            var controller = new SimulationController<SimulationTimeEvent>(4);
            var facility = new PublicFacility(controller);

            var event1 = new PublicEvent(4);
            var event2 = new PublicEvent(4);

            facility.Claim(event1);
            Assert.AreEqual(1, facility.Capacity);
            Assert.AreEqual(1, facility.CurrentlyUsing);
            Assert.AreEqual(1, facility.EventEntryMapping.Count);
            Assert.AreEqual(0, facility.WaitingQueue.Count);
            Assert.AreEqual(0, facility.InterruptedQueue.Count);

            var behaviour = facility.Claim(event2);
            Assert.AreEqual(1, facility.Capacity);
            Assert.AreEqual(1, facility.CurrentlyUsing);
            Assert.AreEqual(2, facility.EventEntryMapping.Count);
            Assert.AreEqual(1, facility.WaitingQueue.Count);
            Assert.AreEqual(0, facility.InterruptedQueue.Count);

            Assert.IsFalse(behaviour.ContinueBehaviour);
            Assert.IsTrue(behaviour.UnscheduleEvent);
        }

        [Test]
        public void Claim_Full_Priority()
        {
            var controller = new SimulationController<SimulationTimeEvent>(4);
            var facility = new PublicFacility(controller);

            var event1 = new PublicEvent(4);
            var event2 = new PublicEvent(4);

            facility.Claim(event1);
            Assert.AreEqual(1, facility.Capacity);
            Assert.AreEqual(1, facility.CurrentlyUsing);
            Assert.AreEqual(1, facility.EventEntryMapping.Count);
            Assert.AreEqual(0, facility.WaitingQueue.Count);
            Assert.AreEqual(0, facility.InterruptedQueue.Count);

            var behaviour = facility.Claim(event2, 200);
            Assert.AreEqual(1, facility.Capacity);
            Assert.AreEqual(1, facility.CurrentlyUsing);
            Assert.AreEqual(2, facility.EventEntryMapping.Count);
            Assert.AreEqual(0, facility.WaitingQueue.Count);
            Assert.AreEqual(1, facility.InterruptedQueue.Count);

            Assert.IsTrue(facility.InterruptedQueue.Queued(facility.EventEntryMapping[event1]));
            Assert.IsTrue(facility.Inside.Contains(facility.EventEntryMapping[event2]));

            Assert.IsTrue(behaviour.ContinueBehaviour);
            Assert.IsFalse(behaviour.UnscheduleEvent);
        }

        [Test]
        public void Claim_Full_PrioritySelction()
        {
            var controller = new SimulationController<SimulationTimeEvent>(4);
            var facility = new PublicFacility(controller);

            var event1 = new PublicEvent(4);
            var event2 = new PublicEvent(4);
            var event3 = new PublicEvent(4);

            facility.Claim(event1, 75);
            facility.Claim(event2, 50);
            Assert.AreEqual(1, facility.Capacity);
            Assert.AreEqual(1, facility.CurrentlyUsing);
            Assert.AreEqual(2, facility.EventEntryMapping.Count);
            Assert.AreEqual(1, facility.WaitingQueue.Count);
            Assert.AreEqual(0, facility.InterruptedQueue.Count);

            Assert.IsTrue(facility.WaitingQueue.Queued(facility.EventEntryMapping[event2]));
            Assert.IsTrue(facility.Inside.Contains(facility.EventEntryMapping[event1]));

            var behaviour = facility.Claim(event3);
            Assert.AreEqual(1, facility.Capacity);
            Assert.AreEqual(1, facility.CurrentlyUsing);
            Assert.AreEqual(3, facility.EventEntryMapping.Count);
            Assert.AreEqual(1, facility.WaitingQueue.Count);
            Assert.AreEqual(1, facility.InterruptedQueue.Count);

            Assert.IsTrue(facility.WaitingQueue.Queued(facility.EventEntryMapping[event2]));
            Assert.IsTrue(facility.InterruptedQueue.Queued(facility.EventEntryMapping[event1]));
            Assert.IsTrue(facility.Inside.Contains(facility.EventEntryMapping[event3]));

            Assert.IsTrue(behaviour.ContinueBehaviour);
            Assert.IsFalse(behaviour.UnscheduleEvent);
        }

        [Test]
        public void Free()
        {
            var controller = new SimulationController<SimulationTimeEvent>(4);
            var facility = new PublicFacility(controller);

            var event1 = new PublicEvent(4);

            facility.Claim(event1);
            Assert.AreEqual(1, facility.Capacity);
            Assert.AreEqual(1, facility.CurrentlyUsing);
            Assert.AreEqual(1, facility.EventEntryMapping.Count);
            Assert.AreEqual(0, facility.WaitingQueue.Count);
            Assert.AreEqual(0, facility.InterruptedQueue.Count);

            var behaviour = facility.Free(event1);
            Assert.AreEqual(1, facility.Capacity);
            Assert.AreEqual(0, facility.CurrentlyUsing);
            Assert.AreEqual(0, facility.EventEntryMapping.Count);
            Assert.AreEqual(0, facility.WaitingQueue.Count);
            Assert.AreEqual(0, facility.InterruptedQueue.Count);

            Assert.IsTrue(behaviour.ContinueBehaviour);
            Assert.IsFalse(behaviour.UnscheduleEvent);
        }

        [Test]
        public void Free_WithWaiting()
        {
            var controller = new SimulationController<SimulationTimeEvent>(4);
            var facility = new PublicFacility(controller);

            var event1 = new PublicEvent(4);
            var event2 = new PublicEvent(4);
            facility.Claim(event1);

            var behaviour = facility.Claim(event2);
            Assert.AreEqual(1, facility.Capacity);
            Assert.AreEqual(1, facility.CurrentlyUsing);
            Assert.AreEqual(2, facility.EventEntryMapping.Count);
            Assert.AreEqual(1, facility.WaitingQueue.Count);
            Assert.AreEqual(0, facility.InterruptedQueue.Count);

            Assert.IsFalse(behaviour.ContinueBehaviour);
            Assert.IsTrue(behaviour.UnscheduleEvent);

            facility.Free(event1);
            Assert.AreEqual(1, facility.Capacity);
            Assert.AreEqual(1, facility.CurrentlyUsing);
            Assert.AreEqual(1, facility.EventEntryMapping.Count);
            Assert.AreEqual(0, facility.WaitingQueue.Count);
            Assert.AreEqual(0, facility.InterruptedQueue.Count);

            Assert.IsTrue(facility.Inside.Contains(facility.EventEntryMapping[event2]));
        }

        [Test]
        public void Free_WithInterrupted()
        {
            var controller = new SimulationController<SimulationTimeEvent>(4);
            var facility = new PublicFacility(controller);

            var event1 = new PublicEvent(4);
            var event2 = new PublicEvent(4);
            facility.Claim(event1);

            var behaviour = facility.Claim(event2, 200);
            Assert.AreEqual(1, facility.Capacity);
            Assert.AreEqual(1, facility.CurrentlyUsing);
            Assert.AreEqual(2, facility.EventEntryMapping.Count);
            Assert.AreEqual(0, facility.WaitingQueue.Count);
            Assert.AreEqual(1, facility.InterruptedQueue.Count);

            Assert.IsTrue(facility.InterruptedQueue.Queued(facility.EventEntryMapping[event1]));
            Assert.IsTrue(facility.Inside.Contains(facility.EventEntryMapping[event2]));

            Assert.IsTrue(behaviour.ContinueBehaviour);
            Assert.IsFalse(behaviour.UnscheduleEvent);

            facility.Free(event2);
            Assert.AreEqual(1, facility.Capacity);
            Assert.AreEqual(1, facility.CurrentlyUsing);
            Assert.AreEqual(1, facility.EventEntryMapping.Count);
            Assert.AreEqual(0, facility.WaitingQueue.Count);
            Assert.AreEqual(0, facility.InterruptedQueue.Count);

            Assert.IsTrue(facility.Inside.Contains(facility.EventEntryMapping[event1]));
        }

        [Test]
        public void Free_WithWaiting_WithInterrupted()
        {
            var controller = new SimulationController<SimulationTimeEvent>(4);
            var facility = new PublicFacility(controller);

            var event1 = new PublicEvent(4);
            var event2 = new PublicEvent(4);
            var event3 = new PublicEvent(4);
            facility.Claim(event1);
            facility.Claim(event2, 200);
            facility.Claim(event3);

            Assert.AreEqual(1, facility.Capacity);
            Assert.AreEqual(1, facility.CurrentlyUsing);
            Assert.AreEqual(3, facility.EventEntryMapping.Count);
            Assert.AreEqual(1, facility.WaitingQueue.Count);
            Assert.AreEqual(1, facility.InterruptedQueue.Count);

            Assert.IsTrue(facility.InterruptedQueue.Queued(facility.EventEntryMapping[event1]));
            Assert.IsTrue(facility.WaitingQueue.Queued(facility.EventEntryMapping[event3]));
            Assert.IsTrue(facility.Inside.Contains(facility.EventEntryMapping[event2]));

            facility.Free(event2);
            Assert.AreEqual(1, facility.Capacity);
            Assert.AreEqual(1, facility.CurrentlyUsing);
            Assert.AreEqual(2, facility.EventEntryMapping.Count);
            Assert.AreEqual(1, facility.WaitingQueue.Count);
            Assert.AreEqual(0, facility.InterruptedQueue.Count);

            Assert.IsTrue(facility.WaitingQueue.Queued(facility.EventEntryMapping[event3]));
            Assert.IsTrue(facility.Inside.Contains(facility.EventEntryMapping[event1]));

            facility.Free(event1);
            Assert.AreEqual(1, facility.Capacity);
            Assert.AreEqual(1, facility.CurrentlyUsing);
            Assert.AreEqual(1, facility.EventEntryMapping.Count);
            Assert.AreEqual(0, facility.WaitingQueue.Count);
            Assert.AreEqual(0, facility.InterruptedQueue.Count);

            Assert.IsTrue(facility.Inside.Contains(facility.EventEntryMapping[event3]));
        }
    }
}
using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using UnityDES;
using UnityDES.Events;
using UnityDES.Utils;


namespace Events
{
    public class EventTests
    {
        public class PublicTestEvent : SimulationTimeEvent
        {
            public PublicTestEvent(int ticksPerFrame = 10) : base(ticksPerFrame)
            {
            }

            public int State { get; protected set; } = 0;

            public PublicTestEvent ScheduledEvent { get; protected set; }

            public override IEnumerator<BehaviourResult<SimulationTimeEvent, SimulationTime>> Behaviour()
            {
                State = 1;
                // reschedule to next tick
                yield return Reschedule(QueueKey.TickLength, false);

                State = 2;
                // continue processing the behaviour
                yield return Continue();

                State = 3;
                ScheduledEvent = new PublicTestEvent(QueueKey.TicksPerFrame);
                // reschedule to tick after the next one
                yield return ScheduleNewAndContinue(ScheduledEvent, QueueKey.TickLength * 4);

                State = 4;
                // remove the event from the queue
                yield return Unschedule();
            }

            public new void IncreaseKey(float time) => base.IncreaseKey(time);
        }

        public class PublicSimulationController : SimulationController<SimulationTimeEvent>
        {
            public PriorityQueue<SimulationTimeEvent, SimulationTime> QueuedEvents { get => Events; }

            public PublicSimulationController(int ticksPerFrame) : base(ticksPerFrame)
            {
            }
        }

        [Test]
        public void IncreaseKey()
        {
            var simTime = new SimulationTime(4);
            var @event = new PublicTestEvent();
            @event.QueueKey = new SimulationTime(simTime);

            Assert.AreEqual(simTime, @event.QueueKey);
            simTime.DoTick();
            Assert.AreNotEqual(simTime, @event.QueueKey);
            @event.IncreaseKey(simTime.TickLength);
            Assert.AreEqual(simTime, @event.QueueKey);

            simTime.DoTick(2);
            Assert.AreNotEqual(simTime, @event.QueueKey);
            @event.IncreaseKey(simTime.TickLength * 2);
            Assert.AreEqual(simTime, @event.QueueKey);

            simTime.DoTick(14);
            Assert.AreNotEqual(simTime, @event.QueueKey);
            @event.IncreaseKey(simTime.TickLength * 14);
            Assert.AreEqual(simTime, @event.QueueKey);
        }

        [Test]
        public void Behaviour()
        {
            var @event = new PublicTestEvent();
            @event.QueueKey = new SimulationTime(4);

            var iterator = @event.Behaviour();
            Assert.AreEqual(0, @event.State);
            Assert.IsTrue(iterator.Current.RescheduleTime == 0);
            Assert.IsFalse(iterator.Current.ContinueBehaviour);
            Assert.IsFalse(iterator.Current.ScheduleNewEvent);
            Assert.IsFalse(iterator.Current.UnscheduleEvent);
            Assert.IsFalse(iterator.Current.ResetBehaviour);

            Assert.IsTrue(iterator.MoveNext());
            Assert.AreEqual(1, @event.State);
            Assert.AreEqual(.25f, iterator.Current.RescheduleTime);
            Assert.IsFalse(iterator.Current.ContinueBehaviour);
            Assert.IsFalse(iterator.Current.ScheduleNewEvent);
            Assert.IsFalse(iterator.Current.UnscheduleEvent);
            Assert.IsFalse(iterator.Current.ResetBehaviour);

            Assert.IsTrue(iterator.MoveNext());
            Assert.AreEqual(2, @event.State);
            Assert.IsTrue(iterator.Current.RescheduleTime <= 0);
            Assert.IsTrue(iterator.Current.ContinueBehaviour);
            Assert.IsFalse(iterator.Current.ScheduleNewEvent);
            Assert.IsFalse(iterator.Current.UnscheduleEvent);
            Assert.IsFalse(iterator.Current.ResetBehaviour);

            Assert.IsTrue(iterator.MoveNext());
            Assert.AreEqual(3, @event.State);
            Assert.IsTrue(iterator.Current.RescheduleTime <= 0);
            Assert.IsTrue(iterator.Current.ContinueBehaviour);
            Assert.IsTrue(iterator.Current.ScheduleNewEvent);
            Assert.IsFalse(iterator.Current.UnscheduleEvent);
            Assert.IsFalse(iterator.Current.ResetBehaviour);

            Assert.IsTrue(iterator.MoveNext());
            Assert.AreEqual(4, @event.State);
            Assert.IsTrue(iterator.Current.RescheduleTime <= 0);
            Assert.IsFalse(iterator.Current.ContinueBehaviour);
            Assert.IsFalse(iterator.Current.ScheduleNewEvent);
            Assert.IsTrue(iterator.Current.UnscheduleEvent);
            Assert.IsFalse(iterator.Current.ResetBehaviour);

            Assert.IsFalse(iterator.MoveNext());
            Assert.AreEqual(4, @event.State);
        }

        [Test]
        public void Run_Complex()
        {
            var event1 = new PublicTestEvent(4);
            var event2 = new PublicTestEvent(4);

            var sim = new PublicSimulationController(event1.QueueKey.TicksPerFrame);
            sim.QueuedEvents.Enqueue(event1);
            sim.QueuedEvents.Enqueue(event2);

            Assert.AreSame(event1, sim.QueuedEvents.Peek());
            Assert.AreEqual(2, sim.QueuedEvents.Count);

            event1.Run(sim);
            Assert.AreEqual(1, event1.State);
            Assert.AreNotSame(event1, sim.QueuedEvents.Peek());
            Assert.AreSame(event2, sim.QueuedEvents.Peek());
            Assert.AreEqual(2, sim.QueuedEvents.Count);

            event2.QueueKey.Tick = 2;
            sim.QueuedEvents.Update(event2);

            Assert.AreSame(event1, sim.QueuedEvents.Peek());
            Assert.AreEqual(1, event1.State);

            event1.Run(sim);
            Assert.AreEqual(4, event1.State);
            Assert.AreNotSame(event1, sim.QueuedEvents.Peek());
            Assert.AreSame(event2, sim.QueuedEvents.Peek());
            Assert.AreEqual(2, sim.QueuedEvents.Count);

            Assert.AreSame(event2, sim.QueuedEvents.Dequeue());
            Assert.AreEqual(1, sim.QueuedEvents.Count);

            Assert.AreSame(event1.ScheduledEvent, sim.QueuedEvents.Dequeue());
            Assert.AreEqual(0, sim.QueuedEvents.Count);
        }
    }
}
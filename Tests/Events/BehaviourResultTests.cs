using System.Collections.Generic;
using NUnit.Framework;
using UnityDES;
using UnityDES.Events;


namespace Events
{
    public class BehaviourResultTests
    {
        public class PublicTestEvent : SimulationTimeEvent
        {
            public PublicTestEvent(int ticksPerFrame = 10) : base(ticksPerFrame)
            {
            }

            public override IEnumerator<BehaviourResult<SimulationTimeEvent, SimulationTime>> Behaviour()
            {
                yield return Reschedule(1f);
            }
        }

        [Test]
        public void Continue()
        {
            BehaviourResult<SimulationTimeEvent, SimulationTime> result;

            result = BehaviourResult<SimulationTimeEvent, SimulationTime>.Continue();
            Assert.IsTrue(result.ContinueBehaviour);
            Assert.IsFalse(result.ScheduleNewEvent);
            Assert.IsFalse(result.UnscheduleEvent);
            Assert.IsFalse(result.ResetBehaviour);
            Assert.IsNull(result.NewEvent);
            Assert.IsTrue(result.RescheduleTime < 0);

            result = BehaviourResult<SimulationTimeEvent, SimulationTime>.Continue(true);
            Assert.IsTrue(result.ContinueBehaviour);
            Assert.IsFalse(result.ScheduleNewEvent);
            Assert.IsFalse(result.UnscheduleEvent);
            Assert.IsTrue(result.ResetBehaviour);
            Assert.IsNull(result.NewEvent);
            Assert.IsTrue(result.RescheduleTime < 0);
        }

        [Test]
        public void Reschedule()
        {
            BehaviourResult<SimulationTimeEvent, SimulationTime> result;

            result = BehaviourResult<SimulationTimeEvent, SimulationTime>.Reschedule(1f, false);
            Assert.IsFalse(result.ContinueBehaviour);
            Assert.IsFalse(result.ScheduleNewEvent);
            Assert.IsFalse(result.UnscheduleEvent);
            Assert.IsFalse(result.ResetBehaviour);
            Assert.IsNull(result.NewEvent);
            Assert.AreEqual(1f, result.RescheduleTime);

            result = BehaviourResult<SimulationTimeEvent, SimulationTime>.Reschedule(1f);
            Assert.IsFalse(result.ContinueBehaviour);
            Assert.IsFalse(result.ScheduleNewEvent);
            Assert.IsFalse(result.UnscheduleEvent);
            Assert.IsTrue(result.ResetBehaviour);
            Assert.IsNull(result.NewEvent);
            Assert.AreEqual(1f, result.RescheduleTime);
        }

        [Test]
        public void Unschedule()
        {
            BehaviourResult<SimulationTimeEvent, SimulationTime> result;

            result = BehaviourResult<SimulationTimeEvent, SimulationTime>.Unschedule();
            Assert.IsFalse(result.ContinueBehaviour);
            Assert.IsFalse(result.ScheduleNewEvent);
            Assert.IsTrue(result.UnscheduleEvent);
            Assert.IsFalse(result.ResetBehaviour);
            Assert.IsNull(result.NewEvent);
            Assert.IsTrue(result.RescheduleTime < 0);
        }

        [Test]
        public void ScheduleNew()
        {
            BehaviourResult<SimulationTimeEvent, SimulationTime> result;
            var newEvent = new PublicTestEvent();

            result = BehaviourResult<SimulationTimeEvent, SimulationTime>.ScheduleNew(1f, newEvent, 2f);
            Assert.IsFalse(result.ContinueBehaviour);
            Assert.IsTrue(result.ScheduleNewEvent);
            Assert.IsFalse(result.UnscheduleEvent);
            Assert.IsFalse(result.ResetBehaviour);
            Assert.AreSame(newEvent, result.NewEvent);
            Assert.AreEqual(2f, result.NewEventScheduleTime);
            Assert.AreEqual(1f, result.RescheduleTime);

            result = BehaviourResult<SimulationTimeEvent, SimulationTime>.ScheduleNew(1f, newEvent, 2f, true);
            Assert.IsFalse(result.ContinueBehaviour);
            Assert.IsTrue(result.ScheduleNewEvent);
            Assert.IsFalse(result.UnscheduleEvent);
            Assert.IsTrue(result.ResetBehaviour);
            Assert.AreSame(newEvent, result.NewEvent);
            Assert.AreEqual(2f, result.NewEventScheduleTime);
            Assert.AreEqual(1f, result.RescheduleTime);
        }

        [Test]
        public void ScheduleNewAndContinue()
        {
            BehaviourResult<SimulationTimeEvent, SimulationTime> result;
            var newEvent = new PublicTestEvent();

            result = BehaviourResult<SimulationTimeEvent, SimulationTime>.ScheduleNewAndContinue(newEvent, 2f);
            Assert.IsTrue(result.ContinueBehaviour);
            Assert.IsTrue(result.ScheduleNewEvent);
            Assert.IsFalse(result.UnscheduleEvent);
            Assert.IsFalse(result.ResetBehaviour);
            Assert.AreSame(newEvent, result.NewEvent);
            Assert.AreEqual(2f, result.NewEventScheduleTime);
            Assert.IsTrue(result.RescheduleTime < 0);

            result = BehaviourResult<SimulationTimeEvent, SimulationTime>.ScheduleNewAndContinue(newEvent, 2f, true);
            Assert.IsTrue(result.ContinueBehaviour);
            Assert.IsTrue(result.ScheduleNewEvent);
            Assert.IsFalse(result.UnscheduleEvent);
            Assert.IsTrue(result.ResetBehaviour);
            Assert.AreSame(newEvent, result.NewEvent);
            Assert.AreEqual(2f, result.NewEventScheduleTime);
            Assert.IsTrue(result.RescheduleTime < 0);
        }
    }
}
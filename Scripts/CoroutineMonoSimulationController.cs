using System.Collections;
using UnityEngine;
using UnityDES.Events;

namespace UnityDES
{
    /// <summary>
    /// The MonoBehaviour proxy class of <see cref="SimulationController"/>.
    /// Simulation ticks will also be run in a coroutine.
    /// </summary>
    public class CoroutineMonoSimulationController : MonoBehaviour, ISimulationController<EventBase<SimulationTime>, SimulationTime>
    {
        //==========================================================================dd==
        //  MonoBehaviour METHODS
        //==========================================================================dd==

        public int TicksPerFrame;

        protected virtual void Start()
        {
            Controller = new SimulationController(TicksPerFrame);
            StartCoroutine("RunAvailableTicksCoroutine");
        }

        //==========================================================================dd==
        //  ISimulationController METHODS
        //==========================================================================dd==

        protected SimulationController Controller { get; set; }

        public SimulationTime SimulationTime => Controller.SimulationTime;
        
        public int SimulationSpeed { get => Controller.SimulationSpeed; set => Controller.SimulationSpeed = value; }

        public bool Reschedule(EventBase<SimulationTime> @event) => Controller.Reschedule(@event);

        public void RunAvailableTicks(float deltaTime) => Controller.RunAvailableTicks(deltaTime);

        public IEnumerator RunAvailableTicksCoroutine() => Controller.RunAvailableTicksCoroutine();

        public void Schedule(EventBase<SimulationTime> @event, int tickCount = 1) => Controller.Schedule(@event, tickCount);

        public bool Unschedule(EventBase<SimulationTime> @event) => Controller.Unschedule(@event);
    }
}

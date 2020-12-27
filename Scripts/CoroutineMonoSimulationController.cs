using System.Collections;
using UnityEngine;
using UnityDES.Events;

namespace UnityDES
{
    /// <summary>
    /// The MonoBehaviour proxy class of <see cref="SimulationController"/>.
    /// Simulation ticks will also be run in a coroutine.
    /// </summary>
    public class CoroutineMonoSimulationController : MonoBehaviour, ISimulationController<SimulationTimeEvent, SimulationTime>
    {
        //==========================================================================dd==
        //  MonoBehaviour METHODS
        //==========================================================================dd==

        public int TicksPerFrame;

        protected virtual void Start()
        {
            Controller = new SimulationController<SimulationTimeEvent>(TicksPerFrame);
            StartCoroutine("RunAvailableTicksCoroutine");
        }

        //==========================================================================dd==
        //  ISimulationController METHODS
        //==========================================================================dd==

        protected SimulationController<SimulationTimeEvent> Controller { get; set; }

        public SimulationTime SimulationTime => Controller.SimulationTime;
        
        public int SimulationSpeed { get => Controller.SimulationSpeed; set => Controller.SimulationSpeed = value; }

        public bool Reschedule(SimulationTimeEvent @event) => Controller.Reschedule(@event);

        public void RunAvailableTicks(float deltaTime) => Controller.RunAvailableTicks(deltaTime);

        public IEnumerator RunAvailableTicksCoroutine() => Controller.RunAvailableTicksCoroutine();

        public void Schedule(SimulationTimeEvent @event, float scheduleTime) => Controller.Schedule(@event, scheduleTime);

        public void Schedule(SimulationTimeEvent @event, int tickCount = 1) => Controller.Schedule(@event, tickCount);

        public bool Unschedule(SimulationTimeEvent @event) => Controller.Unschedule(@event);
    }
}

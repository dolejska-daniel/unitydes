﻿
namespace UnityDES.Events
{
    /// <summary>
    /// Simple base class for any event implementations with <see cref="SimulationTime"/> as a queue key.
    /// </summary>
    public abstract class SimulationTimeEvent : EventBase<SimulationTimeEvent, SimulationTime>
    {
        protected SimulationTimeEvent(SimulationTime simulationTime) : base()
        {
            QueueKey = new SimulationTime(simulationTime);
        }

        protected SimulationTimeEvent(int ticksPerFrame) : base()
        {
            QueueKey = new SimulationTime(ticksPerFrame);
        }

        protected override SimulationTimeEvent This() => this;

        protected override void IncreaseKey(float time) => QueueKey.DoTick(time);
    }
}

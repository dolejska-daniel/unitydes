
namespace UnityDES.Events
{
    /// <summary>
    /// Simple base class for any event implementations with <see cref="SimulationTime"/> as a queue key.
    /// </summary>
    public abstract class SimulationTimeEvent : EventBase<SimulationTime>
    {
        protected SimulationTimeEvent(int ticksPerFrame) : base()
        {
            QueueKey = new SimulationTime(ticksPerFrame);
        }

        protected override void IncreaseKey(float time) => QueueKey.DoTick(time);
    }
}

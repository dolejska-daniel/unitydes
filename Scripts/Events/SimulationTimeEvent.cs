
namespace UnityDES.Events
{
    public abstract class SimulationTimeEvent : EventBase<SimulationTime>
    {
        protected override void IncreaseKey(float time) => QueueKey.DoTick(time);
    }
}

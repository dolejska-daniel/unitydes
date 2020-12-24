using System;
using System.Collections.Generic;

namespace UnityDES
{
    /// <summary>
    /// Structure representing concrete time position within the simulation.
    /// </summary>
    public class SimulationTime
    {
        /// <summary>
        /// Number of simulation frames.
        /// Single simulation frame represents one physical second (for simulation speed = 1).
        /// </summary>
        public int Frame { get; set; }

        /// <summary>
        /// Number of processed ticks in current simulation frame.
        /// </summary>
        public int Tick { get; set; }

        /// <summary>
        /// Number of ticks per <b>simulation frame</b>.
        /// </summary>
        public int TicksPerFrame { get; }

        /// <summary>
        /// Time length of a single tick of the simulation time.
        /// </summary>
        public float TickLength { get => 1f / TicksPerFrame; }

        public SimulationTime(SimulationTime simTime)
            : this(simTime.TicksPerFrame, simTime.Frame, simTime.Tick)
        {
        }

        public SimulationTime(int ticksPerFrame, int frame = 0, int tick = 0)
        {
            TicksPerFrame = ticksPerFrame;
            Frame = frame;
            Tick = tick;
        }

        /// <summary>
        /// Performs Tick (and Frame) offset equal to provided amount of time (<paramref name="time"/>).
        /// </summary>
        /// 
        /// <param name="time">Minimum amount of time to be skipped</param>
        public void DoTick(float time) => DoTick((int)Math.Ceiling(TicksPerFrame * time));

        /// <summary>
        /// Performs Tick (and Frame) offset equal to provided amount of Ticks (<paramref name="tickCount"/>).
        /// </summary>
        /// 
        /// <param name="tickCount">Number of ticks to be skipped</param>
        public void DoTick(int tickCount = 1)
        {
            Tick += tickCount;
            if (Tick >= TicksPerFrame)
            {
                Frame += Tick / TicksPerFrame;
                Tick %= TicksPerFrame;
            }
        }

        public override bool Equals(object obj)
        {
            var o = obj as SimulationTime;
            return o != null && GetHashCode() == o.GetHashCode();
        }

        public override int GetHashCode() => Tuple.Create(Frame, Tick, TicksPerFrame).GetHashCode();

        public override string ToString() => $"SimulationTime({Frame}:{Tick}/{TicksPerFrame})";

        /// <summary>
        /// Simulation time instance comparer.
        /// </summary>
        public static readonly Comparer<SimulationTime> Comparer = Comparer<SimulationTime>.Create((a, b) =>
        {
            var frameDiff = a.Frame - b.Frame;
            return frameDiff != 0
                ? frameDiff
                : a.Tick - b.Tick;
        });

        public static bool operator <(SimulationTime a, SimulationTime b)
            => Comparer.Compare(a, b) < 0;

        public static bool operator >(SimulationTime a, SimulationTime b)
            => Comparer.Compare(a, b) > 0;

        public static bool operator <=(SimulationTime a, SimulationTime b)
            => Comparer.Compare(a, b) <= 0;

        public static bool operator >=(SimulationTime a, SimulationTime b)
            => Comparer.Compare(a, b) >= 0;

        public static bool operator ==(SimulationTime a, SimulationTime b)
            => a?.GetHashCode() == b?.GetHashCode();

        public static bool operator !=(SimulationTime a, SimulationTime b)
            => a?.GetHashCode() != b?.GetHashCode();
    }
}

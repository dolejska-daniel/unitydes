using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using UnityDES;

public class SimulationTimeTests
{
    public static SimulationTime SimulationTime = new SimulationTime(10, 1);

    [Test]
    public void OperatorOverrides()
    {
        SimulationTime simTime, simTimeWrong;

        simTime = new SimulationTime(SimulationTime.TicksPerFrame);
        simTimeWrong = new SimulationTime(4);
        Assert.IsTrue(simTime < SimulationTime);
        Assert.IsTrue(simTime <= SimulationTime);
        Assert.IsFalse(simTime > SimulationTime);
        Assert.IsFalse(simTime >= SimulationTime);

        Assert.IsTrue(SimulationTime > simTime);
        Assert.IsTrue(SimulationTime >= simTime);
        Assert.IsFalse(SimulationTime < simTime);
        Assert.IsFalse(SimulationTime <= simTime);

        Assert.IsTrue(SimulationTime != simTime);
        Assert.IsFalse(SimulationTime == simTime);

        Assert.IsTrue(SimulationTime != simTimeWrong);
        Assert.IsTrue(simTime != simTimeWrong);
        Assert.IsFalse(simTime.Equals(simTimeWrong));
        Assert.IsFalse(SimulationTime.Equals(simTimeWrong));
        Assert.IsFalse(simTime.GetHashCode() == simTimeWrong.GetHashCode());
        Assert.IsFalse(SimulationTime.GetHashCode() == simTimeWrong.GetHashCode());

        simTime = new SimulationTime(SimulationTime.TicksPerFrame, SimulationTime.Frame, SimulationTime.Tick);
        simTimeWrong = new SimulationTime(4, SimulationTime.Frame, SimulationTime.Tick);
        Assert.IsTrue(simTime >= SimulationTime);
        Assert.IsTrue(simTime <= SimulationTime);

        Assert.IsTrue(SimulationTime >= simTime);
        Assert.IsTrue(SimulationTime <= simTime);

        Assert.IsTrue(simTime == SimulationTime);
        Assert.IsFalse(simTime != SimulationTime);

        Assert.IsTrue(SimulationTime != simTimeWrong);
        Assert.IsTrue(simTime != simTimeWrong);
        Assert.IsFalse(simTime.Equals(simTimeWrong));
        Assert.IsFalse(SimulationTime.Equals(simTimeWrong));
        Assert.IsFalse(simTime.GetHashCode() == simTimeWrong.GetHashCode());
        Assert.IsFalse(SimulationTime.GetHashCode() == simTimeWrong.GetHashCode());
    }

    [Test]
    public void DoTick()
    {
        var simTime = new SimulationTime(4, 0, 0);

        Assert.AreEqual(0, simTime.Tick);
        Assert.AreEqual(0, simTime.Frame);
        Assert.AreEqual(4, simTime.TicksPerFrame);

        simTime.DoTick();
        Assert.AreEqual(1, simTime.Tick);
        Assert.AreEqual(0, simTime.Frame);
        Assert.AreEqual(4, simTime.TicksPerFrame);

        simTime.DoTick();
        Assert.AreEqual(2, simTime.Tick);
        Assert.AreEqual(0, simTime.Frame);

        simTime.DoTick(2);
        Assert.AreEqual(0, simTime.Tick);
        Assert.AreEqual(1, simTime.Frame);

        simTime.DoTick(4);
        Assert.AreEqual(0, simTime.Tick);
        Assert.AreEqual(2, simTime.Frame);

        simTime.DoTick(8);
        Assert.AreEqual(0, simTime.Tick);
        Assert.AreEqual(4, simTime.Frame);
    }

    [Test]
    public void DoTick_Time()
    {
        var simTime = new SimulationTime(4, 0, 0);

        Assert.AreEqual(0, simTime.Tick);
        Assert.AreEqual(0, simTime.Frame);
        Assert.AreEqual(4, simTime.TicksPerFrame);

        simTime.DoTick(.1f);
        Assert.AreEqual(1, simTime.Tick);
        Assert.AreEqual(0, simTime.Frame);
        Assert.AreEqual(4, simTime.TicksPerFrame);

        simTime.DoTick(.25f);
        Assert.AreEqual(2, simTime.Tick);
        Assert.AreEqual(0, simTime.Frame);

        simTime.DoTick(.26f);
        Assert.AreEqual(0, simTime.Tick);
        Assert.AreEqual(1, simTime.Frame);

        simTime.DoTick(1f);
        Assert.AreEqual(0, simTime.Tick);
        Assert.AreEqual(2, simTime.Frame);

        simTime.DoTick(2f);
        Assert.AreEqual(0, simTime.Tick);
        Assert.AreEqual(4, simTime.Frame);
    }

    [Test]
    public void DoTick_OperatorOverrides()
    {
        var simTime = new SimulationTime(10, 0, 9);

        Assert.IsTrue(simTime <  SimulationTime);
        Assert.IsTrue(simTime <= SimulationTime);
        Assert.IsFalse(simTime >  SimulationTime);
        Assert.IsFalse(simTime >= SimulationTime);

        simTime.DoTick();
        Assert.IsFalse(SimulationTime >  simTime);
        Assert.IsTrue(SimulationTime  >= simTime);
        Assert.IsFalse(SimulationTime <  simTime);
        Assert.IsTrue(SimulationTime  <= simTime);

        Assert.IsFalse(SimulationTime != simTime);
        Assert.IsTrue(SimulationTime == simTime);

        simTime.DoTick();
        Assert.IsFalse(SimulationTime >  simTime);
        Assert.IsFalse(SimulationTime >= simTime);
        Assert.IsTrue(SimulationTime  <  simTime);
        Assert.IsTrue(SimulationTime  <= simTime);

        Assert.IsTrue(SimulationTime != simTime);
        Assert.IsFalse(SimulationTime == simTime);
    }
}
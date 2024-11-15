#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.UI;
using FTOptix.NativeUI;
using FTOptix.HMIProject;
using FTOptix.Retentivity;
using FTOptix.CoreBase;
using FTOptix.Core;
using FTOptix.NetLogic;
using FTOptix.OPCUAServer;
#endregion

[CustomBehavior]
public class MotorTypeBehavior : BaseNetBehavior
{
    public override void Start()
    {
        // Insert code to be executed when the user-defined behavior is started
        temperatureTask?.Dispose();
        temperatureTask = new PeriodicTask(CalculateTemperature, 100, Node);
        temperatureTask.Start();
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined behavior is stopped
        temperatureTask?.Dispose();
        rampMotorTask?.Dispose();
    }

    [ExportMethod]
    public void StartMotor(int targetSpeed)
    {
        Log.Info($"Starting {Node.BrowseName} at {targetSpeed} RPM");
        Node.IsRunning = true;
        targetMotorSpeed = targetSpeed;
        // Simulate ramping up the motor speed
        rampMotorTask?.Dispose();
        rampMotorTask = new LongRunningTask(RampSpeed, Node);
        rampMotorTask.Start();
    }

    [ExportMethod]
    public void StopMotor()
    {
        Log.Info($"Stopping {Node.BrowseName}");
        Node.IsRunning = false;
        targetMotorSpeed = 0;
        // Simulate ramping down the motor speed
        rampMotorTask?.Dispose();
        rampMotorTask = new LongRunningTask(RampSpeed, Node);
        rampMotorTask.Start();
    }

    private void CalculateTemperature()
    {
        // If the motor is running, ramp up the temperature
        if (Node.IsRunning && Node.Temperature < 40)
            Node.Temperature++;
        else if (!Node.IsRunning && Node.Temperature >= 20)
            Node.Temperature--;
        else
            Node.Temperature = (float) ((Node.IsRunning ? 40 : 20) + (0.1 * rnd.Next(-10, 10)));
    }

    private void RampSpeed()
    {
        Log.Info($"Ramping {Node.BrowseName} speed to {targetMotorSpeed} RPM");

        while (Node.CurrentSpeed != targetMotorSpeed)
        {
            if (rampMotorTask.IsCancellationRequested)
                return;
            else
                System.Threading.Thread.Sleep(50);

            if (Node.CurrentSpeed < targetMotorSpeed)
                Node.CurrentSpeed++;
            else
                Node.CurrentSpeed--;
        }
    }

    private int targetMotorSpeed = 100;
    private readonly Random rnd = new Random();
    private PeriodicTask temperatureTask;
    private LongRunningTask rampMotorTask;

#region Auto-generated code, do not edit!
    protected new MotorType Node => (MotorType)base.Node;
#endregion
}

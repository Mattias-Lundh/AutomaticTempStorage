namespace AutomaticTempStorage
{
    using System;
    using System.Timers;

    using AutomaticTempStorage.schedule;

    public class Heartbeat
    {
        private Timer timer;
        private IHeartbeatSubscriber subscriber;

        public Heartbeat(IHeartbeatSubscriber heartbeatSubscriber)
        {
            this.timer = new Timer(1000) { AutoReset = true };
            this.subscriber = heartbeatSubscriber;
            this.timer.Elapsed += this.TimerElapsed;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            this.subscriber.OnElapsed();
        }

        public void Start()
        {
            this.timer.Start();
        }

        public void Stop()
        {
            this.timer.Stop();
        }

    }
}

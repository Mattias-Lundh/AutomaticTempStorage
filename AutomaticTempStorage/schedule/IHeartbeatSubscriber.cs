using System;
using System.Collections.Generic;
using System.Text;

namespace AutomaticTempStorage.schedule
{
    public interface IHeartbeatSubscriber
    {
        void OnElapsed();
    }
}

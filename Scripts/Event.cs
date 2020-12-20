using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityDES.Utils;

namespace UnityDES
{
    /// <summary>
    /// 
    /// </summary>
    public class Event : IQueueItem<int>
    {
        public int QueueKey { get; set; }
    }
}

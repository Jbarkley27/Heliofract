using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PixelNarval.HPBars
{
    public interface ItargetValueReacher : IEventSystemHandler
    {
        public void targetValueReach(TransitionEventValue value);
    }
}
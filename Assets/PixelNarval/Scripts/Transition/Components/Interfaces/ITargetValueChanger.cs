using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PixelNarval.HPBars
{
    public interface ItargetValueChanger : IEventSystemHandler
    {        
        public void targetValueChange(TransitionEventValue value);
    }
}
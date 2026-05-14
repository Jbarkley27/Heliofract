using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PixelNarval.HPBars
{
    public interface ILastValueChanger : IEventSystemHandler
    {
        public void LastValueChange(TransitionEventValue value);
    }
}
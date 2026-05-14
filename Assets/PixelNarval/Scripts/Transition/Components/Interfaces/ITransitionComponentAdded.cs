using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PixelNarval.HPBars
{
    public interface ITransitionComponentAdded : IEventSystemHandler
    {
        void OnTransitionComponentAdded(TransitionComponent bc);
    }
}
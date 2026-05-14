using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PixelNarval.HPBars
{
    
    [AddComponentMenu("PixelNarval/Transition/CanvasGroupAlphaColorTC")]
    public class CanvasGroupAlphaColorTC : TransitionComponent, IcurrentValueChanger
    {
        [Header("References")]
        [NoNull] [SerializeField] private CanvasGroup canvasGroup;

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void LoadIcon()
        {
            IconsUtils.LoadIcon("CanvasGroupAlphaColorTC", "Outline Icon");
        }
#endif

        public void currentValueChange(TransitionEventValue _)
        {
            SetValue();
        }
        private void SetValue()
        {
            if (canvasGroup == null)
            {
                return;
            }
            canvasGroup.alpha = CurrentValue;
        }


    }
}
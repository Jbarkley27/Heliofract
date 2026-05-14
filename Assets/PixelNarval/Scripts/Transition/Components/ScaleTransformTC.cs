using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PixelNarval.HPBars
{
    [AddComponentMenu("PixelNarval/Transition/ScaleTransformTC")]
    public class ScaleTransformTC : TransitionComponent, IcurrentValueChanger, ItargetValueChanger, ILastValueChanger
    {
        [Header("References")]
        [NoNull] [SerializeField] protected Transform baseTransform;
        [SerializeField] protected Transform echoTransform;

        [System.Flags]
        protected enum ScaleAxisEnum
        {
            X = 1,
            Y = 2,
            Z = 4,
        };

        [Header("Configuration")]
        [SerializeField] ScaleAxisEnum scaleAxis;
        [SerializeField] [Min(0)] float minScaleValue = 0;
        [SerializeField] [Min(0)] float maxScaleValue = 1;
        [SerializeField] public bool invertValue;

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void LoadIcon()
        {
            IconsUtils.LoadIcon("ScaleTransformTC", "ScaleTool On");
        }
#endif

        public void UpdateValues()
        {
            if (baseTransform == null)
            {
                return;
            }
            float current = invertValue ? 1 - CurrentValue : CurrentValue;
            current = Mathf.Lerp(minScaleValue, maxScaleValue, current);
            float target = invertValue ? 1 - TargetValue : TargetValue;
            target = Mathf.Lerp(minScaleValue, maxScaleValue, target);

            Vector3 newCurrentScale = Vector3.one;
            Vector3 newTargetScale = Vector3.one;

            if ((scaleAxis & ScaleAxisEnum.X) != 0)
            {
                newCurrentScale.x = current;
                newTargetScale.x = target;
            }

            if ((scaleAxis & ScaleAxisEnum.Y) != 0)
            {
                newCurrentScale.y = current;
                newTargetScale.y = target;
            }

            if ((scaleAxis & ScaleAxisEnum.Z) != 0)
            {
                newCurrentScale.z = current;
                newTargetScale.z = target;
            }

            if (echoTransform == null)
            {
                baseTransform.localScale = newCurrentScale;
            }
            else
            {
                if (Data.Direction > 0) //Heal
                {
                    baseTransform.localScale = newCurrentScale;
                    echoTransform.localScale = newTargetScale;
                }
                else //Damage
                {
                    baseTransform.localScale = newTargetScale;
                    echoTransform.localScale = newCurrentScale;
                }
            }
        }


            

        public void currentValueChange(TransitionEventValue _)
        {
            UpdateValues();
        }

        public void targetValueChange(TransitionEventValue _)
        {
            UpdateValues();
        }

        public void LastValueChange(TransitionEventValue _)
        {
            UpdateValues();
        }
    }
}
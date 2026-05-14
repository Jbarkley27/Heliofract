using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PixelNarval.HPBars
{
    [AddComponentMenu("PixelNarval/Transition/RotateTransformTC")]
    public class RotateTransformTC : TransitionComponent, IcurrentValueChanger
    {
        [Header("References")]
        [NoNull] [SerializeField] protected Transform baseTransform;
        [SerializeField] protected Transform echoTransform;

        [Header("Configuration")]
        [SerializeField] protected bool clockwise = true;
        [SerializeField] protected bool local = true;
        [SerializeField] protected Vector3 offset;

        [System.Flags]
        protected enum ScaleAxisEnum
        {
            X = 1,
            Y = 2,
            Z = 4,
        };

        [SerializeField] ScaleAxisEnum rotationAxis;
        [SerializeField] public bool invertValue;

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void LoadIcon()
        {
            IconsUtils.LoadIcon("RotateTransformTC", "d_RotateTool On");
        }
#endif

        private void OnValidate()
        {
            if (gameObject.activeInHierarchy)
            {
                UpdateValues();

            }
        }

        public void UpdateValues()
        {
            if (baseTransform == null)
            {
                return;
            }
            
            Vector3 newCurrentRotation = -offset;
            Vector3 newTargetRotation = -offset;
            float direction = (clockwise ? -1 : 1);

            if ((rotationAxis & ScaleAxisEnum.X) != 0)
            {
                newCurrentRotation.x += direction * (invertValue ? 1 - CurrentValue : CurrentValue * 360);
                newTargetRotation.x += direction * (invertValue ? 1 - TargetValue : TargetValue * 360);
            }
            if ((rotationAxis & ScaleAxisEnum.Y) != 0)
            {
                newCurrentRotation.y += direction * (invertValue ? 1 - CurrentValue : CurrentValue * 360);
                newTargetRotation.y += direction * (invertValue ? 1 - TargetValue : TargetValue * 360);
            }
            if ((rotationAxis & ScaleAxisEnum.Z) != 0)
            {
                newCurrentRotation.z += direction * (invertValue ? 1 - CurrentValue : CurrentValue * 360);
                newTargetRotation.z += direction * (invertValue ? 1 - TargetValue : TargetValue * 360);
            }

            if (echoTransform == null)
            {
                if (local)
                {
                    baseTransform.localEulerAngles = newCurrentRotation;
                }
                else
                {
                    baseTransform.eulerAngles = newCurrentRotation;
                }
            }
            else
            {
                if (Data.Direction > 0) //Heal
                {

                    if (local)
                    {
                        baseTransform.localEulerAngles = newCurrentRotation;
                        echoTransform.localEulerAngles = newTargetRotation;
                    }
                    else
                    {
                        baseTransform.eulerAngles = newCurrentRotation;
                        echoTransform.eulerAngles = newTargetRotation;
                    }
                    
                }
                else //Damage
                {

                    if (local)
                    {
                        baseTransform.eulerAngles = newTargetRotation;
                        echoTransform.eulerAngles = newCurrentRotation;
                    }
                    else
                    {
                        baseTransform.eulerAngles = newTargetRotation;
                        echoTransform.eulerAngles = newCurrentRotation;
                    }
                    
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
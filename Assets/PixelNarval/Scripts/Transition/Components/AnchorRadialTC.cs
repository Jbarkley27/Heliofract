using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PixelNarval.HPBars
{
    [AddComponentMenu("PixelNarval/Transition/AnchorRadialTC")]
    public class AnchorRadialTC : TransitionComponent, IcurrentValueChanger, ICommonDataChanger
    {

        protected enum AxisEnum
        {
            X = 1,
            Y = 2,
            Z = 4,
        };

        [Header("References")]
        [NoNull] [SerializeField] RectTransform movingObject;
        [NoNull] [SerializeField] RectTransform referenceObject;

        [Header("Configuration")]
        [SerializeField] private AxisEnum axis = AxisEnum.Z;
        [SerializeField] private Vector2 offset;
        [SerializeField] private float radius;
        [Range(0, 1)]
        [SerializeField] private float angleOffset;
        [SerializeField] private bool angleByValue;
        [SerializeField] private bool clockwise = true;       
        [SerializeField] private bool radiusProportionalToRectWidth = true;       


        Vector2 targetPosition;
        float direction;

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void LoadIcon()
        {
            IconsUtils.LoadIcon("AnchorRadialTC", "Button Icon");
        }
#endif

        private void OnValidate()
        {
            if (gameObject.activeInHierarchy)
            {
                Move();
            }
        }

        private void Move()
        {
            if (referenceObject == null || movingObject == null)
            {
                return;
            }
            
            direction = clockwise ? -1 : 1;
            float angle = angleOffset + (angleByValue ? CurrentValue : 0);
            angle = (angle * direction) * 360;
            float cos = Mathf.Cos(Mathf.Deg2Rad * angle);
            float sin = Mathf.Sin(Mathf.Deg2Rad * angle);

            Vector3 relativePosition = Vector3.one;
            switch (axis)
            {
                case AxisEnum.X:
                    relativePosition = new Vector3(0, -cos, sin);
                    break;
                case AxisEnum.Y:
                    relativePosition = new Vector3(sin, 0, -cos);
                    break;
                case AxisEnum.Z:
                    relativePosition = new Vector3(sin, -cos, 0);
                    break;
                default:
                    relativePosition = Vector3.zero;
                    break;
            }
            targetPosition = referenceObject.localPosition + (radius * (radiusProportionalToRectWidth ? movingObject.rect.width/2 + referenceObject.rect.width / 2 : 1) * direction * relativePosition);
             
            movingObject.localPosition = targetPosition;
        }

        public void currentValueChange(TransitionEventValue _)
        {
            Move();
        }

        public void CommonDataChange(TransitionData _)
        {
            Move();
        }
    } 
}

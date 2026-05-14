using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PixelNarval.HPBars
{
    [DefaultExecutionOrder(100)]
    [RequireComponent(typeof(TransitionData))]
    [AddComponentMenu("PixelNarval/Transition/AnchorToRectTC")]
    public class AnchorToRectTC : TransitionComponent, IcurrentValueChanger, ICommonDataChanger
    {
        [SerializeField] RectConfig movingObject;
        [SerializeField] RectConfig referenceObject;

        [Header("Configuration")]
        [SerializeField] private Vector2 offset;
        [SerializeField] private bool debug;

        Vector2 targetPosition;
        Vector2 selfBorderPosition;

        [System.Serializable]
        public class RectConfig
        {
            [Header("References")]
            [NoNull] public RectTransform reference;

            [Header("Configuration")]
            public Vector2 position;
            public bool proportionalToXValue;
            public bool proportionalToYValue;
            public bool invertedX;
            public bool invertedY;
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void LoadIcon()
        {
            IconsUtils.LoadIcon("AnchorToRectTC", "LayoutElement Icon");
        }

        private void Update()
        {
            if (gameObject.activeInHierarchy && !Application.isPlaying)
            {
                Move();

            }
        }
#endif

        private void OnValidate()
        {
            Move();
        }

        private void Move()
        {
            if (referenceObject?.reference == null || movingObject?.reference == null)
            {
                return;
            }
            
            targetPosition = UIAnchorUtils.RectBorderPoint(referenceObject, CurrentValue, offset);
            selfBorderPosition = UIAnchorUtils.RectBorderPoint(movingObject, CurrentValue, Vector3.zero);
            movingObject.reference.position = targetPosition - (selfBorderPosition - (Vector2)movingObject.reference.position);
        }

        
        private void OnDrawGizmos()
        {
            if (referenceObject == null || !debug)
            {
                return;
            }
            /*
            Matrix4x4 backup = Gizmos.matrix;
            Gizmos.matrix = referenceObject.localToWorldMatrix;
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(referenceObject.rect.min, new Vector2(referenceObject.rect.max.x, referenceObject.rect.min.y)); //top
            Gizmos.DrawLine(referenceObject.rect.min, new Vector2(referenceObject.rect.min.x, referenceObject.rect.max.y)); //left
            Gizmos.DrawLine(referenceObject.rect.max, new Vector2(referenceObject.rect.min.x, referenceObject.rect.max.y)); //bottom
            Gizmos.DrawLine(referenceObject.rect.max, new Vector2(referenceObject.rect.max.x, referenceObject.rect.min.y)); //right
            Gizmos.matrix = backup;
            */

            Gizmos.color = Color.green;
            Gizmos.DrawLine(Vector2.zero, targetPosition - (selfBorderPosition - (Vector2) movingObject.position));
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(Vector2.zero, selfBorderPosition);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(movingObject.position, selfBorderPosition);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Vector2.zero, targetPosition);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelNarval.HPBars
{
    public static class UIAnchorUtils
    {
        public static Vector2 RectBorderPoint(AnchorToRectTC.RectConfig targetRect, float value, Vector2 offset)
        {
            float xPosition = targetRect.proportionalToXValue ? value : (targetRect.position.x + 1) * 0.5f;
            if (targetRect.invertedX)
            {
                xPosition = 1 - xPosition;
            }

            float yPosition = targetRect.proportionalToYValue ? value : (targetRect.position.y + 1) * 0.5f;
            if (targetRect.invertedY)
            {
                yPosition = 1 - yPosition;
            }

            Rect referenceRect = targetRect.reference.rect;

            Vector2 targetPosition = new Vector2
            {
                x = targetPosition.x = Mathf.LerpUnclamped(referenceRect.xMin, referenceRect.xMax, xPosition),
                y = targetPosition.y = Mathf.LerpUnclamped(referenceRect.yMin, referenceRect.yMax, yPosition)
            };
            
            return targetRect.reference.TransformPoint(targetPosition) + (Vector3)offset;
        }
    } 
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PixelNarval.HPBars
{
    public class FloatToTextConverter : MonoBehaviour
    {
        [SerializeField] UnityEvent<string> convertTo;

        
        public void SetPercentageAsText(float value)
        {
            string convertedString = string.Format("{0}%", Mathf.FloorToInt(value * 100).ToString());
            convertTo?.Invoke(convertedString);
        }
    } 
}

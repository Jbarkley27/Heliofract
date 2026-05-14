using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PixelNarval.HPBars
{
    [AddComponentMenu("PixelNarval/Transition/TextContentTC")]
    public class TextContentTC : TransitionComponent, IcurrentValueChanger, ICommonDataChanger
    {
        [Header("References")]
        [NoNull] [SerializeField] protected TMP_Text text;

        [Header("Configuration")]
        
        [Info("Write any text and add any of the following patterns to display dinamic values:\n" +
            "{f}: the current value as a float.\n" +
            "{i}: the current value as an int based on the Max Number set on the BarManager component.\n" +
            "{m}: the Max Number as an int.\n" +
            "{a}: the adjusted current value as a float rounded to the nearest int equivalent.\n" +
            "{p}: the current value as an int representing a percentage(between 0 and 100).\n" +
            "{ap}: the current value as an int representing a percentage adjusted to the nearest int value." )]
        public string formatedText;

        public enum BarTextPositionEnum { FixedLocal, FollowFill }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void LoadIcon()
        {
            IconsUtils.LoadIcon("TextContentTC", "d_Text Icon");

        }        
#endif

        private void Start()
        {
            SetText();
        }

        protected void OnValidate()
        {
            SetText();
            if (formatedText != null &&
                Data.MaxValue <= 0 &&
                    (
                    formatedText.Contains("{i}") ||
                    formatedText.Contains("{m}") ||
                    formatedText.Contains("{a}") ||
                    formatedText.Contains("{ap}")
                    )
                )
            {
                Debug.LogError("Error: Max Value Must be > 0 for this patterns to work");
            }
        }
        protected void SetText()
        {
            if (text == null || Data == null)
            {
                return;
            }
            string formatedText = this.formatedText;

            formatedText = formatedText.Replace(@"{f}", Data.currentValue.FloatValue.ToString("0.00"));
            if (Data.MaxValue > 0)
            {
                formatedText = formatedText.Replace(@"{i}", Data.currentValue.IntValue.ToString());
                formatedText = formatedText.Replace(@"{m}", (Data.MaxValue).ToString());
                formatedText = formatedText.Replace(@"{a}", (Data.currentValue.AdjustedFloatValue).ToString());
                formatedText = formatedText.Replace(@"{ap}", Mathf.FloorToInt(Data.currentValue.AdjustedFloatValue * 100).ToString());
            }
            formatedText = formatedText.Replace(@"{p}", Mathf.FloorToInt(Data.currentValue.FloatValue * 100).ToString());

            if (text.text != formatedText)
            {
                text.SetText(formatedText);

            }
        }

        public void currentValueChange(TransitionEventValue _)
        {
            SetText();
        }

        public void CommonDataChange(TransitionData _)
        {
            SetText();
        }
    }
}
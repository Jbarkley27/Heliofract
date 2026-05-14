using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace PixelNarval.HPBars
{
    [AddComponentMenu("PixelNarval/Transition/ImageFillTC")]
    public class ImageFillTC : TransitionComponent, IcurrentValueChanger, ILastValueChanger, ItargetValueChanger
    {
        [Header("References")]
        [NoNull] [SerializeField] public Image fillImage;
        [SerializeField] public Image echoImage;

        [Header("Configuration")]
        [SerializeField] public bool invertValue;
        [SerializeField] public bool invertedEcho;

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void LoadIcon()
        {
            IconsUtils.LoadIcon("ImageFillTC", "sv_icon_name2");
        }
#endif
        private void UpdateValues()
        {
            if (fillImage == null)
            {
                return;
            }

            if (echoImage == null)
            {
                fillImage.fillAmount = invertValue ? 1 - CurrentValue : CurrentValue;
                
            }
            else
            {
                if (Data.Direction > 0) //Heal
                {
                    if (invertedEcho)
                    {
                        echoImage.fillAmount = invertValue ? 1 - TargetValue : TargetValue;
                    }
                    else
                    {
                        echoImage.fillAmount = invertValue ? 1 - TargetValue : TargetValue;    
                    }
                    fillImage.fillAmount = invertValue ? 1 - CurrentValue : CurrentValue;
                }
                else //Damage
                {
                    if (invertedEcho)
                    {
                        echoImage.fillAmount = invertValue ? 1 -  LastValue : LastValue;
                        fillImage.fillAmount = invertValue ? 1 - CurrentValue : CurrentValue;
                    }
                    else
                    {
                        echoImage.fillAmount = invertValue ? 1 - CurrentValue : CurrentValue;
                        fillImage.fillAmount = invertValue ? 1 - TargetValue : TargetValue;
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
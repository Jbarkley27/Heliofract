using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PixelNarval.HPBars
{
    
    [AddComponentMenu("PixelNarval/Transition/ImageColorTC")]
    public class ImageColorTC : TransitionComponent, IcurrentValueChanger, ILastValueChanger, ItargetValueChanger
    {
        public enum CurveModeEnum
        {
            currentValue,
            FillPercentage
        }

        public enum ColorModeEnum
        {
            Set,
            Add,
            Subtract,
            Multiply,
            AlphaMask
        }

        public enum ApplyOnDirectionEnum
        {
            Both,
            Gain,
            Loss
        }

        [Header("References")]
        [NoNull] [SerializeField] private Graphic fillImage;

        [Header("Configuration")]       
        public CurveModeEnum curveMode;        
        public ColorModeEnum colorMode;        
        public ApplyOnDirectionEnum applyOnDirectionEnum;
        public Gradient gradient;

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void LoadIcon()
        {
            IconsUtils.LoadIcon("ImageColorTC", "ColorPicker-HueRing");
        }
#endif

        private void SetValue ()
        {
            if (applyOnDirectionEnum == ApplyOnDirectionEnum.Gain && Data.Direction < 0 ||
                applyOnDirectionEnum == ApplyOnDirectionEnum.Loss && Data.Direction > 0)
            {
                return;
            }

            float floatValue;
            switch (this.curveMode)
            {
                case CurveModeEnum.currentValue:
                    floatValue = CurrentValue;
                    break;
                case CurveModeEnum.FillPercentage:
                    floatValue = Data.percentage;
                    if (!Application.isPlaying)
                    {
                        return;
                    }
                    break;
                default:
                    floatValue = 0;
                    break;
            }

            SetColor(floatValue);
        }

        private void SetColor(float value)
        {
            if (fillImage == null)
            {
                return;
            }

                Color newColor = this.gradient.Evaluate(value);
            switch (this.colorMode)
            {
                case ColorModeEnum.Set:
                    fillImage.color = newColor;
                    break;
                case ColorModeEnum.Add:
                    fillImage.color += newColor;
                    break;
                case ColorModeEnum.Subtract:
                    fillImage.color -= newColor;
                    break;
                case ColorModeEnum.Multiply:
                    fillImage.color *= newColor;
                    break;
                case ColorModeEnum.AlphaMask:
                    
                    fillImage.color = Color.Lerp(fillImage.color, newColor, newColor.a);
                    break;
                default:
                    break;
            }
        }

        public void currentValueChange(TransitionEventValue _)
        {
            SetValue();
        }

        public void LastValueChange(TransitionEventValue value)
        {
            SetValue();
        }

        public void targetValueChange(TransitionEventValue value)
        {
            SetValue();
        }
    }
}
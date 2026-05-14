using UnityEngine;

namespace PixelNarval.HPBars
{
    [System.Serializable]
    public class TransitionEventValue
    {
        private TransitionEventValueCommonData commonData;
        public PriorityEvent<TransitionEventValue> ValueChangedEvent;

        [SerializeField] private float floatValue;

        public TransitionEventValueCommonData CommonData { get => commonData; set => commonData = value; }
        public float FloatValue
        {
            get
            {
                return floatValue;
            }
            set
            {
                floatValue = Mathf.Clamp01(value);
                ValueChangedEvent?.Invoke(this);
            }
        }

        public float AdjustedFloatValue
        {
            get
            {
                return MathUtils.AdjustedRoundedFloat(FloatValue, commonData.maxValue, commonData.roundingType);
            }
            set
            {
                FloatValue = MathUtils.AdjustedRoundedFloat(value, commonData.maxValue, commonData.roundingType);
            }
        }
        public int IntValue
        {
            get
            {
                return MathUtils.AdjustFloatToInt(FloatValue, commonData.maxValue, commonData.roundingType);
            }
            set
            {
                FloatValue = MathUtils.AdjustIntToFloat(value, commonData.maxValue);
            }
        }


        public TransitionEventValue(TransitionEventValueCommonData commonData)
        {
            this.commonData = commonData;
            ValueChangedEvent = new PriorityEvent<TransitionEventValue>();
        }
    }
}

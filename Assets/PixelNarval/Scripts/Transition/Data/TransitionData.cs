using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PixelNarval.HPBars
{

    [DisallowMultipleComponent]
    [System.Serializable]
    [AddComponentMenu("PixelNarval/Transition/TransitionData")]
    public class TransitionData : MonoBehaviour, ITransitionComponentAdded
    {
        [SerializeField] private TransitionEventValueCommonData eventValueCommon;

        public TransitionEventValue lastValue;
        public TransitionEventValue currentValue;
        public TransitionEventValue targetValue;

        public PriorityEvent<TransitionEventValue> TransitionStartEvent = new PriorityEvent<TransitionEventValue>();
        public PriorityEvent<TransitionEventValue> TransitionEndEvent = new PriorityEvent<TransitionEventValue>();
        public PriorityEvent<TransitionData> CommonDataChangedEvent = new PriorityEvent<TransitionData>();

        private System.Action<TransitionEventValue> TemporalTransitionEndEvent;

        public float percentage;
        public float timer;

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void LoadIcon()
        {
            IconsUtils.LoadIcon("TransitionData", "Horizontal_PN_Logo", true);
        }
#endif

        public void OnTransitionComponentAdded(TransitionComponent bc)
        {
            bc.Data = this;
        }

        public TransitionData()
        {
            if (eventValueCommon == null)
            {
                eventValueCommon = new TransitionEventValueCommonData();
            }
            lastValue = new TransitionEventValue(eventValueCommon);
            currentValue = new TransitionEventValue(eventValueCommon);
            targetValue = new TransitionEventValue(eventValueCommon);

            currentValue.ValueChangedEvent.Subscribe(OnCurrentValueChange, 100);
            targetValue.ValueChangedEvent.Subscribe(OnTargetValueChange, 100);

            TransitionEndEvent.Subscribe(OnTargetReached, 0);
        }

        private void OnTargetReached (TransitionEventValue transitionEventValue)
        {
            TemporalTransitionEndEvent?.Invoke(transitionEventValue);
            TemporalTransitionEndEvent = null;
        }


        private void OnCurrentValueChange (TransitionEventValue _)
        {
            if (currentValue.FloatValue == targetValue.FloatValue)
            {
                TransitionEndEvent?.Invoke(currentValue);
            }
        }

        private void OnTargetValueChange(TransitionEventValue _)
        {
            // May be useful later
        }

        public int MaxValue
        {
            get => eventValueCommon.maxValue;

            set
            {
                eventValueCommon.maxValue = Mathf.Max(0, value);
                CommonDataChangedEvent?.Invoke(this);
            }
        }

        public void SetMaxNumber(int value)
        {
            MaxValue = value;
        }

        public void SetMaxNumberOnly(int value)
        {
            int currentIntValue = currentValue.IntValue;
            MaxValue = value;
            lastValue.IntValue = currentIntValue;       
            currentValue.IntValue = currentIntValue;       
            targetValue.IntValue = currentIntValue;       
        }

        public MathUtils.RoundingType RoundingType
        {
            get => eventValueCommon.roundingType;

            set
            {
                eventValueCommon.roundingType = value;
                CommonDataChangedEvent?.Invoke(this);
            }
        }


        public bool SnapToInt
        {
            get => eventValueCommon.snapToInt;

            set
            {
                eventValueCommon.snapToInt = value;
                CommonDataChangedEvent?.Invoke(this);
            }
        }

        public float Direction
        {
            get => Mathf.Sign(targetValue.FloatValue - lastValue.FloatValue);
        }

        public void ForceCurrentValue(float value)
        {
            lastValue.FloatValue = value;
            targetValue.FloatValue = value;
            currentValue.FloatValue = value;
        }

        public void StartTransitionTo (float value, System.Action<TransitionEventValue> onTransitionEnd = null)
        {
            TemporalTransitionEndEvent = onTransitionEnd;
            lastValue.FloatValue = currentValue.FloatValue;
            targetValue.FloatValue = value;
            TransitionStartEvent?.Invoke(targetValue);
        }

        public void StartTransitionTo(float fromValue, float value, System.Action<TransitionEventValue> onTransitionEnd = null)
        {
            TemporalTransitionEndEvent = onTransitionEnd;
            lastValue.FloatValue = fromValue;
            targetValue.FloatValue = value;
            currentValue.FloatValue = fromValue;
            TransitionStartEvent?.Invoke(targetValue);
        }

        public void StartTransitionToInt(int value, System.Action<TransitionEventValue> onTransitionEnd = null)
        {
            TemporalTransitionEndEvent = onTransitionEnd;
            lastValue.IntValue = currentValue.IntValue;
            targetValue.IntValue = value;
            TransitionStartEvent?.Invoke(targetValue);
        }

        public void StartTransitionToInt(int fromValue, int value, System.Action<TransitionEventValue> onTransitionEnd = null)
        {
            TemporalTransitionEndEvent = onTransitionEnd;
            lastValue.IntValue = fromValue;
            targetValue.IntValue = value;
            currentValue.FloatValue = fromValue;
            TransitionStartEvent?.Invoke(targetValue);
        }

        public void AddIntValue(int value)
        {
            lastValue.FloatValue = currentValue.FloatValue;
            targetValue.IntValue += value;
        }
    } 
}

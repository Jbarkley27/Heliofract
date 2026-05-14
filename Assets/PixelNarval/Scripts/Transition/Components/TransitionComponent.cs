using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PixelNarval.HPBars
{
    [ExecuteInEditMode]
    public class TransitionComponent : MonoBehaviour
    {
        private TransitionData data;
        public TransitionData Data
        {
            get
            {
                if (data == null)
                {
                    data = GetComponentInParent<TransitionData>(true);
                    if (data != null)
                    {
                        SubscribeView();
                    }
                    else
                    {
                        data = gameObject.AddComponent<TransitionData>();
                    }
                }
                return data;
            }
            set
            {
                UnsubscribeView();
                data = value;
                SubscribeView();
            }
        }

        public enum UpdateValueTypeEnum
        {
            NormalValue,
            DiscreteValue            
        }

        public UpdateValueTypeEnum updateValueType;
        public float CurrentValue
        {
            get
            {
                switch (updateValueType)
                {
                    case UpdateValueTypeEnum.NormalValue:
                        return Data.currentValue.FloatValue;
                    case UpdateValueTypeEnum.DiscreteValue:
                        return Data.currentValue.AdjustedFloatValue;
                    default:
                        return Data.currentValue.FloatValue;
                }
            }
        }

        public float TargetValue
        {
            get
            {
                switch (updateValueType)
                {
                    case UpdateValueTypeEnum.NormalValue:
                        return Data.targetValue.FloatValue;
                    case UpdateValueTypeEnum.DiscreteValue:
                        return Data.targetValue.AdjustedFloatValue;
                    default:
                        return Data.targetValue.FloatValue;
                }
            }
        }

        public float LastValue
        {
            get
            {
                switch (updateValueType)
                {
                    case UpdateValueTypeEnum.NormalValue:
                        return Data.lastValue.FloatValue;
                    case UpdateValueTypeEnum.DiscreteValue:
                        return Data.lastValue.AdjustedFloatValue;
                    default:
                        return Data.lastValue.FloatValue;
                }
            }
        }

        [SerializeField][HideInInspector] private int order = 0;

        public void ChangeOrder (int value)
        {
            UnsubscribeView();
            order = value;
            SubscribeView();
        }


        private void Start()
        {
            UnsubscribeView();
            SubscribeView();
        }

        private void OnEnable()
        {
            UnsubscribeView();
            SubscribeView();
        }

        private void OnDisable()
        {
            UnsubscribeView();

        }
        private void OnDestroy()
        {
            UnsubscribeView();
        }

        public virtual void SubscribeView()
        {
            if (Data == null)
            {
                return;
            }
            if (this is ILastValueChanger lastValueChanger)
            {
                Data.lastValue.ValueChangedEvent.Subscribe(lastValueChanger.LastValueChange, order);
            }
            if (this is IcurrentValueChanger currentValueChanger)
            {
                Data.currentValue.ValueChangedEvent.Subscribe(currentValueChanger.currentValueChange, order);
                currentValueChanger.currentValueChange(Data.currentValue); // Set current state
            }
            if (this is ItargetValueChanger targetValueChanger)
            {
                Data.targetValue.ValueChangedEvent.Subscribe(targetValueChanger.targetValueChange, order);
            }
            if (this is ItargetValueReacher targetValueReacher)
            {
                Data.TransitionEndEvent.Subscribe(targetValueReacher.targetValueReach, order);
            }
            if (this is ICommonDataChanger commonDataChanger)
            {
                Data.CommonDataChangedEvent.Subscribe(commonDataChanger.CommonDataChange, order);
            }
        }

        public virtual void UnsubscribeView()
        {
            if (Data == null)
            {
                return;
            }
            if (this is ILastValueChanger lastValueReacher)
            {
                Data.lastValue.ValueChangedEvent.Unsubscribe(lastValueReacher.LastValueChange, order);
            }
            if (this is IcurrentValueChanger currentValueChanger)
            {
                Data.currentValue.ValueChangedEvent.Unsubscribe(currentValueChanger.currentValueChange, order);
            }
            if (this is ItargetValueChanger targetValueChanger)
            {
                Data.targetValue.ValueChangedEvent.Unsubscribe(targetValueChanger.targetValueChange, order);
            }
            if (this is ItargetValueReacher targetValueReacher)
            {
                Data.TransitionEndEvent.Unsubscribe(targetValueReacher.targetValueReach, order);
            }
            if (this is ICommonDataChanger commonDataChanger)
            {
                Data.CommonDataChangedEvent.Unsubscribe(commonDataChanger.CommonDataChange, order);
            }
        }
    }
}
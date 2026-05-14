using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PixelNarval.HPBars
{
    [AddComponentMenu("PixelNarval/Transition/ComposerTC")]
    public class ComposerTC : TransitionComponent, IcurrentValueChanger, ItargetValueChanger, ILastValueChanger
    {
        [Header("References")]
        [NoNull] [SerializeField] protected List<TransitionData> childTransitions;
        [ReadOnly] [SerializeField] protected List<int> partInTransition;

        [Header("Configuration")]
        [SerializeField] protected bool equal;
        [ReadOnly] [SerializeField] private int totalSteps;


        //[Header("Configuration")]
        //[Min(1)][SerializeField] protected int stepsPerTransition = 1;
        //[SerializeField] protected Vector3 offset;


#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void LoadIcon()
        {
            IconsUtils.LoadIcon("ComposerTC", "GridLayoutGroup Icon");
        }
#endif

        //private void SetStepsPerTransition(int stepsPerTransition)
        //{
        //    if (gameObject.activeInHierarchy)
        //    {
        //        foreach (TransitionData component in childTransitions)
        //        {
        //            if (component != null)
        //            {
        //                component.MaxValue = stepsPerTransition;
        //            }
        //        }

        //        Data.MaxValue = stepsPerTransition * childTransitions.Count;
        //        UpdateValues();
        //    }
        //}

        private void OnValidate()
        {
            UpdateTotalSteps();
        }

        public void UpdateValues()
        {
            if (childTransitions == null || childTransitions.Count <= 0)
            {
                return;
            }

            // Get target transition
            int transitionIndex = UpdateTargetTransition(CurrentValue);

            // Get the value for the target transition            
            float transitionValue = UpdateTargetValue(CurrentValue, transitionIndex);

            //Apply values
            for (int i = 0; i < transitionIndex; i++)
            {
                childTransitions[i].currentValue.FloatValue = 1f;
            }
            if (transitionIndex < childTransitions.Count)
            {
                childTransitions[transitionIndex].currentValue.FloatValue = transitionValue;
            }
            for (int i = transitionIndex + 1; i < childTransitions.Count; i++)
            {
                childTransitions[i].currentValue.FloatValue = 0f;
            }
        }          
        
        private void SetNewTransitionValues()
        {
            if (childTransitions == null || childTransitions.Count <= 0)
            {
                return;
            }

            // Get target transition
            int transitionIndex = UpdateTargetTransition(TargetValue);

            // Get the value for the target transition            
            float transitionValue = UpdateTargetValue(TargetValue, transitionIndex);

            //Apply values
            for (int i = 0; i < transitionIndex; i++)
            {
                childTransitions[i].lastValue.FloatValue = childTransitions[i].currentValue.FloatValue;
                childTransitions[i].targetValue.FloatValue = 1f;
            }
            if (transitionIndex < childTransitions.Count)
            {
                childTransitions[transitionIndex].lastValue.FloatValue = childTransitions[transitionIndex].currentValue.FloatValue;
                childTransitions[transitionIndex].targetValue.FloatValue = transitionValue;
                //Debug.Log(targetTransitionValue);
            }
            for (int i = transitionIndex + 1; i < childTransitions.Count; i++)
            {
                childTransitions[i].lastValue.FloatValue = childTransitions[i].currentValue.FloatValue;
                childTransitions[i].targetValue.FloatValue = 0f;
            }

        }

        private void UpdateTotalSteps()
        {
            if (!equal)
            {
                totalSteps = 0;
                partInTransition = new List<int>();
                foreach (TransitionData component in childTransitions)
                {
                    totalSteps += Mathf.Max(component.MaxValue, 1);
                    partInTransition.Add(totalSteps);
                }
            }
        }

        private float UpdateTargetValue(float value, int targetTransition)
        {
            if (equal)
            {
                return (value - (1f / childTransitions.Count) * targetTransition) * childTransitions.Count;
            }
            else
            {
                int minPartValue = (targetTransition == 0) ? 0 : partInTransition[targetTransition - 1];
                int maxPartValue = partInTransition[targetTransition];
                int adjustedValue = MathUtils.AdjustFloatToInt(value, totalSteps, Data.RoundingType);
                return (float)(adjustedValue - minPartValue) / (maxPartValue - minPartValue);
            }
        }

        private int UpdateTargetTransition(float value)
        {
            if (equal)
            {
                return Mathf.Min(MathUtils.AdjustFloatToInt(value, childTransitions.Count, Data.RoundingType), childTransitions.Count);
            }
            else
            {
                int targetIntValue = MathUtils.AdjustFloatToInt(value, totalSteps, Data.RoundingType);
                for (int i = 0; i < partInTransition.Count; i++)
                {
                    if (targetIntValue <= partInTransition[i])
                    {
                        return i;
                    }
                }
            }
            return 0;
        }
        public void currentValueChange(TransitionEventValue _)
        {
            UpdateValues();
        }
        public void targetValueChange(TransitionEventValue _)
        {

            SetNewTransitionValues();
        }
        public void LastValueChange(TransitionEventValue _)
        {
            SetNewTransitionValues();
        }
    }
}
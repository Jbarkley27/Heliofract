using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PixelNarval.HPBars
{
    [RequireComponent(typeof(TransitionData))]
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [AddComponentMenu("PixelNarval/Transition/TransitionUpdateManager")]
    public class TransitionUpdateManager : MonoBehaviour
    {

        [SerializeField] private TransitionData transitionData;

        [Header("Transition Config")]
        [SerializeField] private bool stopped;
        [SerializeField] private bool simulateOnEditor;
        [SerializeField] protected List<TransitionData> childTransitions;
        public enum CompositeTransitionTypeEnum { simultaneous, distributed, sequence };
        public CompositeTransitionTypeEnum compositeTransitionTypeEnum;
        public enum CompositeTransitionWaitForEnum { first, last };
        public CompositeTransitionWaitForEnum compositeTransitionWaitFor;


        [SerializeField] private TransitionUpdateManagerConfig transitionConfig;
        protected TransitionUpdateController controller;

        public TransitionData TransitionData { 
            get
            {
                if (transitionData == null)
                {
                    transitionData = GetComponent<TransitionData>();
                }
                return transitionData;
            }
            set => transitionData = value; 
        }

        public TransitionUpdateManagerConfig TransitionConfig {
            get
            {
                if (transitionConfig == null)
                {
                    transitionConfig = new TransitionUpdateManagerConfig();
                }
                return transitionConfig;
            }
            set => transitionConfig = value; 
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void LoadIcon()
        {
            IconsUtils.LoadIcon("TransitionManager", "CollabMoved Icon");

        }
#endif

        private void OnValidate()
        {
            Start();
        }

        private void Start()
        {
            TransitionData.targetValue.ValueChangedEvent.Unsubscribe(StartTransition);
            TransitionData.targetValue.ValueChangedEvent.Subscribe(StartTransition);


            if (transitionConfig == null)
            {
                return;
            }
            if (transitionConfig.advancedConfig)
            {
                switch (transitionConfig.fillType)
                {
                    case TransitionUpdateManagerConfig.fillTypeEnum.perTime:
                        switch (transitionConfig.timeFllType)
                        {
                            case TransitionUpdateManagerConfig.timeFillTypeEnum.speedOverTime:
                                controller = new SpeedOverTimeTransitionUpdateController(TransitionData, transitionConfig);
                                break;
                            case TransitionUpdateManagerConfig.timeFillTypeEnum.fillOverTime:
                                controller = new FillOverTimeTransitionUpdateController(TransitionData, transitionConfig);
                                break;
                            default:
                                break;
                        }
                        break;
                    case TransitionUpdateManagerConfig.fillTypeEnum.perFrame:
                        controller = new PerFrameTransitionUpdateController(TransitionData, transitionConfig);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                controller = new SimpleFillOverTimeTransitionUpdateController(TransitionData, transitionConfig);
            }
            StartTransition(null);
        }

        private void StartTransition (TransitionEventValue _)
        {
            controller.StartTransition();
        }

        private void Update()
        {
            if (!simulateOnEditor && !Application.isPlaying)
            {
                return;
            }

            if (
                controller != null &&
                !stopped
                )
            {
                controller.CheckUpdateAndStep();
            }
        }        
    }
}
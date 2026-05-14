using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PixelNarval.TextureGenerators;

namespace PixelNarval.HPBars
{
    public abstract class TextureGeneratorTC : TransitionComponent, ICommonDataChanger
    {
        [Header("References")]
        [SerializeField] [NoNull] protected List<Image> targetImages = new List<Image>();

        [SerializeField] protected Vector2Int resolution = new Vector2Int(512, 512);
        [SerializeField] protected Vector4 border = new Vector4();
        [SerializeField] protected FilterMode filterMode = FilterMode.Bilinear;
        [SerializeField] protected Color color = Color.white;
        [SerializeField] protected Gradient colorGradient = new Gradient() { colorKeys = new GradientColorKey[] { new GradientColorKey(Color.white, 0) } };
        
        [SerializeField] protected ColorGradientTypeEnum colorGradientType;

        [SerializeField] protected bool useMaxValue;
        [Min(1)] [SerializeField] protected int segmentNumber;
        [SerializeField] protected float segmentsSeparation = 5;
        

        [SerializeField] [HideInInspector] Texture2D texture;

        protected void OnValidate()
        {
            resolution.x = Mathf.Max(resolution.x, 1);
            resolution.y = Mathf.Max(resolution.y, 1);
            if (useMaxValue)
            {
                segmentNumber = Mathf.Max(1, Data.MaxValue);
            }
            RegenerateImages();
        }

        protected void RegenerateImages()
        {
            if (useMaxValue)
            {
                segmentNumber = Mathf.Max(1, Data.MaxValue);
            }
            texture = Generate();
            SetSprites();
        }

        protected abstract Texture2D Generate();

        protected void SetSprites()
        {
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect, border: border);

            foreach (var item in targetImages)
            {
                if (item != null)
                {
                    item.sprite = sprite;

                }
            }
        }

        public virtual void CommonDataChange(TransitionData value)
        {
            RegenerateImages();
        }
    }
}
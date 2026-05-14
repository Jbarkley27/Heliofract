using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelNarval.TextureGenerators
{
    public enum ColorGradientTypeEnum { Horizontal, Vertical, Radial, Angular }
    public static class SegmentedTextureGenerator
    {

        public static Texture2D GenerateRectangularTexture(Gradient gradient, ColorGradientTypeEnum gradientType, Vector2Int resolution, FilterMode filterMode=FilterMode.Bilinear, int segmentNumber=0, float segmentsSeparation= 0, bool horizontalSections = true)
        {
            Color[] pixelArray = new Color[resolution.x * resolution.y];
            float axisDistance = 0;
            if (segmentNumber > 1)
            {
                axisDistance = (horizontalSections ? resolution.y : resolution.x) / (float)segmentNumber;
            }
            float partsSeparationHalf = segmentsSeparation * 0.5f;

            Vector2 centerCoords = new Vector2();
            if (resolution.x % 2 == 1)
            {
                centerCoords.x = ((resolution.x - 1)  * 0.5f);
            }
            else
            {
                centerCoords.x = ((resolution.x - 1) * 0.5f);
            }
            if (resolution.y % 2 == 1)
            {
                centerCoords.y = ((resolution.y - 1) * 0.5f);
            }
            else
            {
                centerCoords.y = ((resolution.y - 1) * 0.5f);
            }

            float minX = 0;
            float maxX = resolution.x;
            float minY = 0;
            float maxY = resolution.y;

            int positionInArray;
            float pixelAngle = 0;
            float distance;

            for (int i = 0; i < resolution.x; i++)
            {
                for (int j = 0; j < resolution.y; j++)
                {
                    positionInArray = i + (j * resolution.x);
                    distance = Vector2.Distance(new Vector2(i, j), centerCoords);
                    pixelAngle = ((Mathf.Atan2(j - centerCoords.y, i - centerCoords.x) * Mathf.Rad2Deg) + 630) % 360;

                    Color color = gradient.Evaluate(0);

                    switch (gradientType)
                    {
                        case ColorGradientTypeEnum.Horizontal:
                            color = gradient.Evaluate(Mathf.InverseLerp(minX, maxX, i));
                            break;
                        case ColorGradientTypeEnum.Vertical:
                            color = gradient.Evaluate(Mathf.InverseLerp(minY, maxY, j));
                            break;
                        case ColorGradientTypeEnum.Radial:
                            color = gradient.Evaluate(Mathf.InverseLerp(0, Mathf.Max(maxX, maxY) * 0.5f, distance));
                            break;
                        case ColorGradientTypeEnum.Angular:
                            color = gradient.Evaluate(Mathf.InverseLerp(360, 0, pixelAngle));
                            break;
                        default:
                            break;
                    }

                    Color transparentColor = color;
                    transparentColor.a = 0;

                    if (segmentNumber > 1)
                    {
                        int linearDistance = horizontalSections ? j : i;

                        float distanceMod = Mathf.Abs((linearDistance + axisDistance) % axisDistance);

                        if ((distanceMod < partsSeparationHalf || distanceMod > axisDistance - partsSeparationHalf))
                        {
                            pixelArray[positionInArray] = transparentColor;
                            continue;
                        }
                    }
                    pixelArray[positionInArray] = color;
                }
            }
            //return pixelArray;

            Texture2D texture = new Texture2D(resolution.x, resolution.y);
            texture.SetPixels(pixelArray);
            texture.filterMode = filterMode;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.Apply();

            return texture;
        }

        public static Texture2D GenerateCircularTexture(Gradient gradient, ColorGradientTypeEnum gradientType, Vector2Int resolution, Vector2 center, float externalRadius = 1, float internalRadius = 0.8f, float angleOffset=0, FilterMode filterMode = FilterMode.Bilinear, int segmentNumber = 0, float originSegmentsSeparation = 0,  float borderSegmentsSeparation = 0)
        {
            
            Color[] pixelArray = new Color[resolution.x * resolution.y];
            Vector2 currentPixelCoords = new Vector2();
            Vector2 centerCoords = new Vector2();
            if (resolution.x % 2 == 1)
            {
                centerCoords.x = ((resolution.x - 1) * center.x);
            }
            else
            {
                centerCoords.x = ((resolution.x - 1) * center.x);
            }
            if (resolution.y % 2 == 1)
            {
                centerCoords.y = ((resolution.y - 1) * center.y);
            }
            else
            {
                centerCoords.y = ((resolution.y - 1) * center.y);
            }
            float maxDimention = Mathf.Max(resolution.x * 0.5f, resolution.y * 0.5f);
            if (internalRadius > externalRadius)
            {
                internalRadius = externalRadius;
            }

            float iRadius = (internalRadius * maxDimention);
            float eRadius = (externalRadius * maxDimention);
            float pixelAngle = 0;
            float skipAngle = 0;
            if (segmentNumber > 1)
            {
                skipAngle = 360.0f / segmentNumber;
            }            

            float minX = Mathf.Max(0, centerCoords.x - eRadius);
            float maxX = Mathf.Min(resolution.x, centerCoords.x + eRadius);
            float minY = Mathf.Max(0, centerCoords.y - eRadius);
            float maxY = Mathf.Min(resolution.y, centerCoords.y + eRadius);

            for (int i = 0; i < resolution.x; i++)
            {
                currentPixelCoords.x = i;
                for (int j = 0; j < resolution.y; j++)
                {
                    currentPixelCoords.y = j;
                    float distance = Vector2.Distance(currentPixelCoords, centerCoords);



                    //float partsSeparationHalf = Mathf.Lerp(originSegmentsSeparation, borderSegmentsSeparation, Mathf.InverseLerp(iRadius, eRadius, distance))  * 0.5f;
                    float partsSeparationHalf = borderSegmentsSeparation* 0.5f;

                    pixelAngle = ((Mathf.Atan2(j - centerCoords.y, i - centerCoords.x) * Mathf.Rad2Deg) + angleOffset + 630) % 360;
                    float angleMod = Mathf.Min(Mathf.Abs(pixelAngle % skipAngle), Mathf.Abs((pixelAngle % skipAngle) - skipAngle));


                    if (distance <= (iRadius + eRadius) / 2 && distance + 2 > (iRadius + eRadius) / 2)
                    {
                        float e = Mathf.Lerp(originSegmentsSeparation, borderSegmentsSeparation, Mathf.InverseLerp(iRadius, eRadius, distance)) * 0.5f;
                    }
                    float a = Mathf.InverseLerp(iRadius, eRadius, distance);

                    Color color = gradient.Evaluate(0);

                    switch (gradientType)
                    {
                        case ColorGradientTypeEnum.Horizontal:
                            color = gradient.Evaluate(Mathf.InverseLerp(minX, maxX, i));                            
                            break;
                        case ColorGradientTypeEnum.Vertical:
                            color = gradient.Evaluate(Mathf.InverseLerp(minY, maxY, j));
                            break;
                        case ColorGradientTypeEnum.Radial:
                            color = gradient.Evaluate(a);
                            break;
                        case ColorGradientTypeEnum.Angular:
                            color = gradient.Evaluate(Mathf.InverseLerp(360, 0, pixelAngle));
                            break;
                        default:
                            break;
                    }

                    Color transparentColor = color;
                    transparentColor.a = 0;

                    if (distance > eRadius || distance < iRadius)
                    {
                        pixelArray[i + (j * resolution.x)] = transparentColor;
                        continue;
                    }

                    

                    float b = Mathf.Lerp(originSegmentsSeparation, borderSegmentsSeparation, Mathf.InverseLerp(iRadius, eRadius, distance * Mathf.Cos(Mathf.Deg2Rad * angleMod)));

                    //float angleDist =  (angleMod * distance);
                    float angleDist =  (Mathf.Sin(angleMod * Mathf.Deg2Rad) * distance);
                    if (segmentNumber > 1)
                    {

                        //if ((angleMod < partsSeparationHalf || angleMod > skipAngle - partsSeparationHalf))
                        if (angleDist < b / 2)
                        {
                            pixelArray[i + (j * resolution.x)] = transparentColor;
                            continue;
                        }
                    }

                    pixelArray[i + (j * resolution.x)] = color;
                    //pixelArray[i + (j * resolution.x)] = Color.Lerp(color, Color.red, Mathf.InverseLerp(originSegmentsSeparation, borderSegmentsSeparation, b));
                    //Smooth border
                    if (filterMode != FilterMode.Point && Mathf.Abs(eRadius - distance) < 1)
                    {
                        pixelArray[i + (j * resolution.x)].a = Mathf.Lerp(0.2f, 1, Mathf.Abs(eRadius - distance));
                        continue;
                    }
                    else if (filterMode != FilterMode.Point && Mathf.Abs(distance - iRadius) < 1 && iRadius > 0)
                    {
                        pixelArray[i + (j * resolution.x)].a = Mathf.Lerp(0.2f, 1, Mathf.Abs(distance - iRadius));
                    }
                    else
                    {
                        //pixelArray[i + (j * resolution.x)] = new Color(Mathf.InverseLerp(0, skipAngle / 2, angleDist), 1, 1, 1);

                    }
                }
            }
            //return pixelArray;

            Texture2D texture = new Texture2D(resolution.x, resolution.y);
            texture.SetPixels(pixelArray);
            texture.filterMode = filterMode;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.Apply();

            return texture;
        }

    }

    
}
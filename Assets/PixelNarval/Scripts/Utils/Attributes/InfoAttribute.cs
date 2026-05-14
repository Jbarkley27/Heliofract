using UnityEngine;


namespace PixelNarval.HPBars
{
    public class InfoAttribute : PropertyAttribute
    {
        public string message;
        public InfoAttribute (string message)
        {
            this.message = message;
        }
    }
}
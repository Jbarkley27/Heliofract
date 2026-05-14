using UnityEngine.EventSystems;

namespace PixelNarval.HPBars
{
    public interface ICommonDataChanger : IEventSystemHandler
    {
        public void CommonDataChange(TransitionData value);
    }
}
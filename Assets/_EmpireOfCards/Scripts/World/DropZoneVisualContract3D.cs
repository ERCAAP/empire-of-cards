using EmpireOfCards.Data;

namespace EmpireOfCards.World
{
    public interface IDropZoneVisual3D
    {
        bool CanAccept(CardData card);
        void SetHighlight(bool on, bool valid);
        void SetPulse(bool on);
        void ShowPreview(string text, bool valid);
        void ClearPreview();
    }
}

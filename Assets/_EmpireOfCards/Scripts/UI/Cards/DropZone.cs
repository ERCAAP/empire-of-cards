namespace EmpireOfCards.UI.Cards
{
    /// <summary>
    /// Type of drop zone on the board.
    /// Shared by both the deprecated 2D DropZone and the active SlotZone3D.
    /// </summary>
    public enum DropZoneType
    {
        BusinessSlot,
        EmployeeSlot,
        UpgradeSlot,
        ActionZone,
        SellZone,
        BurnZone
    }

    // The DropZone MonoBehaviour class has been deprecated. See SlotZone3D.cs instead.
}

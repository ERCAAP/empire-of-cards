namespace EmpireOfCards.UI.Cards
{
    /// <summary>
    /// Type of drop zone on the board.
    /// Shared by both the deprecated 2D DropZone and the active SlotZone3D.
    /// </summary>
    public enum DropZoneType
    {
        // === Legacy (kept for backward compatibility) ===
        BusinessSlot,
        EmployeeSlot,
        UpgradeSlot,
        ActionZone,
        SellZone,
        BurnZone,

        // === Slot System v2 (GDD v3.0 Section 4) ===
        OperationSlot,      // Physical infrastructure (table, kitchen, server)
        StaffSlot,          // Employees (cook, barista, developer)
        MarketingSlot,      // Ad campaigns (flyer, influencer, google ads)
        SupplierSlot,       // Supply deals (butcher, firebase, organic)
        TempEffectSlot      // Temporary events/crises
    }

    // The DropZone MonoBehaviour class has been deprecated. See SlotZone3D.cs instead.
}

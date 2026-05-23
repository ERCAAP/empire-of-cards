using EmpireOfCards.Core;
using EmpireOfCards.Presentation;
using EmpireOfCards.UI.Cards;
using EmpireOfCards.UI.Clarity;
using EmpireOfCards.World;

namespace EmpireOfCards.Bootstrap
{
    public static class InteractionWiringService
    {
        public static void Wire(InputManager3D inputManager3D, ManagerBundle managers, Board3D board3D)
        {
            inputManager3D.OnCardHoverEnter += card =>
            {
                managers.uiManager?.ShowCardClarity(card != null ? card.CardData : null);
            };

            inputManager3D.OnCardHoverExit += _ =>
            {
                managers.uiManager?.HideClarity();
            };

            inputManager3D.OnDragSlotHoverChanged += (card, slot, valid) =>
            {
                if (managers.uiManager == null)
                    return;

                if (card == null || slot == null)
                {
                    managers.uiManager.HideClarity();
                }
                else
                {
                    managers.uiManager.ShowSlotClarity(card.CardData, slot, valid);
                    slot.ShowPreview(GameClarityFormatter.BuildProjectedDeltaLine(card.CardData), valid);
                }

                foreach (var zone in board3D.AllSlots)
                {
                    if (zone != null && zone != slot)
                        zone.ClearPreview();
                }
            };

            inputManager3D.OnCardDropped += (card, slot) =>
            {
                var gm = managers.gameManager;
                if (gm == null)
                    return;

                bool success = false;
                bool requiresPlayCost = slot.ZoneType != DropZoneType.SellZone && slot.ZoneType != DropZoneType.BurnZone;
                bool playCostPaid = false;

                if (requiresPlayCost && card != null && card.CardData != null && card.CardData.playCost > 0)
                {
                    playCostPaid = gm.SpendMoney(card.CardData.playCost);
                    if (!playCostPaid)
                    {
                        card.ReturnToHand();
                        return;
                    }
                }

                switch (slot.ZoneType)
                {
                    case DropZoneType.OperationSlot:
                        success = managers.boardManager.PlaceCardInSlot(card.CardData, SlotType.Operation, slot.SlotIndex);
                        break;

                    case DropZoneType.StaffSlot:
                        success = managers.boardManager.PlaceCardInSlot(card.CardData, SlotType.Staff, slot.SlotIndex, slot.ParentBusinessIndex);
                        break;

                    case DropZoneType.MarketingSlot:
                        success = managers.boardManager.PlaceCardInSlot(card.CardData, SlotType.Marketing, slot.SlotIndex);
                        break;

                    case DropZoneType.SupplierSlot:
                        success = managers.boardManager.PlaceCardInSlot(card.CardData, SlotType.Supplier, slot.SlotIndex);
                        break;

                    case DropZoneType.TempEffectSlot:
                        success = managers.boardManager.PlaceCardInSlot(card.CardData, SlotType.TempEffect, slot.SlotIndex);
                        break;

                    case DropZoneType.BusinessSlot:
                        success = managers.boardManager.PlaceBusiness(card.CardData, slot.SlotIndex);
                        break;

                    case DropZoneType.EmployeeSlot:
                        success = managers.boardManager.PlaceEmployee(card.CardData, slot.ParentBusinessIndex);
                        break;

                    case DropZoneType.UpgradeSlot:
                        success = managers.boardManager.PlaceUpgrade(card.CardData, -1);
                        if (success)
                        {
                            switch (card.CardData.upgradeEffectType)
                            {
                                case UpgradeEffectType.ExtraAction:
                                    gm.AddExtraAction(card.CardData.extraActions);
                                    break;
                                case UpgradeEffectType.ReduceFBIRisk:
                                    break;
                            }
                        }
                        break;

                    case DropZoneType.ActionZone:
                        EventBus.ActionExecuted(card.CardData);
                        success = managers.boardManager.PlaceUpgrade(card.CardData, -1);
                        break;

                    case DropZoneType.SellZone:
                        int price = managers.economyManager.GetSellPrice(card.CardData);
                        gm.GainMoney(price);
                        card.IsInHand = false;
                        managers.deckManager.BurnCard(card.CardData);
                        success = true;
                        break;
                }

                if (success)
                {
                    gm.UseAction();
                    managers.deckManager.RemoveFromHand(card.CardData);
                    slot.PlaceCard(card);
                    EventBus.CardPlayed(card.CardData);

                    switch (slot.ZoneType)
                    {
                        case DropZoneType.ActionZone:
                            UnityEngine.Object.Destroy(card.gameObject, 0.5f);
                            slot.RemoveCard();
                            break;
                        case DropZoneType.SellZone:
                        case DropZoneType.BurnZone:
                            UnityEngine.Object.Destroy(card.gameObject, 0.3f);
                            slot.RemoveCard();
                            break;
                    }
                }
                else
                {
                    if (playCostPaid && card != null && card.CardData != null && card.CardData.playCost > 0)
                        gm.GainMoney(card.CardData.playCost);
                    card.ReturnToHand();
                }
            };
        }
    }
}

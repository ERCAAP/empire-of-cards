using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Gameplay;
using EmpireOfCards.UI.Cards;
using EmpireOfCards.World;

namespace EmpireOfCards.Bootstrap
{
    /// <summary>
    /// Wires all manager cross-references, button callbacks, and 3D interaction events.
    /// Pure static helper -- no state of its own.
    /// All wiring uses typed Init() methods.
    /// </summary>
    public static class WiringService
    {
        /// <summary>
        /// One-call entry: wires managers, UI, buttons, and 3D interaction.
        /// </summary>
        public static void WireAll(
            GameDataBundle data,
            ManagerBundle managers,
            Board3D board3D,
            CardFactory cardFactory,
            Hand3D hand3D,
            HUDBundle hud,
            Camera mainCamera)
        {
            // Expose the UIManager created by HUDBuilder back into the ManagerBundle
            managers.uiManager = hud.uiManager;

            WireManagerReferences(data, managers, board3D, cardFactory, hand3D, hud, mainCamera);
            WireButtonCallbacks(hud, managers);
            Wire3DInteraction(managers.inputManager3D, hand3D);

            Debug.Log("[WiringService] All wiring complete.");
        }

        // ================================================================
        // MANAGER CROSS-REFERENCES
        // ================================================================

        private static void WireManagerReferences(
            GameDataBundle data,
            ManagerBundle m,
            Board3D board3D,
            CardFactory cardFactory,
            Hand3D hand3D,
            HUDBundle hud,
            Camera mainCamera)
        {
            // === GameManager: data + all manager references ===
            m.gameManager.Init(
                data.balanceData, data.startingDeck,
                m.turnManager, m.economyManager, m.deckManager, m.boardManager,
                m.comboSystem, m.territoryManager, m.fbiSystem, m.rivalAI,
                m.shopManager, m.uiManager, m.audioManager, m.vfxManager, m.saveManager);

            // === EconomyManager ===
            m.economyManager.Init(data.balanceData, m.boardManager, m.comboSystem, m.abilitySystem, m.slotManager);

            // === ComboSystem ===
            m.comboSystem.Init(data.combos, m.boardManager);

            // === FBISystem ===
            m.fbiSystem.Init(data.balanceData, m.boardManager, m.comboSystem);

            // === RivalAI ===
            m.rivalAI.Init(data.rivalData);

            // === MetaProgressionSystem ===
            m.metaProgressionSystem.Init(data.metaProgressionData);
            m.gameManager.SetMetaProgressionSystem(m.metaProgressionSystem);

            // === CompanyTierSystem ===
            m.companyTierSystem.Init(m.boardManager, m.comboSystem);
            m.gameManager.SetCompanyTierSystem(m.companyTierSystem);

            // === SlotManager ===
            m.slotManager.Init();
            m.gameManager.SetSlotManager(m.slotManager);

            // === ShopManager ===
            m.shopManager.Init(data.shopPool, m.deckManager, m.economyManager, m.comboSystem);

            // === AudioManager ===
            m.audioManager.Init(m.musicSourceA, m.musicSourceB, m.sfxSource);

            // === UIManager: all panel references ===
            m.uiManager.Init(
                hud.topBarUI, hud.actionBarUI, hud.shopPanel,
                hud.comboPopup, hud.eventPopup, hud.rivalPopup,
                hud.scoreScreen, hud.gameOverScreen);

            // === Neglect warning text -> UIManager ===
            if (hud.neglectWarningText != null)
                m.uiManager.SetNeglectWarningText(hud.neglectWarningText);

            // === TopBarUI: TMP_Text and Image sub-elements ===
            hud.topBarUI.Init(hud.moneyText, hud.turnText, hud.fbiBarFillImg, null);

            // === ActionBarUI: action dot Image[] ===
            hud.actionBarUI.Init(hud.actionDotImages);

            // === ShopPanel: shopManager reference ===
            hud.shopPanel.Init(m.shopManager);

            // === Shop bias indicator text -> ShopPanel ===
            if (hud.shopBiasText != null)
                hud.shopPanel.SetBiasText(hud.shopBiasText);

            // === InputManager3D: camera reference ===
            m.inputManager3D.SetCamera(mainCamera);

            // === Hand3D: cardFactory + deckManager + anchor (anchor already set by bootstrap) ===
            hand3D.Init(cardFactory, m.deckManager, hand3D.transform.parent);

            // === Board3D: boardManager reference ===
            board3D.Init(m.boardManager);

            // === CardFactory: allCards reference for lookup ===
            cardFactory.Init(data.allCards);

            // === Event Deck: filter event cards from allCards and pass to TurnManager ===
            var eventCards = data.allCards.Where(c => c != null && c.cardType == CardType.Event).ToList();
            m.turnManager.SetEventDeck(eventCards);

            Debug.Log("[WiringService] All manager, UI, and 3D references wired via typed Init() calls.");
        }

        // ================================================================
        // BUTTON CALLBACKS
        // ================================================================

        private static void WireButtonCallbacks(HUDBundle hud, ManagerBundle m)
        {
            // End Turn button -> TurnManager.EndPlayPhase()
            if (hud.endTurnButton != null && m.turnManager != null)
                hud.endTurnButton.onClick.AddListener(() => m.turnManager.EndPlayPhase());

            // Shop button -> toggle shop
            if (hud.shopButton != null && m.uiManager != null)
            {
                bool shopOpen = false;
                hud.shopButton.onClick.AddListener(() => {
                    shopOpen = !shopOpen;
                    if (shopOpen) m.uiManager.ShowShop();
                    else m.uiManager.HideShop();
                });
            }

            // Shop close -> hide shop
            if (hud.shopCloseButton != null && m.uiManager != null)
                hud.shopCloseButton.onClick.AddListener(() => m.uiManager.HideShop());

            // UIManager end turn event -> TurnManager
            if (m.uiManager != null && m.turnManager != null)
                m.uiManager.OnEndTurnClicked += () => m.turnManager.EndPlayPhase();

            Debug.Log("[WiringService] Button callbacks wired.");
        }

        // ================================================================
        // 3D INTERACTION
        // ================================================================

        private static void Wire3DInteraction(InputManager3D inputManager3D, Hand3D hand3D)
        {
            // --- Card drop -> BoardManager placement ---
            inputManager3D.OnCardDropped += (card, slot) =>
            {
                var gm = GameManager.Instance;
                if (gm == null) return;

                bool success = false;

                switch (slot.ZoneType)
                {
                    case DropZoneType.BusinessSlot:
                        success = gm.BoardManager.PlaceBusiness(card.CardData, slot.SlotIndex);
                        break;

                    case DropZoneType.EmployeeSlot:
                        success = gm.BoardManager.PlaceEmployee(card.CardData, slot.ParentBusinessIndex);
                        break;

                    case DropZoneType.UpgradeSlot:
                        // Upgrade area is separate from businesses - use -1 for global placement
                        success = gm.BoardManager.PlaceUpgrade(card.CardData, -1);
                        if (success)
                        {
                            // Apply immediate upgrade effects (P2 #14)
                            switch (card.CardData.upgradeEffectType)
                            {
                                case UpgradeEffectType.ExtraAction:
                                    gm.AddExtraAction(card.CardData.extraActions);
                                    break;
                                case UpgradeEffectType.ReduceFBIRisk:
                                    // FBI system already checks for security upgrade
                                    // on the board each resolve phase; nothing extra needed.
                                    break;
                                // Other upgrades (IncomePercent*, GlobalCustomer*, etc.)
                                // are passive and handled by IncomeCalculator each turn.
                            }
                        }
                        break;

                    case DropZoneType.ActionZone:
                        EventBus.ActionExecuted(card.CardData);
                        success = true;
                        break;

                    case DropZoneType.SellZone:
                        int price = gm.EconomyManager.GetSellPrice(card.CardData);
                        gm.GainMoney(price);
                        // Mark as not-in-hand BEFORE BurnCard, because BurnCard fires
                        // EventBus.CardBurned which triggers Hand3D.RemoveCardFromHand.
                        // If IsInHand is still true, Hand3D would Destroy the 3D object
                        // before our delayed Destroy gets a chance to show it briefly.
                        card.IsInHand = false;
                        gm.DeckManager.BurnCard(card.CardData); // Remove from deck permanently
                        success = true;
                        break;
                }

                if (success)
                {
                    gm.UseAction();

                    // Remove the card from DeckManager's hand list so it does NOT
                    // re-enter the discard pile during DiscardHand() at next turn.
                    gm.DeckManager.RemoveFromHand(card.CardData);

                    // PlaceCard MUST run before CardPlayed so IsInHand is false
                    // when Hand3D.RemoveCardFromHand checks — otherwise it destroys
                    // the card that should stay on the board.
                    slot.PlaceCard(card);
                    EventBus.CardPlayed(card.CardData);

                    // Action / Sell / Burn cards are instant — destroy after brief display
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
                    card.ReturnToHand();
                }
            };

            // Phase-gating (InputManager3D) and turn-start layout refresh (Hand3D)
            // are now handled via OnEnable/OnDisable in each MonoBehaviour,
            // avoiding leaked lambda subscriptions on EventBus.

            Debug.Log("[WiringService] 3D interaction wired: InputManager3D -> BoardManager.");
        }
    }
}

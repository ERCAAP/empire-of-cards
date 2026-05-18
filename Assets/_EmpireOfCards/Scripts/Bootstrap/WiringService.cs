using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Gameplay;
using EmpireOfCards.Helpers;
using EmpireOfCards.UI.Cards;
using EmpireOfCards.World;

namespace EmpireOfCards.Bootstrap
{
    /// <summary>
    /// Wires all manager cross-references, button callbacks, and 3D interaction events.
    /// Pure static helper -- no state of its own.
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
            RuntimeWiring.SetField(m.gameManager, "balanceData", data.balanceData);
            RuntimeWiring.SetField(m.gameManager, "startingDeck", data.startingDeck);
            RuntimeWiring.SetField(m.gameManager, "turnManager", m.turnManager);
            RuntimeWiring.SetField(m.gameManager, "economyManager", m.economyManager);
            RuntimeWiring.SetField(m.gameManager, "deckManager", m.deckManager);
            RuntimeWiring.SetField(m.gameManager, "boardManager", m.boardManager);
            RuntimeWiring.SetField(m.gameManager, "comboSystem", m.comboSystem);
            RuntimeWiring.SetField(m.gameManager, "territoryManager", m.territoryManager);
            RuntimeWiring.SetField(m.gameManager, "fbiSystem", m.fbiSystem);
            RuntimeWiring.SetField(m.gameManager, "rivalAI", m.rivalAI);
            RuntimeWiring.SetField(m.gameManager, "shopManager", m.shopManager);
            RuntimeWiring.SetField(m.gameManager, "uiManager", m.uiManager);
            RuntimeWiring.SetField(m.gameManager, "audioManager", m.audioManager);
            RuntimeWiring.SetField(m.gameManager, "vfxManager", m.vfxManager);
            RuntimeWiring.SetField(m.gameManager, "saveManager", m.saveManager);

            // === EconomyManager ===
            RuntimeWiring.SetField(m.economyManager, "balanceData", data.balanceData);
            RuntimeWiring.SetField(m.economyManager, "boardManager", m.boardManager);
            RuntimeWiring.SetField(m.economyManager, "comboSystem", m.comboSystem);

            // === ComboSystem ===
            RuntimeWiring.SetField(m.comboSystem, "allCombos", data.combos);
            RuntimeWiring.SetField(m.comboSystem, "boardManager", m.boardManager);

            // === FBISystem ===
            RuntimeWiring.SetField(m.fbiSystem, "balanceData", data.balanceData);
            RuntimeWiring.SetField(m.fbiSystem, "boardManager", m.boardManager);
            RuntimeWiring.SetField(m.fbiSystem, "comboSystem", m.comboSystem);

            // === RivalAI ===
            RuntimeWiring.SetField(m.rivalAI, "data", data.rivalData);

            // === ShopManager ===
            RuntimeWiring.SetField(m.shopManager, "shopPool", data.shopPool);
            RuntimeWiring.SetField(m.shopManager, "deckManager", m.deckManager);
            RuntimeWiring.SetField(m.shopManager, "economyManager", m.economyManager);
            RuntimeWiring.SetField(m.shopManager, "comboSystem", m.comboSystem);

            // === AudioManager ===
            RuntimeWiring.SetField(m.audioManager, "musicSourceA", m.musicSourceA);
            RuntimeWiring.SetField(m.audioManager, "musicSourceB", m.musicSourceB);
            RuntimeWiring.SetField(m.audioManager, "sfxSource", m.sfxSource);

            // === UIManager: all panel references ===
            RuntimeWiring.SetField(m.uiManager, "topBar", hud.topBarUI);
            RuntimeWiring.SetField(m.uiManager, "actionBar", hud.actionBarUI);
            RuntimeWiring.SetField(m.uiManager, "shopPanel", hud.shopPanel);
            RuntimeWiring.SetField(m.uiManager, "comboPopup", hud.comboPopup);
            RuntimeWiring.SetField(m.uiManager, "eventPopup", hud.eventPopup);
            RuntimeWiring.SetField(m.uiManager, "rivalPopup", hud.rivalPopup);
            RuntimeWiring.SetField(m.uiManager, "scoreScreen", hud.scoreScreen);
            RuntimeWiring.SetField(m.uiManager, "gameOverScreen", hud.gameOverScreen);

            // === TopBarUI: TMP_Text and Image sub-elements ===
            RuntimeWiring.SetField(hud.topBarUI, "moneyText", hud.moneyText);
            RuntimeWiring.SetField(hud.topBarUI, "turnText", hud.turnText);
            RuntimeWiring.SetField(hud.topBarUI, "fbiBarFill", hud.fbiBarFillImg);

            // === ActionBarUI: action dot Image[] ===
            RuntimeWiring.SetField(hud.actionBarUI, "actionDots", hud.actionDotImages);

            // === ShopPanel: shopManager reference ===
            RuntimeWiring.SetField(hud.shopPanel, "shopManager", m.shopManager);

            // === InputManager3D: camera reference ===
            RuntimeWiring.SetField(m.inputManager3D, "mainCamera", mainCamera);

            // === Hand3D: cardFactory + deckManager ===
            RuntimeWiring.SetField(hand3D, "cardFactory", cardFactory);
            RuntimeWiring.SetField(hand3D, "deckManager", m.deckManager);

            // === Board3D: boardManager reference ===
            RuntimeWiring.SetField(board3D, "boardManager", m.boardManager);

            // === CardFactory: allCards reference for lookup ===
            RuntimeWiring.SetField(cardFactory, "allCards", data.allCards);

            Debug.Log("[WiringService] All manager, UI, and 3D references wired via RuntimeWiring.");
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
                        success = gm.BoardManager.PlaceUpgrade(card.CardData, slot.SlotIndex);
                        break;

                    case DropZoneType.ActionZone:
                        EventBus.ActionExecuted(card.CardData);
                        success = true;
                        break;

                    case DropZoneType.SellZone:
                        int price = gm.EconomyManager.GetSellPrice(card.CardData);
                        gm.GainMoney(price);
                        success = true;
                        break;
                }

                if (success)
                {
                    gm.UseAction();
                    EventBus.CardPlayed(card.CardData);
                    slot.PlaceCard(card);
                }
                else
                {
                    card.ReturnToHand();
                }
            };

            // --- Enable/disable input based on turn phase ---
            EventBus.OnPhaseStarted += phase =>
            {
                inputManager3D.InputEnabled = (phase == TurnPhase.PlayPhase);
            };

            // --- Turn start: refresh hand layout ---
            EventBus.OnTurnStarted += turn =>
            {
                if (hand3D != null)
                    hand3D.RefreshLayout();
            };

            Debug.Log("[WiringService] 3D interaction wired: InputManager3D -> BoardManager, phase gating enabled.");
        }
    }
}

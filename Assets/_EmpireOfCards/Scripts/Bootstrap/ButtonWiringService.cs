namespace EmpireOfCards.Bootstrap
{
    public static class ButtonWiringService
    {
        public static void Wire(HUDBundle hud, ManagerBundle m)
        {
            if (hud.endTurnButton != null && m.turnManager != null)
                hud.endTurnButton.onClick.AddListener(() => m.turnManager.EndPlayPhase());

            if (hud.shopButton != null && m.uiManager != null)
            {
                bool shopOpen = false;
                hud.shopButton.onClick.AddListener(() =>
                {
                    shopOpen = !shopOpen;
                    if (shopOpen)
                        m.uiManager.ShowShop();
                    else
                        m.uiManager.HideShop();
                });
            }

            if (hud.shopCloseButton != null && m.uiManager != null)
                hud.shopCloseButton.onClick.AddListener(() => m.uiManager.HideShop());

            if (m.uiManager != null && m.turnManager != null)
                m.uiManager.OnEndTurnClicked += () => m.turnManager.EndPlayPhase();
        }
    }
}

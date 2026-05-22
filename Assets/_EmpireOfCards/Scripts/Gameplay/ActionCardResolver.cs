using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.Gameplay.Staff;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// Subscribes to EventBus.OnActionExecuted and resolves each action card's
    /// effect based on its ActionEffectType (GDD Section 3.3).
    /// Created by ManagerFactory and lives as a child of the GameManager object.
    /// Status: partial legacy surface. Some effects still reflect the older
    /// business-centric prototype and should be treated as compatibility logic
    /// until the venture-first action taxonomy fully replaces them.
    /// </summary>
    public partial class ActionCardResolver : MonoBehaviour
    {
        private void OnEnable()
        {
            EventBus.OnActionExecuted += ResolveAction;
        }

        private void OnDisable()
        {
            EventBus.OnActionExecuted -= ResolveAction;
        }

        private void ResolveAction(CardData card)
        {
            if (card == null || card.cardType != CardType.Action) return;

            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogError("[ActionCardResolver] GameManager.Instance is null.");
                return;
            }

            if (!TryResolveActiveEffect(gm, card) && !TryResolveLegacyEffect(gm, card))
            {
                Debug.LogWarning($"[ActionCardResolver] Unsupported ActionEffectType in active v4 runtime: {card.actionEffectType}");
            }

            Debug.Log($"[ActionCardResolver] Resolved: {card.cardName} ({card.actionEffectType})");
        }
    }
}

using EmpireOfCards.Core.StateMachine;
using EmpireOfCards.Data;

namespace EmpireOfCards.Core.TurnPhases
{
    /// <summary>
    /// Turn phase 3: Player plays cards with 3 actions (GDD Section 4.1, Step 3).
    /// Polls for player input -- waits until player ends turn or runs out of actions.
    /// </summary>
    public class PlayPhase : IState
    {
        private readonly TurnManager _turnManager;

        public PlayPhase(TurnManager tm) { _turnManager = tm; }

        public void Enter()
        {
            _turnManager.ResetPlayerEndedPlayPhase();
            // UI enables card dragging, action dots, end turn button
        }

        public void Tick()
        {
            var gm = GameManager.Instance;

            // POLLING: check if player ended turn or ran out of actions
            if (_turnManager.PlayerEndedPlayPhase || gm.PlayerActions <= 0)
            {
                _turnManager.CompleteCurrentPhase();
            }
        }

        public void Exit()
        {
            // UI disables card dragging
        }

        /// <summary>
        /// Maps a CardData type to the default SlotType it should target.
        /// Used by InputManager3D to highlight valid zones when player picks up a card.
        /// Returns null for Action cards (single-use, no slot).
        /// </summary>
        public static SlotType? GetDefaultSlotType(CardData card)
        {
            if (card == null) return null;

            switch (card.cardType)
            {
                case CardType.Business:
                    return SlotType.Operation;

                case CardType.Employee:
                    return SlotType.Staff;

                case CardType.Upgrade:
                    // Route by tag: Marketing or Logistics/Supplier tags
                    if (card.tags != null)
                    {
                        foreach (var tag in card.tags)
                        {
                            if (tag == CardTag.Marketing || tag == CardTag.Influencer)
                                return SlotType.Marketing;
                            if (tag == CardTag.Logistics || tag == CardTag.Organic)
                                return SlotType.Supplier;
                        }
                    }
                    return SlotType.Operation; // Default upgrades go to operation

                case CardType.Event:
                    return SlotType.TempEffect;

                case CardType.Action:
                    return null; // Single-use, targets ActionZone directly

                default:
                    return null;
            }
        }
    }
}

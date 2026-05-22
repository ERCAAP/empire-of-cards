using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    public partial class RivalAI
    {
        private List<RivalQueuedAction> BuildQueuedActionsInternal(float playerShare, int currentTurn)
        {
            _queuedActions.Clear();
            rivalPressure = Mathf.Max(0f, rivalPressure * 0.65f);

            RivalMove primaryMove = DecideMove(playerShare, currentTurn);
            _queuedActions.Add(BuildAction(primaryMove));
            _runtimeState.campaignsLaunched++;

            if (currentTurn >= 6 || playerShare >= 46f || _runtimeState.escalationLevel >= 2)
            {
                RivalMove secondaryMove = ChooseSecondaryMove(primaryMove, currentTurn);
                _queuedActions.Add(BuildAction(secondaryMove));
            }

            if (currentTurn >= 4 || playerShare >= 50f)
                _runtimeState.escalationLevel = Mathf.Clamp(_runtimeState.escalationLevel + 1, 0, 5);

            return _queuedActions;
        }
    }
}

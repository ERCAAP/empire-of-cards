using EmpireOfCards.Core;
using EmpireOfCards.Gameplay;

namespace EmpireOfCards.Gameplay.Board
{
    /// <summary>
    /// Manages business closure and reopening.
    /// Tracks closedTurnsRemaining and auto-reopens when the timer expires.
    /// </summary>
    public class ClosureManager
    {
        /// <summary>
        /// Callback for reopening a business. BoardManager provides its
        /// ReopenBusiness method so this class stays decoupled.
        /// </summary>
        public delegate void ReopenBusinessDelegate(int businessIndex);

        private readonly ReopenBusinessDelegate _reopenBusiness;

        public ClosureManager(ReopenBusinessDelegate reopenBusiness)
        {
            _reopenBusiness = reopenBusiness;
        }

        /// <summary>
        /// Closes a business for the specified number of turns.
        /// </summary>
        public void CloseBusiness(ActiveBusiness business, int businessIndex, int turns)
        {
            if (business == null) return;

            business.isClosed = true;
            business.closedTurnsRemaining = turns;
            EventBus.BusinessClosed(businessIndex);
        }

        /// <summary>
        /// Reopens a previously closed business (resets closure state).
        /// </summary>
        public void ReopenBusiness(ActiveBusiness business, int businessIndex)
        {
            if (business == null) return;

            business.isClosed = false;
            business.closedTurnsRemaining = 0;
            EventBus.BusinessReopened(businessIndex);
        }

        /// <summary>
        /// Ticks all closed businesses in the list. Decrements their remaining
        /// turns and reopens any whose timer has expired.
        /// </summary>
        public void TickClosures(System.Collections.Generic.List<ActiveBusiness> businesses)
        {
            for (int i = 0; i < businesses.Count; i++)
            {
                ActiveBusiness business = businesses[i];
                if (!business.isClosed) continue;

                business.closedTurnsRemaining--;
                if (business.closedTurnsRemaining <= 0)
                {
                    ReopenBusiness(business, i);
                }
            }
        }
    }
}

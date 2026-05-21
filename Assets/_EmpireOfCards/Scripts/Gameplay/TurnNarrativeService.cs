using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    public static class TurnNarrativeService
    {
        public static string GetBriefHeadline(BoardPressureType pressure, VentureType venture, TechCategoryProfile category)
        {
            if (category != null)
            {
                return pressure switch
                {
                    BoardPressureType.CapacityShortfall => $"{category.displayName} growth is outrunning the stack.",
                    BoardPressureType.LowRating => $"{category.displayName} trust is getting fragile.",
                    BoardPressureType.HighLegalRisk => $"{category.displayName} risk is becoming visible.",
                    _ => pressure switch
                    {
                        BoardPressureType.LowCash => "Cash runway is tightening.",
                        BoardPressureType.WeakQuality => "Quality is underperforming.",
                        BoardPressureType.StaffInstability => "Team stability is fragile.",
                        BoardPressureType.LowDemand => "Demand needs a push.",
                        _ => "Board is stable, push your edge."
                    }
                };
            }

            return venture switch
            {
                VentureType.FastFood => pressure switch
                {
                    BoardPressureType.CapacityShortfall => "Kitchen pressure is spiking.",
                    BoardPressureType.LowRating => "Local trust is wobbling.",
                    BoardPressureType.WeakQuality => "Ingredient quality is falling behind.",
                    _ => pressure switch
                    {
                        BoardPressureType.LowCash => "Cash runway is tightening.",
                        BoardPressureType.HighLegalRisk => "Legal exposure is rising.",
                        BoardPressureType.StaffInstability => "Team stability is fragile.",
                        BoardPressureType.LowDemand => "Demand needs a push.",
                        _ => "Board is stable, push your edge."
                    }
                },
                VentureType.Cafe => pressure switch
                {
                    BoardPressureType.CapacityShortfall => "The bar is backing up.",
                    BoardPressureType.LowRating => "Neighborhood trust is fading.",
                    BoardPressureType.StaffInstability => "The shift is starting to crack.",
                    _ => pressure switch
                    {
                        BoardPressureType.LowCash => "Cash runway is tightening.",
                        BoardPressureType.HighLegalRisk => "Legal exposure is rising.",
                        BoardPressureType.WeakQuality => "Quality is underperforming.",
                        BoardPressureType.LowDemand => "Demand needs a push.",
                        _ => "Board is stable, push your edge."
                    }
                },
                VentureType.ClothingStore => pressure switch
                {
                    BoardPressureType.CapacityShortfall => "The floor cannot absorb demand.",
                    BoardPressureType.LowRating => "Brand trust is slipping.",
                    BoardPressureType.WeakQuality => "Fabric and fit are under pressure.",
                    _ => pressure switch
                    {
                        BoardPressureType.LowCash => "Cash runway is tightening.",
                        BoardPressureType.HighLegalRisk => "Legal exposure is rising.",
                        BoardPressureType.StaffInstability => "Team stability is fragile.",
                        BoardPressureType.LowDemand => "Demand needs a push.",
                        _ => "Board is stable, push your edge."
                    }
                },
                VentureType.GroceryStore => pressure switch
                {
                    BoardPressureType.CapacityShortfall => "Shelf pressure is building.",
                    BoardPressureType.LowRating => "Mahalle trust is slipping.",
                    BoardPressureType.WeakQuality => "Freshness discipline is slipping.",
                    _ => pressure switch
                    {
                        BoardPressureType.LowCash => "Cash runway is tightening.",
                        BoardPressureType.HighLegalRisk => "Legal exposure is rising.",
                        BoardPressureType.StaffInstability => "Team stability is fragile.",
                        BoardPressureType.LowDemand => "Demand needs a push.",
                        _ => "Board is stable, push your edge."
                    }
                },
                _ => pressure switch
                {
                    BoardPressureType.CapacityShortfall => "Rush pressure is building.",
                    BoardPressureType.LowCash => "Cash runway is tightening.",
                    BoardPressureType.LowRating => "Trust is slipping.",
                    BoardPressureType.HighLegalRisk => "Legal exposure is rising.",
                    BoardPressureType.WeakQuality => "Quality is underperforming.",
                    BoardPressureType.StaffInstability => "Team stability is fragile.",
                    BoardPressureType.LowDemand => "Demand needs a push.",
                    _ => "Board is stable, push your edge."
                }
            };
        }

        public static string GetBriefDetail(BoardPressureType pressure, VentureType venture, TechCategoryProfile category, VentureBoardSnapshot snapshot)
        {
            if (category != null)
            {
                return pressure switch
                {
                    BoardPressureType.CapacityShortfall => $"{category.displayName} traffic is beating delivery capacity. Backend and support must catch up.",
                    BoardPressureType.LowCash => $"{category.displayName} spend is too hot. Paid growth and infra discipline matter now.",
                    BoardPressureType.LowRating => $"{category.displayName} reviews are fragile. Fix trust before pushing more installs.",
                    BoardPressureType.HighLegalRisk => $"{category.displayName} is carrying visible risk. Privacy, dark patterns, or unstable launches may cascade.",
                    BoardPressureType.WeakQuality => $"{category.displayName} quality is lagging. Product reliability and core experience need help.",
                    BoardPressureType.StaffInstability => $"{category.displayName} team flow is shaky. Burnout or rushed releases can snowball.",
                    BoardPressureType.LowDemand => $"{category.displayName} discovery is too weak. ASO, creators, or targeted acquisition should lead.",
                    _ => $"{category.displayName}: rating {snapshot.rating:0.0}, quality {snapshot.quality:0.0}, share {snapshot.marketShare:0.0}."
                };
            }

            return venture switch
            {
                VentureType.FastFood => pressure switch
                {
                    BoardPressureType.CapacityShortfall => $"Demand {snapshot.demand:0.0} is outrunning kitchen and counter throughput.",
                    BoardPressureType.LowCash => $"Cash is {snapshot.cash:0}. Delivery spend and wages are squeezing margin.",
                    BoardPressureType.LowRating => $"Rating is {snapshot.rating:0.0}. Reviews and quality fixes should lead.",
                    BoardPressureType.HighLegalRisk => $"Legal risk is {snapshot.legalRisk:0}. Fake reviews or hygiene shortcuts can cascade.",
                    BoardPressureType.WeakQuality => $"Quality is {snapshot.quality:0.0}. Ingredient discipline and cleanup matter now.",
                    BoardPressureType.StaffInstability => $"Staff stability is {snapshot.staffStability:0.0}. Rush fatigue will slow service.",
                    BoardPressureType.LowDemand => $"Demand is only {snapshot.demand:0.0}. Local buzz and Google presence need help.",
                    _ => $"Rating {snapshot.rating:0.0}, quality {snapshot.quality:0.0}, share {snapshot.marketShare:0.0}."
                },
                VentureType.Cafe => pressure switch
                {
                    BoardPressureType.CapacityShortfall => $"Demand {snapshot.demand:0.0} is overrunning the bar and floor flow.",
                    BoardPressureType.LowCash => $"Cash is {snapshot.cash:0}. Premium beans and staffing are biting margin.",
                    BoardPressureType.LowRating => $"Rating is {snapshot.rating:0.0}. Slow service or weak drinks are visible.",
                    BoardPressureType.HighLegalRisk => $"Legal risk is {snapshot.legalRisk:0}. Shortcut ambience or complaints may escalate.",
                    BoardPressureType.WeakQuality => $"Quality is {snapshot.quality:0.0}. Beans, milk, and consistency are trailing.",
                    BoardPressureType.StaffInstability => $"Staff stability is {snapshot.staffStability:0.0}. Burnout can spill into reviews.",
                    BoardPressureType.LowDemand => $"Demand is only {snapshot.demand:0.0}. Regulars and visual discovery need a push.",
                    _ => $"Rating {snapshot.rating:0.0}, quality {snapshot.quality:0.0}, share {snapshot.marketShare:0.0}."
                },
                VentureType.ClothingStore => pressure switch
                {
                    BoardPressureType.CapacityShortfall => $"Demand {snapshot.demand:0.0} is outpacing floor conversion and stock handling.",
                    BoardPressureType.LowCash => $"Cash is {snapshot.cash:0}. Discounting and return pressure are squeezing margin.",
                    BoardPressureType.LowRating => $"Rating is {snapshot.rating:0.0}. Fit, returns, and brand trust need recovery.",
                    BoardPressureType.HighLegalRisk => $"Legal risk is {snapshot.legalRisk:0}. Cheap fabric or shady claims can rebound.",
                    BoardPressureType.WeakQuality => $"Quality is {snapshot.quality:0.0}. Fabric, fit, and atelier quality are behind.",
                    BoardPressureType.StaffInstability => $"Staff stability is {snapshot.staffStability:0.0}. The sales floor is getting brittle.",
                    BoardPressureType.LowDemand => $"Demand is only {snapshot.demand:0.0}. Vitrine storytelling and trend pull need help.",
                    _ => $"Rating {snapshot.rating:0.0}, quality {snapshot.quality:0.0}, share {snapshot.marketShare:0.0}."
                },
                VentureType.GroceryStore => pressure switch
                {
                    BoardPressureType.CapacityShortfall => $"Demand {snapshot.demand:0.0} is beating shelf and checkout throughput.",
                    BoardPressureType.LowCash => $"Cash is {snapshot.cash:0}. Thin margin and waste are active this turn.",
                    BoardPressureType.LowRating => $"Rating is {snapshot.rating:0.0}. Freshness trust is too fragile right now.",
                    BoardPressureType.HighLegalRisk => $"Legal risk is {snapshot.legalRisk:0}. SKT shortcuts or trust gaps can snowball.",
                    BoardPressureType.WeakQuality => $"Quality is {snapshot.quality:0.0}. Freshness and shelf discipline are slipping.",
                    BoardPressureType.StaffInstability => $"Staff stability is {snapshot.staffStability:0.0}. Empty shelves and slow lines can follow.",
                    BoardPressureType.LowDemand => $"Demand is only {snapshot.demand:0.0}. Convenience and neighborhood loyalty need help.",
                    _ => $"Rating {snapshot.rating:0.0}, quality {snapshot.quality:0.0}, share {snapshot.marketShare:0.0}."
                },
                _ => pressure switch
                {
                    BoardPressureType.CapacityShortfall => $"Demand {snapshot.demand:0.0} > capacity {snapshot.capacity:0.0}. Add staff or throughput.",
                    BoardPressureType.LowCash => $"Cash is {snapshot.cash:0}. Margin discipline matters this turn.",
                    BoardPressureType.LowRating => $"Rating is {snapshot.rating:0.0}. Recovery and quality upgrades now pay off.",
                    BoardPressureType.HighLegalRisk => $"Legal risk is {snapshot.legalRisk:0}. Defensive reactions are valuable.",
                    BoardPressureType.WeakQuality => $"Quality is {snapshot.quality:0.0}. Supplier and staff choices are lagging.",
                    BoardPressureType.StaffInstability => $"Staff stability is {snapshot.staffStability:0.0}. Burnout will cascade if ignored.",
                    BoardPressureType.LowDemand => $"Demand is only {snapshot.demand:0.0}. Marketing or discovery should lead.",
                    _ => $"Rating {snapshot.rating:0.0}, quality {snapshot.quality:0.0}, share {snapshot.marketShare:0.0}."
                }
            };
        }

        public static string GetRecommendedMove(BoardPressureType pressure, string buildIdentity)
        {
            return pressure switch
            {
                BoardPressureType.CapacityShortfall => "Play a Fix Capacity or staff card before adding more demand.",
                BoardPressureType.LowDemand => "Play a Create Demand card only if your board can absorb the traffic.",
                BoardPressureType.LowRating => "Play a Recover Rating or quality card before pushing harder.",
                BoardPressureType.HighLegalRisk => "Play a Reduce Risk reaction and avoid another shortcut.",
                BoardPressureType.LowCash => "Play an Improve Margin card or cut expensive pressure lanes.",
                BoardPressureType.WeakQuality => "Play a supplier or quality-focused staff card.",
                BoardPressureType.StaffInstability => "Stabilize the team before the next rush turn.",
                _ => $"Lean into your build: {buildIdentity}."
            };
        }

        public static bool TryBuildOpeningBrief(
            int currentTurn,
            VentureType ventureType,
            int operationCount,
            int staffCount,
            int supplierCount,
            int marketingCount,
            BoardPressureType pressure,
            string buildIdentity,
            out TurnBriefData brief)
        {
            brief = null;

            if (currentTurn > 3)
                return false;

            string headline;
            string detail;
            string move;

            switch (ventureType)
            {
                case VentureType.FastFood:
                    if (operationCount <= 1)
                    {
                        headline = "Open the floor before you chase volume.";
                        detail = "The counter is live, but the rush still needs seating and service flow around the grill.";
                        move = "Play Extra Tables first. Add a Line Cook or counter helper before heavy flyer demand.";
                    }
                    else if (staffCount == 0)
                    {
                        headline = "Put people on the line.";
                        detail = "Your rush cannot feel real until the grill and front counter have actual labor behind them.";
                        move = "Hire Line Cook or Front Counter Server now. Save big demand pushes until the queue feels stable.";
                    }
                    else if (supplierCount == 0)
                    {
                        headline = "Protect food quality before the reviews hit.";
                        detail = "Ingredient trust and hygiene are what make a busy fast food board survive the first surge.";
                        move = "Take Premium Butcher or Night Cleaner before stacking more marketing pressure.";
                    }
                    else if (marketingCount == 0)
                    {
                        headline = "Now the neighborhood should notice you.";
                        detail = "Your floor is credible enough to start converting local attention into repeat foot traffic.";
                        move = "Push Google Business or Flyer Team only after the board can absorb the next rush.";
                    }
                    else
                    {
                        return false;
                    }
                    break;

                case VentureType.Cafe:
                    if (operationCount <= 1)
                    {
                        headline = "Make the cafe feel open, not just named.";
                        detail = "The espresso bar exists, but guests still need a room, a seat, and visible flow around the counter.";
                        move = "Play Window Seating first. Then line up Senior Barista before you spend on buzz.";
                    }
                    else if (staffCount == 0)
                    {
                        headline = "The room needs a real shift behind it.";
                        detail = "A cafe does not feel alive until the bar and floor have an actual operator holding consistency.";
                        move = "Hire Senior Barista now. Floor Runner is the next stabilizer if service starts backing up.";
                    }
                    else if (supplierCount == 0)
                    {
                        headline = "Lock the taste before you chase the crowd.";
                        detail = "Beans, milk, and drink consistency are what turn one-time curiosity into neighborhood loyalty.";
                        move = "Play Specialty Beans or Milk Contract before you lean into Instagram or repeat traffic.";
                    }
                    else if (marketingCount == 0)
                    {
                        headline = "Now you can build the regular loop.";
                        detail = "Service is credible enough to turn trust into Maps discovery, reels, and repeat morning visits.";
                        move = "Use Maps Reviews first for trust, then Instagram Reels or Stamp Card once the floor still feels smooth.";
                    }
                    else
                    {
                        return false;
                    }
                    break;

                case VentureType.TechApp:
                    if (operationCount <= 1)
                    {
                        headline = "Ship the core before you buy growth.";
                        detail = "The product is live, but the stack still needs more reliability before users arrive at scale.";
                        move = "Play Backend Upgrade or a second product card before paid acquisition starts pulling installs.";
                    }
                    else if (staffCount == 0)
                    {
                        headline = "Put real builders behind the MVP.";
                        detail = "The app still feels fragile until a developer or support layer turns the launch into a system.";
                        move = "Hire Core Developer first. Support or PM follow once the product loop is real.";
                    }
                    else if (supplierCount == 0)
                    {
                        headline = "Stability economics come before growth economics.";
                        detail = "Cloud, analytics, and payment infrastructure decide whether growth becomes retention or chaos.";
                        move = "Take Cloud Credits or Export Pipeline before the biggest acquisition cards.";
                    }
                    else if (marketingCount == 0)
                    {
                        headline = "Now turn reliability into user growth.";
                        detail = "You finally have enough product trust to scale discovery without instantly burning reviews.";
                        move = "Lead with ASO Push. Add paid acquisition only if backend and rating still look safe.";
                    }
                    else
                    {
                        return false;
                    }
                    break;

                case VentureType.ClothingStore:
                    if (operationCount <= 1)
                    {
                        headline = "Dress the floor before you advertise the brand.";
                        detail = "The storefront exists, but the customer still needs depth, fit confidence, and visible merchandise logic.";
                        move = "Play Inventory Rail first. Then add fit support before forcing trend demand.";
                    }
                    else if (staffCount == 0)
                    {
                        headline = "Style needs staff, not only display.";
                        detail = "A clothing board feels empty until a stylist or tailor can convert browsing into trust.";
                        move = "Hire Floor Stylist first. Tailor becomes the next key stabilizer.";
                    }
                    else if (supplierCount == 0)
                    {
                        headline = "Protect fit and fabric before trend traffic.";
                        detail = "Returns and weak materials are what make a promising clothing run collapse early.";
                        move = "Play Reliable Atelier or Premium Fabric Mill before aggressive demand plays.";
                    }
                    else if (marketingCount == 0)
                    {
                        headline = "Now the collection is ready to be seen.";
                        detail = "The board has enough fit confidence to turn display into higher-value browsing and conversion.";
                        move = "Use Instagram Lookbook or Window Story Display once return pressure feels covered.";
                    }
                    else
                    {
                        return false;
                    }
                    break;

                case VentureType.GroceryStore:
                    if (operationCount <= 1)
                    {
                        headline = "Make the store function before you extend convenience.";
                        detail = "Fresh shelves are live, but the basket still needs smoother checkout and a clearer trip flow.";
                        move = "Play Checkout Upgrade first. Then add the first staff layer before growing convenience demand.";
                    }
                    else if (staffCount == 0)
                    {
                        headline = "Neighborhood trust starts with the people inside.";
                        detail = "A grocery board does not feel dependable until the register and shelf rhythm have actual staff support.";
                        move = "Hire Trusted Cashier first. Follow with stock or fresh-keeping support if lines stay messy.";
                    }
                    else if (supplierCount == 0)
                    {
                        headline = "Protect freshness before you widen reach.";
                        detail = "Morning supply quality and spoilage discipline are what make repeat traffic stick in grocery.";
                        move = "Take Morning Hal Route before you invest harder in convenience or late-night traffic.";
                    }
                    else if (marketingCount == 0)
                    {
                        headline = "Now convenience can become loyalty.";
                        detail = "The store is credible enough to convert service reliability into repeat neighborhood demand.";
                        move = "Use WhatsApp Orders or Late Night Sign once checkout and freshness still feel under control.";
                    }
                    else
                    {
                        return false;
                    }
                    break;

                default:
                    return false;
            }

            brief = new TurnBriefData
            {
                currentTurn = currentTurn,
                pressure = pressure,
                headline = headline,
                detail = detail,
                recommendedMove = move,
                buildIdentity = buildIdentity
            };

            return true;
        }

        public static string BuildReportHeadline(
            VentureType venture,
            TechCategoryProfile category,
            int netIncome,
            float overload,
            float marketShare)
        {
            if (category != null)
            {
                if (netIncome < 0)
                    return $"{category.displayName} burned runway this turn.";
                if (overload > 0.25f)
                    return $"{category.displayName} growth outran stability.";
            }

            if (venture == VentureType.FastFood && overload > 0.25f)
                return "The rush outran your kitchen.";
            if (venture == VentureType.Cafe && overload > 0.25f)
                return "The shift lost pace under pressure.";
            if (venture == VentureType.ClothingStore && netIncome < 0)
                return "Discounting and returns ate the turn.";
            if (venture == VentureType.GroceryStore && netIncome < 0)
                return "Margin got squeezed by waste and convenience.";
            if (netIncome < 0)
                return "This turn burned cash.";
            if (overload > 0.25f)
                return "Growth outpaced the board.";
            if (marketShare > 55f)
                return "You tightened your grip on the market.";
            return "Board held together this turn.";
        }

        public static string BuildPrimaryReason(
            float overload,
            int gross,
            int salaries,
            int upkeep,
            int tax,
            int netIncome,
            float rating,
            float cash,
            float rivalPressureImpact,
            string rivalPressureStyle)
        {
            if (overload > 0.25f)
                return $"Demand beat capacity by {overload:0.0}, so trust took a hit.";

            if (netIncome < 0 && salaries + upkeep > gross)
                return $"Salaries and upkeep ({salaries + upkeep}) outpaced revenue {gross}.";

            if (rating < 3.4f)
                return $"Low trust kept rating at {rating:0.0}; recovery now has higher payoff.";

            if (cash < 120f)
                return $"Cash fell to {cash:0}; margin pressure is now your main fight.";

            if (rivalPressureImpact > 0.1f)
                return $"Rival pressure ({rivalPressureStyle}) slowed your share gain this turn.";

            return $"Revenue {gross} covered salaries {salaries}, upkeep {upkeep}, and tax {tax}.";
        }

        public static void AppendCategoryReason(System.Collections.Generic.List<string> reasons, TechCategoryProfile category)
        {
            if (category == null || reasons == null)
                return;

            reasons.Add(category.scenarioNote);
        }
    }
}

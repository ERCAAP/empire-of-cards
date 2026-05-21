using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Data;

namespace EmpireOfCards.World
{
    public class CardFactory3D : MonoBehaviour
    {
        readonly List<Card3D> _pool = new();

        public Card3D CreateCard(CardData data)
        {
            Card3D card = GetFromPool();

            if (card == null)
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                card = go.AddComponent<Card3D>();
                card.BuildVisuals();
            }

            card.gameObject.SetActive(true);
            card.SetData(data);
            card.SetState(Card3DState.InHand);
            card.SetGlow(false);
            return card;
        }

        public void ReturnCard(Card3D card)
        {
            if (card == null) return;

            card.SetState(Card3DState.Discarded);
            card.SetGlow(false);
            card.gameObject.SetActive(false);
            card.transform.SetParent(transform);
            _pool.Add(card);
        }

        Card3D GetFromPool()
        {
            if (_pool.Count == 0) return null;

            var card = _pool[_pool.Count - 1];
            _pool.RemoveAt(_pool.Count - 1);
            return card;
        }
    }
}

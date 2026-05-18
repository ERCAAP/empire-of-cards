using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Gameplay;
using EmpireOfCards.UI.Cards;

namespace EmpireOfCards.World
{
    public class Board3D : MonoBehaviour
    {
        [SerializeField] private BoardManager boardManager;

        /// <summary>
        /// Assigns all dependencies without reflection.
        /// Called by WiringService instead of RuntimeWiring.SetField().
        /// </summary>
        public void Init(BoardManager board)
        {
            this.boardManager = board;
        }

        // Created at runtime
        private readonly List<SlotZone3D> _businessSlots = new List<SlotZone3D>();
        private readonly List<SlotZone3D> _employeeSlots = new List<SlotZone3D>();
        private readonly List<SlotZone3D> _upgradeSlots = new List<SlotZone3D>();
        private readonly List<GameObject> _territoryBlocks = new List<GameObject>();
        private readonly List<MeshRenderer> _territoryRenderers = new List<MeshRenderer>();
        private SlotZone3D _sellZone;
        private SlotZone3D _actionZone;

        public IReadOnlyList<SlotZone3D> BusinessSlots => _businessSlots;

        public void BuildBoard()
        {
            // === TABLE (large flat cube) ===
            var table = GameObject.CreatePrimitive(PrimitiveType.Cube);
            table.name = "Table";
            table.transform.SetParent(transform);
            table.transform.localPosition = new Vector3(0, -0.25f, 0);
            table.transform.localScale = new Vector3(14, 0.5f, 10);
            table.GetComponent<MeshRenderer>().material.color = new Color(0.35f, 0.22f, 0.12f); // Wood
            table.layer = LayerMask.NameToLayer("Default");
            Destroy(table.GetComponent<Collider>()); // Table doesn't need raycast

            // === TERRITORY MAP (10 cubes, top of board) ===
            for (int i = 0; i < 10; i++)
            {
                var terr = GameObject.CreatePrimitive(PrimitiveType.Cube);
                terr.name = $"Territory_{i + 1:D2}";
                terr.transform.SetParent(transform);
                float x = (i - 4.5f) * 1.1f;
                terr.transform.localPosition = new Vector3(x, 0.1f, 3.5f);
                terr.transform.localScale = new Vector3(0.9f, 0.2f, 0.9f);
                terr.GetComponent<MeshRenderer>().material.color = Color.gray;
                Destroy(terr.GetComponent<Collider>());
                _territoryBlocks.Add(terr);
                _territoryRenderers.Add(terr.GetComponent<MeshRenderer>());
            }

            // === PLAYER BUSINESS SLOTS (5 cubes, center-bottom) ===
            for (int i = 0; i < 5; i++)
            {
                var slot = GameObject.CreatePrimitive(PrimitiveType.Cube);
                slot.name = $"BusinessSlot_{i + 1:D2}";
                slot.transform.SetParent(transform);
                float x = (i - 2) * 2.5f;
                slot.transform.localPosition = new Vector3(x, 0.05f, -0.5f);
                slot.transform.localScale = new Vector3(1.8f, 0.1f, 2.5f);
                slot.GetComponent<MeshRenderer>().material.color = new Color(0.25f, 0.25f, 0.3f);

                var zone = slot.AddComponent<SlotZone3D>();
                zone.RuntimeInit(DropZoneType.BusinessSlot, i);
                _businessSlots.Add(zone);

                // Employee sub-slots (3 per business)
                for (int e = 0; e < 3; e++)
                {
                    var empSlot = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    empSlot.name = $"EmpSlot_{e + 1:D2}";
                    empSlot.transform.SetParent(slot.transform);
                    empSlot.transform.localPosition = new Vector3(0, 1f, -(e + 1) * 0.35f);
                    empSlot.transform.localScale = new Vector3(0.45f, 0.5f, 0.3f);
                    empSlot.GetComponent<MeshRenderer>().material.color = new Color(0.2f, 0.35f, 0.2f);

                    var empZone = empSlot.AddComponent<SlotZone3D>();
                    empZone.RuntimeInit(DropZoneType.EmployeeSlot, e, i);
                    _employeeSlots.Add(empZone);
                }

                // Hide slots 4-5 initially
                if (i >= 3) slot.SetActive(false);
            }

            // === RIVAL AREA (3 slots, top) ===
            for (int i = 0; i < 3; i++)
            {
                var rivalSlot = GameObject.CreatePrimitive(PrimitiveType.Cube);
                rivalSlot.name = $"RivalSlot_{i + 1:D2}";
                rivalSlot.transform.SetParent(transform);
                float x = (i - 1) * 2.8f;
                rivalSlot.transform.localPosition = new Vector3(x, 0.05f, 2f);
                rivalSlot.transform.localScale = new Vector3(1.8f, 0.1f, 1.5f);
                rivalSlot.GetComponent<MeshRenderer>().material.color = new Color(0.5f, 0.15f, 0.15f);
                Destroy(rivalSlot.GetComponent<Collider>()); // Read-only
            }

            // === UPGRADE AREA (left side) ===
            for (int i = 0; i < 4; i++)
            {
                var upSlot = GameObject.CreatePrimitive(PrimitiveType.Cube);
                upSlot.name = $"UpgradeSlot_{i + 1:D2}";
                upSlot.transform.SetParent(transform);
                upSlot.transform.localPosition = new Vector3(-5.5f, 0.05f, -1f + i * 1.2f);
                upSlot.transform.localScale = new Vector3(1.2f, 0.1f, 1f);
                upSlot.GetComponent<MeshRenderer>().material.color = new Color(0.35f, 0.2f, 0.45f);

                var upZone = upSlot.AddComponent<SlotZone3D>();
                upZone.RuntimeInit(DropZoneType.UpgradeSlot, i);
                _upgradeSlots.Add(upZone);
            }

            // === SELL ZONE (right side) ===
            var sell = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sell.name = "SellZone";
            sell.transform.SetParent(transform);
            sell.transform.localPosition = new Vector3(5.5f, 0.05f, -1f);
            sell.transform.localScale = new Vector3(1.5f, 0.1f, 1.5f);
            sell.GetComponent<MeshRenderer>().material.color = new Color(0.6f, 0.4f, 0.1f);
            _sellZone = sell.AddComponent<SlotZone3D>();
            _sellZone.RuntimeInit(DropZoneType.SellZone, 0);

            // === ACTION ZONE (center, for action cards) ===
            var action = GameObject.CreatePrimitive(PrimitiveType.Cube);
            action.name = "ActionZone";
            action.transform.SetParent(transform);
            action.transform.localPosition = new Vector3(5.5f, 0.05f, 1f);
            action.transform.localScale = new Vector3(1.5f, 0.1f, 1.5f);
            action.GetComponent<MeshRenderer>().material.color = new Color(0.6f, 0.15f, 0.15f);
            _actionZone = action.AddComponent<SlotZone3D>();
            _actionZone.RuntimeInit(DropZoneType.ActionZone, 0);

            // === EVENT DISPLAY (center-top) ===
            var eventArea = GameObject.CreatePrimitive(PrimitiveType.Cube);
            eventArea.name = "EventDisplay";
            eventArea.transform.SetParent(transform);
            eventArea.transform.localPosition = new Vector3(0, 0.15f, 2f);
            eventArea.transform.localScale = new Vector3(1.2f, 0.02f, 1.7f);
            eventArea.GetComponent<MeshRenderer>().material.color = new Color(0.9f, 0.8f, 0.15f, 0.3f);
            Destroy(eventArea.GetComponent<Collider>());
        }

        // Called by TerritoryManager to update visuals
        public void UpdateTerritoryVisuals(int playerCount, int rivalCount)
        {
            for (int i = 0; i < _territoryRenderers.Count; i++)
            {
                if (i < playerCount)
                    _territoryRenderers[i].material.color = new Color(0.2f, 0.5f, 1f); // Player blue
                else if (i < playerCount + rivalCount)
                    _territoryRenderers[i].material.color = new Color(0.9f, 0.2f, 0.2f); // Rival red
                else
                    _territoryRenderers[i].material.color = Color.gray; // Empty
            }
        }
    }
}

using NUnit.Framework;
using UnityEngine;
using EmpireOfCards.Bootstrap;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Tests.EditMode
{
    public class RunLaunchCoordinatorTests
    {
        [Test]
        public void FindVenture_ReturnsMatchingVenture()
        {
            VentureData cafe = ScriptableObject.CreateInstance<VentureData>();
            VentureData tech = ScriptableObject.CreateInstance<VentureData>();

            cafe.ventureType = VentureType.Cafe;
            tech.ventureType = VentureType.TechApp;

            VentureData result = RunLaunchCoordinator.FindVenture(new[] { cafe, tech }, VentureType.TechApp);

            Assert.That(result, Is.SameAs(tech));

            Object.DestroyImmediate(cafe);
            Object.DestroyImmediate(tech);
        }

        [Test]
        public void FindVenture_ReturnsNullWhenNoMatchExists()
        {
            VentureData cafe = ScriptableObject.CreateInstance<VentureData>();
            cafe.ventureType = VentureType.Cafe;

            VentureData result = RunLaunchCoordinator.FindVenture(new[] { cafe }, VentureType.FastFood);

            Assert.That(result, Is.Null);

            Object.DestroyImmediate(cafe);
        }
    }
}

using UnityEngine;
using System.Reflection;

namespace EmpireOfCards.Helpers
{
    // DEPRECATED: Use manager Init() methods instead of SetField reflection.
    // Each manager now has a public Init() that accepts its dependencies directly.
    // WiringService should call Init() on each manager instead of using SetField().
    /// <summary>
    /// Helper to set [SerializeField] private fields at runtime.
    /// Used by GameSceneBootstrap to wire manager references that
    /// cannot be assigned through the Inspector when components
    /// are created via AddComponent at runtime.
    /// </summary>
    [System.Obsolete("Use manager Init() methods instead of SetField reflection.")]
    public static class RuntimeWiring
    {
        /// <summary>
        /// Sets a private or public field on the target object by name.
        /// Searches NonPublic and Public instance fields.
        /// </summary>
        public static void SetField(object target, string fieldName, object value)
        {
            if (target == null)
            {
                Debug.LogWarning("[RuntimeWiring] Target is null.");
                return;
            }

            var type = target.GetType();
            var field = type.GetField(fieldName,
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

            if (field != null)
            {
                field.SetValue(target, value);
            }
            else
            {
                Debug.LogWarning($"[RuntimeWiring] Field '{fieldName}' not found on {type.Name}");
            }
        }
    }
}

using UnityEngine;

namespace EmpireOfCards.World
{
    /// <summary>
    /// A 3D arrow built from primitives that bobs up and down to draw
    /// attention to a target position on the board. Used by TutorialManager
    /// to point at territory blocks, hand area, business slots, etc.
    /// </summary>
    public class TutorialArrow3D : MonoBehaviour
    {
        private Transform _shaft;
        private Transform _head;
        private float _bobSpeed = 3f;
        private float _bobAmount = 0.2f;
        private Vector3 _basePos;
        private Material _material;

        // Pulse state
        private float _pulseSpeed = 2f;
        private float _pulseMin = 0.6f;
        private float _pulseMax = 1f;
        private Color _baseColor;

        /// <summary>
        /// Creates a 3D arrow at the given world position, pointing in
        /// the specified direction. Built from two cubes (shaft + head).
        /// Emissive material so it stands out in any lighting.
        /// </summary>
        public static TutorialArrow3D Create(Vector3 position, Vector3 pointDirection, Color color)
        {
            var go = new GameObject("TutorialArrow3D");
            var arrow = go.AddComponent<TutorialArrow3D>();

            go.transform.position = position;

            // --- Arrow shaft (elongated thin cube) ---
            var shaft = GameObject.CreatePrimitive(PrimitiveType.Cube);
            shaft.name = "Shaft";
            shaft.transform.SetParent(go.transform);
            shaft.transform.localScale = new Vector3(0.08f, 0.5f, 0.08f);
            shaft.transform.localPosition = Vector3.zero;
            var shaftCollider = shaft.GetComponent<Collider>();
            if (shaftCollider != null) Object.Destroy(shaftCollider);
            arrow._shaft = shaft.transform;

            // --- Arrow head (wider flat cube at the tip) ---
            var head = GameObject.CreatePrimitive(PrimitiveType.Cube);
            head.name = "Head";
            head.transform.SetParent(go.transform);
            head.transform.localScale = new Vector3(0.22f, 0.15f, 0.22f);
            head.transform.localPosition = new Vector3(0, -0.35f, 0);
            // Rotate 45 degrees for a diamond/arrowhead look
            head.transform.localRotation = Quaternion.Euler(0, 45f, 0);
            var headCollider = head.GetComponent<Collider>();
            if (headCollider != null) Object.Destroy(headCollider);
            arrow._head = head.transform;

            // --- Orient the arrow so the head points in pointDirection ---
            // The arrow's local "down" (-Y) is the pointing direction.
            // LookRotation needs a forward and up. We set up = -pointDirection so
            // the arrow's -Y axis aligns with pointDirection.
            if (pointDirection != Vector3.zero)
            {
                go.transform.rotation = Quaternion.LookRotation(Vector3.forward, -pointDirection.normalized);
            }

            // --- Emissive material ---
            var mat = CreateEmissiveMaterial(color);
            shaft.GetComponent<MeshRenderer>().material = mat;
            head.GetComponent<MeshRenderer>().material = mat;
            arrow._material = mat;
            arrow._baseColor = color;

            arrow._basePos = position;

            return arrow;
        }

        /// <summary>
        /// Creates a second factory variant that takes a start position and a
        /// target position. The arrow is placed midway between them, pointing
        /// from start toward target.
        /// </summary>
        public static TutorialArrow3D CreateBetween(Vector3 from, Vector3 to, Color color)
        {
            Vector3 midpoint = (from + to) * 0.5f + Vector3.up * 0.5f;
            Vector3 direction = (to - from).normalized;
            return Create(midpoint, direction, color);
        }

        // ------------------------------------------------------------------
        //  Update -- bob + pulse
        // ------------------------------------------------------------------

        private void Update()
        {
            // Bob up and down along the arrow's local up axis
            float offset = Mathf.Sin(Time.time * _bobSpeed) * _bobAmount;
            transform.position = _basePos + transform.up * offset;

            // Gentle emissive pulse
            if (_material != null)
            {
                float pulse = Mathf.Lerp(_pulseMin, _pulseMax,
                    (Mathf.Sin(Time.time * _pulseSpeed) + 1f) * 0.5f);
                _material.SetColor("_EmissionColor", _baseColor * pulse * 2f);
            }
        }

        // ------------------------------------------------------------------
        //  Public API
        // ------------------------------------------------------------------

        /// <summary>
        /// Moves the arrow to a new base position.
        /// </summary>
        public void SetTarget(Vector3 newPos)
        {
            _basePos = newPos;
            transform.position = newPos;
        }

        /// <summary>
        /// Changes the pointing direction without moving the arrow.
        /// </summary>
        public void SetDirection(Vector3 pointDirection)
        {
            if (pointDirection != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(Vector3.forward, -pointDirection.normalized);
        }

        /// <summary>
        /// Changes the arrow color at runtime.
        /// </summary>
        public void SetColor(Color color)
        {
            _baseColor = color;
            if (_material != null)
            {
                _material.color = color;
                _material.SetColor("_EmissionColor", color * 2f);
            }
        }

        /// <summary>
        /// Destroys this arrow and its GameObjects.
        /// </summary>
        public void DestroyArrow()
        {
            if (_material != null)
                Object.Destroy(_material);

            Object.Destroy(gameObject);
        }

        // ------------------------------------------------------------------
        //  Helpers
        // ------------------------------------------------------------------

        private static Material CreateEmissiveMaterial(Color color)
        {
            // Try URP lit shader first, fall back to Standard
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
                shader = Shader.Find("Standard");

            var mat = new Material(shader);
            mat.color = color;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", color * 2f);
            return mat;
        }
    }
}

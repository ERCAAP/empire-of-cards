using UnityEngine;

namespace EmpireOfCards.Helpers
{
    /// <summary>
    /// Creates colored materials that work with whatever render pipeline is active.
    /// Extracts the shader from Unity's own default primitive material, guaranteeing compatibility.
    /// </summary>
    public static class MaterialHelper
    {
        static Shader _cachedShader;

        static Shader GetShader()
        {
            if (_cachedShader != null) return _cachedShader;

            // Create a temp cube, grab its shader (always matches active pipeline), destroy
            var temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _cachedShader = temp.GetComponent<Renderer>().sharedMaterial.shader;
            Object.DestroyImmediate(temp);
            return _cachedShader;
        }

        public static Material Create(Color color)
        {
            var mat = new Material(GetShader());
            mat.color = color;
            return mat;
        }

        public static Material CreateEmissive(Color color, Color emissionColor, float intensity = 1f)
        {
            var mat = Create(color);
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", emissionColor * intensity);
            return mat;
        }

        public static Material CreateTransparent(Color color)
        {
            var mat = Create(color);
            mat.SetFloat("_Surface", 1); // URP transparent
            mat.SetFloat("_Blend", 0);
            mat.SetOverrideTag("RenderType", "Transparent");
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.renderQueue = 3000;
            return mat;
        }
    }
}

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using TMPro;
using System.IO;

namespace EmpireOfCards.Editor
{
    /// <summary>
    /// Generates TMP SDF Font Assets from .ttf files in _Project/Fonts/.
    /// Menu: EmpireOfCards > Generate Font Assets
    ///
    /// Font assignments for Empire of Cards:
    ///   Nunito          — General UI (menus, buttons, tooltips, card descriptions)
    ///   Oswald          — Card titles (UPPERCASE, condensed, strong)
    ///   Bangers         — Combo/effect text ("COMBO!", "FBI RAID!", "YOU WIN!")
    ///   Space Mono      — Numbers/stats (money counter, turn counter, scores)
    /// </summary>
    public static class FontAssetGenerator
    {
        private const string FontSourceDir = "Assets/_Project/Fonts";
        private const string FontOutputDir = "Assets/_Project/Fonts/TMP_Assets";

        // Character set: ASCII + Latin Extended (covers all Western European languages)
        private const string CharacterSet =
            "!\"#$%&'()*+,-./0123456789:;<=>?@" +
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`" +
            "abcdefghijklmnopqrstuvwxyz{|}~" +
            " \u00A0\u00A1\u00A2\u00A3\u00A4\u00A5\u00A6\u00A7\u00A8\u00A9\u00AA\u00AB\u00AC\u00AD\u00AE\u00AF" +
            "\u00B0\u00B1\u00B2\u00B3\u00B4\u00B5\u00B6\u00B7\u00B8\u00B9\u00BA\u00BB\u00BC\u00BD\u00BE\u00BF" +
            "\u00C0\u00C1\u00C2\u00C3\u00C4\u00C5\u00C6\u00C7\u00C8\u00C9\u00CA\u00CB\u00CC\u00CD\u00CE\u00CF" +
            "\u00D0\u00D1\u00D2\u00D3\u00D4\u00D5\u00D6\u00D7\u00D8\u00D9\u00DA\u00DB\u00DC\u00DD\u00DE\u00DF" +
            "\u00E0\u00E1\u00E2\u00E3\u00E4\u00E5\u00E6\u00E7\u00E8\u00E9\u00EA\u00EB\u00EC\u00ED\u00EE\u00EF" +
            "\u00F0\u00F1\u00F2\u00F3\u00F4\u00F5\u00F6\u00F7\u00F8\u00F9\u00FA\u00FB\u00FC\u00FD\u00FE\u00FF" +
            // Turkish specific
            "\u011E\u011F\u0130\u0131\u015E\u015F" +
            // Common currency & symbols
            "\u20AC\u00A3\u00A5\u2022\u2026\u2013\u2014\u2018\u2019\u201C\u201D\u2020\u2021\u2122\u00AE\u00A9";

        [MenuItem("EmpireOfCards/Generate Font Assets")]
        public static void GenerateAllFontAssets()
        {
            if (!AssetDatabase.IsValidFolder(FontOutputDir))
            {
                AssetDatabase.CreateFolder(FontSourceDir, "TMP_Assets");
            }

            string[] ttfFiles = Directory.GetFiles(
                Path.GetFullPath(FontSourceDir), "*.ttf", SearchOption.TopDirectoryOnly);

            if (ttfFiles.Length == 0)
            {
                Debug.LogError("[FontAssetGenerator] No .ttf files found in " + FontSourceDir);
                return;
            }

            int created = 0;
            foreach (string fullPath in ttfFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(fullPath);
                string assetPath = FontSourceDir + "/" + Path.GetFileName(fullPath);
                string outputPath = FontOutputDir + "/" + fileName + " SDF.asset";

                // Skip if already exists
                if (AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(outputPath) != null)
                {
                    Debug.Log($"[FontAssetGenerator] Skipping {fileName} (already exists)");
                    continue;
                }

                Font sourceFont = AssetDatabase.LoadAssetAtPath<Font>(assetPath);
                if (sourceFont == null)
                {
                    Debug.LogWarning($"[FontAssetGenerator] Could not load font: {assetPath}");
                    continue;
                }

                // Determine atlas size based on font
                int atlasSize = GetAtlasSize(fileName);
                int samplingSize = GetSamplingSize(fileName);

                TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(
                    sourceFont, samplingSize, 5, GlyphRenderMode.SDFAA, atlasSize, atlasSize);

                if (fontAsset == null)
                {
                    Debug.LogError($"[FontAssetGenerator] Failed to create asset for {fileName}");
                    continue;
                }

                AssetDatabase.CreateAsset(fontAsset, outputPath);

                // Save material as sub-asset
                if (fontAsset.material != null)
                {
                    fontAsset.material.name = fileName + " SDF Material";
                }

                EditorUtility.SetDirty(fontAsset);
                created++;
                Debug.Log($"[FontAssetGenerator] Created: {outputPath}");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[FontAssetGenerator] Done! {created} font assets created in {FontOutputDir}");
            Debug.Log("[FontAssetGenerator] Font guide:");
            Debug.Log("  Nunito (Regular/Bold/ExtraBold)  -> UI text, menus, tooltips");
            Debug.Log("  Oswald (Medium/Bold)             -> Card titles (UPPERCASE)");
            Debug.Log("  Bangers                          -> Combo popups, effects");
            Debug.Log("  SpaceMono (Regular/Bold)         -> Money, stats, counters");
        }

        private static int GetAtlasSize(string fontName)
        {
            // Bangers is a display font with fewer glyphs, smaller atlas is fine
            if (fontName.Contains("Bangers")) return 1024;
            // Mono fonts need precision
            if (fontName.Contains("SpaceMono")) return 2048;
            // Default
            return 2048;
        }

        private static int GetSamplingSize(string fontName)
        {
            // Display/title fonts benefit from larger sampling
            if (fontName.Contains("Bangers")) return 64;
            if (fontName.Contains("Oswald")) return 48;
            // Default for body text
            return 42;
        }
    }
}
#endif

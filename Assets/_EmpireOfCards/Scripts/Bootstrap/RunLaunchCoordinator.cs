using System;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.Save;
using EmpireOfCards.World;
using UnityEngine;

namespace EmpireOfCards.Bootstrap
{
    public sealed class RunLaunchCoordinator
    {
        private readonly GameManager _gameManager;
        private readonly SaveManager _saveManager;
        private readonly Board3D _board3D;
        private readonly Camera _mainCamera;
        private readonly Hand3D _hand3D;

        public RunLaunchCoordinator(GameManager gameManager, SaveManager saveManager, Board3D board3D, Camera mainCamera, Hand3D hand3D)
        {
            _gameManager = gameManager;
            _saveManager = saveManager;
            _board3D = board3D;
            _mainCamera = mainCamera;
            _hand3D = hand3D;
        }

        public bool StartNewRun(VentureData venture, string runName, TechCategoryProfile techCategory, TutorialManager tutorialManager)
        {
            if (_gameManager == null)
                return false;

            if (venture != null)
                _gameManager.SetSelectedVenture(venture);
            ApplyPresentationProfile(venture);
            if (!string.IsNullOrWhiteSpace(runName))
                _gameManager.SetRunDisplayName(runName);

            _gameManager.SetRunCategory(
                techCategory != null ? techCategory.categoryId : null,
                techCategory != null ? ResolveTechCategoryName(techCategory) : null);
            _gameManager.SetCurrentRunSlotId(CreateRunSlotId());
            _gameManager.StartNewRun();
            tutorialManager?.TryStartTutorial();
            return true;
        }

        public bool TryRestoreSavedRun(string slotId, VentureData[] ventures)
        {
            if (_gameManager == null || _saveManager == null || !_saveManager.HasRunSave(slotId))
                return false;

            RunSaveData run = _saveManager.LoadRun(slotId);
            if (run == null || !LaunchVentureScope.IsEnabled((VentureType)run.ventureType))
                return false;

            VentureData venture = FindVenture(ventures, (VentureType)run.ventureType);
            if (venture == null)
                return false;

            _gameManager.SetSelectedVenture(venture);
            ApplyPresentationProfile(venture);
            _gameManager.SetCurrentRunSlotId(run.slotId);
            _gameManager.SetRunDisplayName(run.runName);
            _gameManager.SetRunCategory(run.runCategoryId, run.runCategoryLabel);
            _gameManager.RestoreRunCheckpoint(run);
            _board3D?.RefreshSlotOccupancyVisuals();
            return true;
        }

        public string CreateRunSlotId()
        {
            return _saveManager != null
                ? _saveManager.CreateRunSlotId()
                : Guid.NewGuid().ToString("N");
        }

        public static VentureData FindVenture(VentureData[] ventures, VentureType type)
        {
            if (ventures == null)
                return null;

            for (int i = 0; i < ventures.Length; i++)
            {
                var venture = ventures[i];
                if (venture != null && venture.ventureType == type)
                    return venture;
            }

            return null;
        }

        private static string ResolveTechCategoryName(TechCategoryProfile profile)
        {
            return LocalizationManager.GetWithFallback(profile.labelKey, profile.displayName);
        }

        private void ApplyPresentationProfile(VentureData venture)
        {
            BoardCameraProfile profile = venture != null && venture.themeProfile != null
                ? venture.themeProfile.cameraProfile
                : null;
            SceneRuntimeFactory.ApplyCameraProfile(_mainCamera, _hand3D, profile);
        }
    }
}

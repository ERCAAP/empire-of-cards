using EmpireOfCards.Data;

namespace EmpireOfCards.Bootstrap
{
    public partial class GameSceneBootstrap
    {
        private void StartSceneFlow()
        {
            if (RunLaunchConfig.LaunchMode == RunLaunchMode.LoadRun)
            {
                TryRestoreConfiguredRun();
                return;
            }

            if (_ventureSelectionUI != null && _ventures != null && _ventures.Length > 0)
            {
                _ventureSelectionUI.Init(_ventures);
                _ventureSelectionUI.OnVentureSelected += OnVentureChosen;
                _ventureSelectionUI.Show();
                return;
            }

            StartConfiguredRun(null);
        }

        private void HandleVentureChosen(VentureData venture)
        {
            _ventureSelectionUI.OnVentureSelected -= OnVentureChosen;
            _pendingRunName = null;
            _pendingTechCategory = null;
            ShowRunNamePrompt(venture);
        }

        private void StartConfiguredRun(VentureData venture)
        {
            _runLaunchCoordinator?.StartNewRun(venture, _pendingRunName, _pendingTechCategory, _tutorialManager);
            _pendingRunName = null;
            _pendingTechCategory = null;
        }

        private void TryRestoreConfiguredRun()
        {
            if (_runLaunchCoordinator == null || !_runLaunchCoordinator.TryRestoreSavedRun(RunLaunchConfig.SelectedRunSlotId, _ventures))
            {
                RunLaunchConfig.PrepareNewRun();
                StartConfiguredRun(null);
            }
        }
    }
}

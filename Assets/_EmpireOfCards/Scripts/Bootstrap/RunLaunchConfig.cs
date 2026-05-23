namespace EmpireOfCards.Bootstrap
{
    public enum RunLaunchMode
    {
        NewRun,
        LoadRun
    }

    public static class RunLaunchConfig
    {
        public static RunLaunchMode LaunchMode { get; private set; } = RunLaunchMode.NewRun;
        public static string SelectedRunSlotId { get; private set; }

        public static void PrepareNewRun()
        {
            LaunchMode = RunLaunchMode.NewRun;
            SelectedRunSlotId = null;
        }

        public static void PrepareLoadRun(string slotId)
        {
            LaunchMode = RunLaunchMode.LoadRun;
            SelectedRunSlotId = slotId;
        }
    }
}

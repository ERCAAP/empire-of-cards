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

        public static void PrepareNewRun()
        {
            LaunchMode = RunLaunchMode.NewRun;
        }

        public static void PrepareLoadRun()
        {
            LaunchMode = RunLaunchMode.LoadRun;
        }
    }
}

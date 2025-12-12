namespace api.BusinessLogic
{
    public class BllModeManager
    {
        public BllMode CurrentBllMode { get; private set; } = BllMode.Linq; // Default to LINQ
        // public BllMode CurrentBllMode { get; private set; } = BllMode.Sproc; // Default to SProc (for testing)

        public void SetBllMode(BllMode mode)
        {
            CurrentBllMode = mode;
        }
    }
}

namespace api.BusinessLogic
{
    public class BllModeManager
    {
        public BllMode CurrentBllMode { get; private set; } = BllMode.Linq; // Default to LINQ

        public void SetBllMode(BllMode mode)
        {
            CurrentBllMode = mode;
        }
    }
}

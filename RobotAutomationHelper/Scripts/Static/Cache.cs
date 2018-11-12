namespace RobotAutomationHelper.Scripts.Static
{
    internal static class Cache
    {
        internal static void ClearCache()
        {
            FilesAndFolderStructure.CleanUp();
            SuggestionsClass.CleanUp();
        }
    }
}

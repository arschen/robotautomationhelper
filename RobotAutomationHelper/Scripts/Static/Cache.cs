namespace RobotAutomationHelper.Scripts.Static
{
    public static class Cache
    {
        public static void ClearCache()
        {
            FilesAndFolderStructure.CleanUp();
            SuggestionsClass.CleanUp();
        }
    }
}

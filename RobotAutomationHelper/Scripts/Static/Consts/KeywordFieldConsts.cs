namespace RobotAutomationHelper.Scripts.Static.Consts
{
    public class KeywordFieldConsts
    {
        public const int FieldsHeight = 20;
        public const int HorizontalDistanceBetweenKeywords = 10;
        public const int VerticalDistanceBetweenKeywords = 30;

        public const int LabelX = 10;
        public const int LabelWidth = 20;
        public const int SettingsLabelWidth = 80;

        public const int NameX = LabelWidth + LabelX;
        public const int NameWidth = 280;

        public const int AddImplementationX = NameX + NameWidth + HorizontalDistanceBetweenKeywords;
        public const int AddImplementationWidth = 120;

        public const int AddKeywordX = AddImplementationX + AddImplementationWidth + HorizontalDistanceBetweenKeywords;
        public const int AddKeywordWidth = 20;

        public const int RemoveKeywordX = AddKeywordX + AddKeywordWidth + HorizontalDistanceBetweenKeywords;
        public const int RemoveKeywordWidth = 20;

        public const int ParamX = RemoveKeywordX + RemoveKeywordWidth + HorizontalDistanceBetweenKeywords;
        public const int ParamWidth = 75;

        public static readonly string[] LabelNames = { "Test Setup", "Test Teardown", "Suite Setup", "Suite Teardown" };
    }
}

namespace RobotAutomationHelper.Scripts.Static.Consts
{
    internal class KeywordFieldConsts
    {
        internal const int FieldsHeight = 20;
        internal const int HorizontalDistanceBetweenKeywords = 10;
        internal const int VerticalDistanceBetweenKeywords = 30;

        internal const int LabelX = 10;
        internal const int LabelWidth = 20;
        internal const int SettingsLabelWidth = 80;

        internal const int NameX = LabelWidth + LabelX;
        internal const int NameWidth = 280;

        internal const int AddImplementationX = NameX + NameWidth + HorizontalDistanceBetweenKeywords;
        internal const int AddImplementationWidth = 120;

        internal const int AddKeywordX = AddImplementationX + AddImplementationWidth + HorizontalDistanceBetweenKeywords;
        internal const int AddKeywordWidth = 20;

        internal const int RemoveKeywordX = AddKeywordX + AddKeywordWidth + HorizontalDistanceBetweenKeywords;
        internal const int RemoveKeywordWidth = 20;

        internal const int ParamX = RemoveKeywordX + RemoveKeywordWidth + HorizontalDistanceBetweenKeywords;
        internal const int ParamWidth = 75;

        internal static readonly string[] LabelNames = { "Test Setup", "Test Teardown", "Suite Setup", "Suite Teardown" };
    }
}

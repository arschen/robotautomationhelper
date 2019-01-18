using System;
using System.Collections.Generic;
using System.Drawing;
using RobotAutomationHelper.Scripts.CustomControls;
using RobotAutomationHelper.Scripts.Objects;
using RobotAutomationHelper.Scripts.Static;

namespace RobotAutomationHelper.Forms
{
    public partial class ParamAddForm : BaseKeywordAddForm
    {
        private Keyword _кeyword;
        public int ParamsCount;

        public ParamAddForm(BaseKeywordAddForm parent) : base(parent)
        {
            InitializeComponent();
            FormType = FormType.Params;
        }

        public void ShowParamContent(Keyword keyword)
        {
            _кeyword = keyword;
            ThisFormKeywords = new List<Keyword>();
            ThisFormKeywords = keyword.ForLoopKeywords;

            // set Keyword Name and Documentation field text
            if (keyword.Name != null)
                KeywordName.Text = keyword.Name.Trim();

            if (keyword.Documentation != null)
                KeywordDocumentation.Text = keyword.Documentation.Replace("[Documentation]", "").Trim();

            // adds args and paramList to lists for dynamically adding them to fields
            var paramsList = new List<Param>();
            if (keyword.Params != null)
                paramsList.AddRange(keyword.Params);

            //List<string> args = StringAndListOperations.ReturnListOfArgs(Кeyword.Arguments);

            if (paramsList.Count != 0)
                foreach (var param in paramsList)
                {
                    ParamsCount++;
                    FormControls.AddControl("LabelWithToolTip", "DynamicStep" + ParamsCount + "ParamName",
                        ParamsCount,
                        new Point(10 - HorizontalScroll.Value, 123 + (ParamsCount - 1) * 30 - VerticalScroll.Value),
                        new Size(80, 20),
                        param.Name,
                        Color.Black,
                        null,
                        this);

                    FormControls.AddControl("TextBox", "DynamicStep" + ParamsCount + "ParamValue",
                        ParamsCount,
                        new Point(100 - HorizontalScroll.Value, 120 + (ParamsCount - 1) * 30 - VerticalScroll.Value),
                        new Size(280, 20),
                        paramsList[ParamsCount - 1].Value,
                        Color.Black,
                        null,
                        this);
                }

            InitialYValue = 120 + ParamsCount * 30;

            if (keyword.Type.Equals(KeywordType.ForLoopElements) || keyword.Type.Equals(KeywordType.ForLoopInRange))
            {
                if (ThisFormKeywords != null && ThisFormKeywords.Count != 0)
                {
                    // adds the keywords in the form
                    foreach (var steps in ThisFormKeywords)
                    {
                        AddKeywordField(steps, NumberOfKeywordsInThisForm + 1, false, true);
                        NumberOfKeywordsInThisForm++;
                    }
                }
                else
                {
                    // add a single Кeyword field if no keywords are available
                    ThisFormKeywords = new List<Keyword>
                    {
                        new Keyword("New Keyword", FilesAndFolderStructure.GetFolder(FolderType.Resources) + "Auto.robot", keyword.Parent)
                    };
                    AddKeywordField(ThisFormKeywords[0], NumberOfKeywordsInThisForm + 1, true, true);
                    NumberOfKeywordsInThisForm++;
                    FilesAndFolderStructure.AddFileToSavedFiles(ThisFormKeywords[0].OutputFilePath);
                }
            }

            ShowDialog();
        }

        public void UpdateListNamesAndUpdateStateOfSave()
        {
            var namesList = new List<string>();
            for (var i = 1; i <= NumberOfKeywordsInThisForm; i++)
            {
                if (Controls.Find("DynamicStep" + i + "Name", false).Length > 0)
                    namesList.Add(Controls["DynamicStep" + i + "Name"].Text);
            }
            ((ButtonWithToolTip) Save).UpdateState(namesList, FilesAndFolderStructure.GetFolder(FolderType.Resources) + "Auto.robot");
        }

        private void Save_Click(object sender, EventArgs e)
        {
            var formParams = new List<Param>();
            for (var i = 1; i <= ParamsCount; i++)
                formParams.Add(new Param(Controls["DynamicStep" + i + "ParamName"].Text, Controls["DynamicStep" + i + "ParamValue"].Text));

            if (_кeyword.Type.Equals(KeywordType.ForLoopElements) || _кeyword.Type.Equals(KeywordType.ForLoopInRange))
            {
                if (ThisFormKeywords != null && ThisFormKeywords.Count != 0)
                {
                    _кeyword.ForLoopKeywords = new List<Keyword>();
                    foreach (var step in ThisFormKeywords)
                    {
                        var temp = new Keyword(step.Parent);
                        temp.CopyKeyword(step);
                        _кeyword.ForLoopKeywords.Add(temp);
                    }
                }
            }

            _кeyword.Params = formParams;
            Close();
        }

        private void Skip_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

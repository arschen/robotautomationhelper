using RobotAutomationHelper.Scripts;
using System;
using System.Collections.Generic;

namespace RobotAutomationHelper.Forms
{
    internal partial class ParamAddForm : BaseKeywordAddForm
    {
        private Keyword keyword;
        internal int paramsCount = 0;

        internal ParamAddForm(BaseKeywordAddForm parent) : base(parent)
        {
            InitializeComponent();
            FormType = FormType.Params;
        }

        internal void ShowParamContent(Keyword keyword)
        {
            this.keyword = keyword;
            ThisFormKeywords = new List<Keyword>();
            ThisFormKeywords = keyword.ForLoopKeywords;

            // set Keyword Name and Documentation field text
            if (keyword.Name != null)
                KeywordName.Text = keyword.Name.Trim();

            if (keyword.Documentation != null)
                KeywordDocumentation.Text = keyword.Documentation.Replace("[Documentation]", "").Trim();

            // adds args and paramList to lists for dynamicly adding them to fields
            List<Param> paramsList = new List<Param>();
            if (keyword.Params != null)
                paramsList.AddRange(keyword.Params);

            //List<string> args = StringAndListOperations.ReturnListOfArgs(keyword.Arguments);

            if (paramsList != null && paramsList.Count != 0)
                foreach (Param param in paramsList)
                {
                    paramsCount++;
                    FormControls.AddControl("LabelWithToolTip", "DynamicStep" + paramsCount + "ParamName",
                        paramsCount,
                        new System.Drawing.Point(10 - this.HorizontalScroll.Value, 123 + (paramsCount - 1) * 30 - this.VerticalScroll.Value),
                        new System.Drawing.Size(80, 20),
                        param.Name,
                        System.Drawing.Color.Black,
                        null,
                        this);

                    FormControls.AddControl("TextBox", "DynamicStep" + paramsCount + "ParamValue",
                        paramsCount,
                        new System.Drawing.Point(100 - this.HorizontalScroll.Value, 120 + (paramsCount - 1) * 30 - this.VerticalScroll.Value),
                        new System.Drawing.Size(280, 20),
                        paramsList[paramsCount - 1].Value,
                        System.Drawing.Color.Black,
                        null,
                        this);
                }

            initialYValue = 120 + paramsCount * 30;

            if (keyword.Type.Equals(KeywordType.FOR_LOOP_ELEMENTS) || keyword.Type.Equals(KeywordType.FOR_LOOP_IN_RANGE))
            {
                if (ThisFormKeywords != null && ThisFormKeywords.Count != 0)
                {
                    // adds the keywords in the form
                    foreach (Keyword steps in ThisFormKeywords)
                    {
                        AddKeywordField(steps, NumberOfKeywordsInThisForm + 1);
                        NumberOfKeywordsInThisForm++;
                    }
                }
                else
                {
                    // add a single keyword field if no keywords are available
                    ThisFormKeywords = new List<Keyword>
                    {
                        new Keyword("New Keyword", FilesAndFolderStructure.GetFolder(FolderType.Resources) + "Auto.robot")
                    };
                    AddKeywordField(ThisFormKeywords[0], NumberOfKeywordsInThisForm + 1);
                    NumberOfKeywordsInThisForm++;
                    FilesAndFolderStructure.AddFileToSavedFiles(ThisFormKeywords[0].OutputFilePath);
                }
            }

            var dialogResult = this.ShowDialog();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            List<Param> formParams = new List<Param>();
            for (int i = 1; i <= paramsCount; i++)
                formParams.Add(new Param(this.Controls["DynamicStep" + i + "ParamName"].Text, this.Controls["DynamicStep" + i + "ParamValue"].Text));

            keyword.Params = formParams;
            this.Close();
        }

        private void Skip_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

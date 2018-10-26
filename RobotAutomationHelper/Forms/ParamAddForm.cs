using RobotAutomationHelper.Scripts;
using System;
using System.Collections.Generic;

namespace RobotAutomationHelper.Forms
{
    internal partial class ParamAddForm : BaseKeywordAddForm
    {
        private Keyword keyword;
        private int paramsCount = 0;

        internal ParamAddForm()
        {
            InitializeComponent();
        }

        internal void ShowParamContent(Keyword keyword)
        {
            this.keyword = keyword;

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
                    FormControls.AddControl("LabelWithToolTip", "DynamicStep" + paramsCount + "Name",
                        paramsCount,
                        new System.Drawing.Point(10 - this.HorizontalScroll.Value, 123 + (paramsCount - 1) * 30 - this.VerticalScroll.Value),
                        new System.Drawing.Size(80, 20),
                        param.Name,
                        System.Drawing.Color.Black,
                        null,
                        this);

                    FormControls.AddControl("TextBox", "DynamicStep" + paramsCount + "Value",
                        paramsCount,
                        new System.Drawing.Point(100 - this.HorizontalScroll.Value, 120 + (paramsCount - 1) * 30 - this.VerticalScroll.Value),
                        new System.Drawing.Size(280, 20),
                        paramsList[paramsCount - 1].Value,
                        System.Drawing.Color.Black,
                        null,
                        this);
                }

            var dialogResult = this.ShowDialog();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            List<Param> formParams = new List<Param>();
            for (int i = 1; i <= paramsCount; i++)
                formParams.Add(new Param(this.Controls["DynamicStep" + i + "Name"].Text, this.Controls["DynamicStep" + i + "Value"].Text));

            keyword.Params = formParams;
            this.Close();
        }

        private void Skip_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

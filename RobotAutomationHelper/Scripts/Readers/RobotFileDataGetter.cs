using RobotAutomationHelper.Scripts.Objects;
using RobotAutomationHelper.Scripts.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotAutomationHelper.Scripts.Readers
{
    public class RobotFileDataGetter
    {
        List<RobotFile> RobotFiles = new List<RobotFile>();

        // returns the index of the specific tag - keyword / test cases / settings / variables
        public void GetTheDataFromAllTheRobotFiles()
        {
            var testCases = new List<TestCase>();
            var listOfVariables = new List<Variables>();
            foreach (var fileName in FilesAndFolderStructure.GetFullSavedFiles(FolderType.Root))
                RobotFiles.Add(new RobotFile(fileName));

            foreach (RobotFile currentFile in RobotFiles)
                if (currentFile.TestCasesList != null && currentFile.TestCasesList.Count > 0)
                    foreach (TestCase currentTestCase in currentFile.TestCasesList)
                    {
                        var tempTestCase = new TestCase(currentTestCase.Name, currentFile.fileName)
                        {
                            Steps = new List<Keyword>()
                        };
                        if (currentTestCase.Steps != null && currentTestCase.Steps.Count > 0)
                            foreach (Keyword currentKeyword in currentTestCase.Steps)
                            {
                                var tempKeyword = new Keyword(null);
                                if (currentKeyword.Type == KeywordType.Custom)
                                {
                                    tempKeyword.CopyKeyword(currentKeyword);
                                    if (currentKeyword.Params != null && currentKeyword.Params.Count > 0)
                                        if (currentKeyword.ImportFileName != null && !currentKeyword.ImportFileName.Equals(""))
                                            foreach (RobotFile tempFile in RobotFiles)
                                                if (tempFile.fileName.EndsWith("\\" + currentKeyword.ImportFileName + ".robot"))
                                                {
                                                    foreach (Keyword importKeyword in tempFile.KeywordsList)
                                                        if (importKeyword.Name.Equals(currentKeyword.Name))
                                                        {
                                                            for (int i = 0; i < currentKeyword.Params.Count; i++)
                                                                tempKeyword.Params[i].Name = importKeyword.Params[i].Name;
                                                            break;
                                                        }
                                                    break;
                                                }

                                    if (tempKeyword.Keywords != null && tempKeyword.Keywords.Count > 0)
                                        GetDataForInternalKeywords(tempKeyword);
                                }
                                else
                                    tempKeyword.CopyKeyword(currentKeyword);
                                tempTestCase.Steps.Add(tempKeyword);
                            }
                        testCases.Add(tempTestCase);
                    }
        }

        public void GetDataForInternalKeywords(Keyword keyword)
        {
            foreach (Keyword currentKeyword in keyword.Keywords)
            {
                var tempKeyword = new Keyword(keyword);
                if (currentKeyword.Type == KeywordType.Custom)
                {
                    tempKeyword.CopyKeyword(currentKeyword);
                    if (currentKeyword.Params != null && currentKeyword.Params.Count > 0)
                        if (currentKeyword.ImportFileName != null && !currentKeyword.ImportFileName.Equals(""))
                            foreach (RobotFile tempFile in RobotFiles)
                                if (tempFile.fileName.EndsWith("\\" + currentKeyword.ImportFileName + ".robot"))
                                {
                                    foreach (Keyword importKeyword in tempFile.KeywordsList)
                                        if (importKeyword.Name.Equals(currentKeyword.Name))
                                        {
                                            for (int i = 0; i < currentKeyword.Params.Count; i++)
                                                tempKeyword.Params[i].Name = importKeyword.Params[i].Name;
                                            tempKeyword.Keywords.AddRange(importKeyword.Keywords);
                                            break;
                                        }
                                    break;
                                }

                    if (tempKeyword.Keywords != null && tempKeyword.Keywords.Count > 0)
                        GetDataForInternalKeywords(tempKeyword);
                }
                else
                    tempKeyword.CopyKeyword(currentKeyword);
                currentKeyword.Keywords.Add(tempKeyword);
            }
        }
    }
}

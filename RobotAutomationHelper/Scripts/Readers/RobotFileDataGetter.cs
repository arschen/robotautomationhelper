using RobotAutomationHelper.Scripts.Objects;
using RobotAutomationHelper.Scripts.Static;
using System;
using System.Collections.Generic;

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
                                    if (currentKeyword.ImportFileName != null && !currentKeyword.ImportFileName.Equals(""))
                                    {
                                        foreach (RobotFile importFile in RobotFiles)
                                            if (importFile.fileName.EndsWith("\\" + currentKeyword.ImportFileName + ".robot"))
                                            {
                                                foreach (Keyword importKeyword in importFile.KeywordsList)
                                                    if (importKeyword.Name.Equals(currentKeyword.Name))
                                                    {
                                                        if (currentKeyword.Params != null && currentKeyword.Params.Count > 0)
                                                            for (int i = 0; i < currentKeyword.Params.Count; i++)
                                                                tempKeyword.Params[i].Name = importKeyword.Params[i].Name;
                                                        tempKeyword.Keywords.AddRange(importKeyword.Keywords);
                                                        tempKeyword.OutputFilePath = importFile.fileName;
                                                        break;
                                                    }
                                                break;
                                            }
                                    }
                                    else
                                    {
                                        bool isFound = false;
                                        foreach (string fileName in currentFile.resources)
                                        {
                                            foreach (RobotFile importFile in RobotFiles)
                                                if (importFile.fileName.Equals(fileName))
                                                {
                                                    foreach (Keyword importKeyword in importFile.KeywordsList)
                                                        if (importKeyword.Name.Equals(currentKeyword.Name))
                                                        {
                                                            if (currentKeyword.Params != null && currentKeyword.Params.Count > 0)
                                                                for (int i = 0; i < currentKeyword.Params.Count; i++)
                                                                    tempKeyword.Params[i].Name = importKeyword.Params[i].Name;
                                                            tempKeyword.Keywords.AddRange(importKeyword.Keywords);
                                                            tempKeyword.OutputFilePath = importFile.fileName;
                                                            isFound = true;
                                                            break;
                                                        }
                                                    break;
                                                }
                                            if (isFound) break;
                                        }
                                    }

                                    if (tempKeyword.Keywords != null && tempKeyword.Keywords.Count > 0)
                                        tempKeyword.CopyKeyword(GetDataForInternalKeywords(tempKeyword, currentFile));
                                }
                                else
                                    tempKeyword.CopyKeyword(currentKeyword);
                                tempTestCase.Steps.Add(tempKeyword);
                            }
                        testCases.Add(tempTestCase);
                    }

            foreach (TestCase tempCase in testCases)
            {
                Console.WriteLine(tempCase.Name + " " + tempCase.OutputFilePath);
                foreach (Keyword tempStep in tempCase.Steps)
                    Console.WriteLine("     " + tempStep.Name + " " + tempStep.OutputFilePath);
            }
        }

        public Keyword GetDataForInternalKeywords(Keyword keyword, RobotFile file)
        {
            var returnKeyword = new Keyword(keyword.Parent);
            returnKeyword.CopyKeyword(keyword);
            returnKeyword.Keywords = new List<Keyword>();

            foreach (Keyword currentKeyword in keyword.Keywords)
            {
                var tempKeyword = new Keyword(keyword);
                tempKeyword.CopyKeyword(currentKeyword);
                if (currentKeyword.Type == KeywordType.Custom)
                {  
                    if (currentKeyword.ImportFileName != null && !currentKeyword.ImportFileName.Equals(""))
                    {
                        foreach (RobotFile importFile in RobotFiles)
                            if (importFile.fileName.EndsWith("\\" + currentKeyword.ImportFileName + ".robot"))
                            {
                                foreach (Keyword importKeyword in importFile.KeywordsList)
                                    if (importKeyword.Name.Equals(currentKeyword.Name))
                                    {
                                        if (currentKeyword.Params != null && currentKeyword.Params.Count > 0)
                                            for (int i = 0; i < currentKeyword.Params.Count; i++)
                                                tempKeyword.Params[i].Name = importKeyword.Params[i].Name;
                                        tempKeyword.Keywords.AddRange(importKeyword.Keywords);
                                        tempKeyword.OutputFilePath = importFile.fileName;
                                        break;
                                    }
                                break;
                            }
                    }
                    else
                    {
                        bool isFound = false;
                        foreach (string fileName in file.resources)
                        {
                            foreach (RobotFile importFile in RobotFiles)
                                if (importFile.fileName.Equals(fileName))
                                {
                                    foreach (Keyword importKeyword in importFile.KeywordsList)
                                        if (importKeyword.Name.Equals(currentKeyword.Name))
                                        {
                                            if (currentKeyword.Params != null && currentKeyword.Params.Count > 0)
                                                for (int i = 0; i < currentKeyword.Params.Count; i++)
                                                    tempKeyword.Params[i].Name = importKeyword.Params[i].Name;
                                            tempKeyword.Keywords.AddRange(importKeyword.Keywords);
                                            tempKeyword.OutputFilePath = importFile.fileName;
                                            isFound = true;
                                            break;
                                        }
                                    break;
                                }
                            if (isFound) break;
                        }
                    }

                    if (tempKeyword.Keywords != null && tempKeyword.Keywords.Count > 0)
                    {
                        foreach (RobotFile importFile in RobotFiles)
                            if (importFile.fileName.Equals(tempKeyword.OutputFilePath))
                            {
                                GetDataForInternalKeywords(tempKeyword, importFile);
                                break;
                            }
                    }
                }

                returnKeyword.Keywords.Add(tempKeyword);
            }

            return returnKeyword;
        }
    }
}

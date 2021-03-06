﻿using Moq;
using NUnit.Framework;
using Rubberduck.Inspections.Concrete;
using Rubberduck.Inspections.QuickFixes;
using Rubberduck.Parsing.Inspections.Abstract;
using Rubberduck.Parsing.VBA;
using Rubberduck.VBEditor.SafeComWrappers;
using Rubberduck.VBEditor.SafeComWrappers.Abstract;
using RubberduckTests.Mocks;

namespace RubberduckTests.QuickFixes
{
    [TestFixture]
    public class AccessSheetUsingCodeNameQuickFixTests : QuickFixTestBase 
    {
        [Test]
        [Category("QuickFixes")]
        public void SheetAccessedUsingString_QuickFixWorks_UsingSheetThroughWorkbookModule()
        {
            const string inputCode = @"
Public Sub Foo()
    ThisWorkbook.Sheets(""Sheet1"").Range(""A1"") = ""foo""
End Sub";

            const string expectedCode = @"
Public Sub Foo()
    Sheet1.Range(""A1"") = ""foo""
End Sub";

            var actualCode = ApplyQuickFixToFirstInspectionResult(inputCode, state => new SheetAccessedUsingStringInspection(state, state.ProjectsProvider));
            Assert.AreEqual(expectedCode, actualCode);
        }

        [Test]
        [Category("QuickFixes")]
        public void SheetAccessedUsingString_QuickFixWorks_AssigningSheetToVariable()
        {
            const string inputCode = @"
Public Sub Foo()
    Dim ws As Worksheet
    Set ws = ThisWorkbook.Worksheets(""Sheet1"")
    ws.Cells(1, 1) = ""foo""
    Bar ws
End Sub

Public Sub Bar(ws As Worksheet)
End Sub";

            const string expectedCode = @"
Public Sub Foo()
    
    
    Sheet1.Cells(1, 1) = ""foo""
    Bar Sheet1
End Sub

Public Sub Bar(ws As Worksheet)
End Sub";

            var actualCode = ApplyQuickFixToFirstInspectionResult(inputCode, state => new SheetAccessedUsingStringInspection(state, state.ProjectsProvider));
            Assert.AreEqual(expectedCode, actualCode);
        }

        [Test]
        [Category("QuickFixes")]
        public void SheetAccessedUsingString_QuickFixWorks_AssigningSheetToUnusedVariable()
        {
            const string inputCode = @"
Public Sub Foo()
    Dim ws As Worksheet
    Set ws = ThisWorkbook.Worksheets(""Sheet1"")
End Sub";

            const string expectedCode = @"
Public Sub Foo()
    
    
End Sub";

            var actualCode = ApplyQuickFixToFirstInspectionResult(inputCode, state => new SheetAccessedUsingStringInspection(state, state.ProjectsProvider));
            Assert.AreEqual(expectedCode, actualCode);
        }

        [Test]
        [Category("QuickFixes")]
        public void SheetAccessedUsingString_QuickFixWorks_AssigningSheetToVariableDeclaredInList()
        {
            const string inputCode = @"
Public Sub Foo()
    Dim ws As Worksheet, s As String
    Set ws = ThisWorkbook.Worksheets(""Sheet1"")
    ws.Cells(1, 1) = ""foo""
End Sub";

            const string expectedCode = @"
Public Sub Foo()
    Dim s As String
    
    Sheet1.Cells(1, 1) = ""foo""
End Sub";

            var actualCode = ApplyQuickFixToFirstInspectionResult(inputCode, state => new SheetAccessedUsingStringInspection(state, state.ProjectsProvider));
            Assert.AreEqual(expectedCode, actualCode);
        }

        [Test]
        [Category("QuickFixes")]
        public void SheetAccessedUsingString_QuickFixWorks_AssigningSheetToVariableDeclaredLastInList()
        {
            const string inputCode = @"
Public Sub Foo()
    Dim s As String, ws As Worksheet
    Set ws = ThisWorkbook.Worksheets(""Sheet1"")
    ws.Cells(1, 1) = ""foo""
End Sub";

            const string expectedCode = @"
Public Sub Foo()
    Dim s As String
    
    Sheet1.Cells(1, 1) = ""foo""
End Sub";

            var actualCode = ApplyQuickFixToFirstInspectionResult(inputCode, state => new SheetAccessedUsingStringInspection(state, state.ProjectsProvider));
            Assert.AreEqual(expectedCode, actualCode);
        }

        [Test]
        [Category("QuickFixes")]
        public void SheetAccessedUsingString_QuickFixWorks_SheetVariableWithSameNameAsOtherDeclarations()
        {
            const string inputCode = @"
Public Sub Foo()
    Dim ws As Worksheet
    Set ws = ThisWorkbook.Worksheets(""Sheet1"")
    ws.Cells(1, 1) = ""foo""
End Sub

Public Sub ws()
    Dim ws As Worksheet
End Sub";

            const string expectedCode = @"
Public Sub Foo()
    
    
    Sheet1.Cells(1, 1) = ""foo""
End Sub

Public Sub ws()
    Dim ws As Worksheet
End Sub";

            var actualCode = ApplyQuickFixToFirstInspectionResult(inputCode, state => new SheetAccessedUsingStringInspection(state, state.ProjectsProvider));
            Assert.AreEqual(expectedCode, actualCode);
        }

        [Test]
        [Category("QuickFixes")]
        public void SheetAccessedUsingString_QuickFixWorks_SheetNameDifferentThanSheetCodeName()
        {
            const string inputCode = @"
Public Sub Foo()
    Dim ws As Worksheet
    Set ws = ThisWorkbook.Worksheets(""Name"")
    ws.Cells(1, 1) = ""foo""
End Sub

Public Sub ws()
    Dim ws As Worksheet
End Sub";

            const string expectedCode = @"
Public Sub Foo()
    
    
    CodeName.Cells(1, 1) = ""foo""
End Sub

Public Sub ws()
    Dim ws As Worksheet
End Sub";

            var actualCode = ApplyQuickFixToFirstInspectionResult(inputCode, state => new SheetAccessedUsingStringInspection(state, state.ProjectsProvider));
            Assert.AreEqual(expectedCode, actualCode);
        }

        protected override IQuickFix QuickFix(RubberduckParserState state)
        {
            return new AccessSheetUsingCodeNameQuickFix(state);
        }

        protected override IVBE TestVbe(string code, out IVBComponent component)
        {
            var builder = new MockVbeBuilder();
            var project = builder.ProjectBuilder("VBAProject", ProjectProtection.Unprotected)
                .AddComponent("Module1", ComponentType.StandardModule, code)
                .AddComponent("Sheet1", ComponentType.Document, "",
                    properties: new[]
                    {
                        CreateVBComponentPropertyMock("Name", "Sheet1").Object,
                        CreateVBComponentPropertyMock("CodeName", "Sheet1").Object
                    })
                .AddComponent("SheetWithDifferentCodeName", ComponentType.Document, "",
                    properties: new[]
                    {
                        CreateVBComponentPropertyMock("Name", "Name").Object,
                        CreateVBComponentPropertyMock("CodeName", "CodeName").Object
                    })
                .AddReference(ReferenceLibrary.Excel)
                .Build();

            component = project.Object.VBComponents[0];

            return builder.AddProject(project).Build().Object;
        }

        private static Mock<IProperty> CreateVBComponentPropertyMock(string propertyName, string propertyValue)
        {
            var propertyMock = new Mock<IProperty>();
            propertyMock.SetupGet(m => m.Name).Returns(propertyName);
            propertyMock.SetupGet(m => m.Value).Returns(propertyValue);

            return propertyMock;
        }
    }
}

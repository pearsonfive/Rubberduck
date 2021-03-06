using Rubberduck.Inspections.Concrete;
using Rubberduck.Parsing.Inspections.Abstract;
using Rubberduck.Refactorings.MoveCloserToUsage;
using Rubberduck.Resources.Inspections;

namespace Rubberduck.Inspections.QuickFixes
{
    /// <summary>
    /// Moves field declaration to the procedure scope it's used in.
    /// </summary>
    /// <inspections>
    /// <inspection name="MoveFieldCloserToUsageInspection" />
    /// </inspections>
    /// <canfix procedure="true" module="true" project="true" />
    /// <example>
    /// <before>
    /// <![CDATA[
    /// Option Explicit
    /// Private value As Long
    /// 
    /// Public Sub DoSomething()
    ///     value = 42
    ///     Debug.Print value
    /// End Sub
    /// ]]>
    /// </before>
    /// <after>
    /// <![CDATA[
    /// Option Explicit
    /// 
    /// Public Sub DoSomething()
    ///     Dim value As Long
    ///     value = 42
    ///     Debug.Print value
    /// End Sub
    /// ]]>
    /// </after>
    /// </example>
    public sealed class MoveFieldCloserToUsageQuickFix : RefactoringQuickFixBase
    {
        public MoveFieldCloserToUsageQuickFix(MoveCloserToUsageRefactoring refactoring)
            : base(refactoring, typeof(MoveFieldCloserToUsageInspection))
        {}

        protected override void Refactor(IInspectionResult result)
        {
            Refactoring.Refactor(result.Target);
        }

        public override string Description(IInspectionResult result)
        {
            return string.Format(InspectionResults.MoveFieldCloserToUsageInspection, result.Target.IdentifierName);
        }
    }
}
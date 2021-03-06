using Rubberduck.Inspections.Concrete;
using Rubberduck.Parsing.Inspections.Abstract;
using Rubberduck.Refactorings.EncapsulateField;

namespace Rubberduck.Inspections.QuickFixes
{
    /// <summary>
    /// Runs the 'Encapsulate Field' refactoring, which prompts for identifier names for the new property and its value parameter.
    /// </summary>
    /// <inspections>
    /// <inspection name="EncapsulatePublicFieldInspection" />
    /// </inspections>
    /// <canfix procedure="true" module="true" project="true" />
    /// <example>
    /// <before>
    /// <![CDATA[
    /// Option Explicit
    /// Public SomeValue As Long
    /// ]]>
    /// </before>
    /// <after>
    /// <![CDATA[
    /// Option Explicit
    /// Private SomeValue As Long
    /// 
    /// Public Property Get SomeProperty() As Long
    ///     SomeProperty = SomeValue
    /// End Property
    /// 
    /// Public Property Let SomeProperty(ByVal value As Long)
    ///     SomeValue = value
    /// End Property
    /// ]]>
    /// </after>
    /// </example>
    public sealed class EncapsulateFieldQuickFix : RefactoringQuickFixBase
    {
        public EncapsulateFieldQuickFix(EncapsulateFieldRefactoring refactoring)
            : base(refactoring, typeof(EncapsulatePublicFieldInspection))
        {}

        protected override void Refactor(IInspectionResult result)
        {
            Refactoring.Refactor(result.Target);
        }

        public override string Description(IInspectionResult result)
        {
            return string.Format(Resources.Inspections.QuickFixes.EncapsulatePublicFieldInspectionQuickFix, result.Target.IdentifierName);
        }
    }
}
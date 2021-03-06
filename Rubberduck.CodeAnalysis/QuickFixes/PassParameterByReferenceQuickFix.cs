using Rubberduck.Inspections.Abstract;
using Rubberduck.Inspections.Concrete;
using Rubberduck.Parsing.Grammar;
using Rubberduck.Parsing.Inspections.Abstract;
using Rubberduck.Parsing.Rewriter;

namespace Rubberduck.Inspections.QuickFixes
{
    /// <summary>
    /// Modifies a parameter to be passed by reference.
    /// </summary>
    /// <inspections>
    /// <inspection name="AssignedByValParameterInspection" />
    /// </inspections>
    /// <canfix procedure="true" module="true" project="true" />
    /// <example>
    /// <before>
    /// <![CDATA[
    /// Public Sub DoSomething(ByVal value As Long)
    ///     value = 42
    ///     Debug.Print value
    /// End Sub
    /// ]]>
    /// </before>
    /// <after>
    /// <![CDATA[
    /// Public Sub DoSomething(ByRef value As Long)
    ///     value = 42
    ///     Debug.Print value
    /// End Sub
    /// ]]>
    /// </after>
    /// </example>
    public sealed class PassParameterByReferenceQuickFix : QuickFixBase
    {
        public PassParameterByReferenceQuickFix()
            : base(typeof(AssignedByValParameterInspection))
        {}

        public override void Fix(IInspectionResult result, IRewriteSession rewriteSession)
        {
            var rewriter = rewriteSession.CheckOutModuleRewriter(result.Target.QualifiedModuleName);

            var token = ((VBAParser.ArgContext)result.Target.Context).BYVAL().Symbol;
            rewriter.Replace(token, Tokens.ByRef);
        }

        public override string Description(IInspectionResult result) => Resources.Inspections.QuickFixes.PassParameterByReferenceQuickFix;

        public override bool CanFixInProcedure => true;
        public override bool CanFixInModule => true;
        public override bool CanFixInProject => true;
    }
}
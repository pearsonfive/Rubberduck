using Rubberduck.Inspections.Abstract;
using Rubberduck.Inspections.Concrete;
using Rubberduck.Parsing.Grammar;
using Rubberduck.Parsing.Inspections.Abstract;
using Rubberduck.Parsing.Rewriter;

namespace Rubberduck.Inspections.QuickFixes
{
    /// <summary>
    /// Makes the 'Local' keyword of an 'On Error' statement implicit.
    /// </summary>
    /// <inspections>
    /// <inspection name="OnLocalErrorInspection" />
    /// </inspections>
    /// <canfix procedure="true" module="true" project="true" />
    /// <example>
    /// <before>
    /// <![CDATA[
    /// Option Explicit
    /// 
    /// Public Sub DoSomething()
    ///     On Local Error GoTo CleanFail
    ///     Debug.Print 42 / 0
    /// CleanExit:
    ///     Exit Sub
    /// CleanFail:
    ///     Debug.Print Err.Description
    ///     Resume CleanExit
    /// End Sub
    /// ]]>
    /// </before>
    /// <after>
    /// <![CDATA[
    /// Option Explicit
    /// 
    /// Public Sub DoSomething()
    ///     On Error GoTo CleanFail
    ///     Debug.Print 42 / 0
    /// CleanExit:
    ///     Exit Sub
    /// CleanFail:
    ///     Debug.Print Err.Description
    ///     Resume CleanExit
    /// End Sub
    /// ]]>
    /// </after>
    /// </example>
    public sealed class RemoveLocalErrorQuickFix : QuickFixBase
    {
        public RemoveLocalErrorQuickFix()
            : base(typeof(OnLocalErrorInspection))
        {}

        public override void Fix(IInspectionResult result, IRewriteSession rewriteSession)
        {
            var errorStmt = (VBAParser.OnErrorStmtContext)result.Context;

            var rewriter = rewriteSession.CheckOutModuleRewriter(result.QualifiedSelection.QualifiedName);
            rewriter.Replace(errorStmt.ON_LOCAL_ERROR(), Tokens.On + " " + Tokens.Error);
        }

        public override string Description(IInspectionResult result) => Resources.Inspections.QuickFixes.RemoveLocalErrorQuickFix;

        public override bool CanFixInProcedure => true;
        public override bool CanFixInModule => true;
        public override bool CanFixInProject => true;
    }
}
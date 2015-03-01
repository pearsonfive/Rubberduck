﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rubberduck.VBA;
using Rubberduck.Inspections;
using Microsoft.Vbe.Interop;
using Rubberduck.VBA.Nodes;

namespace Rubberduck.Inspections
{
    public class Inspector : IInspector
    {
        private readonly IRubberduckParser _parser;
        private readonly IList<IInspection> _inspections;

        public Inspector(IRubberduckParser parser, IEnumerable<IInspection> inspections)
        {
            _parser = parser;
            _inspections = inspections.ToList();
        }

        public IList<ICodeInspectionResult> FindIssues(VBProject project)
        {
            var code = new VBProjectParseResult(_parser.Parse(project));

            var results = new ConcurrentBag<ICodeInspectionResult>();
            var inspections = _inspections.Where(inspection => inspection.Severity != CodeInspectionSeverity.DoNotShow)
                .Select(inspection =>
                    new Task(() =>
                    {
                        var result = inspection.GetInspectionResults(code);
                        var count = result.Count();
                        if (count > 0)
                        {
                            RaiseIssuesFound(count);

                            foreach (var inspectionResult in result)
                            {
                                results.Add(inspectionResult);
                            }
                        }
                    })).ToArray();

            foreach (var inspection in inspections)
            {
                inspection.Start();
            }

            Task.WaitAll(inspections);

            return results.ToList();
        }

        public event EventHandler<InspectorIssuesFoundEventArg> IssuesFound;
        private void RaiseIssuesFound(int count)
        {
            var handler = IssuesFound;
            if (handler == null)
            {
                return;
            }

            var args = new InspectorIssuesFoundEventArg(count);
            handler(this, args);
        }
    }
}

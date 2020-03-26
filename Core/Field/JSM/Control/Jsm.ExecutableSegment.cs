﻿using OpenVIII.Fields.Scripts.Instructions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace OpenVIII.Fields.Scripts
{
    public static partial class Jsm
    {
        public class ExecutableSegment : Segment, IJsmInstruction, IEnumerable<IJsmInstruction>
        {
            public ExecutableSegment(Int32 from, Int32 to)
                : base(from, to)
            {
            }

            public virtual IScriptExecuter GetExecuter()
            {
                return GetExecuter(_list);
            }

            internal static IScriptExecuter GetExecuter(IEnumerable<IJsmInstruction> instructions)
            {
                return new Executer(instructions);
            }

            public IEnumerator<IJsmInstruction> GetEnumerator() => _list.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();

            private sealed class Executer : IScriptExecuter, IEnumerable<IJsmInstruction>
            {
                private readonly IEnumerable<IJsmInstruction> _list;

                public Executer(IEnumerable<IJsmInstruction> list)
                {
                    _list = list;
                }

                public IEnumerable<IAwaitable> Execute(IServices services)
                {
                    foreach (var instr in _list)
                    {
                        if (instr is JsmInstruction singleInstruction)
                        {
                            yield return singleInstruction.Execute(services);
                        }
                        else if (instr is ExecutableSegment segment)
                        {
                            // TODO: Change recursion to the loop
                            var nested = segment.GetExecuter();
                            foreach (var result in nested.Execute(services))
                                yield return result;
                        }
                        else
                        {
                            throw new NotSupportedException($"Cannot execute instruction [{instr}] of type [{instr.GetType()}].");
                        }
                    }
                }

                public IEnumerator<IJsmInstruction> GetEnumerator() => _list.GetEnumerator();
                IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
            }
        }
    }
}
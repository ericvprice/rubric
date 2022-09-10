#r "..\bin\Debug\net6.0\Rubric.dll"
#r "..\bin\Debug\net6.0\Rubric.Tests.dll"
using Rubric;
using Rubric.Tests;
using System.Threading;

Task<bool> DoesApply(IEngineContext context, TestInput input, TestOutput output, CancellationToken token)
=> Task.FromResult(input.InputFlag == true);

Task Apply(IEngineContext context, TestInput input, TestOutput output, CancellationToken token)
{ output.TestFlag = true; return Task.CompletedTask; }
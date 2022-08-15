#r "..\bin\Debug\net6.0\Rubric.dll"
#r "..\bin\Debug\net6.0\Rubric.Tests.dll"
using Rubric;
using Rubric.Tests;
using System.Threading;

Task<bool> DoesApply(IEngineContext context, TestInput input, CancellationToken token)
=> Task.FromResult(true);

Task Apply(IEngineContext context, TestInput input, CancellationToken token)
{ input.InputFlag = true; return Task.CompletedTask; }

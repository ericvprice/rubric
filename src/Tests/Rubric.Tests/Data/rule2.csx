#r "..\bin\Debug\net6.0\Rubric.dll"
#r "..\bin\Debug\net6.0\Rubric.Tests.dll"
using Rubric;
using Rubric.Tests;
using System.Threading;

Task<bool> DoesApply(IEngineContext context, TestInput input, CancellationToken token)
=> Task.FromResult(input.InputFlag);

Task Apply(IEngineContext context, TestInput input, CancellationToken token)
{ input.Counter++; return Task.CompletedTask; }
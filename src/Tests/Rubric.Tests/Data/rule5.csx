#r "..\bin\Debug\net6.0\Rubric.dll"
#r "..\bin\Debug\net6.0\Rubric.Tests.dll"
using Rubric;
using Rubric.Tests;
using System.Threading;

Task<bool> DoesApply(IEngineContext context, TestOutput output, CancellationToken token)
=> Task.FromResult(true);

Task Apply(IEngineContext context, TestOutput output, CancellationToken token)
{ output.Counter++; return Task.CompletedTask; }
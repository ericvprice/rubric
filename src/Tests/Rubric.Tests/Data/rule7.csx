#r "..\bin\Debug\net6.0\Rubric.dll"
#r "..\bin\Debug\net6.0\Rubric.Tests.dll"
#r "..\..\Rubric.TestAssembly3\bin\Debug\net6.0\Rubric.TestAssembly3.dll"

using Rubric;
using Rubric.Tests;
using System.Threading;
using Rubric.TestAssembly3;

Task<bool> DoesApply(IEngineContext context, TestInput input, CancellationToken token)
=> Task.FromResult(true);

Task Apply(IEngineContext context, TestInput input, CancellationToken token)
{ input.InputFlag = true; var foo = new TestDep(); return Task.CompletedTask; }
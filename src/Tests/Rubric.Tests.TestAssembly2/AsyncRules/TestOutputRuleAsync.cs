﻿using System.Diagnostics.CodeAnalysis;

namespace Rubric.Tests.TestAssembly2.AsyncRules;

[ExcludeFromCodeCoverage]
// ReSharper disable once UnusedType.Global
internal class TestOutputRuleAsync : DefaultAsyncRule<TestAssemblyOutput2>
{
  public override Task Apply(IEngineContext context, TestAssemblyOutput2 input, CancellationToken t)
   => Task.CompletedTask;

}

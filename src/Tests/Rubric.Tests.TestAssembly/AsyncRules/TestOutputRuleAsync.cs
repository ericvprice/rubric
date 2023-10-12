﻿using System.Diagnostics.CodeAnalysis;

namespace Rubric.Tests.TestAssembly.AsyncRules;

[ExcludeFromCodeCoverage]
// ReSharper disable once UnusedType.Global
internal class TestOutputRuleAsync : DefaultAsyncRule<TestAssemblyOutput>
{
  public override Task Apply(IEngineContext context, TestAssemblyOutput input, CancellationToken t)
   => Task.CompletedTask;

}

﻿using Rubric.Tests.TestAssembly;
using System.Diagnostics.CodeAnalysis;

namespace Rubric.Tests.TestAssembly2.TestRules;

[ExcludeFromCodeCoverage]
// ReSharper disable once UnusedType.Global
internal class TestInputOutputRule : DefaultRule<TestAssemblyInput, TestAssemblyOutput2>
{
  public override void Apply(IEngineContext context, TestAssemblyInput input, TestAssemblyOutput2 output)
  {
  }

}

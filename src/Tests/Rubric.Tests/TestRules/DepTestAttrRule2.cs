﻿using Rubric.Dependency;

namespace Rubric.Tests.TestRules;

[DependsOn(typeof(DepTestAttrRule))]
internal class DepTestAttrRule2 : DepTestAttrRule
{
  public DepTestAttrRule2(bool expected, bool flagValue = true) : base(expected, flagValue) { }

}
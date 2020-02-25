﻿using RulesEngine.Dependency;
using RulesEngine.Rules;

namespace RulesEngine.Tests.DependencyRules
{
    [DependsOn("dep1")]
    [DependsOn("dep2")]
    [Provides("dep3")]
    public class DepTestPostRule : PostRule<TestOutput>
    {
        private readonly bool _shouldApply;

        public DepTestPostRule(bool shouldApply) => _shouldApply = shouldApply;

        public override void Apply(IEngineContext context, TestOutput obj) => obj.TestFlag = true;

        public override bool DoesApply(IEngineContext context, TestOutput obj) => _shouldApply;
    }
}
using Rubric.Rulesets;
using Rubric.Tests.TestRules;

namespace Rubric.Tests.Engines;

public class EngineOfTInTOutTests
{
  [Fact]
  public void Applies()
  {
    var rule = new TestDefaultRule();
    var input = new TestInput();
    var output = new TestOutput();
    var engine = new RuleEngine<TestInput, TestOutput>(
        null,
        new IRule<TestInput, TestOutput>[] { rule },
        null
    );
    engine.Apply(input, output);
    Assert.True(input.InputFlag);
    Assert.True(output.TestFlag);
  }

  [Fact]
  public void AppliesOrder()
  {
    var rule = new TestDefaultPreRule();
    var rule2 = new TestPreRule(true, false);
    var input = new TestInput();
    var output = new TestOutput();
    var engine = new RuleEngine<TestInput, TestOutput>(
        new IRule<TestInput>[] { rule, rule2 },
        null,
        null
    );
    engine.Apply(input, output);
    Assert.False(input.InputFlag);
  }

  [Fact]
  public void AppliesOrderReverse()
  {
    var rule = new TestDefaultPreRule();
    var rule2 = new TestPreRule(true, false);
    var input = new TestInput { InputFlag = true };
    var output = new TestOutput();
    var engine = new RuleEngine<TestInput, TestOutput>(
        new IRule<TestInput>[] { rule2, rule },
        null,
        null
    );
    engine.Apply(input, output);
    Assert.True(input.InputFlag);
  }

  [Fact]
  public void ApplyException()
  {
    var rule = new TestExceptionRule(false);
    var engine = new RuleEngine<TestInput, TestOutput>(null, new Rule<TestInput, TestOutput>[] { rule }, null);
    var input = new TestInput();
    var output = new TestOutput();
    Assert.Throws<Exception>(() => engine.Apply(input, output));
    Assert.True(input.InputFlag);
    Assert.True(output.TestFlag);
  }

  [Fact]
  public void ApplyExceptionnHandlerIgnore()
  {
    var rule = new TestExceptionRule(false);
    var engine = new RuleEngine<TestInput, TestOutput>(null, new Rule<TestInput, TestOutput>[] { rule }, null, ExceptionHandlers.Ignore);
    var input = new TestInput();
    var output = new TestOutput();
    var context = new EngineContext();
    engine.Apply(input, output, context);
    Assert.Null(context.GetLastException());
    Assert.True(input.InputFlag);
    Assert.True(output.TestFlag);
  }

  [Fact]
  public void ApplyManyExceptionHandlerHaltItem()
  {
    var rule = new TestExceptionRule(false);
    var engine = new RuleEngine<TestInput, TestOutput>(null, new Rule<TestInput, TestOutput>[] { rule }, null, ExceptionHandlers.HaltItem);
    var input = new TestInput();
    var input2 = new TestInput();
    var output = new TestOutput();
    var context = new EngineContext();
    engine.Apply(new[] { input, input2 }, output, context);
    var exception = context.GetLastException();
    Assert.NotNull(exception);
    Assert.IsType<ItemHaltException>(exception);
    Assert.Equal(rule, exception.Rule);
    Assert.Equal(input2, exception.Input);
    Assert.Equal(output, exception.Output);
    Assert.NotNull(exception.Context);
    Assert.True(input.InputFlag);
    Assert.True(input2.InputFlag);
    Assert.True(output.TestFlag);
  }

  [Fact]
  public void ApplyManyExceptionnHandlerHaltEngine()
  {
    var rule = new TestExceptionRule(false);
    var engine = new RuleEngine<TestInput, TestOutput>(null, new Rule<TestInput, TestOutput>[] { rule }, null, ExceptionHandlers.HaltEngine);
    var input = new TestInput();
    var input2 = new TestInput();
    var output = new TestOutput();
    var context = new EngineContext();
    engine.Apply(new[] { input, input2 }, output, context);
    var ex = context.GetLastException();
    Assert.NotNull(ex);
    Assert.IsType<EngineHaltException>(ex);
    Assert.Equal(rule, ex.Rule);
    Assert.Equal(input, ex.Input);
    Assert.Equal(output, ex.Output);
    Assert.NotNull(ex.Context);
    Assert.True(input.InputFlag);
    Assert.False(input2.InputFlag);
    Assert.True(output.TestFlag);
  }

  [Fact]
  public void Constructor()
  {
    var logger = new TestLogger();
    var ruleSet = new Ruleset<TestInput, TestOutput>();
    var engine = new RuleEngine<TestInput, TestOutput>(ruleSet, ExceptionHandlers.HaltEngine, logger);
    Assert.Equal(logger, engine.Logger);
    Assert.Equal(ExceptionHandlers.HaltEngine, engine.ExceptionHandler);
  }

  [Fact]
  public void ConstructorNullLoggerAndHandler()
  {
    var ruleSet = new Ruleset<TestInput, TestOutput>();
    var engine = new RuleEngine<TestInput, TestOutput>(ruleSet);
    Assert.NotNull(engine.Logger);
    Assert.NotNull(engine.ExceptionHandler);
  }

  [Fact]
  public void DoesApplyException()
  {
    var rule = new TestExceptionRule(true);
    var engine = new RuleEngine<TestInput, TestOutput>(null, new Rule<TestInput, TestOutput>[] { rule }, null);
    var input = new TestInput();
    var output = new TestOutput();
    var context = new EngineContext();
    var exception = Assert.Throws<Exception>(() => engine.Apply(input, output, context));
    Assert.IsNotType<EngineException>(exception);
    Assert.Null(context.GetLastException());
    Assert.False(input.InputFlag);
    Assert.False(output.TestFlag);
  }

  [Fact]
  public void FullRun()
  {
    var engine = EngineBuilder.ForInputAndOutput<TestInput, TestOutput>()
                              .WithPreRule("test")
                                .WithPredicate((_, _) => true)
                                .WithAction((_, i) => i.Items.Add("pre"))
                              .EndRule()
                              .WithRule("test")
                                .WithPredicate((_, _, _) => true)
                                .WithAction((_, i, o) =>
                                {
                                  i.Items.Add("rule");
                                  o.Outputs.Add("rule");
                                })
                              .EndRule()
                              .WithPostRule("test")
                                .WithPredicate((_, _) => true)
                                .WithAction((_, o) => o.Outputs.Add("postrule"))
                              .EndRule()
                              .Build();
    var input1 = new TestInput();
    var input2 = new TestInput();
    var output = new TestOutput();
    engine.Apply(new[] { input1, input2 }, output);
    Assert.Equal(2, input1.Items.Count);
    Assert.Contains("pre", input1.Items);
    Assert.Contains("rule", input1.Items);
    Assert.Equal(2, input2.Items.Count);
    Assert.Contains("pre", input2.Items);
    Assert.Contains("rule", input2.Items);
    Assert.Equal(3, output.Outputs.Count);
    Assert.Equal(2, output.Outputs.Count(o => o == "rule"));
    Assert.Single(output.Outputs.Where(o => o == "postrule"));
  }

  [Fact]
  public void NotApplies()
  {
    var rule = new TestRule(false);
    var input = new TestInput();
    var output = new TestOutput();
    var engine = new RuleEngine<TestInput, TestOutput>(
        null,
        new IRule<TestInput, TestOutput>[] { rule },
        null
    );
    engine.Apply(input, output);
    Assert.False(input.InputFlag);
    Assert.False(output.TestFlag);
  }

  [Fact]
  public void PostApplies()
  {
    var rule = new TestDefaultPostRule();
    var input = new TestInput();
    var output = new TestOutput();
    var engine = new RuleEngine<TestInput, TestOutput>(
        null,
        null,
        new IRule<TestOutput>[] { rule }
    );
    engine.Apply(input, output);
    Assert.True(output.TestFlag);
  }

  [Fact]
  public void PostApplyException()
  {
    var testPostRule = new TestExceptionPostRule(false);
    var engine =
        new RuleEngine<TestInput, TestOutput>(null, null, new Rule<TestOutput>[] { testPostRule });
    var input = new TestInput();
    var output = new TestOutput();
    var context = new EngineContext();
    var exception = Assert.Throws<Exception>(() => engine.Apply(input, output, context));
    Assert.IsNotType<EngineException>(exception);
    Assert.Null(context.GetLastException());
    Assert.True(output.TestFlag);
  }

  [Fact]
  public void PostDoesApplyException()
  {
    var testPostRule = new TestExceptionPostRule(true);
    var engine =
        new RuleEngine<TestInput, TestOutput>(null, null, new Rule<TestOutput>[] { testPostRule });
    var input = new TestInput();
    var output = new TestOutput();
    var context = new EngineContext();
    var exception = Assert.Throws<Exception>(() => engine.Apply(input, output, context));
    Assert.IsNotType<EngineException>(exception);
    Assert.Null(context.GetLastException());
    Assert.False(output.TestFlag);
  }

  [Fact]
  public void PostNotApplies()
  {
    var rule = new TestPostRule(false);
    var input = new TestInput();
    var output = new TestOutput();
    var engine = new RuleEngine<TestInput, TestOutput>(
        null,
        null,
        new IRule<TestOutput>[] { rule }
    );
    engine.Apply(input, output);
    Assert.False(output.TestFlag);
  }

  [Fact]
  public void PreApplies()
  {
    var rule = new TestDefaultPreRule();
    var input = new TestInput();
    var output = new TestOutput();
    var engine = new RuleEngine<TestInput, TestOutput>(
        new IRule<TestInput>[] { rule },
        null,
        null
    );
    engine.Apply(input, output);
    Assert.True(input.InputFlag);
  }

  [Fact]
  public void PreApplyException()
  {
    var testPreRule = new TestExceptionPreRule(false);
    var engine = new RuleEngine<TestInput, TestOutput>(new Rule<TestInput>[] { testPreRule }, null, null);
    var input = new TestInput();
    var output = new TestOutput();
    var context = new EngineContext();
    var exception = Assert.Throws<Exception>(() => engine.Apply(input, output, context));
    Assert.IsNotType<EngineException>(exception);
    Assert.Null(context.GetLastException());
    Assert.True(input.InputFlag);
  }

  [Fact]
  public void PreDoesApplyException()
  {
    var testPreRule = new TestExceptionPreRule(true);
    var engine = new RuleEngine<TestInput, TestOutput>(new Rule<TestInput>[] { testPreRule }, null, null);
    var input = new TestInput();
    var output = new TestOutput();
    var context = new EngineContext();
    var exception = Assert.Throws<Exception>(() => engine.Apply(input, output, context));
    Assert.IsNotType<EngineException>(exception);
    Assert.Null(context.GetLastException());
    Assert.False(input.InputFlag);
  }

  [Fact]
  public void PreNotApplies()
  {
    var rule = new TestPreRule(false);
    var input = new TestInput();
    var output = new TestOutput();
    var engine = new RuleEngine<TestInput, TestOutput>(
        new IRule<TestInput>[] { rule },
        null,
        null
    );
    engine.Apply(input, output);
    Assert.False(input.InputFlag);
  }

  [Fact]
  public void ExceptionHandlingIgnore()
  {
    var testInput = new TestInput
    {
      Items = new() { "PreException", "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput
    {
      Outputs = new() { "PostException" }
    };
    var engine = GetExceptionEngine(ExceptionHandlers.Ignore);
    var context = new EngineContext();
    engine.Apply(new[] { testInput, testInput2 }, testOutput, context);
    Assert.Null(context.GetLastException());
    Assert.Equal(4, testInput.Items.Count);
    Assert.Equal(4, testInput2.Items.Count);
    Assert.Equal(2, testOutput.Outputs.Count);

  }

  [Fact]
  public void ExceptionHandlingPreThrow()
  {
    var testInput = new TestInput
    {
      Items = new() { "PreException", "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var context = new EngineContext();
    var engine = GetExceptionEngine(ExceptionHandlers.Rethrow);
    Assert.Throws<Exception>(() => engine.Apply(new[] { testInput, testInput2 }, testOutput, context));
    Assert.Null(context.GetLastException());
    Assert.Equal(3, testInput.Items.Count);
    Assert.Empty(testInput2.Items);
    Assert.Empty(testOutput.Outputs);

  }

  [Fact]
  public void ExceptionHandlingPreItem()
  {
    var testInput = new TestInput
    {
      Items = new() { "PreException", "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetExceptionEngine(ExceptionHandlers.HaltItem);
    var context = new EngineContext();
    engine.Apply(new[] { testInput, testInput2 }, testOutput, context);
    var ex = context.GetLastException();
    Assert.NotNull(ex);
    Assert.IsType<ItemHaltException>(ex);
    Assert.Equal(testInput, ex.Input);
    Assert.Equal(engine.PreRules.First(), ex.Rule);
    Assert.NotNull(ex.Context);
    Assert.Equal(3, testInput.Items.Count);
    Assert.Equal(4, testInput2.Items.Count);
    Assert.Equal(2, testOutput.Outputs.Count);

  }

  [Fact]
  public void ExceptionHandlingPreEngine()
  {
    var testInput = new TestInput
    {
      Items = new() { "PreException", "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var context = new EngineContext();
    var engine = GetExceptionEngine(ExceptionHandlers.HaltEngine);
    engine.Apply(new[] { testInput, testInput2 }, testOutput, context);
    var ex = context.GetLastException();
    Assert.NotNull(ex);
    Assert.IsType<EngineHaltException>(ex);
    Assert.Equal(testInput, ex.Input);
    Assert.Equal(engine.PreRules.First(), ex.Rule);
    Assert.NotNull(ex.Context);
    Assert.Equal(3, testInput.Items.Count);
    Assert.Empty(testInput2.Items);
    Assert.Empty(testOutput.Outputs);
  }

  [Fact]
  public void ExceptionHandlingPreHandlerException()
  {
    var testInput = new TestInput
    {
      Items = new() { "PreException", "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetExceptionEngine(new LambdaExceptionHandler(
      (_, _, _, _, _) => throw new InvalidOperationException()));
    var context = new EngineContext();
    Assert.Throws<InvalidOperationException>(() => engine.Apply(new[] { testInput, testInput2 }, testOutput, context));
    Assert.Null(context.GetLastException());
    Assert.Equal(3, testInput.Items.Count);
    Assert.Empty(testInput2.Items);
    Assert.Empty(testOutput.Outputs);
  }

  [Fact]
  public void ExceptionHandlingHandlerException()
  {
    var testInput = new TestInput
    {
      Items = new() { "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetExceptionEngine(new LambdaExceptionHandler(
      (_, _, _, _, _) => throw new InvalidOperationException()));
    var context = new EngineContext();
    Assert.Throws<InvalidOperationException>(() => engine.Apply(new[] { testInput, testInput2 }, testOutput, context));
    Assert.Null(context.GetLastException());
    Assert.Equal(4, testInput.Items.Count);
    Assert.Empty(testInput2.Items);
    Assert.Empty(testOutput.Outputs);
  }

  [Fact]
  public void ExceptionHandlingPreManualItem()
  {
    var testInput = new TestInput
    {
      Items = new() { "PreException" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var context = new EngineContext();
    var engine = GetEngineExceptionEngine<ItemHaltException>();
    engine.Apply(new[] { testInput, testInput2 }, testOutput, context);
    var ex = context.GetLastException();
    Assert.NotNull(ex);
    Assert.IsType<ItemHaltException>(ex);
    Assert.Equal(testInput, ex.Input);
    Assert.Equal(engine.PreRules.First(), ex.Rule);
    Assert.NotNull(ex.Context);
    Assert.Equal(2, testInput.Items.Count);
    Assert.Equal(4, testInput2.Items.Count);
    Assert.Equal(2, testOutput.Outputs.Count);
  }

  [Fact]
  public void ExceptionHandlingManualEngine()
  {
    var testInput = new TestInput
    {
      Items = new() { "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetEngineExceptionEngine<EngineHaltException>();
    var context = new EngineContext();
    engine.Apply(new[] { testInput, testInput2 }, testOutput, context);
    var ex = context.GetLastException();
    Assert.NotNull(ex);
    Assert.IsType<EngineHaltException>(ex);
    Assert.Equal(testInput, ex.Input);
    Assert.Equal(engine.Rules.First(), ex.Rule);
    Assert.NotNull(ex.Context);
    Assert.Equal(4, testInput.Items.Count);
    Assert.Empty(testInput2.Items);
    Assert.Empty(testOutput.Outputs);
  }

  [Fact]
  public void ExceptionHandlingManualItem()
  {
    var testInput = new TestInput
    {
      Items = new() { "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetEngineExceptionEngine<ItemHaltException>();
    var context = new EngineContext();
    engine.Apply(new[] { testInput, testInput2 }, testOutput, context);
    var ex = context.GetLastException();
    Assert.NotNull(ex);
    Assert.IsType<ItemHaltException>(ex);
    Assert.Equal(testInput, ex.Input);
    Assert.Equal(engine.Rules.First(), ex.Rule);
    Assert.NotNull(ex.Context);
    Assert.Equal(4, testInput.Items.Count);
    Assert.Equal(4, testInput2.Items.Count);
    Assert.Equal(2, testOutput.Outputs.Count);
  }

  [Fact]
  public void ExceptionHandlingPreManualEngine()
  {
    var testInput = new TestInput
    {
      Items = new() { "PreException" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetEngineExceptionEngine<EngineHaltException>();
    var context = new EngineContext();
    engine.Apply(new[] { testInput, testInput2 }, testOutput, context);
    var ex = context.GetLastException();
    Assert.NotNull(ex);
    Assert.IsType<EngineHaltException>(ex);
    Assert.Equal(testInput, ex.Input);
    Assert.Equal(engine.PreRules.First(), ex.Rule);
    Assert.NotNull(ex.Context);
    Assert.NotNull(ex);
    Assert.IsType<EngineHaltException>(ex);
    Assert.Equal(testInput, ex.Input);
    Assert.Equal(engine.PreRules.First(), ex.Rule);
    Assert.NotNull(ex.Context);
    Assert.Equal(2, testInput.Items.Count);
    Assert.Empty(testInput2.Items);
    Assert.Empty(testOutput.Outputs);
  }

  [Fact]
  public void ExceptionHandlingPostManualEngine()
  {
    var testInput = new TestInput();
    var testInput2 = new TestInput();
    var testOutput = new TestOutput
    {
      Outputs = new() { "PostException" }
    };
    var engine = GetEngineExceptionEngine<EngineHaltException>();
    var context = new EngineContext();
    engine.Apply(new[] { testInput, testInput2 }, testOutput, context);
    var ex = context.GetLastException();
    Assert.NotNull(ex);
    Assert.IsType<EngineHaltException>(ex);
    Assert.Equal(engine.PostRules.First(), ex.Rule);
    Assert.NotNull(ex.Context);
    Assert.NotNull(ex);
    Assert.IsType<EngineHaltException>(ex);
    Assert.Equal(4, testInput.Items.Count);
    Assert.Equal(4, testInput2.Items.Count);
    Assert.Equal(2, testOutput.Outputs.Count);
  }

  [Fact]
  public void ExceptionHandlingPostManualItem()
  {
    var testInput = new TestInput();
    var testInput2 = new TestInput();
    var testOutput = new TestOutput
    {
      Outputs = new() { "PostException" }
    };
    var engine = GetEngineExceptionEngine<ItemHaltException>();
    engine.Apply(new[] { testInput, testInput2 }, testOutput);
    Assert.Equal(4, testInput.Items.Count);
    Assert.Equal(4, testInput2.Items.Count);
    Assert.Equal(2, testOutput.Outputs.Count);
  }

  [Fact]
  public void ExceptionHandlingPostManualEngineSingleItem()
  {
    var testInput = new TestInput();
    var testOutput = new TestOutput
    {
      Outputs = new() { "PostException" }
    };
    var context = new EngineContext();
    var engine = GetEngineExceptionEngine<EngineHaltException>();
    engine.Apply(testInput, testOutput, context);
    var ex = context.GetLastException();
    Assert.NotNull(ex);
    Assert.IsType<EngineHaltException>(ex);
    Assert.Equal(engine.PostRules.First(), ex.Rule);
    Assert.NotNull(ex.Context);
    Assert.NotNull(ex);
    Assert.IsType<EngineHaltException>(ex);
    Assert.Equal(4, testInput.Items.Count);
    Assert.Equal(2, testOutput.Outputs.Count);
  }

  [Fact]
  public void ExceptionHandlingPostManualItemSingleItem()
  {
    var testInput = new TestInput();
    var testOutput = new TestOutput
    {
      Outputs = new() { "PostException" }
    };
    var context = new EngineContext();
    var engine = GetEngineExceptionEngine<ItemHaltException>();
    engine.Apply(testInput, testOutput, context);
    Assert.Null(context.GetLastException());
    Assert.Equal(4, testInput.Items.Count);
    Assert.Equal(2, testOutput.Outputs.Count);
  }

  private static IRuleEngine<TestInput, TestOutput> GetExceptionEngine(IExceptionHandler handler)
   => EngineBuilder.ForInputAndOutput<TestInput, TestOutput>()
                  .WithPreRule("testprerule")
                    .WithAction((_, i) =>
                    {
                      i.Items.Add("testprerule");
                      if (i.Items.Contains("PreException")) throw new();
                      i.Items.Add("testprerule");
                    })
                  .EndRule()
                  .WithRule("testrule")
                    .WithAction((_, i, _) =>
                    {
                      i.Items.Add("testrule");
                      if (i.Items.Contains("Exception")) throw new();
                      i.Items.Add("testrule2");
                    })
                  .EndRule()
                  .WithPostRule("testpostrule")
                    .WithAction((_, o) =>
                    {
                      o.Outputs.Add("testpostrule");
                      if (o.Outputs.Contains("PostException")) throw new();
                      o.Outputs.Add("testpostrule2");
                    })
                  .EndRule()
                  .WithExceptionHandler(handler)
                  .Build();

  private static IRuleEngine<TestInput, TestOutput> GetEngineExceptionEngine<T>() where T : EngineException, new()
   => EngineBuilder.ForInputAndOutput<TestInput, TestOutput>()
                .WithPreRule("testprerule")
                  .WithAction((_, i) =>
                  {
                    i.Items.Add("testprerule");
                    if (i.Items.Contains("PreException")) throw new T();
                    i.Items.Add("testprerule");
                  })
                .EndRule()
                .WithRule("testrule")
                  .WithAction((_, i, _) =>
                  {
                    i.Items.Add("testrule");
                    if (i.Items.Contains("Exception")) throw new T();
                    i.Items.Add("testrule2");
                  })
                .EndRule()
                .WithPostRule("testpostrule")
                  .WithAction((_, o) =>
                  {
                    o.Outputs.Add("testpostrule");
                    if (o.Outputs.Contains("PostException")) throw new T();
                    o.Outputs.Add("testpostrule2");
                  })
                .EndRule()
                .Build();
}

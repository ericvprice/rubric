using Rubric.Extensions.Serialization;
using Rubric.Rules.Scripted;
using System.IO;
using System.Text.Json;

namespace Rubric.Tests.Scripting;

public class ScriptTests
{
  [Fact]
  public async Task BasicTestsOfT()
  {
    var rule = new ScriptedRule<TestInput>(
        "test",
        @"using Rubric.Tests;
          Task<bool> DoesApply(IEngineContext context, TestInput input, CancellationToken t) => Task.FromResult(true);
          Task Apply(IEngineContext context, TestInput input, CancellationToken t) {input.InputFlag = true; return Task.CompletedTask;}
        "
    );
    var input = new TestInput();
    var context = new EngineContext();
    Assert.True(await rule.DoesApply(context, input, default));
    Assert.Equal("test", rule.Name);
    Assert.Empty(rule.Dependencies);
    Assert.Single(rule.Provides);
    await rule.Apply(context, input, default);
    Assert.True(input.InputFlag);
  }

  [Fact]
  public async Task BasicTestsOfTInTOut()
  {
    var rule = new ScriptedRule<TestInput, TestOutput>(
        "test",
        @"
          using Rubric.Tests;
          Task<bool> DoesApply(IEngineContext context, TestInput input, TestOutput output, CancellationToken t) => Task.FromResult(input.InputFlag);
          Task Apply(IEngineContext context, TestInput input, TestOutput output, CancellationToken t) { output.TestFlag = true; return Task.CompletedTask; }
        "
    );
    var input = new TestInput { InputFlag = true };
    var output = new TestOutput();
    var context = new EngineContext();
    Assert.True(await rule.DoesApply(context, input, output, default));
    Assert.Equal("test", rule.Name);
    Assert.Empty(rule.Dependencies);
    Assert.Empty(rule.Provides);
    await rule.Apply(context, input, output, default);
    Assert.True(output.TestFlag);
  }


  [Fact]
  public async Task RuleSetTFromJson()
  {
    const string fileName = "Data\\TestRulesT.json";
    var options = new JsonSerializerOptions
    {
      AllowTrailingCommas = true,
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
      ReadCommentHandling = JsonCommentHandling.Skip
    };
    var ruleSetModel = JsonSerializer.Deserialize<AsyncRulesetModel<TestInput>>(
      await File.ReadAllTextAsync(fileName),
      options);
    Assert.NotNull(ruleSetModel);
    ruleSetModel.BasePath = Path.Combine(Directory.GetCurrentDirectory(), "Data");
    var ruleset = new JsonRuleSet<TestInput>(ruleSetModel);
    Assert.Equal(2, ruleset.AsyncRules.Count());
    var engine = new AsyncRuleEngine<TestInput>(ruleset);
    var input = new TestInput();
    await engine.ApplyAsync(input);
    Assert.True(input.InputFlag);
  }

  [Fact]
  public async Task RuleSetTInTOutFromJson()
  {
    const string fileName = "Data\\TestRulesTU.json";
    var options = new JsonSerializerOptions
    {
      AllowTrailingCommas = true,
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
      ReadCommentHandling = JsonCommentHandling.Skip
    };
    var ruleSetModel = JsonSerializer.Deserialize<AsyncRulesetModel<TestInput, TestOutput>>(
      await File.ReadAllTextAsync(fileName),
      options);
    Assert.NotNull(ruleSetModel);
    ruleSetModel.BasePath = Path.Combine(Directory.GetCurrentDirectory(), "Data");
    var scriptOptions = ScriptingHelpers.GetDefaultOptions<TestInput, TestOutput>();
    var ruleset = new JsonRuleSet<TestInput, TestOutput>(ruleSetModel, scriptOptions);
    Assert.Single(ruleset.AsyncPreRules);
    Assert.Single(ruleset.AsyncRules);
    Assert.Single(ruleset.AsyncPostRules);
    var engine = new AsyncRuleEngine<TestInput, TestOutput>(ruleset);
    var input = new TestInput();
    var output = new TestOutput();
    await engine.ApplyAsync(input, output);
    Assert.True(input.InputFlag);
    Assert.True(output.TestFlag);
    Assert.Equal(1, output.Counter);
  }
}
using System.IO;
using System.Linq;
using System.Text.Json;
using Rubric.Scripting;

namespace Rubric.Tests.Scripting;

public class ScriptTests
{
  [Fact]
  public async Task BasicTestsOfT()
  {
    var rule = new ScriptedRule<TestInput>(
        "test",
        "return true;",
        "Input.InputFlag = true;"
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
  public async Task BasicTestsOfTU()
  {
    var rule = new ScriptedRule<TestInput, TestOutput>(
        "test",
        "return Input.InputFlag == true;",
        "Output.TestFlag = true;"
    );
    var input = new TestInput() { InputFlag = true };
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
  public async Task RuleSetFromJson()
  {
    var fileName = "Data\\TestRules.json";
    var options = new JsonSerializerOptions
    {
      AllowTrailingCommas = true,
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
      ReadCommentHandling = JsonCommentHandling.Skip
    };
    var ruleSetModel = JsonSerializer.Deserialize<AsyncRulesetModel<TestInput>>(
      await File.ReadAllTextAsync(fileName),
      options);
    ruleSetModel.BasePath = Path.Combine(Directory.GetCurrentDirectory(), "Data");
    var ruleset = new JsonRuleSet<TestInput>(ruleSetModel);
    Assert.Equal(2, ruleset.AsyncRules.Count());
    var engine = new AsyncRuleEngine<TestInput>(ruleset, false);
    var input = new TestInput();
    await engine.ApplyAsync(input);
    Assert.True(input.InputFlag);
  }
}
using System.IO;

namespace Rubric.Scripting;

public static class RuleLoader
{

  public static IAsyncRule<X> LoadFromModel<X>(RuleModel model, string basePath)
   => new ScriptedRule<X>
      (
        model.Name,
        string.IsNullOrWhiteSpace(model.DoesApplyScript)
            ? string.IsNullOrWhiteSpace(model.DoesApply)
                ? "return true"
                : model.DoesApply
            : File.ReadAllText(Path.Combine(basePath, model.DoesApplyScript)),
        string.IsNullOrWhiteSpace(model.ApplyScript)
            ? string.IsNullOrWhiteSpace(model.Apply)
                ? "return true"
                : model.DoesApply
            : File.ReadAllText(Path.Combine(basePath, model.ApplyScript)),
        model.DependsOn,
        model.Provides
      );

  public static IAsyncRule<X, Y> LoadFromModel<X, Y>(RuleModel model, string basePath)
    => new ScriptedRule<X, Y>(
          model.Name,
          string.IsNullOrWhiteSpace(model.DoesApplyScript)
              ? string.IsNullOrWhiteSpace(model.DoesApply)
                  ? "return true"
                  : model.DoesApply
              : File.ReadAllText(Path.Combine(basePath, model.DoesApplyScript)),
          string.IsNullOrWhiteSpace(model.ApplyScript)
              ? string.IsNullOrWhiteSpace(model.Apply)
                  ? "return true"
                  : model.DoesApply
              : File.ReadAllText(Path.Combine(basePath, model.ApplyScript)),
          model.DependsOn,
          model.Provides);

}
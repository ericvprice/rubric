using System.IO;

namespace Rubric.Scripting;

public static class RuleLoader
{

  public static IAsyncRule<X> LoadFromModel<X>(RuleModel model, string basePath)
   => new ScriptedRule<X>
      (
        model.Name,
        File.ReadAllText(Path.Combine(basePath, model.Script)),
        model.DependsOn,
        model.Provides
      );

  public static IAsyncRule<X, Y> LoadFromModel<X, Y>(RuleModel model, string basePath)
    => new ScriptedRule<X, Y>(
          model.Name,
          File.ReadAllText(Path.Combine(basePath, model.Script)),
          model.DependsOn,
          model.Provides);

}
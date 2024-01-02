[![Build](https://github.com/ericvprice/rubric/actions/workflows/build.yaml/badge.svg?branch=master)](https://github.com/ericvprice/rubric/actions/workflows/build.yaml)    ![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)
# Rubric

Rubric.Extensions is a .NET library providing dependency injection extensions
for Rubric compatible with the Microsoft.Extensions.DependencyInjection framework.

## Usage

* `AddRuleEngine*(...)` - Inject a rule engine implementation with the given types.  A lambda to configure the builder may be provided to configure the engine.  Async versions are provided.
* `AddRules*(Assembly a = null)` - Scan the given assembly, or if null is provided, the assemblies containing the provided types in the generic arguments, for rules compatible with engines of that type.
* `AddScriptedRules*(IConfiguration config, string section)` - Load rules for engines of this type from external `.csx` scripts in the given configuration section.

## Scripted Rules

Rules can be loaded from CSX files.  Assembly loading is ignored when processing the script,
and references to Rubric and the assemblies of the type arguments are automatically added to the script
compilation context.  The options for compilation can be customized when you add it to the ServiceCollection.
Paths in the configuration files are relative to the app's ContentPath (i.e. their root directory is the same as the root directory for your config file paths).

Example JSON configuration:
```json
{
  "preRules": {
    "rule3": {
      "script": "Data/rule3.csx",
      //Since script files aren't classes, and can't take our custom attributes,
      //we have to specify them in this ruleset configuration.
      "provides": ["dependencyName"],
      "dependsOn": [],
      //Similarly, caching behavior can be set in the JSON configuration
      "cacheBehavior": "[PerExecution|PerInput|None]"
    }
  },
  "rules": {
    "rule4": {
      "script": "Data/rule4.csx"
    }
  },
  "postRules": {
    "rule5": {
      "script": "Data/rule5.csx"
    }
  }
}
```
Example script file:

```csharp
//Note, the references below are at Design time only (to help with Intellisense and the like)
//All reference loading ('#r') commands are stripped from the script before compilation.
//The app tightly controls the build context: you must load code dependencies by setting up ScriptOptions
//in AddScriptedRules.  Rubric automatically adds itself, a few .NET core assemblies,
//and the assemblies of the types provided in the generic signature to the compilation context.
#r "..\bin\Debug\net6.0\Rubric.dll"
#r "..\bin\Debug\net6.0\Rubric.Tests.dll"
using Rubric;
using Rubric.Tests;
using System.Threading;

Task<bool> DoesApply(IEngineContext context, TestInput input, TestOutput output, CancellationToken token)
=> Task.FromResult(input.InputFlag == true);

Task Apply(IEngineContext context, TestInput input, TestOutput output, CancellationToken token)
{ output.TestFlag = true; return Task.CompletedTask; }
```
[![Build](https://github.com/ericvprice/rubric/actions/workflows/build.yaml/badge.svg?branch=master)](https://github.com/ericvprice/rubric/actions/workflows/build.yaml)    ![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)
# Rubric

Rubric.Extensions is a .NET library providing dependency injection extensions
for Rubic comapatible with Microsoft.Extensions.DependencyInjection.

## Usage

* `AddRuleEngine*(...)` - Inject a rule engine implementation with the given types.  A lambda to configure the builder may be provided to configure the engine.  Async versions are provided.
* `AddRules*(Assembly a == null)` - Scan the given assembly, or if null is provided, the assemblies containing the provided types in the generic arguments, for rules compatible with engines of that type.
* `AddScriptedRules*(IConfiguration config, string section)` - Load rules for engines of this type from external `.csx` scripts in the given configuration section.

## Scripted Rules

Rules can be loaded from CSX files.  Assembly loading is ignored when processing the script,
and references to Rubic and the assemblies of the type arguments are automatically added to the script
compilation context.
# RuleEngine

RulesEngine is a library for .NET Standard with support for both synchronous
and asynchronous processing, optional parallelized execution for asynchronous processing,
and rule dependency resolution.

## Concepts
---

### Motivation

A rule engine is, essentially, a turbocharged version of the [strategy pattern](https://en.wikipedia.org/wiki/Strategy_pattern).  It provides a framework for consistently implementing code compliant with [SOLID](https://en.wikipedia.org/wiki/SOLID) (and other) comprehensive architectures:

* Rules should be highly-focused and single-purpose transformations, implementing the [single responsibility principle](https://en.wikipedia.org/wiki/Single_responsibility_principle)
* Rules are composed into engines, and can be easily extended and conditionally added to the engine composition, implementing the [open/closed principle](https://en.wikipedia.org/wiki/Open%E2%80%93closed_principle).
* Rules are injected into engines, implementing the [dependency injection principle](https://en.wikipedia.org/wiki/Dependency_inversion_principle).

By using declarative ordering and conditional injection, extremely complicated conditional and configurable processing can be accomplished in a decomposed fashion while avoiding large methods or large "god" processing classes with many function calls.

Transforming a complicated algorithm into a rules engine implemenation involves, at a high level:

1) Translating input normalization statements into preprocessing rules
2) Translating `if` statements and their bodies, or decomposed methods, into processing rules.
3) Using the engine context to hold the local variables shared by rules that don't appear in the output.
4) Translate output normalization statements into postprocessing rules.

From a development methodology standpoint, a rule engine help decompose complex processing into easily unit-testable rule classes.

### Engines

Engines are created from rules that apply one or several input objects and/or an output object.  Synchronous engines accept rules with synchronous rule implementations, and run their rules one after the other as determined by their dependency ordering.  Asyncronous engines accept rules with both asynchronous and synchronous rule implementations, with the later being wrapped in asynchronous adapters.  Asynchronous engines can optionally execute their rules in parallel, automatically determining what rules can be run in parallel given their dependencies.

### Engine Contexts

Engine contexts act as a per-execution temporary object stash where rules can loosely communicate with each other via a dictionary interface.  In addition, contexts can hold global processing flags passed into the engine at executiong time. Care should be taken when using the stash:

* Dependency relations should be used to ensure value are written before being read
* When using parallelized processing in asynchronous engines, the rule execution order is not guaranteed unless dependency relationships enforce it.  Be aware of possible race conditions.

### Rules

Rules come in three flavors, in both synchrnous and asynchrnous varieties:

1) Preprocessing rules are conditionally applied to input objects before the other rule types are run.
2) Processing rules conditionally apply input objects to the output object after preprocessing rules are run and before postprocessing rules are run.
3) PostProcessing rules conditionally applied to the output object after the other rule types are run.

All rules are constructed from two implemented (or fluently provided) methods:

* `DoesApply` is a predicate function that dynamically determines whether the rule runs.
    * For peprocessing rules, the engine context and the input object being processed are available to determine executability.
    * For postprocessing rules, the engine context and the output object being processed are available to determine executability.
    * For processing rules, the engine context and both the current input object and the output object are availble to detemine executability.
* `Apply` is a processing method that applies the rule.

Rules can be either implemented as classes with declarative dependencies, or built via fluent builders with explicit depencies.  Engine constructors are also provided for convenient usage with dependency injection libraries.

## Usage
---

### Class-based Composition

```csharp

public class MyDefaultPreprocessingRule<MyInputType> : DefaultPreRule<MyInputType>
{
    public void Apply(IEngineContext context, MyInputType input)
    {
        //Unconitionally normalize or preprocess each input object with this rule
    }
}

public class MyRule<MyInputType, MyOutputType> : DefaultRule<MyInputType, MyOutputType>
{
    public void DoesApply(IEngineContext, MyInputType input, MyOutputType output)
    {
        //Determine whether to execute this rule
    }
    public void Apply(IEngineContext context, MyInputType input, MyOutputType output)
    {
        //Apply this rule if does apply returns true for the input and output
    }
}

public class MyDefaulPostRule<MyOutputType> : DefaultRule<MyOutputType> {
    public void Apply(IEngineContext context, OutputType output) {
        //Unconditionally apply this rule to the output for postprocessing
    }
}

//Ensure this always executes after MyDefaultPostRule
[DependsOn(typeof(MyDefaultPostRule))]
public class MyDefaulPostRule2<MyOutputType> : DefaultRule<MyOutputType> {
    public void Apply(IEngineContext context, OutputType output) {
        //Unconditionally apply this rule to the output
    }
}

public MyOutputType ProcessInputsWithDirectConstruction(IEnumearble<MyInputType> inputs) {
    var engine = new RulesEngine<MyInputType, MyOutputType>(
        new [] { new MyDefaultPreprocessingRule() },
        new [] { new MyProcessingRule() },
        new [] {
            new MyDefaultPostProcessingRule2(),
            new MyDefaultPostPreprocessingRule()
        }
    )
    var output = new MyOutputType(...);
    var context = new EngineContext();
    //Inject any global flags or parameters into context
    rulesEngine.Apply(inputs, output, context);
}

public MyOutputType ProcessInputsWithFluentConstruction(IEnumearble<MyInputType> inputs) {
    var engine = EngineBuilder.ForInputAndOutput<MyInputType, MyOutputType>()
                              .WithPreRule(new MyDefaultPreprocessingRule())
                              .WithRule(new MyDefaultProcessingRule())
                              //Even though we add the 2nd rule first, it will run second
                              //due to the dependency attribute
                              .WithPostRule(new MyDefaultPostPreprocessingRule2()}
                              .WithPostRule(new MyDefaultPostPreprocessingRule()}
                              .Build();
    var output = new MyOutputType(...);
    var context = new EngineContext();
    //Inject any global flags or parameters into context
    rulesEngine.Apply(inputs, output, context);
}
 ```

 ### Lambda-based Composition

 ```csharp

public MyOutputType ProcessInputsWithFluentConstruction(IEnumearble<MyInputType> inputs) {
    var engine = EngineBuilder.ForInputAndOutput<MyInputType, MyOutputType>()
                              .WithPreRule("mydefaultprerule")
                                .WithPredicate((context, input) => true)
                                .WithAction((context, input) => {...})
                              .EndRule()
                              //Executes after MyDefaultRule, since it depends
                              //on a defined dependency it provides
                              .WithRule("MyDefaultRule2")
                                .WithPredicate((context, input) => true)
                                .WithAction((context, input, output) => {...})
                                .DependsOn("MyDependency");
                              .EndRule()
                              //Executes after both other rules, since it depends
                              //on the first rule by name, and is added after the second rule
                              .WithRule("MyDefaultRule3")
                                .WithPredicate((context, input) => true)
                                .WithAction((context, input, output) => {...})
                                .DependsOn("MyDefaultRule");
                              .EndRule()
                              .WithRule("MyDefaultRule")
                                .WithPredicate((context, input) => true)
                                .WithAction((context, input, output) => {...})
                                .Provides("MyDependency");
                              .EndRule()
                              .WithPostRule("MyDefaultPostRule")
                                .WithPredicate((context, output) => true)
                                .WithAction((context, output) => {...})
                              .EndRule()
                              .Build();
    var output = new MyOutputType(...);
    var context = new EngineContext();
    //Inject any global flags or parameters into context
    rulesEngine.Apply(inputs, output, context);
}
 ```

Asynchronous engine composition follows analogously.  Set `ProcessInParallel` flag to `true` to enable parallel processing of rules.  By default, the engine executes serailly.

## Debug, Logging, and Exceptions

Once an engine is constructed, you can view a dump of the rules (and their ordering) by examining their properties.  All rules are exposed via appropriately named properties.  In addition, when debugging inside the rules, the engine context will always contain an instance of the engine available for you to inspect under the `ContextKeys.ENGINE` key.

When an exception is thrown during execution, the exception is wrapped in an `EngineHaltException` which provides the current input and output (as applicable to the rule type the exception occured in) as well as the captured engine context.  When executing an asynchronous engine in parallel mode, an `AggregateException` will be thrown instead, with each concurrently executing rule that threw the exception being wrapped in an `EngineHaltException` as above.

The engines also accept a `Microsoft.Extensions.Logging.Abstractions.ILogger` instance and will output trace statements as the execute.  This logger may be accessed through the context with the key `ContextKeys.LOGGER` key within rules.

## License

This project is licensed under the MIT license. See the LICENSE file for more info.
# Rubric

Rubric is a .NET library providing synchronous and asynchronous rule engines with optional parallelized execution and rule dependency resolution.

## Concepts
---

### Motivation

A rule engine is, essentially, a turbocharged version of the [strategy pattern](https://en.wikipedia.org/wiki/Strategy_pattern).  It provides a framework for consistently implementing highly code compliant with [SOLID](https://en.wikipedia.org/wiki/SOLID) (and other) comprehensive architectures, while avoiding traiditional problems of what class should "own" functionality:

* Rules should be highly-focused and single-purpose transformations, implementing the [single responsibility principle](https://en.wikipedia.org/wiki/Single_responsibility_principle)
* Rules are composed into engines, and can be easily extended and conditionally added to the engine composition, implementing the [open/closed principle](https://en.wikipedia.org/wiki/Open%E2%80%93closed_principle).
* Rules are injected into engines, implementing the [dependency injection principle](https://en.wikipedia.org/wiki/Dependency_inversion_principle).

By using declarative ordering and conditional injection, extremely complicated conditional and configurable processing can be accomplished in a decomposed fashion while avoiding large methods or large "god" processing classes with many function calls.

Transforming a complicated algorithm into a rule engine implemenation involves, at a high level:

1) Translating input normalization statements into preprocessing rules
2) Translating `if` statements and their bodies, or decomposed methods, into processing rules.
3) Using the engine context engine if necessary to coordinate and shared data between rules.
4) Translate output normalization statements into postprocessing rules.

### Engines

Engines are created from rules and apply them to one or several input objects and potentially an output object for aggregation.  Synchronous engines accept rules with synchronous rule implementations, and run their rules one after the other as determined by their dependency ordering.  Asyncronous engines accept rules with both asynchronous and synchronous rule implementations, with the later being wrapped in asynchronous adapters.  Asynchronous engines can optionally execute their rules in parallel, automatically determining what rules can be run in parallel given their dependencies, and process items in parallel as well.  Support is offered for consuming `IAsyncEnumerable<T>`.

### Engine Contexts

Engine contexts act as a per-execution temporary object stash where rules can loosely communicate with each other via a dictionary interface.  In addition, contexts can hold global processing flags passed into the engine at executiong time. Care should be taken when using the stash:

* Dependency relations should be used to ensure values are written before being read
* When using parallelized processing in asynchronous engines, the rule execution order is not guaranteed unless dependency relationships enforce it.  Be aware of possible race conditions.

### Rules

Rules come in three flavors, in both synchrnous and asynchronous varieties:

1) Preprocessing rules are conditionally applied to an input object before the other rules types are run on that input object.
2) Processing rules conditionally apply and input object to the output object after preprocessing rules are run and before postprocessing rules are run.
3) PostProcessing rules conditionally applied to the output object after the other rule types are run.

All rules are constructed from two implemented (or fluently provided) methods:

* `DoesApply` is a predicate function that dynamically determines whether the rule runs.
    * For peprocessing rules, the engine context and the input object being processed are available to determine executability.
    * For postprocessing rules, the engine context and the output object being processed are available to determine executability.
    * For processing rules, the engine context and both the current input object and the output object are availble to detemine executability.
* `Apply` is a processing method that applies the rule.

For asynchronous engines, a cancellation token is provided as well.

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

Asynchronous engine composition follows analogously.  Set `ProcessInParallel` flag to `true` to enable parallel processing of rules.  By default, the engine executes serially.  Parallelization of inputs is enabled in the `Apply*` calls.

## Exceptions

When an non-engine exception is thrown during execution, the exception is wrapped is passed to an optionally provided exception handler (by default, the exception is not caught).  Users may short-circuit engine execution using `ItemHaltException` and `EngineHaltException` which will halt the execution of the current item or the entire engine's execution, respectively.  These can be thrown directly from the rules, or from a custom exception handler.  The engine will populate these 2 known exceptions with contextual information, and place the exception the `LastException` property.  These exceptions are not rethrown.

In asynchronous engines, all the above statements apply, except that if one is executing either rules (for `ItemHaltException`) or inputs (for `EngineHaltException`) asynchronously, the engine will attempt to cancel all other tasks being executed in parallel.  Long running rules can check the cancellation token (or pass it to other async functions) to allow graceful exiting.  The engine can guarantee that:

1) As soon as any `*HaltException` is encountered, any parallized tasks have their cancellation requested through their token.
2) All internal paralllized tasks in the engine check the cancellation token before executing each piece of user code, and will exit as soon as possible.

`TaskCancelledException`s caused by user-cancellation of the provided token will be bubbled up and not processed by the exception handling mechanism.  The user should handle this exception appropriately.  Uncaught `TaskCancellationException`s not arising from the same token provided by the user will be processed as any other normal exception.

## Logging and Debugging

The engines accept a `Microsoft.Extensions.Logging.Abstractions.ILogger` instance and will output trace statements as they execute user code.  This logger may be accessed through the context with the key `ContextKeys.LOGGER` key within rules.

## License

This project is licensed under the MIT license. See the LICENSE file for more info.
![Build](https://github.com/ericvprice/rubric/actions/workflows/build.yaml/badge.svg?branch=develop)   ![100% code coverage](https://img.shields.io/badge/Code%20Coverage-100%-brightgreen.svg)    ![.Net Standard: 2.1](https://img.shields.io/badge/netstandard-2.1-blue.svg)    ![C#10](https://img.shields.io/badge/c%23-10-blue.svg)   ![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)


# Rubric

Rubric is a .NET library providing synchronous and asynchronous rule engines.

Features include:
* parallel rule and item processing (for asynchronous engines)
* rule dependency resolution
* three independent stages of rules (preprocessing, processing, and postprocessing)
* conveient dependency injection options and a fluent builder interface
* short-circuiting exceptions to halt the processing of an item or the entire engine
* user-injected exception handling

## Motivation

A rule engine is essentially a version of the [strategy pattern](https://en.wikipedia.org/wiki/Strategy_pattern) where the strategies and their selections are combined into composable rules.  It provides a framework for consistently implementing complex code in [SOLID](https://en.wikipedia.org/wiki/SOLID) architectures, while avoiding traditional problems of what class should "own" functionality. [This excellent series of blog posts by Eric Lippert](https://ericlippert.com/2015/04/27/wizards-and-warriors-part-one/) lays out the sorts of complex domain problems that can be solved by a rule engine.

## Concepts

### Engines

Engines are created from rules and apply them to one or several input objects and potentially an output object.  Synchronous engines run their rules sequentially as determined by their dependency ordering.  Asyncronous engines can execute in parallel, automatically determining what rules can be run in parallel given their dependencies, and process items in parallel as well.

### Engine Contexts

Engine contexts act as a holder of conveient properties about the engine current executing and  as a per-execution temporary object stash where rules can loosely communicate with each other.  When using parallelized processing in asynchronous engines, the rule execution order is not guaranteed unless dependency relationships specify them: be aware of possible race conditions when dealing wiht the context.

### Rules

Rules come in three type, in both synchronous and asynchronous varieties.  Engines will execute the rules in this order, and all rules of one type are executed before the next type is run:

1) Preprocessing rules are conditionally applied to an input object.
2) Processing rules conditionally apply an input object to the output object.
3) Postprocessing rules conditionally apply to the output object.


All rules are constructed from two implemented (or fluently provided) methods:

* `DoesApply` is the predicate function that dynamically determines whether the rule runs.
* `Apply` is the processing method that applies the rule.

Rules can be either implemented as classes with declarative dependencies, or built via fluent builders with explicit dependencies.  Engine constructors are also provided for convenient usage with dependency injection libraries, and are highly recommended for most scenarios.  A [companion library](/src/Rubric.Extensions/README.md) is provided for integration with the [`Microsoft.Extensions.DependencyInjection`](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) library.

## Usage
---

### Class-based Composition (via constructor or fluent composition)

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

Asynchronous engine composition follows analogously.

## Exception Handling

When an non-engine exception is thrown during execution, the exception is wrapped and passed to an optionally provided exception handler.  Users may short-circuit engine execution using `ItemHaltException` and `EngineHaltException` which will halt the execution of the current item or the entire engine's execution, respectively.  These can be thrown directly from the rules, or from a custom exception handler.  The engine will populate these 2 known exceptions with contextual information, and place the exception the `LastException` property.  These exceptions are not rethrown.

In asynchronous engines, all the above statements apply, except that if one is executing either rules (for `ItemHaltException`) or inputs (for `EngineHaltException`) in parallel, the engine will attempt to cancel all other tasks being executed in parallel.  Long running rules can check the cancellation token (or pass it to other async functions) to allow graceful exiting.  The engine guarantees that:

1) As soon as any `*HaltException` is encountered, any parallized tasks have their cancellation requested through their token.
2) All internal parallelized tasks in the engine check the cancellation token before executing each piece of user code, and will exit as soon as possible.

`TaskCancelledException`s caused by user-cancellation of the provided token will not be processed by the exception handling mechanism.  The user should handle this exception appropriately.  Uncaught `TaskCancellationException`s not arising from the same token provided by the user will be processed as any other normal exception.

## Logging

The engines accept an optional `Microsoft.Extensions.Logging.Abstractions.ILogger` instance and will output trace statements as they execute user code.  Engines will set context information in the logger about the execution status.  You can access this logger via the context in your rules.

## License

This project is licensed under the MIT license. See the LICENSE file for more info.
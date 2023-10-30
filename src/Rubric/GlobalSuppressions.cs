// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Security", "CA5394:Do not use insecure randomness", Justification = "Non-secure applications only", Scope = "type", Target = "~T:Rubric.Engines.Probabilistic.ProbabilisticEngineExtensions")]
[assembly: SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Closed in continuations", Scope = "namespaceanddescendants", Target = "~N:Rubric.Engines")]
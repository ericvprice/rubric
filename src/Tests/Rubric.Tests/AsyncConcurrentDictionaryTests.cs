namespace Rubric.Tests;

public class AsyncConcurrentDictionaryTests
{

  // Warnings disabled to ensure we are directly calling desired IEnumerable properties.
#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.
#pragma warning disable xUnit2017 // Do not use Contains() to check if a value exists in a collection
  [Fact]
  public void SmokeTests()
  {
    var dict = new AsyncConcurrentDictionary<string, string>();
    Assert.Empty(dict);
    Assert.Equal(0, dict.Count);
    dict.Add("test", "value");
    Assert.NotEmpty(dict);
    Assert.Equal(1, dict.Count);
    Assert.True(dict.ContainsKey("test"));
    Assert.True(dict.TryGetValue("test", out _));
    dict.Remove("test");
    Assert.Empty(dict);
    var kvp = new KeyValuePair<string, string>("key", "value");
    dict.Add(kvp);
    Assert.True(dict.Contains(kvp));
    dict.Remove(kvp);
    Assert.Empty(dict);
    dict["test"] = "value";
    Assert.Equal("value", dict["test"]);
    Assert.NotEmpty(dict.Keys);
    Assert.NotEmpty(dict.Values);
    var array = new KeyValuePair<string, string>[1];
    dict.CopyTo(array, 0);
    Assert.NotEmpty(array);
    dict.Clear();
    Assert.Empty(dict);
    Assert.False(dict.IsReadOnly);
    Assert.NotNull(dict.GetEnumerator());
  }

  [Fact]
  public void ExceptionTests()
  {
    var dict = new AsyncConcurrentDictionary<string, string>();
    Assert.Throws<ArgumentNullException>(() => dict.GetOrAdd("test", null));
    Assert.ThrowsAsync<ArgumentNullException>(() => dict.GetOrAddAsync("test", null));
  }

  [Fact]
  public void CachedGet()
  {
    var counter = 0;
    var dict = new AsyncConcurrentDictionary<string, string>();
    dict.GetOrAdd("key", _ =>
    {
      counter++;
      return "value";

    });
    dict.GetOrAdd("key", _ =>
    {
      counter++;
      return "value";

    });
    Assert.Equal(1, counter);
    Assert.Equal("value", dict["key"]);
  }

  [Fact]
  public async Task CachedGetAsync()
  {
    var counter = 0;
    var dict = new AsyncConcurrentDictionary<string, string>();
    await dict.GetOrAddAsync("key", _ =>
    {
      counter++;
      return Task.FromResult("value");

    });
    await dict.GetOrAddAsync("key", _ =>
    {
      counter++;
      return Task.FromResult("value");

    });
    Assert.Equal(1, counter);
    Assert.Equal("value", dict["key"]);
  }
}
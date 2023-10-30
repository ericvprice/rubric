namespace Rubric.Tests;

[PredicateCaching(CacheBehavior.PerInput)]
public class TestCacheBehavior
{
}
[PredicateCaching(CacheBehavior.PerInput, "testkey")]
public class TestCacheBehavior2
{
}

public class CacheBehaviorAttribute
{
  [Fact]
  public void CacheAttributeDefaultKey()
  {
    var cacheBehvaior = typeof(TestCacheBehavior).GetPredicateCaching();
    Assert.Equal(CacheBehavior.PerInput, cacheBehvaior.Behavior);
    Assert.Equal(typeof(TestCacheBehavior).FullName, cacheBehvaior.Key);
  }

  [Fact]
  public void CacheAttributeCustomKey()
  {
    var cacheBehvaior = typeof(TestCacheBehavior2).GetPredicateCaching();
    Assert.Equal(CacheBehavior.PerInput, cacheBehvaior.Behavior);
    Assert.Equal("testkey", cacheBehvaior.Key);
  }

  [Fact]
  public void PredicateCachingEquality()
  {
    var first = new PredicateCaching();
    var second = new PredicateCaching();
    Assert.True(first.Equals(second));
    Assert.True(first.Equals((object)second));
    Assert.True(first == second);
    Assert.False(first != second);
    Assert.False(first.Equals(new object()));
    Assert.Equal(first.GetHashCode(), second.GetHashCode());
    Assert.NotEqual(0, new PredicateCaching(CacheBehavior.None, "").GetHashCode());
  }
}


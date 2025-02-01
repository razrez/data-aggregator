namespace DataProvider.API.Models
{
  public class InfinispanSettings
  {
    public string Url { get; set; } = "http://infinispan:11222";
    public string User { get; set; } = "infinispan";
    public string Password { get; set; } = "secret";
    public string CacheName { get; set; } = "cache-name";
  }
}
using DataProvider.API.Models;

namespace DataProvider.API.Services;

public interface IInfinispanService
{
  Task<Post?> GetPostByKey(string key);
  Task<List<string>> GetAllKeys(int start = 0, int max = 999999);
}


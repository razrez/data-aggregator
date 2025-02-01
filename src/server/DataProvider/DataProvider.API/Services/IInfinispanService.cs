using DataProvider.API.Models;

namespace DataProvider.API.Services;

public interface IInfinispanService
{
  Task<Post?> GetPostByKey(long key);
  Task<List<long>> GetAllKeys(int start = 0, int max = 999999);
  Task PutPostByKey(long key, Post post);
}


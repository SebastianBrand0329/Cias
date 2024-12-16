using ApiCursosCias.Services.Helpers.Extension;
using System.Diagnostics;
using System.Runtime.Caching;

namespace ApiCursosCias.Services.Helpers.CacheMemory;

public interface ICacheService
{
    T GetData<T>(string request = null);

    bool SetData<T>(T value, string request = null);

    object RemoveData();
}

public class CacheService : ICacheService
{
    private readonly ObjectCache _memoryCache = MemoryCache.Default;

    public T GetData<T>(string request = null)
    {
        try
        {
            string key = $"{request ?? string.Empty}{new StackTrace().GetFrame(1).GetMethod().GetRealMethodFromAsyncMethod()?.Name?.ToLower()}";
            T item = (T)_memoryCache.Get(key);
            return item;
        }
        catch
        {
            throw;
        }
    }

    public bool SetData<T>(T value, string request = null)
    {
        var res = true;
        try
        {
            string key = $"{request ?? string.Empty}{new StackTrace().GetFrame(1).GetMethod().GetRealMethodFromAsyncMethod()?.Name?.ToLower()}";
            DateTimeOffset expirationTime = DateTimeOffset.UtcNow.AddMinutes(2);
            if (!string.IsNullOrEmpty(key))
                _memoryCache.Set(key, value, expirationTime);
            else
                res = false;

            return res;
        }
        catch
        {
            throw;
        }
    }

    public object RemoveData()
    {
        var res = true;
        try
        {
            string key = new StackTrace().GetFrame(1).GetMethod().GetRealMethodFromAsyncMethod()?.Name?.ToLower();
            if (!string.IsNullOrEmpty(key))
                _memoryCache.Remove(key);
            else
                res = false;

            return res;
        }
        catch
        {
            throw;
        }
    }
}
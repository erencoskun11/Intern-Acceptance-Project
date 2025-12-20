namespace Application.Services.Interfaces
{
    public interface ICacheService
    {
        bool TryGet<T>(string key, out T value);

        void Set<T>(string key, T value, TimeSpan? expiration = null);

        void Remove(string key);
    }
}

using System.Threading.Tasks;

namespace Core
{
    public interface IStorageRepository
    {
        Task<string> SaveMessageToStorage<T>(T message);

        Task<T> RestoreMessageFromStorage<T>(string jsonFileName);
    }
}
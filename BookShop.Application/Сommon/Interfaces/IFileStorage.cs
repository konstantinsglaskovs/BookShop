using ErrorOr;

namespace BookShop.Application.Сommon.Interfaces;

public interface IFileStorage
{
    Task<ErrorOr<Success>> UploadAsync(Stream stream, string storageKey, string contentType);

    Task<ErrorOr<Success>> DeleteAsync(string storageKey);

    string GetPublicUrl(string storageKey);
}

using ErrorOr;
using FileSignatures;

namespace BookShop.Web.Validators.Images
{
    public class ImageFileValidator
    {
        private const long MaxFileSize = 10 * 1024 * 1024;

        private readonly IFileFormatInspector _fileFormatInspector;

        public ImageFileValidator(IFileFormatInspector fileFormatInspector)
        {
            _fileFormatInspector = fileFormatInspector;
        }

        public ErrorOr<ValidatedImageFile> Validate(IFormFile file)
        {
            if (file.Length == 0)
            {
                return Error.Validation(
                    code: "Image.Empty",
                    description: "The uploaded image is empty.");
            }

            if (file.Length > MaxFileSize)
            {
                return Error.Validation(
                    code: "Image.TooLarge",
                    description: "The image must not exceed 10 MB.");
            }

            using var stream = file.OpenReadStream();

            var format = _fileFormatInspector.DetermineFileFormat(stream);

            if (format is null)
            {
                return Error.Validation(
                    code: "Image.InvalidFormat",
                    description: "The uploaded file is not a valid image.");
            }

            return format.MediaType switch
            {
                "image/jpeg" => new ValidatedImageFile
                {
                    Extension = ".jpg",
                    ContentType = "image/jpeg"
                },

                "image/png" => new ValidatedImageFile
                {
                    Extension = ".png",
                    ContentType = "image/png"
                },

                "image/webp" => new ValidatedImageFile
                {
                    Extension = ".webp",
                    ContentType = "image/webp"
                },

                _ => Error.Validation(
                    code: "Image.UnsupportedFormat",
                    description: "Only JPEG, PNG and WebP images are supported.")
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace AttendEdgeWebService.Infrastructure.Utils
{
    public class StaticMethods
    {
        public static string[] SplitString(string inputtedString, char separation)
        {
            if (inputtedString.IsNullOrEmpty())
                return null;

            return inputtedString.Trim().Split(separation);
        }

        public static string SaveBase64Image(string base64String, string relativeFolder, string uniqueFileName)
       {
            if (string.IsNullOrWhiteSpace(base64String) || string.IsNullOrWhiteSpace(relativeFolder))
                throw new ArgumentException("Invalid input parameters.");

            // Strip metadata if present
            string base64Data = Regex.Replace(base64String, @"^data:image\/[a-zA-Z]+;base64,", string.Empty);

            // Convert to byte array
            byte[] imageBytes = Convert.FromBase64String(base64Data);

            // Resolve physical path
            string physicalFolderPath = HostingEnvironment.MapPath(relativeFolder);
            if (!Directory.Exists(physicalFolderPath))
                Directory.CreateDirectory(physicalFolderPath);

            // Build full file path
            string fullFilePath = Path.Combine(physicalFolderPath, uniqueFileName);

            // Save to disk
            File.WriteAllBytes(fullFilePath, imageBytes);

            // Return relative path for DB or client use
            return Path.Combine(relativeFolder.TrimStart('~', '/'), uniqueFileName).Replace("\\", "/");
        }
        public static string GetImageBase64(string imagePath)
        {
            if (!File.Exists(imagePath))
                return null;

            byte[] bytes = File.ReadAllBytes(imagePath);
            return Convert.ToBase64String(bytes);
        }

        public static string GeneratePassword(int length = 12, bool useUppercase = true, bool useLowercase = true, bool useNumbers = true, bool useSpecial = true)
        {
            const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowercase = "abcdefghijklmnopqrstuvwxyz";
            const string numbers = "0123456789";
            const string special = "!@#$%^&*()-_=+[]{}|;:,.<>?";

            StringBuilder charSet = new StringBuilder();
            if (useUppercase) charSet.Append(uppercase);
            if (useLowercase) charSet.Append(lowercase);
            if (useNumbers) charSet.Append(numbers);
            if (useSpecial) charSet.Append(special);

            if (charSet.Length == 0)
                throw new ArgumentException("At least one character set must be enabled.");

            Random rng = new Random();
            return new string(Enumerable.Range(0, length)
                .Select(_ => charSet[rng.Next(charSet.Length)])
                .ToArray());
        }

        public static string GetImageTypeFromBase64(string base64String)
        {
            if (string.IsNullOrWhiteSpace(base64String))
                return "Invalid input";

            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64String);

                // Check for PNG
                if (imageBytes.Length > 8 &&
                    imageBytes[0] == 0x89 && imageBytes[1] == 0x50 &&
                    imageBytes[2] == 0x4E && imageBytes[3] == 0x47 &&
                    imageBytes[4] == 0x0D && imageBytes[5] == 0x0A &&
                    imageBytes[6] == 0x1A && imageBytes[7] == 0x0A)
                    return "PNG";

                // Check for JPEG
                if (imageBytes.Length > 3 &&
                    imageBytes[0] == 0xFF && imageBytes[1] == 0xD8 &&
                    imageBytes[2] == 0xFF)
                    return "JPEG";

                // Check for GIF
                if (imageBytes.Length > 6 &&
                    imageBytes[0] == 0x47 && imageBytes[1] == 0x49 &&
                    imageBytes[2] == 0x46 && imageBytes[3] == 0x38 &&
                    (imageBytes[4] == 0x39 || imageBytes[4] == 0x37) &&
                    imageBytes[5] == 0x61)
                    return "GIF";

                // Check for BMP
                if (imageBytes.Length > 2 &&
                    imageBytes[0] == 0x42 && imageBytes[1] == 0x4D)
                    return "BMP";

                // Check for WEBP (RIFF header + WEBP signature)
                if (imageBytes.Length > 12 &&
                    imageBytes[0] == 0x52 && imageBytes[1] == 0x49 &&
                    imageBytes[2] == 0x46 && imageBytes[3] == 0x46 &&
                    imageBytes[8] == 0x57 && imageBytes[9] == 0x45 &&
                    imageBytes[10] == 0x42 && imageBytes[11] == 0x50)
                    return "WEBP";

                return "Unknown";
            }
            catch
            {
                return "Invalid base64";
            }
        }

    }
}

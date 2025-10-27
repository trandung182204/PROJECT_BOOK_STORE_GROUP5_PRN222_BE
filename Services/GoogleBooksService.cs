using System.Net;
using System.Text.Json;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Repositories;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public class GoogleBooksService
    {
        private readonly IBookRepository _bookRepo;
        private readonly HttpClient _httpClient;

        public GoogleBooksService(IBookRepository bookRepo, IHttpClientFactory httpClientFactory)
        {
            _bookRepo = bookRepo;
            _httpClient = httpClientFactory.CreateClient();
        }

        // Import theo keyword
        public async Task<int> ImportBooksFromGoogleAsync(string keyword, int maxResults = 10)
        {
            string url = $"https://www.googleapis.com/books/v1/volumes?q={WebUtility.UrlEncode(keyword)}&maxResults={maxResults}";
            return await ImportBooksFromUrlAsync(url);
        }

        // Import theo category (subject)
        public async Task<int> ImportBooksByCategoryAsync(string category, int maxResults = 10)
        {
            string url = $"https://www.googleapis.com/books/v1/volumes?q=subject:{WebUtility.UrlEncode(category)}&maxResults={maxResults}";
            return await ImportBooksFromUrlAsync(url);
        }

        // Core logic
        private async Task<int> ImportBooksFromUrlAsync(string url)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("items", out var items))
                return 0;

            int count = 0;

            foreach (var item in items.EnumerateArray())
            {
                var volumeInfo = item.GetProperty("volumeInfo");

                // ✅ Bỏ qua sách không có ảnh
                if (!volumeInfo.TryGetProperty("imageLinks", out var img))
                    continue;

                if (!img.TryGetProperty("thumbnail", out var thumb))
                    continue;

                string? thumbnailUrl = thumb.GetString();
                if (string.IsNullOrEmpty(thumbnailUrl))
                    continue;

                var book = new Book
                {
                    Code = item.GetProperty("id").GetString() ?? Guid.NewGuid().ToString(),
                    Title = volumeInfo.TryGetProperty("title", out var t) ? t.GetString() ?? "Unknown" : "Unknown",
                    Author = volumeInfo.TryGetProperty("authors", out var authors)
                                ? string.Join(", ", authors.EnumerateArray().Select(a => a.GetString()))
                                : "Unknown",
                    Publisher = volumeInfo.TryGetProperty("publisher", out var p) ? p.GetString() ?? "" : "",
                    PublicationYear = DateTime.Now.Year,
                    Description = volumeInfo.TryGetProperty("description", out var desc) ? desc.GetString() ?? "" : "",
                    ThumbnailUrl = thumbnailUrl, // ✅ đảm bảo có ảnh thật
                    Language = volumeInfo.TryGetProperty("language", out var lang) ? lang.GetString() ?? "" : "",
                    Isbn = GetIsbnFromVolume(volumeInfo),
                    Price = Random.Shared.Next(10, 100),
                    DiscountPrice = 0,
                    StockQuantity = 10,
                    CreatedAt = DateTime.Now,
                    Status = "active"
                };

                await _bookRepo.AddIfNotExistsAsync(book);
                count++;
            }

            return count;
        }

        private string? GetIsbnFromVolume(JsonElement volume)
        {
            if (!volume.TryGetProperty("industryIdentifiers", out var ids))
                return null;

            foreach (var id in ids.EnumerateArray())
            {
                if (id.TryGetProperty("identifier", out var value))
                    return value.GetString();
            }

            return null;
        }
    }
}

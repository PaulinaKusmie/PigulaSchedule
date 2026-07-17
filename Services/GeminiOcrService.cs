using PigulaSchedule.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PigulaSchedule.Services
{
    public class GeminiOcrService : IGeminiOcrService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public GeminiOcrService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        private async Task<string> RecognizeScheduleAsync(byte[] imageBytes)
        {
            var client = _httpClientFactory.CreateClient("GeminiClient");
            var base64 = Convert.ToBase64String(imageBytes);

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new object[]
                        {
                            new
                            {
                                inline_data = new
                                {
                                    mime_type = "image/jpeg",
                                    data = base64
                                }
                            },
                            new
                            {

                                text =
                                @"Odczytaj grafik pracy z tabeli.

                                            Zasady:

                                            1.Pierwszy wiersz tabeli zawiera daty.
                                            2.Trzeci wiersz tabeli zawiera zmiany.
                                            3.Każda kolumna odpowiada jednej dacie.
                                            4.Dla każdej kolumny odczytaj wartość z trzeciego wiersza.

                                            Konwersje:

                                            EDn->ED
                                            ENn->EN
                                            E2->E1
                                            puste pole -> DW
                                            W->W

                                            Nigdy nie przesuwaj wartości do sąsiedniej kolumny.

                                            Każda data z pierwszego wiersza musi wystąpić dokładnie raz.

                                            Liczba wynikowych wierszy ma być równa liczbie dat.

                                            Zwróć wyłącznie wynik:

                                                        DD.MM | ZMIANA

                                            Przykład:

                                                        01.05 | ED
                                            02.05 | EN
                                            03.05 | DW
                                            04.05 | W
                                            05.05 | E1

                                            Nie dodawaj żadnych komentarzy ani wyjaśnień"


                            }
                        }
                    }
                }
            };

            var json = System.Text.Json.JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var apiKey = Constans.APIKey;
            var response = await client.PostAsync(
                $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}",
                content);

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);

            return doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? "";
        }

        Task<string> IGeminiOcrService.RecognizeScheduleAsync(byte[] imageBytes)
        {
            return RecognizeScheduleAsync(imageBytes);
        }
    }
}

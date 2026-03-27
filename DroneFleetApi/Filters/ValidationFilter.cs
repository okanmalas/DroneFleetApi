using System.ComponentModel.DataAnnotations;

namespace DroneFleetApi.Filters;

// <T> ifadesi, bu filtrenin DroneDTO, FlightLogDTO gibi her türlü sınıf için çalışabilmesini sağlar (Joker eleman).
public class ValidationFilter<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // 1. Gelen isteğin içinden bizim DTO çantamızı (T) bul
        var dto = context.Arguments.OfType<T>().FirstOrDefault();

        if (dto is not null)
        {
            var hatalar = new List<ValidationResult>();
            var kontrolMerkezi = new ValidationContext(dto);
            
            // 2. DTO'nun içindeki [Required], [MaxLength] gibi kuralları kontrol et
            bool gecerliMi = Validator.TryValidateObject(dto, kontrolMerkezi, hatalar, true);

            if (!gecerliMi)
            {
                // 3. Eğer kural ihlali varsa, kapıyı kapat ve kullanıcıya şık bir 400 Bad Request dön
                var hataMesajlari = hatalar.Select(h => h.ErrorMessage).ToList();
                return Results.BadRequest(new { Mesaj = "Veri Doğrulama Hatası!", Detaylar = hataMesajlari });
            }
        }

        // 4. Her şey kurallara uygunsa, kapıyı aç ve isteğin asıl Endpoint'e gitmesine izin ver
        return await next(context);
    }
}
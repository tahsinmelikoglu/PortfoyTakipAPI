namespace PortfoyTakipAPI.DTOs
{
    public record VarlikRequestParameters(
        int PageNumber = 1,
        int PageSize = 10,
        string? SearchTerm = null,
        string? SortBy = "Id", // Varsayılan sıralama kolonu
        bool IsDescending = false
    );
}


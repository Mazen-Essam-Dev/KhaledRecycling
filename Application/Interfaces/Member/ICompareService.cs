
using Domain.DTOs;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Member
{
    public interface ICompareService
    {
        Task<int> LevenshteinDistance(string s, string t);
        Task<double> SimilarityPercentage(string? s, string? t);

    }
}

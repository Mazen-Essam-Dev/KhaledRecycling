using Application.Interfaces.Member;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Member
{
    public class CompareService :ICompareService
    {
        public async Task<int> LevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0) return m;
            if (m == 0) return n;

            for (int i = 0; i <= n; d[i, 0] = i++) { }
            for (int j = 0; j <= m; d[0, j] = j++) { }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }

        public async Task<double> SimilarityPercentage(string? s, string? t)
        {
            if (s == null || t == null) return (double)0;
            int distance = await LevenshteinDistance(s, t);
            int maxLen = Math.Max(s.Length, t.Length);
            return (1.0 - (double)distance / maxLen) * 100;
        }
    }
}

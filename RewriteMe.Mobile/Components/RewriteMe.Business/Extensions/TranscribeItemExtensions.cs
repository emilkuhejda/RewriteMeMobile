using System;
using System.Linq;
using RewriteMe.Domain.WebApi;

namespace RewriteMe.Business.Extensions
{
    public static class TranscribeItemExtensions
    {
        public static int ToAverageConfidence(this TranscribeItem transcribeItem)
        {
            var alternatives = transcribeItem.Alternatives.ToList();
            var averageConfidence = alternatives.Sum(x => x.Confidence) / alternatives.Count;

            return (int)Math.Round(averageConfidence * 100);
        }
    }
}

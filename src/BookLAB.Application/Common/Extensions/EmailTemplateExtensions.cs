using System.Text.Json;
using BookLAB.Domain.Entities;

namespace BookLAB.Application.Common.Extensions
{
    public static class EmailTemplateExtensions
    {
        public static List<string> GetVariables(
            this EmailTemplate template)
        {
            if (string.IsNullOrWhiteSpace(template.VariablesJson))
                return [];

            return JsonSerializer.Deserialize<List<string>>(
                template.VariablesJson
            ) ?? [];
        }
    }
}
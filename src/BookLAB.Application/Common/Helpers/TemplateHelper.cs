namespace BookLAB.Application.Common.Helpers
{
    public static class TemplateHelper
    {
        public static string PopulateTemplate(string template, Dictionary<string, string> values)
        {
            foreach (var item in values)
            {
                template = template.Replace($"{{{{{item.Key}}}}}", item.Value);
            }
            return template;
        }
    }
}

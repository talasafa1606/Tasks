namespace Task1Bank.Middlewares;

public class LanguageMiddleware
{
    private readonly RequestDelegate _next;
    private const string DefaultLanguage = "en";
    private readonly string[] _supportedLanguages = { "en", "fr", "de" };
    
    public LanguageMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("Accept-Language", out var headerLanguage))
        {
            context.Items["Language"] = DefaultLanguage;
        }
        else
        {
            var languages = headerLanguage.ToString().Split(',');
            var primaryLanguage = languages.FirstOrDefault()?.Split(';').FirstOrDefault()?.Split('-').FirstOrDefault();
            
            context.Items["Language"] = _supportedLanguages.Contains(primaryLanguage) ? primaryLanguage : DefaultLanguage;
        }
        
        await _next(context);
    }
}
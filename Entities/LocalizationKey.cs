namespace Task1Bank.Entities;

public class LocalizationKey
{
    public int Id { get; set; }
    public string Key { get; set; }
    public string EnglishText { get; set; }
    public string SpanishText { get; set; }
    public string FrenchText { get; set; }
    
    public string GetTranslation(string languageCode)
    {
        return languageCode.ToLower() switch
        {
            "es" => SpanishText ?? EnglishText, 
            "fr" => FrenchText ?? EnglishText,  
            _ => EnglishText                    
        };
    }
}
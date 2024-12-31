namespace USProApplication.Models.API
{
    public class CompanyData
    {
        public string? ОГРН { get; set; }
        public string? ИНН { get; set; }
        public string? КПП { get; set; }
        public string? НаимСокр { get; set; }
        public JuridicalAddress? ЮрАдрес { get; set; }
        public List<Director>? Руковод { get; set; }
    }
}

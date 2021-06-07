namespace Api.Dto.Base
{
    public abstract class TemplateDto : ITemplateDto
    {
        public bool Estado { get; set; }
        public int Version { get; set; }
    }
}

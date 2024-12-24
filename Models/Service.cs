namespace USProApplication.Models
{
    /// <summary>
    /// Услуга
    /// </summary>
    public class Service
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Аббривиатура
        /// </summary>
        public string Abbreviation { get; set; } = string.Empty;

        /// <summary>
        /// Cтоимость
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; } = string.Empty;

        public Service Clone()
        {
            return new Service
            {
                Id = this.Id,
                Name = this.Name,
                Abbreviation = this.Abbreviation,
                Price = this.Price,
                Description = this.Description
            };
        }
    }
}

namespace ServiceProducts.Domain.Models
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public Category() { }

        public Category(Guid id, string name, string? description = null)
        {
            Id = id;
            Name = name;
            Description = description;
        }
    }
}

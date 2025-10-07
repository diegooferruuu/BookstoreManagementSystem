namespace BookstoreManagementSystem.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string? Description { get; set; } 

        public Category() { }

        public Category(int id, string name, string? description = null)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public override string ToString()
        {
            return $"{Id}: {Name} - {(string.IsNullOrEmpty(Description) ? "Sin descripción" : Description)}";
        }
    }
}

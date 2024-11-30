namespace Hangfire.Database.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class FieldDescriptionAttribute : Attribute
    {
        public FieldDescriptionAttribute(string desc, string? name = null)
        {
            if (name != null)
            {
                Name = name;
            }
            else
            {
                Name = TypeId.ToString() ?? string.Empty;
            }
            Description = desc;
        }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
    }
}

namespace Hangfire.Database.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class TableDescriptionAttribute : Attribute
    {
        public TableDescriptionAttribute(string desc)
        {
            Description = desc;
        }
        public virtual string Description { get; set; }
    }
}

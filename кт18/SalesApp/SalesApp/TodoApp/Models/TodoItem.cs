namespace TodoApp.Models
{
    public class TodoItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = "";
        public bool IsDone { get; set; }
        public Priority Priority { get; set; } = Priority.Medium;
        public DateOnly? DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? CompletedAt { get; set; }
    }

    public enum Priority
    {
        Low,
        Medium,
        High
    }

    public enum TodoFilter
    {
        All,
        Active,
        Done
    }

    public enum TodoSort
    {
        Newest,
        Priority,
        DueDate,
        Title
    }
}

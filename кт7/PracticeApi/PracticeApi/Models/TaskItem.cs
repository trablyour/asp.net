namespace PracticeApi.Models
{
    // не Task, чтобы не путать с System.Threading.Tasks.Task
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public TaskItemStatus Status { get; set; } = TaskItemStatus.New;
        public DateTime CreatedAt { get; set; }
    }

    public enum TaskItemStatus
    {
        New,
        InProgress,
        Done
    }
}

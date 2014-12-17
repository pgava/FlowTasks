namespace Flow.Tasks.Data.Core
{
    public class TaskOutParameter
    {
        public int TaskOutParameterId { get; set; }

        public int PropertyId { get; set; }
        public Property Property { get; set; }

        public int TaskDefinitionId { get; set; }
        public TaskDefinition TaskDefinition { get; set; }
    }
}

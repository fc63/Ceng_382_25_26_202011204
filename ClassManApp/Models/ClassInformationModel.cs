namespace ClassManApp.Models
{
    public class ClassInformationModel
    {
        private static int _nextId = 1;

        public int Id { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public string Description { get; set; } = string.Empty;

        public ClassInformationModel()
        {
            Id = _nextId++;
        }
    }
}
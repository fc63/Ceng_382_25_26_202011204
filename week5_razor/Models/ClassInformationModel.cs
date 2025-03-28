/*
prompt:"Create a new folder named Models in the root of a Razor Pages project. Inside that folder, add a C# class named ClassInformationModel.cs with the following properties:

    int Id → auto-incremented using a static field

    string ClassName

    int StudentCount

    string Description

The constructor should automatically assign a unique Id value by incrementing a static counter. Put this class inside the week5_razor.Models namespace"
*/
namespace week5_razor.Models
{
    public class ClassInformationModel
    {
        private static int _nextId = 1;

        public int Id { get; set; }
        public string ClassName { get; set; }
        public int StudentCount { get; set; }
        public string Description { get; set; }

        public ClassInformationModel()
        {
            Id = _nextId++;
        }
    }
}
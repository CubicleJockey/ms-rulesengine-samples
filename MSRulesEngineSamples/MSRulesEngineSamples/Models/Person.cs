namespace MSRulesEngineSamples.Models
{
    public class Person
    {
        public int Id { get; }
        public string Name { get; }
        public DateTime BirthDate { get; }

        public Person(int id, string name, DateTime birthDate)
        {
            Id = id;
            Name = name;
            BirthDate = birthDate;
        }

        public int Age => (int)((DateTime.Now - BirthDate).TotalDays / 365.242199);
    }
}

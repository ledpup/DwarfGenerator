using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DwarfGenerator
{
    public enum Sex
    {
        Male,
        Female,
    }

    public class Person
    {
        public Person()
        {
            Parents = new List<Person>();
            Partners = new List<Person>();
            Children = new List<Person>();
        }

        public string FirstName;
        public string LastName;
        public Sex Sex;
        public int Age;
        public int Generation;
        public Person LineageParent;
        public List<Person> Partners;
        public List<Person> Parents;
        public List<Person> Children;

        public string Name { get { return FirstName + " " + LastName; } }
        public IEnumerable<Person> Siblings { get { return LineageParent != null ? LineageParent.Children.Where(x => x != this) : new List<Person>(); } }
    }
}

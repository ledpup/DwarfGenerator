using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Tabular;

namespace DwarfGenerator
{
    class Program
    {
        private static Random _random;
        static string[] _maleNames = new[] { "Gris", "Drungen", "Lorik", "Turk", "Tork", "Helk", "Fumis", "Ease", "Junik", "Vikeg", "Kilo", "Urk", "Tallohaz", "Zinc", "Gorn", "Gord", "Ritter", "Pilo", "Xilo", "Crit", "Ori", "Gamil", "Gimli", "Balin", "Durin", "Hutob", "Furi", "Tad", "Gokre", "Dibo", "Yorgur", "Rakri", "Dodob", "Hobbir", "Tolso", "Gohdi", "Gargin", "Durim", "Garrak", "Bgar", "Hartil", "Gilin", "Gruunn", "Fargol", "Thaim", "Chalor", "Dimil" };
        static string[] _femaleNames = new [] { "Bullia", "Gis", "Ula", "Misany", "Tha", "Olcras", "Ley", "Demis", "Xema", "Dapple", "Mase", "Mattle", "Ses", "Bis", "Nuz", "Paspa", "Yis", "Fes", "Cris", "Gamana", "Dís", "Suti", "Ralla", "Quen", "Edis", "Gangla", "Risu", "Nija", "Uana", "Lofera", "Btee", "Flef", "Nohsah", "Keliti", "Rerora", "Dimondi", "Herria", "Ovgini", "Kilria", "Doria", "Stordria", "Thinina", "Runana", "Bofia", "Fardina", "Garzade", "Muskila", "Besil" };
        static string[] _firstPart = new [] { "Stone", "Fire", "Bone", "Metal", "Shield", "Gryphon", "Fungi", "Cave", "Shadow", };
        static string[] _secondPart = new [] { "wall", "axe", "smith", "blade", "door", "cave", "gate", "dungeon", "hide", "helm", "plate", "cutter", "herder", "hunter", "guard", "pit", "pick", "hammer", "sling", "mine" };

        static List<Person> _people;
        private const int _maxGeneration = 4;

        static void Main(string[] args)
        {
            _random = new Random();

            _people = new List<Person>();

            for (var i = 0; i < 200; i++)
                _people.Add(RandomPerson(0));

            CreateGeneration(0, _people);

            var objects = _people.Select(x => new { 
                x.Name,
                x.Sex,
                x.Age,
                x.Generation,
                Parents = string.Join(",", x.Parents.Select(p => p.Name)),
                Partners = string.Join(",", x.Partners.Select(p => p.Name)),
                Children = string.Join(",", x.Children.Select(c => c.FirstName)),
                Siblings = string.Join(",", x.Siblings.Select(s => s.FirstName)),
            });

            using (var demo3TextOut = new StreamWriter("dwarfs.html"))
            {
                TableRenderer.Render(objects, new HtmlTableWriter(demo3TextOut));
            }

            Process.Start("dwarfs.html");
        }

        private static void CreateGeneration(int generation, List<Person> people)
        {

            if (generation >= _maxGeneration)
                return;

            generation++;

            var matureMales = people.Where(x => x.Age > 30 && x.Sex == Sex.Male).ToList();
            var matureFemale = people.Where(x => x.Age > 40 && x.Sex == Sex.Female).ToList();

            matureFemale.ForEach(x => AssignPartners(x, matureMales));
            var potentialMothers = matureFemale.Where(x => x.Partners.Any()).ToList();

            var allChildren = new List<Person>();

            potentialMothers.ForEach(x => allChildren.AddRange(Protegeny(x, generation)));

            CreateGeneration(generation, allChildren);

            _people.AddRange(allChildren);
        }

        private static Person RandomPerson(int generation)
        {
            var sex = (Sex)_random.Next(2);
            return new Person
                             {
                                 Sex = sex,
                                 FirstName = RandomFirstName(sex),
                                 LastName = RandomLastName(),
                                 Age = RandomAge(generation),
                             };
        }

        private static int RandomAge(int generation)
        {
            var age = _random.Next(50);
            for (var i = 0; i < _maxGeneration - generation; i++)
                age += 20 + _random.Next(50);
            return age;
        }

        private static string RandomFirstName(Sex sex)
        {
            return sex == Sex.Male
                       ? _maleNames[_random.Next(_maleNames.Length)]
                       : _femaleNames[_random.Next(_femaleNames.Length)];
        }

        private static string RandomLastName()
        {
            return _firstPart[_random.Next(_firstPart.Length)] + _secondPart[_random.Next(_secondPart.Length)];
        }

        private static IEnumerable<Person> Protegeny(Person mother, int generation)
        {
            var childrenNumber = _random.Next(10);

            for (var i = 0; i < childrenNumber; i++)
            {
                var child = RandomPerson(generation);
                while (mother.Children.Any(x => x.FirstName == child.FirstName))
                    child.FirstName = RandomFirstName(child.Sex);

                child.LastName = mother.LastName;
                child.LineageParent = mother;
                child.Parents.Add(mother);
                var father = RandomFather(mother.Partners);
                child.Parents.Add(father);
                child.Generation = generation;
                mother.Children.Add(child);
                father.Children.Add(child);
            }

            return mother.Children;
        }

        private static Person RandomFather(IList<Person> fathers)
        {
            return fathers[_random.Next(fathers.Count)];
        }

        private static void AssignPartners(Person person, IList<Person> potentialPartners)
        {
            var partners = new List<Person>();
            
            if (!potentialPartners.Any())
                return;

            var numberOfPartners = _random.Next(4);

            for (var i = 0; i < numberOfPartners; i++)
            {
                if (!potentialPartners.Any())
                    break;

                var partner = potentialPartners[_random.Next(potentialPartners.Count)];
                partner.Partners.Add(person);
                partners.Add(partner);
                potentialPartners.Remove(partner);
            }

            person.Partners.AddRange(partners);
        }
    }
}
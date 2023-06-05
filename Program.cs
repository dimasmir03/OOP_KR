using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using static PoolTrackingSystem.Program;

namespace PoolTrackingSystem
{
    class Pool
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Type { get; set; } //sport, medical, combined
        public List<Trainer> Trainers { get; set; }
        public List<Group> Groups { get; set; }
        public List<Subscription> Subscriptions { get; set; }

        public Pool(int id, string name,
                    string address, string type)
        {
            Trainers = new List<Trainer>();
            Groups = new List<Group>();
            Subscriptions = new List<Subscription>();

            Id = id; Name = name;
            Address = address;
            Type = type;
        }

        public void AddTrainer(Trainer trainer1)
        {
            Trainers.Add(trainer1);
            var trainer = Trainers.Single(x => x == trainer1);
            trainer.Pools.Add(this);
            foreach (var group in Groups)
            {
                if (group.Trainer == null)
                {
                    group.Trainer = trainer;
                    trainer.Groups.Add(group);
                    break;
                }
                if (!trainer.Groups.Contains(group)) trainer.Groups.Add(group);
                //Console.WriteLine($"{group.Id} - {group.Trainer.Name}");
            }
        }

        public void AddGroup(Group group)
        {
            Groups.Add(group);
        }
    }

    class Trainer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string[] DaysOfWeek { get; set; }
        public List<Pool> Pools { get; set; }

        public List<Group> Groups { get; set; }
        public List<string> Timetable { get; set; }

        public Trainer(int id, string name,
                       int days)
        {
            Groups = new List<Group>();
            Pools = new List<Pool>();
            Timetable = new List<string>();

            Id = id; Name = name;
            foreach (var d in days.ToString())
            {
                int i = (int)(d - '0');
                Timetable.Add( ((DayWeek)i).ToString() );
            }
        }

        public int GetTotalRevenue(Pool pool)
        {
            return Groups.Where(g => g.Pool == pool).Sum(gs => gs.Subscription.Price * (gs.Participants.Count == 0 ? 0 : gs.Participants.Count));
        }

        public void AddGroup(Group group)
        {
            Groups.Add(group);
            Groups.Single(x => x == group).Trainer = this;
        }

        public void getTimeTable()
        {
            Console.WriteLine("Дни недели:");
            foreach (var t in Timetable)
            {
                Console.WriteLine($"{t} ");
            }
        }
    }


    class Group
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Category { get; set; } //beginners, teenagers, adults, athletes
        public Subscription Subscription { get; set; } //foreign key to Subscription
        public List<Participant> Participants { get; set; }

        private Pool pool;
        public Pool Pool
        {
            get
            {
                return pool;
            }
            set
            {
                pool = value;
                pool.AddGroup(this);
            }
        }
        public Trainer Trainer { get; set; }

        public Group() => Participants = new List<Participant>();

        public Group(int id, string category, Pool pool,
                     Subscription subscriptionid)
        {
            Participants = new List<Participant>();


            Id = id; Category = category;
            Subscription = subscriptionid;
            this.pool = pool;
            this.pool.AddGroup(this);

        }

        public void AddParticipant(Participant participant)
        {
            Participants.Add(participant);
            Participants.Find(f => f == participant).Group = this;
        }
    }

    class Subscription
    {
        public int Id { get; set; }
        public string Type { get; set; } //1 visit per week, 2 visits per week, etc.
        public int NumberOfVisits { get; set; }
        public int Price { get; set; }

        public Subscription(int id, string type, int numofvisits, int price)
        {
            Id = id; Type = type; NumberOfVisits = numofvisits; Price = price;
        }
    }

    class Participant
    {
        int Id { get; set; }
        public string Name { get; set; }
        public Subscription Subscription { get; set; }
        public Pool Pool { get; set; }
        public Group Group { get; set; }
        public Trainer Trainer { get; set; }

        public Participant(int id, string name, Pool pool, Subscription subscription)
        {
            Id = id;
            Name = name; Subscription = subscription;
            Pool = pool;
        }
    }

        /*public static class Category
        {
            public static readonly string Beginers = "Начинающие";
            public static readonly string Teenagers = "Подростки";
            public static readonly string Adults = "Взрослые";
            public static readonly string Athletes = "Спортсмены";
        }*/


    class Program
    {
        static List<Pool> pools = new List<Pool>();
        static List<Trainer> trainers = new List<Trainer>();
        static List<Group> groups = new List<Group>();
        static List<Subscription> subscriptions = new List<Subscription>();

        public static List<string> Category = new List<string>()
        {
            "Начинающие",
            "Подростки",
            "Взрослые",
            "Спортсмены",
        };

        public enum DayWeek
        {
            Воскресенье = 1,
            Понедельник,
            Вторник,
            Среда,
            Четверг,
            Пятница,
            Суббота,
        }

/*        public enum DayWeek
        {
            Sunday = 1,
            Monday,
            Tuesday,
            Wednesday,
            Thursday,
            Friday,
            Saturday,
        }*/

        public static void AddParticipant(Participant participant)
        {
            try
            {
                groups.Where(g => g.Subscription == participant.Subscription).First().AddParticipant(participant);
                //Console.WriteLine($"{participant.Name} - {participant.Group.Id} - {participant.Pool.Name}");
            }
            catch (Exception) { }
        }

        public static void AddGroup()
        {

            Group newgroup = new Group();
            Console.WriteLine("Введите символ '*'отмены...");
            //if (Console.ReadKey(true).KeyChar == '*') return;

            Console.WriteLine("Бассейны: ");
            foreach (var p in pools)
            {
                Console.WriteLine(p.Id + "-" + p.Name);
            }
            Console.WriteLine("Выберите Бассейн группы: ");
            var id = int.Parse(Console.ReadLine());
            newgroup.Pool = pools.Single(p => p.Id == id);
            Console.Clear();

            Console.WriteLine("Категории групп: ");
            for (int i = 0; i < Category.Count; i++)
            {
                Console.WriteLine(i + "-" + Category[i]);
            }
            Console.WriteLine("Выберите категорию группы: ");
            newgroup.Category = Category[int.Parse(Console.ReadLine())];
            Console.Clear();

            Console.WriteLine("Тренеры: ");
            foreach (var t in trainers.Where(t => t.Pools.Contains(newgroup.Pool)))
            {
                Console.WriteLine(t.Id + "-" + t.Name);
            }
            Console.WriteLine("Выберите Тренера для группы: ");
            id = int.Parse(Console.ReadLine());
            newgroup.Trainer = trainers.Single(p => p.Id == id);
            Console.Clear();

            Console.WriteLine("Абонементы: ");
            foreach (var s in subscriptions)
            {
                Console.WriteLine(s.Id + "-" + s.Type);
            }
            Console.WriteLine("Выберите Абонемент группы: ");
            id = int.Parse(Console.ReadLine());
            newgroup.Subscription = subscriptions.Single(p => p.Id == id);
            newgroup.Id = groups.Last().Id + 1;

            groups.Add(newgroup);
            Console.WriteLine("Новая группа добавлена");
        }

        public static void DeleteGroup()
        {
            Console.WriteLine("Группы: ");
            foreach (var g in groups)
            {
                Console.WriteLine(g.Id + " - " + g.Pool.Name + " - " + g.Trainer.Name);
            }
            Console.WriteLine("Выберите Бассейн группы: ");
            var id = int.Parse(Console.ReadLine());
            groups.Remove(groups.Single(p => p.Id == id));
            Console.WriteLine("Группа удалена");
            Console.Clear();
        }

        public static void LoadFiles()
        {
            //добавляем абонементы
            string[] subscriptionsfile = File.ReadAllText("subscriptions.txt").Split('\n');
            foreach (var subscription in subscriptionsfile)
            {
                int id = int.Parse(subscription.Split(';')[0]);
                string type = subscription.Split(';')[1];
                int numofvisits = int.Parse(subscription.Split(';')[2]);
                int price = int.Parse(subscription.Split(';')[3]);
                subscriptions.Add(new Subscription(id, type, numofvisits, price));
            }
            //добавляем бассейны
            string[] poolsfile = File.ReadAllText("pools.txt").Split('\n');
            foreach (var pool in poolsfile)
            {
                int id = int.Parse(pool.Split(';')[0]);
                string name = pool.Split(';')[1];
                string address = pool.Split(';')[2];
                string type = pool.Split(';')[3];
                pools.Add(new Pool(id, name, address, type));
            }
            //добавляем группы
            string[] groupsfile = File.ReadAllText("groups.txt").Split('\n');
            foreach (var group in groupsfile)
            {
                int id = int.Parse(group.Split(';')[0]);
                string category = group.Split(';')[1];
                Pool pool = pools[int.Parse(group.Split(';')[2])];
                Subscription subscription = subscriptions[int.Parse(group.Split(';')[3])];
                groups.Add(new Group(id, category, pool, subscription));


                /*groups.Add(new Group(6, "Начинающие", pools[0], subscriptions[1]));
                groups.Add(new Group(7, "Подростки", pools[1], subscriptions[2]));
                groups.Add(new Group(8, "Взрослые", pools[2], subscriptions[0]));*/
                /* trainers[2].AddGroup(groups[1]);
                trainers[1].AddGroup(groups[2]);
                trainers[0].AddGroup(groups[3]);

                trainers[2].AddGroup(groups[3]);
                trainers[1].AddGroup(groups[4]);
                trainers[0].AddGroup(groups[5]);

                trainers[2].AddGroup(groups[6]);
                trainers[1].AddGroup(groups[7]);
                trainers[0].AddGroup(groups[8]);*/
            }
            //добавляем участников групп
            string[] participantsfile = File.ReadAllText("participants.txt").Split('\n');
            //participantsfile.RemoveAt(participantsfile.Count-1);
            foreach (var participant in participantsfile)
            {
                //Console.WriteLine(participantsfile.Count);
                int id = int.Parse(participant.Split(';')[0]);
                string name = participant.Split(';')[1];
                Pool pool = pools[int.Parse(participant.Split(';')[2])];
                Subscription subscription = subscriptions[int.Parse(participant.Split(';')[3])];
                AddParticipant(new Participant(id, name, pool, subscription));
            }
            //добавляем тренеров
            string[] trainersfile = File.ReadAllText("trainers.txt").Split('\n');
            foreach (var trainer in trainersfile)
            {
                int id = int.Parse(trainer.Split(';')[0]);
                string name = trainer.Split(';')[1];
                int dayswoork = int.Parse(trainer.Split(';')[2]);
                trainers.Add(new Trainer(id, name, dayswoork));
            }

            pools[0].AddTrainer(trainers[0]);
            pools[0].AddTrainer(trainers[2]);
            pools[1].AddTrainer(trainers[1]);
            pools[1].AddTrainer(trainers[0]);
            pools[2].AddTrainer(trainers[2]);
            pools[2].AddTrainer(trainers[1]);

        }

        static void Main()
        {
            Console.WriteLine("Загрузка файлов");
            LoadFiles();
            Console.WriteLine("Загрузка завершена");

            while (true)
            {
                //Console.Clear();
                Console.WriteLine("   Выберите № задачи\n"
                + "1 - Сгруппировать тренеров по бассейнам.\n"
                + "2 - Итоговая прибыль каждого тренера в каждом бассейн.\n"
                + "3 - Тренеры, работающие с начинающим.\n"
                + "4 - Список посетителей, занимающихся с заданным тренером.\n"
                + "5 - Количество групп в каждом бассейне по дням недели.\n"
                + "6 - Бассейн с максимальной выручкой.\n"
                + "7 - Добавление группы в заданный бассейн.\n"
                + "8 - Удаление группы в заданном бассейне.\n\n"
                + "0 - Выход из программы");
                var c = Console.ReadKey(true).KeyChar;
                Console.Clear();
                switch (c)
                {
                    case '1':
                        {
                            Console.WriteLine("Групировка тренеров по бассейнам:");
                            pools.ForEach(p => { Console.WriteLine(p.Name + "\nТренеры:"); p.Trainers.ForEach(t => Console.WriteLine($"\t{t.Name}")); });
                            Console.WriteLine();
                            Console.ReadKey();
                            continue;
                        }
                    case '2':
                        {
                            Console.WriteLine("Итоговая прибыль каждого тренера в каждом бассейне:");
                            trainers.ForEach(t => { Console.WriteLine(t.Name); t.Pools.ForEach(p => Console.WriteLine($"\t{p.Name}: {t.GetTotalRevenue(p)}")); });
                            Console.ReadKey();
                            continue;
                        }
                    case '3':
                        {
                            Console.WriteLine("Тренеры, работающие с начинающими:");
                            trainers.Where(n => n.Groups.Where(g => g.Category == "Начинающие").Count() > 0).ToList().ForEach(tr => Console.WriteLine(tr.Name));
                            Console.ReadKey();
                            continue;
                        }
                    case '4':
                        {
                            Console.WriteLine("Список посетителей, занимающихся с заданным тренером: ");
                            trainers.ForEach(tr => Console.WriteLine(tr.Id + "-" + tr.Name));
                            Console.WriteLine("Выберите номер тренера: ");
                            int ti = int.Parse(Console.ReadLine());
                            Console.WriteLine($"Посетители, занимающиеся у {trainers.Single(n => n.Id == ti).Name}");
                            trainers.Single(n => n.Id == ti).Groups.ForEach(g => g.Participants.ForEach(par => Console.WriteLine(par.Name)));
                            Console.ReadKey();
                            continue;
                        }
                    case '5':
                        {
                            Console.WriteLine("Количество групп в каждом бассейне по дням недели:");
                            
                            foreach (var pool in pools)
                            {
                                Console.WriteLine(pool.Name+':');
                                foreach (var day in Enum.GetNames(typeof(DayWeek)))
                                {
                                    try
                                    {
                                        Console.WriteLine($"\t{day}: {pool.Trainers.Single(t => t.Timetable.Contains(day)).Groups.Count}");
                                    }
                                    catch (Exception)
                                    {
                                        Console.WriteLine($"\t{day}:0");
                                    }
                                }
                            }
                            Console.ReadKey();
                            continue;
                        }
                    case '6':
                        {
                            Console.WriteLine("Бассейн с максимальной выручкой: ");
                            var m = pools.Select(p => new { p.Name, Sum = p.Groups.Sum(g => g.Subscription.Price * g.Participants.Count) }).OrderBy(n => n.Sum).First();
                            Console.WriteLine($"{m.Name} - {m.Sum}");
                            Console.ReadKey();
                            continue;
                        }
                    case '7':
                        {
                            Console.WriteLine("Добавление группы в заданный бассейн");
                            AddGroup();
                            foreach (var item in groups)
                            {
                                Console.WriteLine($"{item.Id} - {item.Trainer.Name} - {item.Category} - {item.Pool.Name} - {item.Subscription.Type}");
                            }
                            Console.ReadKey();
                            continue;
                        }
                    case '8':
                        Console.WriteLine("Удаление группы в заданном бассейне");
                        DeleteGroup();
                        Console.ReadKey();
                        continue;
                    case '0':
                        Console.WriteLine("Вы уверены, что хотите завершить работу программы?\nНажмите любую клавишу для выхода, ESC для отмены...");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape) { Console.Clear(); continue; }

                        Console.WriteLine("\nЗавершение работы программы...");
                        return;
                    default:
                        Console.WriteLine("Выбран некорректный пункт меню. Выберите значение от 1 до 8 или нуль!\nНажмите любую клавишу, чтобы вернуться в меню...");
                        Console.ReadKey();
                        continue;
                }
            }
        }
    }
}
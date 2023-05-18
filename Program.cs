using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// 111;Молоток;15C;Технология;200;Годен;11.02.2023 //
public class Device
{
    public int Key { get; set; }
    public string Name { get; set; }
    public string InventoryNum { get; set; }
    public string Profile { get; set; }
    public double Cost { get; set; }
    public string Status { get; set; }
    public DateTime DateOfInsp { get; set; }
    public Device(int key, string name, string inventorynum, string profile, double cost, string status, DateTime dateofinsp)
    { Key = key; Name = name; InventoryNum = inventorynum; Profile = profile; Cost = cost; Status = status; DateOfInsp = dateofinsp; }
}
// 111;Школа;Технология;14.05.2023 //
public class Subdivision
{
    public int Key { get; set; }
    public string Name { get; set; }
    public string Profile { get; set; }
    public DateTime CurrentDate { get; set; }
    public List<string> Devices { get; set; }
    public Subdivision(int key, string name, string profile, DateTime currentdate, List<string> devices)
    { Key = key; Name = name; Profile = profile; CurrentDate = currentdate; Devices = devices; }
}
// 111;Школа;Молоток;15С;16.05.2023;Списан;12.04.2023;50 //
public class Muster
{
    public int Key { get; set; }
    public string Organization { get; set; }
    public string Device { get; set; }
    public string InventoryNum { get; set; }
    public DateTime NextDate { get; set; }
    public string Result { get; set; }
    public DateTime DateOfInsp { get; set; }
    public double CostOfInsp { get; set; }
    public Muster(int key, string organization, string device, string inventorynum, DateTime nextdate, string result, DateTime dateofinsp, double costofinsp)
    { Key = key; Organization = organization; Device = device; InventoryNum = inventorynum; NextDate = nextdate; Result = result; DateOfInsp = dateofinsp; CostOfInsp = costofinsp; }
}
internal class Program
{
    static void Main(string[] args)
    {

        int m;

        StreamReader FileIn = new StreamReader("приборы.txt", Encoding.UTF8);

        string str;

        while ((str = FileIn.ReadLine()) != null) // Пока не конец файла
        {
            m = 7;
            string[] ms = new string[m]; // m - количество полей в классе

            ms = str.Split(';'); // Разбивание на массив строк

            // Добавление строки через конструктор с параметрами

            devices.Add(new Device(int.Parse(ms[0]), ms[1], ms[2], ms[3], double.Parse(ms[4]), ms[5], DateTime.Parse(ms[6])));

        }
        FileIn.Close();

        FileIn = new StreamReader("подразделения.txt", Encoding.UTF8);

        while ((str = FileIn.ReadLine()) != null) // Пока не конец файла
        {
            m = 4;
            string[] ms = new string[m]; // m - количество полей в классе

            ms = str.Split(';'); // Разбивание на массив строк

            // Добавление строки через конструктор с параметрами

            subdivisions.Add(new Subdivision(int.Parse(ms[0]), ms[1], ms[2], DateTime.Parse(ms[3]), ms[4].Split(',').ToList()));
        }// Преобразование учитывает описание класса
        FileIn.Close();

        FileIn = new StreamReader("поверки.txt", Encoding.UTF8);

        while ((str = FileIn.ReadLine()) != null) // Пока не конец файла
        {
            m = 8;
            string[] ms = new string[m]; // m - количество полей в классе

            ms = str.Split(';'); // Разбивание на массив строк

            // Добавление строки через конструктор с параметрами

            musters.Add(new Muster(int.Parse(ms[0]), ms[1], ms[2], ms[3], DateTime.Parse(ms[4]), ms[5], DateTime.Parse(ms[6]), double.Parse(ms[7])));

        }// Преобразование учитывает описание класса
        FileIn.Close();

        //////////////////////////////////////////////////

        char c;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("\tВыберите № задачи\n");

            Console.WriteLine("1 - Поменять статус прибора.\n" +
                                "2 - Осуществить поверку.\n" +
                                "3 - Добавить новый прибор в базу данных.\n" +
                                "4 - Удалить прибор из базы данных.\n" +
                                "5 - Добавление нового подразделения в базу данных.\n" +
                                "6 - Удаление подразделения из базы данных.\n" +
                                "7 - Просмотр списка приборов, находящихся на консервации.\n" +
                                "8 - Просмотр списка списанных приборов.\n" +
                                "\n0 - Выход из программы");

            c = Console.ReadKey(true).KeyChar;

            switch (c)
            {
                case '1':
                    Console.Clear();
                    Console.WriteLine("Введите ключ прибора, статус которого нужно поменять:");
                    EditStatus(int.Parse(Console.ReadLine()));
                    Console.ReadKey();
                    continue;
                case '2':
                    Console.Clear();
                    Muster();
                    Console.WriteLine("Поверка осуществлена. Нажмите любую клавишу, чтобы продолжить...");
                    Console.ReadKey();
                    continue;
                case '3':
                    Console.Clear();
                    AddDevice();
                    Console.WriteLine("Прибор добавлен. Нажмите любую клавишу, чтобы продолжить...");
                    Console.ReadKey();
                    continue;
                case '4':
                    Console.Clear();
                    Console.WriteLine("Введите ключ прибора, который хотите удалить: ");
                    DeleteDevice(int.Parse(Console.ReadLine()));
                    Console.WriteLine("Прибор удалён. Нажмите любую клавишу, чтобы продолжить...");
                    Console.ReadKey();
                    continue;
                case '5':
                    Console.Clear();
                    AddSub();
                    Console.WriteLine("Подразделение добавлено. Нажмите любую клавишу, чтобы продолжить...");
                    Console.ReadKey();
                    continue;
                case '6':
                    Console.Clear();
                    Console.WriteLine("Введите ключ подразделения, которое хотите удалить: ");
                    DeleteSub(int.Parse(Console.ReadLine()));
                    Console.WriteLine("Подразделение удалено. Нажмите любую клавишу, чтобы продолжить...");
                    Console.ReadKey();
                    continue;
                case '7':
                    Console.Clear();
                    var devicesOnCons = devices.Where(device => device.Status == "На консервации");
                    var organizationOfThisDevice = subdivisions.Where(subdivision => subdivision.Devices.Any(device => devices.Any(d => d.Name == device)));
                    foreach (var device in devicesOnCons)
                    {
                        Console.WriteLine($"{device.Name}\n");
                        //Console.WriteLine($"{organizationOfThisDevice}\n");
                    }
                    Console.WriteLine("Список выведен. Нажмите любую клавишу, чтобы продолжить...");
                    Console.ReadKey();
                    continue;
                case '8':
                    Console.Clear();

                    Console.ReadKey();
                    continue;
                case '0':
                    Console.WriteLine("Вы уверены, что хотите завершить работу программы?\nНажмите любую клавишу для выхода, ESC для отмены...");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape) { Console.Clear(); continue; }

                    Console.WriteLine("\nЗавершение работы программы...");
                    return;
                default:
                    Console.Clear();
                    Console.WriteLine("Выбран некорректный пункт меню. Выберите значение от 1 до 8 или нуль!\nНажмите любую клавишу, чтобы вернуться в меню...");
                    Console.ReadKey();
                    continue;
            }
        }
    }

    static List<Device> devices = new List<Device>();
    static List<Subdivision> subdivisions = new List<Subdivision>();
    static List<Muster> musters = new List<Muster>();

    static void Muster()
    {
        string str;
        int m = 8;
        string[] ms = new string[m];
        Console.WriteLine("Введите данные поверки:");
        Console.WriteLine("Ключ:");
        str = Console.ReadLine() + ";";
        Console.WriteLine("Организация:");
        str += Console.ReadLine() + ";";
        Console.WriteLine("Прибор:");
        str += Console.ReadLine() + ";";
        Console.WriteLine("Инвентарный №:");
        str += Console.ReadLine() + ";";
        Console.WriteLine("Дата следующей поверки (DD.MM.YY HH: MM: SS):");
        str += Console.ReadLine() + ";";
        Console.WriteLine("Результат (Годен / Не годен):");
        str += Console.ReadLine() + ";";
        Console.WriteLine("Дата поверки (DD.MM.YY HH: MM: SS):");
        str += Console.ReadLine() + ";";
        Console.WriteLine("Стоимость:");
        str += Console.ReadLine() + ";";

        ms = str.Split(';');

        musters.Add(new Muster(int.Parse(ms[0]), ms[1], ms[2], ms[3], DateTime.Parse(ms[4]), ms[5], DateTime.Parse(ms[6]), double.Parse(ms[7])));

        StreamWriter Fileout = new StreamWriter("поверки.txt", false);
        foreach (var muster in musters)
        {
            str = ($"{muster.Key};{muster.Organization};{muster.Device};{muster.InventoryNum};{muster.NextDate};{muster.Result};{muster.DateOfInsp};{muster.CostOfInsp}");

            Fileout.WriteLine(str);
        }
        Fileout.Close();

        var deviceMuster = devices.FirstOrDefault(device => device.Name == ms[2]);
        deviceMuster.Status = ms[5];
        deviceMuster.DateOfInsp = DateTime.Parse(ms[6]);
        Fileout = new StreamWriter("приборы.txt", false);
        foreach (var device in devices)
        {
            str = ($"{device.Key};{device.Name};{device.InventoryNum};{device.Profile};{device.Cost};{device.Status};{device.DateOfInsp}");

            Fileout.WriteLine(str);
        }
        Fileout.Close();

    }
    static void EditStatus(int key)
    {
        StreamWriter fileOut = new StreamWriter("приборы.txt", false);
        string str;
        bool deviceFound = false;

        var deviceToUpdate = devices.FirstOrDefault(device => device.Key == key);
        if (deviceToUpdate != null)
        {
            Console.WriteLine("Прибор " + deviceToUpdate.Name + ". Текущий статус: " + deviceToUpdate.Status + "\nВыберите, какой новый статус установить и нажмите Enter:");
            Console.WriteLine("1. Годен\n2. Не годен\n3. Списан\n4. На консервации\n5. На ремонте");

            while (true)
            {
                switch (Console.ReadKey().KeyChar)
                {
                    case '1':
                        deviceToUpdate.Status = "Годен";
                        break;
                    case '2':
                        deviceToUpdate.Status = "Не годен";
                        break;
                    case '3':
                        deviceToUpdate.Status = "Списан";
                        break;
                    case '4':
                        deviceToUpdate.Status = "На консервации";
                        break;
                    case '5':
                        deviceToUpdate.Status = "На ремонте";
                        break;
                    default:
                        Console.WriteLine("Выберите пункт от 1 до 5!!!");
                        continue;
                }
                break;
            }
            deviceFound = true;
        }
        foreach (var device in devices)
        {
            str = ($"{device.Key};{device.Name};{device.InventoryNum};{device.Profile};{device.Cost};{device.Status};{device.DateOfInsp}");
            fileOut.WriteLine(str);
        }

        fileOut.Close();

        if (!deviceFound) Console.WriteLine("Файл пуст или прибора с таким ключом нет.");
    }
    static void AddDevice()
    {
        string str;
        int m = 7;
        string[] ms = new string[m];
        Console.WriteLine("Введите данные прибора:");
        Console.WriteLine("Ключ:");
        str = Console.ReadLine() + ";";
        Console.WriteLine("Название:");
        str += Console.ReadLine() + ";";
        Console.WriteLine("Инвентарный №:");
        str += Console.ReadLine() + ";";
        Console.WriteLine("Профиль:");
        str += Console.ReadLine() + ";";
        Console.WriteLine("Стоимость:");
        str += Console.ReadLine() + ";";
        Console.WriteLine("Состояние:");
        str += Console.ReadLine() + ";";
        Console.WriteLine("Дата поверки (DD.MM.YY HH: MM: SS):");
        str += Console.ReadLine() + ";";

        ms = str.Split(';');

        devices.Add(new Device(int.Parse(ms[0]), ms[1], ms[2], ms[3], double.Parse(ms[4]), ms[5], DateTime.Parse(ms[6])));

        StreamWriter Fileout = new StreamWriter("приборы.txt", false);
        foreach (var device in devices)
        {
            str = ($"{device.Key};{device.Name};{device.InventoryNum};{device.Profile};{device.Cost};{device.Status};{device.DateOfInsp}");

            Fileout.WriteLine(str);
        }
        Fileout.Close();
    }
    // 111;Школа;Технология;14.05.2023;Молоток,Дрель //
    static void AddSub()
    {
        string str;
        int m = 4;
        string[] ms = new string[m];
        Console.WriteLine("Введите данные прибора:");
        Console.WriteLine("Ключ:");
        str = Console.ReadLine() + ";";
        Console.WriteLine("Название:");
        str += Console.ReadLine() + ";";
        Console.WriteLine("Профиль:");
        str += Console.ReadLine() + ";";
        Console.WriteLine("Текущая дата (DD.MM.YY HH: MM: SS):");
        str += Console.ReadLine() + ";";
        Console.WriteLine("Приборы через запятую (Линейки,карандаши):");
        str += Console.ReadLine() + ";";

        ms = str.Split(';');

        subdivisions.Add(new Subdivision(int.Parse(ms[0]), ms[1], ms[2], DateTime.Parse(ms[3]), ms[4].Split(',').ToList()));

        StreamWriter Fileout = new StreamWriter("подразделения.txt", false);
        foreach (var subdivision in subdivisions)
        {
            str = ($"{subdivision.Key};{subdivision.Name};{subdivision.Profile};{subdivision.CurrentDate};");
            foreach (var device in subdivision.Devices)
            {
                if (device != subdivision.Devices[subdivision.Devices.Count() - 1])
                    str += device + ",";
                else
                    str += device;
            }
            Fileout.WriteLine(str);
        }
        Fileout.Close();
    }
    static bool DeleteDevice(int DelKey)
    {
        string str;
        int removeDeviceNum = 0;
        bool f = false;
        foreach (var device in devices)
        {
            if (device.Key == DelKey)
            {
                f = true;
                break;
            }
            removeDeviceNum++;
        }
        if (f)
        {
            devices.Remove(devices[removeDeviceNum]);
            StreamWriter Fileout = new StreamWriter("приборы.txt", false);
            foreach (var device in devices)
            {
                str = ($"{device.Key};{device.Name};{device.InventoryNum};{device.Profile};{device.Cost};{device.Status};{device.DateOfInsp}");

                Fileout.WriteLine(str);
            }
            Fileout.Close();
            return true;
        }
        return false;
    }
    static bool DeleteSub(int DelKey)
    {
        string str;
        int removeSubNum = 0;
        bool f = false;
        foreach (var subdivision in subdivisions)
        {
            if (subdivision.Key == DelKey)
            {
                f = true;
                break;
            }
            removeSubNum++;
        }
        if (f)
        {
            subdivisions.Remove(subdivisions[removeSubNum]);
            StreamWriter Fileout = new StreamWriter("подразделения.txt", false);
            foreach (var subdivision in subdivisions)
            {
                str = ($"{subdivision.Key};{subdivision.Name};{subdivision.Profile};{subdivision.CurrentDate}");

                Fileout.WriteLine(str);
            }
            Fileout.Close();
            return true;
        }
        return false;
    }
}
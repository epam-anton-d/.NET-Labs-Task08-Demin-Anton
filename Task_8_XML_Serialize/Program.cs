using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml.Serialization;

namespace Task_8_XML_Serialize
{
    public delegate void Ask();

    class Program
    {
        static void Main(string[] args)
        {
            const string bankPattern = @"Банк\:\s+[А-Яа-я0-9]+";
            const string clientPattern = @"Клиент\:(\s+[А-Я][а-я]+){1,3}\,\s+(?([3])3[01]|[0-2]?[0-9])\.(?([1])1[0-2]|0[0-9])\.[1-2][0-9][0-9][0-9]";
            char[] separator = new char [] { ' ', ',' };
            string bankTemp = "";
            string clientTemp;
            string line;
            string[] readWord;
            List<Bank> bankList = new List<Bank>();
            List<Client> clientList = new List<Client>();         
            

            // Ввод данных в фильтр.
            Console.Write("Введите фамилию или оставьте поле пустым для вывода всех: ");
            string familiaFind = Console.ReadLine();
            Console.Write("Введите имя или оставьте поле пустым для вывода всех: ");
            string nameFind = Console.ReadLine();
            Console.Write("Введите отчество или оставьте поле пустым для вывода всех: ");
            string otchestvoFind = Console.ReadLine();
            Console.Write("Введите название банка или оставьте поле пустым для вывода всех: ");
            string bankNameFind = Console.ReadLine();
            
            try
            {
                // Чтение файлов из файла input.txt.
                System.IO.StreamReader file =
                    new System.IO.StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt"), Encoding.GetEncoding(1251));
                while ((line = file.ReadLine()) != null)
                {
                    // Проверка на соответствие шаблону банка.
                    if (Regex.IsMatch(line, bankPattern))
                    {
                        readWord = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        bankTemp = readWord[1];
                    }
                    else if (bankTemp == "")
                    {
                        continue;
                    }
                    // Проверка на соответствие шаблону клиента.
                    else if (Regex.IsMatch(line, clientPattern))
                    {
                        readWord = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        clientTemp = "";

                        for (int i = 1; i < readWord.Length - 1; i++)
                        {
                            clientTemp += readWord[i] + " ";
                        }
                        clientList.Add(new Client() { Fio = clientTemp, Birthday = Convert.ToDateTime(readWord[readWord.Length - 1]), BankName = bankTemp });
                    }
                    
                }


                file.Close();

            }
            catch
            {
                Console.WriteLine("Ошибка чтения из файла.");
            }


            // Применение фильтров:
            // 1) По фамилии.
            var queryFamilia =
                        from client in clientList
                        where client.Fio.Contains(familiaFind)
                        select new
                        {
                            Fio = client.Fio,
                            Birthday = client.Birthday,
                            BankName = client.BankName
                        };

            // 2) По имени.
            var queryName =
                        from client in queryFamilia
                        where client.Fio.Contains(nameFind)
                        select new
                        {
                            Fio = client.Fio,
                            Birthday = client.Birthday,
                            BankName = client.BankName
                        };

            // 3) По отчеству.
            var queryOtchestvo =
                        from client in queryName
                        where client.Fio.Contains(otchestvoFind)
                        select new
                        {
                            Fio = client.Fio,
                            Birthday = client.Birthday,
                            BankName = client.BankName
                        };

            // 4) По банку.
            var queryBankName =
                        from client in queryOtchestvo
                        where client.BankName.Contains(bankNameFind)
                        select new
                        {
                            Fio = client.Fio,
                            Birthday = client.Birthday,
                            BankName = client.BankName
                        };

            

            // Вывод результата.
            if (familiaFind == "" && nameFind == "" && otchestvoFind == "" && bankNameFind == "")
            {
                Console.WriteLine("Вы не ввели ни одного фильтра.");
            }
            else
            {
                Console.WriteLine();
                foreach (var item in queryBankName)
                {
                    Console.WriteLine("{0} - {1:d} - {2}", item.Fio, item.Birthday, item.BankName);
                }

                List<Client> query = new List<Client>();
                foreach (var item in queryBankName)
                {
                    query.Add(new Client() { Fio = item.Fio, Birthday = item.Birthday, BankName = item.BankName });
                }

                Console.WriteLine();
                // Сериализация. 
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Client>));

                    using (var stream = new FileStream("Result.xml", FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        serializer.Serialize(stream, query);
                    }

                    Console.WriteLine("Сериализация прошла успешно!");
                }
                catch
                {
                    Console.WriteLine("Ошибка при сериализации.");
                }
            }


            Console.ReadKey();
        }
    }
}

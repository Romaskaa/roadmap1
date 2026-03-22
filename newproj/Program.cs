using project.Collections;
using project.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            var registrationController = new RegistrationController();
            var roadmapController = new RoadmapController();

            while (true)
            {
                Console.WriteLine("МЕНЮ:");
                Console.WriteLine("1) Регистрация пользователя");
                Console.WriteLine("2) Получение дорожной карты");
                Console.WriteLine("0) Выход");
                Console.Write("Выберите действие: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        RegisterFlow(registrationController);
                        break;

                    case "2":
                        RoadmapFlow(roadmapController);
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Неверный выбор.");
                        break;
                }

                Console.WriteLine();
            }
        }

        static void RegisterFlow(RegistrationController controller)
        {
            Console.Write("Введите ФИО: ");
            string name = Console.ReadLine();

            Console.Write("Введите дату въезда (дд.мм.гггг): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime entryDate))
            {
                Console.WriteLine("Ошибка: некорректная дата въезда.");
                return;
            }

            if (entryDate > DateTime.Now.Date)
            {
                Console.WriteLine("Ошибка: дата въезда не может быть в будущем.");
                return;
            }

            Console.Write("Есть ли заявление на патент или разрешение? (да/нет): ");
            string answer = Console.ReadLine().Trim().ToLower();

            DateTime? applicationDate = null;

            if (answer == "да")
            {
                Console.Write("Введите дату подачи заявления (дд.мм.гггг): ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime appDate))
                {
                    Console.WriteLine("Ошибка: некорректная дата подачи заявления.");
                    return;
                }

                if (appDate > DateTime.Now.Date)
                {
                    Console.WriteLine("Ошибка: дата подачи заявления не может быть в будущем.");
                    return;
                }

                applicationDate = appDate;
            }

            string login = controller.EnterCitizen(name, entryDate, applicationDate);
            Console.WriteLine($"\nВаш логин: {login}");

            var purposes = controller.GetListPurposes();
            Console.WriteLine("\nВыберите цель пребывания:");
            for (int i = 0; i < purposes.Count; i++)
                Console.WriteLine($"{i + 1}. {purposes[i]}");

            if (!int.TryParse(Console.ReadLine(), out int purposeIndex) || purposeIndex < 1 || purposeIndex > purposes.Count)
            {
                Console.WriteLine("Ошибка: неверный выбор цели пребывания.");
                return;
            }

            string selectedPurpose = purposes[purposeIndex - 1];

            var citizenships = controller.GetListCitizenships();
            Console.WriteLine("\nВыберите гражданство:");
            for (int i = 0; i < citizenships.Count; i++)
                Console.WriteLine($"{i + 1}. {citizenships[i]}");

            if (!int.TryParse(Console.ReadLine(), out int citizenshipIndex) || citizenshipIndex < 1 || citizenshipIndex > citizenships.Count)
            {
                Console.WriteLine("Ошибка: неверный выбор гражданства.");
                return;
            }

            string selectedCitizenship = citizenships[citizenshipIndex - 1];

            if (selectedCitizenship == "РФ")
            {
                Console.WriteLine("\nДорожная карта предусмотрена только для иностранных граждан.");
                Console.WriteLine("Регистрация прекращена.\n");
                return;
            }

            controller.ChoosePurposeAndCitizenship(login, selectedPurpose, selectedCitizenship);

            Console.Write("Является ли пользователь участником Государственной программы переселения соотечественников или членом его семьи? (да/нет): ");
            string isResettlementParticipant = Console.ReadLine().Trim().ToLower() == "да" ? "Да" : "Нет";
            controller.SetProfileProperty(login, Profile.ResettlementProgramPropertyName, isResettlementParticipant);

            Console.WriteLine("\nРегистрация успешно завершена.");
        }

        static void RoadmapFlow(RoadmapController controller)
        {
            Console.Write("Введите логин: ");
            string login = Console.ReadLine();

            string message = controller.GetMessage(login);

            Console.WriteLine("\nВаша дорожная карта:");
            Console.WriteLine(message);
        }
    }
}

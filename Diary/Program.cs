using System;

namespace Diary
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            XMLWorker diary = new XMLWorker();
            //Console.WriteLine(diary.CheckUserExist("korsa3"));

            var data = diary.GetUserData("korsa");
            foreach (var item in data)
            {
                Console.WriteLine(item.Key);
                foreach (var records in item)
                {
                    Console.WriteLine("\t" + records.Value);
                }
            }

            //diary.RegisterUser("kor23dsf54gsa123", "ol123olo");
            Console.WriteLine(diary.SignUp("korsa", "123qwe"));
            diary.AddNote(DateTime.Now, "korsa", "olol");
        }
    }
}

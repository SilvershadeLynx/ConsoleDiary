using System;

namespace Diary
{
    class UserInterface
    {
        private XMLWorker XMLWorker;
        private string login;

        public UserInterface()
        {
            XMLWorker = XMLWorker.GetInstance();
        }

        public void StartDiary()
        {
            MainMenu();
        }

        private void MainMenu()
        {
            string[] menuMain = new string[] { "Register new user", "Sign up", "Exit" };
            string[] menuYesNo = new string[] { "Yes", "No" };
            string spacer = new string(' ', Console.WindowWidth);
            int action;
            bool exit = default;

            for (; ; )
            {
                Console.CursorVisible = false;
                Console.SetCursorPosition(0, 0);
                Console.Write(spacer);
                Console.Write("\rWelcome to Console Diary!");

                action = SelectMenu(menuMain);

                if (action == menuMain.Length - 1)
                {
                    Console.Write("Are you sure?");
                    action = SelectMenu(menuYesNo);

                    switch (action)
                    {
                        case 0:
                            {
                                exit = true;
                                break;
                            }
                        case 1:
                            {
                                continue;
                            }
                        default:
                            {
                                Console.WriteLine("Unexpected error. Exiting...");
                                break;
                            }
                    }
                }
                else if (action == menuMain.Length - 2)
                {
                    bool signed;
                    do
                    {
                        signed = TrySignUp();

                        if (!signed)
                        {
                            Console.Write("Try again?");
                            action = SelectMenu(menuYesNo);

                            switch (action)
                            {
                                case 0:
                                    {
                                        continue;
                                    }
                                case 1:
                                    {
                                        break;
                                    }
                                default:
                                    {
                                        Console.WriteLine("Unexpected error. Exiting...");
                                        break;
                                    }
                            }
                            break;
                        }
                    } while (!signed);

                    if (signed)
                    {
                        Console.CursorVisible = true;
                        string today = DateTime.Now.ToShortDateString(),
                            note = default,
                            message = $"Today is {today}, make your notes." +
                            $"\nType \"Data\" to show your note " +
                            $"\nor type \"Quit\" to exit";

                        Console.WriteLine(message);

                        do
                        {
                            note = Console.ReadLine();
                            if (note.ToLower() != "quit")
                            {
                                if (note.ToLower() == "data")
                                {
                                    ShowUserData();
                                    Console.WriteLine(message);
                                }
                                else
                                {
                                    XMLWorker.AddNote(today, login, note);
                                }
                            }
                        } while (note.ToLower() != "quit");

                    }
                    Console.Clear();
                }
                else
                {
                    while (!TryRegiter())
                    {
                        Console.Write("Try again?");
                        action = SelectMenu(menuYesNo);

                        switch (action)
                        {
                            case 0:
                                {
                                    continue;
                                }
                            case 1:
                                {
                                    break;
                                }
                            default:
                                {
                                    Console.WriteLine("Unexpected error. Exiting...");
                                    break;
                                }
                        }
                        break;
                    }
                }

                if (exit)
                {
                    break;
                }
                else
                {
                    continue;
                }
            }
        }

        private int SelectMenu(params string[] args)
        {
            int currentSelection;
            int spacerLength = 0;
            string pointer = "  ";
            ConsoleKey keyPressed;

            for (int i = 0; i < args.Length; i++)
            {
                if (spacerLength < args[i].Length)
                {
                    spacerLength = args[i].Length + pointer.Length;
                }
            }
            string spacer = new string(' ', spacerLength);
            int currentCursorPos = Console.CursorTop;
            currentSelection = currentCursorPos;

            do
            {
                for (int i = currentCursorPos; i < currentCursorPos + args.Length; i++)
                {
                    Console.SetCursorPosition(0, i + 1);

                    if (i == currentSelection)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                        pointer = "\u25BA ";
                    }

                    Console.Write(spacer);
                    Console.Write($"\r{pointer}{ args[i - currentCursorPos]}");
                    pointer = "  ";
                    Console.ResetColor();
                }

                keyPressed = Console.ReadKey(true).Key;

                switch (keyPressed)
                {
                    case ConsoleKey.UpArrow:
                        {
                            if (currentSelection >= currentCursorPos + 1)
                            {
                                currentSelection--;
                            }
                            else
                            {
                                currentSelection = args.Length - 1 + currentCursorPos;
                            }
                            break;
                        }
                    case ConsoleKey.DownArrow:
                        {
                            if (currentSelection < args.Length - 1 + currentCursorPos)
                            {
                                currentSelection++;
                            }
                            else
                            {
                                currentSelection = currentCursorPos;
                            }
                            break;
                        }
                }

            } while (keyPressed != ConsoleKey.Enter);

            Console.Clear();
            return currentSelection - currentCursorPos;
        }

        private bool TrySignUp()
        {
            bool signed = default;

            Console.WriteLine("Enter your login:");
            string login = Console.ReadLine();
            Console.WriteLine("Enter password:");
            string password = Console.ReadLine();

            try
            {
                signed = XMLWorker.SignUp(login, password);
                if (!signed)
                {
                    Console.WriteLine("Wrong password");
                }
                else
                {
                    Console.Clear();
                    this.login = login;
                    Console.WriteLine($"Welcome back {login}!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return signed;
        }

        private bool TryRegiter()
        {
            bool registered = default;

            Console.WriteLine("Enter your login:");
            string login = Console.ReadLine();
            Console.WriteLine("Enter password:");
            string password = Console.ReadLine();

            try
            {
                XMLWorker.RegisterUser(login, password);
                registered = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return registered;
        }

        private void ShowUserData()
        {
            var data = XMLWorker.GetUserData(login);
            foreach (var records in data)
            {
                Console.WriteLine(new string('-', 25));
                Console.WriteLine(records.Key);
                Console.WriteLine(new string('-', 25));
                foreach (var record in records)
                {
                    Console.WriteLine("\t" + record.Value);
                }
            }
            Console.WriteLine();
        }
    }
}

using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

class Program
{
    public static bool debug = true; //DEBUG

    static bool user_logged_in = false;
    static public int[] pwd_array = new int[100];
    static public string userdata_path = null;
    static public string[] user_data;
    static public bool user_data_available = false;
    static int number2;
    static string assembly_pwd = "";
    static int result;
    static public int free_line = -1;
    static bool user_exists = false;
    static int user_id;
    static ConsoleKeyInfo KeyInfo;


    //MD5
    static string data_path;
    static string target_hash;
    static public int number = 0;
    static string[] list;
    static void Main(string[] args)
    {
        Console.Title = "Mainframe";
        while (true)
        {
            while (!user_logged_in)
            {
                string current_path;
                var rnd = new Random();
                if (debug)
                {
                    current_path = "E:\\C#testing\\mainframe\\";  //DEBUG
                }
                else
                {
                    current_path = Directory.GetCurrentDirectory(); //Release
                }
                try
                {
                    userdata_path = Path.Combine(current_path, "data\\user_data.bin");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Couldn't acces user data");
                    Console.ReadKey();
                }
                try
                {
                    if (userdata_path != null)
                    {
                        user_data = File.ReadAllLines(userdata_path);
                        user_data_available = true;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("error");
                    Console.ReadKey();
                }
                int character;
                int key_action;
                for (key_action = 0; key_action == 0;)
                {
                    Console.Clear();
                    Console.ResetColor();
                    Console.WriteLine("[1] Sign up");
                    Console.WriteLine("[2] Log in");
                    char readkey = Console.ReadKey().KeyChar;
                    int number;
                    if (int.TryParse(readkey.ToString(), out number))
                    {
                        if ((number < 3) && (number > 0))
                        {
                            key_action = number;
                        }
                    }
                }
                switch (key_action)
                {
                    case 1:
                        int sucesscode = 0;
                        while (sucesscode == 0)
                        {
                            Console.Clear();
                            Console.Write("Enter username: ");
                            string real_created_username = Console.ReadLine();
                            string created_username = real_created_username.ToLower();
                            Console.WriteLine();
                            string enterText1 = "Enter password: ";
                            string created_password = CheckPassword(enterText1);
                            Console.WriteLine();
                            string enterText2 = "Confirm password: ";
                            string created_password_confirm = CheckPassword(enterText2);
                            Console.Clear();
                            bool created_username_available = true;
                            for (int i = 0; i <= 999; i++)
                            {
                                if (user_data[i] == created_username)
                                {
                                    created_username_available = false;
                                    Console.WriteLine("Username already chosen");
                                    Console.ReadKey();
                                    break;
                                }
                            }
                            if (created_username_available == true)
                            {
                                if (created_password == created_password_confirm)
                                {

                                    for (int i = 0; i <= 999; i++)
                                    {
                                        if (user_data[i] == "")
                                        {
                                            free_line = i;
                                            i = 1000;
                                        }
                                        if (free_line >= 0)
                                        {
                                            try
                                            {
                                                user_data[free_line] = real_created_username; //Name
                                                string salt = GenerateRandomString(4);
                                                user_data[2000 + free_line] = salt; //Salt
                                                user_data[1000 + free_line] = CreateHash(created_password, userdata_path, salt); //Hash PWD
                                                sucesscode++;
                                                Console.Clear();
                                                Console.ForegroundColor = ConsoleColor.Green;
                                                Console.WriteLine("Success!");
                                                Console.ResetColor();
                                                Console.Write("Welcome, ");
                                                Console.ResetColor();
                                                Console.ForegroundColor = ConsoleColor.Green;
                                                Console.WriteLine(real_created_username);
                                                Console.ResetColor();
                                                Console.WriteLine("Restart program to sign in");
                                                Console.ReadKey();

                                            }
                                            catch (Exception ex)
                                            {
                                                Console.ForegroundColor = ConsoleColor.Red;
                                                Console.WriteLine("Critical error: {0}", ex);
                                                Console.WriteLine("Could not save data");
                                                Console.ResetColor();
                                                Console.ReadKey();
                                            }
                                            if (sucesscode > 0)
                                            {
                                                File.WriteAllLines(userdata_path, user_data);
                                            }
                                            else
                                            {
                                                Console.ForegroundColor = ConsoleColor.Red;
                                                Console.WriteLine("Could not save data due do internal error");
                                                Console.ResetColor();
                                                Console.ReadKey();
                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Passwords don't match");
                                    Console.ResetColor();
                                    Console.ReadKey();
                                }
                            }
                        }

                        break;
                    case 2:
                        Console.Clear();
                        Console.Write("Enter your username: ");
                        string trueinput_username = Console.ReadLine();
                        user_data = File.ReadAllLines(userdata_path);
                        string input_username = trueinput_username.ToLower();
                        for (int i = 0; i < 999; i++)
                        {
                            if (input_username == user_data[i].ToLower())
                            {
                                user_exists = true;
                                user_id = i;
                            }
                        }
                        if (user_exists)
                        {
                            Console.Clear();
                            Console.WriteLine("Welcome back!");
                            Console.WriteLine();
                            Console.ResetColor();
                            string enterText = "Please enter password: ";
                            string input_password = CheckPassword(enterText);
                            Console.WriteLine();
                            string user_salt = user_data[user_id + 2000];
                            string password_probe = CreateHash(input_password, userdata_path, user_salt);
                            string password_saved = user_data[user_id + 1000];
                            if (debug)
                            {
                                Console.WriteLine("User_Salt: " + user_salt);
                                Console.WriteLine("User saved_pwd: " + password_saved);
                                Console.WriteLine("User input_pwd: " + password_probe);
                                if (password_probe == password_saved)
                                {
                                    Console.WriteLine("The passwords match");
                                }
                                Console.ReadKey();
                            }
                            if (password_probe == password_saved)
                            {
                                Console.Clear();
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("Sucess!");
                                Console.ResetColor();
                                Console.Write("Welcome back, ");
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine(user_data[user_id]);
                                Console.ResetColor();
                                Console.WriteLine("Press any key to continue!");
                                user_logged_in = true;
                                Console.ReadKey();
                                break;

                            }
                            else if (password_probe != password_saved)
                            {
                                Console.Clear();
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Password is not correct");
                                Console.ResetColor();
                                Console.ReadKey();
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("Internal error");
                                Console.ReadKey();
                            }
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("User does not exist");
                            Console.ResetColor();
                            Console.ReadKey();
                        }

                        break;
                    default:
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Critical error");
                        Console.ResetColor();
                        Thread.Sleep(5000);
                        break;
                }
            }//WORKS
            Console.Clear();
            while (user_logged_in)
            {
                int key_action;
                for (key_action = -1; key_action == -1;)
                {
                    Console.Clear();
                    Console.ResetColor();
                    Console.WriteLine("[1] IP Logger v2");
                    Console.WriteLine("[2] MD5");
                    Console.WriteLine();
                    Console.WriteLine("[0] Log out");
                    char readkey = Console.ReadKey().KeyChar;
                    int number;
                    if (int.TryParse(readkey.ToString(), out number))
                    {
                        if ((number < 3) && (number > -1))
                        {
                            key_action = number;
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("Invalid number");
                            Console.ReadKey();
                        }
                    }
                }
                Console.Clear();
                switch (key_action)
                {
                    case 0:
                        user_logged_in = false;
                        break;
                    case 1:
                        IpLogger_v2();
                        break;
                    case 2:
                        DoMD5();
                        break;
                    default:
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Internal error, please try again or restart the program");
                        Console.ReadKey();
                        Console.ResetColor();
                        break;
                }
            }
        }  
    }
    static string GenerateRandomString(int length)
    {
        var rnd = new Random();
        string characters = "abcdefgjijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!§$% &/ () = ß ?{ []}\\+*~#";
        string return_string = "";
        for (int i = 0; i < length; i++)
        {
            char randomchar = characters[rnd.Next(0, characters.Length)];
            return_string += randomchar;
        }
        return return_string;
    }
    static string CreateHash(string pwd, string path, string salt)
    {
        string[] userdata = File.ReadAllLines(path);
        Thread.Sleep(1000);
        Console.Clear();
        if (debug)
        {
            Console.WriteLine("Password: " + pwd);
            Console.WriteLine("Salt: " + salt);
            Console.WriteLine("Combined: " + salt + pwd);
        }
        pwd = salt + pwd;
        byte[] bytes = Encoding.UTF8.GetBytes(pwd);
        string hexString = BitConverter.ToString(bytes).Replace("-", string.Empty);
        if (debug)
        {
            Console.WriteLine("Combined HEX: " + hexString);
        }
        if (hexString.Length > 30)
        {
            Thread.Sleep(1000);
            hexString = hexString.Substring(0, 30);
        }
        Thread.Sleep(800);
        string hexValueString = Convert.ToString(hexString);
        int max_length = 30;
        if (hexValueString.Length > 30)
        {
            max_length = 30;
        }
        else
        {
            max_length = hexValueString.Length;
        }
        for (int i = 0; i < max_length; i++)
        {
            Thread.Sleep(30);
            int number = Convert.ToInt32(hexValueString[i]);

            try
            {
                number2 = Convert.ToInt32(hexValueString[i + 1]);
            }
            catch (Exception)
            {

            }
            pwd_array[i] = number * number2;
        }
        if (debug)
        {
            Console.Write("Result: ");
            for (int i = 0; i < pwd_array.Length; i++)
            {
                Console.Write(pwd_array[i]);
            }
            Console.WriteLine();
            Console.ReadKey();
            Thread.Sleep(750);
        }
        string assembly_pwd = "";
        for (int i = 0; i < pwd_array.Length; i++)
        {
            assembly_pwd += Convert.ToString(pwd_array[i]);
        }
        return assembly_pwd;
    }
    static string CheckPassword(string EnterText)
    {
        string EnteredVal = "";
        try
        {
            Console.Write(EnterText);
            EnteredVal = "";
            do
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    EnteredVal += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && EnteredVal.Length > 0)
                    {
                        EnteredVal = EnteredVal.Substring(0, (EnteredVal.Length - 1));
                        Console.Write("\b \b");
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        if (string.IsNullOrWhiteSpace(EnteredVal))
                        {
                            Console.WriteLine("");
                            Console.WriteLine("Empty value not allowed.");
                            CheckPassword(EnterText);
                            break;
                        }
                        else
                        {
                            Console.WriteLine("");
                            break;
                        }
                    }
                }
            } while (true);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return EnteredVal;
    }
    static void IpLogger_v2()
    {
        string path;
        if (debug)
        {
            path = "E:\\C#testing\\Mainframe\\";
        }
        else
        {
            path = Directory.GetCurrentDirectory();
        }
        string filepath = Path.Combine(path, "data\\iplogger");
        string[] tempfiledata;
        if (File.Exists(filepath))
        {
            tempfiledata = File.ReadAllLines(filepath);
        }
        else
        {
            tempfiledata = new string[0];
        }
        string currenttime = DateTime.Now.AddMilliseconds(DateTime.Now.Millisecond).ToString("HH:mm:ss.fff");
        string currentdate = DateTime.Now.ToString("dd.MM.yyyyy");
        currentdate = currentdate.Replace('.', '/');
        string currentdatefull = '[' + currentdate + " " + currenttime + ']';
        Console.WriteLine("{0} Program loaded", currentdatefull);
        File.AppendAllText(filepath, Environment.NewLine);
        File.AppendAllText(filepath, Environment.NewLine + currentdatefull);

        string hostname = Dns.GetHostName();
        IPHostEntry IpAdress = Dns.GetHostEntry(hostname);
        IPHostEntry ipEntry = Dns.GetHostEntry(hostname);
        IPAddress[] addr = ipEntry.AddressList;
        string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        File.AppendAllText(filepath, Environment.NewLine + userName);

        Console.WriteLine("Der Hostname ist: {0}", hostname);
        for (int i = 0; i < addr.Length; i++)
        {
            Console.WriteLine("IP Address {0}: {1} ", i, addr[i].ToString());
            File.AppendAllText(filepath, Environment.NewLine + addr[i].ToString());
        }
        Console.ReadKey();
    }
    static void DoMD5()
    {
        Console.Clear();
        string local_data_path = "data\\wordlist.txt";
        string current_path;
        if (debug)
        {
            current_path = "E:\\C#testing\\Mainframe";
        }
        else
        {
            current_path = Directory.GetCurrentDirectory();
        }
        try
        {
            data_path = Path.Combine(current_path, local_data_path);
            try
            {
                list = File.ReadAllLines(data_path);
                int character;
                int key_action;
                Console.Write("Enter target hash: ");
                target_hash = Console.ReadLine().ToUpper();
                Console.Clear();
                for (key_action = 0; key_action == 0;)
                {
                    Console.Clear();
                    Console.ResetColor();
                    Console.WriteLine("[1] Normal mode");
                    Console.WriteLine("[2] Visual mode");
                    Console.WriteLine("[3] Optimized mode");
                    char readkey = Console.ReadKey().KeyChar;
                    int number;
                    if (int.TryParse(readkey.ToString(), out number))
                    {
                        if ((number < 4) && (number > 0))
                        {
                            key_action = number;
                        }
                    }
                }
                Console.Clear();
                switch (key_action)
                {
                    case 1:
                        long ammount_of_tries = 0;
                        for (int i = 0; i < list.Length; i++)
                        {
                            for (int j = 0; j < list.Length; j++)
                            {
                                ammount_of_tries++;
                                string targetword = list[i] + list[j];
                                string result = CreateMD5(targetword);
                                if (result == target_hash)
                                {
                                    Console.Clear();
                                    Console.WriteLine(targetword + " resulted in " + result);
                                    Console.WriteLine("The targets were: " + list[i] + " , " + list[j]);
                                    string whatYouWant = ammount_of_tries.ToString("#,##0");
                                    Console.WriteLine(whatYouWant + " tries were needed to find the correct combination");
                                    Console.ReadKey();
                                    i = list.Length + 1;
                                    break;
                                }
                                if (ammount_of_tries % 5000000 == 0)
                                {
                                    string whatYouWant = ammount_of_tries.ToString("#,##0");
                                    Console.WriteLine(whatYouWant + " tries    " + result);
                                }
                            }
                        }
                        break;
                    case 2:
                        long two_ammount_of_tries = 0;
                        for (int i = 0; i < list.Length; i++)
                        {
                            for (int j = 0; j < list.Length; j++)
                            {
                                two_ammount_of_tries++;
                                string targetword = list[i] + list[j];
                                string result = CreateMD5(targetword);
                                if (result == target_hash)
                                {
                                    Console.Clear();
                                    Console.WriteLine(targetword + " resulted in " + result);
                                    Console.WriteLine("The targets were: " + list[i] + " , " + list[j]);
                                    string whatYouWant2 = two_ammount_of_tries.ToString("#,##0");
                                    Console.WriteLine(whatYouWant2 + " tries were needed to find the correct combination");
                                    Console.ReadKey();
                                    i = list.Length + 1;
                                    break;
                                }
                                string whatYouWant = two_ammount_of_tries.ToString("#,##0");
                                Console.WriteLine(whatYouWant + " tries    " + result);
                            }
                        }
                        break;
                    case 3:
                        Console.WriteLine("Working...");
                        for (int i = 0; i < list.Length; i++)
                        {
                            for (int j = 0; j < list.Length; j++)
                            {
                                string targetword = list[i] + list[j];
                                string result = CreateMD5(targetword);
                                if (result == target_hash)
                                {
                                    Console.Clear();
                                    Console.WriteLine(targetword + " resulted in " + result);
                                    Console.WriteLine("The targets were: " + list[i] + " , " + list[j]);
                                    Console.WriteLine("Ammount of tries unknown due to optimization");
                                    Console.ReadKey();
                                    i = list.Length + 1;
                                }
                            }
                        }
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Internal error");
                        Console.WriteLine("key_action invalid");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not acces \"wordlist.txt\"");
                Console.WriteLine("System: " + ex.Message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Could not find \"wordlist.txt\"");
            Console.WriteLine("System: " + ex.Message);
        }

    }
    public static string CreateMD5(string input)
    {
        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            return Convert.ToHexString(hashBytes);
        }
    }
}
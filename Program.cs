using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Dating_Application
{  
    class Preference
    {
        public string Username { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; }
        public List<string> AttractedTo { get; set; }
        public int Heigth { get; set; }
        public List<string> SkinColor { get; set; }
        public List<string> HairColor { get; set; }
        public List<string> EyeColor { get; set; }
        public List<string> Interest { get; set; }
    }
    class Profile : Preference
    { 
        public string Name { get; set; }
    }
    class SqlManagement
    {
        private string connectionString = @"Data Source=LAPTOP-S00DPV1P;Initial Catalog=DatingApplication;Integrated Security=true";
        private SqlConnection connection;
        private SqlCommand command;
        private string queryResult;
        public SqlManagement()
        {
            connection = new SqlConnection(connectionString);
            
            connection.Open();
        }

        public string RunQuery(string query)
        {
            queryResult = "";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if(reader.FieldCount > 1 && i < reader.FieldCount - 1)
                            {
                                queryResult += reader.GetValue(i) + ",";
                            }
                            else
                            {
                                queryResult += reader.GetValue(i);
                            }
                            
                        }
                        
                    }
                }
            }
            return queryResult;
        }

        public List<string> RunQueryList(string query)
        {
            List<string> queryResultList = new List<string>();
            string bufferQuery = "";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (reader.FieldCount > 1 && i < reader.FieldCount - 1)
                            {
                                bufferQuery += reader.GetValue(i) + ",";
                            }
                            else
                            {
                                bufferQuery += reader.GetValue(i);
                                queryResultList.Add(bufferQuery);
                                bufferQuery = "";
                            }

                        }

                    }
                }
            }
            return queryResultList;
        }

        public bool ValidUsername(string newUsername)
        {
            bool validUsername = true;
            string foundUsername = "";

            using (SqlCommand command = new SqlCommand($"SELECT Username FROM [Security] WHERE Username = '{newUsername}'", connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        foundUsername = (string)reader.GetValue(0);
                    }
                }
            }

            if (foundUsername != "")
            {
                validUsername = false;
                Console.Clear();
                Console.WriteLine("Username in use, try again");
                Console.ReadKey();
            }

            return validUsername;
        }

        public void RunNonquery(string input)
        {
            command = connection.CreateCommand();
            command.CommandText = input;
            command.ExecuteNonQuery();

        }

        ~SqlManagement()
        {
            connection.Close();
        }
    }
    class Security
    {
        private SqlManagement sqlAccess;
        public Security(SqlManagement sqlA)
        {
            sqlAccess = sqlA;
        }
        public string Login()
        {
            string username = "";
            string password = "";
            username = ExtraUtilities.GetStringAnswer("Username");
            string validation = sqlAccess.RunQuery($"SELECT Username FROM [Security] WHERE Username = '{username}'");
            Console.WriteLine(validation);
            Console.Clear();
            if (validation == "")
            {
                Console.WriteLine("Username doesn't exist you fucking loser, learn how to spell or make an account");
                Console.ReadKey();
                return username = "";
                
            }
            else
            {
                 
                password = ExtraUtilities.GetStringAnswer("Password");
                validation = sqlAccess.RunQuery($"SELECT Password FROM [Security] WHERE Username = '{username}'");

                if (password != validation)
                {
                    Console.Clear();
                    Console.WriteLine("Incorrect password, try again.");
                    Console.ReadKey();
                    return "";

                }
            }
            Console.Clear();
            return username;
        }
        public void CreateNewUser()
        {
            bool validInput = true;
            string username;
            do
            {
                username = ExtraUtilities.GetStringAnswer("Choose Username");
                validInput = sqlAccess.ValidUsername(username);
            } while (validInput == false);
            string password = ExtraUtilities.GetStringAnswer("Choose Password");
            string name = ExtraUtilities.GetStringAnswer("Choose display name");
            int age = ExtraUtilities.GetIntAnswer("Age");
            string sex = ExtraUtilities.GetStringAnswer("What sex do you identify as?");
            string attractedTo = ExtraUtilities.GetStringAnswer("What sex/sexes are you attracted to? Seperate by \", \"", true);
            int height = ExtraUtilities.GetIntAnswer("How tall are you? In cm");
            string skinColor = ExtraUtilities.GetStringAnswer("What is your ethnicity?");
            string hairColor = ExtraUtilities.GetStringAnswer("What is your hair color?");
            string eyeColor = ExtraUtilities.GetStringAnswer("What color are your eyes?");
            string interest = ExtraUtilities.GetStringAnswer("What are your interest? Seperate by \", \"", true);

            sqlAccess.RunNonquery($"INSERT INTO [Security] VALUES ('{username}', '{password}')");
            sqlAccess.RunNonquery($"INSERT INTO [Profile] VALUES ('{username}', '{name}', {age}, '{sex}', '{attractedTo}', {height}, '{skinColor}', '{hairColor}', '{eyeColor}', '{interest}')");

            age = ExtraUtilities.GetIntAnswer("What age people are you looking for, minimum will be set to 18"); 
            sex = attractedTo;
            attractedTo = sex;
            height = ExtraUtilities.GetIntAnswer("How tall should they be? In cm");
            skinColor = ExtraUtilities.GetStringAnswer("What ethnicity? Seperate by \", \"", true);
            hairColor = ExtraUtilities.GetStringAnswer("What hair colors? Seperate by \", \"", true); 
            eyeColor = ExtraUtilities.GetStringAnswer("What color eyes? Seperate by \", \"", true);
            interest = ExtraUtilities.GetStringAnswer("What interest shoudl they have? Seperate by \", \"", true);

            sqlAccess.RunNonquery($"INSERT INTO [Preferences] VALUES ('{username}', {age}, '{sex}', '{attractedTo}', {height}, '{skinColor}', '{hairColor}', '{eyeColor}', '{interest}')");
        }
        public Profile GetUserInfo(string username)
        {
            
            Profile profile = new Profile();
            string[] userInfo = sqlAccess.RunQuery($"SELECT * FROM [Profile] WHERE Username = '{username}'").Split(',');
            profile.Username = userInfo[0];
            profile.Name = userInfo[1];
            profile.Age = Convert.ToInt32(userInfo[2]);
            profile.Sex = userInfo[3];
            profile.AttractedTo = userInfo[4].Split(";").ToList();
            profile.Heigth = Convert.ToInt32(userInfo[5]);
            profile.SkinColor = userInfo[6].Split(";").ToList();
            profile.HairColor = userInfo[7].Split(";").ToList();
            profile.EyeColor = userInfo[8].Split(";").ToList();//Discovery, even if split doesn't affect the string it stil makes in an array that can be turned into a list even though a string cant be cast as list
            profile.Interest = userInfo[9].Split(";").ToList();
            return profile;
        }
        public List<Profile> GetUserInfo(string sex, int age, int height)
        {
            List<Profile> profileList = new List<Profile>();
            
            List<string> userInfoList = sqlAccess.RunQueryList($"SELECT Name, Age, Sex, Height, SkinColor, HairColor, EyeColor, Interest FROM [Profile] WHERE Sex = '{sex}' AND Age >= {age - 3} AND Age <= {age + 3} AND Height >= {height * 0.8} AND Height <= {height * 1.2}");
            foreach (string item in userInfoList)
            {
                string[] userInfo = item.Split(",");
                Profile profile = new Profile();
                profile.Name = userInfo[0];
                profile.Age = Convert.ToInt32(userInfo[1]);
                profile.Sex = userInfo[2];
                profile.Heigth = Convert.ToInt32(userInfo[3]);
                profile.SkinColor = userInfo[4].Split(";").ToList();
                profile.HairColor = userInfo[5].Split(";").ToList();
                profile.EyeColor = userInfo[6].Split(";").ToList();//Discovery, even if split doesn't affect the string it stil makes in an array that can be turned into a list even though a string cant be cast as list
                profile.Interest = userInfo[7].Split(";").ToList();
                profileList.Add(profile);
            }
            
            return profileList;
        }
        public Preference GetUserPreference(string username)
        {
            Preference preference = new Preference();
            
            string[] userInfo = sqlAccess.RunQuery($"SELECT * FROM [Preferences] WHERE Username = '{username}'").Split(',');
            preference.Username = userInfo[0];
            preference.Age = Convert.ToInt32(userInfo[1]);
            preference.Sex = userInfo[2];
            preference.AttractedTo = userInfo[3].Split(";").ToList();
            preference.Heigth = Convert.ToInt32(userInfo[4]);
            preference.SkinColor = userInfo[5].Split(";").ToList();
            preference.HairColor = userInfo[6].Split(";").ToList();
            preference.EyeColor = userInfo[7].Split(";").ToList();//Discovery, even if split doesn't affect the string it stil makes in an array that can be turned into a list even though a string cant be cast as list
            preference.Interest = userInfo[8].Split(";").ToList();

            return preference;
        }
        public void DeleteUserQuery(string username)
        {
            sqlAccess.RunNonquery($"DELETE FROM [Security] WHERE Username = '{username}'");
            sqlAccess.RunNonquery($"DELETE FROM [Profile] WHERE Username = '{username}'");
            sqlAccess.RunNonquery($"DELETE FROM [Preferences] WHERE Username = '{username}'");
        }
    }
    class ProfileManagement
    {
        private SqlManagement sqlAccess;

        public ProfileManagement(SqlManagement sqlA)
        {
            sqlAccess = sqlA;
        }
        public void EditUserInfo(Security security, string username)
        {
            Console.Clear();
            Console.WriteLine("Choose an option");
            Profile profile = security.GetUserInfo(username);
            string[] options = {"Name", "Age", "Sex", "Sexual Orientation", "Height", "Skin color", "Hair color", "Eye color", "Interest"};
            ExtraUtilities.MenuOptions(options);
            Console.WriteLine("0 -> Exit");
            string editString;
            int editInt;
            char menuChoice = Console.ReadKey().KeyChar;
            switch (menuChoice)
            {
                case '1':
                    editString = ExtraUtilities.GetStringAnswer($"Your current {options[int.Parse(menuChoice.ToString()) - 1]} is set to {profile.Name}. What do you want to change it to? -> ");
                    sqlAccess.RunNonquery($"UPDATE [Profile] SET [Name] = '{editString}' WHERE Username = '{profile.Username}'");
                    break;
                case '2':
                    editInt = ExtraUtilities.GetIntAnswer($"Your current {options[int.Parse(menuChoice.ToString()) - 1]} is set to {profile.Age}. What do you want to change it to? -> ");
                    sqlAccess.RunNonquery($"UPDATE [Profile] SET Age = {editInt} WHERE Username = '{profile.Username}'");
                    break;
                case '3':
                    editString = ExtraUtilities.GetStringAnswer($"Your current {options[int.Parse(menuChoice.ToString()) - 1]} is set to {profile.Sex}. What do you want to change it to? -> ");
                    sqlAccess.RunNonquery($"UPDATE [Profile] SET [Sex] = '{editString}' WHERE Username = '{profile.Username}'");
                    break;
                case '4':
                    editString = ExtraUtilities.GetStringAnswer($"Your current {options[int.Parse(menuChoice.ToString()) - 1]} is set to {ExtraUtilities.StringFromList(profile.AttractedTo)}. What do you want to change it to? Seperate by \", \" -> ", true);
                    sqlAccess.RunNonquery($"UPDATE [Profile] SET [AttractedTo] = '{editString}' WHERE Username = '{profile.Username}'");
                    break;
                case '5':
                    editInt = ExtraUtilities.GetIntAnswer($"Your current {options[int.Parse(menuChoice.ToString()) - 1]} is set to {profile.Heigth}. What do you want to change it to? -> ");
                    sqlAccess.RunNonquery($"UPDATE [Profile] SET [Height] = {editInt} WHERE Username = '{profile.Username}'");
                    break;
                case '6':
                    editString = ExtraUtilities.GetStringAnswer($"Your current {options[int.Parse(menuChoice.ToString()) - 1]} is set to {ExtraUtilities.StringFromList(profile.SkinColor)}. What do you want to change it to? -> ");
                    sqlAccess.RunNonquery($"UPDATE [Profile] SET [SkinColor] = '{editString}' WHERE Username = '{profile.Username}'");
                    break;
                case '7':
                    editString = ExtraUtilities.GetStringAnswer($"Your current {options[int.Parse(menuChoice.ToString()) - 1]} is set to {ExtraUtilities.StringFromList(profile.HairColor)}. What do you want to change it to? Seperate by \", \" -> ", true);
                    sqlAccess.RunNonquery($"UPDATE [Profile] SET [HairColor] = '{editString}' WHERE Username = '{profile.Username}'");
                    break;
                case '8':
                    editString = ExtraUtilities.GetStringAnswer($"Your current {options[int.Parse(menuChoice.ToString()) - 1]} is set to {ExtraUtilities.StringFromList(profile.EyeColor)}. What do you want to change it to? Seperate by \", \" -> ", true);
                    sqlAccess.RunNonquery($"UPDATE [Profile] SET [EyeColor] = '{editString}' WHERE Username = '{profile.Username}'");
                    break;
                case '9':
                    editString = ExtraUtilities.GetStringAnswer($"Your current {options[int.Parse(menuChoice.ToString()) - 1]} is set to {ExtraUtilities.StringFromList(profile.Interest)}. What do you want to change it to? Seperate by \", \" -> ", true);
                    sqlAccess.RunNonquery($"UPDATE [Profile] SET [Interest] = '{editString}' WHERE Username = '{profile.Username}'");
                    break;
                case 'e':
                    break;


            }
            Console.Clear();
            Console.WriteLine("Changes Saved!");
            Console.ReadKey();

        }

        public void EditUserPreferences(Security security, string username)
        {
            Console.Clear();
            Console.WriteLine("Choose an option");
            Preference preference = security.GetUserPreference(username);
            string[] options = { "Age", "Sex", "Sexual Orientation", "Height", "Skin color", "Hair color", "Eye color", "Interest" };
            ExtraUtilities.MenuOptions(options);
            Console.WriteLine("0 -> Exit");
            string editString;
            int editInt;
            char menuChoice = Console.ReadKey().KeyChar;
            switch (menuChoice)
            {
                case '1':
                    editInt = ExtraUtilities.GetIntAnswer($"Your current {options[int.Parse(menuChoice.ToString()) - 1]} preference is set to {preference.Age}. What do you want to change it to? -> ");
                    sqlAccess.RunNonquery($"UPDATE [Preferences] SET Age = {editInt} WHERE Username = '{preference.Username}'");
                    break;
                case '2':
                    editString = ExtraUtilities.GetStringAnswer($"Your current {options[int.Parse(menuChoice.ToString()) - 1]} preference is set to {preference.Sex}. What do you want to change it to? -> ");
                    sqlAccess.RunNonquery($"UPDATE [Preferences] SET [Sex] = '{editString}' WHERE Username = '{preference.Username}'");
                    break;
                case '3':
                    editString = ExtraUtilities.GetStringAnswer($"Your current {options[int.Parse(menuChoice.ToString()) - 1]} preference is set to {ExtraUtilities.StringFromList(preference.AttractedTo)}. What do you want to change it to? Seperate by \", \" -> ", true);
                    sqlAccess.RunNonquery($"UPDATE [Preferences] SET [AttractedTo] = '{editString}' WHERE Username = '{preference.Username}'");
                    break;
                case '4':
                    editInt = ExtraUtilities.GetIntAnswer($"Your current {options[int.Parse(menuChoice.ToString()) - 1]} preference is set to {preference.Heigth}. What do you want to change it to? -> ");
                    sqlAccess.RunNonquery($"UPDATE [Preferences] SET [Height] = {editInt} WHERE Username = '{preference.Username}'");
                    break;
                case '5':
                    editString = ExtraUtilities.GetStringAnswer($"Your current {options[int.Parse(menuChoice.ToString()) - 1]} preference is set to {ExtraUtilities.StringFromList(preference.SkinColor)}. What do you want to change it to? Seperate by \", \" -> ", true);
                    sqlAccess.RunNonquery($"UPDATE [Preferences] SET [SkinColor] = '{editString}' WHERE Username = '{preference.Username}'");
                    break;
                case '6':
                    editString = ExtraUtilities.GetStringAnswer($"Your current {options[int.Parse(menuChoice.ToString()) - 1]} preference is set to {ExtraUtilities.StringFromList(preference.HairColor)}. What do you want to change it to? Seperate by \", \" -> ", true);
                    sqlAccess.RunNonquery($"UPDATE [Preferences] SET [HairColor] = '{editString}' WHERE Username = '{preference.Username}'");
                    break;
                case '7':
                    editString = ExtraUtilities.GetStringAnswer($"Your current {options[int.Parse(menuChoice.ToString()) - 1]} preference is set to {ExtraUtilities.StringFromList(preference.EyeColor)}. What do you want to change it to? Seperate by \", \" -> ", true);
                    sqlAccess.RunNonquery($"UPDATE [Preferences] SET [EyeColor] = '{editString}' WHERE Username = '{preference.Username}'");
                    break;
                case '8':
                    editString = ExtraUtilities.GetStringAnswer($"Your current {options[int.Parse(menuChoice.ToString()) - 1]} preference is set to {ExtraUtilities.StringFromList(preference.Interest)}. What do you want to change it to? Seperate by \", \" -> ", true);
                    sqlAccess.RunNonquery($"UPDATE [Preferences] SET [Interest] = '{editString}' WHERE Username = '{preference.Username}'");
                    break;
                case 'e':
                    break;
            }
            Console.Clear();
            Console.WriteLine("Changes Saved!");
            Console.ReadKey();
        }

        public bool DeleteUser(Security security, string username)
        {
            Console.Clear();
            Console.Write("Are you certain you want to delete your account y/n");
            char choice = Console.ReadKey().KeyChar;
            Console.Clear();
            bool delete = false;
            if (choice == 'y' || choice == 'Y')
            {
                if (security.Login() == username)
                {
                    security.DeleteUserQuery(username);
                    delete = true;
                }
            }
            return delete;

        }
    }
    class Search
    {
        public void FindMatch(Security security, string username)
        {
            Console.Clear();
            Preference preference = security.GetUserPreference(username);
            List<Profile> possibleMatch = security.GetUserInfo(preference.Sex, preference.Age, preference.Heigth);

            foreach (Profile item in possibleMatch)
            {
                Console.WriteLine($"Name: {item.Name} Gender: {item.Sex} Age: {item.Age} Height: {item.Heigth} Ethnicity: {ExtraUtilities.StringFromList(item.SkinColor)} Hair Color: {ExtraUtilities.StringFromList(item.HairColor)} Eye Color: {ExtraUtilities.StringFromList(item.EyeColor)} Interest: {ExtraUtilities.StringFromList(item.Interest)}");
                
            }
            Console.ReadKey();
        }
    }
    static class Menu
    {
        
        public static bool StartScreen(Security security, ProfileManagement profileManagement, Search search)
        {
            Console.Clear();
            Console.WriteLine("Hello user!\n1 -> Log into existing user\n2 -> Create new user\n3 -> Exit");
            char choice = Console.ReadKey().KeyChar;
            Console.Clear();
            bool exit = false;
            bool exitProfileScreen = false;
            switch (choice)
            {
                case '1':
                    string username = security.Login();
                    if(username != "")
                    {
                        do
                        {
                           exitProfileScreen = ProfileScreen(profileManagement, security, search, username);
                        } while (exitProfileScreen == false);
                        
                    }
                    break;
                case '2':
                    security.CreateNewUser();
                    break;
                 case '3':
                    exit = true;
                    return exit;
                    

            }
            return exit;
        }

        private static bool ProfileScreen(ProfileManagement profileManagment, Security security, Search search, string username)
        {
            Console.Clear();
            bool exit = false;
            Console.WriteLine($"Hello {username}!\n1 -> Search for matches\n2 -> Edit user information\n3 -> Edit user preferences\n4 -> Delete account\n5 -> Exit");
            char choice = Console.ReadKey().KeyChar;
            switch (choice)
            {
                case '1':
                    search.FindMatch(security, username);
                    break;
                case '2':
                    profileManagment.EditUserInfo(security, username);
                    break;

                case '3':
                    profileManagment.EditUserPreferences(security, username);
                    break;

                case '4':
                    exit = profileManagment.DeleteUser(security, username);
                    break;
                case '5':
                    exit = true;
                    break;
            }

            return exit;
        }
    }
    static class ExtraUtilities
    {
      
        public static string GetStringAnswer(string questionText)
        {
            bool isNull = true;
            string answer = "";
            do
            {
                Console.Clear();
                Console.Write($"{questionText}: ");
                answer = Console.ReadLine().Trim();
                isNull = IsNull(answer);
                
            } while (isNull == true);
            return answer;
        }
        public static string GetStringAnswer(string questionText, bool listFormat)
        {
            bool isNull = true;
            string answer = "";
            do
            {
                Console.Clear();
                Console.Write($"{questionText}: ");
                answer = Console.ReadLine().Replace(", ", ";").Trim();
                isNull = IsNull(answer);
                

            } while (isNull == true);
            return answer;
        }
        public static int GetIntAnswer(string questionText)
        {
            bool validInput = false;
            int answer = 0;
            Console.Clear();
            
            do
            {
                Console.Write($"{questionText}: ");
                validInput = int.TryParse(Console.ReadLine(), out answer);
                Console.Clear();
                if (validInput == false || answer < 18)
                {
                    validInput = false;
                    Console.WriteLine("Wrong! Try again.");
                    Console.ReadKey();
                    Console.Clear();
                    
                }
            } while (validInput == false);
            return answer;
        }
       
        public static bool IsNull(string answer)
        {
            bool isNull = false;
            if(answer == null || answer == "")
            {
                isNull = true;
                Console.WriteLine("The string is null, it isn't valid");
            }
            return isNull;
        }

        public static string StringFromList(List<string> listToString)
        {
            /*Turns a given list<string> into a single string seperated by ", "*/
            int i = 0;
            string returnString = "";
            foreach (string item in listToString)
            {
                if (i == listToString.Count - 1)
                {
                    returnString += item;
                } 
                else
                {
                    returnString += item + ", ";
                }
                i++;
            }
            return returnString;
        }

        public static void MenuOptions(string[] menuOptions)
        {
            
            for (int i = 0; i < menuOptions.Length; i++)
            {
                Console.WriteLine($"{i+1} -> {menuOptions[i]}");
            }
        }
    }
    class Program
    {
       
        static void Main(string[] args)
        {
            Console.ReadKey();
            Search search = new Search();
            SqlManagement sqlAccess = new SqlManagement();
            ProfileManagement profileManagement = new ProfileManagement(sqlAccess);
            Security security = new Security(sqlAccess);
            bool exit = false;
            do
            {
                exit = Menu.StartScreen(security, profileManagement, search);
            } while (exit == false);
        }
    }
}

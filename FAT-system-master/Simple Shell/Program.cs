namespace simple_Shell
{
    class Program
    {
        public static string PATH_ON_PC = "testtt.TXT"; // Path to the virtual disk file
        public static Directory current; // Current directory
        public static string currentPath; // Current path

        // Main method
        static void Main(string[] args)
        {
            VirtualDisk.initialize(PATH_ON_PC); // Initialize the virtual disk
            currentPath = new string(current.dir_name).Trim(); // Set the current path

            Parser parser = new Parser(); // Create a parser

            while (true) // Main loop
            {
                Console.Write(currentPath + "\\>"); // Display the prompt
                current.ReadDirectory(); // Read the current directory

                string input = Console.ReadLine(); // Get user input
                parser.parse_input(input); // Parse and execute the input
            }
        }
    }
}
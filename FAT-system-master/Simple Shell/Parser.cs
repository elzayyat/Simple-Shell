namespace simple_Shell
{
    class Parser
    {
        // Parse (تقسم) user input
        public void parse_input(string str)
        {
            Token token = new Token(); // Create a Token to store the command and arguments
            var argument = str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); // Split input into arguments

            // Handle different argument lengths
            if (argument.Length == 1) // If there is only a command
            {
                token.command = argument[0]; // Set the command
                token.value = null; // No value (first argument)
                token.sec_value = null; // No secondary value (second argument)
                action(token); // Execute the action based on the Token
            }
            else if (argument.Length == 2) // If there is a command and one argument
            {
                token.command = argument[0]; // Set the command
                token.value = argument[1]; // Set the value (first argument)
                token.sec_value = null; // No secondary value (second argument)
                action(token); // Execute the action based on the Token
            }
            else if (argument.Length == 3) // If there is a command and two arguments
            {
                token.command = argument[0]; // Set the command
                token.value = argument[1]; // Set the value (first argument)
                token.sec_value = argument[2]; // Set the secondary value (second argument)
                action(token); // Execute the action based on the Token
            }
        }

        // Execute actions based on the Token
        void action(Token token)
        {
            switch (token.command) // Switch based on the command
            {
                case "cls": // Clear the console
                    if (token.value != null) // If an argument is provided
                    {
                        Console.WriteLine("Error: 'cls' command does not accept any arguments.");
                    }
                    else // If no argument is provided
                    {
                        Console.Clear(); // Clear the console
                    }
                    break;

                case "quit": // Quit the shell
                    Environment.Exit(0); // Exit the program
                    break;

                case "help": // Display help
                    if (string.IsNullOrWhiteSpace(token.value)) // If no specific command is provided -- fun gahza 
                    {
                        Help helper = new Help(); // Create a Help object to display general help
                        helper.doHelp(token); // Display all available commands
                    }
                    else // If a specific command is provided
                    {
                        Help helper = new Help(token); // Create a Help object to display help for the specific command
                    }
                    break;

                case "cd": // Change directory
                    if (token.value == null) // If no directory is specified
                        return; // Do nothing
                    else
                        cd(token.value); // Call the cd method to change the directory
                    break;

                case "md": // Make directory
                    if (token.value == null) // If no directory name is specified
                        Console.WriteLine("ERROR, you should specify folder name to make\n md[path]name");
                    else
                        md(token.value); // Call the md method to create the directory
                    break;

                case "dir": // List directory contents
                    dir(token.value); // Pass the directory path argument to the dir method
                    break;

                case "rd": // Remove directory
                    if (token.value == null) // If no directory name is specified
                        Console.WriteLine("ERROR,\n you should specify folder name to delete\n rd[pah]Name");
                    else
                        rd(token.value); // Call the rd method to remove the directory
                    break;

                case "import": // Import file from the computer
                    if (token.value == null) // If no file name is specified
                        Console.WriteLine("ERROR\n, you should specify File name to import\n import [dest]filename");
                    else
                        import(token.value); // Call the import method to import the file
                    break;

                case "type": // Display file content
                    if (token.value == null || token.sec_value != null) // If no file name is specified or extra arguments are provided
                        Console.WriteLine("ERROR\n, you should specify file name to show its content\n type [dest]filename");
                    else
                        type(token.value); // Call the type method to display the file content
                    break;

                case "export": // Export file to the computer
                    if (token.value == null || token.sec_value == null) // If source or destination is not specified
                        Console.WriteLine("ERROR,\nThe Correct syntax is \n import   [Source File] [destination]\n");
                    else
                        export(token.value, token.sec_value); // Call the export method to export the file
                    break;

                case "rename": // Rename a file or directory
                    if (token.value == null || token.sec_value == null) // If old or new name is not specified
                        Console.WriteLine("ERROR,\nThe Correct syntax is \n rename   [old name] [new name]\n");
                    else
                        rename(token.value, token.sec_value); // Call the rename method to rename the file or directory
                    break;

                case "del": // Delete a file
                    if (token.value == null) // If no file name is specified
                        Console.WriteLine("ERROR,\nThe Correct syntax is \n del   [file name]\n");
                    else
                        del(token.value); // Call the del method to delete the file
                    break;

                case "copy": // Copy a file
                    if (token.value == null || token.sec_value == null) // If source or destination is not specified
                        Console.WriteLine("ERROR,\nThe Correct syntax is \n copy   [Source File] [destination]\n");
                    else
                        copy(token.value, token.sec_value); // Call the copy method to copy the file
                    break;

                case "create": // Create a new file
                    if (token.value == null) // If no file name is specified
                    {
                        Console.WriteLine("ERROR: You must specify a file name.\nUsage: create <filename>");
                    }
                    else
                    {
                        createFile(token.value); // Call the createFile method to create the file
                    }
                    break;

                case "tree": // Display directory structure in a tree-like format
                    if (token.value != null || token.sec_value != null) // If any arguments are provided
                    {
                        Console.WriteLine("ERROR: The 'tree' command does not accept any arguments.\nUsage: tree");
                    }
                    else
                    {
                        tree(Program.current, 0); // Call the tree method to display the directory structure
                    }
                    break;

                case "clearphysical": // Clear content of a physical file
                    if (token.value == null) // If no file path is specified
                    {
                        Console.WriteLine("ERROR: You must specify a file path.\nUsage: clearphysical <filepath>");
                    }
                    else
                    {
                        FileOperations.ClearPhysicalFileContent(token.value); // Call the ClearPhysicalFileContent method
                    }
                    break;

                default: // Unknown command
                    Console.WriteLine("Unknown Command.."); // Display an error message
                    break;
            }
        }
//--------------------------------------------------------------------------------------------------


        // Create a new file
        public static void createFile(string fileName, string content = "")
        {
            if (Program.current.searchDirectory(fileName) != -1) // If the file already exists
            {
                Console.WriteLine($"ERROR: File '{fileName}' already exists.");
                return;
            }

            int size = content.Length; // Get the size of the content
            int fc = size > 0 ? FAT.GetEmptyCulster() : 0; // Get an empty cluster if content exists
            FILE newFile = new FILE(fileName, 0x0, fc, Program.current, content, size); // Create a FILE object
            newFile.writeFile(); // Write the file to disk

            DirectoryEntry d = new DirectoryEntry(fileName, 0x0, fc, size); // Create a directory entry
            Program.current.entries.Add(d); // Add the entry to the current directory
            Program.current.WriteDirectory(); // Write the directory to disk
            Console.WriteLine($"File '{fileName}' created successfully.");
        }

        // Display directory structure in a tree-like format
        public static void tree(Directory dir, int level)
        {
            string indent = new string(' ', level * 4); // Create indentation based on the level
            Console.WriteLine($"{indent}{new string(dir.dir_name).TrimEnd(' ', '_', '.')}"); // Display the directory name (trimmed)

            foreach (var entry in dir.entries) // Iterate through directory entries
            {
                string entryName = new string(entry.dir_name).TrimEnd(' ', '_', '.'); // Trim underscores, spaces, and dots
                if (entry.dir_attr == 0x10) // If the entry is a directory
                {
                    Directory subDir = new Directory(new string(entry.dir_name), entry.dir_attr, entry.firs_cluster, dir); // Create a Directory object
                    subDir.ReadDirectory(); // Read the subdirectory
                    tree(subDir, level + 1); // Recursively display the subdirectory
                }
                else // If the entry is a file
                {
                    Console.WriteLine($"{indent}    {entryName}"); // Display the file name (trimmed)
                }
            }
        }

        // Display the contents of a file
        public static void type(string name)
        {
            int j = Program.current.searchDirectory(name); // Search for the file in the current directory
            if (j != -1) // If the file exists
            {
                DirectoryEntry entry = Program.current.entries[j]; // Get the directory entry
                if (entry.dir_attr == 0x0) // If the entry is a file
                {
                    FILE file = new FILE(name, 0x0, entry.firs_cluster, Program.current, "", entry.dir_fileSize); // Create a FILE object
                    file.ReadFile(); // Read the file content
                    Console.WriteLine(file.content); // Display the content
                }
                else
                {
                    Console.WriteLine($"Error: '{name}' is not a file."); // Not a file
                }
            }
            else
            {
                Console.WriteLine("The System Cannot Find the File specified"); // File not found
            }
        }

        // Change the current directory
        public static void cd(string path = null)
        {
            if (path == null) // If no argument is provided
            {
                // Change to the home directory (e.g., M:\)
                Directory rootDirectory = new Directory("M:", 0x10, 5, null); // Create the root directory
                rootDirectory.ReadDirectory(); // Read the root directory
                Program.current = rootDirectory; // Set the current directory to the root directory
                Program.currentPath = "M:"; // Update the current path
                return;
            }

            Directory dir = changeMyCurrentDirectory(path, true, false); // Change to the specified directory
            if (dir != null) // If the directory exists
            {
                if (dir.dir_attr == 0x10) // If the target is a directory
                {
                    dir.ReadDirectory(); // Read the directory
                    Program.current = dir; // Update the current directory
                }
                else // If the target is a file
                {
                    Console.WriteLine($"Error: '{path}' is not a directory."); // Display error message
                }
            }
            else // If the directory does not exist
            {
                Console.WriteLine($"Error: Directory '{path}' does not exist."); // Display error message
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------

        // Move to a directory used in another operation
        public static void moveToDirUsedInAnother(string path)
        {
            Directory dir = changeMyCurrentDirectory(path, false, false); // Change to the specified directory
            if (dir != null) // If the directory exists
            {
                dir.ReadDirectory(); // Read the directory
                Program.current = dir; // Update the current directory
            }
            else
            {
                Console.WriteLine("The system cannot find the specified folder.!"); // Directory not found
            }
        }

        // Change the current directory
        private static Directory changeMyCurrentDirectory(string p, bool usedInCD, bool isUsedInRD)
        {
            Directory d = null; // Initialize directory
            string[] arr = p.Split('\\'); // Split the path
            string path;
            if (arr.Length == 1) // If the path is a single directory name
            {
                if (arr[0] != "..") // If the directory is not ".." (parent directory)
                {
                    int i = Program.current.searchDirectory(arr[0]); // Search for the directory
                    if (i == -1) // If the directory does not exist
                        return null; // Return null
                    else
                    {
                        string nameOfDesiredFolder = new string(Program.current.entries[i].dir_name); // Get the directory name
                        byte attr = Program.current.entries[i].dir_attr; // Get the directory attribute
                        int firstCluster = Program.current.entries[i].firs_cluster; // Get the first cluster
                        d = new Directory(nameOfDesiredFolder, attr, firstCluster, Program.current); // Create a Directory object
                        d.ReadDirectory(); // Read the directory
                        path = Program.currentPath; // Update the current path
                        path += "\\" + nameOfDesiredFolder.Trim(); // Add the directory name to the path
                        if (usedInCD) // If used in cd command
                            Program.currentPath = path; // Update the current path
                    }
                }
                else // If the directory is ".." (parent directory)
                {
                    if (Program.current.parent != null) // If the parent directory exists
                    {
                        d = Program.current.parent; // Set the parent directory
                        d.ReadDirectory(); // Read the parent directory
                        path = Program.currentPath; // Update the current path
                        path = path.Substring(0, path.LastIndexOf('\\')); // Remove the last directory from the path
                        if (usedInCD) // If used in cd command
                            Program.currentPath = path; // Update the current path
                    }
                    else
                    {
                        d = Program.current; // Set the current directory
                        d.ReadDirectory(); // Read the current directory
                    }
                }
            }
            else if (arr.Length > 1) // If the path contains multiple directories
            {
                List<string> ListOfHandledPath = new List<string>(); // Initialize list to handle the path
                for (int i = 0; i < arr.Length; i++) // Iterate through the path
                    if (arr[i] != " ") // If the directory is not empty
                        ListOfHandledPath.Add(arr[i]); // Add the directory to the list

                Directory rootDirectory = new Directory("M:", 0x10, 5, null); // Create the root directory
                rootDirectory.ReadDirectory(); // Read the root directory

                if (ListOfHandledPath[0].Equals("m:") || ListOfHandledPath[0].Equals("M:")) // If the path starts with "M:"
                {
                    path = "M:"; // Set the path to "M:"
                    int howLongIsMyWay;
                    if (isUsedInRD || usedInCD) // If used in rd or cd command
                        howLongIsMyWay = ListOfHandledPath.Count; // Set the length of the path
                    else
                        howLongIsMyWay = ListOfHandledPath.Count - 1; // Set the length of the path minus one

                    for (int i = 1; i < howLongIsMyWay; i++) // Iterate through the path
                    {
                        int j = rootDirectory.searchDirectory(ListOfHandledPath[i]); // Search for the directory
                        if (j != -1) // If the directory exists
                        {
                            Directory tempOfParent = rootDirectory; // Set the parent directory
                            string newName = new string(rootDirectory.entries[j].dir_name); // Get the directory name
                            byte attr = rootDirectory.entries[j].dir_attr; // Get the directory attribute
                            int fc = rootDirectory.entries[j].firs_cluster; // Get the first cluster
                            rootDirectory = new Directory(newName, attr, fc, tempOfParent); // Create a Directory object
                            rootDirectory.ReadDirectory(); // Read the directory
                            path += "\\" + newName.Trim(); // Add the directory name to the path
                        }
                        else
                        {
                            return null; // Directory not found
                        }
                    }
                    d = rootDirectory; // Set the directory
                    if (usedInCD) // If used in cd command
                        Program.currentPath = path; // Update the current path
                }
                else if (ListOfHandledPath[0] == "..") // If the path starts with ".." (parent directory)
                {
                    d = Program.current; // Set the current directory
                    for (int i = 0; i < ListOfHandledPath.Count; i++) // Iterate through the path
                    {
                        if (d.parent != null) // If the parent directory exists
                        {
                            d = d.parent; // Set the parent directory
                            d.ReadDirectory(); // Read the parent directory
                            path = Program.currentPath; // Update the current path
                            path = path.Substring(0, path.LastIndexOf('\\')); // Remove the last directory from the path
                            if (usedInCD) // If used in cd command
                                Program.currentPath = path; // Update the current path
                        }
                        else
                        {
                            break; // Stop if no parent directory exists
                        }
                    }
                }
                else
                    return null; // Invalid path
            }
            return d; // Return the directory
        }

        // Check if a folder name is valid
        public static bool IsValidFolderName(string name)
        {
            // Define a list of invalid symbols
            char[] invalidSymbols = { '/', '\\', ':', '*', '?', '"', '<', '>', '|' };

            // Check if the name contains any invalid symbols
            foreach (char symbol in invalidSymbols)
            {
                if (name.Contains(symbol))
                {
                    return false; // Name contains an invalid symbol
                }
            }

            return true; // Name is valid
        }

        // Create a directory
        public static void md(string name)
        {
            // Check if the directory name contains a file extension
            if (name.Contains('.'))
            {
                Console.WriteLine("Error: Directory names cannot contain file extensions (e.g., '.txt').");
                return; // Exit the method without creating the directory
            }

            // Check if the folder name contains invalid symbols
            if (!IsValidFolderName(name))
            {
                Console.WriteLine("Error: Folder name contains invalid symbols. The following symbols are not allowed: / \\ : * ? \" < > |");
                return; // Exit the method without creating the directory
            }

            string[] arr = name.Split('\\'); // Split the path
            if (arr.Length == 1) // If the path is a single directory name
            {
                if (Program.current.searchDirectory(arr[0]) == -1) // If the directory does not exist
                {
                    DirectoryEntry d = new DirectoryEntry(arr[0], 0x10, 0, 0); // Create a directory entry

                    if (FAT.GetEmptyCulster() != -1) // If there is an empty cluster
                    {
                        Program.current.entries.Add(d); // Add the directory entry
                        Program.current.WriteDirectory(); // Write the directory to disk
                        if (Program.current.parent != null) // If the parent directory exists
                        {
                            Program.current.parent.updateContent(Program.current.getDirectoryEntry()); // Update the parent directory
                            Program.current.parent.WriteDirectory(); // Write the parent directory to disk
                        }
                        FAT.writeFat(); // Write the FAT to disk
                        Console.WriteLine($"Directory '{arr[0]}' created successfully.");
                    }
                    else
                    {
                        Console.WriteLine("The Disk is Full :("); // Disk is full
                    }
                }
                else
                {
                    Console.WriteLine($"{arr[0]} already exists."); // Directory already exists
                }
            }
            else if (arr.Length > 1) // If the path contains multiple directories
            {
                Directory dir = changeMyCurrentDirectory(name, false, false); // Change to the specified directory
                if (dir == null) // If the directory does not exist
                {
                    Console.WriteLine($"The Path {name} does not exist.");
                }
                else
                {
                    if (FAT.GetEmptyCulster() != -1) // If there is an empty cluster
                    {
                        DirectoryEntry d = new DirectoryEntry(arr[arr.Length - 1], 0x10, 0, 0); // Create a directory entry
                        dir.entries.Add(d); // Add the directory entry
                        dir.WriteDirectory(); // Write the directory to disk
                        dir.parent.updateContent(dir.getDirectoryEntry()); // Update the parent directory
                        dir.parent.WriteDirectory(); // Write the parent directory to disk
                        FAT.writeFat(); // Write the FAT to disk
                        Console.WriteLine($"Directory '{arr[arr.Length - 1]}' created successfully.");
                    }
                    else
                    {
                        Console.WriteLine("The Disk is Full :("); // Disk is full
                    }
                }
            }
        }

        // List directory contents
        public static void dir(string path = null)
        {
            Directory targetDir = Program.current; // Default to the current directory

            if (path != null) // If a directory path is provided
            {
                int index = Program.current.searchDirectory(path); // Search for the path in the current directory
                if (index == -1) // If the path does not exist
                {
                    Console.WriteLine($"Directory or file '{path}' not found.");
                    return;
                }

                DirectoryEntry entry = Program.current.entries[index]; // Get the directory entry
                if (entry.dir_attr == 0x0) // If the entry is a file
                {
                    Console.WriteLine($"Error: '{path}' is not a directory.");
                    return;
                }
                else if (entry.dir_attr == 0x10) // If the entry is a directory
                {
                    targetDir = new Directory(new string(entry.dir_name), entry.dir_attr, entry.firs_cluster, Program.current); // Create a Directory object
                    targetDir.ReadDirectory(); // Read the directory
                }
            }

            int fc = 0, dc = 0, fz_sum = 0; // Initialize counters for files, directories, and file sizes
            Console.WriteLine("Directory of " + (path ?? Program.currentPath)); // Display the target directory path
            Console.WriteLine();
            for (int i = 0; i < targetDir.entries.Count; i++) // Iterate through directory entries
            {
                string entryName = new string(targetDir.entries[i].dir_name).TrimEnd(' ', '_', '.'); // Trim underscores, spaces, and dots
                if (targetDir.entries[i].dir_attr == 0x0) // If the entry is a file
                {
                    Console.WriteLine($"\t{targetDir.entries[i].dir_fileSize} \t {entryName}"); // Display file size and name
                    fc++; // Increment file counter
                    fz_sum += targetDir.entries[i].dir_fileSize; // Add file size to total size
                }
                else if (targetDir.entries[i].dir_attr == 0x10) // If the entry is a directory
                {
                    Console.WriteLine($"\t<DIR> {entryName}"); // Display directory name
                    dc++; // Increment directory counter
                }
            }
            Console.WriteLine($"{"\t\t"}{fc} File(s)    {fz_sum} bytes"); // Display total files and size
            Console.WriteLine($"{"\t\t"}{dc} Dir(s)    {VirtualDisk.getFreeSpace()} bytes free"); // Display total directories and free space
        }

        // Remove a directory
        public static void rd(string name)
        {
            int index = Program.current.searchDirectory(name); // Search for the directory in the current directory
            if (index != -1) // If the directory exists
            {
                DirectoryEntry entry = Program.current.entries[index]; // Get the directory entry
                if (entry.dir_attr == 0x10) // If the entry is a directory
                {
                    Console.Write($"Are you sure you want to delete '{name}'? (Y/N): "); // Prompt for confirmation
                    string choice = Console.ReadLine().ToLower(); // Get user input
                    if (choice == "y") // If the user confirms
                    {
                        Directory dir = new Directory(new string(entry.dir_name), entry.dir_attr, entry.firs_cluster, Program.current); // Create a Directory object
                        dir.deleteDirectory(); // Delete the directory
                        Console.WriteLine($"Directory '{name}' deleted successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Deletion canceled."); // Deletion canceled
                    }
                }
                else // If the entry is not a directory
                {
                    Console.WriteLine($"Error: '{name}' is not a directory."); // Display error message
                }
            }
            else // If the directory does not exist
            {
                Console.WriteLine($"Error: Directory '{name}' does not exist."); // Display error message
            }
        }

        // Import a file from the computer
        public static void import(string dest)
        {
            if (File.Exists(dest)) // If the file exists on the computer
            {
                string content = File.ReadAllText(dest); // Read the file content
                int size = content.Length; // Get the file size
                string[] names = dest.Split("\\"); // Split the file path
                string name = names[names.Length - 1]; // Get the file name
                int j = Program.current.searchDirectory(name); // Search for the file in the current directory
                if (j == -1) // If the file does not exist in the current directory
                {
                    int fc;
                    if (size > 0) // If the file has content
                        fc = FAT.GetEmptyCulster(); // Get an empty cluster
                    else
                        fc = 0; // No cluster needed
                    FILE newFile = new FILE(name, 0X0, fc, Program.current, content, size); // Create a FILE object
                    newFile.writeFile(); // Write the file to disk

                    DirectoryEntry d = new DirectoryEntry(new string(name), 0X0, fc, size); // Create a directory entry
                    Program.current.entries.Add(d); // Add the directory entry
                    Program.current.WriteDirectory(); // Write the directory to disk
                }
                else
                {
                    Console.WriteLine($"{name} is already exist in your virtual disk"); // File already exists
                }
            }
            else
            {
                Console.WriteLine("The file you specified does not exist in your computer"); // File not found on the computer
            }
        }

        // Export a file to the computer
        public static void export(string source, string dest)
        {
            string[] path = source.Split("\\"); // Split the source path
            if (path.Length > 1) // If the source path contains subdirectories
            {
                Directory dir = changeMyCurrentDirectory(source, false, false); // Change to the specified directory
                if (dir == null) // If the directory does not exist
                    Console.WriteLine($"The Path {source} Is not exist");
                else
                {
                    source = path[path.Length - 1]; // Get the file name
                    int j = dir.searchDirectory(source); // Search for the file in the directory
                    if (j != -1) // If the file exists
                    {
                        if (System.IO.Directory.Exists(dest)) // If the destination directory exists on the computer
                        {
                            int fc = dir.entries[j].firs_cluster; // Get the first cluster
                            int sz = dir.entries[j].dir_fileSize; // Get the file size
                            string content = null; // Initialize content
                            FILE file = new FILE(source, 0x0, fc, dir, content, sz); // Create a FILE object
                            file.ReadFile(); // Read the file content
                            StreamWriter sw = new StreamWriter(dest + "\\" + source); // Create a StreamWriter
                            sw.Write(file.content); // Write the content to the file
                            sw.Flush(); // Flush the stream
                            sw.Close(); // Close the stream
                        }
                        else
                        {
                            Console.WriteLine("The system cannot find the path specified in the computer disk"); // Destination directory not found
                        }
                    }
                    else
                    {
                        Console.WriteLine("The system cannot find the file you want to export in the virtual disk"); // File not found in the virtual disk
                    }
                }
            }
            else // If the file is in the current directory
            {
                int j = Program.current.searchDirectory(source); // Search for the file in the current directory
                if (j != -1) // If the file exists
                {
                    if (System.IO.Directory.Exists(dest)) // If the destination directory exists on the computer
                    {
                        int fc = Program.current.entries[j].firs_cluster; // Get the first cluster
                        int sz = Program.current.entries[j].dir_fileSize; // Get the file size
                        string content = null; // Initialize content
                        FILE file = new FILE(source, 0x0, fc, Program.current, content, sz); // Create a FILE object
                        file.ReadFile(); // Read the file content
                        StreamWriter sw = new StreamWriter(dest + "\\" + source); // Create a StreamWriter
                        sw.Write(file.content); // Write the content to the file
                        sw.Flush(); // Flush the stream
                        sw.Close(); // Close the stream
                    }
                    else
                    {
                        Console.WriteLine("The system cannot find the path specified in the computer disk"); // Destination directory not found
                    }
                }
                else
                {
                    Console.WriteLine("The system cannot find the file you want to export in the virtual disk"); // File not found in the virtual disk
                }
            }
        }

        // Rename a file or directory
        // Rename a file or directory
        public static void rename(string oldName, string newName, string newContent = null)
        {
            string[] path = oldName.Split("\\"); // Split the old name path
            Directory dir = null;

            if (path.Length > 1) // If the old name path contains subdirectories
            {
                dir = changeMyCurrentDirectory(oldName, false, false); // Change to the specified directory
                if (dir == null) // If the directory does not exist
                {
                    Console.WriteLine($"The Path {oldName} Is not exist");
                    return;
                }
                oldName = path[path.Length - 1]; // Get the old name
            }
            else // If the old name is in the current directory
            {
                dir = Program.current; // Use the current directory
            }

            int j = dir.searchDirectory(oldName); // Search for the old name
            if (j == -1) // If the old name does not exist
            {
                Console.WriteLine("The System Cannot Find the File specified");
                return;
            }

            DirectoryEntry entry = dir.entries[j]; // Get the directory entry

            // Check if the new name already exists
            if (dir.searchDirectory(newName) != -1)
            {
                Console.WriteLine("Duplicate File Name exists or file cannot be found");
                return;
            }

            // Handle file names with or without extensions
            if (entry.dir_attr == 0x0) // If the entry is a file
            {
                string[] newNameParts = newName.Split('.'); // Split the new name into name and extension
                string fileName = newNameParts[0]; // Get the file name
                string fileExtension = newNameParts.Length > 1 ? newNameParts[1] : ""; // Get the extension (if any)

                char[] properName = getProperFileName(fileName.ToCharArray(), fileExtension.ToCharArray()); // Get the proper file name
                entry.dir_name = properName; // Update the directory entry name

                // If new content is provided, update the file content
                if (newContent != null)
                {
                    FILE file = new FILE(newName, 0x0, entry.firs_cluster, dir, newContent, newContent.Length); // Create a FILE object
                    file.writeFile(); // Write the new content to the file
                    entry.dir_fileSize = newContent.Length; // Update the file size
                }
            }
            else if (entry.dir_attr == 0x10) // If the entry is a directory
            {
                char[] properName = getProperDirName(newName.ToCharArray()); // Get the proper directory name
                entry.dir_name = properName; // Update the directory entry name
            }

            dir.entries[j] = entry; // Update the entry in the directory
            dir.WriteDirectory(); // Write the directory to disk
            Console.WriteLine($"File '{oldName}' renamed to '{newName}' successfully.");
        }

        // Delete a file
        public static void del(string fileName)
        {
            string[] path = fileName.Split("\\"); // Split the file name path
            if (path.Length > 1) // If the file name path contains subdirectories
            {
                Directory dir = changeMyCurrentDirectory(fileName, false, false); // Change to the specified directory
                if (dir == null) // If the directory does not exist
                {
                    Console.WriteLine($"The Path {fileName} Is not exist");
                    return;
                }
                else
                {
                    fileName = path[path.Length - 1]; // Get the file name
                    int j = dir.searchDirectory(fileName); // Search for the file in the directory
                    if (j != -1) // If the file exists
                    {
                        if (dir.entries[j].dir_attr == 0x0) // If the entry is a file
                        {
                            int fc = dir.entries[j].firs_cluster; // Get the first cluster
                            int sz = dir.entries[j].dir_fileSize; // Get the file size
                            FILE file = new FILE(fileName, 0x0, fc, dir, null, sz); // Create a FILE object
                            file.deleteFile(); // Delete the file
                            Console.WriteLine($"File '{fileName}' deleted successfully.");
                        }
                        else
                        {
                            Console.WriteLine($"Error: '{fileName}' is not a file."); // Not a file
                        }
                    }
                    else
                    {
                        Console.WriteLine("The System Cannot Find The file specified"); // File not found
                    }
                }
            }
            else // If the file is in the current directory
            {
                int j = Program.current.searchDirectory(fileName); // Search for the file in the current directory
                if (j != -1) // If the file exists
                {
                    if (Program.current.entries[j].dir_attr == 0x0) // If the entry is a file
                    {
                        int fc = Program.current.entries[j].firs_cluster; // Get the first cluster
                        int sz = Program.current.entries[j].dir_fileSize; // Get the file size
                        FILE file = new FILE(fileName, 0x0, fc, Program.current, null, sz); // Create a FILE object
                        file.deleteFile(); // Delete the file
                        Console.WriteLine($"File '{fileName}' deleted successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Error: '{fileName}' is not a file."); // Not a file
                    }
                }
                else
                {
                    Console.WriteLine("The System Cannot Find The file specified"); // File not found
                }
            }
        }

        // Copy a file
        public static void copy(string source, string dest)
        {
            int j = Program.current.searchDirectory(source); // Search for the source file in the current directory
            int fc;
            int sz;

            if (source == dest) // If the source and destination are the same
            {
                Console.WriteLine("The file cannot be copied onto itself"); // Display error message
                return;
            }

            if (j != -1) // If the source file exists
            {
                fc = FAT.GetEmptyCulster(); // Get an empty cluster
                sz = Program.current.entries[j].dir_fileSize; // Get the file size
                Directory dir = changeMyCurrentDirectory(dest, false, true); // Change to the destination directory
                if (dir == null) // If the destination directory does not exist
                {
                    Console.WriteLine($"The Path {source} Is not exist"); // Display error message
                    return;
                }

                int x = dir.searchDirectory(source); // Search for the source file in the destination directory
                if (x != -1) // If the source file exists in the destination directory
                {
                    Console.Write("The File is already existed, Do you want to overwrite it? Please enter Y for yes or N for no:"); // Prompt for confirmation
                    string choice = Console.ReadLine().ToLower(); // Get user input
                    if (choice.Equals("y")) // If the user confirms
                    {
                        int f = Program.current.entries[j].firs_cluster; // Get the first cluster
                        string content = null; // Initialize content
                        FILE file = new FILE(source, 0x0, f, dir, content, sz); // Create a FILE object
                        file.ReadFile(); // Read the file content
                        content = file.content; // Get the file content

                        FILE newFile = new FILE(source, 0X0, fc, dir, content, sz); // Create a new FILE object
                        newFile.writeFile(); // Write the file to disk

                        DirectoryEntry d = new DirectoryEntry(new string(source), 0X0, fc, sz); // Create a directory entry
                        dir.entries.Add(d); // Add the directory entry
                        dir.WriteDirectory(); // Write the directory to disk
                    }
                    else
                    {
                        return; // Do not overwrite
                    }
                }
                else // If the source file does not exist in the destination directory
                {
                    int f = Program.current.entries[j].firs_cluster; // Get the first cluster
                    string content = null; // Initialize content
                    FILE file = new FILE(source, 0x0, f, dir, content, sz); // Create a FILE object
                    file.ReadFile(); // Read the file content
                    content = file.content; // Get the file content

                    FILE newFile = new FILE(source, 0X0, fc, dir, content, sz); // Create a new FILE object
                    newFile.writeFile(); // Write the file to disk

                    DirectoryEntry d = new DirectoryEntry(new string(source), 0X0, fc, sz); // Create a directory entry
                    dir.entries.Add(d); // Add the directory entry
                    dir.WriteDirectory(); // Write the directory to disk
                }
            }
            else
            {
                Console.WriteLine($"The File {source} Is Not Existed In your disk"); // Source file not found
            }
        }

        // Get a proper file name
        public static char[] getProperFileName(char[] fname, char[] extension)
        {
            char[] dir_name = new char[11]; // Initialize directory name
            int length = fname.Length, count = 0, lenOfEx = extension.Length; // Initialize counters
            if (fname.Length >= 7) // If the file name is 7 characters or more
            {
                for (int i = 0; i < 7; i++) // Add the first 7 characters
                {
                    dir_name[count] = fname[i];
                    count++;
                }
                dir_name[count] = '.'; // Add the dot
                count++;
            }
            else if (length < 7) // If the file name is less than 7 characters
            {
                for (int i = 0; i < length; i++) // Add the characters
                {
                    dir_name[count] = fname[i];
                    count++;
                }
                for (int i = 0; i < 7 - length; i++) // Add underscores
                {
                    dir_name[count] = '_';
                    count++;
                }
                dir_name[count] = '.'; // Add the dot
                count++;
            }
            for (int i = 0; i < lenOfEx; i++) // Add the extension
            {
                dir_name[count] = extension[i];
                count++;
            }
            for (int i = 0; i < 3 - lenOfEx; i++) // Add spaces if the extension is less than 3 characters
            {
                dir_name[count] = ' ';
                count++;
            }
            return dir_name; // Return the proper file name
        }

        // Get a proper(المناسب) directory name
        public static char[] getProperDirName(char[] name)
        {
            char[] dir_name = new char[11]; // Initialize directory name
            if (name.Length <= 11) // If the name is 11 characters or less
            {
                int j = 0;
                for (int i = 0; i < name.Length; i++) // Add the characters
                {
                    j++;
                    dir_name[i] = name[i];
                }
                for (int i = ++j; i < dir_name.Length; i++) // Add spaces
                    dir_name[i] = ' ';
            }
            else // If the name is more than 11 characters
            {
                int j = 0;
                for (int i = 0; i < 11; i++) // Add the first 11 characters
                {
                    j++;
                    dir_name[i] = name[i];
                }
            }
            return dir_name; // Return the proper directory name
        }
    }
}
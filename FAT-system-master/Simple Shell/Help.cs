using simple_Shell;

class Help
{
    // Command descriptions with brackets for better readability
    private string help_cd = "[cd] - Change the current default directory to . If the argument is not present, report the current directory. If the directory does not exist, an appropriate error should be reported.";
    private string help_dir = "[dir] - List the contents of the directory. If a directory path is provided, list the contents of that directory.\n";
    private string help_cls = "[cls] - Clear the shell content.\n";
    private string help_quit = "[quit] - Quit the shell.\n";
    private string help_copy = "[copy] - Copies one or more files to another location.\n";
    private string help_del = "[del] - Deletes one or more files.\n";
    private string help_help = "[help] - Provides Help information for commands.\n";
    private string help_md = "[md] - Creates a directory. Folder names cannot contain the following symbols: / \\ : * ? \" < > |\n";
    private string help_create = "[create] - Creates a new file. File names cannot contain the following symbols: / \\ : * ? \" < > |\n";
    private string help_rd = "[rd] - Removes a directory.\n";
    private string help_rename = "[rename] - Renames a file.\n";
    private string help_type = "[type] - Displays the contents of a text file.\n";
    private string help_import = "[import] - Import text file(s) from your computer.\n";
    private string help_export = "[export] - Export text file(s) to your computer.\n";
    private string help_tree = "[tree] - Displays the directory structure and content in a tree-like format.\n";
    private string help_clearphysical = "[clearphysical] - Clears the content of a physical file on your computer.\n"; // New command description
    private string help_command; // Combined help command string

    // Constructor with Token parameter
    public Help(Token token)
    {
        // Combine all help descriptions into one string
        help_command = help_cd + help_dir + help_cls + help_quit + help_copy + help_del + help_help + help_md + help_rd + help_rename + help_type + help_import + help_export + help_create + help_tree + help_clearphysical;
        // Call doHelp to display the appropriate help information
        doHelp(token);
    }

    // Default constructor
    public Help()
    {
        // Combine all help descriptions into one string
        help_command = help_cd + help_dir + help_cls + help_quit + help_copy + help_del + help_help + help_md + help_rd + help_rename + help_type + help_import + help_export + help_create + help_tree + help_clearphysical;
    }

    // Method to display help information
    public void doHelp(Token token)
    {
        if (token.value == null) // If no specific command is provided
        {
            Console.WriteLine(help_command); // Display all commands
        }
        else // If a specific command is provided
        {
            switch (token.value) // Display help for the specified command
            {
                case "cd":
                    Console.WriteLine(help_cd);
                    break;
                case "dir":
                    Console.WriteLine(help_dir);
                    break;
                case "cls":
                    Console.WriteLine(help_cls);
                    break;
                case "quit":
                    Console.WriteLine(help_quit);
                    break;
                case "copy":
                    Console.WriteLine(help_copy);
                    break;
                case "del":
                    Console.WriteLine(help_del);
                    break;
                case "help":
                    Console.WriteLine(help_help);
                    break;
                case "md":
                    Console.WriteLine(help_md);
                    break;
                case "rd":
                    Console.WriteLine(help_rd);
                    break;
                case "rename":
                    Console.WriteLine(help_rename);
                    break;
                case "type":
                    Console.WriteLine(help_type);
                    break;
                case "import":
                    Console.WriteLine(help_import);
                    break;
                case "export":
                    Console.WriteLine(help_export);
                    break;
                case "create":
                    Console.WriteLine(help_create);
                    break;
                case "tree":
                    Console.WriteLine(help_tree);
                    break;
                case "clearphysical": // New command
                    Console.WriteLine(help_clearphysical);
                    break;
                default:
                    Console.WriteLine($"{token.value} => This Command is not supported by the help utility.");
                    break;
            }
        }
    }
}
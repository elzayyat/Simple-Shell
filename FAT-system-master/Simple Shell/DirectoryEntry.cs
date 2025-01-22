namespace simple_Shell
{
    internal class DirectoryEntry
    {
        public char[] dir_name = new char[11]; // File or directory name (11 characters)
        public byte dir_attr; // Attribute (0x0 for file, 0x10 for directory)
        public byte[] dir_empty = new byte[12]; // Empty space (reserved)
        public int firs_cluster; // First cluster of the file or directory
        public int dir_fileSize; // File size

        // Constructor
        public DirectoryEntry(string name, byte attr, int fcluster, int fileSize)
        {
            dir_attr = attr; // Set attribute
            firs_cluster = fcluster; // Set first cluster
            this.dir_fileSize = fileSize; // Set file size

            if (attr == 0X0) // If it's a file
            {
                HandleFileName(name); // Handle file name
            }
            else if (attr == 0x10) // If it's a directory
            {
                HandleDirName(name.ToCharArray()); // Handle directory name
            }
        }

        // Handle file name
        public void HandleFileName(string name)
        {
            string[] fileNameParts = name.Split('.'); // Split the name into name and extension
            string fileName = fileNameParts[0]; // Get the file name
            string fileExtension = fileNameParts.Length > 1 ? fileNameParts[1] : ""; // Get the extension (if any)

            // Ensure the file name and extension fit within the 11-character limit
            char[] fileNameChars = fileName.ToCharArray();
            char[] fileExtensionChars = fileExtension.ToCharArray();
            char[] dirName = new char[11];

            int count = 0;
            for (int i = 0; i < Math.Min(fileNameChars.Length, 8); i++) // Copy up to 8 characters of the file name
            {
                dirName[count++] = fileNameChars[i];
            }
            dirName[count++] = '.'; // Add the dot
            for (int i = 0; i < Math.Min(fileExtensionChars.Length, 3); i++) // Copy up to 3 characters of the extension
            {
                dirName[count++] = fileExtensionChars[i];
            }
            for (int i = count; i < 11; i++) // Fill the remaining characters with spaces
            {
                dirName[i] = ' ';
            }

            dir_name = dirName; // Set the directory name
        }

        // Handle directory name
        public void HandleDirName(char[] name)
        {
            if (name.Length <= 11) // If the name is 11 characters or less
            {
                int j = 0;
                for (int i = 0; i < name.Length; i++) // Copy the characters
                {
                    j++;
                    dir_name[i] = name[i];
                }
                for (int i = j; i < dir_name.Length; i++) // Pad with spaces
                {
                    dir_name[i] = ' ';
                }
            }
            else // If the name is more than 11 characters
            {
                int j = 0;
                for (int i = 0; i < 11; i++) // Copy the first 11 characters
                {
                    j++;
                    dir_name[i] = name[i];
                }
            }
        }
    }
}
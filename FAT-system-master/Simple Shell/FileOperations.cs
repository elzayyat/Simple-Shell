namespace simple_Shell
{
    class FileOperations
    {
        // Create a new file
        public static void CreateFile(string name, Directory parent, string content = "")
        {
            int size = content.Length; // Get the size of the content
            int fc = size > 0 ? FAT.GetEmptyCulster() : 0; // Get an empty cluster if content exists
            FILE newFile = new FILE(name, 0x0, fc, parent, content, size); // Create a new FILE object
            newFile.writeFile(); // Write the file to disk

            DirectoryEntry d = new DirectoryEntry(name, 0x0, fc, size); // Create a directory entry
            parent.addEntry(d); // Add the entry to the parent directory
        }

        // Read a file
        public static void ReadFile(string name, Directory parent)
        {
            int index = parent.searchDirectory(name); // Search for the file in the directory
            if (index != -1) // If the file exists
            {
                DirectoryEntry entry = parent.entries[index]; // Get the directory entry
                FILE file = new FILE(name, 0x0, entry.firs_cluster, parent, "", entry.dir_fileSize); // Create a FILE object
                file.ReadFile(); // Read the file content
                Console.WriteLine(file.content); // Display the content
            }
            else
            {
                Console.WriteLine("File not found."); // File not found
            }
        }

        // Delete a file
        public static void DeleteFile(string name, Directory parent)
        {
            int index = parent.searchDirectory(name); // Search for the file in the directory
            if (index != -1) // If the file exists
            {
                DirectoryEntry entry = parent.entries[index]; // Get the directory entry
                FILE file = new FILE(name, 0x0, entry.firs_cluster, parent, "", entry.dir_fileSize); // Create a FILE object
                file.deleteFile(); // Delete the file
                parent.removeEntry(entry); // Remove the entry from the parent directory
            }
            else
            {
                Console.WriteLine("File not found."); // File not found
            }
        }

        // Write content to a file
        public static void WriteFile(string name, Directory parent, string content)
        {
            int index = parent.searchDirectory(name); // Search for the file in the directory
            if (index != -1) // If the file exists
            {
                DirectoryEntry entry = parent.entries[index]; // Get the directory entry
                FILE file = new FILE(name, 0x0, entry.firs_cluster, parent, content, content.Length); // Create a FILE object
                file.writeFile(); // Write the content to the file
                entry.dir_fileSize = content.Length; // Update the file size
                parent.updateContent(entry, entry); // Update the directory entry
            }
            else
            {
                Console.WriteLine("File not found."); // File not found
            }
        }

        // Clear the content of a physical file
        public static void ClearPhysicalFileContent(string filePath)
        {
            if (File.Exists(filePath)) // Check if the file exists
            {
                File.WriteAllText(filePath, string.Empty); // Clear the file content by writing an empty string
                Console.WriteLine($"File '{filePath}' content cleared successfully.");
            }
            else
            {
                Console.WriteLine($"File '{filePath}' does not exist."); // Error if the file does not exist
            }
        }
    }
}
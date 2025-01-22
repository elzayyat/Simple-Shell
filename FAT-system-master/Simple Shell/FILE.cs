namespace simple_Shell
{
    class FILE : DirectoryEntry
    {
        public Directory parent; // Parent directory
        public string content; // File content

        // Constructor
        public FILE(string name, byte attr, int fcluster, Directory parent, string content, int filesize)
            : base(name, attr, fcluster, filesize)
        {
            this.content = content; // Initialize content
            this.parent = parent; // Initialize parent directory
        }

        // Get the directory entry
        public DirectoryEntry GetDirectory_Entry()
        {
            return new DirectoryEntry(new string(this.dir_name), this.dir_attr, this.firs_cluster, this.dir_fileSize);
        }

        // Write file content to disk
        public void writeFile()
        {
            byte[] byteContent = ConvertContentToBytes(content); // Convert content to bytes
            List<byte[]> listOfArrayOfBytes = VirtualDisk.splitBytes(byteContent); // Split bytes into clusters

            int clusterFATIndex;
            if (this.firs_cluster != 0) // If the file already has a cluster
            {
                clusterFATIndex = this.firs_cluster;
            }
            else // If the file does not have a cluster
            {
                clusterFATIndex = FAT.GetEmptyCulster(); // Get an empty cluster
                this.firs_cluster = clusterFATIndex; // Set the first cluster
            }
            int lastCluster = -1;
            for (int i = 0; i < listOfArrayOfBytes.Count; i++) // Write each cluster
            {
                if (clusterFATIndex != -1)
                {
                    VirtualDisk.writeCluster(clusterFATIndex, listOfArrayOfBytes[i]); // Write the cluster
                    FAT.setClusterNext(clusterFATIndex, -1); // Mark the cluster as used
                    if (lastCluster != -1)
                        FAT.setClusterNext(lastCluster, clusterFATIndex); // Link the previous cluster to the current one
                    lastCluster = clusterFATIndex;
                    clusterFATIndex = FAT.GetEmptyCulster(); // Get the next empty cluster
                }
            }
        }

        // Read file content from disk
        public void ReadFile()
        {
            if (this.firs_cluster != 0) // If the file has a cluster
            {
                content = string.Empty; // Initialize content
                int cluster = this.firs_cluster; // Start from the first cluster
                int next = FAT.getClusterNext(cluster); // Get the next cluster
                List<byte> ls = new List<byte>(); // List to store file content
                do
                {
                    ls.AddRange(VirtualDisk.readCluster(cluster)); // Read the cluster
                    cluster = next; // Move to the next cluster
                    if (cluster != -1)
                        next = FAT.getClusterNext(cluster); // Get the next cluster
                }
                while (next != -1); // Continue until no more clusters
                content = ConvertBytesToContent(ls.ToArray()); // Convert bytes to content
            }
        }

        // Delete a file
        public void deleteFile()
        {
            if (this.firs_cluster != 0) // If the file has a cluster
            {
                clearFileSize(); // Clear the file size
            }
            if (this.parent != null) // If the file has a parent directory
            {
                string dirName = new string(dir_name); // Get the file name
                int index = this.parent.searchDirectory(dirName); // Search for the file in the directory
                if (index != -1) // If the file exists
                {
                    this.parent.entries.RemoveAt(index); // Remove the file from the directory
                    this.parent.WriteDirectory(); // Write the updated directory to disk
                    FAT.writeFat(); // Update the FAT
                }
            }
        }

        // Clear file size
        public void clearFileSize()
        {
            int clusterIndex = this.firs_cluster; // Start from the first cluster
            int next = FAT.getClusterNext(clusterIndex); // Get the next cluster
            if (clusterIndex == 5 && next == 0) // If the cluster is reserved
                return;
            do
            {
                FAT.setClusterNext(clusterIndex, 0); // Mark the cluster as free
                clusterIndex = next; // Move to the next cluster
                if (clusterIndex != -1)
                    next = FAT.getClusterNext(clusterIndex); // Get the next cluster
            } while (clusterIndex != -1); // Continue until no more clusters
        }

        // Convert content to bytes
        public static byte[] ConvertContentToBytes(string Con)
        {
            byte[] contentBytes = new byte[Con.Length]; // Initialize byte array
            for (int i = 0; i < Con.Length; i++)
            {
                contentBytes[i] = (byte)Con[i]; // Convert each character to a byte
            }
            return contentBytes;
        }

        // Convert bytes to content
        public static string ConvertBytesToContent(byte[] bytes)
        {
            string con = string.Empty; // Initialize content
            for (int i = 0; i < bytes.Length; i++)
            {
                if ((char)bytes[i] != '\0') // If the byte is not null
                    con += (char)bytes[i]; // Add the character to the content
                else
                    break; // Stop if null is encountered
            }
            return con;
        }
    }
}
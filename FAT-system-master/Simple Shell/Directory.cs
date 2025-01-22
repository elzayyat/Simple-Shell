namespace simple_Shell
{
    internal class Directory : DirectoryEntry
    {
        public Directory parent; // Parent directory
        public List<DirectoryEntry> entries; // List of directory entries

        // Constructor
        public Directory(string name, byte attr, int fcluster, Directory parent, int filesize = 0)
            : base(name, attr, fcluster, filesize)
        {
            entries = new List<DirectoryEntry>(); // Initialize entries
            this.parent = parent; // Set parent directory
        }

        // Create a directory entry from bytes
        public DirectoryEntry makeDirectory(byte[] b)
        {
            char[] name = new char[11]; // Initialize name
            for (int i = 0; i < name.Length; i++)
            {
                name[i] = (char)b[i]; // Convert bytes to characters
            }

            byte attr = b[11]; // Get attribute

            byte[] empty = new byte[12]; // Initialize empty space
            int j = 12;
            for (int i = 0; i < empty.Length; i++)
            {
                empty[i] = b[j]; // Copy empty space
                j++;
            }

            byte[] firstClusterByte = new byte[4]; // Initialize first cluster
            for (int i = 0; i < firstClusterByte.Length; i++)
            {
                firstClusterByte[i] = b[j]; // Copy first cluster
                j++;
            }
            int firstCluster = BitConverter.ToInt32(firstClusterByte, 0); // Convert bytes to integer

            byte[] sizeByte = new byte[4]; // Initialize file size
            for (int i = 0; i < sizeByte.Length; i++)
            {
                sizeByte[i] = b[j]; // Copy file size
                j++;
            }
            int fileSize = BitConverter.ToInt32(sizeByte, 0); // Convert bytes to integer

            DirectoryEntry newD = new DirectoryEntry(new string(name), attr, firstCluster, fileSize); // Create directory entry
            newD.dir_empty = empty; // Set empty space
            newD.dir_fileSize = fileSize; // Set file size

            return newD;
        }

        // Update directory content
        public void updateContent(DirectoryEntry d)
        {
            ReadDirectory(); // Read the directory
            string dName = new string(d.dir_name); // Get the directory name
            int index = searchDirectory(dName); // Search for the directory
            if (index != -1) // If the directory exists
            {
                entries.RemoveAt(index); // Remove the old entry
                entries.Insert(index, d); // Insert the new entry
            }
        }

        // Get the directory entry
        public DirectoryEntry getDirectoryEntry()
        {
            return new DirectoryEntry(new string(this.dir_name), this.dir_attr, this.firs_cluster, this.dir_fileSize);
        }

        // Search for a directory entry
        public int searchDirectory(string name)
        {
            ReadDirectory(); // Ensure the directory entries are up-to-date

            // Trim trailing spaces and underscores from the input name
            name = name.TrimEnd(' ', '_');

            for (int i = 0; i < entries.Count; i++) // Iterate through directory entries
            {
                // Trim trailing spaces and underscores from the entry name
                string entryName = new string(entries[i].dir_name).TrimEnd(' ', '_');

                if (entryName.Equals(name, StringComparison.OrdinalIgnoreCase)) // Compare names
                {
                    return i; // Return the index of the matching entry
                }
            }
            return -1; // File not found
        }
        // Add a directory entry
        public void addEntry(DirectoryEntry d)
        {
            entries.Add(d); // Add the entry
            WriteDirectory(); // Write the directory to disk
        }

        // Remove a directory entry
        public void removeEntry(DirectoryEntry d)
        {
            ReadDirectory(); // Read the directory
            string searchName = new string(d.dir_name); // Get the directory name
            int index = searchDirectory(searchName); // Search for the directory
            entries.RemoveAt(index); // Remove the entry
            WriteDirectory(); // Write the directory to disk
        }

        // Update directory content
        public void updateContent(DirectoryEntry old, DirectoryEntry neW)
        {
            ReadDirectory(); // Read the directory
            string oldName = new string(old.dir_name); // Get the old name
            int index = searchDirectory(oldName); // Search for the old entry
            if (index != -1) // If the old entry exists
            {
                entries.RemoveAt(index); // Remove the old entry
                entries.Add(neW); // Add the new entry
            }
        }

        // Read the directory from disk
        public void ReadDirectory()
        {
            if (this.firs_cluster != 0) // If the directory has a cluster
            {
                entries = new List<DirectoryEntry>(); // Initialize entries
                int cluster = this.firs_cluster; // Start from the first cluster
                int next = FAT.getClusterNext(cluster); // Get the next cluster
                List<byte> ls = new List<byte>(); // List to store directory content
                do
                {
                    ls.AddRange(VirtualDisk.readCluster(cluster)); // Read the cluster
                    cluster = next; // Move to the next cluster
                    if (cluster != -1)
                        next = FAT.getClusterNext(cluster); // Get the next cluster
                }
                while (next != -1); // Continue until no more clusters

                for (int i = 0; i < ls.Count; i++) // Process directory entries
                {
                    byte[] b = new byte[32]; // Initialize a directory entry
                    for (int k = i * 32, m = 0; m < b.Length && k < ls.Count; m++, k++)
                    {
                        b[m] = ls[k]; // Copy bytes
                    }
                    if (b[0] == 0) // If the entry is empty
                        break;

                    // Convert byte array to DirectoryEntry using makeDirectory
                    entries.Add(makeDirectory(b));
                }
            }
        }

        // Write the directory to disk
        public void WriteDirectory()
        {
            byte[] dirsorfilesBYTES = new byte[entries.Count * 32]; // Initialize byte array
            for (int i = 0; i < entries.Count; i++) // Convert entries to bytes
            {
                byte[] b = DirToByte(this.entries[i]); // Convert entry to bytes
                for (int j = i * 32, k = 0; k < b.Length; k++, j++)
                    dirsorfilesBYTES[j] = b[k]; // Copy bytes
            }
            List<byte[]> bytesls = VirtualDisk.splitBytes(dirsorfilesBYTES); // Split bytes into clusters
            int clusterFATIndex;
            if (this.firs_cluster != 0) // If the directory has a cluster
            {
                clusterFATIndex = this.firs_cluster; // Use the existing cluster
            }
            else // If the directory does not have a cluster
            {
                clusterFATIndex = FAT.GetEmptyCulster(); // Get an empty cluster
                this.firs_cluster = clusterFATIndex; // Set the first cluster
            }
            int lastCluster = -1;
            for (int i = 0; i < bytesls.Count; i++) // Write clusters
            {
                if (clusterFATIndex != -1)
                {
                    VirtualDisk.writeCluster(clusterFATIndex, bytesls[i]); // Write the cluster
                    FAT.setClusterNext(clusterFATIndex, -1); // Mark the cluster as used
                    if (lastCluster != -1)
                        FAT.setClusterNext(lastCluster, clusterFATIndex); // Link the previous cluster to the current one
                    lastCluster = clusterFATIndex;
                    clusterFATIndex = FAT.GetEmptyCulster(); // Get the next empty cluster
                }
            }
            if (entries.Count == 0) // If the directory is empty
            {
                if (firs_cluster != 0)
                    FAT.setClusterNext(firs_cluster, 0); // Mark the cluster as free
                firs_cluster = 0; // Reset the first cluster
            }
            if (this.parent != null) // If the directory has a parent
            {
                this.parent.updateContent(this.getDirectoryEntry()); // Update the parent directory
                this.parent.WriteDirectory(); // Write the parent directory to disk
            }
            FAT.writeFat(); // Write the FAT to disk
        }

        // Delete the directory
        public void deleteDirectory()
        {
            clearDirSize(); // Clear the directory size
            if (this.parent != null) // If the directory has a parent
            {
                int index = this.parent.searchDirectory(new string(this.dir_name)); // Search for the directory
                if (index != -1) // If the directory exists
                {
                    this.parent.ReadDirectory(); // Read the parent directory
                    this.parent.entries.RemoveAt(index); // Remove the directory
                    this.parent.WriteDirectory(); // Write the parent directory to disk
                }
            }
            if (Program.current == this) // If the current directory is being deleted
            {
                if (this.parent != null) // If the directory has a parent
                {
                    Program.current = this.parent; // Set the current directory to the parent
                    Program.currentPath = Program.currentPath.Substring(0, Program.currentPath.LastIndexOf('\\')); // Update the current path
                    Program.current.ReadDirectory(); // Read the current directory
                }
            }
            FAT.writeFat(); // Write the FAT to disk
        }

        // Clear the directory size
        public void clearDirSize()
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
            FAT.writeFat(); // Write the FAT to disk
        }

        // Get the directory size on disk
        public int getMySizeOnDisk()
        {
            int size = 0;
            int clusterIndex = this.firs_cluster; // Start from the first cluster
            int next = FAT.getClusterNext(clusterIndex); // Get the next cluster
            do
            {
                size++; // Increment size
                clusterIndex = next; // Move to the next cluster
                if (clusterIndex != -1)
                    next = FAT.getClusterNext(clusterIndex); // Get the next cluster
            } while (clusterIndex != -1); // Continue until no more clusters
            return size;
        }

        // Check if a directory entry can be added
        public bool canAddEntry(DirectoryEntry d)
        {
            int needeSize = (entries.Count + 1) * 32; // Calculate needed size
            int neededClusters = needeSize / 1024; // Calculate needed clusters
            int rem = needeSize % 1024; // Calculate remaining bytes
            if (rem > 0)
                neededClusters++; // Add an extra cluster if there are remaining bytes
            neededClusters += d.dir_fileSize / 1024; // Add clusters for file size
            int rem2 = d.dir_fileSize % 1024; // Calculate remaining bytes for file size
            if (rem2 > 0)
                neededClusters++; // Add an extra cluster if there are remaining bytes
            return getMySizeOnDisk() + FAT.getAvailableClusters() > neededClusters; // Check if there is enough space
        }

        // Convert a directory entry to bytes
        public static byte[] DirToByte(DirectoryEntry d)
        {
            byte[] bytes = new byte[32]; // Initialize byte array
            for (int i = 0; i < d.dir_name.Length; i++) // Copy name
            {
                bytes[i] = (byte)d.dir_name[i];
            }
            bytes[11] = d.dir_attr; // Copy attribute
            int j = 12;
            for (int i = 0; i < d.dir_empty.Length; i++) // Copy empty space
            {
                bytes[j] = d.dir_empty[i];
                j++;
            }
            byte[] firstClusterInBytes = BitConverter.GetBytes(d.firs_cluster); // Convert first cluster to bytes
            for (int i = 0; i < firstClusterInBytes.Length; i++)
            {
                bytes[j] = firstClusterInBytes[i];
                j++;
            }
            byte[] SizeInBytes = BitConverter.GetBytes(d.dir_fileSize); // Convert file size to bytes
            for (int i = 0; i < SizeInBytes.Length; i++)
            {
                bytes[j] = SizeInBytes[i];
                j++;
            }
            return bytes;
        }
    }
}
namespace simple_Shell
{
    internal class VirtualDisk          // Manages the virtual disk, including reading and writing clusters.
                                        //    5  methods (initialize(string path) ,, readCluster(int clusterIndex)  ,,
                                        // writeCluster(int clusterIndex, byte[] bytes) ,, getFreeSpace(): ,, splitBytes(byte[] bytes) 
                                        //
                                        //It is used by other classes (e.g., Directory, FAT) to store and retrieve data.
    {
        public static FileStream Disk; // File stream for the virtual disk

        // Initialize the virtual disk
        public static void initialize(string path)
        {
            if (!File.Exists(path)) // If the disk file does not exist
            {
                Disk = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite); // Create the disk file
                byte[] b = new byte[1024]; // Initialize a cluster (1024 bytes)
                for (int i = 0; i < b.Length; i++)
                    b[i] = 0; // Fill the cluster with zeros
                writeCluster(0, b); // Write the first cluster
                FAT.prepareFat(); // Prepare the FAT (File Allocation Table)
                Directory root = new Directory("M:", 0x10, 5, null); // Create the root directory
                root.WriteDirectory(); // Write the root directory to disk
                Program.current = root; // Set the current directory to the root
                FAT.writeFat(); // Write the FAT to disk
            }
            else // If the disk file exists
            {
                Disk = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite); // Open the disk file
                FAT.readFat(); // Read the FAT
                Directory root = new Directory("M:", 0x10, 5, null); // Create the root directory
                root.ReadDirectory(); // Read the root directory from disk
                Program.current = root; // Set the current directory to the root
            }
        }

        // Read a cluster from the disk
        public static byte[] readCluster(int clusterIndex)
        {
            Disk.Seek(clusterIndex * 1024, SeekOrigin.Begin); // Move to the specified cluster
            byte[] b = new byte[1024]; // Initialize a cluster
            Disk.Read(b, 0, 1024); // Read the cluster
            return b;
        }

        // Write a cluster to the disk
        public static void writeCluster(int clusterIndex, byte[] bytes)
        {
            Disk.Seek(clusterIndex * 1024, SeekOrigin.Begin); // Move to the specified cluster
            Disk.Write(bytes, 0, bytes.Length); // Write the cluster
            Disk.Flush(); // Flush the data to the disk
        }

        // Get the free space on the disk ,, Calculates and returns the amount of free space on the disk.
        public static int getFreeSpace()
        {
            return FAT.getAvailableClusters() * 1024; // Calculate free space in bytes
        }

        // Split data into clusters (1024 bytes each)
        public static List<byte[]> splitBytes(byte[] bytes)
        {
            List<byte[]> ls = new List<byte[]>(); // List to store clusters
            if (bytes.Length > 0) // If there is data to split
            {
                int number_of_arrays = bytes.Length / 1024; // Calculate the number of full clusters
                int rem = bytes.Length % 1024; // Calculate remaining bytes
                for (int i = 0; i < number_of_arrays; i++) // Split full clusters
                {
                    byte[] b = new byte[1024];
                    for (int j = i * 1024, k = 0; k < 1024; j++, k++)
                        b[k] = bytes[j];
                    ls.Add(b);
                }
                if (rem > 0) // Split remaining bytes
                {
                    byte[] b1 = new byte[1024];
                    for (int i = number_of_arrays * 1024, k = 0; k < rem; i++, k++)
                        b1[k] = bytes[i];
                    ls.Add(b1);
                }
            }
            else // If there is no data to split
            {
                byte[] b1 = new byte[1024]; // Create an empty cluster
                ls.Add(b1);
            }
            return ls;
        }
    }
}
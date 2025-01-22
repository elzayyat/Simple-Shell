namespace simple_Shell
{
    internal class FAT          // (File Allocation Table)
                                //  7 methodes ( prepareFat() 
    {
        static int[] fat = new int[1024]; // FAT table

        // Prepare the FAT
        public static void prepareFat()
        {
            for (int i = 0; i < fat.Length; i++) // Initialize the FAT
            {
                if (i == 0 || i == 4) // Mark reserved clusters
                    fat[i] = -1;
                else if (i > 0 && i <= 3) // Link the first few clusters
                    fat[i] = i + 1;
                else // Mark free clusters
                    fat[i] = 0;
            }
        }

        // Write the FAT to disk
        public static void writeFat()
        {
            byte[] bytes = new byte[fat.Length * sizeof(int)]; // Convert FAT to bytes
            Buffer.BlockCopy(fat, 0, bytes, 0, bytes.Length);
            List<byte[]> ls = VirtualDisk.splitBytes(bytes); // Split bytes into clusters
            for (int i = 0; i < ls.Count; i++)
            {
                Console.WriteLine($"Writing FAT cluster {i + 1}"); // Debug log
                VirtualDisk.writeCluster(i + 1, ls[i]); // Write clusters to disk
            }
        }

        // Read the FAT from disk
        public static void readFat()
        {
            byte[] buffer = new byte[4096]; // Initialize buffer
            List<byte> ls = new List<byte>(); // List to store FAT bytes
            for (int i = 0; i < 4; i++)
                ls.AddRange(VirtualDisk.readCluster(i + 1)); // Read FAT clusters
            byte[] bytes = ls.ToArray(); // Convert list to array
            int[] fatAsIntegers = new int[bytes.Length / sizeof(int)]; // Convert bytes to integers
            Buffer.BlockCopy(bytes, 0, fatAsIntegers, 0, bytes.Length);
            fat = fatAsIntegers; // Update the FAT
        }

        // Get an empty cluster
        public static int GetEmptyCulster()
        {
            for (int i = 0; i < fat.Length; i++) // Search for a free cluster
                if (fat[i] == 0)
                    return i;
            return -1; // No free clusters
        }

        // Get the next cluster
        public static int getClusterNext(int index)
        {
            if (index >= 0 && index < fat.Length) // Check if the index is valid
                return fat[index];
            return -1; // Invalid index
        }

        // Set the next cluster
        public static void setClusterNext(int index, int next)
        {
            fat[index] = next; // Update the FAT
        }

        // Get the number of available clusters
        public static int getAvailableClusters()
        {
            int count = 0;
            for (int i = 0; i < fat.Length; i++) // Count free clusters
                if (fat[i] == 0)
                    count++;
            return count;
        }
    }
}
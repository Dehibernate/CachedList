using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;


namespace PrimeSieve
{
    [Serializable()]
    public class CachedList<T>
    {
        public string folder;
        public long buffer;
        private int arrayCount;
        private int current;
        private T[] list;

        public CachedList(long max, long buffer, string folder)
        {
            this.buffer = buffer;
            this.folder = folder;
            arrayCount = ((max % buffer) == 0) ? (int)(max / buffer) : (int)(max / buffer + 1);

            current = 0;

            list = new T[buffer];

        }

        public int getList(long n)
        {
            if (n == 0) return 0;
            return ((n % buffer) == 0) ? (int)(n / buffer - 1) : (int)(n / buffer);

        }

        public T this[long i]
        {

            get
            {
                if (current != getList(i))
                {
                    save();
                    current = getList(i);
                    load(current);
                }

                long offset = i % buffer;
                //System.GC.Collect();
                return list[offset];
            }
            set
            {
                if (current != getList(i))
                {
                    save();
                    current = getList(i);
                    load(current);
                }

                long offset = i % buffer;
                list[offset] = value;
                //System.GC.Collect();
            }
        }

        public void close()
        {
            for (int i = 0; i <= arrayCount; i++)
            {
                File.Delete(String.Format("{0}data{1}.bin", folder, i));
            }

        }

        public void load(int arrayNum)
        {
            try
            {
                using (Stream stream = File.Open((String.Format("{0}data{1}.bin", folder, arrayNum)), FileMode.Open))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    list = (T[])bin.Deserialize(stream);
                }
            }
            catch (FileNotFoundException)
            {
                list = new T[buffer];
            }
        }

        public void save()
        {
            using (Stream stream = File.Open(String.Format("{0}data{1}.bin", folder, current), FileMode.Create))
            {
                BinaryFormatter bin = new BinaryFormatter();
                bin.Serialize(stream, list);
            }
        }
    }

}

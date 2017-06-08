using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabbit.Framework.Server.Data
{
    public interface IDataRepository
    {
        string Get(string key);
        void Create(string key, string value);
        void Delete(string key);
        void Update(string key, string value);
    }

    public class FileSystemDataRepository : IDataRepository
    {
        private object _lock = new object();
        private readonly string _fileLocation = null;

        public FileSystemDataRepository(string fileLocation)
        {
            if (string.IsNullOrWhiteSpace(fileLocation))
                throw new ArgumentNullException("fileLocation");

            _fileLocation = fileLocation;

            if (!File.Exists(_fileLocation))
            {
                try
                {
                    File.Create(_fileLocation);
                }
                catch(Exception ex)
                {
                    throw new ArgumentNullException("Unable to create file at location " + fileLocation, ex);
                }
            }
        }

        private Dictionary<string, string> Get()
        {
            string str = null;
            lock (_lock)
            {
                str = File.ReadAllText(_fileLocation);
            }

            if (string.IsNullOrWhiteSpace(str))
                return new Dictionary<string, string>();
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(str);
        }

        private void Save(Dictionary<string, string> items)
        {
            if (items == null)
                items = new Dictionary<string, string>();
            string str = JsonConvert.SerializeObject(items);

            lock (_lock)
            {
                File.WriteAllText(_fileLocation, str);
            }
        }

        public string Get(string key)
        {
            var items = Get();
            if (items != null && items.ContainsKey(key))
                return items[key];
            return null;
        }

        public void Create(string key, string value)
        {
            var items = Get();
            if (items == null)
                items = new Dictionary<string, string>();
            items.Add(key, value);
            Save(items);
        }

        public void Delete(string key)
        {
            var items = Get();
            if (items != null && items.ContainsKey(key))
            {
                items.Remove(key);
                Save(items);
            }
        }

        public void Update(string key, string value)
        {
            var items = Get();
            if (items != null && items.ContainsKey(key))
            {
                items.Remove(key);
                items.Add(key, value);
                Save(items);
            }
            else
                Create(key, value);
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabbit.Framework.Server.Data
{
    public interface ILoggingRepository
    {
        LogItem Create(LogItem item);
    }

    public class FileSystemLoggingRepository : ILoggingRepository
    {
        private object _lock = new object();
        private readonly string _fileLocation = null;

        public FileSystemLoggingRepository(string fileLocation)
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

        private List<LogItem> Get()
        {
            string str = null;
            lock (_lock)
            {
                str = File.ReadAllText(_fileLocation);
            }

            if (string.IsNullOrWhiteSpace(str))
                return new List<LogItem>();
            return JsonConvert.DeserializeObject<List<LogItem>>(str);
        }

        private void Save(List<LogItem> items)
        {
            if (items == null)
                items = new List<LogItem>();
            string str = JsonConvert.SerializeObject(items);

            lock (_lock)
            {
                File.WriteAllText(_fileLocation, str);
            }
        }

        public LogItem Create(LogItem item)
        {
            var items = Get();
            if (items == null)
                items = new List<LogItem>();
            item.Id = Guid.NewGuid().ToString("N");
            items.Add(item);
            Save(items);
            return item;
        }
    }

    public class LogItem
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public LogItemPriority Priority { get; set; }
    }

    public enum LogItemPriority
    {
        Info,
        Debug,
        Warning,
        Error,
        Critical
    }
}

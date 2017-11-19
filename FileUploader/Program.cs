using System;

namespace FileUploader
{
    public class Program
    {      
        static void Main(string[] args)
        {
            var serviceHost = new ServiceHost();
            serviceHost.Run().Wait();
        }
    }
}

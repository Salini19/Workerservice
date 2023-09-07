using Microsoft.Extensions.Configuration;

namespace Workerservice
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly TimeSpan _period = TimeSpan.FromMinutes(2);
        public Worker(ILogger<Worker> logger,IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            using PeriodicTimer timer = new PeriodicTimer(_period);
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {

                try
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    string fileLocation = _configuration.GetValue<string>("FilePath");
                    string fileName = _configuration.GetValue<string>("FileName");
                    CreateFile(fileLocation, fileName);
                    //await Task.Delay(2000);
                    //DeleteFile(fileLocation);
                    //await Task.Delay(1000, stoppingToken);
                }
                catch (Exception ex)
                {

                    _logger.LogInformation(
                     $"Failed to execute PeriodicHostedService with exception message {ex.Message}. Good luck next round!");
                }
               


            }

        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service Started..");
            

            return base.StartAsync(cancellationToken);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {

           
            _logger.LogInformation("Service Stopped..");

            return base.StopAsync(cancellationToken);
        }

        private void CreateFile(string fileLocation, string fileName)
        {
            var file = fileLocation + "\\" + fileName;
            if (!File.Exists(file))
            {
                for (int i = 1; i <= 5; i++)
                {
                    var fName = fileLocation + "\\" + "File_" + i + ".txt";
                    File.Create(fName).Dispose();
                    _logger.LogInformation("File created :{fName}", fName);
                    File.AppendAllText(fName, "File Created _"+" "+i);
                    
                }

                var log = "File created in " + System.Environment.MachineName + " at " + file;
               
            }
        }


        private void DeleteFile(string fileLocation)
        {
            try
            {
                string[] files = Directory.GetFiles(fileLocation);
                foreach (var file in files)
                {
                    File.Delete(file);
                    _logger.LogInformation("Deleted the files {file} at {time}", file, DateTime.Now);
                }

            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
        }
    }
}
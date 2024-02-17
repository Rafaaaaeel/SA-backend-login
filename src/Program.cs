IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .UseContentRoot(Directory.GetCurrentDirectory())
    .ConfigureWebHostDefaults(webBuilder => 
    {
        webBuilder.UseStartup<Startup>();
    });

await builder.Build().RunAsync();

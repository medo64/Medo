using Medo.Configuration;

RecentFiles.MaximumCount = 3;

RecentFiles.Add(new FileInfo("test1.txt"));
RecentFiles.Add(new FileInfo("test2.txt"));
RecentFiles.Add(new FileInfo("test3.txt"));
RecentFiles.Add(new FileInfo("test4.txt"));
RecentFiles.Add(new FileInfo("test5.txt"));

RecentFiles.Add(new FileInfo("test3.txt"));
RecentFiles.Add(new FileInfo("test1.txt"));

foreach (var file in RecentFiles.GetFiles()) {
    Console.WriteLine(@$"'{file}'");
}
Console.ReadLine();

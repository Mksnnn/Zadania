using System.IO;

string path;
string newPath;
string s;

path = Console.ReadLine();
path.Replace('\\', '/');

if(File.Exists(path))
{
    s = File.ReadAllText(path);
    s = s.Replace("praca", "job");
    newPath = path.Substring(0, path.Length - 4);
    newPath += "_changed-" + DateTime.Today.ToString("dd MM yyyy") + ".txt";
    File.WriteAllText(newPath, s);
}else
{
    Console.WriteLine("Plik nie istnieje, spróbuj ponownie.");
    path = Console.ReadLine();
    path.Replace('\\', '/');
}
    


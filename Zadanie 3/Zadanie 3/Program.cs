using System.IO;

string path = "C:/test/users-" + DateTime.Now.ToString("HH_mm_dd_MM_yyyy")+".txt";

string[] imiona = {"Ania", "Kasia", "Basia", "Zosia"};
string[] nazwiska = {"Kowalska", "Nowak"};

Random r = new Random();

File.WriteAllText(path, "LP,Imie,Nazwisko,Rok urodzenia\n");
for(int i=0;i<100;i++)
{
    string s = i.ToString() + ",";
    s += imiona[r.Next(imiona.Length)] + ",";
    s += nazwiska[r.Next(nazwiska.Length)] + ",";
    s += r.Next(1990, 2000).ToString() + "\n";
    File.AppendAllText(path, s);
}
using System.IO;

string path = "C:/test/test_inf_tec.txt";

string s;
int n = 0;

try 
{
    s = File.ReadAllText(path);
}
catch (Exception e)
{
    Console.WriteLine(e.ToString());
    return;
}

for(int i=0;i<s.Length;i++)
{
    if (s[i] == 'a')
    {
        n++;
    }
}
Console.WriteLine("Ilość \"a\" w tekście: " + n);
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

Console.Write("Podaj PLN: ");

double pln = double.Parse(Console.ReadLine());

var client = new HttpClient();
var response = await client.GetStringAsync("https://api.nbp.pl/api/exchangerates/rates/a/usd");

using JsonDocument jdoc = JsonDocument.Parse(response);
JsonElement root = jdoc.RootElement;

double kurs = root.GetProperty("rates")[0].GetProperty("mid").GetDouble();
Console.Write(Math.Round(pln,2).ToString()+"zł = $"+Math.Round((pln/kurs),2).ToString());
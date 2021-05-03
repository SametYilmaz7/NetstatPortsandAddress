using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;


namespace vize_odevi
{
    public class Veriler
    {
        public string port_numarasi { get; set; }
        public string foreign_adresi { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            using (Process p = new Process())
            {
                ProcessStartInfo ps = new ProcessStartInfo();
                ps.Arguments = "-a -n";
                ps.FileName = "netstat.exe";
                ps.UseShellExecute = false;
                ps.WindowStyle = ProcessWindowStyle.Hidden;
                ps.RedirectStandardInput = true;
                ps.RedirectStandardOutput = true;
                ps.RedirectStandardError = true;

                p.StartInfo = ps;
                p.Start();

                StreamReader stdOutput = p.StandardOutput;
                StreamReader stdError = p.StandardError;

                string content = stdOutput.ReadToEnd() + stdError.ReadToEnd();
                string exitStatus = p.ExitCode.ToString();

                string[] rows = Regex.Split(content, "\r\n");
               
                foreach (var item in rows)
                {

                    if (item == "") continue;
                    if (item.Trim() == "Active Connections") continue;
                    if (item == "UDP") continue;
                    if (item.Contains("LISTENING")) continue;
                    if (item.Contains("127.0.0.1")) continue;
                    if (item.Contains("TCP"))
                    {
                        // Verileri bölme işlemleri

                        string[] portu_parcala = item.Split(new char[] { ' ', ':' });
                        Veriler veri = new Veriler();
                        veri.foreign_adresi = portu_parcala[12];
                        string[] adres_parcala = item.Split(new char[] { ' ', ':' });
                        veri.port_numarasi = adres_parcala[7];
                        
                        // Parçaladığımız verileri json dosyasına yazdırma işlemi

                        string Json_yaz = JsonConvert.SerializeObject(veri);                      
                        TextWriter yazdir = new StreamWriter(@".\json_verileri.json", true); //"." olan yere dosyanın oluşacağı yeri giriniz.
                        yazdir.WriteLine(Json_yaz);
                        yazdir.Close();                       
                    }
                }
                Console.WriteLine("Dosya olusturuldu.");
            }
        }
    }
}
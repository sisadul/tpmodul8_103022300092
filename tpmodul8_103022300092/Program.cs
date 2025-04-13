using System.Reflection.PortableExecutable;
using System.Text.Json;
using System.Text.Json.Serialization;

class CovidConfig
{
    public string filePath = Path.Combine(Directory.GetCurrentDirectory(), "covid_config.json");


    [JsonPropertyName("satuan_Suhu")]
    public string Suhu { get; set; }
    [JsonPropertyName("bata_hari_demam")]
    public int batasHarianDemam { get; set; }
    [JsonPropertyName("pesan_ditolak")]
    public string PesanDitolak { get; set; }
    [JsonPropertyName("pesan_diterima")]
    public string PesanDiterima { get; set; }
    public CovidConfig()
    {
        setDefault();
    }
    public void LoadConfig()
    {
        if (File.Exists(filePath))
        {
            try
            {
                string data = File.ReadAllText(filePath);
                var config = JsonSerializer.Deserialize<CovidConfig>(data);
                Suhu = config.Suhu;
                batasHarianDemam = config.batasHarianDemam;
                PesanDiterima = config.PesanDiterima;
                PesanDitolak = config.PesanDitolak;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error!!! : " + ex.Message);
            }
        }
        else
        {
            SaveConfig();
            Console.WriteLine("File tidak ditemukan, file akan dimasukan ke dalam file baru");
        }
    }
    public void SaveConfig()
    {
        try
        {
            string data = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, data);
            Console.WriteLine("Configurasi berhasil disimpan");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Gagal menyimpan configurasi : {ex.Message}");
        }
    }
    public void UbahSatuan()
    {
        if (Suhu != "celcius")
        {
            Suhu = "celcius";
        }
        else 
        {
            Suhu = "fahrenheit";
        }
        SaveConfig();
    }
    public string GetMessage(string status)
    {
        return status == "ditolak" ? PesanDitolak : PesanDiterima;
    }
    public void setDefault()
    {
        this.Suhu = "celcius";
        this.batasHarianDemam = 14;
        this.PesanDiterima = "Anda dipersilahkan untuk masuk ke dalam gedung ini";
        this.PesanDitolak = "Anda tidak diperbolehkan masuk ke dalam gedung ini";
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine(File.ReadAllText("covid_config.json")); // debug
        var config = new CovidConfig();
        config.LoadConfig();

        Console.WriteLine("Apakah Anda ingin mengubah satuan suhu? (y/n): ");
        string input = Console.ReadLine();
        if (input.ToLower() == "y")
        {
            config.UbahSatuan();
            Console.WriteLine($"Satuan suhu telah diubah menjadi {config.Suhu}.");
        }

        Console.WriteLine($"Satuan suhu saat ini adalah {config.Suhu}.");
        Console.WriteLine();
        Console.Write($"Berapa suhu badan anda saat ini? ({config.Suhu}): ");
        double suhu = Convert.ToDouble(Console.ReadLine());

        Console.Write("Berapa hari yang lalu anda terakhir mengalami gejala demam? ");
        int hariDemam = Convert.ToInt32(Console.ReadLine());

        // Validasi suhu berdasarkan satuan
        if (config.Suhu.ToLower() == "celcius")
        {
            if (suhu < 36.5 || suhu > 37.5)
            {
                Console.WriteLine(config.GetMessage("ditolak"));
                return;
            }
        }
        else if (config.Suhu.ToLower() == "fahrenheit")
        {
            if (suhu < 97.7 || suhu > 99.5)
            {
                Console.WriteLine(config.GetMessage("ditolak"));
                return;
            }
        }
        else
        {
            Console.WriteLine("Satuan suhu tidak dikenali.");
            return;
        }

        if (hariDemam >= config.batasHarianDemam)
        {
            Console.WriteLine(config.GetMessage("ditolak"));
        }
        else
        {
            Console.WriteLine(config.GetMessage("diterima"));
        }


    }
}

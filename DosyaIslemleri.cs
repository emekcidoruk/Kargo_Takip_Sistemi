using Kargo_Takip_Sistemi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using static Kargo_Takip_Sistemi.Enums;

public static class DosyaIslemleri
{
    public static void GonderiKaydet(Gonderi gonderi, string dosyaYolu)
    {
        string satir = "";

        if (gonderi is YurticiGonderi yg)
        {
            satir = $"{yg.TakipNo},{yg.Gonderici},{yg.Alici},{yg.Durum},YurticiGonderi,{yg.Il},{yg.Ilce}";
        }
        else if (gonderi is YurtdisiGonderi yd)
        {
            satir = $"{yd.TakipNo},{yd.Gonderici},{yd.Alici},{yd.Durum},YurtdisiGonderi,{yd.Ulke},{yd.GonderiUcreti}";
        }

        File.AppendAllText(dosyaYolu, satir + Environment.NewLine);
    }

    public static void GonderiGuncelle(List<Gonderi> gonderiler, string dosyaYolu)
    {
        var satirlar = new List<string>();
        foreach (var g in gonderiler)
        {
            if (g is YurticiGonderi yg)
            {
                satirlar.Add($"{yg.TakipNo},{yg.Gonderici},{yg.Alici},{yg.Durum},Yurtici,{yg.Il},{yg.Ilce},{yg.GonderiUcreti}");
            }
            else if (g is YurtdisiGonderi yd)
            {
                satirlar.Add($"{yd.TakipNo},{yd.Gonderici},{yd.Alici},{yd.Durum},Yurtdisi,{yd.Ulke},{yd.GonderiUcreti}");
            }
        }
        File.WriteAllLines(dosyaYolu, satirlar);
    }

    public static void GonderileriDosyayaYaz(List<Gonderi> liste, string dosyaYolu)
    {
        var satirlar = new List<string>();

        foreach (var gonderi in liste)
        {
            if (gonderi is YurticiGonderi yg)
            {
                satirlar.Add($"{yg.TakipNo},{yg.Gonderici},{yg.Alici},{yg.Durum},Yurtici,{yg.Il},{yg.Ilce},{yg.GonderiUcreti}");
            }
            else if (gonderi is YurtdisiGonderi yd)
            {
                satirlar.Add($"{yd.TakipNo},{yd.Gonderici},{yd.Alici},{yd.Durum},Yurtdisi,{yd.Ulke},{yd.GonderiUcreti}");
            }
        }

        File.WriteAllLines(dosyaYolu, satirlar);
    }

    public static List<Gonderi> GonderileriOku(string dosyaYolu)
    {
        List<Gonderi> liste = new List<Gonderi>(); // Gönderi listesini başlat

        if (!File.Exists(dosyaYolu))
        {
            MessageBox.Show("Dosya bulunamadı: " + dosyaYolu);
            return liste;
        }

        string[] satirlar = File.ReadAllLines(dosyaYolu);

        foreach (string satir in satirlar)
        {
            string[] p = satir.Split(',');

            if (p.Length < 5) // En az 5 eleman olmalı (TakipNo, Gonderici, Alici, Durum, GonderiTipi)
                continue;

            // Durum çevir
            GonderiDurumu durum;
            if (p[3] == "Bekliyor")
                durum = GonderiDurumu.Bekliyor;
            else if (p[3] == "Yolda")
                durum = GonderiDurumu.Yolda;
            else if (p[3] == "TeslimEdildi")
                durum = GonderiDurumu.TeslimEdildi;
            else
                continue;

            // Gönderi tipi kontrol
            if (p[4] == "YurticiGonderi" && p.Length >= 7) // Yurtiçi gönderi kontrolü
            {
                liste.Add(new YurticiGonderi
                {
                    TakipNo = p[0],
                    Gonderici = p[1],
                    Alici = p[2],
                    Durum = durum,
                    Il = p[5],
                    Ilce = p[6]
                });
            }
            else if (p[4] == "YurtdisiGonderi") // Yurtdışı gönderi kontrolü
            {
                liste.Add(new YurtdisiGonderi
                {
                    TakipNo = p[0],
                    Gonderici = p[1],
                    Alici = p[2],
                    Durum = durum
                    // Diğer alanlar eklenebilir
                });
            }
        }



        return liste;

    }
}


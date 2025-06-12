using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Kargo_Takip_Sistemi.Enums;

namespace Kargo_Takip_Sistemi
{
    public partial class Form1 : Form
    {
        List<Gonderi> gonderiler = new List<Gonderi>();
        string dosyaYolu = @"C:\Kargo\Kargo.txt";

        public Form1()
        {
            InitializeComponent();
            cmbDurum.Items.AddRange(Enum.GetNames(typeof(GonderiDurumu))); // Durum enum değerlerini combobox'a ekliyoruz
            cmbGonderiTipi.Items.AddRange(new string[] { "YurticiGonderi", "YurtdisiGonderi" }); // Gönderi tiplerini combobox'a ekliyoruz

            if (!Directory.Exists("Data")) Directory.CreateDirectory("Data"); // Data klasörü yoksa oluşturuyoruz
            if (!File.Exists(dosyaYolu)) File.Create(dosyaYolu).Close(); // Dosya yoksa oluşturuyoruz

            gonderiler = DosyaIslemleri.GonderileriOku(dosyaYolu); // Dosyadan gönderileri okuyoruz
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            string takip = txtTakipNo.Text.Trim();
            string gonderici = txtGonderici.Text.Trim();
            string alici = txtAlici.Text.Trim();
            string durumStr = cmbDurum.SelectedItem?.ToString();
            string tip = cmbGonderiTipi.SelectedItem?.ToString();
            string ek1 = txtEk1.Text.Trim();
            string ek2 = txtEk2.Text.Trim();

            if (string.IsNullOrEmpty(takip) || string.IsNullOrEmpty(gonderici) || string.IsNullOrEmpty(alici) ||
                string.IsNullOrEmpty(durumStr) || string.IsNullOrEmpty(tip) || string.IsNullOrEmpty(ek1) || string.IsNullOrEmpty(ek2))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.");
                return;
            }

            GonderiDurumu durum = EnumCoz(durumStr); // Durum enum değerini alıyoruz
            Gonderi gonderi = null;

            if (tip == "YurticiGonderi")
            {
                gonderi = new YurticiGonderi
                {
                    TakipNo = takip,
                    Gonderici = gonderici,
                    Alici = alici,
                    Durum = durum,
                    Il = ek1,
                    Ilce = ek2
                };
            }
            else if (tip == "YurtdisiGonderi")
            {
                gonderi = new YurtdisiGonderi
                {
                    TakipNo = takip,
                    Gonderici = gonderici,
                    Alici = alici,
                    Durum = durum,
                    Ulke = ek1,
                    GonderiUcreti = double.TryParse(ek2, out double ucret) ? ucret : 0 // Ek2'yi ücret olarak alıyoruz, eğer parse edilemezse 0 olarak ayarlıyoruz
                };
            }

            gonderiler.Add(gonderi); // Gönderiyi listeye ekliyoruz
            DosyaIslemleri.GonderiKaydet(gonderi, dosyaYolu); // Gönderiyi dosyaya kaydediyoruz
            MessageBox.Show("Gönderi eklendi."); // Kullanıcıya bilgi veriyoruz
            Listele(); // Listeyi güncelliyoruz
        }

        private void btnAra_Click(object sender, EventArgs e)
        {
            string takip = txtTakipNo.Text.Trim(); // Takip numarasını alıyoruz
            var bulunan = gonderiler.Find(g => g.TakipNo == takip); // Liste içinde takip numarasına göre arama yapıyoruz
            if (bulunan != null) // Eğer gönderi bulunduysa
            {
                MessageBox.Show($"Gönderi: {bulunan.Gonderici} → {bulunan.Alici}, Durum: {bulunan.Durum}"); // Gönderi bilgilerini gösteriyoruz
            }
            else // Eğer gönderi bulunamadıysa
            {
                MessageBox.Show("Gönderi bulunamadı.");
            }
        }

        private void btnListele_Click(object sender, EventArgs e)
        {
            Listele();
        }

        private void Listele()
        {
            lstGonderiler.Items.Clear(); // Liste kutusunu temizliyoruz
            foreach (var g in gonderiler) // Gönderi listesini dolaşıyoruz
            {
                lstGonderiler.Items.Add($"{g.TakipNo} - {g.Gonderici} → {g.Alici} [{g.Durum}]"); // Her gönderiyi liste kutusuna ekliyoruz
            }
        }

        private GonderiDurumu EnumCoz(string deger)
        {
            if (deger == "Bekliyor") return GonderiDurumu.Bekliyor; // Bekliyor durumu
            if (deger == "Yolda") return GonderiDurumu.Yolda; // Yolda durumu
            if (deger == "TeslimEdildi") return GonderiDurumu.TeslimEdildi; // Teslim Edildi durumu
            return GonderiDurumu.Bekliyor; // Varsayılan olarak Bekliyor durumu döndürüyoruz
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (lstGonderiler.SelectedIndex == -1) // Eğer liste kutusunda hiçbir gönderi seçilmemişse
            {
                MessageBox.Show("Lütfen güncellenecek gönderiyi seçin."); // Kullanıcıya bilgi veriyoruz
                return;
            }

            var seciliGonderi = gonderiler[lstGonderiler.SelectedIndex]; // Seçilen gönderiyi alıyoruz

            if (cmbDurum.SelectedItem == null) // Eğer durum combobox'ında hiçbir şey seçilmemişse
            {
                MessageBox.Show("Lütfen geçerli bir durum seçin.");
                return;
            }

            string secilenDurum = cmbDurum.SelectedItem.ToString(); // Seçilen durumu alıyoruz

            // EnumCoz metodunu kullanarak enum değerini alıyoruz
            GonderiDurumu yeniDurum = EnumCoz(secilenDurum);

            seciliGonderi.DurumGuncelle(yeniDurum);

            DosyaIslemleri.GonderiGuncelle(gonderiler, dosyaYolu); // Güncellenen gönderileri dosyaya yazıyoruz

            Listele();

            MessageBox.Show("Gönderi durumu güncellendi.");
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (lstGonderiler.SelectedIndex == -1) // Eğer liste kutusunda hiçbir gönderi seçilmemişse
            {
            MessageBox.Show("Lütfen silinecek gönderiyi seçin.");
            return;
            }

            var seciliGonderi = gonderiler[lstGonderiler.SelectedIndex]; // Seçilen gönderiyi alıyoruz

            var onay = MessageBox.Show($"{seciliGonderi.TakipNo} takip numaralı gönderiyi silmek istediğinize emin misiniz?", 
                                "Onay", MessageBoxButtons.YesNo);
            if (onay == DialogResult.Yes) // Eğer kullanıcı onay verirse
            {
                gonderiler.RemoveAt(lstGonderiler.SelectedIndex); // Seçilen gönderiyi listeden siliyoruz
                DosyaIslemleri.GonderileriDosyayaYaz(gonderiler, dosyaYolu); // Güncellenen gönderi listesini dosyaya yazıyoruz
                Listele(); // Listeyi güncelliyoruz
                MessageBox.Show("Gönderi silindi.");
            }
        }
    }
}

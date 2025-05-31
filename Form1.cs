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
            cmbDurum.Items.AddRange(Enum.GetNames(typeof(GonderiDurumu)));
            cmbGonderiTipi.Items.AddRange(new string[] { "YurticiGonderi", "YurtdisiGonderi" });

            if (!Directory.Exists("Data")) Directory.CreateDirectory("Data");
            if (!File.Exists(dosyaYolu)) File.Create(dosyaYolu).Close();

            gonderiler = DosyaIslemleri.GonderileriOku(dosyaYolu);
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

            GonderiDurumu durum = EnumCoz(durumStr);
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
                    GonderiUcreti = double.TryParse(ek2, out double ucret) ? ucret : 0
                };
            }

            gonderiler.Add(gonderi);
            DosyaIslemleri.GonderiKaydet(gonderi, dosyaYolu);
            MessageBox.Show("Gönderi eklendi.");
            Listele();
        }

        private void btnAra_Click(object sender, EventArgs e)
        {
            string takip = txtTakipNo.Text.Trim();
            var bulunan = gonderiler.Find(g => g.TakipNo == takip);
            if (bulunan != null)
            {
                MessageBox.Show($"Gönderi: {bulunan.Gonderici} → {bulunan.Alici}, Durum: {bulunan.Durum}");
            }
            else
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
            lstGonderiler.Items.Clear();
            foreach (var g in gonderiler)
            {
                lstGonderiler.Items.Add($"{g.TakipNo} - {g.Gonderici} → {g.Alici} [{g.Durum}]");
            }
        }

        private GonderiDurumu EnumCoz(string deger)
        {
            if (deger == "Bekliyor") return GonderiDurumu.Bekliyor;
            if (deger == "Yolda") return GonderiDurumu.Yolda;
            if (deger == "TeslimEdildi") return GonderiDurumu.TeslimEdildi;
            return GonderiDurumu.Bekliyor;
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (lstGonderiler.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen güncellenecek gönderiyi seçin.");
                return;
            }

            var seciliGonderi = gonderiler[lstGonderiler.SelectedIndex];

            if (cmbDurum.SelectedItem == null)
            {
                MessageBox.Show("Lütfen geçerli bir durum seçin.");
                return;
            }

            string secilenDurum = cmbDurum.SelectedItem.ToString();

            // EnumCoz metodunu kullanarak enum değerini alıyoruz
            GonderiDurumu yeniDurum = EnumCoz(secilenDurum);

            seciliGonderi.DurumGuncelle(yeniDurum);

            DosyaIslemleri.GonderiGuncelle(gonderiler, dosyaYolu);

            Listele();

            MessageBox.Show("Gönderi durumu güncellendi.");
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (lstGonderiler.SelectedIndex == -1)
    {
        MessageBox.Show("Lütfen silinecek gönderiyi seçin.");
        return;
    }

    var seciliGonderi = gonderiler[lstGonderiler.SelectedIndex];

    var onay = MessageBox.Show($"{seciliGonderi.TakipNo} takip numaralı gönderiyi silmek istediğinize emin misiniz?", 
                                "Onay", MessageBoxButtons.YesNo);
    if (onay == DialogResult.Yes)
    {
        gonderiler.RemoveAt(lstGonderiler.SelectedIndex);
        DosyaIslemleri.GonderileriDosyayaYaz(gonderiler, dosyaYolu);
        Listele();
        MessageBox.Show("Gönderi silindi.");
    }
        }
    }
}

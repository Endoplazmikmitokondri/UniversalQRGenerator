using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using QRCoder;

namespace UniversalQRGenerator
{
    public partial class Form1 : Form
    {
        // "null!" kullanarak derleyiciye bu değerlerin InitializeUI içinde atanacağını garanti ediyoruz.
        private ComboBox cmbDataType = null!;
        private TextBox txtInput = null!;
        private Label lblHint = null!;
        private PictureBox picQR = null!;
        private Button btnGenerate = null!, btnSave = null!, btnLoadLogo = null!, btnClearLogo = null!;
        private NumericUpDown numLogoSize = null!;
        private Bitmap? selectedLogo = null; // Soru işareti (?) null olabileceğini belirtir
        private Label lblLogoWarning = null!, lblColorWarning = null!;

        // Renk Seçimi
        private Color qrColor = Color.Black;
        private Color bgColor = Color.White;
        private Panel pnlQrColor = null!, pnlBgColor = null!;

        public Form1()
        {
            InitializeUI();
            this.Text = "Universal QR Code Generator - Open Source";
            this.Size = new Size(1000, 780);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void InitializeUI()
        {
            // --- SOL PANEL (GİRİŞ VE AYARLAR) ---
            Panel pnlLeft = new Panel { Dock = DockStyle.Left, Width = 450, Padding = new Padding(15), BackColor = Color.WhiteSmoke };

            GroupBox grpData = new GroupBox { Text = "1. QR Content & Data", Dock = DockStyle.Top, Height = 220, Font = new Font("Segoe UI", 10, FontStyle.Bold), Padding = new Padding(10) };
            
            cmbDataType = new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList, Height = 30, Font = new Font("Segoe UI", 10) };
            cmbDataType.Items.AddRange(new string[] { "Plain Text / URL", "Wi-Fi Network", "VCard (Contact)", "WhatsApp Message" });
            cmbDataType.SelectedIndexChanged += CmbDataType_SelectedIndexChanged;
            
            lblHint = new Label { Dock = DockStyle.Top, Height = 40, Font = new Font("Segoe UI", 8, FontStyle.Regular), ForeColor = Color.DimGray, Padding = new Padding(0, 5, 0, 0) };
            txtInput = new TextBox { Dock = DockStyle.Fill, Multiline = true, ScrollBars = ScrollBars.Vertical, Font = new Font("Segoe UI", 10, FontStyle.Regular) };

            grpData.Controls.Add(txtInput);
            grpData.Controls.Add(lblHint);
            grpData.Controls.Add(cmbDataType);
            cmbDataType.SelectedIndex = 0;

            Panel spacer1 = new Panel { Dock = DockStyle.Top, Height = 15 };

            GroupBox grpAppearance = new GroupBox { Text = "2. Appearance & Logo", Dock = DockStyle.Top, Height = 230, Font = new Font("Segoe UI", 10, FontStyle.Bold), Padding = new Padding(10) };

            lblColorWarning = new Label { Dock = DockStyle.Top, Height = 20, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = Color.Red, Text = "", TextAlign = ContentAlignment.TopCenter };

            FlowLayoutPanel flpColors = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40 };
            pnlQrColor = new Panel { Width = 30, Height = 30, BackColor = qrColor, BorderStyle = BorderStyle.FixedSingle, Cursor = Cursors.Hand };
            pnlQrColor.Click += (s, e) => { qrColor = PickColor(qrColor); pnlQrColor.BackColor = qrColor; CheckContrast(); };
            pnlBgColor = new Panel { Width = 30, Height = 30, BackColor = bgColor, BorderStyle = BorderStyle.FixedSingle, Cursor = Cursors.Hand };
            pnlBgColor.Click += (s, e) => { bgColor = PickColor(bgColor); pnlBgColor.BackColor = bgColor; CheckContrast(); };

            flpColors.Controls.Add(new Label { Text = "QR Color:", AutoSize = true, Margin = new Padding(0, 8, 5, 0), Font = new Font("Segoe UI", 9) });
            flpColors.Controls.Add(pnlQrColor);
            flpColors.Controls.Add(new Label { Text = "Background:", AutoSize = true, Margin = new Padding(20, 8, 5, 0), Font = new Font("Segoe UI", 9) });
            flpColors.Controls.Add(pnlBgColor);

            lblLogoWarning = new Label { Dock = DockStyle.Bottom, Height = 20, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = Color.DarkOrange, Text = "", TextAlign = ContentAlignment.BottomLeft };

            Panel pnlLogoSize = new Panel { Dock = DockStyle.Bottom, Height = 35 };
            numLogoSize = new NumericUpDown { Left = 100, Top = 5, Minimum = 5, Maximum = 30, Value = 15, Width = 60 };
            numLogoSize.ValueChanged += NumLogoSize_ValueChanged;
            pnlLogoSize.Controls.Add(new Label { Text = "Logo Size (%):", Left = 0, Top = 8, AutoSize = true, Font = new Font("Segoe UI", 9) });
            pnlLogoSize.Controls.Add(numLogoSize);

            FlowLayoutPanel flpLogo = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 45 };
            btnLoadLogo = new Button { Text = "Load Logo", Width = 120, Height = 35, Font = new Font("Segoe UI", 9) };
            btnLoadLogo.Click += (s, e) => LoadLogo();
            
            btnClearLogo = new Button { Text = "Remove", Width = 80, Height = 35, Font = new Font("Segoe UI", 9), Enabled = false };
            btnClearLogo.Click += (s, e) => ClearLogo();

            flpLogo.Controls.Add(btnLoadLogo);
            flpLogo.Controls.Add(btnClearLogo);

            grpAppearance.Controls.Add(flpColors);
            grpAppearance.Controls.Add(lblColorWarning);
            grpAppearance.Controls.Add(flpLogo);
            grpAppearance.Controls.Add(pnlLogoSize);
            grpAppearance.Controls.Add(lblLogoWarning);

            btnGenerate = new Button { Text = "GENERATE QR CODE", Dock = DockStyle.Bottom, Height = 60, BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand };
            btnGenerate.Click += (s, e) => GenerateQR();

            pnlLeft.Controls.Add(btnGenerate);
            pnlLeft.Controls.Add(grpAppearance);
            pnlLeft.Controls.Add(spacer1);
            pnlLeft.Controls.Add(grpData);

            Panel pnlPreview = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(30) };
            picQR = new PictureBox { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom, BorderStyle = BorderStyle.FixedSingle, BackColor = Color.White };
            
            btnSave = new Button { Text = "Save as PNG File", Dock = DockStyle.Bottom, Height = 50, BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold), Cursor = Cursors.Hand };
            btnSave.Click += SaveQR;

            pnlPreview.Controls.Add(picQR);
            pnlPreview.Controls.Add(new Panel { Dock = DockStyle.Bottom, Height = 10 });
            pnlPreview.Controls.Add(btnSave);

            this.Controls.Add(pnlPreview);
            this.Controls.Add(pnlLeft);
        }

        // object? sender şeklinde '?' eklendi
        private void NumLogoSize_ValueChanged(object? sender, EventArgs e)
        {
            if (numLogoSize.Value > 20)
            {
                lblLogoWarning.Text = "Warning: Sizes above 20% may make the QR unreadable!";
            }
            else
            {
                lblLogoWarning.Text = "";
            }
        }

        private void CheckContrast()
        {
            double qrLuma = (0.299 * qrColor.R + 0.587 * qrColor.G + 0.114 * qrColor.B) / 255;
            double bgLuma = (0.299 * bgColor.R + 0.587 * bgColor.G + 0.114 * bgColor.B) / 255;

            if (Math.Abs(qrLuma - bgLuma) < 0.4)
                lblColorWarning.Text = "⚠️ Low Contrast! QR may not scan. Use dark QR on light BG.";
            else if (qrLuma > bgLuma)
                lblColorWarning.Text = "⚠️ Inverted Colors! Some older scanners prefer dark QR on light BG.";
            else
                lblColorWarning.Text = ""; 
        }

        // object? sender
        private void CmbDataType_SelectedIndexChanged(object? sender, EventArgs e)
        {
            switch (cmbDataType.SelectedIndex)
            {
                case 0: lblHint.Text = "Enter any website link (https://...) or plain text."; break;
                case 1: lblHint.Text = "Enter WiFi Network Name and Password separated by a comma.\nFormat: NetworkName,Password123"; break;
                case 2: lblHint.Text = "Enter Full Name and Phone Number separated by a comma.\nFormat: John Doe,+1234567890"; break;
                case 3: lblHint.Text = "Enter phone number with country code.\nFormat: +905551234567"; break;
            }
        }

        private Color PickColor(Color current)
        {
            using (ColorDialog cd = new ColorDialog { Color = current })
            {
                return cd.ShowDialog() == DialogResult.OK ? cd.Color : current;
            }
        }

        private void LoadLogo()
        {
            using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Images|*.png;*.jpg;*.jpeg" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    selectedLogo = new Bitmap(ofd.FileName);
                    btnLoadLogo.Text = "Logo Added!";
                    btnLoadLogo.BackColor = Color.LightGreen;
                    btnClearLogo.Enabled = true;
                    // Null göndermek yerine doğru parametreleri gönderiyoruz
                    NumLogoSize_ValueChanged(this, EventArgs.Empty); 
                }
            }
        }

        private void ClearLogo()
        {
            selectedLogo = null;
            btnLoadLogo.Text = "Load Logo";
            btnLoadLogo.BackColor = Color.White;
            btnClearLogo.Enabled = false;
            lblLogoWarning.Text = ""; 
        }

        private void GenerateQR()
        {
            string input = txtInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("Please enter some data to generate a QR code.", "Missing Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string payload = FormatPayload(input);

            try
            {
                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.H))
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    Bitmap qrImage = qrCode.GetGraphic(20, qrColor, bgColor, selectedLogo, (int)numLogoSize.Value);
                    picQR.Image = qrImage;
                    picQR.BackColor = bgColor; 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating QR: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string FormatPayload(string input)
        {
            string[] parts = input.Split(',');

            return cmbDataType.SelectedIndex switch
            {
                1 => parts.Length >= 2 ? $"WIFI:S:{parts[0].Trim()};T:WPA;P:{parts[1].Trim()};;" : $"WIFI:S:{input};T:nopass;;",
                2 => parts.Length >= 2 ? $"BEGIN:VCARD\nVERSION:3.0\nFN:{parts[0].Trim()}\nTEL:{parts[1].Trim()}\nEND:VCARD" : $"BEGIN:VCARD\nVERSION:3.0\nFN:{input}\nEND:VCARD",
                3 => $"whatsapp://send?phone={input.Replace("+", "").Replace(" ", "")}",
                _ => input
            };
        }

        // object? sender
        private void SaveQR(object? sender, EventArgs e)
        {
            if (picQR.Image == null) return;
            using (SaveFileDialog sfd = new SaveFileDialog { Filter = "PNG Image|*.png", FileName = "MyGeneratedQR" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    picQR.Image.Save(sfd.FileName, ImageFormat.Png);
                    MessageBox.Show("QR Code saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}

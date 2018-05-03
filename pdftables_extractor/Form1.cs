using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Http;
using System.IO;

namespace pdftable_extractor
{
    public partial class Form1 : Form
    {
        const string format = "xlsx-single";
        const string apiKey = " "; //put here your api key from pdftables
        const string uploadURL = "https://pdftables.com/api?key=" + apiKey + "&format=" + format;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void startPDFImport(string pdfFilename)
        {
            var task =  PDFToExcel(pdfFilename, @"e:\test.xlsx"); //put here your excel/csv/xml path
            task.Wait();
            MessageBox.Show("Import finished. Exit Code: " + ((int)task.Result).ToString()); //this message box is just to show that process is terminated

            // Do something with your new xlsx, xml or csv file
        }

        static async Task<HttpStatusCode> PDFToExcel(string pdfFilename, string xlsxFilename)
        {
            using (var f = System.IO.File.OpenRead(pdfFilename))
            {
                var client = new HttpClient();
                var mpcontent = new MultipartFormDataContent();
                mpcontent.Add(new StreamContent(f));

                using (var response = await client.PostAsync(uploadURL, mpcontent).ConfigureAwait(false))
                {
                    if ((int)response.StatusCode == 200)
                    {
                        using (
                            Stream contentStream = await response.Content.ReadAsStreamAsync(),
                            stream = File.Create(xlsxFilename))
                        {
                            await contentStream.CopyToAsync(stream);
                        }
                    }
                    return response.StatusCode;
                }
            }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void buttonPDF2table_Click(object sender, EventArgs e)
        {
            OpenFileDialog pdfDialog = new OpenFileDialog();
            pdfDialog.Filter = "PDF files (*.pdf) | *.pdf";
            pdfDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); //choos your initial directory
            pdfDialog.Title = "Import PDF file";

            if (pdfDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                startPDFImport(pdfDialog.FileName);
            }
        }
    }
}

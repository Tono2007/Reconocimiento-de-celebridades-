using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;


namespace ia
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
			openFileDialog1.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
			openFileDialog1.Multiselect = false;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
					
					string dd = openFileDialog1.FileName;
                    pictureBox1.Image = Image.FromFile(dd);
                    label1.Text = dd;
                    const string subscriptionKey = "c2e23e1c5b75443da67b28d494a81b3b";
                    const string uriBase = "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0/models/landmarks/analyze";
                    HttpClient client = new HttpClient();
                    string requestParameters = "model=celebrities";
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                    string uri = uriBase + "?" + requestParameters;
                    HttpResponseMessage response;
                    
                    FileStream fileStream = new FileStream(dd, FileMode.Open, FileAccess.Read);
                    BinaryReader binaryReader = new BinaryReader(fileStream);
                    byte[] byteData = binaryReader.ReadBytes((int)fileStream.Length);
                    using (ByteArrayContent content = new ByteArrayContent(byteData))
                    {
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        response = await client.PostAsync(uri, content);
                        string json = await response.Content.ReadAsStringAsync();
                        string contentString;
                        Console.WriteLine("\nResponse:\n");
                        if (string.IsNullOrEmpty(json))
                            contentString = string.Empty;

                        json = json.Replace(Environment.NewLine, "").Replace("\t", "");

                        string INDENT_STRING = "    ";
                        var indent = 0;
                        var quoted = false;
                        var sb = new StringBuilder();
                        for (var i = 0; i < json.Length; i++)
                        {
                            var ch = json[i];
                            switch (ch)
                            {
                                case '{':
                                case '[':
                                    sb.Append(ch);
                                    if (!quoted)
                                    {
                                        sb.AppendLine();
                                        Enumerable.Range(0, ++indent).ForEach(item => sb.Append(INDENT_STRING));
                                    }
                                    break;
                                case '}':
                                case ']':
                                    if (!quoted)
                                    {
                                        sb.AppendLine();
                                        Enumerable.Range(0, --indent).ForEach(item => sb.Append(INDENT_STRING));
                                    }
                                    sb.Append(ch);
                                    break;
                                case '"':
                                    sb.Append(ch);
                                    bool escaped = false;
                                    var index = i;
                                    while (index > 0 && json[--index] == '\\')
                                        escaped = !escaped;
                                    if (!escaped)
                                        quoted = !quoted;
                                    break;
                                case ',':
                                    sb.Append(ch);
                                    if (!quoted)
                                    {
                                        sb.AppendLine();
                                        Enumerable.Range(0, indent).ForEach(item => sb.Append(INDENT_STRING));
                                    }
                                    break;
                                case ':':
                                    sb.Append(ch);
                                    if (!quoted)
                                        sb.Append(" ");
                                    break;
                                default:
                                    sb.Append(ch);
                                    break;
                            }
                        }
                        contentString = sb.ToString();
                        label2.Text = contentString;
                        bool b = contentString.Contains("name");
                        if (b)
                        {
                            string stringInicial = "\"name\":";
                            int terminaString = contentString.LastIndexOf("\"confidence");
                            String nuevoString = contentString.Substring(0, terminaString);
                            int offset = stringInicial.Length;
                            int iniciaString = nuevoString.LastIndexOf(stringInicial) + offset;
                            int cortar = nuevoString.Length - iniciaString;
                            nuevoString = nuevoString.Substring(iniciaString, cortar);
                            label5.Text = nuevoString;
                        }

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }

                



            }
        }
    }
}

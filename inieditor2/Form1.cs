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

namespace inieditor2
{
    public partial class Form1 : Form
    {
        private Dictionary<string, Dictionary<string, string>> iniData;
        private TableLayoutPanel iniLayoutPanel;

        public Form1()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Create the layout panel for the INI file
            iniLayoutPanel = new TableLayoutPanel();
            iniLayoutPanel.Dock = DockStyle.Fill;
            iniLayoutPanel.AutoScroll = true;

            // Add the layout panel to the main group box
            iniGroupBox.Controls.Add(iniLayoutPanel);
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            // Display the open file dialog and get the selected INI file path
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "INI files (*.ini)|*.ini";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                iniPathTextBox.Text = openFileDialog.FileName;

                try
                {
                    // Read in the INI file
                    string iniFilePath = iniPathTextBox.Text;
                    iniData = new Dictionary<string, Dictionary<string, string>>();

                    string currentSection = null;
                    using (StreamReader reader = new StreamReader(iniFilePath))
                    {
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine().Trim();

                            // Check if the line is a comment or blank line
                            if (line.StartsWith(";") || line.Length == 0)
                            {
                                continue;
                            }

                            // Check if the line is a section
                            if (line.StartsWith("[") && line.EndsWith("]"))
                            {
                                currentSection = line.Substring(1, line.Length - 2);
                                iniData[currentSection] = new Dictionary<string, string>();
                            }
                            else
                            {
                                // Split the line into a key-value pair
                                int equalsIndex = line.IndexOf("=");
                                string key = line.Substring(0, equalsIndex).Trim();
                                string value = line.Substring(equalsIndex + 1).Trim();

                                // Add the key-value pair to the current section
                                iniData[currentSection][key] = value;
                            }
                        }
                    }

                    // Create controls for each section, key, and value
                    foreach (string section in iniData.Keys)
                    {
                        GroupBox sectionGroupBox = new GroupBox();
                        sectionGroupBox.Text = section;
                        sectionGroupBox.AutoSize = true;

                        TableLayoutPanel sectionLayoutPanel = new TableLayoutPanel();
                        sectionLayoutPanel.ColumnCount = 2;
                        sectionLayoutPanel.Dock = DockStyle.Fill;
                        sectionLayoutPanel.AutoScroll= true;
                       // sectionLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                       // sectionLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                        sectionLayoutPanel.AutoSize = true;

                        foreach (string key in iniData[section].Keys)
                        {
                            Label keyLabel = new Label();
                            keyLabel.Text = key;
                            keyLabel.AutoSize = true;
                            keyLabel.TextAlign = ContentAlignment.TopLeft;
                            keyLabel.Anchor = AnchorStyles.Left;

                            TextBox valueTextBox = new TextBox();
                            valueTextBox.Text = iniData[section][key];
                            
                            valueTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                            valueTextBox.TextChanged += (s, ev) =>
                            {
                                iniData[section][key] = valueTextBox.Text;
                            };

                            sectionLayoutPanel.RowCount++;
                            sectionLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                            sectionLayoutPanel.Controls.Add(keyLabel, 0, sectionLayoutPanel.RowCount - 1);
                            sectionLayoutPanel.Controls.Add(valueTextBox, 1, sectionLayoutPanel.RowCount - 1);
                        }

                        iniLayoutPanel.ColumnCount = 2;
                        sectionGroupBox.Controls.Add(sectionLayoutPanel);
                        iniLayoutPanel.Controls.Add(sectionGroupBox);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error reading INI file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Save the INI file
                string iniFilePath = iniPathTextBox.Text;
                using (StreamWriter writer = new StreamWriter(iniFilePath))
                {
                    foreach (string section in iniData.Keys)
                    {
                        writer.WriteLine("[" + section + "]");

                        foreach (string key in iniData[section].Keys)
                        {
                            writer.WriteLine(key + "=" + iniData[section][key]);
                        }

                        writer.WriteLine("");
                    }
                }

                MessageBox.Show("INI file saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving INI file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string aboutText = "Created by ChatGPT. To use this, click Load and open an ini file. If it is not a valid ini with a section and value-key pair, it will not load.";
            string aboutTitle = "About";

            MessageBox.Show(aboutText, aboutTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}


using HashesGenerator.Dto;
using System.Security.Cryptography;

namespace HashesGenerator

{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                txtLocationPath.Text = fbd.SelectedPath;
            }
        }

        private void btnHashGenerator_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtLocationPath.Text)) {
                MessageBox.Show("Please select a folder loction first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; 
            }
                

            DirectoryInfo di = new DirectoryInfo(txtLocationPath.Text);
            FileInfo[] files = di.GetFiles();
            fillGrid(files);
        }


        private void fillGrid(FileInfo[] files) {

            var list = GenerateHashes(files);

            for (int i = 0; i < list.Count; i++) {
                int row = dataGrid.Rows.Add();

                //ID
                dataGrid.Rows[row].Cells[0].Value = (i+1);
                //Name File
                dataGrid.Rows[row].Cells[1].Value = list[i].Name;
                //Generate Hash
                dataGrid.Rows[row].Cells[2].Value = list[i].Hash;
                //Hash to Comapre
                dataGrid.Rows[row].Cells[3].Value = "";
                //isEqual
                dataGrid.Rows[row].Cells[4].Value = "";
            } 
        }

        private List<DtoFiles> GenerateHashes(FileSystemInfo[] files)
        {
            List<DtoFiles> filesList = new List<DtoFiles> ();

            using (SHA256 mySHA256 = SHA256.Create())
            {
                foreach (FileInfo fInfo in files)
                {
                    try
                    {
                        DtoFiles dtoFile = new DtoFiles(); 
                        dtoFile.Name = fInfo.Name;
                        
                        var hash = GetChecksum(HashingAlgoTypes.SHA256, fInfo.FullName);

                        dtoFile.Hash = hash;

                        filesList.Add(dtoFile);
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine($"I/O Exception: {e.Message}");
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        Console.WriteLine($"Access Exception: {e.Message}");
                    }
                }
            }

            return filesList;
        }

        // Display the byte array in a readable format.
        public static string GetChecksum(HashingAlgoTypes hashingAlgoType, string filename)
        {
            using (var hasher = HashAlgorithm.Create(hashingAlgoType.ToString()))
            {
                using (var stream = System.IO.File.OpenRead(filename))
                {
                    var hash = hasher.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "");
                }
            }
        }

        private void dataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGrid.Columns[e.ColumnIndex].Name == "HashToCompare" && !string.IsNullOrEmpty(dataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()))
            {
                dataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().ToUpper().Trim();
                validHashes(e.ColumnIndex, e.RowIndex);
            }
        }

        private void validHashes(int columnIndex, int indexRow) {
            var hashGenerated = dataGrid.Rows[indexRow].Cells[(columnIndex - 1)].Value.ToString();
            var hashToCompare = dataGrid.Rows[indexRow].Cells[columnIndex].Value.ToString();

            if (hashGenerated == hashToCompare) {
                //set value of isEqual Column
                dataGrid.Rows[indexRow].Cells[4].Value = "True";
                dataGrid.Rows[indexRow].Cells[4].Style.BackColor = Color.Green;
            } else {
                dataGrid.Rows[indexRow].Cells[4].Value = "False";
                dataGrid.Rows[indexRow].Cells[4].Style.BackColor = Color.Red;
            }
        }
    }

    public enum HashingAlgoTypes
    {
        MD5,
        SHA1,
        SHA256,
        SHA384,
        SHA512
    }
}
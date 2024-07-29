using Atlassian.Jira;
using OfficeOpenXml;

namespace JiraToolsEndered
{
    public partial class MainForm : Form
    {
        private TextBox txtQuery;
        private Button btnImport;
        private Button btnExport;
        private Label lblResults;
        private System.Threading.Timer timer;

        private string jiraEmail;
        private string jiraPassword;
        private IPagedQueryResult<Issue> issues;

        public MainForm(string email, string password)
        {
            InitializeComponent();

            jiraEmail = email;
            jiraPassword = password;

            Text = "Relatório Jira";

            Label lblQuery = new Label() { Text = "Jira Query", Top = 20, Left = 20 };
            txtQuery = new TextBox() { Top = 45, Left = 20, Width = 400 };

            btnImport = new Button() { Text = "Importar", Top = 80, Left = 20 };
            btnImport.Click += BtnImport_Click;

            btnExport = new Button() { Text = "Exportar para Excel", Top = 80, Left = 120 };
            btnExport.Click += BtnExport_Click;

            lblResults = new Label() { Text = "Resultados:", Top = 120, Left= 20, AutoSize = true };

            Controls.Add(lblQuery);
            Controls.Add(txtQuery);
            Controls.Add(btnImport);
            Controls.Add(lblResults);

            //timer = new Timer();
            //timer.Interval = 7 * 24 * 60 * 60 * 1000;// 1 semana em milissegundos
            //timer.Tick
        }

        private void BtnExport_Click(object? sender, EventArgs e)
        {
            if (issues == null || !issues.Any())
            {
                MessageBox.Show("Não há dados para exportar. Por favor, importe dados primeiro");
                return;
            }

            using SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "Excel Workbook|*.xlsx"
            };
            
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Relatório de Chamados");

                // Adicionar cabeçalhos
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Título";
                worksheet.Cells[1, 3].Value = "Tipo";
                worksheet.Cells[1, 4].Value = "Status";
                worksheet.Cells[1, 5].Value = "Projeto";
                worksheet.Cells[1, 6].Value = "Solver Group";

                // Adicionar dados dos chamados
                int row = 2;
                foreach (var issue in issues)
                {
                    worksheet.Cells[row, 1].Value = issue.Key.Value;
                    worksheet.Cells[row, 2].Value = issue.Summary;
                    worksheet.Cells[row, 3].Value = issue.Type.Name;
                    worksheet.Cells[row, 4].Value = issue.Status.Name;
                    worksheet.Cells[row, 5].Value = issue.Project;
                    worksheet.Cells[row, 6].Value = issue["Solver Group"];
                    row++;
                }

                // Ajustar largura das colunas
                worksheet.Cells.AutoFitColumns();

                // Salvar o arquivo
                var fileInfo = new FileInfo(saveFileDialog.FileName);
                package.SaveAs(fileInfo);
            }

            MessageBox.Show("Relatório exportado com sucesso!");
        }

        private async void BtnImport_Click(object? sender, EventArgs e)
        {
            await FecthAdnDisplayIssues();
        }

        private async Task FecthAdnDisplayIssues()
        {
            var jira = Jira.CreateRestClient("https://", jiraEmail, jiraPassword);
            issues = await jira.Issues.GetIssuesFromJqlAsync(txtQuery.Text);

            var totalIssues = issues.Count();
            int incidents = 0, requests = 0, canceled = 0, solverGroupITEC = 0;

            foreach (var issue in issues)
            {
                if (issue.Project == "Production" || issue.Project == "Homologation")
                {
                    switch (issue.Type.Name)
                    {
                        case "Incident":
                            incidents++;
                            break;
                        case "Request":
                            requests++;
                            break;
                        case "Canceled":
                            canceled++;
                            break;
                    }
                }

                if (issue["Solver Group"] == "ITEC:Support Watts")
                {
                    solverGroupITEC++;
                }
            }

            // Calculando percentuais
            double incidentPercentage = totalIssues > 0 ? (double)incidents / totalIssues * 100 : 0;
            double requestPercentage = totalIssues > 0 ? (double)requests / totalIssues * 100 : 0;
            double canceledPercentage = totalIssues > 0 ? (double)canceled / totalIssues * 100 : 0;

            // Exibindo os resultados
            lblResults.Text = $"Total de Chamados: {totalIssues}\n" +
                              $"Incidentes: {incidentPercentage:F2}%\n" +
                              $"Requisições: {requestPercentage:F2}%\n" +
                              $"Cancelados: {canceledPercentage:F2}%\n" +
                              $"Solver Group 'ITEC:Support Watts': {solverGroupITEC}";
        }
    }
}

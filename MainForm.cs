using System;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Text;
using System.Drawing;
using System.IO;
using System.Collections.Generic;

namespace radar_settinf_tool_project
{
    // main 문 
    public class MainForm : Form
    {
        private RadioButton intergration_btn;
        private RadioButton individual_btn;
        private TextBox InputServerIp;
        private TextBox InputServerPort;
        private Button ConnentServerBtn;
        private RichTextBox resultBox;
        private TextBox uidBox;
        private ComboBox uidComboBox;
       
        public MainForm()
        {
            // main form 기본 세팅
            this.Text = "Main Form";
            this.Size = new System.Drawing.Size(500, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            // 함수들
            InitModeSelection(); // 라디오 버튼
            InitServerControls(); // 서버 ip, port
            InitSetValue();// uid, command, value
            ResultPannel();

            AppendLog("로그인 성공");     // 초록색
            AppendLog("연결 실패");       // 빨간색
            AppendLog("설정 완료");       // 기본 흰색

        }

        //여기서 부터 라디오 버튼, 서버, 포트 connect 버튼 만들기
        private void InitModeSelection()
        {
            Label modeLabel = new Label()
            {
                Text = "세팅 방법",
                Location = new Point(20, 10),
                AutoSize = true,
                Font = new Font("맑은 고딕", 10, FontStyle.Bold)
            };

            individual_btn = new RadioButton()
            {
                Text = "개별 세팅",
                Location = new Point(22, 40),
                //Checked = true
            };

            intergration_btn = new RadioButton()
            {
                Text = "통합 세팅",
                Location = new Point(130, 40),
            };

            //  이벤트 핸들러 연결
            individual_btn.CheckedChanged += ModeChanged;
            intergration_btn.CheckedChanged += ModeChanged;

            this.Controls.Add(modeLabel);
            this.Controls.Add(individual_btn);
            this.Controls.Add(intergration_btn);
        }


        private void InitServerControls()
        {
            int baseX = 20; // UID 기준 위치
            int textBoxWidth = 150;
            int verticalSpacing = 55;
            int currentY = 95;

            // 서버 IP Label & TextBox
            Label server_Ip_Label = new Label()
            {
                Text = "서버 IP",
                Location = new Point(baseX, currentY - 25),
                AutoSize = true,
                Font = new Font("맑은 고딕", 10, FontStyle.Bold)
            };
            InputServerIp = new TextBox()
            {
                Location = new Point(baseX, currentY),
                Width = textBoxWidth,
                Text = "kibana.a2uictai.com"
            };

            currentY += verticalSpacing;

            // 서버 Port Label & TextBox
            Label server_Port_Label = new Label()
            {
                Text = "서버 Port",
                Location = new Point(baseX, currentY - 25),
                AutoSize = true,
                Font = new Font("맑은 고딕", 10, FontStyle.Bold)
            };
            InputServerPort = new TextBox()
            {
                Location = new Point(baseX, currentY),
                Width = textBoxWidth,
                Text = "17090"
            };

            currentY += verticalSpacing;

            // Connect 버튼
            ConnentServerBtn = new Button()
            {
                Text = "Connect to Server",
                Width = textBoxWidth,
                Height = 35,
                Location = new Point(baseX, currentY)
            };

            currentY += 45;

            // 상태 라벨
            Label checked_label = new Label()
            {
                Text = "Socket is not connected.",
                Width = textBoxWidth,
                Height = 35,
                Location = new Point(baseX, currentY),
                BackColor = Color.LightYellow,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false
            };

            // 컨트롤 추가
            this.Controls.Add(server_Ip_Label);
            this.Controls.Add(InputServerIp);
            this.Controls.Add(server_Port_Label);
            this.Controls.Add(InputServerPort);
            this.Controls.Add(ConnentServerBtn);
            this.Controls.Add(checked_label);
        }


        private void InitSetValue()
        {
            int baseX = 230;
            int textBoxWidth = 200;
            int labelOffsetY = -25;
            int verticalSpacing = 60;
            int currentY = 92;

            Label uidLabel = new Label()
            {
                Text = "UID",
                Location = new Point(baseX, currentY + labelOffsetY),
                AutoSize = true,
                Font = new Font("맑은 고딕", 10, FontStyle.Bold)
            };

            // TextBox 생성
            uidBox = new TextBox()
            {
                Location = new Point(baseX, currentY),
                Width = textBoxWidth
            };

            //  ComboBox 생성 (초기에는 숨김)
            uidComboBox = new ComboBox()
            {
                Location = new Point(baseX, currentY),
                Width = textBoxWidth,
                Visible = false,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            //uidComboBox.Items.AddRange(new string[] { "211_1", "211_2", "211_3" }); // 예시 항목

            this.Controls.Add(uidLabel);
            this.Controls.Add(uidBox);
            this.Controls.Add(uidComboBox);

            currentY += verticalSpacing;

            // Command
            Label commandLabel = new Label()
            {
                Text = "Command",
                Location = new Point(baseX, currentY + labelOffsetY),
                AutoSize = true,
                Font = new Font("맑은 고딕", 10, FontStyle.Bold)
            };
            TextBox commandBox = new TextBox()
            {
                Location = new Point(baseX, currentY),
                Width = textBoxWidth
            };

            currentY += verticalSpacing;

            // Value
            Label valueLabel = new Label()
            {
                Text = "Value",
                Location = new Point(baseX, currentY + labelOffsetY),
                AutoSize = true,
                Font = new Font("맑은 고딕", 10, FontStyle.Bold)
            };
            TextBox valueBox = new TextBox()
            {
                Location = new Point(baseX, currentY),
                Width = textBoxWidth
            };

            currentY += verticalSpacing;

            Button sendButton = new Button()
            {
                Text = "Send",
                Location = new Point(baseX, currentY - 20),
                Width = textBoxWidth,
                Height = 30
            };

            this.Controls.Add(commandLabel);
            this.Controls.Add(commandBox);
            this.Controls.Add(valueLabel);
            this.Controls.Add(valueBox);
            this.Controls.Add(sendButton);
        }




        private void ResultPannel()
        {
            // RichTextBox 생성 (필드에 직접 할당)
            resultBox = new RichTextBox();

            // 위치 및 크기 설정
            resultBox.Location = new Point(10, 300);
            resultBox.Size = new Size(460, 450);

            // 속성 설정
            resultBox.ReadOnly = true;
            resultBox.ScrollBars = RichTextBoxScrollBars.Vertical;
            resultBox.WordWrap = true;
            resultBox.BackColor = Color.White;
            resultBox.Font = new Font("Consolas", 10);

            // 폼에 추가
            this.Controls.Add(resultBox);
        }


        // 로그를 출력하는 함수
        private void AppendLog(string message)
        {
            if (resultBox == null) return;

            Color color = Color.Black;
            if (message.Contains("성공"))
            {
                color = Color.LimeGreen;
            }
            else if (message.Contains("실패"))
            {
                color = Color.Red;
            }

            resultBox.SelectionStart = resultBox.TextLength;
            resultBox.SelectionLength = 0;

            resultBox.SelectionColor = color;
            resultBox.AppendText($"{DateTime.Now:HH:mm:ss} - {message}\n");
            resultBox.SelectionColor = resultBox.ForeColor;

            resultBox.ScrollToCaret(); // 자동 스크롤
        }

        // json file read 함수
        private void LoadUidJson()
        {
            try
            {
                string exePath = AppDomain.CurrentDomain.BaseDirectory;
                // bin\Debug\netX.X\ → radar_settinf_tool_project\ 로 이동
                string projectPath = Directory.GetParent(exePath).Parent.Parent.Parent.FullName;

                // 프로젝트 루트 기준으로 json/uid.json 경로 설정
                string jsonFilePath = Path.Combine(projectPath, "json", "uid_list.json");

                if (!File.Exists(jsonFilePath))
                {
                    AppendLog($"UID JSON 파일이 존재하지 않습니다: {jsonFilePath}");
                    return;
                }

                string jsonText = File.ReadAllText(jsonFilePath, Encoding.UTF8);
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonText);

                if (dict == null)
                {
                    AppendLog("UID JSON 파싱 실패: 객체가 null입니다.");
                    return;
                }

                uidComboBox.Items.Clear();

                foreach (var pair in dict)
                {
                    uidComboBox.Items.Add($"{pair.Key} : {pair.Value}");
                }

                AppendLog($"UID JSON 파일에서 {dict.Count}개의 항목을 불러왔습니다.");
            }
            catch (Exception ex)
            {
                AppendLog($"UID JSON 로딩 중 오류: {ex.Message}");
            }
        }



        //------------- ↓↓ 이벤트 함수----------------

        // 개별 선택에 따라서 창 다르게 띄우기
        private void ModeChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null && rb.Checked)
            {
                bool isIntegration = intergration_btn.Checked;

                if (uidBox != null && uidComboBox != null)
                {
                    uidBox.Visible = !isIntegration;
                    uidComboBox.Visible = isIntegration;
                }

                AppendLog(isIntegration ? "통합 세팅 모드로 전환됨" : "개별 세팅 모드로 전환됨");
                if (isIntegration)
                {
                    LoadUidJson(); // json 파일에서 값 읽어오기
                }
                
            }
        }




    }
}
using System;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Text;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;

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
        private TextBox commandBox;
        private TextBox valueBox;
        private ComboBox uidComboBox;
        private List<string> UidStringList = new List<string>(); // uid를 저장할 배열
        private bool check_socket = false;
        private Label checked_label;

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
            ResultPannel(); // log 창

            AppendLog("로그인 성공");     // 초록색
            //AppendLog("연결 실패");       // 빨간색
            //AppendLog("설정 완료");       // 기본 흰색

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
            checked_label = new Label()
            {
                Text = check_socket ? "Socket is connected." : "Socket is not connected.",
                Width = textBoxWidth,
                Height = 35,
                Location = new Point(baseX, currentY),
                BackColor = check_socket ? Color.IndianRed : Color.LightYellow,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false
            };

            // 서버연결 버튼 이벤트 추가
            ConnentServerBtn.Click += ConnectSocketEvent;

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
            commandBox = new TextBox()
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
            valueBox = new TextBox()
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

            sendButton.Click += SendDataToRaderEvent;

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

                    UidStringList.Add(pair.Value.Split('/')[1]);
                }
                //Console.WriteLine("UID 리스트 : " + string.Join(", ", UidStringList));

                AppendLog($"UID JSON 파일에서 {dict.Count}개의 항목을 불러왔습니다.");
            }
            catch (Exception ex)
            {
                AppendLog($"UID JSON 로딩 중 오류: {ex.Message}");
            }
        }

        // 소켓 초기화 함수.
        private Socket InitSocket(string server_ip, int server_port)
        {
            // 소켓 생성 및 연결
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // IPv4, 스트림 소켓 (TCP), TCP 프로토콜
                socket.Connect(server_ip, server_port);
                return socket;
            }
            catch (Exception ex)
            {
                Console.WriteLine("연결 실패: " + ex.Message);
                AppendLog($"연결 실패:  + {ex.Message}");
                return null;
            }
        }

        private Dictionary<string, object> SendControlMessage(Socket clientSocket, string uid, string command, string value)
        {
            var result = new Dictionary<string, object>();

            try
            {
                if (clientSocket == null || !clientSocket.Connected)
                {
                    result["status"] = "error";
                    result["error"] = "Socket is not initialized or not connected.";
                    return result;
                }

                // Control 메시지 생성
                var message = new Dictionary<string, string>
                {
                    { "type", "control" },
                    { "uid", uid }, // uid_textbox.text
                    { "command", command }, // command_textbox.text
                    { "value", value } // value_textbox.text
                };

                // JSON 직렬화 후 전송
                string jsonMessage = JsonConvert.SerializeObject(message);
                byte[] dataToSend = Encoding.UTF8.GetBytes(jsonMessage);
                clientSocket.Send(dataToSend);

                // 응답 수신
                byte[] buffer = new byte[8192];
                int received = clientSocket.Receive(buffer);
                string responseJson = Encoding.UTF8.GetString(buffer, 0, received);

                // 응답 JSON 파싱
                var responseDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJson);
                return responseDict;
            }
            catch (Exception ex)
            {
                result["status"] = "error";
                result["error"] = ex.Message;
                return result;
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

        // 소켓 연결 이벤트 함수
        private void ConnectSocketEvent(object sender, EventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                Console.WriteLine($"ip : {InputServerIp.Text}, port : {int.Parse(InputServerPort.Text)}");
                Socket sock = InitSocket(InputServerIp.Text, int.Parse(InputServerPort.Text));
                if (sock != null)
                {
                    AppendLog($"[소켓 연결 성공] 서버 IP:{InputServerIp.Text}, 포트:{InputServerPort.Text}");
                    check_socket = true;
                    checked_label.Text = "Socket is connected.";
                    checked_label.BackColor = Color.Green;
                }
                else
                {
                    check_socket = false;
                    checked_label.Text = "Socket is not connected.";
                    checked_label.BackColor = Color.LightYellow;
                }
            }
            catch (InvalidCastException)
            {
                AppendLog("[소켓 연결 실패] sender가 Button 타입이 아닙니다.");
            }
            catch (Exception ex)
            {
                AppendLog($"[소켓 연결 실패] 예외 발생: {ex.Message}");
            }
        }

        // 레이더에 세팅 값 보내는 이벤트
        private async void SendDataToRaderEvent(object sender, EventArgs e)
        {
            try
            {
                Socket sock = InitSocket(InputServerIp.Text, int.Parse(InputServerPort.Text));
                if (sock == null) return;

                // 타임아웃 설정 (3초)
                sock.ReceiveTimeout = 3000;

                // 비동기 처리
                var responseDict = await Task.Run(() =>
                    SendControlMessage(sock,
                        uidBox.Text.Replace(" ", ""),
                        commandBox.Text.Replace(" ", ""),
                        valueBox.Text.Replace(" ", ""))
                );

                string status = responseDict.ContainsKey("status") ? responseDict["status"]?.ToString() : null;
                string type = responseDict.ContainsKey("type") ? responseDict["type"]?.ToString() : null;

                if (status == "success" || type == "cli")
                {
                    MessageBox.Show("Command sent successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to send command.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                string responseStr = JsonConvert.SerializeObject(responseDict, Formatting.Indented);
                AppendLog($"서버 응답: {responseStr}");
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
            {
                AppendLog("응답 시간 초과 (서버가 재시작 중일 수 있음).");
                MessageBox.Show("서버 응답이 없습니다. 서버가 재시작 중일 수 있습니다.", "Timeout", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
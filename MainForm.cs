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
using System.Reflection;


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
        public static List<(string Uid, int Port)> UidPortTupleList = new List<(string, int)>();//튜플에 uid, port 저장
        private bool check_socket = false;
        private Label checked_label;
        private Label server_Ip_Label;
        private Label server_Port_Label;

        private readonly List<string> CommandList = new List<string> { "sta", "host2bps", "reset", "wsp", "lsp", "save", "uid", "minthr", "maxthr", "floorthr", "mins", "drops", "dropx",
                                                                    "dropxm", "dropthr", "movt", "meast", "dropt", "alertt", "cleart", "left", "right","bedh", "radarh", "angle", "mods"};

        public MainForm()
        {
            // main form 기본 세팅
            this.Text = "원격 세팅";
            this.Size = new System.Drawing.Size(900, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            // 함수들
            InitModeSelection(); // 라디오 버튼
            InitServerControls(); // 서버 ip, port
            InitSetValue();// uid, command, value
            ResultPannel(); // log 창

            AppendLog("로그인 성공");     // 초록색


        }

        //여기서 부터 라디오 버튼, 서버, 포트 connect 버튼 만들기
        private void InitModeSelection()
        {

            Label setting = new Label()
            {
                Text = "세팅 방법",
                Location = new Point(22, 15),
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


            this.Controls.Add(setting);
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
            server_Ip_Label = new Label()
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
            server_Port_Label = new Label()
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
            resultBox.Size = new Size(870, 450);

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
            else if (message.Contains("실패") || message.Contains("Error")) //message.Contains("not")
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
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = Path.Combine(AppContext.BaseDirectory, "json");
                    openFileDialog.Filter = "JSON 파일 (*.json)|*.json";
                    openFileDialog.Title = "UID JSON 파일 선택";

                    if (openFileDialog.ShowDialog() != DialogResult.OK)
                    {
                        AppendLog("파일 선택이 취소되었습니다.");
                        return;
                    }

                    string jsonFilePath = openFileDialog.FileName;

                    string jsonText = File.ReadAllText(jsonFilePath, Encoding.UTF8);
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(jsonText);

                    if (dict == null)
                    {
                        AppendLog("UID JSON 파싱 실패: 객체가 null입니다.");
                        return;
                    }

                    uidComboBox.Items.Clear();
                    UidStringList.Clear();
                    UidPortTupleList.Clear();

                    foreach (var pair in dict)
                    {
                        var valueDict = pair.Value;

                        if (valueDict.TryGetValue("uid", out object uidObj) &&
                            valueDict.TryGetValue("port", out object portObj))
                        {
                            string uidFull = uidObj.ToString();
                            int port = Convert.ToInt32(portObj);

                            uidComboBox.Items.Add($"{pair.Key} : {uidFull}");

                            if (uidFull.StartsWith("21b7/"))
                            {
                                string uidStripped = uidFull.Substring(5);
                                UidStringList.Add(uidStripped);
                                UidPortTupleList.Add((uidStripped, port));
                            }
                            else
                            {
                                UidStringList.Add(uidFull);
                                UidPortTupleList.Add((uidFull, port));
                            }
                        }
                    }

                    if (uidComboBox.Items.Count > 0)
                        uidComboBox.SelectedIndex = 0;

                    AppendLog($"선택한 파일에서 {dict.Count}개의 UID 항목을 불러왔습니다.");
                }
            }
            catch (Exception ex)
            {
                AppendLog($"UID JSON 로딩 중 오류: {ex.Message}");
            }
        }


        // 모든 uid를 세팅 하는 함수

        private async Task SendAllUidCommandsAsync(string serverIp, string command, string value)
        {
            List<string> resultLines = new List<string>(); // 결과 저장용 리스트
            int success_count = 0, fail_count = 0;

            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string logFileName = $"{DateTime.Now:yyyy_MM_dd}_fail_setting.log";
            string logFilePath = Path.Combine(exePath, "log", logFileName);
            string logDirPath = Path.Combine(exePath, "log");

            List<(string serverip, string uid, int port, string command, string value)> failSendList = new List<(string, string, int, string, string)>();// 전송 실패한 정보 리스트



            foreach (var (uid, port) in UidPortTupleList)
            {
                Socket sock = null;

                if (sock != null && sock.Connected) // 연결되어있는 소켓 초기화
                {
                    try
                    {
                        sock.Shutdown(SocketShutdown.Both);
                    }
                    catch (Exception ex)
                    {
                        AppendLog($"기존 소켓 종료 중 오류: {ex.Message}");
                    }
                    finally
                    {
                        sock.Close();
                        sock = null;
                    }
                }
                AppendLog($"UID: {uid}, Port: {port} 서버에 연결 시도 중...");

                try
                {
                    // 소켓 연결을 Task로 감싸고 10초 타임아웃 설정(10초 안에 연결 못하면 다음 소켓 연결로 넘어감)
                    var connectTask = Task.Run(() => InitSocket(serverIp, port));
                    var timeoutTask = Task.Delay(TimeSpan.FromSeconds(10));

                    var completedTask = await Task.WhenAny(connectTask, timeoutTask);



                    if (completedTask == timeoutTask)
                    {
                        AppendLog($"UID {uid} 포트 {port}: 10초 내에 소켓 연결 실패 (타임아웃)");
                        continue;
                    }

                    sock = connectTask.Result;

                    if (sock == null)
                    {
                        AppendLog($"UID {uid} 포트 {port}: 소켓 연결 실패");
                        continue;
                    }

                    var response = await Task.Run(() => SendControlMessage(sock, uid, command, value));

                    string responseStr = JsonConvert.SerializeObject(response, Formatting.None);// Formatting.Indented
                    string responseStrCompact = JsonConvert.SerializeObject(response, Formatting.None);


                    string status = response.ContainsKey("status") ? response["status"]?.ToString() : null;
                    string type = response.ContainsKey("type") ? response["type"]?.ToString() : null;

                    if (status == "success" || type == "cli" )
                    {

                        AppendLog($"UID {uid} 포트 {port}에 명령 전송 성공");
                        AppendLog($"서버 응답: {responseStr}");
                        resultLines.Add(responseStrCompact);
                        success_count++;
                    }
                    else
                    {
                        AppendLog($"UID {uid} 포트 {port}에 명령 전송 실패: {responseStr}");
                        fail_count++;
                        string logLine = $" [{DateTime.Now:HH:mm:ss}] [Fail Setting] UID: {uid}, Port: {port}, Command: {command}, Value: {value}";
                        if (!Directory.Exists(logDirPath))
                        {
                            Directory.CreateDirectory(logDirPath);
                        }

                        File.AppendAllText(logFilePath, logLine + Environment.NewLine);
                        string serverip = InputServerIp.Text;
                        failSendList.Add((serverip, uid, port, command, value)); // 리스트에 실패한 데이터 저장.


                    }
                }
                catch (Exception ex)
                {
                    AppendLog($"UID {uid} 포트 {port} 처리 중 예외 발생: {ex.Message}");
                }
                finally
                {
                    if (sock != null && sock.Connected)
                    {
                        try
                        {
                            sock.Shutdown(SocketShutdown.Both);
                            sock.Close();
                        }
                        catch { }
                    }
                }
            }
            // 세팅 값 로그 창에 출력
            if (fail_count > 0)
            {
                SettingResultForm resultForm = new SettingResultForm(success_count, fail_count,failSendList);
                resultForm.ShowDialog(); // 모달 방식으로 띄우기
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

                // 5초 수신 타임아웃 설정
                clientSocket.ReceiveTimeout = 5000;

                // Control 메시지 생성
                var message = new Dictionary<string, string>
                {
                    { "type", "control" },
                    { "uid", uid },
                    { "command", command },
                    { "value", value }
                };

                // JSON 직렬화 후 전송
                string jsonMessage = JsonConvert.SerializeObject(message);
                byte[] dataToSend = Encoding.UTF8.GetBytes(jsonMessage);
                clientSocket.Send(dataToSend);

                // save 명령은 서버 재시작으로 응답이 없으므로 성공으로 간주
                if (command == "save")
                {
                    Console.WriteLine("save 명령어 입력 - 응답 없이 성공 처리");
                    result["status"] = "success";
                    result["note"] = "Response skipped due to 'save' command.";
                    return result;
                }

                // 응답 수신 (최대 5초 대기)
                byte[] buffer = new byte[8192];
                int received = clientSocket.Receive(buffer);
                string responseJson = Encoding.UTF8.GetString(buffer, 0, received);

                // 응답 JSON 파싱
                var responseDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJson);
                return responseDict;
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
            {
                result["status"] = "error";
                result["error"] = "Receive timeout (no response from server).";
                return result;
            }
            catch (Exception ex)
            {
                result["status"] = "error";
                result["error"] = ex.Message;
                return result;
            }
        }


        // 개별 세팅으로 서버에 데이터 보내는 함수
        private async Task SendDataToServver()
        {
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string logFileName = $"{DateTime.Now:yyyy_MM_dd}_fail_setting.log";
            string logFilePath = Path.Combine(exePath, "log", logFileName);
            string logDirPath = Path.Combine(exePath, "log");
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

                string responseStr = JsonConvert.SerializeObject(responseDict, Formatting.None);


                if (status == "success" || type == "cli")
                {
                    AppendLog($"UID {uidBox.Text} 포트 {InputServerPort.Text}에 명령 전송 성공");
                    AppendLog($"서버 응답: {responseStr}");
                }
                else
                {
                    AppendLog($"UID {uidBox.Text} 포트 {InputServerPort.Text}에 명령 전송 실패");
                    AppendLog($"서버 응답: {responseStr}");


                    string logLine = $" [{DateTime.Now:HH:mm:ss}] [Fail Setting] UID: {uidBox.Text}, Port: {InputServerPort.Text}, Command: {commandBox.Text}, Value: {valueBox.Text}";
                    if (!Directory.Exists(logDirPath))
                    {
                        Directory.CreateDirectory(logDirPath);
                    }

                    File.AppendAllText(logFilePath, logLine + Environment.NewLine);
                }


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
                    LoadUidJson(); // 통합 모드일 때 JSON 불러오기
                }

                // InitServerControls() 안에 있는 컨트롤들 표시/숨기기
                if (InputServerIp != null) InputServerIp.Visible = !isIntegration;
                if (InputServerPort != null) InputServerPort.Visible = !isIntegration;
                if (ConnentServerBtn != null) ConnentServerBtn.Visible = !isIntegration;
                if (checked_label != null) checked_label.Visible = !isIntegration;
                if (server_Ip_Label != null) server_Ip_Label.Visible = !isIntegration;
                if (server_Port_Label != null) server_Port_Label.Visible = !isIntegration;
            }
        }

        // 통합 세팅 후 세팅 값 확인 이벤트
        private void CheckSettingValue(object sender, EventArgs e)
        {

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
            if (individual_btn.Checked)// 개별 세팅 버튼 선택 되었을 때
            {
                string uid = uidBox.Text;
                string command = commandBox.Text.Trim().ToLower();
                if (string.IsNullOrWhiteSpace(uid) || string.IsNullOrWhiteSpace(command))
                {
                    AppendLog("[Error] uid 와 command 명령어는 필수 입력 조건입니다.");
                }
                else if (!CommandList.Contains(command))
                {
                    AppendLog($"[Error] '{command}' 는 지원하지 않는 명령어입니다.");
                }
                else
                {
                    await SendDataToServver();
                }

            }
            else if (intergration_btn.Checked) // 통합세팅 버튼 선택 되었을 때
            {
                string command2 = commandBox.Text.Trim().ToLower();
                if (string.IsNullOrWhiteSpace(command2))
                {
                    AppendLog("[Error] command 명령어는 필수 입력 조건입니다.");
                }
                else if (!CommandList.Contains(command2))
                {
                    AppendLog($"[Error] '{command2}' 는 지원하지 않는 명령어입니다.");
                }
                else
                {
                    // 서버로 데이터 보내기
                    string serverIp = InputServerIp.Text;
                    string command = commandBox.Text;
                    string value = valueBox.Text;
                    await SendAllUidCommandsAsync(serverIp, command, value);
                }

            }

        }

    }
}